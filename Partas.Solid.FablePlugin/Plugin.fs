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
        let (|ValueUnrollerFeedback|) (ctx: PluginContext) (expr: Expr): Expr list =
            if ctx |> PluginContext.hasFlag ComponentFlag.SkipCEOptimisation
            then [ expr ]
            else [ expr ] |> function
                | ValueUnroller ctx exprs -> exprs
        /// There is a tendency for `toArray` and `delay` to generate in property value scenarios.
        /// As I do not know their purpose outside of this use case, I am hesitant to pervasively
        /// flatten these expressions out. Instead, I choose to only perform this action exclusively
        /// for property values. This is because the output otherwise does not seem to work well with
        /// SolidJs
        /// Unlike the builder collectors, the procedure of unrolling these type of list expressions
        /// does not further transformation the expressions. This is left to the caller.
        let private (|ValueUnroller|) (ctx: PluginContext): Expr list -> Expr list = function
            | [] -> []
            | Sequential(ValueUnroller ctx exprs) :: ValueUnroller ctx rest ->
                exprs @ rest
            | expr :: ValueUnroller ctx rest ->
                match expr with
                | Call(Import({ Selector = (Utils.StartsWith "toArray" | Utils.StartsWith "toList") }, Any, None), { Args = ValueUnroller ctx exprs }, typ, range) ->
                    Value(NewArray(ArrayValues exprs, Any, ArrayKind.MutableArray), range) :: rest
                | Call(Import({ Selector = Utils.StartsWith "delay" }, Any, None), { Args = ValueUnroller ctx exprs }, typ, range) ->
                    exprs @ rest
                | Lambda({ Name = Utils.StartsWith "unitVar"; IsCompilerGenerated = true }, ValueUnrollerFeedback ctx exprs, range) ->
                    exprs @ rest
                | Call(Import({ Selector = Utils.StartsWith "append" }, Any, None), { Args = ValueUnroller ctx exprs }, typ, range) ->
                    exprs @ rest
                | Call(Import({Selector = Utils.StartsWith "singleton"}, Any, None), { Args = value :: ValueUnroller ctx exprs }, typ, range) ->
                    exprs @ (value :: rest)
                | Call(Import({ Selector = Utils.StartsWith "empty"; Path = Utils.EndsWith "Seq.js" }, Any, None), { Args = []; GenericArgs = typ :: _ }, _, _) ->
                    Value(ValueKind.Null(typ), None) :: rest
                | Call(callee, ({ Args = ValueUnroller ctx exprs } as callInfo), typ, range) ->
                    Call(callee, { callInfo with Args = exprs }, typ, range) :: rest
                | TypeCast(ValueUnrollerFeedback ctx exprs, typ) ->
                    exprs @ rest
                | IfThenElse(ValueUnrollerFeedback ctx guardExprs, ValueUnrollerFeedback ctx thenExprs, ValueUnrollerFeedback ctx elseExprs, range) ->
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
                | { ThisArg = Some(IdentExpr(Ident.IdentIs ctx IdentType.Props))
                    MemberRef = MemberRef.Option.PartasName ctx name
                    Args = expr :: _ } ->
                    Some(name, expr)
                | _ ->
                    None
            // This is a setter to a val mutable field.
            | Set(
                (
                    // Defined locally
                    IdentExpr(Ident.IdentIs ctx IdentType.Props)
                    // inherited
                  | TypeCast(IdentExpr(Ident.IdentIs ctx IdentType.Props), _)
                )
                ,FieldSet(name)
                ,_
                ,expr
                ,_) ->
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
        let private (|PropertyGetter|_|) (ctx: PluginContext) = function
            | Get(
                (
                    // Defined locally
                    IdentExpr(Ident.IdentIs ctx IdentType.Props)
                    // Inherited
                  | TypeCast(IdentExpr(Ident.IdentIs ctx IdentType.Props), _)
                ),
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
                      | ValueUnrollerFeedback ctx [ expr ]
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
            // Inlined named overloads to `[<DefaultValue>] val mutable` properties/attributes
            | Let(
                    { IsThisArgument = true; IsCompilerGenerated = true },
                    IdentExpr(Ident.IdentIs ctx IdentType.ReturnVal),
                    Set(
                        IdentExpr({  IsThisArgument = true (*; IsCompilerGenerated = false *) }),
                        FieldSet(prop),
                        _, expr, _
                    )
                ) ->
                (prop, transform ctx expr) |> Some
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
                    "on" | "bool" | "data" | "attr" | "use"
                    ), [ Value(StringConstant prop, _); value ] ->
                    let propName =
                        match extensionName with
                        | "bool" | "on" | "use" -> $"{extensionName}:{prop}"
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
                | "spread", [ (TypeCast(
                        Get(
                            IdentExpr({ Name = ident }),
                            TupleIndex indx,
                            _,
                            _
                            )
                        , _
                        )
                    ) | (
                        Get(
                            IdentExpr({ Name = ident }),
                            TupleIndex indx,
                            _,
                            _
                        )
                    ) ] ->
                    Some("{..." + ident + "[" + string indx + "]} bool:n$", Value(ValueKind.BoolConstant(false), None))
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
                        when callee = importJsxCreate && typ = jsxElementType ->
                        let c: Ident = { Name = "PARTAS_POLYPROPS"
                                         Type = typ
                                         IsMutable = false
                                         IsThisArgument = false
                                         IsCompilerGenerated = true
                                         Range = range
                                          }
                        Lambda(c,
                            Call(
                            callee,
                            { callInfo with Args = [
                                        tagName;
                                        Value(
                                            NewList(Some(
                                                Value(NewTuple([ Value(StringConstant "{...PARTAS_POLYPROPS} on:n$", None); TypeCast(Value(ValueKind.BoolConstant false, None), Type.Any) ], false), None)
                                                , internalCollection
                                            ), Any),
                                            None
                                            )
                            ]  },
                            typ, range), None)
                    | expr -> expr
                    |> fun expr ->
                        Some(attrName, expr)
                    
                    
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
                    Args = ValueUnrollerFeedback ctx [ value ] :: _
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
        // If we do not position this above the local tag recogniser, then it will never be reached in cases that it is
        // a local tag using the PartasImport attribute
        // Non LibraryImports that require LibraryImport injection via the attribute PartasImportAttribute
        | Call(
            (   (* Callee *)
                (Expr.ImportedConstructor ctx & Import(_, t, r)) // Captures constructors from other modules with PartasImportAttribute
              | (IdentExpr(Ident.IdentIs ctx IdentType.Constructor) & IdentExpr({ Type = t; Range = r })) // Captures constructors defined in the same module with PartasImportAttribute
            ),
            {   (* CallInfo *)
                Args = PropCollector ctx props
            }, (* Type *)
            (Type.PartasName ctx typeName & Type.HasPartasImport ctx (selector, path)),
            range) ->
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
            (   (* Callee *)
                (Expr.ImportedConstructor ctx & Import(_, t, r)) // Captures constructors from other modules with PartasImportAttribute
              | (IdentExpr(Ident.IdentIs ctx IdentType.Constructor) & IdentExpr({ Type = t; Range = r })) // Captures constructors defined in the same module with PartasImportAttribute
            ),
            {   (* CallInfo *)
                Args = PropCollector ctx props
            }, (* Type *)
            (Type.PartasName ctx typeName & Type.HasPartasProxyImport ctx (key, selector, path)),
            range) ->
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
            (IdentExpr(Ident.IdentIs ctx IdentType.Constructor)),
            (CallInfo.Constructor ctx & {
                Args = PropCollector ctx props
            }),
            ((Type.PartasName ctx (Utils.EndsWithTrimmed "_$ctor" typeName)) | (Type.PartasName ctx typeName)),
            range) ->
            ElementBuilder.create (TagSource.AutoImport typeName) props range
            |> Some
        // Native import
        | Call(
            (Expr.NativeImportedConstructor ctx),
            {
                Args = PropCollector ctx props
            },
            (Type.PartasName ctx typeName),
            range) ->
            ElementBuilder.create (TagSource.AutoImport typeName) props range
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
            ElementBuilder.create (TagSource.LibraryImport imp) props range
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
        | Call(Import({ Kind = MemberImport(MemberRef({ FullName = Utils.EndsWith "HtmlElementExtensions" | Utils.EndsWith "PolymorphicExtensions" }, _)) }, _, _) as callee, ({
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
            Expr.ImportedContainerExtensionName ctx "Run", // run is being called. builder being applied to obj. obj is arg 1.
            {   // Check the first argument, which is the argument this is being called against, for the constructor which called it.
                Args = TagConstructor ctx tagInfo :: BuilderCollector ctx rest // The rest are the computations being applied
            },
            typ,
            range) ->
            { tagInfo with Children = rest @ tagInfo.Children }
            |> Some
        
        | Let(Ident.IdentIs ctx IdentType.Element, TagConstructor ctx tagInfo, BuilderCollectorFeedback ctx body) ->
            { tagInfo with Children = body @ tagInfo.Children }
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
            {
                TagSource = TagSource.LibraryImport(Import({ imp with Selector = typeName + ".Provider" }, typ, irange))
                Properties = [("value", Sequential(props))]
                Children = []
                Range = irange
            }
            |> Some
        // Local defined context
        | CurriedApply(
                IdentExpr({ Name = typeName; Type = Type.PartasName ctx "ContextProvider" }),
                props,
                _,
                range
            ) ->
            {
                TagSource = TagSource.AutoImport $"{typeName}.Provider"
                Properties = [("value", Sequential(props))]
                Children = []
                Range = range
            }
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
            // Note: this must come before recognizer for lambda with 3 and lambda with 2 parameters
            // Captures and correctly generates lambda in ChildLambdaProvider4
            | Lambda(
                Ident.IdentIs ctx IdentType.Cont,
                TypeCast(Lambda(p1, Lambda(p2, Lambda(p3, Lambda(p4, BuilderCollectorFeedback ctx expr, _), _), _), _), _),
                _
                ) ->
                let getHead = List.tryHead >> Option.defaultValue (Value(UnitConstant, None))
                Delegate([ p1; p2; p3; p4], getHead expr, None, []) :: restBuilds
            // Captures and correctly generates lambda in ChildLambdaProvider3
            | Lambda(
                Ident.IdentIs ctx IdentType.Cont,
                TypeCast(Lambda(p1, Lambda(p2, Lambda(p3, BuilderCollectorFeedback ctx expr, _), _), _), _),
                _
                ) ->
                let getHead = List.tryHead >> Option.defaultValue (Value(UnitConstant, None))
                Delegate([ p1; p2; p3], getHead expr, None, []) :: restBuilds
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
            // hand this off to our main transform loop
            | Get(BuilderCollectorFeedback ctx [ expr ], ExprGet(BuilderCollectorFeedback ctx [ otherExprs ]), _, range) ->
                Get(expr, ExprGet otherExprs, Any, range) :: restBuilds
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
    
    // TODO - correct documentation and refactor since TagInfo is refactored out            
    /// <summary>
    /// Collates TagInfos into ElementBuilders which can then be rendered via `renderElement`<br/>
    /// Performs final clean up of properties by trimming reserved identifiers of the attribute keys
    /// </summary>
    let collectTagInfo (ctx: PluginContext): ElementBuilder -> ElementBuilder = function
        | { Properties = props } as eleBuilder ->
            {
                eleBuilder with
                    Properties =
                        props
                        |> List.map (fun (prop,expr) -> (prop |> Utils.trimReservedIdentifiers, expr) )
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
        // Expression match and transform any `props` accesses or assignments
        | PropsGetterOrSetter ctx expr ->
            expr
        // Transform calls that are getters
        | Call(
            Expr.ImportedGetter ctx,
            {
                ThisArg = Some(ident)
                Args = args
                MemberRef = MemberRef.Option.PartasName ctx prop
            } & { MemberRef =Some(MemberRef.MemberRefIs ctx MemberRefType.Getter) },
            _,
            _) ->
            Get(
                expr = ident,
                kind = GetKind.FieldGet(
                        {
                            Name = prop
                            FieldType = None
                            IsMutable = false
                            MaybeCalculated = false
                            Tags = []
                        }
                    ),
                typ = Any,
                range = None
            )
        
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
                decisionTree,
                targets
                |> List.map (fun (target, expr) -> target, transform ctx expr)
                )
        // Within a computation expression, property assignment for `props` is transformed out. In cases where a null
        // lambda 'husk' remains, we reduce to a null expression.
        | Lambda(
                { Name = name; IsCompilerGenerated = true },
                TypeCast(IdentExpr({ Name = otherName; IsCompilerGenerated = true }), Unit),
                None)
            when name = otherName -> Value(UnitConstant, None)
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
                 NewAnonymousRecord(_)
                | NewArray(ArrayValues _, _, _)
                | NewList(Some _, _)
                | NewRecord(_)
                | StringTemplate(_)
                | NewOption(Some _, _, _)
                | NewTuple(_)
                | NewUnion(_)
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
        | Get(expr, ExprGet(getExpr) & ExprGet(Call(callee, callInfo, _, _)), typ, range) ->
            match transform ctx getExpr with
            | Value(UnitConstant, None) | Value(Null(Any), None) -> transform ctx expr
            | getExpr ->
                // PluginContext.debugDisposal ctx "Transform of Get ExprGet" getExpr
                Get(
                    transform ctx expr,
                    ExprGet(getExpr),
                    typ,
                    range)
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
        // No further first pass transformations applicable
        | _ as expr -> expr
        
    /// Plugin support for extending Polymorphic attributes
    module Polymorphism =
        let (|PolymorphicAttribute|_|) (ctx: PluginContext) : string -> string option = function
            | "as'" -> Some "as"
            | "asChild" -> Some "asChild"
            | Utils.StartsWithTrimmed "__PARTAS_POLYMORPHIC__" attrName -> Some (attrName |> Utils.trimReservedIdentifiers)
            | _ -> None
    /// Plugin support for the TagValue magics
    module TagValue =
        module EntityRef =
            let (|TagValue|_|) = function
                | { FullName = "Partas.Solid.Builder.TagValue" } -> Some()
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
            | DeclaredType(EntityRef.TagValue, _) -> Some()
            | LambdaType(GetType, _ ) -> Some()
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
        
        let (|TagRender|_|) (ctx: PluginContext): Expr -> Expr option =
            let rec (|UnrollTypeCast|) = function
                | TypeCast(UnrollTypeCast expr, _) -> expr
                | expr -> expr
            function
            // A call to render a tagvalue with a constructor
            | Call(
                Import(ImportInfo.Render, _, _),
                {
                    ThisArg = thisArg // contains info on the left expr of render
                    Args = (
                        UnrollTypeCast(TagConstructor ctx tagInfo) :: BuilderCollector ctx rest
                    )
                }, _, _ ) ->
                { tagInfo with Children = rest @ tagInfo.Children }
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
                Import(ImportInfo.Render, _, _),
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
                    | Some expr -> expr
                    | _ ->
                        failwith "unreachable"
                { TagSource = TagSource.LibraryImport importExpr
                  Properties = props
                  Children = []
                  Range = range }
                |> renderElement ctx
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
                Body = memberDecl.Body |> AST.transform ctx
        }
    override this.TransformCall(_, _, expr) =
        expr
    
    new() = SolidComponentAttribute(int ComponentFlag.Default)
    new(compileOptions: ComponentFlag) = SolidComponentAttribute(int compileOptions)
