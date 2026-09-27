/// A compact tree view of the Fable AST, for writing plugin patterns against.
///
/// Every node is labelled with the field it sits in, named as the union case declares it (`callee`,
/// `info.Args[0]`, `body`), so a line reads as the pattern that reaches it:
///
///     Call : Partas.Solid.HtmlElement @4:5
///     │  · info.MemberRef = MemberRef(Partas.Solid.Tags.div, "Partas.Solid.Tags.div.Run", instance) [method]
///     ├─ callee: Import "div" from "..." : any @none
///     └─ info.Args[0]: IdentExpr PARTAS_BUILDER : Partas.Solid.Tags.div [gen]
///
/// `Detail.Plugin`, the default, shows the fields Partas.Solid.FablePlugin matches on: names, selectors, paths,
/// member refs, types, the `this`/compiler-generated flags, tags, and whether a range is None. `Detail.Full` adds
/// the rest (signature types, generic arguments everywhere, every attribute); `Detail.Shape` keeps only the cases.
///
/// Ranges are printed as editor positions, 1-based line and column; Fable's own columns are 0-based.
module AstPrinter

open System.Text
open Fable.AST
open Fable.AST.Fable

type Detail =
    | Shape
    | Plugin
    | Full

type Options =
    { Detail: Detail
      /// Levels printed below the root; deeper subtrees are summarised as a count.
      MaxDepth: int option
      /// String constants and macros longer than this are cut.
      MaxString: int
      /// Entity attributes printed at Detail.Plugin (all are printed at Detail.Full).
      Attributes: Set<string>
      /// Resolves entities, for attributes, union case and record field names. Workbench supplies Fable's.
      Entity: EntityRef -> Entity option
      /// Resolves member refs, for their kind (getter, setter, constructor) and parameter names.
      Member: MemberRef -> MemberFunctionOrValue option }

    static member Default =
        { Detail = Plugin
          MaxDepth = None
          MaxString = 60
          Attributes =
            set
                [ "Fable.Core.JS.PojoAttribute"
                  "Partas.Solid.PartasImportAttribute"
                  "Partas.Solid.PartasProxyImportAttribute" ]
          Entity = fun _ -> None
          Member = fun _ -> None }

/// One printed node: `Label` is the field it sits in, `Head` its case and inline fields, `Props` the fields that
/// get a line of their own.
type Node =
    { Label: string
      Head: string
      Props: (string * string) list
      Children: Node list }

let private node label head = { Label = label; Head = head; Props = []; Children = [] }

let private tryResolve f x = try f x with _ -> None

let private str (opts: Options) (s: string) =
    let s = if s.Length > opts.MaxString then s.Substring(0, opts.MaxString) + "…" else s
    "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r") + "\""

let private join (sep: string) (xs: string seq) = System.String.Join(sep, xs)

let private range (opts: Options) (r: SourceLocation option) =
    match opts.Detail, r with
    | Shape, _ -> ""
    | _, None -> "@none"
    | Plugin, Some r -> $"@{r.start.line}:{r.start.column + 1}"
    | Full, Some r -> $"@{r.start.line}:{r.start.column + 1}-{r.``end``.line}:{r.``end``.column + 1}"

let private attributeName (fullName: string) =
    let name = fullName.Substring(fullName.LastIndexOf('.') + 1)
    if name.EndsWith "Attribute" then name.Substring(0, name.Length - "Attribute".Length) else name

/// `{Pojo}` after an entity with an attribute worth seeing at this detail.
let private entityAttributes (opts: Options) (ref: EntityRef) =
    match opts.Detail with
    | Shape -> ""
    | _ ->
        match tryResolve opts.Entity ref with
        | None -> ""
        | Some ent ->
            ent.Attributes
            |> Seq.map _.Entity.FullName
            |> Seq.filter (fun n -> opts.Detail = Full || opts.Attributes.Contains n)
            |> Seq.map attributeName
            |> List.ofSeq
            |> function
                | [] -> ""
                | names -> " {" + join ", " names + "}"

let rec typ (opts: Options) (t: Type) : string =
    let typ = typ opts
    let args (ts: Type list) = if ts.IsEmpty then "" else "<" + (ts |> List.map typ |> join ", ") + ">"
    let strct isStruct = if isStruct then "struct " else ""
    match t with
    | Measure name -> $"measure {name}"
    | MetaType -> "Type"
    | Any -> "any"
    | Unit -> "unit"
    | Boolean -> "bool"
    | Char -> "char"
    | String -> "string"
    | Regex -> "regex"
    | Number(kind, info) ->
        let info =
            match info with
            | NumberInfo.Empty -> ""
            | NumberInfo.IsMeasure m -> $"<{m}>"
            | NumberInfo.IsEnum e -> $" enum {e.FullName}"
        $"%A{kind}{info}"
    | Option(t, isStruct) -> $"{strct isStruct}{typ t} option"
    | Tuple(ts, isStruct) -> strct isStruct + "(" + (ts |> List.map typ |> join " * ") + ")"
    | Array(t, kind) ->
        match kind with
        | MutableArray -> $"{typ t}[]"
        | ResizeArray -> $"ResizeArray<{typ t}>"
        | ImmutableArray -> $"ImmutableArray<{typ t}>"
    | List t -> $"{typ t} list"
    | LambdaType(a, r) -> $"({typ a} -> {typ r})"
    | DelegateType(ts, r) -> "Delegate(" + (ts |> List.map typ |> join ", ") + $" -> {typ r})"
    | GenericParam(name, isMeasure, constraints) ->
        let measure = if isMeasure then " measure" else ""
        let constraints =
            if opts.Detail = Full && not constraints.IsEmpty then
                " when " + (constraints |> List.map (sprintf "%A") |> join ", ")
            else
                ""
        $"'{name}{measure}{constraints}"
    | DeclaredType(ref, genArgs) -> ref.FullName + args genArgs + entityAttributes opts ref
    | AnonymousRecordType(names, genArgs, isStruct) ->
        let fields = Seq.zip names genArgs |> Seq.map (fun (n, t) -> $"{n}: {typ t}") |> join "; "
        $"{strct isStruct}{{| {fields} |}}"
    | Nullable(t, isStruct) -> $"{strct isStruct}{typ t} | null"

let private typed (opts: Options) (t: Type) = if opts.Detail = Shape then "" else " : " + typ opts t

let ident (opts: Options) (i: Ident) =
    let flags =
        [ if i.IsThisArgument then "this"
          if i.IsCompilerGenerated then "gen"
          if i.IsMutable then "mutable"
          if i.IsInlineIfLambda then "inlineIfLambda" ]
    let flags = if flags.IsEmpty || opts.Detail = Shape then "" else " [" + join ", " flags + "]"
    let r = if opts.Detail = Full then " " + range opts i.Range else ""
    i.Name + typed opts i.Type + flags + r

let private idents (opts: Options) (is: Ident list) = "(" + (is |> List.map (ident opts) |> join ", ") + ")"

/// What the member is (`[getter]`, `[ctor (a, b)]`), from the resolver.
let private memberKind (opts: Options) (m: MemberRef) =
    match tryResolve opts.Member m with
    | None -> ""
    | Some m ->
        let kind =
            if m.IsConstructor then "ctor"
            elif m.IsGetter then "getter"
            elif m.IsSetter then "setter"
            elif m.IsProperty then "property"
            elif m.IsValue then "value"
            else "method"
        let flags =
            [ if m.IsExtension then "extension"
              if m.IsInline then "inline" ]
        let parameters =
            if m.IsConstructor || opts.Detail = Full then
                m.CurriedParameterGroups
                |> List.map (fun g -> "(" + (g |> List.map (fun p -> defaultArg p.Name "_") |> join ", ") + ")")
                |> join " "
            else
                ""
        let attributes =
            if opts.Detail = Full then
                m.Attributes |> Seq.map (fun a -> attributeName a.Entity.FullName) |> join ", "
            else
                ""
        let parts = kind :: flags @ [ parameters; if attributes = "" then "" else "{" + attributes + "}" ]
        " [" + (parts |> List.filter ((<>) "") |> join " ") + "]"

let memberRef (opts: Options) (m: MemberRef) =
    let described =
        match m with
        | MemberRef(entity, info) ->
            let fields =
                [ entity.FullName + entityAttributes opts entity
                  str { opts with MaxString = System.Int32.MaxValue } info.CompiledName
                  if info.IsInstance then "instance"
                  if opts.Detail = Full then
                      match info.NonCurriedArgTypes with
                      | Some ts -> "args=[" + (ts |> List.map (typ opts) |> join ", ") + "]"
                      | None -> ()
                      if not info.AttributeFullNames.IsEmpty then
                          "attrs=[" + (info.AttributeFullNames |> List.map attributeName |> join ", ") + "]" ]
            "MemberRef(" + join ", " fields + ")"
        | GeneratedMemberRef g ->
            let case, info =
                match g with
                | GeneratedFunction i -> "GeneratedFunction", i
                | GeneratedValue i -> "GeneratedValue", i
                | GeneratedGetter i -> "GeneratedGetter", i
                | GeneratedSetter i -> "GeneratedSetter", i
            let entity = info.DeclaringEntity |> Option.map (fun e -> ", " + e.FullName) |> Option.defaultValue ""
            $"GeneratedMemberRef {case}({info.Name}{entity})"
    described + memberKind opts m

let private importKind (opts: Options) (k: ImportKind) =
    match k with
    | UserImport isInline -> $"UserImport {isInline}"
    | LibraryImport info ->
        $"LibraryImport(IsInstanceMember={info.IsInstanceMember}, IsModuleMember={info.IsModuleMember})"
    | MemberImport m -> $"MemberImport {memberRef opts m}"
    | ClassImport e -> $"ClassImport {e.FullName}{entityAttributes opts e}"

let private tags (ts: string list) = "[" + join "; " ts + "]"

/// Field names of a record, or of one case of a union, for labelling its values.
let private recordFields (opts: Options) (ref: EntityRef) =
    tryResolve opts.Entity ref |> Option.map (fun e -> e.FSharpFields |> List.map _.Name)

let private unionCase (opts: Options) (ref: EntityRef) (tag: int) =
    tryResolve opts.Entity ref |> Option.bind (fun e -> List.tryItem tag e.UnionCases)

let private indexed (label: string) (names: string list option) (i: int) =
    match names |> Option.bind (List.tryItem i) with
    | Some name -> $"{label}[{i}] ({name})"
    | None -> $"{label}[{i}]"

let rec expr (opts: Options) (label: string) (e: Expr) : Node =
    let expr = expr opts
    let typed = typed opts
    let range = range opts
    let plugin = opts.Detail >= Plugin
    let full = opts.Detail = Full
    let many label (es: Expr list) = es |> List.mapi (fun i -> expr $"{label}[{i}]")
    let opt label (e: Expr option) = e |> Option.map (expr label) |> Option.toList
    let head (parts: string list) = parts |> List.filter ((<>) "") |> join " "
    let callInfo (prefix: string) (info: CallInfo) =
        let props =
            [ if plugin then
                  match info.MemberRef with
                  | Some m -> ($"{prefix}.MemberRef", memberRef opts m)
                  | None -> ($"{prefix}.MemberRef", "None")
                  if not info.Tags.IsEmpty then ($"{prefix}.Tags", tags info.Tags)
                  if not info.GenericArgs.IsEmpty then
                      ($"{prefix}.GenericArgs", info.GenericArgs |> List.map (typ opts) |> join ", ")
              if full && not info.SignatureArgTypes.IsEmpty then
                  ($"{prefix}.SignatureArgTypes", info.SignatureArgTypes |> List.map (typ opts) |> join ", ") ]
        let children = opt $"{prefix}.ThisArg" info.ThisArg @ many $"{prefix}.Args" info.Args
        props, children
    let withRange r (n: Node) = { n with Head = head [ n.Head; range r ] }
    match e with
    | IdentExpr i -> node label $"IdentExpr {ident opts i}"
    | Value(kind, r) ->
        let value (case: string) = node label $"Value {case}"
        (match kind with
        | ThisValue t -> value $"ThisValue{typed t}"
        | BaseValue(i, t) ->
            let bound = i |> Option.map (fun i -> $" ({ident opts i})") |> Option.defaultValue ""
            value $"BaseValue{bound}{typed t}"
        | TypeInfo(t, ts) -> value $"TypeInfo {typ opts t} {tags ts}"
        | Null t -> value $"Null{typed t}"
        | UnitConstant -> value "UnitConstant"
        | BoolConstant b -> value $"BoolConstant {b}"
        | CharConstant c -> value $"CharConstant '{c}'"
        | StringConstant s -> value $"StringConstant {str opts s}"
        | NumberConstant(n, info) ->
            let info =
                match info with
                | NumberInfo.Empty -> ""
                | NumberInfo.IsMeasure m -> $" <{m}>"
                | NumberInfo.IsEnum ent -> $" enum {ent.FullName}"
            value $"NumberConstant %A{n}{info}"
        | RegexConstant(source, flags) -> value $"""RegexConstant {str opts source} %A{flags}"""
        | StringTemplate(tag, parts, values) ->
            { value "StringTemplate" with
                Props = [ ("parts", parts |> List.map (str opts) |> join ", ") ]
                Children = opt "tag" tag @ many "values" values }
        | NewOption(v, t, isStruct) ->
            let case = if v.IsSome then "Some" else "None"
            { value $"""NewOption {case}{typed t}{if isStruct then " struct" else ""}""" with
                Children = opt "value" v }
        | NewArray(newKind, t, arrayKind) ->
            let case, children =
                match newKind with
                | ArrayValues values -> "ArrayValues", many "newKind.values" values
                | ArrayAlloc size -> "ArrayAlloc", [ expr "newKind.size" size ]
                | ArrayFrom from -> "ArrayFrom", [ expr "newKind.expr" from ]
            { value $"NewArray {case} %A{arrayKind}{typed t}" with Children = children }
        | NewList(headAndTail, t) ->
            let children =
                match headAndTail with
                | Some(h, tl) -> [ expr "head" h; expr "tail" tl ]
                | None -> []
            let case = if headAndTail.IsSome then "cons" else "empty"
            { value $"NewList {case}{typed t}" with Children = children }
        | NewTuple(values, isStruct) ->
            { value $"""NewTuple{if isStruct then " struct" else ""}""" with
                Children = many "values" values }
        | NewRecord(values, ref, genArgs) ->
            let names = recordFields opts ref
            { value $"NewRecord {typ opts (DeclaredType(ref, genArgs))}" with
                Children = values |> List.mapi (fun i -> expr (indexed "values" names i)) }
        | NewAnonymousRecord(values, names, genArgs, isStruct) ->
            let generics = if full then " " + (genArgs |> List.map (typ opts) |> join ", ") else ""
            { value $"""NewAnonymousRecord{if isStruct then " struct" else ""}{generics}""" with
                Children = values |> List.mapi (fun i -> expr (indexed "values" (Some(List.ofArray names)) i)) }
        | NewUnion(values, tag, ref, genArgs) ->
            let case = unionCase opts ref tag
            let caseName = case |> Option.map (fun c -> $" ({c.Name})") |> Option.defaultValue ""
            let fields = case |> Option.map (fun c -> c.UnionCaseFields |> List.map _.Name)
            { value $"NewUnion tag={tag}{caseName} {typ opts (DeclaredType(ref, genArgs))}" with
                Children = values |> List.mapi (fun i -> expr (indexed "values" fields i)) })
        |> withRange r
    | Lambda(arg, body, name) ->
        let name = name |> Option.map (fun n -> $"name={n}") |> Option.defaultValue ""
        { node label (head [ "Lambda"; ident opts arg; name ]) with Children = [ expr "body" body ] }
    | Delegate(args, body, name, ts) ->
        let name = name |> Option.map (fun n -> $"name={n}") |> Option.defaultValue ""
        { node label (head [ "Delegate"; idents opts args; name ]) with
            Props = [ if plugin && not ts.IsEmpty then ("tags", tags ts) ]
            Children = [ expr "body" body ] }
    | ObjectExpr(members, t, baseCall) ->
        let memberNode i (m: ObjectExprMember) =
            { node $"members[{i}]" (head [ "ObjectExprMember"; m.Name; idents opts m.Args ]) with
                Props =
                    [ if plugin then ("MemberRef", memberRef opts m.MemberRef)
                      if full then ("IsMangled", string m.IsMangled) ]
                Children = [ expr "Body" m.Body ] }
        { node label $"ObjectExpr{typed t}" with
            Children = List.mapi memberNode members @ opt "baseCall" baseCall }
    | TypeCast(inner, t) -> { node label $"TypeCast{typed t}" with Children = [ expr "expr" inner ] }
    | Test(inner, kind, r) ->
        let kind =
            match kind with
            | TypeTest t -> $"TypeTest {typ opts t}"
            | OptionTest isSome -> $"OptionTest isSome={isSome}"
            | ListTest isCons -> $"ListTest isCons={isCons}"
            | UnionCaseTest tag -> $"UnionCaseTest tag={tag}"
        { node label (head [ "Test"; kind; range r ]) with Children = [ expr "expr" inner ] }
    | Call(callee, info, t, r) ->
        let props, children = callInfo "info" info
        { node label (head [ "Call" + typed t; range r ]) with
            Props = props
            Children = expr "callee" callee :: children }
    | CurriedApply(applied, args, t, r) ->
        { node label (head [ "CurriedApply" + typed t; range r ]) with
            Children = expr "applied" applied :: many "args" args }
    | Operation(kind, ts, t, r) ->
        let case, children =
            match kind with
            | Unary(op, operand) -> $"Unary %A{op}", [ expr "operand" operand ]
            | Binary(op, left, right) -> $"Binary %A{op}", [ expr "left" left; expr "right" right ]
            | Logical(op, left, right) -> $"Logical %A{op}", [ expr "left" left; expr "right" right ]
        { node label (head [ "Operation"; case + typed t; range r ]) with
            Props = [ if plugin && not ts.IsEmpty then ("tags", tags ts) ]
            Children = children }
    | Import(info, t, r) ->
        // Never cut: the plugin matches selectors and paths by prefix and suffix.
        let whole = str { opts with MaxString = System.Int32.MaxValue }
        { node label (head [ $"Import {whole info.Selector} from {whole info.Path}" + typed t; range r ]) with
            Props = [ if plugin then ("info.Kind", importKind opts info.Kind) ] }
    | Emit(info, t, r) ->
        let props, children = callInfo "info.CallInfo" info.CallInfo
        { node label (head [ $"Emit {str opts info.Macro}" + typed t; range r ]) with
            Props = [ if plugin && info.IsStatement then ("info.IsStatement", "true") ] @ props
            Children = children }
    | DecisionTree(inner, targets) ->
        let target i (bound: Ident list, body) =
            { node $"targets[{i}]" (head [ "Target"; idents opts bound ]) with Children = [ expr "body" body ] }
        { node label "DecisionTree" with Children = expr "expr" inner :: List.mapi target targets }
    | DecisionTreeSuccess(index, bound, t) ->
        { node label $"DecisionTreeSuccess targetIndex={index}{typed t}" with
            Children = many "boundValues" bound }
    | Let(i, value, body) ->
        { node label $"Let {ident opts i}" with Children = [ expr "value" value; expr "body" body ] }
    | LetRec(bindings, body) ->
        let binding i (b: Ident, value) = expr $"bindings[{i}] {ident opts b}" value
        { node label "LetRec" with Children = List.mapi binding bindings @ [ expr "body" body ] }
    | Get(inner, kind, t, r) ->
        let case, kindChildren =
            match kind with
            | TupleIndex i -> $"TupleIndex {i}", []
            | ExprGet key -> "ExprGet", [ expr "kind.expr" key ]
            | FieldGet info ->
                let extra =
                    [ if full then
                          info.FieldType |> Option.map (fun t -> $"FieldType={typ opts t}") |> Option.defaultValue ""
                          if info.IsMutable then "mutable"
                          if info.MaybeCalculated then "maybeCalculated"
                      if plugin && not info.Tags.IsEmpty then tags info.Tags ]
                    |> List.filter ((<>) "")
                let extra = if extra.IsEmpty then "" else " (" + join ", " extra + ")"
                $"FieldGet {str opts info.Name}{extra}", []
            | UnionField info ->
                let case = unionCase opts info.Entity info.CaseIndex
                let caseName = case |> Option.map (fun c -> $" {c.Name}") |> Option.defaultValue ""
                let field =
                    case
                    |> Option.bind (fun c -> List.tryItem info.FieldIndex c.UnionCaseFields)
                    |> Option.map (fun f -> $" ({f.Name})")
                    |> Option.defaultValue ""
                $"UnionField {info.Entity.FullName}{caseName} case={info.CaseIndex} field={info.FieldIndex}{field}", []
            | UnionTag -> "UnionTag", []
            | ListHead -> "ListHead", []
            | ListTail -> "ListTail", []
            | OptionValue -> "OptionValue", []
        { node label (head [ "Get"; case + typed t; range r ]) with
            Children = expr "expr" inner :: kindChildren }
    | Set(inner, kind, t, value, r) ->
        let case, kindChildren =
            match kind with
            | ExprSet key -> "ExprSet", [ expr "kind.expr" key ]
            | FieldSet name -> $"FieldSet {str opts name}", []
            | ValueSet -> "ValueSet", []
        { node label (head [ "Set"; case + typed t; range r ]) with
            Children = expr "expr" inner :: kindChildren @ [ expr "value" value ] }
    | Sequential exprs -> { node label "Sequential" with Children = many "exprs" exprs }
    | WhileLoop(guard, body, r) ->
        { node label (head [ "WhileLoop"; range r ]) with Children = [ expr "guard" guard; expr "body" body ] }
    | ForLoop(i, start, limit, body, isUp, r) ->
        { node label (head [ "ForLoop"; ident opts i; (if isUp then "up" else "down"); range r ]) with
            Children = [ expr "start" start; expr "limit" limit; expr "body" body ] }
    | TryCatch(body, catch, finalizer, r) ->
        let catch =
            catch |> Option.map (fun (i, handler) -> expr $"catch {ident opts i}" handler) |> Option.toList
        { node label (head [ "TryCatch"; range r ]) with
            Children = expr "body" body :: catch @ opt "finalizer" finalizer }
    | IfThenElse(guard, thenExpr, elseExpr, r) ->
        { node label (head [ "IfThenElse"; range r ]) with
            Children = [ expr "guardExpr" guard; expr "thenExpr" thenExpr; expr "elseExpr" elseExpr ] }
    | Unresolved(unresolved, t, r) ->
        let case, children =
            match unresolved with
            | UnresolvedTraitCall(_, traitName, isInstance, _, args) ->
                $"UnresolvedTraitCall {traitName} instance={isInstance}", many "argExprs" args
            | UnresolvedReplaceCall(thisArg, args, info, attached) ->
                $"UnresolvedReplaceCall {info.DeclaringEntityFullName}.{info.CompiledName}",
                opt "thisArg" thisArg @ many "args" args @ opt "attachedCall" attached
            | UnresolvedInlineCall(name, _, callee, info) ->
                let _, children = callInfo "info" info
                $"UnresolvedInlineCall {name}", opt "callee" callee @ children
        { node label (head [ "Unresolved"; case + typed t; range r ]) with Children = children }
    | Extended(extended, r) ->
        let case, children =
            match extended with
            | Throw(thrown, t) -> "Throw" + typed t, opt "expr" thrown
            | Debugger -> "Debugger", []
            | Curry(curried, arity) -> $"Curry arity={arity}", [ expr "expr" curried ]
        { node label (head [ "Extended"; case; range r ]) with Children = children }
    | Quote(quoted, isTyped, r) ->
        { node label (head [ $"Quote typed={isTyped}"; range r ]) with Children = [ expr "quotedExpr" quoted ] }

let memberDecl (opts: Options) (label: string) (m: MemberDecl) : Node =
    let plugin = opts.Detail >= Plugin
    let full = opts.Detail = Full
    { node label $"MemberDecl {m.Name} {idents opts m.Args}" with
        Props =
            [ if plugin then ("MemberRef", memberRef opts m.MemberRef)
              if plugin && not m.Tags.IsEmpty then ("Tags", tags m.Tags)
              if full then
                  ("IsMangled", string m.IsMangled)
                  match m.ImplementedSignatureRef with
                  | Some s -> ("ImplementedSignatureRef", memberRef opts s)
                  | None -> () ]
        Children = [ expr opts "Body" m.Body ] }

let rec declaration (opts: Options) (label: string) (d: Declaration) : Node =
    match d with
    | ModuleDeclaration m ->
        { node label $"ModuleDeclaration {m.Name} ({m.Entity.FullName})" with
            Children = m.Members |> List.mapi (fun i -> declaration opts $"Members[{i}]") }
    | ActionDeclaration a -> { node label "ActionDeclaration" with Children = [ expr opts "Body" a.Body ] }
    | MemberDeclaration m -> memberDecl opts label m
    | ClassDeclaration c ->
        { node label $"ClassDeclaration {c.Name} ({c.Entity.FullName}{entityAttributes opts c.Entity})" with
            Children =
                (c.Constructor |> Option.map (memberDecl opts "Constructor") |> Option.toList)
                @ (c.BaseCall |> Option.map (expr opts "BaseCall") |> Option.toList)
                @ (c.AttachedMembers |> List.mapi (fun i -> memberDecl opts $"AttachedMembers[{i}]")) }

let rec private size (n: Node) = 1 + List.sumBy size n.Children

/// Draws a node and its subtree as box-drawing lines.
let render (opts: Options) (root: Node) =
    let sb = StringBuilder()
    let line (s: string) = sb.Append(s.TrimEnd()).Append('\n') |> ignore
    let rec go (indent: string) (connector: string) (childIndent: string) (depth: int) (n: Node) =
        let label = if n.Label = "" then "" else n.Label + ": "
        line (indent + connector + label + n.Head)
        let inner = indent + childIndent
        let bar = if n.Children.IsEmpty then "   " else "│  "
        for name, value in n.Props do
            line $"{inner}{bar}· {name} = {value}"
        match opts.MaxDepth with
        | Some max when depth >= max && not n.Children.IsEmpty ->
            line $"{inner}└─ … {List.sumBy size n.Children} nodes"
        | _ ->
            n.Children
            |> List.iteri (fun i child ->
                let isLast = i = n.Children.Length - 1
                go inner (if isLast then "└─ " else "├─ ") (if isLast then "   " else "│  ") (depth + 1) child)
    go "" "" "" 0 root
    sb.ToString().TrimEnd()

/// The tree of an expression as text.
let printExpr (opts: Options) (e: Expr) = expr opts "" e |> render opts

/// The tree of a member declaration (its header, then its body) as text.
let printMember (opts: Options) (m: MemberDecl) = memberDecl opts "" m |> render opts

/// The tree of a file's declarations as text.
let printDeclarations (opts: Options) (ds: Declaration list) =
    ds |> List.map (declaration opts "" >> render opts) |> join "\n\n"
