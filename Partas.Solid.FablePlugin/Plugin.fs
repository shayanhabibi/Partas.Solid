namespace Partas.Solid

// This will take a different approach of being more contextually driven and parsing deep into expression trees.
// The returned tree will be ordered appropriately by default.
// We will attempt to handle all edge cases through thorough testing.

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
    let [<Literal>] version = "4.0"
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
[<RequireQualifiedAccess; Struct>]
type TransformationKind =
    | MemberDecl
    | MemberCall
    | TypeMemberDecl

[<RequireQualifiedAccess>]
type MemberRefType =
    | Setter
    | Getter
    | Constructor
    | Generated
    | None

[<RequireQualifiedAccess>]
type IdentType =
    | ReturnVal
    | Constructor
    | Props
    | Other

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
        
    let helper = _.Helper
    let getEntity = _.Helper.GetEntity
    let getMember = _.Helper.GetMember
    let logWarning = _.Helper.LogWarning
    let logError = _.Helper.LogError
    let addSetter = _.SetterCollector
    let addGetter = _.GetterCollector
    let peekSetter = _.SetterArray.ToArray() >> Array.toList
    let peekGetter = _.GetterArray.ToArray() >> Array.toList
    let getGetters (ctx: PluginContext) =
        let getters = peekGetter ctx
        ctx.GetterArray.Clear()
        getters
    let getSetters (ctx: PluginContext) =
        let setters = peekSetter ctx
        ctx.SetterArray.Clear()
        setters
    let debugDisposal (ctx: PluginContext) (disposalContext: string) (expr: Expr)=
        ctx
        |> helper
        |> _.Options
        |> _.Verbosity
        |> function
            | Verbosity.Silent -> ()
            | Verbosity.Verbose -> ()
            | Verbosity.Normal ->
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
[<RequireQualifiedAccess>]
type TagSource =
    | AutoImport of tagName: string
    | LibraryImport of importExpr: Expr

module TagSource =
    module AutoImport =
        let create value = TagSource.AutoImport value
    module LibraryImport =
        let create value = TagSource.LibraryImport value
    let transform = function
        | TagSource.AutoImport tagName ->
            Value(StringConstant(tagName), None)
        | TagSource.LibraryImport expr ->
            expr

type PropInfo = string * Expr
type PropList = PropInfo list

type TagInfo =
    | WithBuilder of tagName: TagSource * propsAndChildren: CallInfo * range: SourceLocation option
    | Constructor of tagName: TagSource * props: Expr list * range: SourceLocation option
    | Combined of tagName: TagSource * props: Expr list * propsAndChildren: CallInfo * range: SourceLocation option

type ElementBuilder =
    {
        TagSource: TagSource
        Properties: PropList
        Children: Expr list
    }

module ElementBuilder =
    let bindTagSource from target = { target with TagSource = from.TagSource }
    let appendProperties from target = { target with Properties = target.Properties @ from.Properties }
    let appendChildren from target = { target with Children = target.Children @ from.Children }
    let combine from target =
        target
        |> bindTagSource from
        |> appendProperties from
        |> appendChildren from
    let addChild child target = { target with Children = child :: target.Children }
    let addProperty prop target = { target with Properties = prop :: target.Properties }

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
    let rec (|StartsWithEither|_|) (possibles: string list) (input: string) =
        possibles
        |> List.exists input.StartsWith
    let rec (|EndsWithEither|_|) (possibles: string list) (input: string) =
        possibles
        |> List.exists input.EndsWith
    let getTailOfFullName (fullName: string) =
        fullName.Split('.')
        |> Array.rev
        |> Array.head
        |> trimReservedIdentifiers
    
/// Prebaked expressions
module Baked =
    let renderProp ((name: string, expr: Expr): PropInfo) = Value(NewTuple([ Value(StringConstant name, None); TypeCast(expr, Type.Any) ], false), None)
    let renderAttribute (name: string, expr: Expr): Expr = renderProp (name, expr)
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
            Name = "others"
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
                    Name = "[local, others]"
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
                { Name = "local"
                  Type = jsxElementType
                  IsMutable = false
                  IsThisArgument = true
                  IsCompilerGenerated = true
                  Range = None },
            GetKind.ExprGet(Value(StringConstant(Helpers.trimReservedIdentifiers getTarget), None)),
            Type.Any,
            None
        )
module internal rec AST =
    // Broadly speaking, the transformation should prioritise capturing Exprs which are resulting in tags and
    // redirecting them to a different transformation tree.
    // Otherwise, all exprs will be stepped through and changed appropriately if required.
    // The following must all be changed:
    // * accessors to props
    // * conditionals and inner expressions that can potentially be tags
    // * setters to props
   
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
            

    module Ident =
        let private (|PartasSolidType|_|) (ctx: PluginContext): Type -> unit option = function
            | DeclaredType({ FullName = Helpers.StartsWith "Partas.Solid" }, _)
            | LambdaType(_, PartasSolidType ctx ) -> Some()
            | _ -> None
        let (|IdentIs|) (ctx: PluginContext): Ident -> IdentType = function
            | { Name = Helpers.StartsWith "returnVal"; Type = Type.PartasName ctx _ } -> IdentType.ReturnVal
            | { Name = Helpers.EndsWith "_$ctor"; Type = Type.PartasName ctx _ } -> IdentType.Constructor
            | { Name = "props"; IsThisArgument = true; Type = Type.PartasName ctx _ } -> IdentType.Props
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
    /// <summary>
    /// Recognizes whether current expression is considered a prop setter by
    /// any of our definitions.
    /// </summary>
    /// <remarks>
    /// It is imperative that this recognizer comes AFTER the PropertyGetter
    /// since it is greedier and will consume and nullify attribute expressions
    /// that involve the use of the properties argument.
    /// </remarks>
    let (|PropertySetter|_|) (ctx: PluginContext) = function
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
    let (|PropertyGetter|_|) (ctx: PluginContext) = function
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
        | _ -> None
    
    (*
    The tag constructor recognizer actively matches ANY expression pattern that, according to our
    scheme, should be transpiled into a JSX tag. This must occur from within this recognizer, regardless
    of whether that expression is a method chained to a tag or other.
    
    This is imperative for logical flow.
    ie:
    INPUT: Transform expr
    1) Is Getter? Lift target and render prop accessor
    2) Is Setter? Lift target and render null
    3) Is tag constructor? transform contents and render element. Special render pattern for attributes.
    4) Is Attribute Expression? These should not render from here as they must be
        within a constructor. Log error.
    5) Expand expression, recursive transformation, or emit
    *)
    let (|TagConstructor|_|) (ctx: PluginContext) = function
        | Call(
            (IdentExpr(Ident.IdentIs ctx IdentType.Constructor)),
            (CallInfo.Constructor ctx as callInfo),
            ((Type.PartasName ctx (Helpers.EndsWithTrimmed "_$ctor" typeName)) | (Type.PartasName ctx typeName)),
            range) ->
            TagInfo.Constructor( TagSource.AutoImport typeName, callInfo.Args, range)
            |> Some
        | Call(
            (Expr.ImportedConstructor ctx & Import(importee, t, r) ),
            callInfo,
            (Type.PartasName ctx typeName),
            range) ->
            TagInfo.Constructor (TagSource.LibraryImport (Import({ importee with Selector = typeName }, t, r)), callInfo.Args, range)
            |> Some
        // WITH PROPS - TODO
        | Let(Ident.IdentIs ctx IdentType.ReturnVal, TagConstructor ctx tagInfo, body) ->
            match tagInfo with
            | TagInfo.Combined(tagSource, props, propsAndChildren, range) ->
                TagInfo.Combined(tagSource, body :: props, propsAndChildren, range)
            | TagInfo.Constructor(tagSource, props, range) ->
                TagInfo.Constructor(tagSource, body :: props, range)
            | TagInfo.WithBuilder(tagSource, propsAndChildren, range) ->
                TagInfo.Combined(tagSource, [ body ], propsAndChildren, range)
            |> Some
        // WITH PROPS & EXTENSION - TODO
        | Call(Import({ Kind = MemberImport(MemberRef({ FullName = Helpers.EndsWith "HtmlElementExtensions" }, _)) }, _, _) as callee, ({
                Args = TagConstructor ctx tagInfo :: rest 
            } as callInfo), _, _) ->
            match tagInfo with
            | TagInfo.Combined(tagSource, props, propsAndChildren, range) ->
                TagInfo.Combined(tagSource, props, { propsAndChildren with Args = rest @ propsAndChildren.Args }, range)
            | TagInfo.Constructor(tagSource, props, range) ->
                TagInfo.Combined(tagSource, props, { callInfo with Args = rest }, range)
            | TagInfo.WithBuilder(tagSource, propsAndChildren, range) ->
                TagInfo.WithBuilder(tagSource, { propsAndChildren with Args = rest @ propsAndChildren.Args }, range)
            |> Some
            
        | _ -> None
    let renderElement (ctx: PluginContext) (builder: ElementBuilder) =
        
        let renderProp ((name: string, expr: Expr): PropInfo) = Value(NewTuple([ Value(StringConstant name, None); TypeCast(expr, Type.Any) ], false), None)
        let renderPropList = List.map renderProp
        let renderTagName = function
            | TagSource.LibraryImport imp -> imp
            | TagSource.AutoImport name -> Value(StringConstant name, None)
        let internalCollection = (builder.Children @ (builder.Properties |> renderPropList))|> Baked.flattenExpressions
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
    let collectCallInfo (ctx: PluginContext): CallInfo -> Expr list = function
        | CallInfo.ExtensionHandler ctx propInfo ->
            [ Baked.renderAttribute propInfo ]
        | _ -> []
            
    let collectTagInfo (ctx: PluginContext): TagInfo -> ElementBuilder = function
        | TagInfo.WithBuilder(tagSource, propsAndChildren, range) ->
            { TagSource = tagSource; Children = []; Properties = [] }
        | TagInfo.Combined(tagSource, props, propsAndChildren, range) ->
            // Console.WriteLine propsAndChildren
            { TagSource = tagSource; Children = collectCallInfo ctx propsAndChildren; Properties = collectProperties ctx props }
        | TagInfo.Constructor(tagSource, props, range) ->
            { TagSource = tagSource; Children = []; Properties = collectProperties ctx props }
    let collectProperties (ctx: PluginContext): Expr list -> PropInfo list = function
        | [] -> []
        | AttributeExpression ctx propInfo :: tail -> propInfo :: collectProperties ctx tail
        | Sequential exprs :: tail -> collectProperties ctx tail @ collectProperties ctx exprs
        | expr :: tail ->
            expr |> PluginContext.debugDisposal ctx "PropertyCollection"
            collectProperties ctx tail
    
    let transform (ctx: PluginContext) (expr: Expr)=
        match expr with
        // TODO
        | TagConstructor ctx tagInfo ->
            // Console.ForegroundColor <- ConsoleColor.Yellow
            // Console.WriteLine(callee)
            // Console.ResetColor()
            // callInfo.Args
            // |> List.map (transform ctx)
            // |> Baked.flattenExpressions
            // Value(ValueKind.Null(Type.Any), None)
            tagInfo |> collectTagInfo ctx |> renderElement ctx
        // TODO
        | Sequential expressions ->
            expressions
            |> List.map (transform ctx)
            |> Sequential
        // IMPORTANT - Must come before the PropertySetter recognizer
        | PropertyGetter ctx prop ->
            PluginContext.addGetter ctx prop
            Baked.propGetter prop
        // DONE - Property setters are lifted by the ctx
        | PropertySetter ctx (prop, expr) ->
            // Return nothing, the property should be absorbed by the ctx
            PluginContext.addSetter ctx (prop, transform ctx expr)
            Expr.Value(ValueKind.Null(Type.Any), None)
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
        // We SHOULD NOT be capturing attribute expressions at this level
        | AttributeExpression ctx propInfo ->
            PluginContext.logWarning ctx $"Attribute expression was parsed at the top level and was reduced out:\n{propInfo}"
            snd propInfo
        | _ as expr -> expr

            
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
        let ctx = PluginContext.create pluginHelper TransformationKind.MemberDecl
        match memberDecl with
        | SchemaRules.ValidMemberRef ctx finalName ->
            let newExpr =
                memberDecl.Body
                |> AST.transform ctx
                |> Baked.convertGettersToObject (PluginContext.getGetters ctx)
                |> Baked.convertSettersToObject (PluginContext.getSetters ctx)
            { memberDecl with Body = newExpr }
            
            
        | _ ->
            memberDecl
    override this.TransformCall(pluginHelper, memb, expr) =
        let ctx = PluginContext.create pluginHelper TransformationKind.MemberCall
        expr
            
type PrintExpressionAttribute() =
    inherit MemberDeclarationPluginAttribute()
    override _.FableMinimumVersion = FableRequirements.version
    override this.Transform(pluginHelper, file, memberDecl) =
        Console.BackgroundColor <- ConsoleColor.Gray
        Console.ForegroundColor <- ConsoleColor.Black
        Console.WriteLine memberDecl.MemberRef
        Console.ResetColor()
        memberDecl
    override this.TransformCall(_,_,expr) =
        expr