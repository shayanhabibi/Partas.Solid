namespace Partas.Solid

open System
open Fable
open Fable.AST
open Fable.AST.Fable

module Utils = Patterns

/// Defines patterns for the PluginHelper which establishes whether the plugin is available
/// for use in the environment based on the language target etc
[<RequireQualifiedAccess>]
module internal FableRequirements =
    // Fable 4 does not detect the plugin
    [<Literal>]
    let version = "5.0"

    let (|Language|_|) (input: PluginHelper) =
        match input.Options.Language with
        | Language.JavaScript -> true
        | _ -> false

    let (|FileExtension|_|) (input: PluginHelper) =
        match input.Options.FileExtension with
        | Utils.EndsWith (".js")
        | Utils.EndsWith (".jsx") -> true
        | _ -> false

/// Patterns which specify whether certain members are valid for the attribute to
/// compile to.
module internal SchemaRules =
    /// <summary>
    /// Specifies whether a member meets our requirements for using the
    /// <c>SolidTypeComponent</c> Attribute.
    /// </summary>
    /// <param name="ctx">PluginContext record</param>
    /// <param name="memberDecl">MemberDecl record</param>
    /// <remarks>
    /// Specification as it currently stands:<br/>
    /// - self identifier strictly named <c>props</c><br/>
    /// - no other parameters
    /// </remarks>
    let (|ValidMemberRef|_|) (ctx: PluginContext) (memberDecl: MemberDecl) =
        match memberDecl with
        | { MemberRef = MemberRef (({ FullName = Utils.StartsWith "Partas.Solid" } as declaringEntity),
                                   ({ IsInstance = true
                                      NonCurriedArgTypes = Some ([ Type.Unit ]) } as entityRef)) } ->
            match memberDecl.Args with
            | [ { Name = "props"
                  IsThisArgument = true }
                { Name = Utils.StartsWith "unitVar" } ] ->
                declaringEntity.DisplayName
                |> Some
            | _ ->
                PluginContext.helper ctx
                |> _.LogWarning(
                    "The self identifier must be named `props`, and no arguments must be provided",
                    memberDecl.Args
                    |> List.randomChoice
                    |> _.Range.Value
                )

                None
        | _ ->
            PluginContext.helper ctx
            |> _.LogWarning(
                "This is an invalid member declaration for this attribute. Ensure it follows the following pattern: `member props.typeDef =`. Ensure it is declared within a namespace starting with `Partas.Solid`",
                memberDecl.Args
                |> List.randomChoice
                |> _.Range.Value
            )

            None

/// Prebaked expression constructors. Most
/// reused or verbose Expr constructors should
/// be lifted into this module.
module internal Baked =
    let private importMergeProps = AstUtils.Import ("mergeProps", "solid-js")
    let private importSplitProps = AstUtils.Import ("splitProps", "solid-js")
    let jsxElementType = JsxUtils.ElementType
    let spreadPropertyExpression = AstUtils.IdentExpr ("PARTAS_OTHERS")

    /// Converts property setters into a sugar for setting their defaults by
    /// converting them into a mergeProps, which merges an object with the key,value pairs
    /// against the given props (ie overwritting any double ups).
    let convertSettersToObject (values: (string * Expr) list) (rest: Expr) =
        match values with
        | [] -> rest
        | _ ->
            AstUtils.Sequential (
                AstUtils.SetProp (
                    AstUtils.IdentExpr ("props"),
                    AstUtils.Call (
                        importMergeProps,
                        AstUtils.CallInfo (
                            args =
                                [ let trimReservedIdentifier =
                                      fun (name, expr) -> StringUtils.TrimReservedIdentifiers name, expr

                                  AstUtils.Object (List.map trimReservedIdentifier values)
                                  AstUtils.IdentExpr ("props") ]
                        )
                    )
                ),
                rest
            )

    /// It renders the JSX splitProps, with the given values split into PARTAS_LOCAL, and the rest
    /// into PARTAS_OTHERS
    let convertGettersToObject (values: string list) (rest: Expr) =
        Expr.Let (
            ident = AstUtils.Ident ("[PARTAS_LOCAL, PARTAS_OTHERS]"),
            value =
                AstUtils.Call (
                    importSplitProps,
                    AstUtils.CallInfo (
                        args =
                            [ AstUtils.IdentExpr ("props")
                              AstUtils.ValueArray (
                                  values
                                  |> List.map AstUtils.Value
                              ) ]
                    )
                ),
            body = rest
        )

    let propGetter (getTarget: string) =
        AstUtils.GetProp ("PARTAS_LOCAL", StringUtils.TrimReservedIdentifiers getTarget)

    /// Renders an ElementBuilder into the final Expr. Performs ONE final transformation; it will
    /// pattern match the tagname "Fragment" and remove it to render `<> </>`.
    let renderElement (ctx: PluginContext) (builder: ElementBuilder) =
        let renderTagName =
            function
            | TagSource.AutoImport "Fragment" -> AstUtils.Value ("")
            | TagSource.LibraryImport imp -> imp
            | TagSource.AutoImport name -> AstUtils.Value (name)

        JsxUtils.CreateElement (
            renderTagName builder.TagSource,
            builder.Properties,
            builder.Children
            |> List.rev
        )

module internal MemberRef =
    let (|MemberRefIs|) (ctx: PluginContext) : MemberRef -> MemberRefType =
        function
        | MemberRef ({ FullName = Utils.StartsWith "Partas.Solid" }, _) as memberRef ->
            let mref =
                memberRef
                |> PluginContext.getMember ctx

            if mref.IsConstructor then MemberRefType.Constructor
            elif mref.IsSetter then MemberRefType.Setter
            elif mref.IsGetter then MemberRefType.Getter
            else MemberRefType.None
        | GeneratedMemberRef (_) -> MemberRefType.Generated
        | _ -> MemberRefType.None

    let (|PartasName|_|) (ctx: PluginContext) : MemberRef -> string option =
        function
        | MemberRef (_, { CompiledName = compiledName }) ->
            compiledName
            |> _.Split('.')
            |> Array.rev
            |> Array.head
            |> function
                | Utils.StartsWithTrimmed "set_" memberName
                | Utils.StartsWithTrimmed "get_" memberName
                | Utils.EndsWithTrimmed "_$ctor" memberName
                | memberName ->
                    memberName
                    |> StringUtils.TrimReservedIdentifiers
                    |> Some
        | GeneratedMemberRef (_) -> None

    /// <summary>
    /// Used for POJO constructors to extract the prop names from the constructor
    /// </summary>
    let (|ParameterNames|) (ctx: PluginContext) : MemberRef -> _ =
        PluginContext.getMember ctx
        >> _.CurriedParameterGroups
        >> (fun lst ->
            if lst.Length > 1 then
                "This member has more than one parameter group which may cause undefined behaviour with the plugin. Please monitor and report any issues."
                |> PluginContext.logWarning ctx

            lst)
        >> List.collect id
        >> List.distinctBy (fun x -> x.Name, x.Type)
        >> List.map _.Name

    module internal Option =
        let (|PartasName|_|) (ctx: PluginContext) : MemberRef option -> string option =
            function
            | Some (PartasName ctx name) -> Some name
            | _ -> None

/// Contains active patterns to match expressions against common patterns such as an imported constructor,
/// or a setter of an imported property etc.
/// Consider the Expr.ImportedConstructor as an example.
/// They do not recognize constructors in the sense of TagConstructors. They will recognize any constructor which
/// is library imported, or module imported.
module internal Expr =
    let (|ImportedConstructor|_|) (ctx: PluginContext) =
        function
        | Import ({ Kind = UserImport _ }, Any, _)
        | Import ({ Kind = MemberImport (MemberRef.MemberRefIs ctx MemberRefType.Constructor) }, _, _) -> true
        | _ -> false

    let (|NativeImportedConstructor|_|) (ctx: PluginContext) =
        function
        | Import ({ Kind = MemberImport (MemberRef ({ FullName = Utils.StartsWith "Partas.Solid.Tags" | Utils.StartsWith "Partas.Solid.Svg" }, _)) },
                  _,
                  _) & ImportedConstructor ctx -> true
        | _ -> false

    let (|ImportedSetter|_|) (ctx: PluginContext) =
        function
        | Import ({ Selector = Utils.StartsWith "Partas_Solid"
                    Kind = MemberImport (MemberRef.MemberRefIs ctx MemberRefType.Setter) },
                  _,
                  _) -> true
        | _ -> false

    let (|ImportedGetter|_|) (ctx: PluginContext) =
        function
        | Import ({ Selector = Utils.StartsWith "Partas_Solid"
                    Kind = MemberImport (MemberRef.MemberRefIs ctx MemberRefType.Getter) },
                  _,
                  _) -> true
        | _ -> false

    let (|ImportedExtensionName|_|) (ctx: PluginContext) =
        function
        | Import ({ Selector = Utils.StartsWith "HtmlElementExtensions_"
                    Kind = MemberImport ((MemberRef.MemberRefIs ctx MemberRefType.Setter & MemberRef (_, { CompiledName = compiledName }))) },
                  _,
                  _) -> Some compiledName
        | _ -> None

    let (|ImportedContainerExtensionName|_|) (ctx: PluginContext) =
        function
        | Import ({ Selector = Utils.StartsWith "HtmlContainerExtensions_" | Utils.StartsWith "BindingsModule_Extensions"
                    Kind = MemberImport (MemberRef.PartasName ctx _ & MemberRef (_, { CompiledName = compiledName })) },
                  _,
                  _) -> Some compiledName
        | _ -> None

    /// Elucidates the first expr that is not a type cast.
    let rec (|TypeCastDrill|) (ctx: PluginContext) : Expr -> Expr =
        function
        | TypeCast (TypeCastDrill ctx expr, _)
        | expr -> expr

module internal Type =
    let rec private (|GetDeclaredType|_|) (ctx: PluginContext) : Type -> Type option =
        function
        | Type.DeclaredType (_) as typ -> Some typ
        | Type.Array (GetDeclaredType ctx typ, _) -> Some typ
        | Type.List (GetDeclaredType ctx typ) -> Some typ
        | Type.Option (GetDeclaredType ctx typ, _) -> Some typ
        | Type.Tuple (GetDeclaredType ctx typ :: _, _) -> Some typ
        | Type.DelegateType (_, GetDeclaredType ctx typ) -> Some typ
        | Type.LambdaType (_, GetDeclaredType ctx typ) -> Some typ
        | _ -> None

    /// Recursively explores a `Type` AST node until either returning the tail part of a DeclaredType fullname,
    /// or none. It can therefor also be used to ensure the type starts with `Partas.Solid`
    let rec (|PartasName|_|) (ctx: PluginContext) : Type -> string option =
        function
        | GetDeclaredType ctx (Type.DeclaredType ({ FullName = Utils.StartsWith "Partas.Solid" as fullName }, _)) ->
            fullName
            |> _.Split('.')
            |> Array.last
            |> StringUtils.TrimReservedIdentifiers
            |> Some
        | _ -> None

    /// Digs into a type to see if it can find a DeclaredType node; if so, it matches the provided attribute name
    /// and then emits the attribute
    let (|HasAttribute|_|) (ctx: PluginContext) (attrName: string) : Type -> Attribute option =
        function
        | GetDeclaredType ctx (Type.DeclaredType (entityRef, _)) ->
            entityRef
            |> PluginContext.getEntity ctx
            |> _.Attributes
            |> Seq.tryFind (
                _.Entity.FullName
                >> (=) attrName
            )
        | _ -> None

    /// Digs into a type to see if it can find a DeclaredType node; if so, it extracts the attribute key,value pair for
    /// PartasImportAttribute if it is present and returns them
    let (|HasPartasImport|_|) (ctx: PluginContext) : Type -> (string * string) option =
        function
        | HasAttribute ctx "Partas.Solid.PartasImportAttribute" attr ->
            attr
            |> _.ConstructorArgs
            |> List.map _.ToString()
            |> List.pairwise
            |> List.head
            |> Some
        | _ -> None

    /// Digs into a type to see if it can find a DeclaredType node; if so, it extracts the attribute key,selector,path for
    /// PartasProxyImportAttribute if it is present and returns them
    let (|HasPartasProxyImport|_|) (ctx: PluginContext) : Type -> (string * string * string) option =
        function
        | HasAttribute ctx "Partas.Solid.PartasProxyImportAttribute" attr ->
            attr
            |> _.ConstructorArgs
            |> List.map _.ToString()
            |> function
                | [ p1; p2; p3 ] -> Some (p1, p2, p3)
                | _ -> None
        | _ -> None

    /// Determines if type has POJO attribute
    let (|HasPojo|_|) (ctx: PluginContext) : Type -> bool =
        function
        | HasAttribute ctx "Fable.Core.JS.PojoAttribute" _ -> true
        | _ -> false

/// <summary>
/// Pattern matches for EntityRef expr nodes
/// </summary>
module internal EntityRef =
    /// <seealso cref="Partas.Solid.Type.(|HasAttribute|_)"/>
    let (|HasAttribute|_|) (ctx: PluginContext) (attrName: string) : EntityRef -> Attribute option =
        PluginContext.getEntity ctx
        >> _.Attributes
        >> Seq.tryFind (
            _.Entity.FullName
            >> (=) attrName
        )

    let (|HasPojo|_|) (ctx: PluginContext) : EntityRef -> unit option =
        function
        | HasAttribute ctx "Fable.Core.JS.PojoAttribute" _ -> Some ()
        | _ -> None

module internal Ident =
    /// where we can, we use ridiculous names in computations so that the chance of user AST
    /// accidentally being transformed as one of these patterns is almost nil. It also simplifies
    /// debugging the resulting JSX since they are easy to identify.
    /// Unfortunately, this is incompatible with builders for types that have the Import attribute
    // The Import attribute causes the builders to produce default names for functions and just generally
    // ruins the vibe. If this causes issue with native bindings etc, then either the plugin will have to
    // take all cases and inject them manually, or tackle matching the generated names and identifiers.
    let (|IdentIs|) (ctx: PluginContext) : Ident -> IdentType =
        function
        | { Name = Utils.StartsWith "returnVal"
            Type = Type.PartasName ctx _ } -> IdentType.ReturnVal
        | { Name = Utils.EndsWith "_$ctor"
            Type = Type.PartasName ctx _ } -> IdentType.Constructor
        | { Name = "props"
            IsThisArgument = true
            Type = Type.PartasName ctx _ } -> IdentType.Props
        // | { Name = "enumerator"; IsCompilerGenerated = true } -> IdentType.Enumerator(EnumType.Enumerator)
        // | { Name = (
        //         Helpers.StartsWithTrimmed "System.Collections.Generic.IEnumerator`1." value
        //       | Helpers.StartsWithTrimmed "System.Collections.IEnumerator." value
        //         ); IsCompilerGenerated = true } ->
        //     match value with
        //     | Helpers.StartsWith "get_Current" -> EnumType.Current
        //     | Helpers.StartsWith "MoveNext" -> EnumType.Next
        //     | _ ->
        //         $"Got a identity for an enumerator func with an unhandled subtype: {value}"
        //         |> PluginContext.logError ctx
        //         EnumType.Enumerator
        //     |> IdentType.Enumerator
        | { Name = Utils.StartsWith "PARTAS_YIELD"
            Type = Type.PartasName ctx _ } -> IdentType.Yield
        | { Name = Utils.StartsWith "PARTAS_FIRST" } -> IdentType.First
        | { Name = Utils.StartsWith "PARTAS_SECOND" } -> IdentType.Second
        | { Name = Utils.StartsWith "PARTAS_ELEMENT" } -> IdentType.Element
        | { Name = Utils.StartsWith "PARTAS_BUILDER"
            Type = Type.PartasName ctx _ } -> IdentType.Builder
        | { Name = Utils.StartsWith "PARTAS_DELAY" } -> IdentType.Delay
        | { Name = Utils.StartsWith "PARTAS_CONT" } -> IdentType.Cont
        | { Name = Utils.StartsWith "PARTAS_VALUE" } -> IdentType.Value
        | { Name = Utils.StartsWith "PARTAS_TEXT" } -> IdentType.Text
        | _ -> IdentType.Other

    let (|HasPojo|_|) (ctx: PluginContext) : Ident -> bool =
        function
        | { Type = Type.HasPojo ctx } -> true
        | _ -> false

/// Contains patterns to match CallInfo's with common member ref or thisarg patterns.
/// Honestly, they're only used probably once each in current iterations, but I think
/// it helps with the readability of the more complicated patterns.
module internal CallInfo =
    let (|PropertySetter|_|) (ctx: PluginContext) =
        function
        | { ThisArg = Some (IdentExpr (Ident.IdentIs ctx IdentType.Props)) } -> true
        | _ -> false

    let (|Constructor|_|) (ctx: PluginContext) =
        function
        | { MemberRef = Some (MemberRef.MemberRefIs ctx MemberRefType.Constructor) } -> true
        | _ -> false

    let (|Pojo|_|) (ctx: PluginContext) =
        function
        | { MemberRef = Some (MemberRef (EntityRef.HasPojo ctx, _)) } -> true
        | _ -> false

    /// Matches a constructor which has the Pojo attribute. It extracts the parameter names and matches
    /// them to the values in the call and emits them.
    let (|PojoConstructorArgs|_|) (ctx: PluginContext) : CallInfo -> PropList option =
        function
        | { MemberRef = Some (MemberRef (EntityRef.HasPojo ctx, _) & MemberRef.MemberRefIs ctx MemberRefType.Constructor & MemberRef.ParameterNames ctx argNames)
            Args = args } ->
            let length =
                [ argNames.Length; args.Length ]
                |> List.min

            (argNames
             |> List.truncate length,
             args
             |> List.truncate length)
            ||> List.zip
            |> List.filter (
                fst
                >> Option.exists (
                    String.IsNullOrWhiteSpace
                    >> not
                )
            )
            |> List.map (fun (key, value) -> key.Value, value)
            |> Some
        | _ -> None
