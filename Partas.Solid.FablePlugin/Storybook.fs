namespace Partas.Solid

open System
open Fable
open Fable.AST
open Fable.AST.Fable
open Fable.AST.Fable.Expr
open Fable.AST.Fable.Patterns
open Partas.Solid.Baked
open System.Xml.Linq
open System.Xml.XPath

[<assembly: ScanForPlugins>]
do ()

(*
Doc members
DefaultValue
Description = summary
*)
// Make a computation expression for rendering variants and the main

/// </summary>
// Get docs off types
// get docs off members

[<RequireQualifiedAccess>]
type FieldType =
    | Field of Field
    | Member of MemberFunctionOrValue
    member this.Type =
        match this with
        | Field field -> field.FieldType
        | Member memb ->
            if memb.IsSetter then
                memb.CurriedParameterGroups
                |> List.collect id
                |> List.head
            else
                memb.ReturnParameter
            |> _.Type
    member this.XmlDocs =
        match this with
        | Field _ -> None
        | Member memb ->
            memb.XmlDoc
            |> Option.bind (fun docs ->
                if docs |> String.IsNullOrWhiteSpace then
                    None
                else Some docs
                )
    member this.Attributes =
        match this with
        | Field _ -> []
        | Member memb ->
            memb.Attributes |> Seq.toList
    member this.Name =
        match this with
        | Field field -> field.Name
        | Member memb -> memb.DisplayName

type ControlType =
    | Object
    | Boolean
    | Check
    | InlineCheck
    | Radio
    | InlineRadio
    | Select
    | MultiSelect
    | Number
    | Range
    | File
    | Color
    | Date
    | Text
    override this.ToString() =
        match this with
        | Object -> "object"
        | Boolean -> "boolean"
        | Check -> "check"
        | InlineCheck -> "inline-check"
        | Radio -> "radio"
        | InlineRadio -> "inline-radio"
        | Select -> "select"
        | MultiSelect -> "multi-select"
        | Number -> "number"
        | Range -> "range"
        | File -> "file"
        | Color -> "color"
        | Date -> "date"
        | Text -> "text"

[<AutoOpen>]
module Utils =
    type AstUtils with
        static member TableInfo(?summary: string, ?detail: string) =
            [
                if summary.IsSome then
                    "summary", summary.Value |> AstUtils.Value
                if detail.IsSome then
                    "detail", detail.Value |> AstUtils.Value
            ] |> AstUtils.Object
        static member Table(
            ?category: string,
            ?defaultValueSummary: string,
            ?defaultValueDetail: string,
            ?disable: bool,
            ?subcategory: string,
            ?typSummary: string,
            ?typDetail: string) =
            [
                if category.IsSome then
                    "category", AstUtils.Value category.Value
                if defaultValueDetail.IsSome || defaultValueSummary.IsSome then
                    "defaultValue", AstUtils.TableInfo(?summary = defaultValueSummary, ?detail = defaultValueDetail)
                if disable.IsSome then
                    "disable", AstUtils.Value disable.Value
                if typSummary.IsSome || typDetail.IsSome then
                    "type", AstUtils.TableInfo(?summary = typSummary, ?detail = typDetail)
                if subcategory.IsSome then
                    "subcategory", AstUtils.Value subcategory.Value
            ] |> AstUtils.Object
        static member ControlType(
              ?typ: ControlType,
              ?accept: string,
              // todo ?
              ?labels: (string * string) list,
              ?max: Expr,
              ?min: Expr,
              ?presetColors: string list,
              ?step: Expr,
              ?doNotRender: bool
              ) =
            let doNotRender = defaultArg doNotRender false
            [
                if typ.IsSome && not doNotRender then
                    "type", AstUtils.Value(typ.Value.ToString())
                elif doNotRender then
                    "type", AstUtils.Value(false)
                if accept.IsSome then
                    "accept", AstUtils.Value(accept.Value)
                if labels.IsSome then
                    "labels",
                    labels.Value
                    |> List.map (snd >> AstUtils.Value)
                    |> List.zip (labels.Value |> List.map fst)
                    |> AstUtils.Object
                if max.IsSome then
                    "max", max.Value
                if min.IsSome then
                    "min", min.Value
                if presetColors.IsSome then
                    "presetColors",
                    presetColors.Value
                    |> List.map AstUtils.Value
                    |> AstUtils.ValueArray
                if step.IsSome then
                    "step", step.Value

            ] |> AstUtils.Object
        static member ArgType(
            ?control: Expr,
            ?description: string,
            ?conditional: Expr,
            ?mapping: Expr,
            ?name: string,
            ?options: Expr list,
            ?table: Expr,
            ?typ: Expr) =
            [
                if control.IsSome then "control", control.Value
                if description.IsSome then "description", AstUtils.Value(description.Value)
                if conditional.IsSome then "conditional", conditional.Value
                if mapping.IsSome then "mapping", mapping.Value
                if name.IsSome then "name", AstUtils.Value name.Value
                if options.IsSome then "options", AstUtils.ValueArray options.Value
                if table.IsSome then "table", table.Value
                if typ.IsSome then "type", typ.Value
            ] |> AstUtils.Object
        static member Meta(
            comp: Expr,
            ?subComps: Expr list,
            ?args: Expr,
            ?argTypes: Expr,
            ?render: Expr,
            ?stories: string list,
            ?parameters: Expr,
            ?tags: string list
            ) =
            [
                "component", comp
                if subComps.IsSome then "subcomponents", AstUtils.ValueArray subComps.Value
                if args.IsSome then "args", args.Value
                if argTypes.IsSome then "argTypes", argTypes.Value
                if render.IsSome then "render", render.Value
                if stories.IsSome then "stories", stories.Value |> List.map AstUtils.Value |> AstUtils.ValueArray
                if parameters.IsSome then "parameters", parameters.Value
                if tags.IsSome then "tags", tags.Value |> List.map AstUtils.Value |> AstUtils.ValueArray
            ] |> AstUtils.Object
module internal rec StorybookTypeRecursion =
    let (|EntityFullName|): DeclaredType -> string = _.Entity.FullName
    /// Filter interfaces that are predefined as thats too much noise.
    let (|FeedInterface|) (ctx: PluginContext): DeclaredType list -> DeclaredType list = function
        | [] -> []
        | declaredType :: FeedInterface ctx rest ->
            match declaredType with
            | EntityFullName (StartsWith "Partas.Solid.Tags") ->
                rest
            | ent ->
                ent :: rest
    let filterMembers (ctx: PluginContext) (decls: MemberFunctionOrValue seq) =
        decls
        |> Seq.filter (fun memb ->
            $"{memb.FullName} | {memb.ToString()}"
            |> PluginContext.logWarning ctx
            (
                 memb.IsInline
                || memb.IsInternal
                || memb.IsPrivate
                || memb.CurriedParameterGroups |> List.collect id |> List.length > 1
            ) |> not
            && memb.IsSetter
            )

    let getFilteredMembers (ctx: PluginContext) (decl: DeclaredType) =
        let entity = ctx.Helper.GetEntity decl.Entity
        entity.MembersFunctionsAndValues
        |> filterMembers ctx

    let getEntityInterfaces (ctx: PluginContext) (ent: Entity) =
        ent.AllInterfaces
        |> Seq.toList
        |> function FeedInterface ctx interfaces -> interfaces

    let getEntity (ctx: PluginContext) (entityRef: DeclaredType) =
        ctx.Helper.GetEntity entityRef.Entity

    let rec getEntityMembers (ctx: PluginContext) (entity: Entity) =
        let getMembers =
            entity.MembersFunctionsAndValues
            |> filterMembers ctx
            |> Seq.map FieldType.Member
            |> Seq.toList
        entity.BaseType
        |> Option.map (getEntity ctx >> getEntityMembers ctx)
        |> Option.defaultValue []
        |> List.append getMembers
    let rec collectEntityFields (ctx: PluginContext) (ent: Entity) =
        ent.BaseType
        |> Option.map (getEntity ctx >> collectEntityFields ctx)
        |> Option.defaultValue []
        |> List.append ent.FSharpFields
        |> List.filter (_.Name.EndsWith('@') >> not)

    let (|GetGenericArg|) (ctx: PluginContext) = function
        | GetDeclaredType (Type.DeclaredType(ref, _)) ->
            ctx.Helper.GetEntity ref
        | _ -> failwith "Incorrect AST structure. Different to expected."

    let rec collectEntityMembers (ctx: PluginContext) (entity: Entity) =
        let baseAndEntityFields = collectEntityFields ctx entity |> List.map FieldType.Field
        let interfaceMembers =
            getEntityInterfaces ctx entity
            |> Seq.collect (getFilteredMembers ctx)
            |> Seq.map FieldType.Member
            |> Seq.toList
        let entityMembers =
            getEntityMembers ctx entity
        baseAndEntityFields @ interfaceMembers @ entityMembers

module internal rec StorybookCases =
    type CasesExpr = CasesExpr of Expr
    type Cases = {
        PropertyName: string
        Cases: string list
    }
    let makeCases (ctx: PluginContext) (typ: Type) (CasesExpr caseExpr) =
        let fieldExtractor: Expr -> string option = function
            | Get(expr = IdentExpr { Type = identTyp }; kind = (
                GetKind.ExprGet(Value(kind = ValueKind.StringConstant(field)))
               | GetKind.FieldGet( { Name = field } )
                )) when identTyp = typ ->
                Some field
            | _ -> None
        let rec (|GetCases|): Expr -> string list =
            function
            | Expr.Call(callee = GetCases values; info = { Args = GetCases headValues :: exprs }) ->
                values @ headValues @ (exprs |> List.collect (function GetCases values -> values))
            | Expr.CurriedApply(applied = GetCases values; args = exprs) ->
                values @ (exprs |> List.collect (function GetCases values -> values))
            | Expr.DecisionTree(expr = GetCases values; targets = targets) ->
                values @ (targets |> List.collect (snd >> function GetCases values -> values))
            | Expr.Delegate(body = GetCases values) -> values
            | Expr.DecisionTreeSuccess(boundValues = exprs) ->
                List.collect (function GetCases values -> values) exprs
            | Expr.Get(expr = GetCases values; kind = kind) ->
                match kind with
                | ExprGet (GetCases values) -> values
                | _ -> []
                @ values
            | Expr.IfThenElse(guardExpr = GetCases values; elseExpr = GetCases elseValues; thenExpr = GetCases thenValues) ->
                values @ elseValues @ thenValues
            | Expr.Lambda(body = GetCases values) -> values
            | Expr.Let(value = GetCases values; body = GetCases bodyValues) -> values @ bodyValues
            | Expr.LetRec(bindings = exprs; body = GetCases values) ->
                (exprs |> List.collect (snd >> function GetCases values -> values)) @ values
            | Expr.ObjectExpr(baseCall = Some(GetCases values); members = members) ->
                values @ (members |> List.collect (_.Body >> function GetCases memberValues -> memberValues) )
            | Expr.ObjectExpr(members = members) ->
                members |> List.collect (_.Body >> function GetCases values -> values)
            | Expr.Operation(kind = OperationKind.Binary(operator = BinaryOperator.BinaryEqual; right = Value(kind = StringConstant value); left = GetCases values)) ->
                value :: values
            | Expr.Operation(kind = OperationKind.Binary(operator = BinaryOperator.BinaryEqual; right = GetCases values; left = GetCases leftValues)) ->
                leftValues @ values
            | Expr.Sequential(exprs = exprs) ->
                exprs |> List.collect (function GetCases values -> values)
            | Expr.TypeCast(expr = GetCases values) -> values
            | _ -> []
        let field = caseExpr |> findAndDiscardElse (fieldExtractor >> _.IsSome) |> List.choose fieldExtractor
        field |> sprintf "%A" |> PluginContext.logWarning ctx
        field |> List.map(fun fieldName -> {
                PropertyName = fieldName
                Cases = caseExpr
                        |> function GetCases values -> values } )


    let findMatchers (ctx: PluginContext) (expr: Expr list) =
        let matcher =
            findAndDiscardElse (function
            | Let(ident = { Name = StartsWith("matchValue"); Type = Type.String }) -> true
            | _ -> false
            )
        expr |> List.collect matcher |> List.map CasesExpr
        // |> fun values ->
        //     values |> sprintf "%A"
        //     |> PluginContext.logWarning ctx
        //     values

    let getCases (ctx: PluginContext) (entityTyp: Type) (expr: Expr) =
        findMatchers ctx [expr]
        |> List.collect (makeCases ctx entityTyp)
        |> List.groupBy _.PropertyName
        |> List.map (fun (key,cases) -> {
                PropertyName = key
                Cases = cases |> List.collect _.Cases |> List.distinct
            } )

module internal rec StorybookVariantsAndArgs =
    type RawVariantExpr = RawVariantExpr of variantName: string * expr: Expr
    type Variant = Variant of variantName: string * args: (string * Expr) list
    let getVariants (ctx: PluginContext) (expr: Expr) =
        let predicate = function
            | Expr.Sequential (TypeCast(expr = Value(kind = StringConstant (StartsWith "PARTAS_VARIANT"))) :: _ ) -> true
            | _ -> false
        let rec recursiveDiscovery expr =
            findAndDiscardElse predicate expr
            |> List.collect (function
                | Expr.Sequential( _ :: exprs ) as expr ->
                    expr ::
                    (exprs
                    |> List.collect recursiveDiscovery)
                | e -> [ e ]
                )
        let extractVariantExprs = function
            | Sequential (nameExpr :: (Sequential (TypeCast(expr = expr) :: _) :: _)) ->
                let variantName =
                    match nameExpr with
                    | TypeCast(expr = Value(kind = StringConstant (StartsWithTrimmed "PARTAS_VARIANT" variantName))) -> Some variantName
                    | _ -> None
                variantName |> Option.map (fun variantName -> RawVariantExpr(variantName, expr))
            | _ -> None
        let processRawVariantExpr (RawVariantExpr(name,expr)) =
            let predicate = function
                | Set _
                | Call(callee = Import(info = { Kind = ImportKind.MemberImport (MemberRef(info = { CompiledName = StartsWith "set_" })) })) ->
                    true
                | _ -> false
            findAndDiscardElse predicate expr
            |> List.choose (function
                | Set(kind = SetKind.FieldSet propName; value = value) ->
                    Some(propName, value)
                | Call(
                   callee = Import(info = { Kind = ImportKind.MemberImport (MemberRef(info = { CompiledName = StartsWithTrimmed "set_" prop }))})
                   info = { Args = exprs }
                   ) ->
                    Some (prop,
                    if exprs.Length > 1 then
                        Sequential exprs
                    elif exprs.Length = 0 then
                        AstUtils.Unit
                    else exprs |> List.head)
                | _ -> None
                )
            |> fun args ->
                Variant(name, args)
        recursiveDiscovery expr
        |> List.choose extractVariantExprs
        |> List.map processRawVariantExpr
    let getArgs (ctx: PluginContext) (expr: Expr) =
        let predicate = function
            | Lambda(name = Some(StartsWith "PARTAS_ARGS")) -> true
            | _ -> false
        let processRawVariantExpr expr =
            let predicate = function
                | Set _
                | Call(callee = Import(info = { Kind = ImportKind.MemberImport (MemberRef(info = { CompiledName = StartsWith "set_" })) })) ->
                    true
                | _ -> false
            findAndDiscardElse predicate expr
            |> List.choose (function
                | Set(kind = SetKind.FieldSet propName; value = value) ->
                    Some(propName, value)
                | Call(
                   callee = Import(info = { Kind = ImportKind.MemberImport (MemberRef(info = { CompiledName = StartsWithTrimmed "set_" prop }))})
                   info = { Args = exprs }
                   ) ->
                    Some (prop,
                    if exprs.Length > 1 then
                        Sequential exprs
                    elif exprs.Length = 0 then
                        AstUtils.Unit
                    else exprs |> List.head)
                | _ -> None
                )
        findAndDiscardElse predicate expr
        |> List.collect processRawVariantExpr
        |> AstUtils.Object

module internal StorybookAST =
    let getComponentExprFromEntity (ctx: PluginContext) (entity: Entity) =
        let name = entity.DisplayName
        let entRef = entity.Ref
        if entRef.SourcePath |> Option.exists ((=) ctx.Helper.CurrentFile) then
            AstUtils.IdentExpr(name)
        else
            AstUtils.Import(name, entRef.SourcePath |> Option.defaultValue "", entRef)


    let createUnionExprs(ctx: PluginContext) (entity: Entity) =
        let cases =
            if not entity.IsFSharpUnion then None
            else
            Some entity.UnionCases
        cases
        |> Option.map(
            List.mapi (fun idx case ->
                Expr.Value(ValueKind.NewUnion([], idx, entity.Ref, []), None)
            )
        )
    let createStringEnumExprArray (ctx: PluginContext) (values: string list) =
        values |> List.map AstUtils.Value |> AstUtils.ValueArray
    let private extractElementValue elementName: XDocument -> _ =
        _.XPathSelectElement($"//{elementName}")
        >> Option.ofObj
        >> Option.map (_.Value.Trim())

    type MetaExpr =
        | ArgType of string * Expr
        | Arg of string * Expr

    let createMeta (ctx: PluginContext) (expr: Expr) =
        let typ =
            match expr with
            | Call(typ = DeclaredType(_, GetDeclaredType typ :: _)) -> typ
            | _ -> failwith $"CreateMeta: Unexpected expr -> {expr}"
        let entity =
            match typ with
            | StorybookTypeRecursion.GetGenericArg ctx entity ->
                entity
        let predefinedCases = StorybookCases.getCases ctx typ expr
        let properties = StorybookTypeRecursion.collectEntityMembers ctx entity
        let args = StorybookVariantsAndArgs.getArgs ctx expr
        let variants = StorybookVariantsAndArgs.getVariants ctx expr
        let argTypes =
            properties |> List.map (fun prop ->
                let docs = prop.XmlDocs |> Option.map (fun docs ->
                    let normalizedDocs = "<document>" + docs + "</document>"
                    let read value =
                        use reader = new System.IO.StringReader(value)
                        XDocument.Load(reader)
                    let docs = read normalizedDocs
                    docs
                    )
                let getElementValue value = docs |> Option.bind (extractElementValue value)
                let description = getElementValue "summary"
                let defaultValue = getElementValue "defaultValue"
                let options =
                    predefinedCases |> List.tryFind (_.PropertyName >> (=) prop.Name) |> Option.map _.Cases
                let rec makeArgType typ =
                    match typ with
                    | Any ->
                        let control = AstUtils.ControlType(typ = ControlType.Object)
                        AstUtils.ArgType(
                            control = control,
                            table = AstUtils.Table(?defaultValueSummary = defaultValue),
                            ?description = description
                            )
                    | Type.Boolean ->
                        let control = AstUtils.ControlType(typ = ControlType.Boolean)
                        AstUtils.ArgType(
                            control = control,
                            table = AstUtils.Table(?defaultValueSummary = defaultValue),
                            ?description = description
                            )
                    | Char ->
                        let control = AstUtils.ControlType(typ = ControlType.Text)
                        AstUtils.ArgType(
                            control = control,
                            table = AstUtils.Table(typSummary = "Char", typDetail = "Single length text", ?defaultValueSummary = defaultValue),
                            ?description = description
                            )
                    | String ->
                        match options with
                        | Some values ->
                            let control = AstUtils.ControlType(
                                typ =
                                    if values.Length < 6 then ControlType.Radio
                                    else ControlType.Select
                                )
                            AstUtils.ArgType(
                                control = control,
                                table = AstUtils.Table(?defaultValueSummary = defaultValue),
                                options = (values |> List.map AstUtils.Value),
                                ?description = description
                                )
                        | None ->
                            let control = AstUtils.ControlType(typ = ControlType.Text)
                            AstUtils.ArgType(
                                control = control,
                                table = AstUtils.Table(?defaultValueSummary = defaultValue),
                                ?description = description
                                )
                    | Regex ->
                        let control =
                            AstUtils.ControlType(typ = ControlType.Text)
                        AstUtils.ArgType(
                            control = control,
                            table = AstUtils.Table(
                                typSummary = "Regex",
                                typDetail = "Expects valid regex",
                                ?defaultValueSummary = defaultValue
                                ),
                            ?description = description
                            )
                    // todo - support enums
                    | Type.Number (kind, info) ->
                        let control =
                            match kind with
                            | Int8 ->
                                AstUtils.ControlType(
                                    typ = ControlType.Number,
                                    max = AstUtils.Value(int System.SByte.MaxValue),
                                    min = AstUtils.Value(int System.SByte.MinValue),
                                    step = AstUtils.Value(1))
                            | UInt8 ->
                                AstUtils.ControlType(
                                    typ = ControlType.Number,
                                    max = AstUtils.Value(int System.Byte.MaxValue),
                                    min = AstUtils.Value(0),
                                    step = AstUtils.Value(1)
                                    )
                            | Int16 ->
                                AstUtils.ControlType(
                                    typ = ControlType.Number,
                                    max = AstUtils.Value(int System.Int16.MaxValue),
                                    min = AstUtils.Value(int System.Int16.MinValue),
                                    step = AstUtils.Value(1)
                                    )
                            | UInt16 ->
                                AstUtils.ControlType(
                                    typ = ControlType.Number,
                                    max = AstUtils.Value(int System.UInt16.MaxValue),
                                    min = AstUtils.Value(int System.UInt16.MinValue),
                                    step = AstUtils.Value(1)
                                    )
                            | UInt64 ->
                                AstUtils.ControlType(
                                    typ = ControlType.Number,
                                    min = AstUtils.Value(0),
                                    step = AstUtils.Value(1)
                                    )
                            | UNativeInt | UInt128 ->
                                AstUtils.ControlType(typ = ControlType.Number, step = AstUtils.Value(1), min = AstUtils.Value(0))
                            | Int128 | NativeInt | Int64 | BigInt ->
                                AstUtils.ControlType(typ = ControlType.Number, step = AstUtils.Value(1))
                            | Float16 | Float32 ->
                                AstUtils.ControlType(
                                     typ = ControlType.Number,
                                     max = AstUtils.Value(float System.Single.MaxValue),
                                     min = AstUtils.Value(float System.Single.MinValue)
                                     )
                            | Float64 | Decimal ->
                                AstUtils.ControlType( typ = ControlType.Number )
                            | Int32 ->
                                AstUtils.ControlType(
                                    typ = ControlType.Number,
                                    max = AstUtils.Value(int System.Int32.MaxValue),
                                    min = AstUtils.Value(int System.Int32.MinValue),
                                    step = AstUtils.Value(1)
                                    )
                            | UInt32 ->
                                AstUtils.ControlType(
                                    typ = ControlType.Number,
                                    max = AstUtils.Value(int System.UInt32.MaxValue),
                                    min = AstUtils.Value(0),
                                    step = AstUtils.Value(1)
                                    )

                        AstUtils.ArgType(
                            control = control,
                            table = AstUtils.Table(
                                typSummary = kind.ToString(),
                                ?defaultValueSummary = defaultValue
                                ),
                            ?description = description
                            )
                    | Tuple (genArg, _) ->
                        let tupleTyping =
                            genArg |> List.map _.ToString() |> String.concat ", "
                            |> sprintf "Tuple ( %s )"
                        let control = AstUtils.ControlType(typ = ControlType.Object)
                        AstUtils.ArgType(
                            control = control,
                            table = AstUtils.Table(
                                typSummary = tupleTyping,
                                ?defaultValueSummary = defaultValue
                                ),
                            ?description = description
                            )
                    | List genArg
                    | Array (genArg, _) ->
                        let arrayTyping = genArg.ToString() |> sprintf "Array of %s"
                        let control = AstUtils.ControlType(typ = ControlType.Object)
                        AstUtils.ArgType(
                            control = control,
                            table = AstUtils.Table(
                                typSummary = arrayTyping,
                                ?defaultValueSummary = defaultValue
                                ),
                            ?description = description
                            )
                    | Nullable (genArg, _) | Option (genArg, _) -> makeArgType genArg
                    | LambdaType _ | DelegateType _ ->
                        let control = AstUtils.ControlType(doNotRender = true)
                        AstUtils.ArgType(
                            control = control,
                            table = AstUtils.Table(
                                typSummary = "function",
                                ?defaultValueSummary = defaultValue
                                ),
                            ?description = description
                            )
                    | GenericParam (name, isMeasure, constraints) ->
                        let control = AstUtils.ControlType(doNotRender = true)
                        AstUtils.ArgType(
                            control = control,
                            table = AstUtils.Table(
                                typSummary = $"GenericParam {name}",
                                ?defaultValueSummary = defaultValue
                                ),
                            ?description = description
                            )
                    | DeclaredType (ref, genArgs) ->
                        let entity = ctx.Helper.GetEntity ref
                        if entity.IsFSharpRecord then
                            let typName =
                                entity.FSharpFields
                                |> List.map (fun field ->
                                    $"{field.Name}: {field.FieldType}"
                                    )
                                |> String.concat ", "
                                |> sprintf "{ %s }"
                            let control = AstUtils.ControlType(typ = ControlType.Object)
                            AstUtils.ArgType(
                                control = control,
                                table = AstUtils.Table(
                                    typSummary = "Object",
                                    typDetail = typName,
                                    ?defaultValueSummary = defaultValue
                                    ),
                                ?description = description
                                )
                        // elif entity.IsFSharpUnion then
                            // TODO
                        else
                        let control = AstUtils.ControlType(typ = ControlType.Object)
                        AstUtils.ArgType(
                            control = control,
                            table = AstUtils.Table(
                                typSummary = ref.DisplayName,
                                ?typDetail = ref.SourcePath,
                                ?defaultValueSummary = defaultValue
                                ),
                            ?description = description
                            )
                    | AnonymousRecordType (fieldNames, genArgs, isStruct) ->
                        let control = AstUtils.ControlType(typ = ControlType.Object)
                        let typName =
                            fieldNames |> Array.toList
                            |> List.zip genArgs
                            |> List.map(function
                                | typ,name ->
                                    $"{name}: {typ}"
                                )
                            |> String.concat ", "
                            |> sprintf "{ %s }"
                        AstUtils.ArgType(
                            control = control,
                            table = AstUtils.Table(
                                typSummary = "Object",
                                typDetail = typName,
                                ?defaultValueSummary = defaultValue
                                ),
                            ?description = description
                            )
                    | _ -> AstUtils.Unit
                    // | _ -> ()
                let argType =
                    makeArgType prop.Type
                    |> fun expr -> prop.Name, expr
                // [ MetaExpr.ArgType argType ]
                makeArgType prop.Type
                |> fun expr ->
                    prop.Name, expr
                )
        [
            "component", getComponentExprFromEntity ctx entity
            "args", args
            "argTypes", AstUtils.Object argTypes
        ] |> AstUtils.Object
        , variants
        // , ()

    [<return: Struct>]
    let rec (|RootExpr|_|) ctx = function
        | Call(callee=(Import(info = { Selector = StartsWith "StorybookExtensions_Run" }))) as expr ->
            ValueSome expr
        | _ -> ValueNone

    let getRoot (ctx: PluginContext) expr =
        let predicate = function RootExpr ctx _ -> true | _ -> false
        findAndDiscardElse predicate expr
        |> function
            | [ expr ]->
                createMeta ctx expr

            | _ -> failwith $"Unexpected content {expr}"

    let rec transform ctx = function
        // | RootExpr ctx expr -> expr
        | expr -> expr |> getRoot ctx

[<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
type PartasStorybookAttribute(compFlags: int) =
    inherit MemberDeclarationPluginAttribute()
    let flags = enum<ComponentFlag> compFlags
    override this.Transform(pluginHelper, file, memberDecl) =
        let ctx = PluginContext.create pluginHelper TransformationKind.MemberDecl flags
        memberDecl |> printfn "%A"
        memberDecl.Body
        |> StorybookAST.transform ctx
        |> fun e ->
            snd e |> sprintf "%A" |> PluginContext.logWarning ctx
            { memberDecl with Body = fst e }
        // |> printfn "%A"
        // |> printfn "%A"
        // memberDecl
        // |> fun newExpr ->
            // newExpr |> printfn "%A"
            // { memberDecl with Body = newExpr }

    override this.TransformCall(_, _, expr) =
        expr

    override this.FableMinimumVersion = "5.0"
    new() = PartasStorybookAttribute(int ComponentFlag.Default)
    new(componentFlag: ComponentFlag) = PartasStorybookAttribute(int componentFlag)
