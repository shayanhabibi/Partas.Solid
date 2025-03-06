namespace Partas.Solid

open System
open Fable
open Fable.AST
open Fable.AST.Fable

[<assembly: ScanForPlugins>]
do ()

/// Defines patterns for the PluginHelper which establishes whether the plugin is available
/// for use in the environment based on the language target etc
[<RequireQualifiedAccess>]
module FableRequirements =
    // Fable 4 does not detect the plugin
    let [<Literal>] version = "5.0"
    let (|Language|_|) (input: PluginHelper) =
        match input.Options.Language with
        | Language.JavaScript -> Some()
        | _ -> None
    let (|FileExtension|_|) (input: PluginHelper) =
        match input.Options.FileExtension with
        | s when s.EndsWith ".jsx" -> Some()
        | s when s.EndsWith ".js" -> Some() // Temporary allowance
        | _ -> None

/// Identifies what type of member the transformation sequence originated from
/// Currently ?unutilised
[<RequireQualifiedAccess>]
type TransformationKind =
    | MemberDecl
    | MemberCall
    | TypeMemberDecl
/// Used in Active Pattern Matches to identify if a MemberRef is one of these
[<RequireQualifiedAccess>]
type MemberRefType =
    | Setter
    | Getter
    | Constructor
    | Generated
    | None

// [<RequireQualifiedAccess>]
// type EnumType =
//     | Enumerator
//     | CopyStruct
//     | GetEnumerator
//     | Next
//     | Current
/// Idents we are interested in that impact the flow of transformation.
[<RequireQualifiedAccess>]
type IdentType =
    | ReturnVal // Compiler generated
    | Constructor // Partas type with _$ctor
    | Props // identifies props identifier
    // | Enumerator of EnumType
    // Builder - PARTAS_{...}
    | Yield
    | First
    | Second
    | Element
    | Builder
    | Delay
    | Cont
    | Value
    | Text
    // Special cases

    // Unknown
    | Other
    
/// A record which is passed through almost all transformation patterns and methods.
/// It can therefor be used to pass contextual information, or access the plugin helper
/// from within the transformation tree (to shoot out a warning or something)
type PluginContext =
    {
        Helper: PluginHelper
        Kind: TransformationKind
        SetterArray: ResizeArray<string * Expr>
        GetterArray: ResizeArray<string>
        SetterCollector: (string * Expr) -> unit
        GetterCollector: string -> unit
    }
[<RequireQualifiedAccess>]
module PluginContext =
    let create helper kind =
        let ctx = {
            Helper = helper
            Kind = kind
            SetterArray = new ResizeArray<string * Expr> [||]
            GetterArray = new ResizeArray<string> [||]
            SetterCollector = fun _  -> ()
            GetterCollector = fun _ -> ()
        }
        { ctx with SetterCollector = ctx.SetterArray.Add; GetterCollector = ctx.GetterArray.Add }
    /// Get the PluginHelper from the context    
    let helper = _.Helper
    /// Get the Entity from an EntityRef
    let getEntity = _.Helper.GetEntity
    /// Get the MemberFunctionOrValue from a MemberRef
    let getMember = _.Helper.GetMember
    /// Emit a msg on Fable compilation as a warning
    let logWarning = _.Helper.LogWarning
    /// Emit a msg on Fable compilation as an error.
    /// This will complete the transformation before emitting all collected errors.
    let logError = _.Helper.LogError
    /// <summary>
    /// Adds an attribute (or more precise to say the element <c>props</c>) property set name and the value
    /// it is being set to. This is lifted at the end of the transformations to produce a <c>solid-js</c> mergeProps.
    /// </summary>
    let addSetter = _.SetterCollector
    /// <summary>
    /// Adds an attribute (or more precise to say the element <c>props</c>) property access selector to the context.
    /// This is lifted at the end of the transformations to produce a <c>solid-js</c> splitProps.
    /// </summary>
    let addGetter = _.GetterCollector
    /// Access the setter array without clearing it
    let peekSetters = _.SetterArray.ToArray() >> Array.toList
    /// Access the getter array without clearing it
    let peekGetters = _.GetterArray.ToArray() >> Array.toList
    /// <summary>
    /// Lift the getter array values. This clears the context values for the array. Use <c>peekGetters</c> to observe
    /// the elements without clearing them.
    /// </summary>
    let getGetters (ctx: PluginContext) =
        let getters = peekGetters ctx
        ctx.GetterArray.Clear()
        getters
    /// <summary>
    /// Lift the setter array values. This clears the context values for the array. Use <c>peekSetters</c> to observe
    /// the elements without clearing them.
    /// </summary>
    let getSetters (ctx: PluginContext) =
        let setters = peekSetters ctx
        ctx.SetterArray.Clear()
        setters
    /// <summary>
    /// When disposing of an expression (ie not including it in the final transformation tree), you can apply this
    /// func with a <c>PluginContext</c> and a context string (such as in what process this is occuring, eg: 'property
    /// collection') to log a warning of the disposed expression when the fable cli is passed the <c>--verbose</c> flag. <br/>
    /// This is useful for trying to debug why an expression might not be rendering on transpilation.
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="disposalContext"></param>
    /// <param name="expr"></param>
    let debugDisposal (ctx: PluginContext) (disposalContext: string) (expr: Expr)=
        ctx
        |> helper
        |> _.Options
        |> _.Verbosity
        |> function
            | Verbosity.Silent -> ()
            | Verbosity.Normal -> ()
            | Verbosity.Verbose ->
                match expr with
                | Value(UnitConstant, _)
                | IdentExpr({ Name = "returnVal" }) -> ()
                | expr ->
                    let helper = ctx |> helper
                    let msg = $"An expression was disposed of during {disposalContext}:\n{expr}"
                    match expr.Range with
                    | Some range ->
                        helper.LogWarning(msg, range)
                    | _ -> helper.LogWarning(msg)
/// DU which holds a string of the name for the Tag, or an expression containing the name of the tag, and where it
/// should be imported from on compilation.
[<RequireQualifiedAccess>]
type TagSource =
    | AutoImport of tagName: string
    | LibraryImport of importExpr: Expr
/// <summary>
/// Alias for a <c>string * Expr</c> tuple which are compiled into an attribute key,value pair.
/// </summary>
type PropInfo = string * Expr
/// Alias for a list of PropInfos (which in turn is an alias for a string Expr tuple)
type PropList = PropInfo list

/// Helper DU to help discriminate between a tag which is just a constructor (ie no children), an empty constructor with a
/// builder computation (ie has children, but no properties), or a combination of both a constructor with properties, and
/// a builder computation (has children)
type TagInfo =
    | WithBuilder of tagName: TagSource * propsAndChildren: Expr list * range: SourceLocation option
    | Constructor of tagName: TagSource * props: PropList * range: SourceLocation option
    | Combined of tagName: TagSource * props: PropList * propsAndChildren: Expr list * range: SourceLocation option

/// The intermediary object that clearly categorizes expressions for rendering, irregardless of whether the
/// types like TagInfo are refactored
type ElementBuilder =
    {
        TagSource: TagSource
        Properties: PropList
        Children: Expr list
    }

/// Variety of useful Pattern Recognizers for dealing with strings within pattern matchers
[<RequireQualifiedAccess>]
module Helpers =
    let (|StartsWith|_|) (value: string) = function
        | (s: string) when s.StartsWith(value) -> Some()
        | _ -> None
    let (|EndsWith|_|) (value: string) = function
        | (s: string) when s.EndsWith(value) -> Some()
        | _ -> None
    let (|StartsWithTrimmed|_|) (value: string) = function
        | StartsWith value as s ->
            s.Substring(value.Length)
            |> Some
        | _ -> None
    let (|EndsWithTrimmed|_|) (value: string) = function
        | EndsWith value as s ->
            s.Substring(0, s.Length - value.Length)
            |> Some
        | _ -> None
    let rec trimReservedIdentifiers = function
        | EndsWithTrimmed "'" s -> trimReservedIdentifiers s
        | EndsWithTrimmed "`1" s -> trimReservedIdentifiers s
        | s -> s
    
/// Prebaked expressions. Most reused constructors should be lifted into this module.
module Baked =
    let importMergeProps =
        Import(
            { Selector = "mergeProps"
              Path = "solid-js"
              Kind = UserImport false },
            Type.Any,
            None
        )
    let importSplitProps =
        Import(
            {
                Selector = "splitProps"
                Path = "solid-js"
                Kind = UserImport false
            },
            Type.Any,
            None
        )
    let jsxElementType =
        Type.DeclaredType(
            ref =
                { FullName = "Fable.Core.JSX.Element"
                  Path = EntityPath.CoreAssemblyName "Fable.Core" },
            genericArgs = []
        )
    
    let spreadPropertyExpression =
        IdentExpr({
            Name = "PARTAS_OTHERS"
            Type = jsxElementType
            IsMutable = false; IsThisArgument = false; IsCompilerGenerated = true; Range = None
        })

    let convertSettersToObject (values: (string * Expr) list) (rest: Expr) =
        match values with
        | [] -> rest
        | _ ->
            Sequential [
                Expr.Set(
                    expr = IdentExpr({
                        Name = "props"
                        Type = jsxElementType
                        IsMutable = false; IsThisArgument = false; IsCompilerGenerated = true; Range = None
                    }),
                    kind = SetKind.ValueSet,
                    typ = Type.Any,
                    value = Call(
                            callee = importMergeProps,
                            info = CallInfo.Create( args = [
                                Value(
                                    kind = ValueKind.NewAnonymousRecord(
                                        values = (values |> List.map snd),
                                        fieldNames = (values |> List.map (fst >> Helpers.trimReservedIdentifiers) |> List.toArray),
                                        genArgs = [],
                                        isStruct = false
                                    ),
                                    range = None
                                )
                                IdentExpr({
                                    Name = "props"
                                    Type = jsxElementType
                                    IsMutable = false; IsThisArgument = false; IsCompilerGenerated = true; Range = None
                                })
                            ]),
                            typ = Type.Any,
                            range = None
                        ),
                    range = None
                )
                rest
            ]
    let convertGettersToObject (values: string list) (rest: Expr) =
        Expr.Let(
            ident =
                {
                    Name = "[PARTAS_LOCAL, PARTAS_OTHERS]"
                    Type = Type.Any
                    IsMutable = false
                    IsThisArgument = false
                    IsCompilerGenerated = false
                    Range = None
                },
            value =
                Expr.Call(
                    callee = importSplitProps,
                    info =
                        CallInfo.Create(
                            args = [
                                IdentExpr({
                                    Name = "props"
                                    Type = jsxElementType
                                    IsMutable = false; IsThisArgument = false; IsCompilerGenerated = true; Range = None
                                })
                                Value(
                                    kind =
                                        ValueKind.NewArray(
                                            newKind = NewArrayKind.ArrayValues([
                                                for value in values do
                                                    Value(StringConstant(value), None)
                                            ]),
                                            typ = Type.Any,
                                            kind = ArrayKind.ImmutableArray
                                        ),
                                    range = None
                                )
                            ]
                        ),
                    typ = Type.Any,
                    range = None
                ),
            body = rest
        )
    let importJsxCreate = Import(
            info = {
                Selector = "create"
                Path = "@fable-org/fable-library-js/JSX.js"
                Kind = ImportKind.UserImport false
            },
            typ = Type.Any,
            range = None
        )
    let listItemType = Type.Tuple(genericArgs = [ Type.String; Type.Any ], isStruct = false)
    let emptyList = Value(kind = NewList(headAndTail = None, typ = listItemType), range = None)
    let flattenExpressions (exprs: Expr list) =
        (emptyList, exprs)
        ||> List.fold (fun acc prop ->
            Value(kind = NewList(headAndTail = Some(prop, acc), typ = listItemType), range = None))
    let propGetter (getTarget: string) =
        Get(
            IdentExpr
                { Name = "PARTAS_LOCAL"
                  Type = jsxElementType
                  IsMutable = false
                  IsThisArgument = true
                  IsCompilerGenerated = true
                  Range = None },
            GetKind.ExprGet(Value(StringConstant(Helpers.trimReservedIdentifiers getTarget), None)),
            Type.Any,
            None
        )
    let wrapChildrenExpression childrenExpression =
        Value(
            kind =
                NewTuple(
                    values =
                        [ Value(kind = StringConstant "children", range = None)
                          TypeCast(childrenExpression, Type.Any)

                          ],
                    isStruct = false
                ),
            range = None
        )
module internal rec AST =
    module MemberRef =
        let (|MemberRefIs|) (ctx: PluginContext): MemberRef -> MemberRefType = function
            | MemberRef({FullName = Helpers.StartsWith "Partas.Solid"}, _) as memberRef ->
                let mref = memberRef |> PluginContext.getMember ctx
                if mref.IsConstructor then MemberRefType.Constructor
                elif mref.IsSetter then MemberRefType.Setter
                elif mref.IsGetter then MemberRefType.Getter
                else MemberRefType.None
            | GeneratedMemberRef(_) -> MemberRefType.Generated
            | _ -> MemberRefType.None
        let (|PartasName|_|) (ctx: PluginContext): MemberRef -> string option = function
            | MemberRef(_, { CompiledName = compiledName }) ->
                compiledName
                |> _.Split('.')
                |> Array.rev
                |> Array.head
                |> function
                    | Helpers.StartsWithTrimmed "set_" memberName
                    | Helpers.StartsWithTrimmed "get_" memberName
                    | Helpers.EndsWithTrimmed "_$ctor" memberName
                    | memberName -> memberName |> Helpers.trimReservedIdentifiers |> Some
            | GeneratedMemberRef(_) -> None
        module Option =
            let (|PartasName|_|) (ctx: PluginContext): MemberRef option -> string option = function
                | Some(AST.MemberRef.PartasName ctx name) -> Some name
                | _ -> None
    /// Contains active patterns to match expressions against common patterns such as an imported constructor,
    /// or a setter of an imported property etc.
    /// Consider the Expr.ImportedConstructor as an example.
    /// They do not recognize constructors in the sense of TagConstructors. They will recognize any constructor which
    /// is library imported, or module imported.
    module Expr =
        let (|ImportedConstructor|_|) (ctx: PluginContext) = function
            | Import({
                    Kind = UserImport _
                }, Any, _) -> Some()
            | Import(
                {
                    Kind = MemberImport(MemberRef.MemberRefIs ctx MemberRefType.Constructor)
                },
                _,
                _ ) -> Some()
            | _ -> None
        let (|NativeImportedConstructor|_|) (ctx: PluginContext) = function
            | Import({Kind = MemberImport(MemberRef({ FullName = Helpers.StartsWith "Partas.Solid.Tags" ctx }, _))}, _, _)
                & ImportedConstructor ctx ->
                Some()
            | _ -> None
        let (|ImportedSetter|_|) (ctx: PluginContext) = function
            | Import(
                {
                    Selector = Helpers.StartsWith "Partas_Solid"
                    Kind = MemberImport( MemberRef.MemberRefIs ctx MemberRefType.Setter )
                },
                _,
                _ ) ->
                Some()
            | _ -> None
        let (|ImportedGetter|_|) (ctx: PluginContext) = function
            | Import(
                {
                    Selector = Helpers.StartsWith "Partas_Solid"
                    Kind = MemberImport( MemberRef.MemberRefIs ctx MemberRefType.Getter)
                },
                _,
                _)  -> Some()
            | _ -> None
        let (|ImportedExtensionName|_|) (ctx: PluginContext) = function
            | Import(
                {
                    Selector = Helpers.StartsWith "HtmlElementExtensions_"
                    Kind = MemberImport(
                            ( MemberRef.MemberRefIs ctx MemberRefType.Setter
                            & MemberRef(_, { CompiledName = compiledName }))
                        )
                },
                _,
                _
                ) -> Some compiledName
            | _ -> None
        let (|ImportedContainerExtensionName|_|) (ctx: PluginContext) = function
            | Import(
                {
                    Selector = Helpers.StartsWith "HtmlContainerExtensions_" | Helpers.StartsWith "BindingsModule_Extensions"
                    Kind = MemberImport(MemberRef.PartasName ctx _  & MemberRef(_, { CompiledName = compiledName }))
                },
                _,
                _
                ) -> Some compiledName
            | _ -> None

    module Ident =
        /// where we can, we use ridiculous names in computations so that the chance of user AST
        /// accidentally being transformed as one of these patterns is almost nil.
        let (|IdentIs|) (ctx: PluginContext): Ident -> IdentType = function
            | { Name = Helpers.StartsWith "returnVal"; Type = Type.PartasName ctx _ } -> IdentType.ReturnVal
            | { Name = Helpers.EndsWith "_$ctor"; Type = Type.PartasName ctx _ } -> IdentType.Constructor
            | { Name = "props"; IsThisArgument = true; Type = Type.PartasName ctx _ } -> IdentType.Props
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
            | { Name = Helpers.StartsWith "PARTAS_YIELD"; Type = Type.PartasName ctx _ } -> IdentType.Yield
            | { Name = Helpers.StartsWith "PARTAS_FIRST" } -> IdentType.First
            | { Name = Helpers.StartsWith "PARTAS_SECOND" } -> IdentType.Second
            | { Name = Helpers.StartsWith "PARTAS_ELEMENT" } -> IdentType.Element
            | { Name = Helpers.StartsWith "PARTAS_BUILDER"; Type = Type.PartasName ctx _ } -> IdentType.Builder
            | { Name = Helpers.StartsWith "PARTAS_DELAY" } -> IdentType.Delay
            | { Name = Helpers.StartsWith "PARTAS_CONT" } -> IdentType.Cont
            | { Name = Helpers.StartsWith "PARTAS_VALUE" } -> IdentType.Value
            | { Name = Helpers.StartsWith "PARTAS_TEXT" } -> IdentType.Text
            | _ -> IdentType.Other
    module Type =
        /// Recursively explores a `Type` AST node until either returning the tail part of a DeclaredType fullname,
        /// or none. It can therefor also be used to ensure the type starts with `Partas.Solid`
        let (|PartasName|_|) (ctx: PluginContext): Type -> string option = function
            | Type.DeclaredType({ FullName = Helpers.StartsWith "Partas.Solid" as fullName }, _) ->
                fullName
                |> _.Split('.')
                |> Array.rev
                |> Array.head
                |> Helpers.trimReservedIdentifiers
                |> Some
            | Type.Array(PartasName ctx tagSource, _) -> Some tagSource
            | Type.List(PartasName ctx tagSource) -> Some tagSource
            | Type.Option(PartasName ctx tagSource, _) -> Some tagSource
            | Type.Tuple(PartasName ctx tagSource :: _, _) -> Some tagSource
            | Type.DelegateType(_, PartasName ctx tagSource) -> Some tagSource
            | Type.LambdaType(_, PartasName ctx tagSource) -> Some tagSource
            | _ -> None
    /// Contains patterns to match CallInfo's with common member ref or thisarg patterns.
    /// Honestly, they're only used probably once each in current iterations, but I think
    /// it helps with the readability of the more complicated patterns.
    module CallInfo =
        let (|PropertySetter|_|) (ctx: PluginContext) = function
            | { ThisArg = Some(IdentExpr(Ident.IdentIs ctx IdentType.Props)) } ->
                Some()
            | _ -> None
        let (|Constructor|_|) (ctx: PluginContext) = function
            | { MemberRef = Some(MemberRef.MemberRefIs ctx MemberRefType.Constructor) } ->
                Some()
            | _ -> None
        let (|ExtensionHandler|_|) (ctx: PluginContext) = function
            | { MemberRef = Some(MemberRef.PartasName ctx _
                                 & MemberRef(_, { CompiledName = eventName }))
                Args = [ handler ] }
            | { MemberRef = Some(MemberRef.PartasName ctx _)
                Args = [ Value(StringConstant eventName, _); handler ] } ->
                Some(eventName, transform ctx handler)
            | _ -> None
    [<AutoOpen>]
    module AttributesAndProperties =
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
                | { MemberRef = MemberRef.Option.PartasName ctx name
                    Args = expr :: _ } ->
                    Some(name, expr)
                | _ ->
                    None
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
        let private (|PropertyGetter|_|) (ctx: PluginContext) = function
            | Call(
                callee,
                {
                    ThisArg = Some(IdentExpr(Ident.IdentIs ctx IdentType.Props))
                    Args = [ _ ]
                    MemberRef = MemberRef.Option.PartasName ctx prop
                } & { MemberRef =Some(MemberRef.MemberRefIs ctx MemberRefType.Getter) },
                typ,
                range) as expr ->
                Some(prop)
            | _ -> None
        // Collects and transforms any of our 'special' `props` usage so that we can
        // later create the splitProps and mergeProps to suit.
        let (|PropsGetterOrSetter|_|) (ctx: PluginContext) = function
            | PropertyGetter ctx prop ->
                PluginContext.addGetter ctx prop
                Baked.propGetter prop |> Some
            | PropertySetter ctx (prop, expr) ->
                PluginContext.addSetter ctx (prop, transform ctx expr)
                Expr.Value(ValueKind.Null(Type.Any), None)
                |> Some
            | _ -> None
        // Determines at top level whether the current expression could be considered
        // as a attribute expression by any of our definitions
        let (|AttributeExpression|_|) (ctx: PluginContext) = function
            | Call(
                callee,
                {
                    ThisArg = Some(IdentExpr(Ident.IdentIs ctx IdentType.ReturnVal))
                    Args = expr :: _
                    MemberRef = MemberRef.Option.PartasName ctx prop as memberRef
                },
                _,
                _) ->
                match callee, memberRef with
                | Expr.ImportedSetter ctx, _ // Captures properties defined in other modules
                | _, Some(MemberRef.MemberRefIs ctx MemberRefType.Setter) -> // Captures properties defined in self module
                    (prop, transform ctx expr)
                    |> Some
                | _, _ -> None
            // Captured method/Extension call
            | Call(
                Value(ValueKind.UnitConstant, None),
                {
                    MemberRef = Some(MemberRef(_, { CompiledName = extensionName }))
                    Args = exprs
                },
                Type.MetaType,
                None
                ) -> // TODO - handlers for the different extensions that are supported
                let fable4 = ctx |> PluginContext.helper |> _.Options |> _.Define |> List.contains "FABLE_COMPILER_4"
                let fable5 = ctx |> PluginContext.helper |> _.Options |> _.Define |> List.contains "FABLE_COMPILER_5"
                match extensionName, exprs with
                | (
                    "on" | "bool" | "data" | "attr"
                    ), [ Value(StringConstant prop, _); value ] ->
                    let propName =
                        match extensionName with
                        | "bool" | "on" -> $"{extensionName}:{prop}"
                        | "data" -> $"data-{prop}" 
                        | "attr" -> prop
                        | _ -> failwith "Unreachable"
                    Some(propName, transform ctx value)
                | ("ref" | "style'" | "classList"), [ identExpr ] ->
                    Some(extensionName |> Helpers.trimReservedIdentifiers, transform ctx identExpr)
                | "spread", _ when fable4 ->
                    $"Spread is not supported in fable4"
                    |> PluginContext.logError ctx
                    None
                | "spread", [ IdentExpr(Ident.IdentIs ctx IdentType.Props) | TypeCast(IdentExpr(Ident.IdentIs ctx IdentType.Props), _) ]
                    when fable5 ->
                    Some("{...PARTAS_OTHERS} bool:n$", Value(ValueKind.BoolConstant(false), None))
                | "spread", [ (TypeCast(IdentExpr({ Name = ident }), _) | IdentExpr({ Name = ident })) ]
                    when fable5 ->
                    Some("{..." + ident + "} bool:n$", Value(ValueKind.BoolConstant(false), None))
                | "spread", [ expr ]
                    when fable5 ->
                    $"Spread does not support this as a value in Fable 4 or below:\n{expr}"
                    |> PluginContext.logError ctx
                    None
                // -- FEATURE NOT PRESENT IN FABLE COMPILER YET -- //
                // | "spread", [ identExpr ] when fable5 -> 
                //     Some("__SPREAD_PROPERTY__", identExpr)
                | _ ->
                    $"Unhandled extension: {extensionName}\nProvidedValues: {exprs}"
                    |> PluginContext.logError ctx
                    None
            | Call(
                Value(ValueKind.UnitConstant, None),
                {
                    MemberRef = (
                        Some(MemberRef.MemberRefIs ctx MemberRefType.Setter)
                         & MemberRef.Option.PartasName ctx propName
                    )
                    Args = value :: _
                }, _, _) ->
                let transformedPropName =
                    match propName with
                    | Helpers.StartsWithTrimmed "aria" propName ->
                        $"aria-{propName.ToLower()}"
                    | _ -> propName
                Some(transformedPropName, transform ctx value)
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
        // Local tag
        | Call(
            (IdentExpr(Ident.IdentIs ctx IdentType.Constructor)),
            (CallInfo.Constructor ctx & {
                Args = PropCollector ctx props
            }),
            ((Type.PartasName ctx (Helpers.EndsWithTrimmed "_$ctor" typeName)) | (Type.PartasName ctx typeName)),
            range) ->
            TagInfo.Constructor( TagSource.AutoImport typeName, props, range)
            |> Some
        // Native import
        | Call(
            (Expr.NativeImportedConstructor ctx),
            {
                Args = PropCollector ctx props
            },
            (Type.PartasName ctx typeName),
            range) ->
            TagInfo.Constructor (TagSource.AutoImport typeName, props, range)
            |> Some
        // Library Imports
        | Call(
            (Expr.ImportedConstructor ctx & Import({Kind = UserImport false}, _, _) as imp),
            {
                Args = PropCollector ctx props
            },
            (Type.PartasName ctx typeName),
            range) ->
            TagInfo.Constructor (TagSource.LibraryImport imp, props, range)
            |> Some
        // Non LibraryImports that require LibraryImport injection
        | Call(
            (Expr.ImportedConstructor ctx
             & Import({ Selector = ( Helpers.StartsWith "BindingsModule_Route"
                                   | Helpers.StartsWith "BindingsModule_HashRoute") }, t, r)),
            {
                Args = PropCollector ctx props
            },
            (Type.PartasName ctx typeName),
            range) ->
            let importExpr =
                Import(
                    { Selector = typeName
                      Path = "@solidjs/router"
                      Kind = UserImport false },
                    t, r)
            TagInfo.Constructor (TagSource.LibraryImport importExpr, props, range)
            |> Some
        // Non LibraryImports; ie User defined imports
        | Call(
            (Expr.ImportedConstructor ctx & Import(identee, t, r)),
            {
                Args = PropCollector ctx props
            },
            (Type.PartasName ctx typeName),
            range) ->
            let importExpr = Import({identee with Selector = typeName}, t, r)
            TagInfo.Constructor (TagSource.LibraryImport importExpr, props, range)
            |> Some
        // WITH PROPS
        | Let(
            Ident.IdentIs ctx (IdentType.ReturnVal | IdentType.Element),
            TagConstructor ctx tagInfo,
            PropCollectorFeeder ctx bodyProps
            ) ->
            match tagInfo with
            | TagInfo.Combined(tagSource, props, propsAndChildren, range) ->
                TagInfo.Combined(tagSource, bodyProps @ props, propsAndChildren, range)
            | TagInfo.Constructor(tagSource, props, range) ->
                TagInfo.Constructor(tagSource, bodyProps @ props, range)
            | TagInfo.WithBuilder(tagSource, propsAndChildren, range) ->
                TagInfo.Combined(tagSource, bodyProps, propsAndChildren, range)
            |> Some
        // WITH PROPS & EXTENSION
        | Call(Import({ Kind = MemberImport(MemberRef({ FullName = Helpers.EndsWith "HtmlElementExtensions" }, _)) }, _, _) as callee, ({
                Args = TagConstructor ctx tagInfo :: rest 
            } as callInfo), _, _) ->
            // We compile all the information of the extension call into the callinfo and make the rest almost impossible to mistake
            // for a different type of expression. We can then add this to the property list
            [ Call(Expr.Value(ValueKind.UnitConstant, None), { callInfo with Args = rest }, Type.MetaType, None) ]
            |> function
                | PropCollector ctx attributeCall ->
                    match tagInfo with
                    | TagInfo.Combined(tagSource, props, propsAndChildren, range) ->
                        TagInfo.Combined(tagSource, attributeCall @ props, propsAndChildren, range)
                    | TagInfo.Constructor(tagSource, props, range) ->
                        TagInfo.Constructor(tagSource, attributeCall @ props, range)
                    | TagInfo.WithBuilder(tagSource, propsAndChildren, range) ->
                        TagInfo.Combined(tagSource, attributeCall, propsAndChildren, range)
                    |> Some
        // WITH CHILDREN
        | Call(
            Expr.ImportedContainerExtensionName ctx "Run", // run is being called. builder being applied to obj. obj is arg 1.
            {   // Check the first argument, which is the argument this is being called against, for the constructor which called it.
                Args = TagConstructor ctx tagInfo :: BuilderCollector ctx rest // The rest are the computations being applied
                // The rest need to be transformed, pass them back to the transformer.
            },
            typ,
            range) ->
            // 
            match tagInfo with
            | TagInfo.Combined(tagSource, props, propsAndChildren, range) ->
                TagInfo.Combined(tagSource, props, rest @ propsAndChildren, range)
            | TagInfo.Constructor(tagSource, props, range) ->
                TagInfo.Combined(tagSource, props, rest, range)
            | TagInfo.WithBuilder(tagSource, propsAndChildren, range)  ->
                TagInfo.WithBuilder(tagSource, rest @ propsAndChildren, range)
            |> Some
        
        | Let(Ident.IdentIs ctx IdentType.Element, TagConstructor ctx tagInfo, BuilderCollectorFeedback ctx body) ->
            match tagInfo with
            | TagInfo.WithBuilder(tagSource, propsAndChildren, range) ->
                TagInfo.WithBuilder(tagSource, body @ propsAndChildren, range )
            | TagInfo.Combined(tagSource, props, propsAndChildren, range) ->
                TagInfo.Combined(tagSource, props, body @ propsAndChildren, range)
            | TagInfo.Constructor(tagSource, props, range) ->
                TagInfo.Combined(tagSource, props, body, range)
            |> Some
        | _ -> None
    let renderElement (ctx: PluginContext) (builder: ElementBuilder) =
        let renderProp ((name: string, expr: Expr): PropInfo) = Value(NewTuple([ Value(StringConstant name, None); TypeCast(expr, Type.Any) ], false), None)
        let renderPropList = List.map renderProp
        let renderTagName = function
            | TagSource.LibraryImport imp -> imp
            | TagSource.AutoImport name -> Value(StringConstant name, None)
        let internalCollection =
            (builder.Children |> List.rev |> Baked.flattenExpressions |> Baked.wrapChildrenExpression)
            :: renderPropList builder.Properties
            |> Baked.flattenExpressions
        Call(
                callee = Baked.importJsxCreate,
                info =
                    { ThisArg = None
                      Args = [ renderTagName builder.TagSource ; internalCollection  ]
                      SignatureArgTypes = []
                      GenericArgs = []
                      MemberRef = None
                      Tags = [ "jsx" ] },
                typ = Baked.jsxElementType,
                range = None
            )
    /// Collates TagInfos into ElementBuilders which can then be rendered via `renderElement`
    let collectTagInfo (ctx: PluginContext): TagInfo -> ElementBuilder = function
        | TagInfo.WithBuilder(tagSource, BuilderCollector ctx propsAndChildren, range) ->
            { TagSource = tagSource; Children = propsAndChildren; Properties = [] }
        | TagInfo.Combined(tagSource, props, BuilderCollector ctx propsAndChildren, range) ->
            { TagSource = tagSource; Children = propsAndChildren; Properties = props }
        | TagInfo.Constructor(tagSource, props, range) ->
            { TagSource = tagSource; Children = []; Properties = props }
    

            
    /// Transforms a single expression before wrapping it in a list and feeding it back to the BuilderCollector.
    let (|BuilderCollectorFeedback|) (ctx: PluginContext): Expr -> Expr list = fun e -> [ transform ctx e ] |> function
        | BuilderCollector ctx feedback -> feedback
    
    // I'll be very honest, and I can really use some help here,
    // I have no ****ing idea how this pattern correctly organises
    // the expressions. A review is appreciated :) just note
    // that I reverse the list on render - TODO
    /// This active recognizer is a bit more psychotic in terms of recursion
    let (|BuilderCollector|) (ctx: PluginContext): Expr list -> Expr list = function
        | [] -> []
        | Sequential (BuilderCollector ctx headBuilds) :: BuilderCollector ctx tailBuilds ->
            headBuilds @ tailBuilds
        | expr :: BuilderCollector ctx restBuilds ->
            match expr with
            // | Call(
            //     (Get(IdentExpr(Ident.IdentIs ctx IdentType.RouterBuilder), _, _, _)
            //    | Import({ Selector = "uncurry2" }, Any, None)),
            //     { Args = BuilderCollector ctx body },
            //     _, _) ->
            //     body @ restBuilds
            | Let(Ident.IdentIs ctx (IdentType.Element | IdentType.First | IdentType.Text),
                  BuilderCollectorFeedback ctx value,
                  BuilderCollectorFeedback ctx body
                  ) ->
                    value @ body @ restBuilds
            | Let(
                Ident.IdentIs ctx (IdentType.Builder | IdentType.Second),
                BuilderCollectorFeedback ctx value,
                BuilderCollectorFeedback ctx body
                ) ->
                    body @ value @ restBuilds
            | CurriedApply(BuilderCollectorFeedback ctx applied, BuilderCollector ctx args, typ, range) ->
                if args.IsEmpty then applied @ restBuilds
                else CurriedApply(applied.Head, args, typ, range) :: restBuilds
                // args @ applied @ restBuilds
            | Lambda(
                Ident.IdentIs ctx IdentType.Cont,
                TypeCast(Lambda(item, Lambda(index, BuilderCollectorFeedback ctx expr, _), _), _),
                _
                ) ->
                let getHead = List.tryHead >> Option.defaultValue (Value(UnitConstant, None))
                Delegate([ item; index ], getHead expr, None, []) :: restBuilds
            | Lambda(
                Ident.IdentIs ctx (IdentType.Builder | IdentType.Yield | IdentType.Cont),
                BuilderCollectorFeedback ctx expr,
                _
                ) -> expr @ restBuilds
            | IdentExpr(Ident.IdentIs ctx IdentType.Other) ->
                expr :: restBuilds // wtf is this? ok. go on through sir.
            | TypeCast(Value(StringConstant _, _) as text, Unit) ->
                 text :: restBuilds
            | TypeCast(BuilderCollectorFeedback ctx expr, typ) ->
                expr @ restBuilds
            | IfThenElse(
                BuilderCollectorFeedback ctx guardExpr,
                BuilderCollectorFeedback ctx thenExpr,
                BuilderCollectorFeedback ctx elseExpr,
                range) ->
                let getHead = List.tryHead >> Option.defaultValue (Value(UnitConstant, None))
                [ IfThenElse(
                    getHead guardExpr,
                    getHead thenExpr,
                    getHead elseExpr,
                    range) ] @ restBuilds
            | Get(BuilderCollectorFeedback ctx exprs, _, _, _) ->
                let head = exprs |> (List.tryHead >> Option.defaultValue (Value(UnitConstant, None)))
                head :: restBuilds
            
            // This is one of our builder identifiers; no reason it should be rendered.
            | IdentExpr(_)
            // Some transformation said 'na'.
            | Value(UnitConstant, None) -> restBuilds // You have been judged unworthy
            // Trust the F# compiler to not allow invalid elements within the builder.
            | _ ->
                expr :: restBuilds
                
    // Have an expression? Don't know what to do with it? Let me gobble gobble sir.
    let transform (ctx: PluginContext) (expr: Expr)=
        match expr with
        | TagConstructor ctx tagInfo ->
            tagInfo |> collectTagInfo ctx |> renderElement ctx
        | Sequential exprs ->
            exprs
            |> List.map (transform ctx)
            |> Sequential
        | PropsGetterOrSetter ctx expr ->
            expr
        | IfThenElse(guardExpr, thenExpr, elseExpr, range) ->
            IfThenElse(
                transform ctx guardExpr,
                transform ctx thenExpr,
                transform ctx elseExpr,
                range )
        | DecisionTree(decisionTree, targets) ->
            DecisionTree(
                decisionTree,
                targets
                |> List.map (fun (target, expr) -> target, transform ctx expr)
                )
        | TypeCast(expr, DeclaredType _) -> transform ctx expr
        // We SHOULD NOT be capturing attribute expressions at this level. This is the only pattern that we don't want
        // gobble gobbled. But... Perhaps this points to redundancy in property collection.
        | AttributeExpression ctx propInfo ->
            PluginContext.logWarning ctx $"Attribute expression was parsed at the top level and was reduced out:\n{propInfo}"
            snd propInfo
        | CurriedApply(applied, exprs, typ, range) ->
            CurriedApply(transform ctx applied, exprs |> List.map (transform ctx), typ, range )
        | Delegate(args, body, name, tags) ->
            Delegate(args, transform ctx body, name, tags)
        // | Lambda(ident, expr, name) ->
        //     Lambda(ident, transform ctx expr, name)
        // | CurriedApply(expr, _, _, _) ->
        //     transform ctx expr
        | Let(ident, value, body) ->
            Let(ident, transform ctx value, transform ctx body)
        | Call(callee, callInfo, typ, range) ->
            Call(callee, { callInfo with Args = callInfo.Args |> List.map (transform ctx) }, typ, range)
            
        | _ as expr -> expr
        //     Value(UnitConstant, None)
        // // ^ use this as a result stub when constructing patterns to prevent errors obfuscating the screen

(*
    The AST goes as follows:
        Without the builder, (aka without children) tags are just
        Calls to a constructor of the tag type. The constructor call will have the
        properties and what not within its args. Any method calls are displayed as
        such as the callee. The first arg of this method call will be the object
        to which it is applied (ie the constructor) and thereafter the values are
        the arguments.
*)
module internal rec SchemaRules =
    let (|ValidMemberRef|_|) (ctx: PluginContext) (memberDecl: MemberDecl) =
        match memberDecl with
        | { MemberRef = MemberRef(
                ({ FullName = Helpers.StartsWith "Partas.Solid" } as declaringEntity),
                ({ IsInstance = true; NonCurriedArgTypes = Some([Type.Unit]) } as entityRef)
            ) } ->
            match memberDecl.Args with
            | [
                   {Name = "props"; IsThisArgument = true}
                   {Name = Helpers.StartsWith "unitVar"}
              ] -> declaringEntity.DisplayName |> Some
            | _ ->
                PluginContext.helper ctx
                |> _.LogWarning("The self identifier must be named `props`, and no arguments must be provided", memberDecl.Args |> List.randomChoice |> _.Range.Value)
                None 
        | _ ->
            PluginContext.helper ctx
            |> _.LogWarning("This is an invalid member declaration for this attribute. Ensure it follows the following pattern: `member props.typeDef =`. Ensure it is declared within a namespace starting with `Partas.Solid`", memberDecl.Args |> List.randomChoice |> _.Range.Value)
            None

type SolidTypeComponentAttribute() =
    inherit MemberDeclarationPluginAttribute()
    override _.FableMinimumVersion = FableRequirements.version
    override this.Transform(pluginHelper, file, memberDecl) =
        // Console.WriteLine "\nSTART MEMBER DECL!!!"
        // Console.WriteLine memberDecl.Body
        // Console.WriteLine "END MEMBER DECL!!!\n"
        let ctx = PluginContext.create pluginHelper TransformationKind.TypeMemberDecl
        match memberDecl with
        | SchemaRules.ValidMemberRef ctx finalName ->
            let newExpr =
                memberDecl.Body
                |> AST.transform ctx
                |> Baked.convertGettersToObject (PluginContext.getGetters ctx)
                |> Baked.convertSettersToObject (PluginContext.getSetters ctx)
            { memberDecl with Body = newExpr; Name = finalName }
        | _ ->
            $"Unsupported definitions under this attribute are not recommended. Please use `[<SolidComponent>]` instead"
            |> PluginContext.logWarning ctx
            {
                memberDecl with
                    Body = memberDecl.Body
                           |> AST.transform ctx
            }
    override this.TransformCall(pluginHelper, memb, expr) =
        let ctx = PluginContext.create pluginHelper TransformationKind.MemberCall
        expr

/// Applies Solid transformations to `let` bindings.
/// Use SolidTypeComponentAttribute for Type member definitions such
/// as `member props.typeDef =`.
type SolidComponentAttribute() =
    inherit MemberDeclarationPluginAttribute()
    override _.FableMinimumVersion = FableRequirements.version
    override this.Transform(pluginHelper, file, memberDecl) =
        let ctx = PluginContext.create pluginHelper TransformationKind.MemberDecl
        {
            memberDecl with
                Body = memberDecl.Body |> AST.transform ctx
        }
    override this.TransformCall(pluginHelper, memb, expr) =
        expr
