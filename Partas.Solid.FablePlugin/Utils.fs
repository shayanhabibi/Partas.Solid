namespace Fable.AST.Fable

open Fable
open Fable.AST

/// Internal members for AstUtils and JsxUtils
type AstUtilHelpers =
    static member any = Type.Any
    static member range = None

open type AstUtilHelpers

type AstUtils =

    /// Creates a unit constant expression
    static member inline Unit = Expr.Value (ValueKind.UnitConstant, range)
    /// Creates a null constant expression
    static member inline Null = Expr.Value (ValueKind.Null any, range)

    /// Creates a string constant expression
    static member inline Value(stringValue: string) =
        Expr.Value (ValueKind.StringConstant stringValue, range)

    /// Creates a boolean constant expression
    static member inline Value(boolValue: bool) =
        Expr.Value (ValueKind.BoolConstant boolValue, range)

    /// creates an integer constant expression
    static member inline Value(intValue: int) =
        Expr.Value (ValueKind.NumberConstant (NumberValue.Int32 intValue, NumberInfo.Empty), range)

    /// creates a float constant expression
    static member inline Value(floatValue: float) =
        Expr.Value (ValueKind.NumberConstant (NumberValue.Float64 floatValue, NumberInfo.Empty), range)

    static member inline ValueArray(exprs: Expr list) =
        Expr.Value (ValueKind.NewArray (NewArrayKind.ArrayValues (exprs), any, ArrayKind.MutableArray), range)

    /// Combines a variable parameter array of expressions into a sequential expression
    static member inline Sequential([<System.ParamArray>] nodes: Expr array) : Expr =
        nodes
        |> Array.toList
        |> Sequential

    /// Will check if expr list is either empty, and emit a null; a single expr long, and emit that expr; else
    /// wraps the expressions in a Sequential DU
    static member inline Sequential(exprList: Expr list) : Expr =
        match exprList with
        | [] -> AstUtils.Unit
        | [ expr ] -> expr
        | _ -> Sequential exprList

    /// Creates a user import expression with the selector and path
    static member inline Import(selector: string, path: string) =
        Expr.Import (
            { Selector = selector
              Path = path
              Kind = ImportKind.UserImport false },
            any,
            range
        )

    /// Creates a ImportKind.MemberImport expression
    static member inline Import(selector: string, path: string, memberInfo: MemberRef) =
        Expr.Import (
            { Selector = selector
              Path = path
              Kind = ImportKind.MemberImport memberInfo },
            any,
            range
        )

    /// Creates a ImportKind.ClassImport expression
    static member inline Import(selector: string, path: string, classInfo: EntityRef) =
        Expr.Import (
            { Selector = selector
              Path = path
              Kind = ImportKind.ClassImport classInfo },
            any,
            range
        )

    static member inline Call(callee: Expr, info: CallInfo, ?typ: Type, ?range: SourceLocation) =
        let typ = defaultArg typ any
        Expr.Call (callee, info, typ, range)


    /// Creates an anonymous record from the list of string (field) expr (value) tuples
    static member inline Object(pairs: (string * Expr) list) =
        Expr.Value (
            ValueKind.NewAnonymousRecord (
                List.map snd pairs,
                List.map fst pairs
                |> List.toArray,
                List.map (fun _ -> any) pairs,
                false
            ),
            range
        )

    /// Creates an ident expr with the given name
    static member inline IdentExpr(name: string, ?isThisArg: bool, ?isMutable: bool, ?isCompilerGenerated: bool) =
        let isMutable, isCompilerGenerated, isThisArg =
            defaultArg isMutable true, defaultArg isCompilerGenerated true, defaultArg isThisArg false

        Expr.IdentExpr (
            { Type = any
              IsCompilerGenerated = isCompilerGenerated
              IsMutable = isMutable
              IsThisArgument = isThisArg
              Name = name
              Range = range }
        )

    /// Creates an emit statement macro function with the given macro
    static member inline EmitStatement(macro: string) =
        fun callInfo ->
            Expr.Emit (
                { CallInfo = callInfo
                  IsStatement = true
                  Macro = macro },
                any,
                range
            )

    /// Creates an emit statement macro with the given macro and args
    static member inline EmitStatement(macro, callInfo) =
        callInfo
        |> AstUtils.EmitStatement (macro)

    /// Creates an emit macro function with the given macro
    static member inline Emit(macro: string) =
        fun callInfo ->
            Expr.Emit (
                { CallInfo = callInfo
                  IsStatement = false
                  Macro = macro },
                any,
                range
            )

    /// Creates an emit macro expression with the given macro and args
    static member inline Emit(macro, callInfo) =
        callInfo
        |> AstUtils.Emit (macro)

    /// Creates a JS expression
    static member inline EmitPure(js: string, ?isStatement: bool) =
        let isStatement = defaultArg isStatement false

        Expr.Emit (
            { CallInfo =
                { Args = []
                  GenericArgs = []
                  MemberRef = None
                  SignatureArgTypes = []
                  Tags = []
                  ThisArg = None }
              IsStatement = isStatement
              Macro = js },
            any,
            range
        )

    /// Creates a JS statement expression
    static member inline EmitPureStatement(macro) =
        AstUtils.EmitPure (macro, true)

    /// Creates a field info object with an option to prevent beta reduction by setting the hasSideEffects flag
    static member inline FieldInfo(name: string, ?hasSideEffects: bool) =
        let hasSideEffects = defaultArg hasSideEffects true

        { Name = name
          IsMutable = hasSideEffects
          Tags = []
          FieldType = None
          MaybeCalculated = hasSideEffects }

    /// Creates a property getter expression with as `ident.field`
    static member inline GetProp(ident: string, field: string) =
        Expr.Get (AstUtils.IdentExpr (ident, true, false, true), GetKind.ExprGet (AstUtils.Value (field)), any, range)

    /// Creates a property getter expression with `ident.field`
    static member inline GetProp(identExpr: Ident, field: string) =
        Expr.Get (
            Expr.IdentExpr
                { identExpr with
                    IsThisArgument = true
                    IsCompilerGenerated = true },
            GetKind.ExprGet (AstUtils.Value (field)),
            any,
            range
        )

    static member inline GetProp(identExpr: Expr, field: string) =
        Expr.Get (identExpr, GetKind.ExprGet (AstUtils.Value (field)), any, range)

    static member inline GetProp(identExpr: Expr, field: Expr, ?range: SourceLocation) =
        Expr.Get (identExpr, GetKind.ExprGet field, any, range)

    static member inline GetPropField(identExpr: Expr, field: string) =
        Expr.Get (identExpr, GetKind.FieldGet (AstUtils.FieldInfo (field)), any, range)

    static member inline SetProp(identExpr: Expr, value: Expr, ?kind: SetKind, ?typ: Type, ?range: SourceLocation) =
        let kind = defaultArg kind SetKind.ValueSet
        let typ = defaultArg typ any
        Expr.Set (identExpr, kind, typ, value, range)

    /// Flattens a list of expressions into a list value expression
    static member inline Flatten(exprs: Expr list) =
        let listItemType = Type.Tuple ([ Type.String; Type.Any ], isStruct = false)

        let emptyList =
            Value (kind = NewList (headAndTail = None, typ = listItemType), range = None)

        (emptyList, exprs)
        ||> List.fold (fun acc prop -> Value (kind = NewList (headAndTail = Some (prop, acc), typ = listItemType), range = range))

    static member inline Ident(name: string, ?isThisArg: bool, ?typ: Type, ?isMutable: bool, ?isCompilerGenerated: bool, ?range: SourceLocation) =
        let typ = defaultArg typ any
        let isThisArg = defaultArg isThisArg false
        let isMutable = defaultArg isMutable false
        let isCompilerGenerated = defaultArg isCompilerGenerated true

        { Name = name
          Type = typ
          IsCompilerGenerated = isCompilerGenerated
          IsMutable = isMutable
          IsThisArgument = isThisArg
          Range = range }

    static member inline CallInfo
        (?thisArg: Expr, ?args: Expr list, ?genArgs: Type list, ?sigArgTypes: Type list, ?memberRef: MemberRef, ?isCons: bool, ?tag: string)
        =
        CallInfo.Create (
            ?thisArg = thisArg,
            ?args = args,
            ?genArgs = genArgs,
            ?sigArgTypes = sigArgTypes,
            ?memberRef = memberRef,
            ?isCons = isCons,
            ?tag = tag
        )

    static member inline TypeCast(expr: Expr, ?typ: Type) =
        TypeCast (
            expr,
            typ
            |> Option.defaultValue any
        )

type JsxUtils =
    /// The fable jsx element type pre-referenced
    static member inline ElementType =
        Type.DeclaredType (
            { FullName = "Fable.Core.JSX.Element"
              Path = EntityPath.CoreAssemblyName "Fable.Core" },
            []
        )

    /// The fable jsx element create func pre-referenced
    static member inline CreateElementImportExpr =
        AstUtils.Import ("create", "@fable-org/fable-library-js/JSX.js")

    /// Creates a keyvalue tuple from the given string and expression
    static member inline KeyValue(key: string, valueExpr: Expr) =
        Expr.Value (ValueKind.NewTuple ([ AstUtils.Value (key); valueExpr ], false), range)

    /// Creates a keyvalue tuple from the given string and expression
    static member inline KeyValue(key: string, value: string) =
        JsxUtils.KeyValue (key, AstUtils.Value value)

    /// Creates a keyvalue tuple from the given string and expression
    static member inline KeyValue(key: string, value: int) =
        JsxUtils.KeyValue (key, AstUtils.Value value)

    /// Creates keyvalue tuple from the given string and expression
    static member inline KeyValue(key: string, value: float) =
        JsxUtils.KeyValue (key, AstUtils.Value value)

    /// Creates a property tuple (type casting the value expr?)
    static member inline Prop(key: string, valueExpr: Expr) =
        JsxUtils.KeyValue (key, TypeCast (valueExpr, any))

    static member inline Prop(key: string, value: string) =
        JsxUtils.Prop (key, AstUtils.Value value)

    static member inline Prop(key: string, value: int) =
        JsxUtils.Prop (key, AstUtils.Value value)

    static member inline Prop(key: string, value: float) =
        JsxUtils.Prop (key, AstUtils.Value value)

    static member inline Prop(key: string, value: bool) =
        JsxUtils.Prop (key, AstUtils.Value value)

    /// Creates a child prop from the given list of expressions
    static member inline ChildrenProp(children: Expr list) =
        JsxUtils.Prop ("children", AstUtils.Flatten (children))

    /// Creates a JSX element with the given tag name expr, properties, and children
    static member inline CreateElement(tagNameExpr: Expr, properties: (string * Expr) list, children: Expr list) =
        let internalCollection =
            JsxUtils.ChildrenProp (children)
            :: (properties
                |> List.map JsxUtils.Prop)
            |> AstUtils.Flatten

        Call (
            JsxUtils.CreateElementImportExpr,
            { ThisArg = None
              Args = [ tagNameExpr; internalCollection ]
              SignatureArgTypes = []
              GenericArgs = []
              MemberRef = None
              Tags = [ "jsx" ] },
            JsxUtils.ElementType,
            range
        )

    /// Creates a JSX element with the given tag name, properties, and children
    static member inline CreateElement(tagName: string, properties: (string * Expr) list, children: Expr list) =
        JsxUtils.CreateElement (AstUtils.Value (tagName), properties, children)

module Patterns =
    /// Matches strings that start with the provided string
    let (|StartsWith|_|) (value: string) =
        function
        | (s: string) when s.StartsWith (value) -> Some ()
        | _ -> None

    /// Matches string that end with the provided string
    let (|EndsWith|_|) (value: string) =
        function
        | (s: string) when s.EndsWith (value) -> Some ()
        | _ -> None

    /// Matches strings that start with the provided string, and returns the string
    /// with the match trimmed
    let (|StartsWithTrimmed|_|) (value: string) =
        function
        | StartsWith value as s ->
            s.Substring (value.Length)
            |> Some
        | _ -> None

    /// Matches strings that end with the provided string, and returns the string
    /// with the match trimmed
    let (|EndsWithTrimmed|_|) (value: string) =
        function
        | EndsWith value as s ->
            s.Substring (
                0,
                s.Length
                - value.Length
            )
            |> Some
        | _ -> None

    /// Recursively explores an expression tree looking for the first expression that is not a TypeCast.
    let rec (|ExprTypeCastDrill|): Expr -> Expr =
        function
        | TypeCast (ExprTypeCastDrill expr, _)
        | expr -> expr

    /// Digs through core wrappers till it exposes a declared type if one is available (none otherwise)
    let rec (|GetDeclaredType|_|): Type -> Type option =
        function
        | Type.LambdaType (_, GetDeclaredType typ)
        | Type.Tuple (GetDeclaredType typ :: _, _)
        | Type.Option (GetDeclaredType typ, _)
        | Type.List (GetDeclaredType typ)
        | Type.Array (GetDeclaredType typ, _) -> Some typ
        | Type.DeclaredType (_) as typ -> Some typ
        | _ -> None

    /// Matches an entityref that has the given attribute full name. This requires the PluginHelper to be
    /// passed to the pattern.
    let (|EntityRefHasAttribute|_|) (pluginHelper: PluginHelper) (attrName: string) : EntityRef -> Attribute option =
        pluginHelper.GetEntity
        >> _.Attributes
        >> Seq.tryFind (
            _.Entity
            >> _.FullName
            >> (=) attrName
        )

    /// Matches a type that has the given attribute full name. This requires the PluginHelper to be passed to the pattern
    let (|TypeHasAttribute|_|) (pluginHelper: PluginHelper) (attrName: string) : Type -> Attribute option =
        function
        | GetDeclaredType (Type.DeclaredType (EntityRefHasAttribute pluginHelper attrName attr, _)) -> Some attr
        | _ -> None


module Expr =
    let rec findAndDiscardElse (predicate: Expr -> bool) : Expr -> Expr list =
        let filterList (values: Expr list) : Expr list =
            List.collect (findAndDiscardElse predicate) values

        function
        | expr when predicate expr -> [ expr ]
        | Expr.Call (callee = expr; info = { Args = exprs }) ->
            expr
            :: exprs
            |> filterList
        | Expr.CurriedApply (applied = expr; args = exprs) ->
            expr
            :: exprs
            |> filterList
        | Expr.DecisionTree (expr = expr; targets = targets) ->
            expr
            :: List.map snd targets
            |> filterList
        | Expr.DecisionTreeSuccess (boundValues = exprs) -> filterList exprs
        | Expr.Delegate (body = expr) -> findAndDiscardElse predicate expr
        | Expr.Emit (info = { CallInfo = { Args = exprs } }) -> filterList exprs
        | Expr.ForLoop (start = start; body = body; limit = limit) ->
            [ start; body; limit ]
            |> filterList
        | Expr.Get (expr = expr; kind = getKind) ->
            let maybeResult = findAndDiscardElse predicate expr

            if
                maybeResult.IsEmpty
                |> not
            then
                maybeResult
            else
                match getKind with
                | ExprGet expr -> findAndDiscardElse predicate expr
                | _ -> maybeResult
        | Expr.IfThenElse (guardExpr = guardExpr; elseExpr = elseExpr; thenExpr = thenExpr) ->
            [ guardExpr; elseExpr; thenExpr ]
            |> filterList
        | Expr.Lambda (body = expr) -> findAndDiscardElse predicate expr
        | Expr.Let (body = body; value = value) -> filterList [ body; value ]
        | Expr.LetRec (bindings = bindings; body = body) ->
            body
            :: List.map snd bindings
            |> filterList
        | Expr.ObjectExpr (baseCall = exprMaybe; members = members) ->
            members
            |> List.map _.Body
            |> List.append
                [ if exprMaybe.IsSome then
                      exprMaybe.Value ]
            |> filterList
        | Expr.Operation (kind = OperationKind.Binary (left = left; right = right))
        | Expr.Operation (kind = OperationKind.Logical (left = left; right = right)) ->
            [ left; right ]
            |> filterList
        | Expr.Operation (kind = OperationKind.Unary (operand = expr)) -> findAndDiscardElse predicate expr
        | Expr.Sequential exprs -> filterList exprs
        | Expr.Set (expr = expr; value = value; kind = kind) ->
            match kind with
            | ExprSet exprSet ->
                [ expr; value; exprSet ]
                |> filterList
            | _ ->
                [ expr; value ]
                |> filterList
        | Expr.TryCatch (body = expr; catch = catch; finalizer = finalizer) ->
            [ expr
              match catch with
              | Some (_, value) -> value
              | _ -> ()
              match finalizer with
              | Some value -> value
              | _ -> () ]
            |> filterList
        | Expr.TypeCast (expr = expr) -> findAndDiscardElse predicate expr
        | Expr.Value (kind = kind) ->
            match kind with
            | ValueKind.NewAnonymousRecord (values = exprs) -> filterList exprs
            | NewArray (newKind = NewArrayKind.ArrayAlloc expr) -> findAndDiscardElse predicate expr
            | NewArray (newKind = NewArrayKind.ArrayFrom expr) -> findAndDiscardElse predicate expr
            | NewArray (newKind = NewArrayKind.ArrayValues exprs) -> filterList exprs
            | NewList (headAndTail = Some (head, tail)) ->
                [ head; tail ]
                |> filterList
            | NewOption (value = Some expr) -> findAndDiscardElse predicate expr
            | NewTuple (values = exprs)
            | NewUnion (values = exprs)
            | StringTemplate (values = exprs; tag = None)
            | NewRecord (values = exprs) -> filterList exprs
            | StringTemplate (values = exprs; tag = Some expr) ->
                expr
                :: exprs
                |> filterList
            | _ -> []
        | Expr.WhileLoop (body = body; guard = guard) ->
            [ guard; body ]
            |> filterList
        | _ -> []


type StringUtils =
    /// Trims JS reserved identifiers such as `'` and ``#` where # is some number
    static member TrimReservedIdentifiers =
        function
        | Patterns.EndsWithTrimmed "'" s
        | Patterns.EndsWithTrimmed "`1" s -> StringUtils.TrimReservedIdentifiers s
        | s when
            s.Length > 2
            && s
                .Substring(
                    s.Length
                    - 2
                )
                .StartsWith
                '`'
            ->
            s.Substring (
                0,
                s.Length
                - 2
            )
            |> StringUtils.TrimReservedIdentifiers
        | s -> s
