namespace Partas.Solid

open Fable
open Fable.AST
open Fable.AST.Fable

module internal ReactiveMatch =
    let rec private substitute (bindings: Map<string, Expr>) (expr: Expr) : Expr =
        let visit = substitute bindings

        let without ids =
            ids
            |> List.fold (fun env (id: Ident) -> Map.remove id.Name env) bindings
            |> substitute

        match expr with
        | IdentExpr id ->
            Map.tryFind id.Name bindings
            |> Option.defaultValue expr
        | Lambda (arg, body, name) -> Lambda (arg, without [ arg ] body, name)
        | Delegate (args, body, name, tags) -> Delegate (args, without args body, name, tags)
        | Let (id, value, body) -> Let (id, visit value, without [ id ] body)
        | LetRec (values, body) ->
            let visit = without (List.map fst values)

            LetRec (
                values
                |> List.map (fun (id, value) -> id, visit value),
                visit body
            )
        | Call (callee, info, typ, range) ->
            Call (
                visit callee,
                { info with
                    Args = List.map visit info.Args
                    ThisArg = Option.map visit info.ThisArg },
                typ,
                range
            )
        | CurriedApply (callee, args, typ, range) -> CurriedApply (visit callee, List.map visit args, typ, range)
        | Emit (info, typ, range) ->
            let call = info.CallInfo

            Emit (
                { info with
                    CallInfo =
                        { call with
                            Args = List.map visit call.Args
                            ThisArg = Option.map visit call.ThisArg } },
                typ,
                range
            )
        | TypeCast (value, typ) -> TypeCast (visit value, typ)
        | Sequential values -> Sequential (List.map visit values)
        | IfThenElse (guard, yes, no, range) -> IfThenElse (visit guard, visit yes, visit no, range)
        | DecisionTree (tree, targets) ->
            DecisionTree (
                visit tree,
                targets
                |> List.map (fun (ids, body) -> ids, without ids body)
            )
        | DecisionTreeSuccess (index, values, typ) -> DecisionTreeSuccess (index, List.map visit values, typ)
        | Get (value, kind, typ, range) ->
            Get (
                visit value,
                (match kind with
                 | ExprGet index -> ExprGet (visit index)
                 | _ -> kind),
                typ,
                range
            )
        | Set (value, kind, typ, assigned, range) ->
            Set (
                visit value,
                (match kind with
                 | ExprSet index -> ExprSet (visit index)
                 | _ -> kind),
                typ,
                visit assigned,
                range
            )
        | Operation (kind, tags, typ, range) ->
            let kind =
                match kind with
                | Unary (op, value) -> Unary (op, visit value)
                | Binary (op, left, right) -> Binary (op, visit left, visit right)
                | Logical (op, left, right) -> Logical (op, visit left, visit right)

            Operation (kind, tags, typ, range)
        | Test (value, kind, range) -> Test (visit value, kind, range)
        | WhileLoop (guard, body, range) -> WhileLoop (visit guard, visit body, range)
        | ForLoop (id, start, limit, body, isUp, range) -> ForLoop (id, visit start, visit limit, without [ id ] body, isUp, range)
        | TryCatch (body, catch, finalizer, range) ->
            TryCatch (
                visit body,
                catch
                |> Option.map (fun (id, body) -> id, without [ id ] body),
                Option.map visit finalizer,
                range
            )
        | Extended (Throw (Some value, typ), range) -> Extended (Throw (Some (visit value), typ), range)
        | Extended (Curry (value, arity), range) -> Extended (Curry (visit value, arity), range)
        | ObjectExpr (members, typ, baseCall) ->
            ObjectExpr (
                members
                |> List.map (fun member' ->
                    { member' with
                        Body = without member'.Args member'.Body }),
                typ,
                Option.map visit baseCall
            )
        | Value (kind, range) ->
            let kind =
                match kind with
                | NewArray (ArrayValues values, typ, kind) -> NewArray (ArrayValues (List.map visit values), typ, kind)
                | NewArray (ArrayAlloc value, typ, kind) -> NewArray (ArrayAlloc (visit value), typ, kind)
                | NewArray (ArrayFrom value, typ, kind) -> NewArray (ArrayFrom (visit value), typ, kind)
                | NewTuple (values, isStruct) -> NewTuple (List.map visit values, isStruct)
                | NewUnion (values, tag, entity, generics) -> NewUnion (List.map visit values, tag, entity, generics)
                | NewOption (Some value, typ, isStruct) -> NewOption (Some (visit value), typ, isStruct)
                | NewList (Some (head, tail), typ) -> NewList (Some (visit head, visit tail), typ)
                | NewRecord (values, entity, generics) -> NewRecord (List.map visit values, entity, generics)
                | NewAnonymousRecord (values, names, generics, isStruct) -> NewAnonymousRecord (List.map visit values, names, generics, isStruct)
                | StringTemplate (tag, parts, values) -> StringTemplate (Option.map visit tag, parts, List.map visit values)
                | _ -> kind

            Value (kind, range)
        | _ -> expr

    let rec private marker =
        function
        | TypeCast (value, _) -> marker value
        | Emit ({ Macro = "__PARTAS_MATCH__($0, $1)"
                  CallInfo = { Args = [ source; continuation ] } },
                _,
                _) -> Some (source, continuation)
        | _ -> None

    let rec private continuation =
        function
        | TypeCast (value, _) -> continuation value
        | Call (Import ({ Selector = "uncurry2"
                          Path = path
                          Kind = LibraryImport _ },
                        _,
                        _),
                { Args = [ value ] },
                _,
                _) when path.EndsWith ("/Util.js", System.StringComparison.Ordinal) -> continuation value
        | Lambda (arg, body, _) -> Some (arg, body)
        | Delegate ([ arg; builder ], body, _, _) -> Some (arg, Lambda (builder, body, None))
        | _ -> None

    let private lower suffix (transform: Expr -> Expr) (renderChild: Expr -> Expr) source (arg: Ident) body =
        let arms = System.Collections.Generic.List<Ident list * Expr> ()

        let choice (index: int) values =
            AstUtils.Object
                [ "arm", AstUtils.Value index
                  "values",
                  AstUtils.ValueArray (
                      IdentExpr arg
                      :: values
                  ) ]

        let addArm captured ids body values =
            let index = arms.Count

            arms.Add (
                arg
                :: (captured
                    @ ids),
                body
            )

            choice
                index
                ((captured
                  |> List.map IdentExpr)
                 @ values)

        let rec select captured expr =
            match expr with
            | TypeCast (value, _) -> select captured value
            | DecisionTree (tree, targets) ->
                let indices =
                    targets
                    |> List.map (fun (ids, body) ->
                        let index = arms.Count

                        arms.Add (
                            arg
                            :: (captured
                                @ ids),
                            body
                        )

                        index)

                let rec outcomes =
                    function
                    | DecisionTreeSuccess (index, values, _) ->
                        choice
                            indices[index]
                            ((captured
                              |> List.map IdentExpr)
                             @ values)
                    | IfThenElse (guard, yes, no, range) -> IfThenElse (guard, outcomes yes, outcomes no, range)
                    | Let (id, value, body) -> Let (id, value, outcomes body)
                    | Sequential values when not values.IsEmpty ->
                        AstUtils.Sequential (
                            (values
                             |> List.take (
                                 values.Length
                                 - 1
                             ))
                            @ [ outcomes (List.last values) ]
                        )
                    | expr -> expr

                outcomes tree
            | IfThenElse (guard, yes, no, range) -> IfThenElse (guard, select captured yes, select captured no, range)
            | Let (({ IsCompilerGenerated = true
                      Name = name } as id),
                   value,
                   body) when name.StartsWith "matchValue" ->
                Let (
                    id,
                    value,
                    select
                        (captured
                         @ [ id ])
                        body
                )
            | body -> addArm captured [] body []

        let decision =
            select [] body
            |> transform

        let selected =
            AstUtils.Ident ($"PARTAS_MATCH_SELECTION_{suffix}", typ = LambdaType (Unit, Any))

        let readSelection = AstUtils.Call (IdentExpr selected, AstUtils.CallInfo (), Any)

        let matches =
            arms
            |> Seq.mapi (fun index (ids, body) ->
                let values =
                    AstUtils.Ident ($"PARTAS_MATCH_VALUES_{suffix}", typ = LambdaType (Unit, Any))

                let readValues = AstUtils.Call (IdentExpr values, AstUtils.CallInfo (), Any)

                let bindings =
                    ids
                    |> List.mapi (fun index (id: Ident) -> id.Name, Get (readValues, ExprGet (AstUtils.Value index), id.Type, id.Range))
                    |> Map.ofList
                // Fable can put simple pattern projections in branch-local lets instead
                // of decision-tree parameters. Keep these as live accessor projections.
                let rec projection =
                    function
                    | IdentExpr id ->
                        ids
                        |> List.exists (fun source -> source.Name = id.Name)
                    | TypeCast (value, _) -> projection value
                    // Only structural pattern projections can be repeated safely. In
                    // particular, ExprGet can evaluate an effectful index, and an
                    // ordinary field/property read can observe mutable state.
                    | Get (value, (UnionField _ | TupleIndex _ | OptionValue | ListHead | ListTail), _, _) -> projection value
                    | _ -> false

                let rec structuralProjection =
                    function
                    | TypeCast (value, _) -> structuralProjection value
                    | Get (_, (UnionField _ | TupleIndex _ | OptionValue | ListHead | ListTail), _, _) as value -> projection value
                    | _ -> false

                let rec aliases =
                    function
                    | Let (id, value, body) when
                        not id.IsMutable
                        && structuralProjection value
                        ->
                        aliases (substitute (Map.ofList [ id.Name, value ]) body)
                    | TypeCast (value, typ) -> TypeCast (aliases value, typ)
                    | body -> body

                let child =
                    body
                    |> aliases
                    |> substitute bindings
                    |> renderChild

                let condition =
                    AstUtils.Emit ("$0.arm === $1 ? $0.values : undefined", AstUtils.CallInfo (args = [ readSelection; AstUtils.Value index ]))

                JsxUtils.CreateElement (
                    AstUtils.Import ("Match", "solid-js"),
                    [ "when", condition; "keyed", AstUtils.Value false ],
                    [ Delegate ([ values ], child, None, []) ]
                ))
            |> List.ofSeq

        let switch =
            JsxUtils.CreateElement (AstUtils.Import ("Switch", "solid-js"), [], List.rev matches)

        let memo =
            AstUtils.Call (
                AstUtils.Import ("createMemo", "solid-js"),
                AstUtils.CallInfo (
                    args =
                        [ Delegate ([], Let (arg, source, decision), None, [])
                          AstUtils.Object [ "lazy", AstUtils.Value true ] ]
                ),
                selected.Type
            )

        AstUtils.Call (Delegate ([], Let (selected, memo, switch), None, [ "partas-match-scope" ]), AstUtils.CallInfo (), JsxUtils.ElementType)

    /// <summary>Places generated selection declarations before JSX within their lexical scope.</summary>
    /// <param name="expr">The transformed component or render-callback body.</param>
    let rec placeSelections expr =
        let bindings = System.Collections.Generic.List<Ident * Expr> ()

        let rec visit =
            function
            | Call (Delegate ([], Let (id, value, body), _, tags), _, _, _) when List.contains "partas-match-scope" tags ->
                bindings.Add (id, value)
                visit body
            // Never lift across bindings, conditional branches, or function boundaries.
            | Let (id, value, body) -> Let (id, placeSelections value, placeSelections body)
            | LetRec (values, body) ->
                LetRec (
                    values
                    |> List.map (fun (id, value) -> id, placeSelections value),
                    placeSelections body
                )
            | Lambda (arg, body, name) -> Lambda (arg, placeSelections body, name)
            | Delegate (args, body, name, tags) -> Delegate (args, placeSelections body, name, tags)
            | IfThenElse (guard, yes, no, range) -> IfThenElse (placeSelections guard, placeSelections yes, placeSelections no, range)
            | DecisionTree (tree, targets) ->
                DecisionTree (
                    placeSelections tree,
                    targets
                    |> List.map (fun (ids, body) -> ids, placeSelections body)
                )
            | Sequential values -> Sequential (List.map placeSelections values)
            | TypeCast (value, typ) -> TypeCast (visit value, typ)
            | Call (callee, info, typ, range) when List.contains "jsx" info.Tags ->
                Call (
                    callee,
                    { info with
                        Args = List.map visit info.Args },
                    typ,
                    range
                )
            | Call (callee, info, typ, range) ->
                Call (
                    placeSelections callee,
                    { info with
                        Args = List.map placeSelections info.Args
                        ThisArg = Option.map placeSelections info.ThisArg },
                    typ,
                    range
                )
            | Value (NewList (Some (head, tail), typ), range) -> Value (NewList (Some (visit head, visit tail), typ), range)
            | Value (NewTuple (values, isStruct), range) -> Value (NewTuple (List.map visit values, isStruct), range)
            | Value (NewArray (ArrayValues values, typ, kind), range) -> Value (NewArray (ArrayValues (List.map visit values), typ, kind), range)
            | expr -> expr

        let body = visit expr

        (List.ofSeq bindings, body)
        ||> List.foldBack (fun (id, value) body -> Let (id, value, body))

    /// <summary>Rewrites a reactive builder-binding marker to Switch/Match with live pattern-bound values.</summary>
    /// <param name="ctx">The component context supplying unique generated identifiers.</param>
    /// <param name="transform">Transforms the source expression.</param>
    /// <param name="renderChild">Collects and renders a matched element-builder body.</param>
    /// <param name="expr">The expression to inspect for a builder-binding marker.</param>
    let tryLower (ctx: PluginContext) (transform: Expr -> Expr) (renderChild: Expr -> Expr) expr =
        let candidate =
            match expr with
            | Let (id, source, body) ->
                match marker body with
                | Some (IdentExpr used, cont) when
                    used.Name = id.Name
                    && id.Name.StartsWith "PARTAS_MATCH_SOURCE"
                    ->
                    Some (source, cont)
                | _ -> None
            | expr -> marker expr

        candidate
        |> Option.bind (fun (source, cont) ->
            continuation cont
            |> Option.map (fun (arg, body) ->
                ctx.MatchCounter <-
                    ctx.MatchCounter
                    + 1

                lower ctx.MatchCounter transform renderChild (transform source) arg body))
