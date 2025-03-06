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
                    Some(extensionName |> Utils.trimReservedIdentifiers, transform ctx identExpr)
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
                    when fable4 ->
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
                    | Utils.StartsWithTrimmed "aria" propName ->
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
            ((Type.PartasName ctx (Utils.EndsWithTrimmed "_$ctor" typeName)) | (Type.PartasName ctx typeName)),
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
             & Import({ Selector = ( Utils.StartsWith "BindingsModule_Route"
                                   | Utils.StartsWith "BindingsModule_HashRoute") }, t, r)),
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
        | Call(Import({ Kind = MemberImport(MemberRef({ FullName = Utils.EndsWith "HtmlElementExtensions" }, _)) }, _, _) as callee, ({
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
            },
            typ,
            range) ->
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
                expr :: restBuilds // Non builder identifier
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
            | Value(UnitConstant, None) -> restBuilds // You have been judged unworthy
            // Trust the F# compiler to not allow invalid elements within the builder.
            | _ ->
                expr :: restBuilds
                
    /// Collates TagInfos into ElementBuilders which can then be rendered via `renderElement`
    let collectTagInfo (ctx: PluginContext): TagInfo -> ElementBuilder = function
        | TagInfo.WithBuilder(tagSource, BuilderCollector ctx propsAndChildren, range) ->
            { TagSource = tagSource; Children = propsAndChildren; Properties = [] }
        | TagInfo.Combined(tagSource, props, BuilderCollector ctx propsAndChildren, range) ->
            { TagSource = tagSource; Children = propsAndChildren; Properties = props }
        | TagInfo.Constructor(tagSource, props, range) ->
            { TagSource = tagSource; Children = []; Properties = props }
            
    // Have an expression? Don't know what to do with it? Let me gobble gobble sir.
    let transform (ctx: PluginContext) (expr: Expr)=
        match expr with
        | TagConstructor ctx tagInfo ->
            tagInfo |> collectTagInfo ctx |> Baked.renderElement ctx
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
        | Lambda(ident, expr, name) ->
            Lambda(ident, transform ctx expr, name)
        | Let(ident, value, body) ->
            Let(ident, transform ctx value, transform ctx body)
        | Call(callee, callInfo, typ, range) ->
            Call(callee, { callInfo with Args = callInfo.Args |> List.map (transform ctx) }, typ, range)
            
        | _ as expr -> expr

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
