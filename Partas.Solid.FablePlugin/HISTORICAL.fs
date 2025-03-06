// Deprecated/Historical
// See Plugin.fs for current iteration.

namespace Partas.Solid

open System
open Fable
open Fable.AST
open Fable.AST.Fable

[<assembly: ScanForPlugins>]
do () // Prompts fable to utilise this plugin

/// Defines utility active patterns
module Helpers =
    /// Active recognizer alias for _.StartsWith
    let (|StartsWith|_|) (value: string) =
        function
        | (s: string) when s.StartsWith(value) -> Some s
        | _ -> None

    /// Active recognizer alias for _.EndsWith
    let (|EndsWith|_|) (value: string) =
        function
        | (s: string) when s.EndsWith(value) -> Some s
        | _ -> None

    let (|StartsWithTrimmed|_|) (value: string) =
        function
        | (s: string) when s.StartsWith(value) -> s.Substring(value.Length) |> Some
        | _ -> None

    let (|EndsWithTrimmed|_|) (value: string) =
        function
        | (s: string) when s.EndsWith(value) -> s.Substring(0, s.Length - value.Length) |> Some
        | _ -> None

    /// Trims characters from reserved identifiers
    let rec trimReservedIdentifiers =
        function
        | (s: string) when s.EndsWith("'") -> s.Substring(0, s.Length - 1) |> trimReservedIdentifiers
        | s when s.EndsWith("`1") -> s.Substring(0, s.Length - 2) |> trimReservedIdentifiers
        | s -> s

    /// Recursively drills down the value expression of a Let chain.
    let rec (|LetDrill|): Expr -> Expr =
        function
        | Let(_, LetDrill expr, _)
        | expr -> expr

    /// Recursively drills down the callee expression of a Call chain
    let rec (|CallDrill|): Expr -> Expr =
        function
        | Call(CallDrill expr, _, _, _)
        | expr -> expr

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

    /// Contains 'pre-baked' element types or converters
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
                { Selector = "splitProps"
                  Path = "solid-js"
                  Kind = UserImport false },
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
            IdentExpr(
                { Name = "others"
                  Type = jsxElementType
                  IsMutable = false
                  IsThisArgument = false
                  IsCompilerGenerated = true
                  Range = None }
            )

        let convertSettersToObject (values: (string * Expr) list) =
            Expr.Set(
                expr =
                    IdentExpr(
                        { Name = "props"
                          Type = jsxElementType
                          IsMutable = false
                          IsThisArgument = false
                          IsCompilerGenerated = true
                          Range = None }
                    ),
                kind = SetKind.ValueSet,
                typ = Type.Any,
                value =
                    Call(
                        callee = importMergeProps,
                        info =
                            CallInfo.Create(
                                args =
                                    [ Value(
                                          kind =
                                              ValueKind.NewAnonymousRecord(
                                                  values = (values |> List.map snd),
                                                  fieldNames =
                                                      (values
                                                       |> List.map (fst >> trimReservedIdentifiers)
                                                       |> List.toArray),
                                                  genArgs = [],
                                                  isStruct = false
                                              ),
                                          range = None
                                      )
                                      IdentExpr(
                                          { Name = "props"
                                            Type = jsxElementType
                                            IsMutable = false
                                            IsThisArgument = false
                                            IsCompilerGenerated = true
                                            Range = None }
                                      ) ]
                            ),
                        typ = Type.Any,
                        range = None
                    ),
                range = None
            )

        let convertGettersToObject (values: string list) (rest: Expr) =
            Expr.Let(
                ident =
                    { Name = "[local, others]"
                      Type = Type.Any
                      IsMutable = false
                      IsThisArgument = false
                      IsCompilerGenerated = false
                      Range = None },
                value =
                    Expr.Call(
                        callee = importSplitProps,
                        info =
                            CallInfo.Create(
                                args =
                                    [ IdentExpr(
                                          { Name = "props"
                                            Type = jsxElementType
                                            IsMutable = false
                                            IsThisArgument = false
                                            IsCompilerGenerated = true
                                            Range = None }
                                      )
                                      Value(
                                          kind =
                                              ValueKind.NewArray(
                                                  newKind =
                                                      NewArrayKind.ArrayValues(
                                                          [ for value in values do
                                                                Value(StringConstant(value), None) ]
                                                      ),
                                                  typ = Type.Any,
                                                  kind = ArrayKind.ImmutableArray
                                              ),
                                          range = None
                                      ) ]
                            ),
                        typ = Type.Any,
                        range = None
                    ),
                body = rest
            )

        let importJsxCreate =
            Import(
                info =
                    { Selector = "create"
                      Path = "@fable-org/fable-library-js/JSX.js"
                      Kind = ImportKind.UserImport false },
                typ = Type.Any,
                range = None
            )

        let listItemType =
            Type.Tuple(genericArgs = [ Type.String; Type.Any ], isStruct = false)

        let emptyList =
            Value(kind = NewList(headAndTail = None, typ = listItemType), range = None)

        let exprListToExpr (exprs: Expr list) =
            (emptyList, exprs)
            ||> List.fold (fun acc prop ->
                Value(kind = NewList(headAndTail = Some(prop, acc), typ = listItemType), range = None))

        /// <summary>
        /// Generates a getter expression in JSX
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="getTarget"></param>
        let propGetter (targetName: string, getTarget: string) =
            Get(
                IdentExpr
                    { Name = targetName
                      Type = jsxElementType
                      IsMutable = false
                      IsThisArgument = true
                      IsCompilerGenerated = true
                      Range = None },
                GetKind.ExprGet(Value(StringConstant(trimReservedIdentifiers getTarget), None)),
                Type.Any,
                None
            )

        /// <summary>
        /// Creates an assignment expression in JSX
        /// </summary>
        /// <example>
        /// <code>
        /// propSetter(thisIdentExpr, "class'", testStringValueExpr)
        /// // Generates JSX
        /// this.class = "test"
        /// </code>
        /// </example>
        /// <param name="identExpr">The IdentExpr</param>
        /// <param name="setTarget">An Empty string will evaluate the generated expression to not be a ident accessor</param>
        /// <param name="setValue">The expression assigned to the ident</param>
        let propSetter (identExpr: Expr, setTarget: string, setValue: Expr) =
            Set(
                identExpr
                , match setTarget with
                  | "" -> SetKind.ValueSet
                  | _ -> SetKind.ExprSet(Value(StringConstant(trimReservedIdentifiers setTarget), None))
                , Type.Any
                , setValue
                , None
            )

module internal rec AST =
    /// Defines the function passed through the transformer which
    /// accumulates property setters to the transformation origin
    /// whereby it creates the mergeProps as per the schema
    type SetterCollector = (string * Expr) -> unit
    type SetterCollectorArray = ResizeArray<string * Expr>

    module SetterCollector =
        let createArray () = new SetterCollectorArray [||]
        let nullCollector: SetterCollector = fun (target: string, expr: Expr) -> ()
        let createCollector (arr: SetterCollectorArray) : (string * Expr) -> unit = arr.Add

    type GetterCollector = string -> unit
    type GetterCollectorArray = ResizeArray<string>

    module GetterCollector =
        let createArray () = new GetterCollectorArray [||]
        let nullCollector: GetterCollector = fun s -> ()
        let createCollector (arr: GetterCollectorArray) : string -> unit = arr.Add

    type CollectorArrays =
        { Setter: SetterCollectorArray
          Getter: GetterCollectorArray }

    module CollectorArrays =
        let create () : CollectorArrays =
            { Setter = SetterCollector.createArray ()
              Getter = GetterCollector.createArray () }

    type PropertyCollector =
        { Setter: SetterCollector
          Getter: GetterCollector }

    module PropertyCollector =
        let create (collectorArrays: CollectorArrays) =
            { Setter = SetterCollector.createCollector collectorArrays.Setter
              Getter = GetterCollector.createCollector collectorArrays.Getter }

        let addSetter collector = collector.Setter
        let addGetter collector = collector.Getter

    type PluginContext =
        { PropertyCollector: PropertyCollector }

    module PluginContext =
        let create collectorArrays =
            { PropertyCollector = PropertyCollector.create collectorArrays }

        let addSetter = _.PropertyCollector >> PropertyCollector.addSetter
        let addGetter = _.PropertyCollector >> PropertyCollector.addGetter
        let getterName = "local"
        let spreaderName = "others"

    /// Defines a property as a string and Expr tuple
    type PropInfo = string * Expr
    /// Defines a list of property tuples
    type PropList = PropInfo list

    /// Module containing transformation and expression handling of attributes and attribute related expressions.
    module Attributes =
        let private (|ExtensionHandler|_|): CallInfo -> PropInfo option =
            function
            | { Args = [ _; Value(StringConstant eventName, _); handler ] } -> Some(eventName, handler)
            | _ -> None

        /// Active pattern recognizer, matches expressions to property tuples where valid
        let private (|AttributeExpression|_|) (context: PluginContext) : Expr -> PropInfo option =
            function
            | Call(Import(importInfo, _, _), callInfo, _, _) ->
                match importInfo.Kind with
                | ImportKind.MemberImport(MemberRef(entity, memberRefInfo)) when
                    entity.FullName.StartsWith("Oxpecker.Solid")
                    || entity.FullName.StartsWith("Partas.Solid")
                    ->
                    match memberRefInfo.CompiledName, callInfo with
                    | "on", ExtensionHandler(eventName, handler) -> Some("on:" + eventName, handler)
                    | "bool", ExtensionHandler(eventName, handler) -> Some("bool:" + eventName, handler)
                    | "data", ExtensionHandler(eventName, handler) -> Some("data-" + eventName, handler)
                    | "attr", ExtensionHandler(eventName, handler) -> Some(eventName, handler)
                    | "ref", { Args = [ _; identExpr ] } -> Some("ref", identExpr)
                    | "style", { Args = [ _; identExpr ] } -> Some("style", identExpr)
                    | "classList", { Args = [ _; identExpr ] } -> Some("classList", identExpr)
                    | "__SPREAD_PROPERTY__", { Args = [ _; TypeCast(IdentExpr({ Name = "props" }), _) ] }
                    | "__SPREAD_PROPERTY__", { Args = [ _; IdentExpr({ Name = "props" }) ] }
                    | "spread", { Args = [ _; TypeCast(IdentExpr({ Name = "props" }), _) ] }
                    | "spread", { Args = [ _; IdentExpr({ Name = "props" }) ] } ->
                        Some("__SPREAD_PROPERTY__", Helpers.Baked.spreadPropertyExpression)
                    | "spread", { Args = [ _; identExpr ] } -> Some("__SPREAD_PROPERTY__", identExpr)
                    | _ when memberRefInfo.CompiledName.IndexOf("set_") >= 0 ->
                        let propName =
                            memberRefInfo.CompiledName.Split("set_")
                            |> function
                                | arr when arr.Length > 2 -> failwith "naw"
                                | [| _; name |] -> name
                                | _ -> failwith "naww"
                            |> function
                                | name when name.EndsWith("'") -> name |> Helpers.trimReservedIdentifiers
                                | name when name.StartsWith("aria") -> $"aria-{name.Substring(4).ToLower()}"
                                | name -> name

                        let propValue = callInfo.Args.Head

                        match propValue with
                        | GetSet.Getter(targetName, targetGetter) ->
                            propName |> PluginContext.addGetter context
                            (propName, Helpers.Baked.propGetter (PluginContext.getterName, targetGetter))
                        | IdentExpr({ Name = "props" })
                        | TypeCast(IdentExpr({ Name = "props" }), _) ->
                            (propName, Helpers.Baked.spreadPropertyExpression)
                        | TypeCast(expr, DeclaredType({ FullName = "Partas.Solid.Builder.HtmlElement" }, _))
                        | TypeCast(expr, DeclaredType({ FullName = "Oxpecker.Solid.Builder.HtmlElement" }, _)) ->
                            (propName, transform expr context)
                        | Delegate(args, expr, name, tags) ->
                            (propName, Delegate(args, transform expr context, name, tags))
                        | _ -> (propName, propValue)
                        |> Some
                    | _ -> None
                | _ -> None
            | Set(IdentExpr({ Name = Helpers.StartsWith "returnVal" _ }), SetKind.FieldSet name, _, handler, _) ->
                Some(Helpers.trimReservedIdentifiers name, handler)
            | _ -> None

        /// <summary>
        /// Given a list of expressions, it extracts all string * Expr pairs that make up a PropList
        /// </summary>
        let fromExpressions (context: PluginContext) : Expr list -> PropList =
            function
            | [] -> []
            | (Sequential expressions) :: rest -> fromExpressions context rest @ fromExpressions context expressions
            | AttributeExpression context propInfo :: rest -> propInfo :: fromExpressions context rest
            | _ :: rest -> fromExpressions context rest

        // let liftTagSource (context: PluginContext) (tagFinalizer: TagSource -> unit) (tagName: TagSource) (exprs: Expr list): Expr list =
        //     Console.WriteLine "EXPLORING HEAD!"
        //     Console.ForegroundColor <- ConsoleColor.Blue
        //     Console.WriteLine exprs.Head
        //     Console.ResetColor()
        //     Console.WriteLine "END HEAD"
        //     match exprs with
        //     | [] -> []
        //     | Tags.Call.NoChildrenWithHandler(NoChildren(LibraryImport(_) as imp, props, _)) :: rest ->
        //         Console.ForegroundColor <- ConsoleColor.Red
        //         Console.WriteLine imp
        //         Console.ResetColor()
        //         imp |> tagFinalizer
        //         props @ rest
        //     | (Sequential expressions) :: rest -> rest @ expressions
        //     | AttributeExpression context (name, expr) :: rest -> expr :: rest
        //     | _ :: rest -> rest
        //     |> function
        //         | [] -> []
        //         | rest -> rest |> liftTagSource context tagFinalizer tagName

        /// <summary>
        /// Given an existing propList and an expressions, it attempts to extract a prop string * Expr pair from the
        /// expression and add it to the propList
        /// </summary>
        /// <param name="propList">Existing list of properties (string * Expr)</param>
        /// <param name="expr">Expr which is possibly a property (string * Expr)</param>
        let tryAppend
            (propList: PropList)
            (context: PluginContext)
            (tagFinalizer: TagSource -> unit)
            (tagName: TagSource)
            (expr: Expr)
            : PropList =
            match expr with
            | Let({ Name = Helpers.StartsWith "returnVal" _ }, _, Sequential exprs) ->
                fromExpressions context exprs @ propList
            | Tags.Call.NoChildrenWithHandler(NoChildren(LibraryImport(_) as imp, props, _)) ->
                // TODO - check imp against tagName before lifting
                imp |> tagFinalizer
                fromExpressions context props @ propList
            | Tags.Call.NoChildrenWithHandler(NoChildren(_, props, _)) -> fromExpressions context props @ propList
            | _ -> propList

    type TagSource =
        | AutoImport of tagName: string
        | LibraryImport of imp: Expr

    type TagInfo =
        | WithChildren of tagName: TagSource * propsAndChildren: CallInfo * range: SourceLocation option
        | NoChildren of tagName: TagSource * props: Expr list * range: SourceLocation option
        | Combined of tagName: TagSource * props: Expr list * propsAndChildren: CallInfo * range: SourceLocation option

    /// Contains active recognizers for top level tag recognition.
    [<RequireQualifiedAccess>]
    module Tags =
        let (|LibraryTagImport|_|) (expr: Expr) =
            match expr with
            | Call(Import({ Kind = UserImport false }, Any, _) as imp,
                   { Tags = [ "new" ] },
                   DeclaredType(typ, []),
                   range) when
                (typ.FullName.StartsWith("Oxpecker.Solid")
                 || typ.FullName.StartsWith("Partas.Solid"))
                ->
                Some(imp, range)
            | Call(Import({ Selector = Helpers.EndsWithTrimmed "_$ctor" trimmedSelector
                            Kind = MemberImport(MemberRef(typ, _)) } as info,
                          importTyp,
                          importRange),
                   _,
                   _,
                   range) when
                (typ.FullName.StartsWith "Oxpecker.Solid"
                 || typ.FullName.StartsWith "Partas.Solid")
                && not (
                    typ.FullName.StartsWith "Oxpecker.Solid.Tags"
                    || typ.FullName.StartsWith "Partas.Solid.Tags"
                )
                ->
                Some(
                    Import(
                        { info with
                            Selector = trimmedSelector |> Helpers.trimReservedIdentifiers },
                        importTyp,
                        importRange
                    ),
                    range
                )
            | _ -> None
        // let (|TypeDefTagImport|_|) (expr: Expr) =
        //     match expr with
        //     | Call(
        //         Import(
        //             {
        //               Selector = Helpers.EndsWithTrimmed "_$ctor" trimmedSelector
        //               Kind = MemberImport(MemberRef(typ, _ ))
        //             } as info,
        //             importTyp,
        //             importRange
        //         ), _, _, range) when
        //         (typ.FullName.StartsWith "Oxpecker.Solid" || typ.FullName.StartsWith "Partas.Solid")
        //         && not (typ.FullName.StartsWith "Oxpecker.Solid.Tags" || typ.FullName.StartsWith "Partas.Solid.Tags")
        //          ->
        //         Some(Import({ info with Selector = trimmedSelector |> Helpers.trimReservedIdentifiers }, importTyp, importRange), range)
        //     | _ -> None
        let private (|CallTag|_|) condition =
            function
            | Call(Import(importInfo, LambdaType(_, DeclaredType(typ, _)), _), callInfo, _, range) when
                condition importInfo
                ->
                match callInfo.Args with
                | LibraryTagImport(imp, _) :: _
                | Let(_, LibraryTagImport(imp, _), _) :: _ -> LibraryImport imp
                | _ ->
                    match typ.FullName.Split('.') |> Seq.last with
                    | "Fragment" -> ""
                    | tagName -> tagName |> Helpers.trimReservedIdentifiers
                    |> AutoImport
                |> fun tagImport -> Some(tagImport, callInfo, range)
            | _ -> None
        // When we read the AST we typically will see tags being called with the builder computation (ie with children)
        // or without the computation (ie no children).
        /// Recognizer for tags defined by our schema that do not have builder computations
        let (|Constructor|_|) (expr: Expr) =
            let condition = _.Selector.EndsWith("_$ctor")

            match expr with
            | CallTag condition (_, _, range) & LibraryTagImport(tagName, _) -> Some(LibraryImport tagName, range)
            | CallTag condition (tagName, _, range) -> Some(tagName, range)
            | _ -> None

        /// Recognizer for tags defined by our schema that have builder computations
        let (|ConstructorBuilder|_|) (expr: Expr) =
            let condition =
                fun importInfo ->
                    [ "HtmlContainerExtensions_Run"; "BindingsModule_Extensions_run" ]
                    |> List.map importInfo.Selector.StartsWith
                    |> List.exists ((=) true)

            match expr with
            | CallTag condition tagCallInfo -> Some tagCallInfo
            | _ -> None

        let (|NoChildren|_|) = (|Constructor|_|)
        let (|WithChildren|_|) = (|ConstructorBuilder|_|)

        let (|TextNoSiblings|_|) =
            function
            | Lambda({ Name = cont }, TypeCast(textBody, Unit), None) when cont.StartsWith("cont") -> Some textBody
            | _ -> None

        module Let =
            let (|NoChildrenNoProps|NoChildrenWithProps|WithChildren|InvalidExpr|) =
                function
                | Let(_, Tags.ConstructorBuilder(withChildrenTag), _) ->
                    WithChildren(TagInfo.WithChildren withChildrenTag)
                | Let(_, Tags.Constructor(tagName, range), Sequential exprs) ->
                    NoChildrenWithProps(TagInfo.NoChildren(tagName, exprs, range))
                | Let(_, Tags.Constructor(tagName, range), _) ->
                    NoChildrenNoProps(TagInfo.NoChildren(tagName, [], range))
                | _ as expr -> InvalidExpr(expr)

        module Call =
            let (|NoChildrenWithHandler|_|) =
                function
                | Call(Import(ImportInfo.HtmlElementExtension _, _, _), { Args = arg :: _ }, _, range) as expr ->
                    match arg with
                    | Tags.Constructor(tagName, _) -> TagInfo.NoChildren(tagName, [ expr ], range) |> Some
                    | LibraryTagImport(imp, _) -> TagInfo.NoChildren(LibraryImport imp, [ expr ], range) |> Some
                    | Tags.Let.NoChildrenWithProps(TagInfo.NoChildren(tagName, props, _)) ->
                        TagInfo.NoChildren(tagName, expr :: props, range) |> Some
                    | Let(_, LibraryTagImport(imp, _), Sequential exprs) ->
                        TagInfo.NoChildren(LibraryImport imp, expr :: exprs, range) |> Some
                    | NoChildrenWithHandler(TagInfo.NoChildren(tagName, props, _)) ->
                        TagInfo.NoChildren(tagName, expr :: props, range) |> Some
                    | _ -> None


                | _ -> None

    module ImportInfo =
        let (|HtmlElementExtension|_|): ImportInfo -> ImportInfo option =
            function
            | { Selector = Helpers.StartsWith "HtmlElementExtensions_" _ } as importInfo -> Some importInfo
            | _ -> None

        let (|UserImport|_|): ImportInfo -> ImportInfo option =
            function
            | { Kind = ImportKind.UserImport false } as importInfo -> Some importInfo
            | _ -> None

    /// This module contains active recognizers and other utilities/transformations related to extracting and
    /// manipulating the children of a tag/expr
    module Children =
        let appendFromExpr (currentList: Expr list) (context: PluginContext) (expr: Expr) =
            let (|LetElement|_|) =
                function
                | Let({ Name = Helpers.StartsWith "element" _ }, _, _) -> Some()
                | _ -> None

            match expr with
            | Tags.Let.WithChildren tagInfo
            | LetElement & Let(_, Tags.Let.NoChildrenWithProps tagInfo, _)
            | LetElement & Tags.Let.NoChildrenNoProps tagInfo
            | LetElement & Let(_, Tags.Call.NoChildrenWithHandler tagInfo, _) ->
                let newExpr = tagInfo |> TagInfo.transform context
                newExpr :: currentList
            | LetElement & Let(_, next, _) ->
                let newExpr = AST.transform next context
                newExpr :: currentList
            // Lambda with two arguments returning element
            | Lambda({ Name = Helpers.StartsWith "cont" _ }, TypeCast(Lambda(item, Lambda(index, next, _), _), _), _) ->
                Delegate([ item; index ], AST.transform next context, None, []) :: currentList
            // Text Child
            | Tags.TextNoSiblings body -> body :: currentList
            // Text with solid signals inside
            | Let({ Name = Helpers.StartsWith "text" _ }, body, Tags.TextNoSiblings _) -> body :: currentList
            // Text and then tag
            | Let({ Name = Helpers.StartsWith "second" _ },
                  next,
                  Lambda({ Name = Helpers.StartsWith "builder" _ }, Sequential(TypeCast(textBody, Unit) :: _), _)) ->
                next |> Children.appendFromExpr currentList context
            // Parameter then another parameter
            | CurriedApply(Lambda({ Name = Helpers.StartsWith "cont" _ },
                                  TypeCast(lastParameter, Unit),
                                  Some(Helpers.StartsWith "second" _)),
                           _,
                           _,
                           _) -> lastParameter :: currentList
            | CurriedApply(Lambda({ Name = Helpers.StartsWith "builder" _ },
                                  Sequential [ TypeCast(middleParameter, Unit); next ],
                                  _),
                           _,
                           _,
                           _)
            | Lambda({ Name = Helpers.StartsWith "builder" _ }, Sequential [ TypeCast(middleParameter, Unit); next ], _) ->
                next |> Children.appendFromExpr (middleParameter :: currentList) context
            // tag then text
            | Let({ Name = Helpers.StartsWith "first" _ },
                  next1,
                  Lambda({ Name = Helpers.StartsWith "builder" _ }, Sequential [ CurriedApply _; next2 ], _)) ->
                let next1Children = next1 |> appendFromExpr [] context
                let next2Children = next2 |> appendFromExpr [] context
                next2Children @ next1Children @ currentList
            | Let({ Name = Helpers.StartsWith "first" _ },
                  Let(_, expr, _),
                  Let({ Name = Helpers.StartsWith "second" _ }, next, _))
            | Let({ Name = Helpers.StartsWith "first" _ }, expr, Let({ Name = Helpers.StartsWith "second" _ }, next, _)) ->
                match expr with
                | Tags.Let.NoChildrenWithProps tagInfo ->
                    let newExpr = tagInfo |> TagInfo.transform context
                    next |> appendFromExpr (newExpr :: currentList) context
                | Tags.Constructor(tagName, range) ->
                    let newExpr = TagInfo.NoChildren(tagName, [], range) |> TagInfo.transform context
                    next |> appendFromExpr (newExpr :: currentList) context
                | Tags.ConstructorBuilder callInfo ->
                    let newExpr = TagInfo.WithChildren(callInfo) |> TagInfo.transform context
                    next |> appendFromExpr (newExpr :: currentList) context
                | expr ->
                    let newExpr = AST.transform expr context
                    next |> appendFromExpr (newExpr :: currentList) context
            | IfThenElse(guardExpr, thenExpr, elseExpr, range) ->
                IfThenElse(guardExpr, AST.transform thenExpr context, AST.transform elseExpr context, range)
                :: currentList
            | DecisionTree(decisionTree, targets) ->
                DecisionTree(
                    decisionTree,
                    targets |> List.map (fun (target, expr) -> target, AST.transform expr context)
                )
                :: currentList
            // router cases
            | Call(Get(IdentExpr _, FieldGet _, Any, _), { Args = args }, _, _) ->
                match args with
                | [ Call(Import({ Selector = "uncurry2" }, Any, None), { Args = [ Lambda(_, body, _) ] }, _, _) ] ->
                    body |> appendFromExpr currentList context
                | [ Tags.LibraryTagImport(imp, _) ] ->
                    let newExpr =
                        TagInfo.NoChildren(LibraryImport imp, [], None) |> TagInfo.transform context

                    newExpr :: currentList
                | [ Let(_, Tags.LibraryTagImport(imp, _), Sequential exprs) ] ->
                    let newExpr =
                        TagInfo.NoChildren(LibraryImport imp, exprs, None) |> TagInfo.transform context

                    newExpr :: currentList
                | [ Let(_,
                        Let(_, Tags.LibraryTagImport(imp, _), Sequential exprs),
                        Tags.ConstructorBuilder(_, callInfo, _)) ] ->
                    let newExpr =
                        TagInfo.Combined(LibraryImport imp, exprs, callInfo, None)
                        |> TagInfo.transform context

                    newExpr :: currentList
                | [ next1; next2 ] ->
                    let next1Children = next1 |> appendFromExpr [] context
                    let next2Children = next2 |> appendFromExpr [] context
                    next2Children @ next1Children @ currentList
                | [ exprs ] -> exprs :: currentList
                | _ -> currentList
            | Let({ Name = name
                    Range = range
                    Type = DeclaredType({ FullName = fullName }, []) },
                  _,
                  _) when
                ((name.StartsWith("returnVal")
                  && (fullName.StartsWith("Oxpecker.Solid") || fullName.StartsWith("Partas.Solid")))
                 || (name.StartsWith("element")
                     && (fullName.StartsWith("Oxpecker.Solid") || fullName.StartsWith("Partas.Solid"))))
                |> not
                ->
                match range with
                | Some range ->
                    failwith $"`let` binding inside HTML CE can't be converted to JSX:line {range.start.line}\n{expr}"
                | None -> failwith $"`let` binding inside HTML CE can't be converted to JSX \n{expr}"
            | _ -> currentList

        let transform (context: PluginContext) (children: Expr list) =
            GetSet.transformCollection children context
            |> Helpers.Baked.exprListToExpr
            |> Helpers.wrapChildrenExpression

    module PropInfo =
        let transform (propInfo: PropInfo) =
            let name, expr = propInfo

            Value(
                kind =
                    NewTuple(
                        values = [ Value(kind = StringConstant name, range = None); TypeCast(expr, Type.Any) ],
                        isStruct = false
                    ),
                range = None
            )

    module PropList =
        let transform: PropList -> Expr list = List.map PropInfo.transform

    module TagSource =
        let transform: TagSource -> Expr =
            function
            | AutoImport tagName -> Value(StringConstant tagName, None)
            | LibraryImport expr -> expr

    /// Clean non obfuscated record to categorise final expression collections for
    /// transformation into JSX tags/elements
    type ElementBuilder =
        { TagName: TagSource
          Properties: PropList
          Children: Expr list
          Range: SourceLocation option }

    module ElementBuilder =
        /// Create an Element Builder
        let create tagName properties children range =
            { TagName = tagName
              Properties = properties
              Children = children
              Range = range }

        /// Creates an ElementBuilder from a TagInfo.
        /// Performs transformations/collections on TagInfo fields
        /// A collector is passed to collate property setters
        // TODO - should collectors be passed through this point? We should only collect the top scope of the func
        let fromTagInfo (context: PluginContext) : TagInfo -> ElementBuilder =
            function
            | WithChildren(tagName, callInfo, range) ->
                let mutable finalTagSource = tagName
                let tagFinalizer: TagSource -> unit = fun tagSource -> finalTagSource <- tagSource

                let properties =
                    callInfo.Args
                    |> List.fold (fun state -> Attributes.tryAppend state context tagFinalizer tagName) []

                let children =
                    callInfo.Args
                    |> List.fold (fun state -> Children.appendFromExpr state context) []

                create finalTagSource properties children range
            | NoChildren(tagName, propList, range) ->
                // let mutable finalTagSource = tagName
                // let tagFinalizer: TagSource -> unit = fun tagSource -> finalTagSource <- tagSource
                // let _ = propList |> Attributes.liftTagSource context tagFinalizer tagName
                let properties = propList |> Attributes.fromExpressions context
                let children = []
                // Console.WriteLine finalTagSource
                create tagName properties children range
            | Combined(tagName, propList, callInfo, range) ->
                // let mutable finalTagSource = tagName
                // let tagFinalizer: TagSource -> unit = fun tagSource -> finalTagSource <- tagSource
                let properties = propList |> Attributes.fromExpressions context
                // let otherProperties = callInfo.Args |> List.fold (fun state -> Attributes.tryAppend state context tagFinalizer tagName ) []
                let children =
                    callInfo.Args
                    |> List.fold (fun state -> Children.appendFromExpr state context) []
                // create finalTagSource (otherProperties @ properties) children range
                create tagName properties children range

        let transform context (elementBuilder: ElementBuilder) =
            let tagNameExpr = elementBuilder.TagName |> TagSource.transform

            let internalCollection =
                (elementBuilder.Children |> Children.transform context)
                :: (elementBuilder.Properties |> PropList.transform)
                |> Helpers.Baked.exprListToExpr

            Call(
                callee = Helpers.Baked.importJsxCreate,
                info =
                    { ThisArg = None
                      Args = [ tagNameExpr; internalCollection ]
                      SignatureArgTypes = []
                      GenericArgs = []
                      MemberRef = None
                      Tags = [ "jsx" ] },
                typ = Helpers.Baked.jsxElementType,
                range = elementBuilder.Range
            )

    module TagInfo =
        let transform (context: PluginContext) =
            ElementBuilder.fromTagInfo context >> ElementBuilder.transform context

    /// Contains recognizers that match getters/setters of properties
    /// Also contains Transformers that can excise Setters into a collector and
    /// transforms Getters into compatible JSX
    module GetSet =
        let (|Setter|_|) =
            function
            | Call(_,
                   { ThisArg = Some(Expr.IdentExpr({ Name = "props" }) as identExpr)
                     Args = [ args ]
                     MemberRef = Some(MemberRef({ FullName = fullName }, { CompiledName = compiledName }))
                     Tags = [ "value" ] },
                   _,
                   _) as expr when fullName.StartsWith "Oxpecker.Solid" || fullName.StartsWith "Partas.Solid" ->
                match compiledName with
                | Helpers.StartsWithTrimmed "set_" setTarget
                | Helpers.StartsWithTrimmed "HtmlTag.set_" setTarget -> Some(identExpr, setTarget, args)
                | _ -> None
            | _ -> None

        let (|Getter|_|) =
            function
            | Call(Import({ Selector = selector },
                          LambdaType(DeclaredType({ FullName = Helpers.EndsWith "Solid.Builder.HtmlTag" _ }, []), _),
                          None),
                   { ThisArg = Some(IdentExpr({ Name = "props" as targetName }))
                     MemberRef = Some(MemberRef(_, { CompiledName = compiledName }))
                     Tags = [ "value" ] },
                   _,
                   _) as expr when
                selector.StartsWith "Oxpecker_Solid_Builder_HtmlTag"
                || selector.StartsWith "Partas_Solid_Builder_HtmlTag"
                ->
                match compiledName with
                | Helpers.StartsWithTrimmed "get_" getTarget
                | Helpers.StartsWithTrimmed "HtmlTag.get_" getTarget -> Some(targetName, getTarget)
                | _ -> None
            | _ -> None

        let private transform (context: PluginContext) =
            function
            | Setter(identExpr, setTarget, args) ->
                (setTarget, args) |> PluginContext.addSetter context
                None
            | Getter(targetName, getTarget)
            | TypeCast(Getter(targetName, getTarget), _) ->
                PluginContext.addGetter context getTarget
                Helpers.Baked.propGetter (PluginContext.getterName, getTarget) |> Some
            | _ as expr -> expr |> Some

        let transformCollection (exprs: Expr list) (context: PluginContext) = List.choose (transform context) exprs

    let transform (expr: Expr) (context: PluginContext) =
        let (|LetElement|_|) =
            function
            | Let({ Name = Helpers.StartsWith "element" _ }, _, _) -> Some()
            | _ -> None

        match expr with
        | Tags.Let.NoChildrenWithProps tagCall -> tagCall |> TagInfo.transform context
        | LetElement & Let(_, Tags.Let.NoChildrenWithProps tagCall, _) -> tagCall |> TagInfo.transform context
        | Tags.NoChildren(tagName, range) -> TagInfo.NoChildren(tagName, [], range) |> TagInfo.transform context
        | Tags.Call.NoChildrenWithHandler(tagCall)
        | Tags.Let.NoChildrenNoProps(tagCall) -> tagCall |> TagInfo.transform context
        | Tags.ConstructorBuilder(callInfo) -> TagInfo.WithChildren callInfo |> TagInfo.transform context
        | Tags.Let.WithChildren(tagCall) -> tagCall |> TagInfo.transform context
        | Tags.LibraryTagImport(imp, range) ->
            TagInfo.NoChildren(LibraryImport imp, [], range) |> TagInfo.transform context
        | Let(_, Tags.LibraryTagImport(imp, range), Sequential exprs) ->
            TagInfo.NoChildren(LibraryImport imp, exprs, range) |> TagInfo.transform context
        | Let(_, Tags.LibraryTagImport(imp, range), Tags.ConstructorBuilder(_, callInfo, _)) ->
            TagInfo.WithChildren(LibraryImport imp, callInfo, range)
            |> TagInfo.transform context
        | Let(_, Let(_, Tags.LibraryTagImport(imp, range), Sequential exprs), Tags.ConstructorBuilder(_, callInfo, _)) ->
            TagInfo.Combined(LibraryImport imp, exprs, callInfo, range)
            |> TagInfo.transform context
        // | Let(_, Tags.LibraryTagImport(imp, range), Tags.Constructor)
        | Let(name, value, expr) -> Let(name, value, AST.transform expr context)
        // Given a sequence of expressions, we transform the last
        | Sequential expressions ->
            expressions
            |> List.rev
            |> function
                | head :: tail -> // transform last expr only
                    transform head context :: GetSet.transformCollection tail context // extract setters if present
                | [] -> []
            |> List.rev
            |> Sequential
        | IfThenElse(guardExpr, thenExpr, elseExpr, range) ->
            IfThenElse(guardExpr, transform thenExpr context, transform elseExpr context, range)
        | DecisionTree(decisionTree, targets) ->
            targets
            |> List.map (fun (target, expr) -> target, transform expr context)
            |> fun targets -> DecisionTree(decisionTree, targets)
        | TypeCast(expr, DeclaredType _) -> transform expr context
        | _ -> expr

    let transformException (pluginHelper: PluginHelper) (range: SourceLocation option) =
        let childrenExpression =
            Value(
                NewList(
                    Some(
                        Value(StringConstant $"Fable compilation error in {pluginHelper.CurrentFile}", None),
                        Value(NewList(None, Type.Tuple([ Type.String; Type.Any ], false)), None)
                    ),
                    Type.Tuple([ Type.String; Type.Any ], false)
                ),
                None
            )

        let text = childrenExpression |> Helpers.wrapChildrenExpression
        let finalList = text :: []
        let propsExpr = finalList |> Helpers.Baked.exprListToExpr

        Call(
            callee = Helpers.Baked.importJsxCreate,
            info =
                { ThisArg = None
                  Args = [ Value(StringConstant "h1", None); propsExpr ]
                  SignatureArgTypes = []
                  GenericArgs = []
                  MemberRef = None
                  Tags = [ "jsx" ] },
            typ = Helpers.Baked.jsxElementType,
            range = range
        )

// TODO - Attribute switch that changes behaviour of transformer depending on union val
[<RequireQualifiedAccess>]
type SolidAssistance =
    | Auto
    | None

/// <summary>
/// Converts decl into Solid JSX func <br/>
/// A schema applies when applied to type members <br/>
/// </summary>
type SolidTypeComponentAttribute(typ: SolidAssistance) =
    inherit MemberDeclarationPluginAttribute()
    let subtype = typ

    /// Identifies the attribute target against our schema
    let (|TypeDefDecl|InvalidDecl|SomeDecl|) (input: MemberDecl) =
        match input with
        | { MemberRef = MemberRef(({ FullName = tagName }), ({ CompiledName = "typeDef" })) } as mDecl when
            tagName.StartsWith "Oxpecker.Solid" || tagName.StartsWith "Partas.Solid"
            ->
            match mDecl with
            | { Args = idents } when idents.Length <> 2 -> InvalidDecl
            | { Args = [ selfIdentifier
                         { Name = "unitVar"
                           Type = Unit
                           IsCompilerGenerated = true } ] } ->
                match selfIdentifier.Name with
                | "props" -> TypeDefDecl
                | _ -> InvalidDecl
            | _ -> InvalidDecl
        // is a typedef decl
        // ensure takes no args other than self identifier
        | _ -> SomeDecl

    override _.FableMinimumVersion = "4.0"

    override this.Transform(pluginHelper: PluginHelper, file: File, memberDecl: MemberDecl) =
        let storage = AST.CollectorArrays.create ()
        let context = AST.PluginContext.create storage

        let newBody =
            match memberDecl.Body with
            | Extended(Throw _, range) -> AST.transformException pluginHelper range
            | _ -> AST.transform memberDecl.Body context // pass collector through transformers

        let finalBody =
            let bodyWithSplitter =
                newBody
                |> Helpers.Baked.convertGettersToObject (storage.Getter.ToArray() |> Array.toList)

            if storage.Setter.Count > 0 then // if array holds items, then create a mergeProps
                let mergeExpr =
                    storage.Setter.ToArray() |> Array.toList |> Helpers.Baked.convertSettersToObject

                Sequential([ mergeExpr; bodyWithSplitter ]) // add mergeProps to head of body
            else
                bodyWithSplitter // if no items in array then carry on

        let name =
            match memberDecl with
            | SomeDecl -> memberDecl.Name
            | TypeDefDecl -> // if typedef then clip typedef off name
                memberDecl.Name.Substring(0, memberDecl.Name.Length - ("__typeDef").Length)
            | InvalidDecl -> failwith "Invalid MemberDecl target" // an invalid typedef

        { memberDecl with
            Body = finalBody
            Name = name }

    override this.TransformCall(_: PluginHelper, _: MemberFunctionOrValue, expr: Expr) : Expr =
        // Console.WriteLine(expr)
        expr

    new() = SolidTypeComponentAttribute(SolidAssistance.Auto)

type PrintExpressionAttribute() =
    inherit MemberDeclarationPluginAttribute()
    override this.FableMinimumVersion = "4.0"

    override this.Transform(pluginHelper, file, memberDecl) =
        Console.WriteLine memberDecl
        memberDecl

    override this.TransformCall(var0, member_, expr) =
        Console.ForegroundColor <- ConsoleColor.Yellow
        Console.WriteLine expr
        expr
