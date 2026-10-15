namespace Partas.Solid
// The majority of this plugin behaviour is enabled by the following pattern matchers and functions:
// - BuilderCollector
//     Collects everything in the tag builder computations
// - PropsCollector
//     Collects all the properties passed to tag constructors
// - TagConstructor
//     Matches any tag constructor
// - transform
//     Entry point for almost every expression
// Everything else is pretty much just sugar for reducing boiler plate by identifying simple patterns in Exprs.
// A lot of heavy sugar use with patterns against Expr and Ident node patterns using:
// - Ident.IdentIs
// - Utils.StartsWith | Utils.EndsWith | Utils.StartsWithTrimmed | etc
open System
open Fable
open Fable.AST
open Fable.AST.Fable

[<assembly: ScanForPlugins>]
do ()

module internal rec AST =
    module Utils = Patterns
    [<AutoOpen>]
    module AttributesAndProperties =
        type private CollectionItem =
            | Single of Expr
            | Spread of Expr

        // Match Fable library imports, never user functions with similar names.
        let private (|SeqCall|_|) name = function
            | Call(Import({ Selector = selector; Path = path; Kind = LibraryImport _ }, _, _), info, _, _)
                when selector = name && path.EndsWith("/Seq.js", System.StringComparison.Ordinal) -> Some info.Args
            | _ -> None
        let rec private (|SequenceThunk|_|) = function
            | TypeCast(SequenceThunk body, _) -> Some body
            | Lambda({ Type = Unit; IsCompilerGenerated = true }, body, _) -> Some body
            | _ -> None
        let rec private (|DelayedSequence|_|) = function
            | TypeCast(DelayedSequence body, _) -> Some body
            | SeqCall "delay" [ SequenceThunk body ] -> Some body
            | _ -> None
        let private collectionArray items =
            if items |> List.forall (function Single _ -> true | _ -> false) then
                items |> List.map (function Single expr -> expr | Spread _ -> failwith "unreachable") |> AstUtils.ValueArray
            else
                let slots = items |> List.mapi (fun i item -> match item with Single _ -> "$" + string i | Spread _ -> "...($" + string i + ")")
                let args = items |> List.map (function Single expr | Spread expr -> expr)
                AstUtils.Emit("[" + String.concat ", " slots + "]", AstUtils.CallInfo(args = args))

        let rec private normalizeProperty (expr: Expr): Expr =
            let wrap binding items =
                match items with
                | [Single value] -> [Single(binding value)]
                | _ -> [Spread(binding (collectionArray items))]
            let rec collect = function
                | TypeCast(inner, _) -> collect inner
                | SeqCall "singleton" [value] -> Some [Single(normalizeProperty value)]
                | SeqCall "empty" [] -> Some []
                | DelayedSequence body -> collect body
                // The right sequence's construction must be inert: an eager singleton
                // could run its effects before enumeration of the left sequence.
                | SeqCall "append" [left; (DelayedSequence _ as right)] ->
                    match collect left, collect right with
                    | Some left, Some right -> Some(left @ right)
                    | _ -> None
                | IfThenElse(guard, yes, no, range) ->
                    match collect yes, collect no with
                    | Some [Single yes], Some [Single no] ->
                        Some [Single(IfThenElse(normalizeProperty guard, yes, no, range))]
                    | Some yes, Some no ->
                        Some [Spread(IfThenElse(normalizeProperty guard, collectionArray yes, collectionArray no, range))]
                    | _ -> None
                // Retain bindings instead of substituting matchValue names through thunks.
                | Let(ident, value, body) ->
                    collect body |> Option.map (wrap (fun body -> Let(ident, normalizeProperty value, body)))
                | Sequential exprs when not exprs.IsEmpty ->
                    let prefix = exprs |> List.take (exprs.Length - 1) |> List.map normalizeProperty
                    collect (List.last exprs) |> Option.map (wrap (fun body -> AstUtils.Sequential(prefix @ [body])))
                | DecisionTree(tree, targets) ->
                    let collected = targets |> List.map (fun (ids, body) -> ids, collect body)
                    if collected |> List.exists (snd >> Option.isNone) then None
                    else
                        let targets = collected |> List.map (fun (ids, items) -> ids, Option.get items)
                        let single = targets |> List.forall (function _, [Single _] -> true | _ -> false)
                        let body items = match items with [Single value] when single -> value | _ -> collectionArray items
                        let decision = DecisionTree(normalizeProperty tree, targets |> List.map (fun (ids, items) -> ids, body items))
                        Some [if single then Single decision else Spread decision]
                | _ -> None
            match expr with
            | SeqCall "toArray" [DelayedSequence body] ->
                match collect body with
                | Some items ->
                    match collectionArray items, expr with
                    | Emit(info, _, _), Call(_, _, typ, range) -> Emit(info, typ, range)
                    // Seq.toArray returns a plain JS Array even when numeric literals
                    // normally use typed arrays. Preserve its type externally, and its
                    // plain-array representation through the Any-typed literal inside.
                    | (Value(NewArray _, _) as array), Call(_, _, typ, range) ->
                        Emit({ Macro = "$0"; IsStatement = false; CallInfo = AstUtils.CallInfo(args = [array]) }, typ, range)
                    | value, _ -> value
                | None -> expr
            | Call(callee, info, typ, range) ->
                Call(normalizeProperty callee, { info with Args = List.map normalizeProperty info.Args; ThisArg = Option.map normalizeProperty info.ThisArg }, typ, range)
            | CurriedApply(callee, args, typ, range) -> CurriedApply(normalizeProperty callee, List.map normalizeProperty args, typ, range)
            | TypeCast(value, typ) -> TypeCast(normalizeProperty value, typ)
            | Lambda(arg, body, name) -> Lambda(arg, normalizeProperty body, name)
            | Delegate(args, body, name, tags) -> Delegate(args, normalizeProperty body, name, tags)
            | Let(ident, value, body) -> Let(ident, normalizeProperty value, normalizeProperty body)
            | Sequential exprs -> Sequential(List.map normalizeProperty exprs)
            | IfThenElse(guard, yes, no, range) -> IfThenElse(normalizeProperty guard, normalizeProperty yes, normalizeProperty no, range)
            | DecisionTree(tree, targets) -> DecisionTree(normalizeProperty tree, targets |> List.map (fun (ids, body) -> ids, normalizeProperty body))
            | DecisionTreeSuccess(index, values, typ) -> DecisionTreeSuccess(index, List.map normalizeProperty values, typ)
            | Get(value, kind, typ, range) ->
                Get(normalizeProperty value, (match kind with ExprGet index -> ExprGet(normalizeProperty index) | _ -> kind), typ, range)
            | Operation(kind, tags, typ, range) ->
                let kind = match kind with
                           | Unary(op, value) -> Unary(op, normalizeProperty value)
                           | Binary(op, left, right) -> Binary(op, normalizeProperty left, normalizeProperty right)
                           | Logical(op, left, right) -> Logical(op, normalizeProperty left, normalizeProperty right)
                Operation(kind, tags, typ, range)
            | Value(NewArray(ArrayValues values, typ, kind), range) ->
                Value(NewArray(ArrayValues(List.map normalizeProperty values), typ, kind), range)
            | Value(NewTuple(values, isStruct), range) ->
                Value(NewTuple(List.map normalizeProperty values, isStruct), range)
            | _ -> expr

        /// <summary>Materializes recognized attribute array computations while preserving item count, bindings and evaluation order.</summary>
        let unrollValue (ctx: PluginContext) expr =
            if ctx.HasFlag ComponentFlag.SkipCEOptimisation then expr else normalizeProperty expr

        /// <summary>
        /// Recognizes whether current expression is considered a prop setter by
        /// any of our definitions.
        /// </summary>
        /// <remarks>
        /// It is imperative that this recognizer comes AFTER the PropertyGetter
        /// since it is greedier and will consume and nullify attribute expressions
        /// that involve the use of the properties argument.
        /// </remarks>
        let private (|PropertySetter|_|) (ctx: PluginContext) = function
            | Call(_, (CallInfo.PropertySetter ctx as callInfo), _, _)
                // This is a `props.poop <- 5` type call that had the attribute defined
                // within the same module/file/type (Local)
            | Call(Expr.ImportedSetter ctx, callInfo, _, _) ->
                // This is a `props.class' <- "value"` type call that had the extension
                // defined in a different module (Imported)
                match callInfo with
                | { ThisArg = Some(IdentExpr(Ident.IdentIs ctx IdentType.Props))
                    MemberRef = MemberRef.Option.PartasName ctx name
                    Args = expr :: _ } ->
                    Some(name, expr)
                | _ ->
                    None
            // This is a setter to a val mutable field.
            | Set(
                expr = (
                    // Defined locally
                    IdentExpr(Ident.IdentIs ctx IdentType.Props)
                    // inherited
                  | TypeCast(expr = IdentExpr(Ident.IdentIs ctx IdentType.Props))
                );
                kind = FieldSet(name);
                value = expr) ->
                Some(name, expr)

            | _ -> None
        /// <summary>
        /// Recognizes whether current expression is considered a prop setter by any
        /// of our definitions.
        /// </summary>
        /// <remarks>
        /// It is imperative that this recognizer in a recursive transformation tree
        /// comes BEFORE the PropertySetter recognizer. The Setter is greedy, and will
        /// nullify expressions that are attribute expressions which involve the props ident
        /// </remarks>
        let private (|PropertyGetter|_|) (ctx: PluginContext): Expr -> string option = function
            | Get(
                expr = (
                    // Defined locally
                    IdentExpr(Ident.IdentIs ctx IdentType.Props)
                    // Inherited
                  | TypeCast(expr = IdentExpr(Ident.IdentIs ctx IdentType.Props))
                );
                kind = FieldGet({ Name = prop }))
            | Call(
                info = {
                    ThisArg = Some(IdentExpr(Ident.IdentIs ctx IdentType.Props))
                    Args = [ _ ]
                    MemberRef = MemberRef.Option.PartasName ctx prop
                } & { MemberRef =Some(MemberRef.MemberRefIs ctx MemberRefType.Getter) }
                ) ->
                Some(prop)
            | _ -> None
        // Collects and transforms any of our 'special' `props` usage so that we can
        // later create the omit and merge to suit.
        let (|PropsGetterOrSetter|_|) (ctx: PluginContext) = function
            | PropertyGetter ctx prop ->
                PluginContext.addGetter ctx prop
                AstUtils.GetProp(
                    ctx.SelfIdentifier,
                    StringUtils.TrimReservedIdentifiers prop
                    )
                |> Some
            | PropertySetter ctx (prop, expr) ->
                PluginContext.addSetter ctx (prop, transformValue ctx expr)
                AstUtils.Unit
                |> Some
            | _ -> None
        let private lambdaArity (typ: Type) =
            let rec go n = function LambdaType(_, returnType) -> go (n + 1) returnType | _ -> n
            go 0 typ
        // `props.f a b` where `f: A -> B -> C`. Fable sends a function prop uncurried through its setter, but
        // leaves the application through the getter curried; call it uncurried to match. Must be matched before
        // the getter is transformed, which loses its type.
        let (|UncurriedPropsApply|_|) (ctx: PluginContext) = function
            | CurriedApply(PropertyGetter ctx prop & getter, args, typ, range)
                when lambdaArity getter.Type > 1 && args.Length >= lambdaArity getter.Type ->
                PluginContext.addGetter ctx prop
                let now, later = args |> List.map (transform ctx) |> List.splitAt (lambdaArity getter.Type)
                let call =
                    AstUtils.Call(
                        AstUtils.GetProp(ctx.SelfIdentifier, StringUtils.TrimReservedIdentifiers prop),
                        AstUtils.CallInfo(args = now),
                        typ
                        )
                Some(if later.IsEmpty then call else CurriedApply(call, later, typ, range))
            // Partial application, e.g. `onClick = props.onSelect item`: close over the missing arguments
            | CurriedApply(PropertyGetter ctx prop & getter, args, typ, range)
                when lambdaArity getter.Type > 1 ->
                PluginContext.addGetter ctx prop
                let missing =
                    List.init (lambdaArity getter.Type - args.Length) (fun i -> AstUtils.Ident($"PARTAS_ARG{i}", isCompilerGenerated = true))
                let call =
                    AstUtils.Call(
                        AstUtils.GetProp(ctx.SelfIdentifier, StringUtils.TrimReservedIdentifiers prop),
                        AstUtils.CallInfo(args = (args |> List.map (transform ctx)) @ (missing |> List.map IdentExpr)),
                        Any
                        )
                Some(Delegate(missing, call, None, []))
            | _ -> None
        // Coerces a bool attribute value to the strings "true"/"false", for enumerated attributes where Solid 2
        // would otherwise remove the attribute on `false`. Non-bool values pass through.
        let private enumeratedBool (ctx: PluginContext) expr =
            match transform ctx expr with
            | Value(BoolConstant b, r) -> Value(StringConstant(if b then "true" else "false"), r)
            | value when value.Type = Type.Boolean -> AstUtils.Emit("String($0)", AstUtils.CallInfo(args = [ value ]))
            | value -> value
        // Determines at top level whether the current expression could be considered
        // as a attribute expression by any of our definitions
        let (|AttributeExpression|_|) (ctx: PluginContext) = function
            | Call(
                callee,
                {
                    ThisArg = Some(IdentExpr(Ident.IdentIs ctx IdentType.ReturnVal))
                    Args = (
                        TagValue.TagValue ctx expr
                      | expr
                      ) :: _
                    MemberRef = MemberRef.Option.PartasName ctx prop as memberRef
                },
                _,
                _) ->
                match callee, memberRef with
                // Aria module properties: `ariaLabel` -> `aria-label`. ARIA's boolean states take the strings
                // "true"/"false"; Solid 2 removes an attribute whose value is `false`.
                | (Expr.ImportedSetter ctx, Some(MemberRef({ FullName = "Partas.Solid.Aria" }, _)))
                | (_, Some(MemberRef({ FullName = "Partas.Solid.Aria" }, _) & MemberRef.MemberRefIs ctx MemberRefType.Setter))
                    when prop.StartsWith "aria" ->
                    ($"""aria-{(prop.Substring "aria".Length).ToLower()}""", enumeratedBool ctx expr)
                    |> Some
                // `spellcheck` is an enumerated "true"/"false" attribute typed `bool` in the bindings. Solid 2
                // removes an attribute whose value is `false`, so it takes the same coercion as ARIA.
                | (Expr.ImportedSetter ctx, Some(MemberRef({ FullName = "Partas.Solid.HtmlAttributes" }, _)))
                | (_, Some(MemberRef({ FullName = "Partas.Solid.HtmlAttributes" }, _) & MemberRef.MemberRefIs ctx MemberRefType.Setter))
                    when prop = "spellcheck" ->
                    (prop, enumeratedBool ctx expr)
                    |> Some
                | Expr.ImportedSetter ctx, _ // Captures properties defined in other modules
                | _, Some(MemberRef.MemberRefIs ctx MemberRefType.Setter) -> // Captures properties defined in self module
                    (prop, transformValue ctx expr)
                    |> Some
                | _, _ -> None
            // member val set/get
            | Set( // With inheritance within the same module etc, there may be a TypeCast on the IdentExpr.
                expr = Expr.TypeCastDrill ctx (IdentExpr(Ident.IdentIs ctx IdentType.ReturnVal)) // TypeCast drill to first expr
                kind = FieldSet(prop)
                value = expr
                ) ->
                (prop, transformValue ctx expr)
                |> Some
            // Inlined named overloads to `[<DefaultValue>] val mutable` properties/attributes
            | Let(
                    { IsThisArgument = true; IsCompilerGenerated = true },
                    IdentExpr(Ident.IdentIs ctx IdentType.ReturnVal),
                    Set(
                        expr = IdentExpr({  IsThisArgument = true (*; IsCompilerGenerated = false *) })
                        kind = FieldSet(prop)
                        value = expr
                    )
                ) ->
                (prop, transformValue ctx expr) |> Some
            // As above, where Fable let-binds a non-trivial argument before the set
            | Let(
                    { IsThisArgument = true; IsCompilerGenerated = true },
                    IdentExpr(Ident.IdentIs ctx IdentType.ReturnVal),
                    Let(
                        valueIdent,
                        expr,
                        Set(
                            expr = IdentExpr({  IsThisArgument = true })
                            kind = FieldSet(prop)
                            value = Expr.TypeCastDrill ctx (IdentExpr({ Name = valueName }))
                        )
                    )
                ) when valueIdent.Name = valueName ->
                (prop, transformValue ctx expr) |> Some
            // Captured method/Extension call
            | Call(
                Value(ValueKind.UnitConstant, None),
                {
                    MemberRef = Some(MemberRef( info = { CompiledName = extensionName }))
                    Args = exprs
                },
                Type.MetaType,
                None
                ) ->
                let fable4 = ctx |> PluginContext.helper |> _.Options |> _.Define |> List.contains "FABLE_COMPILER_4"
                let fable5 = ctx |> PluginContext.helper |> _.Options |> _.Define |> List.contains "FABLE_COMPILER_5"
                match extensionName, exprs with
                | ("bool" | "data" | "attr"), [ Value(StringConstant prop, _); value ] ->
                    let propName =
                        match extensionName with
                        | "data" -> $"data-{prop}"
                        | "attr" | "bool" -> prop
                        | _ -> failwith "Unreachable"
                    Some(propName, transform ctx value)
                | ("ref" | "style'"), [ identExpr ] ->
                    Some(extensionName |> StringUtils.TrimReservedIdentifiers, transform ctx identExpr)
                | "classObj", [ identExpr ] ->
                    Some("class", transform ctx identExpr)
                | "spread", _ when fable4 ->
                    "Spread is not supported in fable4"
                    |> PluginContext.logError ctx
                    None
                | "spread", [ IdentExpr(Ident.IdentIs ctx IdentType.Props) | TypeCast(IdentExpr(Ident.IdentIs ctx IdentType.Props), _) ]
                    when fable5 ->
                    let identifier =
                        if ctx.HasFlag(ComponentFlag.SkipOmit) || ctx.HasFlag(ComponentFlag.SpreadProps)
                        then ctx.SelfIdentifier
                        else "PARTAS_OTHERS"
                        |> sprintf "{...%s} n$"
                    Some(identifier, Value(ValueKind.BoolConstant(false), None))
                | "spread", [ (TypeCast(IdentExpr({ Name = ident }), _) | IdentExpr({ Name = ident })) ]
                    when fable5 ->
                    Some("{..." + ident + "} n$", AstUtils.Value(false))
                // handles tupled getters `div().spread(someEnumerable[0])`
                | "spread", [ (
                    TypeCast(
                        expr = Get( expr = IdentExpr({ Name = ident })
                                    kind = TupleIndex indx ) )
                    ) | (
                        Get(
                            expr = IdentExpr({ Name = ident })
                            kind = TupleIndex indx
                        )
                    ) ] ->
                    Some("{..." + ident + "[" + string indx + "]} n$", AstUtils.Value(false))
                // handles field getters `div().spread(props.otherProps)`
                | "spread", [ (TypeCast(
                        expr = Get(
                            expr = IdentExpr({ Name = ident })
                            kind = FieldGet { Name = identSuffix }
                            )
                        )
                    ) | (
                        Get(
                            expr = IdentExpr({ Name = ident })
                            kind = FieldGet { Name = identSuffix }
                        )
                    ) ] ->
                    Some("{..." + ident + "." + identSuffix + "} n$", AstUtils.Value(false))
                | "spread", [ expr ]
                    when fable4 ->
                    $"Spread does not support this as a value in Fable 4 or below:\n{expr}"
                    |> PluginContext.logError ctx
                    None
                // If polymorph, then revise the expression to be a lambda call
                // that spreads the props within.
                // Polymorphic attributes by default include: "as'" and "asChild" for supporting
                // Kobalte and ArkUI.
                // But anyone can include a polymorphic attribute by using an inline alias property
                // to the actual propertyname which has to be prepended with __PARTAS_POLYMORPHIC__
                | Polymorphism.PolymorphicAttribute ctx attrName, [ expr ] ->
                    match transform ctx expr with
                    | Call(
                        callee, ({
                            Tags = [ "jsx" ]
                            Args = [ tagName ; internalCollection ]
                        } as callInfo), typ, range)
                        when callee = JsxUtils.CreateElementImportExpr && typ = JsxUtils.ElementType ->
                        let c: Ident = AstUtils.Ident("PARTAS_POLYPROPS")
                        Lambda(c,
                            Call(
                            callee,
                            { callInfo with Args = [
                                        tagName;
                                        Value(
                                            NewList(Some(
                                                JsxUtils.Prop("{...PARTAS_POLYPROPS} n$", AstUtils.Value(false))
                                                , internalCollection
                                            ), Any),
                                            None
                                            )
                            ]  },
                            typ, range), None)
                    | expr -> expr
                    |> fun expr ->
                        Some(attrName, expr)

                | _ ->
                    $"Unhandled extension: {extensionName}\nProvidedValues: {exprs}"
                    |> PluginContext.logError ctx
                    None
            | Call(
                callee = Value(ValueKind.UnitConstant, None)
                info = {
                    MemberRef = (
                        Some(MemberRef.MemberRefIs ctx MemberRefType.Setter)
                         & MemberRef.Option.PartasName ctx propName
                    )
                    Args = value :: _
                }) ->
                match propName with
                | Utils.StartsWithTrimmed "aria" ariaName ->
                    Some($"aria-{ariaName.ToLower()}", enumeratedBool ctx value)
                | "spellcheck" -> Some(propName, enumeratedBool ctx value)
                | _ -> Some(propName, transformValue ctx value)
            | _ -> None
        /// wraps single expressions in a list and feeds back to the list collector
        let (|PropCollectorFeeder|) (ctx: PluginContext): Expr -> PropList = fun e -> [ e ] |> function
            | PropCollector ctx props -> props

        /// Use this active recognizer on expression lists to collect all the properties.
        /// If an unexpected expression is read, then it will dispose of it with a warning in
        /// compilation
        let (|PropCollector|) (ctx: PluginContext): Expr list -> PropList = function
            | [] -> []
            | Sequential (PropCollector ctx headProps) :: PropCollector ctx tailProps ->
                tailProps @ headProps
            | expr :: PropCollector ctx props ->
                match expr with
                | AttributeExpression ctx propInfo -> [ propInfo ]
                | Sequential(PropCollector ctx props) -> props
                | Value(Null(Unit), None) -> []
                | Value(UnitConstant, None) -> []
                | _ ->
                    // There are a lot of artifact expressions disposed of,
                    // but if you notice an attribute isn't being rendered, then
                    // add the `--verbosity` flag to Fable, and then copy the output
                    // to a file and search the text for the expression by some keyword
                    PluginContext.debugDisposal ctx "PropCollector" expr
                    []
                |> (@) props

    (*
    The tag constructor recognizer actively matches ANY expression pattern that, according to our
    scheme, should be transpiled into a JSX tag. This must occur from within this recognizer, regardless
    of whether that expression is a method chained to a tag or other.
    Builders should be a non issue, as they are separated into the appropriate expressions for transformation
    by the BuilderCollectors
    *)
    let (|TagConstructor|_|) (ctx: PluginContext) = function
        // If we do not position this above the local tag recogniser, then it will never be reached in cases that it is
        // a local tag using the PartasImport attribute
        // Non LibraryImports that require LibraryImport injection via the attribute PartasImportAttribute
        | Call(
            callee = (
                Expr.ImportedConstructor ctx & Import(_, t, r) // Captures constructors from other modules with PartasImportAttribute
              | IdentExpr(Ident.IdentIs ctx IdentType.Constructor) & IdentExpr({ Type = t; Range = r })
            // Captures constructors defined in the same module with PartasImportAttribute
            )
            info = {   (* CallInfo *)
                Args = PropCollector ctx props
            }
            typ = (Type.PartasName ctx _ & Type.HasPartasImport ctx (selector, path))
            range = range) ->
            let importExpr =
                Import(
                    { Selector = selector
                      Path = path
                      Kind = UserImport false },
                    t, r)
            ElementBuilder.create (TagSource.LibraryImport importExpr) props range
            |> Some
        // If we do not position this above the local tag recogniser, then it will never be reached in cases that it is
        // a local tag using the PartasImport attribute
        // Non LibraryImports that require LibraryImport injection via the attribute PartasProxyImportAttribute
        | Call(
            callee = (
                Expr.ImportedConstructor ctx & Import(_, t, r) // Captures constructors from other modules with PartasImportAttribute
              | IdentExpr(Ident.IdentIs ctx IdentType.Constructor) & IdentExpr({ Type = t; Range = r })
            // Captures constructors defined in the same module with PartasImportAttribute
            )
            info = {
                Args = PropCollector ctx props
            }
            typ = (Type.PartasName ctx _ & Type.HasPartasProxyImport ctx (key, selector, path))
            range = range) ->
            let importExpr =
                Get(
                    Import(
                        { Selector = selector
                          Path = path
                          Kind = UserImport false },
                        t, r),
                    FieldInfo.Create(key),
                    t,r)
            ElementBuilder.create (TagSource.LibraryImport importExpr) props range
            |> Some
        // Local tag
        | Call(
            IdentExpr(Ident.IdentIs ctx IdentType.Constructor),
            (CallInfo.Constructor ctx & {
                Args = PropCollector ctx props
            }),
            (Type.PartasName ctx (Utils.EndsWithTrimmed "_$ctor" typeName) | Type.PartasName ctx typeName),
            range) ->
            ElementBuilder.create (TagSource.AutoImport typeName) props range
            |> Some
        // Native import
        | Call(
            Expr.NativeImportedConstructor ctx,
            {
                Args = PropCollector ctx props
            },
            Type.PartasName ctx typeName,
            range) ->
            ElementBuilder.create (TagSource.AutoImport typeName) props range
            |> Some
        // Library Imports
        | Call(
            Expr.ImportedConstructor ctx & Import({Kind = UserImport false}, _, _) as imp,
            {
                Args = PropCollector ctx props
                MemberRef  = Some(MemberRef(_, { CompiledName = ".ctor" }))
            },
            (Type.PartasName ctx _typeName & typ),
            range) when Type.isTag ctx typ ->
            ElementBuilder.create (TagSource.LibraryImport imp) props range
            |> Some
        // Non LibraryImports; ie User defined imports
        | Call(
            (Expr.ImportedConstructor ctx & Import(identee, t, r)),
            {
                Args = PropCollector ctx props
                MemberRef = Some(MemberRef(_, { CompiledName = ".ctor" }))
            },
            (Type.PartasName ctx typeName & typ),
            range) when Type.isTag ctx typ -> // an imported JS class in user code is not a tag
            let importExpr = Import({identee with Selector = typeName}, t, r)
            ElementBuilder.create (TagSource.LibraryImport importExpr) props range
            |> Some
        // WITH PROPS
        | Let(
            Ident.IdentIs ctx (IdentType.ReturnVal | IdentType.Element),
            TagConstructor ctx tagInfo,
            PropCollectorFeeder ctx bodyProps
            ) ->
            { tagInfo with Properties = bodyProps @ tagInfo.Properties }
            |> Some
        // WITH PROPS & EXTENSION
        | Call(Import({ Kind = MemberImport(MemberRef({ FullName = Utils.EndsWith "HtmlElementExtensions" | Utils.EndsWith "PolymorphicExtensions" }, _)) }, _, _) as _callee, ({
                Args = TagConstructor ctx tagInfo :: rest
            } as callInfo), _, _) ->
            // We compile all the information of the extension call into the callinfo and make the rest almost impossible to mistake
            // for a different type of expression. We can then add this to the property list
            [ Call(Expr.Value(ValueKind.UnitConstant, None), { callInfo with Args = rest }, Type.MetaType, None) ]
            |> function
                | PropCollector ctx attributeCall ->
                    { tagInfo with Properties = attributeCall @ tagInfo.Properties }
                    |> Some
        // WITH CHILDREN
        | Call(
            callee = Expr.ImportedContainerExtensionName ctx "Run" // run is being called. builder being applied to obj. obj is arg 1.
            info = {   // Check the first argument, which is the argument this is being called against, for the constructor which called it.
                Args = TagConstructor ctx tagInfo :: BuilderCollector ctx rest // The rest are the computations being applied
            }) ->
            { tagInfo with Children = rest @ tagInfo.Children }
            |> Some

        | Let(Ident.IdentIs ctx IdentType.Element, TagConstructor ctx tagInfo, BuilderCollectorFeedback ctx body) ->
            { tagInfo with Children = body @ tagInfo.Children }
            |> Some
        // An imported context
        | CurriedApply(
            applied = Import(
                { Kind = MemberImport(MemberRef(_,{ CompiledName = typeName })) } as imp,
                (Type.PartasName ctx "ContextProvider" as typ),
                irange)
            args = props) ->
            {
                TagSource = TagSource.LibraryImport(Import(imp, typ, irange)) // Solid 2: a context is its own provider
                Properties = [("value", Sequential(props))]
                Children = []
                Range = irange
            }
            |> Some
        // Local defined context
        | CurriedApply(
                applied = IdentExpr({ Name = typeName; Type = Type.PartasName ctx "ContextProvider" })
                args = props
                range = range
            ) ->
            {
                TagSource = TagSource.AutoImport typeName
                Properties = [("value", Sequential(props))]
                Children = []
                Range = range
            }
            |> Some
        | _ -> None

    // A collected child sequence is a value containing every child, not ordinary
    // JavaScript sequencing (whose value is only the last expression).
    let childSequence = function
        | [] -> AstUtils.Unit
        | [ expr ] -> expr
        | exprs ->
            // Separate raw JSX text lines would introduce whitespace between adjacent
            // strings. Expression children retain the exact string values.
            exprs
            |> List.map (function
                | Value(StringConstant text, _) -> JsxUtils.StringExpression text
                | expr -> expr)
            |> List.rev // ChildrenProp builds its internal F# list by prepending.
            |> fun children -> JsxUtils.CreateElement("", [], children)

    let (|ReactiveMatch|_|) (ctx: PluginContext) expr =
        ReactiveMatch.tryLower ctx (transform ctx) (fun body -> childSequence (match body with BuilderCollectorFeedback ctx children -> children)) expr

    /// Transforms a single expression before wrapping it in a list and feeding it back to the BuilderCollector.
    let (|BuilderCollectorFeedback|) (ctx: PluginContext): Expr -> Expr list = fun e -> [ transform ctx e ] |> function
        | BuilderCollector ctx feedback -> feedback

    /// This active recognizer is a bit more psychotic in terms of recursion
    let (|BuilderCollector|) (ctx: PluginContext): Expr list -> Expr list = function
        | [] -> []
        | Sequential (BuilderCollector ctx headBuilds) :: BuilderCollector ctx tailBuilds ->
            headBuilds @ tailBuilds
        | expr :: BuilderCollector ctx restBuilds ->
            match expr with
            | ReactiveMatch ctx expr -> expr :: restBuilds
            // Builder op
            | Let(Ident.IdentIs ctx (IdentType.Element | IdentType.First | IdentType.Text),
                  BuilderCollectorFeedback ctx value,
                  BuilderCollectorFeedback ctx body
                  ) ->
                    value @ body @ restBuilds
            // Builder op
            | Let(
                Ident.IdentIs ctx (IdentType.Builder | IdentType.Second),
                BuilderCollectorFeedback ctx value,
                BuilderCollectorFeedback ctx body
                ) ->
                    body @ value @ restBuilds
            | UncurriedPropsApply ctx expr -> expr :: restBuilds
            // This was somehow related to fixes for Oxpecker.Solid.Tests
            | CurriedApply(BuilderCollectorFeedback ctx applied, BuilderCollector ctx args, typ, range) ->
                if args.IsEmpty then applied @ restBuilds
                else CurriedApply(applied.Head, args, typ, range) :: restBuilds
            // Note: this must come before recognizer for lambda with 3 and lambda with 2 parameters
            // Captures and correctly generates lambda in ChildLambdaProvider4
            | Lambda(
                arg = Ident.IdentIs ctx IdentType.Cont
                body =
                    TypeCast( expr =
                        Lambda( arg = p1;
                        body =
                            Lambda( arg = p2;
                            body =
                                Lambda( arg = p3;
                                body =
                                    Lambda( arg = p4;
                                    body = BuilderCollectorFeedback ctx expr
                                    )
                                )
                            )
                        )
                    )
                ) ->
                Delegate([ p1; p2; p3; p4], childSequence expr, None, []) :: restBuilds
            // Captures and correctly generates lambda in ChildLambdaProvider3
            | Lambda(
                Ident.IdentIs ctx IdentType.Cont,
                TypeCast(Lambda(p1, Lambda(p2, Lambda(p3, BuilderCollectorFeedback ctx expr, _), _), _), _),
                _
                ) ->
                Delegate([ p1; p2; p3], childSequence expr, None, []) :: restBuilds
            // This captures and correctly generates the lambda in the For and Index bindings
            | Lambda(
                Ident.IdentIs ctx IdentType.Cont,
                TypeCast(Lambda(item, Lambda(index, BuilderCollectorFeedback ctx expr, _), _), _),
                _
                ) ->
                Delegate([ item; index ], childSequence expr, None, []) :: restBuilds
            // Builder op
            | Lambda(
                Ident.IdentIs ctx (IdentType.Builder | IdentType.Yield | IdentType.Cont),
                BuilderCollectorFeedback ctx expr,
                _
                ) -> expr @ restBuilds
            // In the case of JSX (not tsx) the type cast is irrelevant for the string
            | TypeCast(Value(StringConstant value, _) as text, Unit)
            // The builder lambda's body is transformed first, which drops the type cast
            | (Value(StringConstant value, _) as text) ->
                 JsxUtils.TextChild(value, text) :: restBuilds
            // Ensure typecasts are transformed
            | TypeCast(BuilderCollectorFeedback ctx expr, _typ) ->
                expr @ restBuilds
            // Ensure conditionals are transformed
            | IfThenElse(
                BuilderCollectorFeedback ctx guardExpr,
                BuilderCollectorFeedback ctx thenExpr,
                BuilderCollectorFeedback ctx elseExpr,
                range) ->
                [ IfThenElse(
                    AstUtils.Sequential guardExpr,
                    childSequence thenExpr,
                    childSequence elseExpr,
                    range) ] @ restBuilds
            // lift any getters/setters
            | PropsGetterOrSetter ctx (BuilderCollectorFeedback ctx exprs) ->
                exprs @ restBuilds
            // | PropsGetterOrSetter ctx expr ->
                // match expr with
                // | Value(UnitConstant, None) | Value(Null(Any), None) -> restBuilds
                // | _ -> expr :: restBuilds
            // This is a prop getter in a builder, don't transform inside as it's already been done.
            | Get(IdentExpr({ IsThisArgument = true ; IsCompilerGenerated = true }), _, _, _) ->
                expr :: restBuilds
            // Every accessor (including union fields/tags) retains its kind, type and range.
            | Get(receiver, kind, typ, range) ->
                let kind = match kind with ExprGet index -> ExprGet(transform ctx index) | _ -> kind
                Get(transform ctx receiver, kind, typ, range) :: restBuilds
            // This is an artifact from the builder and is a noop anyway, so we can  dispose
            | Lambda(
                    { Name = name; IsCompilerGenerated = true },
                    TypeCast(IdentExpr({ Name = otherName; IsCompilerGenerated = true }), Unit),
                    None)
                when name = otherName ->
                restBuilds
            // Identifiers that are not one of our artifact build identifiers are allowed to be rendered
            // The rest will be disposed
            | IdentExpr(Ident.IdentIs ctx IdentType.Other) ->
                expr :: restBuilds
            // This is one of our builder identifiers; no reason it should be rendered.
            | IdentExpr _
            | Value(UnitConstant, None) -> restBuilds // You have been judged unworthy
            // `match` in a builder: a user or match binding, then a decision tree whose targets are children
            // Keep the binding's lexical scope and every child in its body.
            | Let(ident, value, BuilderCollectorFeedback ctx body) ->
                Let(ident, transform ctx value, childSequence body) :: restBuilds
            | DecisionTree(decisionTree, targets) ->
                DecisionTree(
                    transform ctx decisionTree,
                    targets |> List.map (fun (idents, BuilderCollectorFeedback ctx target) -> idents, childSequence target)
                ) :: restBuilds
            // Trust the F# compiler to not allow invalid elements within the builder.
            | _ ->
                expr :: restBuilds

    // TODO - correct documentation and refactor since TagInfo is refactored out
    /// <summary>
    /// Collates TagInfos into ElementBuilders which can then be rendered via `renderElement`<br/>
    /// Performs final clean up of properties by trimming reserved identifiers of the attribute keys
    /// </summary>
    let collectTagInfo (_ctx: PluginContext): ElementBuilder -> ElementBuilder = function
        | { Properties = props } as eleBuilder ->
            {
                eleBuilder with
                    Properties =
                        props
                        |> List.map (fun (prop,expr) -> (prop |> StringUtils.TrimReservedIdentifiers, expr) )
            }
    module SpecialAttributeTransformation =
        let (|Pojo|_|) (ctx: PluginContext): Expr -> Expr option = function
            // If we find an intermediary binding to a constructor of a pojo, the following will be
            // related expressions which are going to be other members.
            // We emit the property setters. For the moment, we are disposing other members, as I do not
            // see what situation they would be utilised.
            // We will capture them and compound them into a single expression
            | Let(
                Ident.IdentIs ctx IdentType.ReturnVal
                & Ident.HasPojo ctx,
                Call(_, CallInfo.PojoConstructorArgs ctx args, _, range),
                PropCollectorFeeder ctx props) as original ->
                let optimisedExpr =
                    let keys,values = args @ props |> List.unzip
                    Value(
                        ValueKind.NewAnonymousRecord(
                            values = (values |> List.map (transform ctx)),
                            fieldNames = (keys |> List.toArray),
                            genArgs = (keys |> List.map (fun _ -> Any)),
                            isStruct = false
                            ),
                        range
                        )
                    |> Some
                if ctx |> PluginContext.hasFlag ComponentFlag.SkipPojoOptimisation
                then original |> Some
                else optimisedExpr

            | _ -> None
    let (|SpecialAttributeTransformation|_|) (ctx: PluginContext): Expr -> Expr option = function
        | SpecialAttributeTransformation.Pojo ctx expr -> Some expr
        | _ -> None
    /// <summary>
    /// Transforms a value assigned to a prop or attribute. Unlike <c>transform</c>, a lone
    /// <c>fun x -> ()</c> (F# <c>ignore</c>) is kept: it has the same shape as the builder 'husk'
    /// that <c>transform</c> reduces out, but here it is the user's value.
    /// </summary>
    let transformValue (ctx: PluginContext) (expr: Expr) =
        let expr = unrollValue ctx expr
        match expr with
        | Lambda(
                { Name = name; IsCompilerGenerated = true },
                TypeCast(IdentExpr({ Name = otherName; IsCompilerGenerated = true }), Unit),
                None)
            when name = otherName -> expr
        | _ -> transform ctx expr
    /// <summary>
    /// First pass transformations of all expressions.<br/>
    /// Reduce AST where reasonable; capture and expand tags where encountered; dispose only in exceptional circumstances.<br/>
    /// </summary>
    /// <param name="ctx">PluginContext</param>
    /// <param name="expr">Expr node to transform</param>
    /// <remarks>
    /// Much of this transformation has to do with picking up
    /// top level expressions that contain nested expressions, and
    /// then passing those nested expressions through the transformation
    /// before assembling them back into the nested structure.
    /// </remarks>
    let transform (ctx: PluginContext) (expr: Expr)=
        match expr with
        | ReactiveMatch ctx expr -> expr
        // Fable sugars like `&&=` and `??=` and others use Emit expressions. Their arguments must be transformed before
        // they are injected into the Emit.
        | Emit({ CallInfo = { Args = exprs } as callInfo } as emitInfo, typ, range) ->
            let transformedExprs = exprs |> List.map (transform ctx)
            Emit({ emitInfo with CallInfo = { callInfo with Args = transformedExprs } }, typ, range)
        // TagValues are treated specially.
        | TagValue.TagValue ctx expr -> expr
        | TagValue.TagRender ctx expr -> expr
        // Expression pattern that prevents/alters the transformation
        // of some special attributes expressions which are transformed by fable
        | SpecialAttributeTransformation ctx expr -> expr
        // Expression pattern that matches a JSX tag expression
        | TagConstructor ctx tagInfo ->
            tagInfo |> collectTagInfo ctx |> Baked.renderElement ctx
        // Transform sequential expressions
        | Sequential exprs ->
            exprs
            |> List.map (transform ctx)
            |> Sequential
        | UncurriedPropsApply ctx expr -> expr
        // Expression match and transform any `props` accesses or assignments
        | PropsGetterOrSetter ctx expr ->
            expr
        // A `[<SolidComponent>]` let binding called for its element. It takes positional arguments, so it
        // cannot be a tag; run its body untracked instead, as `createComponent` does for `<Comp/>`.
        // The arguments are bound outside the untrack: a signal read in one must still re-run the call.
        | Call(callee, ({ ThisArg = None; MemberRef = Some(MemberRef(_, { AttributeFullNames = attrs })) } as callInfo), typ, _)
            when attrs |> List.contains "Partas.Solid.SolidComponentAttribute"
                 && Type.isElement ctx typ ->
            Baked.untrackedCall (transform ctx callee) (callInfo.Args |> List.map (transform ctx)) typ
        // Transform calls that are getters
        | Call(
            callee = Expr.ImportedGetter ctx
            info = {
                ThisArg = Some(ident)
                MemberRef = MemberRef.Option.PartasName ctx prop
            } & { MemberRef =Some(MemberRef.MemberRefIs ctx MemberRefType.Getter) })
         ->
            AstUtils.GetPropField(ident, prop)

        // Transform branch expressions
        | IfThenElse(guardExpr, thenExpr, elseExpr, range) ->
            IfThenElse(
                transform ctx guardExpr,
                transform ctx thenExpr,
                transform ctx elseExpr,
                range )
        // Transform branch expressions
        | DecisionTree(decisionTree, targets) ->
            DecisionTree(
                transform ctx decisionTree,
                targets
                |> List.map (fun (target, expr) -> target, transform ctx expr)
                )
        | DecisionTreeSuccess(index, boundValues, typ) ->
            DecisionTreeSuccess(index, boundValues |> List.map (transform ctx), typ)
        // Within a computation expression, property assignment for `props` is transformed out. In cases where a null
        // lambda 'husk' remains, we reduce to a null expression.
        // F# `ignore` has the same shape; values assigned to props/attributes go through `transformValue` to keep it.
        | Lambda(
                { Name = name; IsCompilerGenerated = true },
                TypeCast(IdentExpr({ Name = otherName; IsCompilerGenerated = true }), Unit),
                None)
            when name = otherName -> AstUtils.Unit
        // Transform the contents of TypeCasts
        | TypeCast(expr, _) -> transform ctx expr
        // Attribute expression pairs are invalid on first pass, as they should be collected within tag expressions.
        | AttributeExpression ctx propInfo ->
            PluginContext.logWarning ctx $"Attribute expression was parsed at the top level and was reduced out:\n{propInfo}"
            snd propInfo
        // Transform nested expression structure
        | CurriedApply(applied, exprs, typ, range) ->
            CurriedApply(transform ctx applied, exprs |> List.map (transform ctx), typ, range )
        // Transform nested expression structure
        | Delegate(args, body, name, tags) ->
            Delegate(args, transform ctx body, name, tags)
        // Transform nested expression structure
        | Lambda(ident, expr, name) ->
            Lambda(ident, transform ctx expr, name)
        // Transform nested expression structure
        | Let(ident, value, body) -> // transform other lets
            Let(ident, transform ctx value, transform ctx body)
        // Transform nested expression structure
        | Call(callee, ({ ThisArg = Some ident } as callInfo), typ, range) -> // transform calls
            Call(callee |> transform ctx, { callInfo with ThisArg = Some (transform ctx ident); Args = callInfo.Args |> List.map (transform ctx) }, typ, range)
        // Transform nested expression structure
        | Call(callee, callInfo, typ, range) -> // transform calls
            Call(callee |> transform ctx, { callInfo with Args = callInfo.Args |> List.map (transform ctx) }, typ, range)
        // Transform nested values
        | Value(
                (
                 NewAnonymousRecord _
                | NewArray _
                | NewList(headAndTail = Some _)
                | NewRecord _
                | StringTemplate _
                | NewOption(value = Some _)
                | NewTuple _
                | NewUnion _
                ) as valueKind,
                range
            ) ->
            let transformValues = List.map (transform ctx)
            Value(
                match valueKind with
                | NewUnion(values, tag, ref, genArgs) ->
                    NewUnion(transformValues values, tag, ref, genArgs)
                | NewTuple(values, isStruct) ->
                    NewTuple(transformValues values, isStruct)
                | StringTemplate(tag, parts, values) ->
                    StringTemplate(tag |> Option.map (transform ctx), parts, transformValues values)
                | NewOption(Some expr, typ, isStruct) ->
                    NewOption(Some (transform ctx expr), typ, isStruct)
                | NewArray(ArrayValues values, typ, kind) ->
                    NewArray(ArrayValues (transformValues values), typ, kind)
                | NewArray(ArrayAlloc expr, typ, kind) ->
                    NewArray(ArrayAlloc (transform ctx expr), typ, kind)
                | NewArray(ArrayFrom expr, typ, kind) ->
                    NewArray(ArrayFrom (transform ctx expr), typ, kind)
                | NewList(Some(expr1, expr2), typ) ->
                    NewList(Some(transform ctx expr1, transform ctx expr2), typ)
                | NewRecord(values, ref, genArgs) ->
                    NewRecord(transformValues values, ref, genArgs)
                | NewAnonymousRecord(values, fieldNames, genArgs, isStruct) ->
                    NewAnonymousRecord(transformValues values, fieldNames, genArgs, isStruct)
                | _ -> failwith "unreachable"
                , range
            )
        // Transform nested expressions
        | Operation(kind, tags, typ, range) ->
            Operation( // transform operations
                match kind with
                | Unary(operator, operand) -> Unary(operator, operand |> transform ctx)
                | Binary(operator, left, right) -> Binary(operator, left |> transform ctx, transform ctx right)
                | Logical(operator, left, right) -> Logical(operator, left |> transform ctx, transform ctx right)
                ,
                tags,
                typ,
                range
            )
        // Capture and include index access
        | Get(expr, ExprGet(getExpr) & ExprGet(Call _), _typ, _range) ->
            match transform ctx getExpr with
            | Value(UnitConstant, None) | Value(Null(Any), None) -> transform ctx expr
            | getExpr ->
                // PluginContext.debugDisposal ctx "Transform of Get ExprGet" getExpr
                AstUtils.GetProp(transform ctx expr, getExpr)
        // Transform nested expressions
        | Get(expr1, kind, typ, range) -> //transform inside Get expressions
            Get(
                transform ctx expr1,
                match kind with
                | ExprGet expr -> ExprGet (transform ctx expr)
                | _ -> kind
                , typ, range
                )
        // Transform nested expressions
        | ObjectExpr(exprMembers, typ, exprOption) -> // transform inside Object expr
            [                                         // eg: jsOptions<SomeType> ( ... )
                for memb in exprMembers do
                    { memb with
                        Body = memb.Body |> transform ctx }
            ]
            |> fun exprMembers ->
                ObjectExpr(exprMembers, typ, exprOption |> Option.map (transform ctx))
        // Transform nested expressions: `props.opt.IsSome`, `while` guards, `failwith` messages etc. can read props
        | Test(expr, kind, range) ->
            Test(transform ctx expr, kind, range)
        | WhileLoop(guard, body, range) ->
            WhileLoop(transform ctx guard, transform ctx body, range)
        | ForLoop(ident, start, limit, body, isUp, range) ->
            ForLoop(ident, transform ctx start, transform ctx limit, transform ctx body, isUp, range)
        | TryCatch(body, catch, finalizer, range) ->
            TryCatch(
                transform ctx body,
                catch |> Option.map (fun (ident, expr) -> ident, transform ctx expr),
                finalizer |> Option.map (transform ctx),
                range )
        | Extended(Throw(Some expr, typ), range) ->
            Extended(Throw(Some (transform ctx expr), typ), range)
        | Extended(Curry(expr, arity), range) ->
            Extended(Curry(transform ctx expr, arity), range)
        | LetRec(bindings, body) ->
            LetRec(bindings |> List.map (fun (ident, expr) -> ident, transform ctx expr), transform ctx body)
        | Set(expr, kind, typ, value, range) ->
            Set(
                transform ctx expr,
                (match kind with
                 | ExprSet expr -> ExprSet (transform ctx expr)
                 | _ -> kind),
                typ,
                transform ctx value,
                range )
        // No further first pass transformations applicable
        | expr -> expr

    /// Plugin support for extending Polymorphic attributes
    module Polymorphism =
        let (|PolymorphicAttribute|_|) (ctx: PluginContext) : string -> string option = function
            | "as'" -> Some "as"
            | "asChild" -> Some "asChild"
            | Utils.StartsWithTrimmed "__PARTAS_POLYMORPHIC__" attrName -> Some (attrName |> StringUtils.TrimReservedIdentifiers)
            | _ -> None
    /// Plugin support for the TagValue magics
    module TagValue =
        module EntityRef =
            let (|TagValue|_|) = function
                | { FullName = "Partas.Solid.Builder.TagValue" } -> true
                | _ -> false
        module ImportInfo =
            let (|TagValue|_|) = function
                | { Kind = MemberImport(MemberRef(EntityRef.TagValue, _)) } -> true
                | _ -> false
            let (|Render|_|) = function
                | { Kind = MemberImport(MemberRef(EntityRef.TagValue, { CompiledName = "render" })) }
                    -> true
                | _ -> false

        /// Will retrieve the wrapped type entity ref if it is a valid TagValue type
        let (|GetType|_|): Type -> bool = function
            | DeclaredType(EntityRef.TagValue, _)
            | LambdaType(GetType, _ ) -> true
            | _ -> false
        let (|CollectProperties|_|) (ctx: PluginContext) = function
            | [ TypeCast(Value(NewAnonymousRecord(values, fields, _, _), _), _) ] ->
                values
                |> List.zip (fields |> Array.toList)
                |> List.map (fun (str,expr) ->
                    expr
                    |> transform ctx
                    |> fun expr ->
                        str |> StringUtils.TrimReservedIdentifiers,
                        expr)
                |> Some
            // `render ()`
            | [] | [ Value(UnitConstant, _) ] -> Some []
            | _ -> None
        /// A component rendered as a tag must compile to a capitalised name: Solid's compiler treats
        /// `<router />` as an HTML element. A lowercase value, or one in a lowercase nested module (whose
        /// compiled name is prefixed with the module's), is a compile error rather than a silent no-op.
        let guardComponentName (ctx: PluginContext) (range: SourceLocation option) (expr: Expr) =
            let name =
                match expr with
                | IdentExpr { Name = name } -> Some name
                | Import({ Selector = name }, _, _) when name <> "default" && name <> "*" -> Some name
                | _ -> None
            match name with
            | Some name when name.Length > 0 && System.Char.IsLower name[0] ->
                ctx.Helper.LogError(
                    $"`{name}` is rendered as a component, but it would compile to <{name} />, which Solid treats as "
                    + "an HTML element. Give it a capitalised name, in a capitalised module.",
                    ?range = Option.orElse range expr.Range)
            | _ -> ()
            expr

        let (|TagValue|_|) (ctx: PluginContext): Expr -> Expr option = function
            | Call(
                callee = Import({ Selector = "op_BangAt"; Kind = MemberImport(MemberRef({ FullName = "Partas.Solid.Builder" }, _)) }, _, _)
                info = {
                    Args = Lambda(body = Call(callee = callee; info = callInfo)) :: _
                }
                range = bangRange) ->
                match callee with
                // Some ident
                | IdentExpr({ Type = Type.PartasName ctx typeName } as identee) ->
                    match callInfo with
                    // In the case where the referenced tag is a constructor, then it is a SolidTypeComponent, and we
                    // actually intend to reference the name of the type.
                    | CallInfo.Constructor ctx ->
                        IdentExpr({ identee with Name = typeName |> StringUtils.TrimReservedIdentifiers }) |> Some
                    // Where the above is not true, the referenced tag is a let binding, and therefore a SolidComponent,
                    // so we intent to reference the name of the binding.
                    | _ ->
                        callee |> guardComponentName ctx bangRange |> Some
                // prevent import of native tags
                | Expr.NativeImportedConstructor ctx
                    & Import(
                        typ = Type.PartasName ctx typeName
                        range = _
                    ) ->
                    AstUtils.IdentExpr(typeName)
                    |> Some
                // an imported tag
                | Import(
                    importInfo,
                    typ & Type.PartasName ctx typeName,
                    range) as expr->
                    // We determine if the imported tag was a constructor. If it was, then it is a SolidTypeComponent,
                    // and the name of the component will be the name of the type.
                    match callInfo with
                    | CallInfo.Constructor ctx ->
                        Import({ importInfo with Selector = typeName }, typ, range)
                        |> Some
                    // If the above is not true, then the tag is a SolidComponent let binding, and the name of the
                    // component will be the name of the binding.
                    | _ ->
                        expr |> guardComponentName ctx bangRange |> Some
                | Import(info = { Kind = UserImport false }) ->
                    Some callee
                | _ -> None
            | TypeCast(TagValue ctx expr, _) -> expr |> Some
            | _ -> None

        let (|TagRender|_|) (ctx: PluginContext): Expr -> Expr option =
            let rec (|UnrollTypeCast|) = function
                | TypeCast(UnrollTypeCast expr, _) -> expr
                | expr -> expr
            function
            // A call to render a tagvalue with a constructor
            | Call(
                callee = Import(info = ImportInfo.Render)
                info = {
                    ThisArg = thisArg // contains info on the left expr of render
                    Args = (
                        UnrollTypeCast(TagConstructor ctx tagInfo) :: BuilderCollector ctx rest
                    )
                }) ->
                { tagInfo with Children = rest @ tagInfo.Children }
                |> collectTagInfo ctx
                |> fun eleBuilder ->
                    match thisArg with
                    // A single-use local `let Wrapper = !@Pill` is inlined by Fable, leaving the `!@` call here
                    | Some(TagValue ctx tag) ->
                        { eleBuilder with TagSource = TagSource.LibraryImport tag }
                    | Some(PropsGetterOrSetter ctx expr) // Make sure we capture getters
                    | Some expr ->
                        { eleBuilder with TagSource = TagSource.LibraryImport expr }
                    | _ -> eleBuilder
                |> Baked.renderElement ctx |> Some
            // a call to render a tagvalue with an anon record
            // todo - support key,value pair list
            | Call(
                callee = Import(ImportInfo.Render, _, _)
                info = {
                    ThisArg = thisArg
                    Args = CollectProperties ctx props
                }
                range = range)
             ->
                let importExpr =
                    match thisArg with
                    | Some(TagValue ctx tag) ->
                        tag
                    | Some(PropsGetterOrSetter ctx expr) ->
                        expr
                    | Some(IdentExpr _ as ident) ->
                        guardComponentName ctx range ident
                    | Some expr -> guardComponentName ctx range expr
                    | _ ->
                        failwith "unreachable"
                { TagSource = TagSource.LibraryImport importExpr
                  Properties = props
                  Children = []
                  Range = range }
                |> Baked.renderElement ctx
                |> Some
            | _ -> None

[<AutoOpen>]
module internal SolidComponentFlagsExtensions =
    let debug (memberDecl: MemberDecl) =
        Console.WriteLine "\nSTART MEMBER DECL!!!"
        Console.WriteLine memberDecl.Body
        Console.WriteLine "END MEMBER DECL!!!\n"

type SolidTypeComponentAttribute(flag: int) =
    inherit MemberDeclarationPluginAttribute()
    let flags = enum<ComponentFlag> flag
    override _.FableMinimumVersion = FableRequirements.version
    override this.Transform(pluginHelper, _, memberDecl) =
        let ctx = PluginContext.create pluginHelper TransformationKind.TypeMemberDecl flags
        if ctx.HasFlag ComponentFlag.DebugMode then debug memberDecl
        match memberDecl with
        // Check that the memberDecl has the correct self identifier of `props`
        | SchemaRules.ValidMemberRef ctx info ->
            PluginContext.setSelfIdentifier ctx info.SelfIdentifier
            let newExpr =
                memberDecl.Body
                |> AST.transform ctx // initiate transformation
                |> ReactiveMatch.placeSelections
                // Create and append the omit expression
                |> if ctx.HasFlag(ComponentFlag.SkipOmit) then id else Baked.convertGettersToObject ctx.SelfIdentifier (PluginContext.getGetters ctx |> List.distinct)
                // Create and append the merge expression if we have any setters
                |> Baked.convertSettersToObject ctx.SelfIdentifier (PluginContext.getSetters ctx)
            { memberDecl with Body = newExpr; Name = info.DisplayName }
        | _ ->
            "Unsupported definitions under this attribute are not recommended. Please use `[<SolidComponent>]` instead"
            |> PluginContext.logWarning ctx
            {
                memberDecl with
                    Body = memberDecl.Body
                           |> AST.transform ctx
                           |> ReactiveMatch.placeSelections
            }
    override this.TransformCall(_, _, expr) =
        expr

    new() = SolidTypeComponentAttribute(int ComponentFlag.Default)
    new(compileOptions: ComponentFlag) = SolidTypeComponentAttribute(int compileOptions)

/// Applies Solid transformations to `let` bindings.
/// Use SolidTypeComponentAttribute for Type member definitions such
/// as `member props.typeDef =`.
type SolidComponentAttribute(flag: int) =
    inherit MemberDeclarationPluginAttribute()
    let flags = enum<ComponentFlag> flag
    override _.FableMinimumVersion = FableRequirements.version
    override this.Transform(pluginHelper, _, memberDecl) =
        let ctx = PluginContext.create pluginHelper TransformationKind.MemberDecl flags
        if ctx.HasFlag ComponentFlag.DebugMode then debug memberDecl
        {
            memberDecl with
                Body = memberDecl.Body |> AST.transform ctx |> ReactiveMatch.placeSelections
        }
    override this.TransformCall(_, _, expr) =
        expr

    new() = SolidComponentAttribute(int ComponentFlag.Default)
    new(compileOptions: ComponentFlag) = SolidComponentAttribute(int compileOptions)
