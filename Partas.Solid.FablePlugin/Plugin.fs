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
open Partas.Solid.Baked

[<assembly: ScanForPlugins>]
do ()

module internal rec AST =
    [<AutoOpen>]
    module AttributesAndProperties =
        let (|ValueUnrollerFeedback|) (expr: Expr): Expr list = [ expr ] |> function
            | ValueUnroller exprs -> exprs
        /// There is a tendency for `toArray` and `delay` to generate in property value scenarios.
        /// As I do not know their purpose outside of this use case, I am hesitant to pervasively
        /// flatten these expressions out. Instead, I choose to only perform this action exclusively
        /// for property values. This is because the output otherwise does not seem to work well with
        /// SolidJs
        /// Unlike the builder collectors, the procedure of unrolling these type of list expressions
        /// does not further transformation the expressions. This is left to the caller.
        let private (|ValueUnroller|): Expr list -> Expr list = function
            | [] -> []
            | Sequential(ValueUnroller exprs) :: ValueUnroller rest ->
                exprs @ rest
            | expr :: ValueUnroller rest ->
                match expr with
                | Call(Import({ Selector = (Utils.StartsWith "toArray" | Utils.StartsWith "toList") }, Any, None), { Args = ValueUnroller exprs }, typ, range) ->
                    Value(NewArray(ArrayValues exprs, Any, ArrayKind.MutableArray), range) :: rest
                | Call(Import({ Selector = Utils.StartsWith "delay" }, Any, None), { Args = ValueUnroller exprs }, typ, range) ->
                    exprs @ rest
                | Lambda({ Name = Utils.StartsWith "unitVar"; IsCompilerGenerated = true }, ValueUnrollerFeedback exprs, range) ->
                    exprs @ rest
                | Call(Import({ Selector = Utils.StartsWith "append" }, Any, None), { Args = ValueUnroller exprs }, typ, range) ->
                    exprs @ rest
                | Call(Import({Selector = Utils.StartsWith "singleton"}, Any, None), { Args = value :: ValueUnroller exprs }, typ, range) ->
                    exprs @ (value :: rest)
                | Call(callee, ({ Args = ValueUnroller exprs } as callInfo), typ, range) ->
                    Call(callee, { callInfo with Args = exprs }, typ, range) :: rest
                | TypeCast(ValueUnrollerFeedback exprs, typ) ->
                    exprs @ rest
                | IfThenElse(ValueUnrollerFeedback guardExprs, ValueUnrollerFeedback thenExprs, ValueUnrollerFeedback elseExprs, range) ->
                    IfThenElse(Sequential guardExprs, Sequential thenExprs, Sequential elseExprs, range) :: rest
                | expr -> expr :: rest
            
        
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
            | Get(
                IdentExpr(Ident.IdentIs ctx IdentType.Props),
                FieldGet({ Name = prop }), _, _
                )
            | Call(
                _,
                {
                    ThisArg = Some(IdentExpr(Ident.IdentIs ctx IdentType.Props))
                    Args = [ _ ]
                    MemberRef = MemberRef.Option.PartasName ctx prop
                } & { MemberRef =Some(MemberRef.MemberRefIs ctx MemberRefType.Getter) },
                _,
                _) ->
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
                Expr.Value(UnitConstant, None)
                |> Some
            | _ -> None
        // Determines at top level whether the current expression could be considered
        // as a attribute expression by any of our definitions
        let (|AttributeExpression|_|) (ctx: PluginContext) = function
            | Call(
                callee,
                {
                    ThisArg = Some(IdentExpr(Ident.IdentIs ctx IdentType.ReturnVal))
                    Args = (
                        TagValue.TagValue ctx expr // Before unrolling, check if it's a TagValue ident expression
                      | ValueUnrollerFeedback [ expr ]
                      |  expr
                      ) :: _
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
            // member val set/get
            | Set(IdentExpr(Ident.IdentIs ctx IdentType.ReturnVal), FieldSet(prop), _, expr, _) ->
                (prop, transform ctx expr)
                |> Some
            // Captured method/Extension call
            | Call(
                Value(ValueKind.UnitConstant, None),
                {
                    MemberRef = Some(MemberRef(_, { CompiledName = extensionName }))
                    Args = exprs
                },
                Type.MetaType,
                None
                ) ->
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
                    Args = (ValueUnrollerFeedback [ value ] | value ) :: _
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
                MemberRef  = Some(MemberRef(_, { CompiledName = ".ctor" }))
            },
            (Type.PartasName ctx typeName),
            range) ->
            TagInfo.Constructor (TagSource.LibraryImport imp, props, range)
            |> Some
        // Non LibraryImports that require LibraryImport injection
        | Call(
            (Expr.ImportedConstructor ctx
             & Import({ Selector = ( Utils.StartsWith "BindingsModule_Route"
                                   | Utils.StartsWith "BindingsModule_HashRoute"
                                   | Utils.StartsWith "BindingsModule_Match"
                                   | Utils.StartsWith "BindingsModule_For"
                                   | Utils.StartsWith "BindingsModule_Index"
                                   | Utils.StartsWith "BindingsModule_Show"
                                   | Utils.StartsWith "BindingsModule_Switch") }, t, r)),
            {
                Args = PropCollector ctx props
            },
            (Type.PartasName ctx typeName),
            range) ->
            let importExpr =
                Import(
                    { Selector = typeName
                      Path =
                          match typeName with
                          | Utils.StartsWith "Route" | Utils.StartsWith "HashRoute" ->
                              "@solidjs/router"
                          | _ -> "solid-js"
                      Kind = UserImport false },
                    t, r)
            TagInfo.Constructor (TagSource.LibraryImport importExpr, props, range)
            |> Some
        // Non LibraryImports; ie User defined imports
        | Call(
            (Expr.ImportedConstructor ctx & Import(identee, t, r)),
            {
                Args = PropCollector ctx props
                MemberRef = Some(MemberRef(_, { CompiledName = ".ctor" }))
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
        // An imported context
        | CurriedApply(
            Import(
                { Kind = MemberImport(MemberRef(_,{ CompiledName = typeName })) } as imp,
                (Type.PartasName ctx "ContextProvider" as typ),
                irange),
            props,
            _,
            range
            ) ->
            TagInfo.Constructor(
                TagSource.LibraryImport(Import({ imp with Selector = typeName + ".Provider" }, typ, irange)),
                [("value", Sequential(props))],
                irange
                )
            |> Some
        // Local defined context
        | CurriedApply(
                IdentExpr({ Name = typeName; Type = Type.PartasName ctx "ContextProvider" }),
                props,
                _,
                range
            ) ->
            TagInfo.Constructor(
                TagSource.AutoImport $"{typeName}.Provider",
                [("value", Sequential(props))],
                range
                )
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
            // This was somehow related to fixes for Oxpecker.Solid.Tests
            | CurriedApply(BuilderCollectorFeedback ctx applied, BuilderCollector ctx args, typ, range) ->
                if args.IsEmpty then applied @ restBuilds
                else CurriedApply(applied.Head, args, typ, range) :: restBuilds
            // This captures and correctly generates the lambda in the For and Index bindings
            | Lambda(
                Ident.IdentIs ctx IdentType.Cont,
                TypeCast(Lambda(item, Lambda(index, BuilderCollectorFeedback ctx expr, _), _), _),
                _
                ) ->
                let getHead = List.tryHead >> Option.defaultValue (Value(UnitConstant, None))
                Delegate([ item; index ], getHead expr, None, []) :: restBuilds
            // Builder op
            | Lambda(
                Ident.IdentIs ctx (IdentType.Builder | IdentType.Yield | IdentType.Cont),
                BuilderCollectorFeedback ctx expr,
                _
                ) -> expr @ restBuilds
            // In the case of JSX (not tsx) the type cast is irrelevant for the string
            | TypeCast(Value(StringConstant _, _) as text, Unit) ->
                 text :: restBuilds
            // Ensure typecasts are transformed
            | TypeCast(BuilderCollectorFeedback ctx expr, typ) ->
                expr @ restBuilds
            // Ensure conditionals are transformed
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
            // lift any getters/setters
            | PropsGetterOrSetter ctx (BuilderCollectorFeedback ctx exprs) ->
                exprs @ restBuilds
            // | PropsGetterOrSetter ctx expr ->
                // match expr with
                // | Value(UnitConstant, None) | Value(Null(Any), None) -> restBuilds
                // | _ -> expr :: restBuilds
            // We can transform the contents of the Get, but since it is a field access, we don't want
            // to dispose of the accessor.
            | Get(BuilderCollectorFeedback ctx [ expr ], (GetKind.FieldGet _ as getKind), typ ,range) ->
                Get(expr, getKind, typ, range) :: restBuilds
            // We can transform the contents of the Get, but since it is a tuple accessor, we can't
            // unwrap it. This would cause accessors to be disposed of.
            | Get((BuilderCollectorFeedback ctx [ expr ] | expr), (TupleIndex _ as getKind), typ, range) ->
                Get(expr, getKind, typ, range) :: restBuilds
            // This is a prop getter in a builder, don't transform inside as it's already been done.
            | Get(IdentExpr({ IsThisArgument = true ; IsCompilerGenerated = true }), _, _, _) ->
                expr :: restBuilds
            | Get(BuilderCollectorFeedback ctx exprs, _, _, _) ->
                let head = exprs |> (List.tryHead >> Option.defaultValue (Value(UnitConstant, None)))
                head :: restBuilds
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
            | IdentExpr(_)
            | Value(UnitConstant, None) -> restBuilds // You have been judged unworthy
            // Trust the F# compiler to not allow invalid elements within the builder.
            | _ ->
                expr :: restBuilds
                
    /// <summary>
    /// Collates TagInfos into ElementBuilders which can then be rendered via `renderElement`<br/>
    /// Performs final clean up of properties by trimming reserved identifiers of the attribute keys
    /// </summary>
    let collectTagInfo (ctx: PluginContext): TagInfo -> ElementBuilder = function
        | TagInfo.WithBuilder(tagSource, BuilderCollector ctx propsAndChildren, range) ->
            {
                TagSource = tagSource
                Children = propsAndChildren
                Properties = []
            }
        | TagInfo.Combined(tagSource, props, BuilderCollector ctx propsAndChildren, range) ->
            {
                TagSource = tagSource
                Children = propsAndChildren
                Properties = props |> List.map (fun (prop,expr) -> (prop |> Utils.trimReservedIdentifiers, expr) )
            }
        | TagInfo.Constructor(tagSource, props, range) ->
            {
                TagSource = tagSource
                Children = []
                Properties = props |> List.map (fun (prop,expr) -> (prop |> Utils.trimReservedIdentifiers, expr) )
            }
            
    /// <summary>
    /// This is the heart of the transformation sequence.<br/>
    /// This must handle the transformation of every expression. All expressions parsed
    /// into the transformer are at some point or another, passed through this
    /// transformer.<br/>
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
        // To enable sugar like `&&=` and `??=`, we have to ensure the arguments
        // are transformed.
        | Emit({ CallInfo = { Args = exprs } as callInfo } as emitInfo, typ, range) ->
            let transformedExprs = exprs |> List.map (transform ctx)
            Emit({ emitInfo with CallInfo = { callInfo with Args = transformedExprs } }, typ, range)
        | TagValue.TagValue ctx expr ->
            expr
        // Enables plugin magic for TagValues
        | TagValue.TagRender ctx expr ->
            expr
        // Matches against any expression pattern that needs to be converted into a JSX tag
        | TagConstructor ctx tagInfo ->
            tagInfo |> collectTagInfo ctx |> Baked.renderElement ctx
        // transform each expr in the sequence
        | Sequential exprs ->
            exprs
            |> List.map (transform ctx)
            |> Sequential
        // any captured Getters or Setters of our self identifier are treated specially
        | PropsGetterOrSetter ctx expr ->
            expr
        // transform conditionals
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
        // While the more pervasive TypeCast transformation is good for 90% of cases,
        // it can leave artifacts from using Setters in builders. We match this single case and dispose
        // before it is passed back to the BuilderCollector
        | Lambda(
                { Name = name; IsCompilerGenerated = true },
                TypeCast(IdentExpr({ Name = otherName; IsCompilerGenerated = true }), Unit),
                None)
            when name = otherName -> Value(UnitConstant, None)
        | TypeCast(expr, _) -> transform ctx expr
        // Attribute expression pairs should have been captured by
        // TagConstructor -> PropCollector. It would be invalid to have them
        // matched here
        | AttributeExpression ctx propInfo ->
            PluginContext.logWarning ctx $"Attribute expression was parsed at the top level and was reduced out:\n{propInfo}"
            snd propInfo
        // Nested expression structures have their descendants transformed before
        // being restored
        | CurriedApply(applied, exprs, typ, range) ->
            CurriedApply(transform ctx applied, exprs |> List.map (transform ctx), typ, range )
        | Delegate(args, body, name, tags) ->
            Delegate(args, transform ctx body, name, tags)
        | Lambda(ident, expr, name) ->
            Lambda(ident, transform ctx expr, name)
        | Let(ident, value, body) -> // transform other lets
            Let(ident, transform ctx value, transform ctx body)
        | Call(callee, callInfo, typ, range) -> // transform calls
            Call(callee, { callInfo with Args = callInfo.Args |> List.map (transform ctx) }, typ, range)
        | Value( // transform inside anon records
                NewAnonymousRecord(values, fieldNames, types, isStruct),
                range
            ) ->
            Value(NewAnonymousRecord(values |> List.map (transform ctx), fieldNames, types, isStruct), range)
        | Value( // transform inside arrays
                NewArray(ArrayValues exprs, typ, kind),
                range
            ) ->
            Value(NewArray(ArrayValues (exprs |> List.map (transform ctx)), typ, kind), range)
        | Value( // transform inside lists
                NewList(Some(expr1, expr2), typ),
                range
            ) ->
            Value(NewList(Some(transform ctx expr1, transform ctx expr2), typ), range)
        | Value( // transform inside records
                NewRecord(exprs, entityRef, genArgs),
                range
            ) ->
            Value(NewRecord(exprs |> List.map (transform ctx), entityRef, genArgs), range)
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
        | _ as expr -> expr

    /// Plugin support for the TagValue magics
    module TagValue =
        module EntityRef =
            let (|TagValue|_|) = function
                | { FullName = "Partas.Solid.Builder.TagValue`1" } -> Some()
                | _ -> None
        module ImportInfo =
            let (|TagValue|_|) = function
                | { Kind = MemberImport(MemberRef(EntityRef.TagValue, _)) } -> Some()
                | _ -> None
            let (|Render|_|) = function
                | { Kind = MemberImport(MemberRef(EntityRef.TagValue, { CompiledName = "render" })) }
                    -> Some()
                | _ -> None
        
        /// Will retrieve the wrapped type entity ref if it is a valid TagValue type
        let (|GetType|_|) = function
            | DeclaredType(EntityRef.TagValue, [ DeclaredType(entRef,_) ]) -> Some entRef
            | LambdaType(GetType entRef, _ ) -> Some entRef
            | _ -> None
        let (|CollectProperties|_|) (ctx: PluginContext) = function
            | [ TypeCast(Value(NewAnonymousRecord(values, fields, _, _), _), _) ] ->
                values
                |> List.zip (fields |> Array.toList)
                |> List.map (fun (s,e) ->
                    e
                    |> transform ctx
                    |> fun e ->
                        s |> Utils.trimReservedIdentifiers,
                        e)
                |> Some
            | _ -> None
        let (|TagValue|_|) (ctx: PluginContext): Expr -> Expr option = function
            | Call(
                Import({ Selector = "op_BangAt"; Kind = MemberImport(MemberRef({ FullName = "Partas.Solid.Builder" }, _)) }, _, _),
                {
                    Args = Lambda(_, Call(callee, _, _, _), _) :: _
                },
                _,
                range
                ) ->
                match callee with
                // Some ident
                | IdentExpr({ Type = Type.PartasName ctx typeName } as identee) ->
                    IdentExpr({ identee with Name = typeName |> Utils.trimReservedIdentifiers }) |> Some
                // prevent import of native tags
                | Expr.NativeImportedConstructor ctx
                    & Import(
                        importInfo,
                        typ & Type.PartasName ctx typeName,
                        range
                    ) ->
                    IdentExpr({ Name = typeName; Type = typ; IsMutable = false ; IsThisArgument = false ; IsCompilerGenerated = true; Range = range })
                    |> Some
                // an imported tag
                | Import(
                    importInfo,
                    typ & Type.PartasName ctx typeName,
                    range) ->
                    Import({ importInfo with Selector = typeName }, typ, range)
                    |> Some
                | Import({ Kind = UserImport false }, _, _) ->
                    Some callee
                | _ -> None
            | TypeCast(TagValue ctx expr, _) -> expr |> Some
            | _ -> None
        let (|TagRender|_|) (ctx: PluginContext): Expr -> Expr option = function
            // A call to render a tagvalue with a constructor
            | Call(
                Import(ImportInfo.Render, GetType eRef, _),
                {
                    ThisArg = thisArg // contains info on the left expr of render
                    Args = (
                        TagConstructor ctx tagInfo :: BuilderCollector ctx rest
                         | TypeCast(TagConstructor ctx tagInfo, _) :: BuilderCollector ctx rest
                    )
                }, _, _ ) ->
                tagInfo
                |> function
                    | TagInfo.WithBuilder(tagName, propsAndChildren, range) ->
                        TagInfo.WithBuilder(tagName, rest @ propsAndChildren, range)
                    | TagInfo.Combined(tagName, props, propsAndChildren, range) ->
                        TagInfo.Combined(tagName, props, rest @ propsAndChildren, range)
                    | TagInfo.Constructor(tagName, props, range) ->
                        TagInfo.Combined(tagName, props, rest, range)
                |> collectTagInfo ctx
                |> fun eleBuilder ->
                    match thisArg with
                    | Some expr // Make sure we capture getters
                    | Some(PropsGetterOrSetter ctx expr) ->
                        { eleBuilder with TagSource = TagSource.LibraryImport expr }
                    | _ -> eleBuilder
                |> renderElement ctx |> Some
            // a call to render a tagvalue with an anon record
            // todo - support key,value pair list
            | Call(
                Import(ImportInfo.Render, GetType eRef, _),
                {
                    ThisArg = thisArg
                    Args = CollectProperties ctx props
                },
                _,
                range
                ) ->
                let importExpr =
                    match thisArg with
                    | Some(PropsGetterOrSetter ctx expr) ->
                        expr
                    | Some(IdentExpr(_) as ident) ->
                        ident
                    | _ ->
                        Import(
                            {
                                Selector =
                                    eRef.FullName |> Utils.trimReservedIdentifiers
                                Path = eRef.SourcePath |> Option.defaultValue ""
                                Kind = UserImport false
                            },
                            Any,
                            range
                        )
                { TagSource = TagSource.LibraryImport importExpr
                  Properties = props
                  Children = [] }
                |> renderElement ctx
                |> Some
            | _ -> None
            
            
                


type SolidTypeComponentAttribute() =
    inherit MemberDeclarationPluginAttribute()
    override _.FableMinimumVersion = FableRequirements.version
    override this.Transform(pluginHelper, file, memberDecl) =
        // Console.WriteLine "\nSTART MEMBER DECL!!!"
        // Console.WriteLine memberDecl.Body
        // Console.WriteLine "END MEMBER DECL!!!\n"
        let ctx = PluginContext.create pluginHelper TransformationKind.TypeMemberDecl
        
        match memberDecl with
        // Check that the memberDecl has the correct self identifier of `props`
        | SchemaRules.ValidMemberRef ctx finalName ->
            let newExpr =
                memberDecl.Body
                |> AST.transform ctx // initiate transformation
                // Create and append the splitProps expression
                |> Baked.convertGettersToObject (PluginContext.getGetters ctx |> List.distinct)
                // Create and append the mergeProps expression if we have any setters
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
        // Console.WriteLine memberDecl
        {
            memberDecl with
                Body = memberDecl.Body |> AST.transform ctx
        }
    override this.TransformCall(pluginHelper, memb, expr) =
        expr