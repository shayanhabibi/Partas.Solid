/// <summary>
/// <b>WARNING</b><br/>
/// This module contains syntax sugars that are not yet battle tested.<br/><br/>
/// Contains the following builders:<br/>effect<br/>memo<br/>mount<br/>cleanup<br/>batch<br/>lazyload<br/>selector<br/>children
/// </summary>
module Partas.Solid.Experimental

open Partas.Solid
open Fable.Core

#nowarn 64 49

/// <summary>
/// Lambdas that are of the signature <c>unit -> unit</c> are common enough that we can
/// build this as a base for things like <c>createEffect</c> to inherit
/// </summary>
[<Erase>]
type NullLambdaBuilder() =
    member inline _.Return(x) = fun () -> x
    member inline _.Bind(m,f) = fun () -> f (m()) ()
    member inline _.Zero() = ignore
    member inline _.Delay(f) = f
    member inline _.Combine([<InlineIfLambda>] PARTAS_FIRST: 'T -> unit,[<InlineIfLambda>] PARTAS_SECOND) = fun () -> PARTAS_FIRST(); PARTAS_SECOND()

/// <summary>
/// Lambdas that take null parameters and return a type are common enough to build
/// a sugar wrapper that things like <c>createMemo</c> can inherit
/// </summary>
[<Erase>]
type BaseLambdaBuilder() =
    member inline _.Return(x) = fun () -> x
    member inline _.Bind(m,f) = fun () -> f (m()) ()
    member inline _.Delay(f) = f
    member inline _.Combine([<InlineIfLambda>] PARTAS_FIRST: 'T -> unit,[<InlineIfLambda>] PARTAS_SECOND) = ignore PARTAS_FIRST; PARTAS_SECOND()
    member inline _.Yield(PARTAS_VALUE) = PARTAS_VALUE

[<Erase>]
type LambdaBuilder() =
    inherit BaseLambdaBuilder()
    member inline _.Zero() = ignore
    member inline _.Run(code: unit -> 'T): unit -> 'T = fun () -> code()
[<Erase>]
type BatchBuilder() =
    inherit BaseLambdaBuilder()
    member inline _.Run(code: unit -> 'T): 'T = batch(fun () -> code())
[<Erase>]
type CreateEffectBuilder() =
    inherit NullLambdaBuilder()
    member inline _.Run(effect) = createEffect(fun () -> effect() |> ignore)
[<Erase>]
type OnMountBuilder() =
    inherit NullLambdaBuilder()
    member inline _.Run(effect) = onMount(fun () -> effect() |> ignore)
[<Erase>]
type OnCleanupBuilder() =
    inherit NullLambdaBuilder()
    member inline _.Run(effect) = onCleanup(fun () -> effect() |> ignore)
[<Erase>]
type CreateMemoBuilder() =
    inherit BaseLambdaBuilder()
    member inline _.Run(computation: unit -> 'T): unit -> 'T = createMemo(fun () -> computation())
[<Erase>]
type CreateSelectorBuilder() =
    inherit BaseLambdaBuilder()
    member inline _.Run(computation: unit -> 'T): 'U -> bool = createSelector(fun () -> computation())
[<Erase>]
type CreateReactionBuilder() =
    inherit BaseLambdaBuilder()
    member inline _.Run(computation: unit -> unit): (unit -> unit) -> unit = createReaction(fun () -> computation())
[<Erase>]
type ChildrenBuilder() =
    inherit BaseLambdaBuilder()
    member inline _.Run(computation: unit -> #HtmlElement): unit -> #HtmlElement = children(fun () -> computation())
[<Erase>]
type LazyBuilder() =
    inherit BaseLambdaBuilder()
    member inline _.Run(computation: unit -> 'T): 'C = lazy'(fun () -> computation())

[<AutoOpen;Erase>]
module Builders =
    /// <summary>
    /// Wraps the computation in <c>fun () -> ...</c>
    /// </summary>
    /// <example><code>
    /// let config = lambda { Data.config.data }
    /// console.log (config())
    /// </code></example>
    let [<Erase>] lambda = LambdaBuilder()
    /// <summary>
    /// Wraps the computation in <c>createEffect(fun () -> ...)</c>
    /// </summary>
    /// <example><code>
    /// effect {
    ///     match Data.Onboarding.accessor() with
    ///     | Some result ->
    ///         Data.Navigation.store
    ///             .Update(navigation result.config.IsOk)
    ///     | _ -> ()
    /// }
    /// </code></example>
    let [<Erase>] effect = CreateEffectBuilder()
    /// <summary>
    /// Wraps the computation in <c>onMount(fun () -> ...)</c>
    /// </summary>
    /// <example><code>
    /// mount {
    ///     configStore.Update Data.Config.data
    /// }
    /// </code></example>
    let [<Erase>] mount = OnMountBuilder()
    /// <summary>
    /// Wraps the computation in <c>onCleanup(fun () -> ...)</c>
    /// </summary>
    /// <example><code>
    /// cleanup {
    ///     configStore.Update Data.Config.data
    /// }
    /// </code></example>
    let [<Erase>] cleanup = OnMountBuilder()
    /// <summary>
    /// Wraps the computation in <c>createMemo(fun () -> ...)</c>
    /// </summary>
    /// <example><code>
    /// memo {
    ///     // code here that returns
    ///     // some expensive value
    /// }
    /// </code></example>
    let [<Erase>] memo = CreateMemoBuilder()
    /// <summary>
    /// Wraps the computation in <c>batch(fun () -> ...)</c>
    /// </summary>
    /// <example><code>
    /// let submissionResult = batch {
    ///     // ... some reactive tasks here
    /// }
    /// </code></example>
    let [<Erase>] batch = BatchBuilder()
    /// <summary>
    /// Wraps the computation in <c>lazy'(fun () -> ...)</c>
    /// </summary>
    /// <example><code>
    /// let comp = lazyload {
    ///     importComponent "./MyComponent.fs.jsx"
    /// }
    /// </code></example>
    let [<Erase>] lazyload = LazyBuilder()
    /// <summary>
    /// Wraps the computation in <c>createSelect(fun () -> ...)</c>
    /// </summary>
    /// <example><code>
    /// let isSelected = selector {
    ///     // ...
    /// }
    /// console.log (isSelected item)
    /// </code></example>
    let [<Erase>] selector = CreateSelectorBuilder()
    /// <summary>
    /// Wraps the computation in <c>children(fun () -> ...)</c>
    /// </summary>
    /// <example><code>
    /// let childs = children {
    ///     props.children
    /// }
    /// div() { childs() }
    /// </code></example>
    let [<Erase>] children = ChildrenBuilder()


/// <summary>
/// Reimplements the Erased Unions with implicit conversion from types into the erasure.
/// This simplifies bindings and consuming bindings as type signatures more closely resemble
/// the target.<br/>
/// Experimenting to see whether this improves or detracts from the developer experience.
/// </summary>
[<Erase>]
module U =
    /// Erased union type to represent one of two possible values.
    /// More info: https://fable.io/docs/communicate/js-from-fable.html#erase-attribute
    [<Erase>]
    type U2<'a, 'b> =
        | Case1 of 'a
        | Case2 of 'b

        static member op_ErasedCast(x: 'a) = Case1 x
        static member op_ErasedCast(x: 'b) = Case2 x
        static member inline op_Implicit(x: 'a): U2<'a, 'b> = Case1 x
        static member inline op_Implicit(x: 'b): U2<'a, 'b> = Case2 x

    /// Erased union type to represent one of three possible values.
    /// More info: https://fable.io/docs/communicate/js-from-fable.html#erase-attribute
    [<Erase>]
    type U3<'a, 'b, 'c> =
        | Case1 of 'a
        | Case2 of 'b
        | Case3 of 'c

        static member op_ErasedCast(x: 'a) = Case1 x
        static member op_ErasedCast(x: 'b) = Case2 x
        static member op_ErasedCast(x: 'c) = Case3 x
        static member inline op_Implicit(x: 'a): U3<'a, 'b, 'c> = Case1 x
        static member inline op_Implicit(x: 'b): U3<'a, 'b, 'c> = Case2 x
        static member inline op_Implicit(x: 'c): U3<'a, 'b, 'c> = Case3 x

    /// Erased union type to represent one of four possible values.
    /// More info: https://fable.io/docs/communicate/js-from-fable.html#erase-attribute
    [<Erase>]
    type U4<'a, 'b, 'c, 'd> =
        | Case1 of 'a
        | Case2 of 'b
        | Case3 of 'c
        | Case4 of 'd

        static member op_ErasedCast(x: 'a) = Case1 x
        static member op_ErasedCast(x: 'b) = Case2 x
        static member op_ErasedCast(x: 'c) = Case3 x
        static member op_ErasedCast(x: 'd) = Case4 x
        static member inline op_Implicit(x: 'a): U4<'a, 'b, 'c, 'd> = Case1 x
        static member inline op_Implicit(x: 'b): U4<'a, 'b, 'c, 'd> = Case2 x
        static member inline op_Implicit(x: 'c): U4<'a, 'b, 'c, 'd> = Case3 x
        static member inline op_Implicit(x: 'd): U4<'a, 'b, 'c, 'd> = Case4 x

    /// Erased union type to represent one of five possible values.
    /// More info: https://fable.io/docs/communicate/js-from-fable.html#erase-attribute
    [<Erase>]
    type U5<'a, 'b, 'c, 'd, 'e> =
        | Case1 of 'a
        | Case2 of 'b
        | Case3 of 'c
        | Case4 of 'd
        | Case5 of 'e

        static member op_ErasedCast(x: 'a) = Case1 x
        static member op_ErasedCast(x: 'b) = Case2 x
        static member op_ErasedCast(x: 'c) = Case3 x
        static member op_ErasedCast(x: 'd) = Case4 x
        static member op_ErasedCast(x: 'e) = Case5 x
        static member inline op_Implicit(x: 'a): U5<'a, 'b, 'c, 'd, 'e> = Case1 x
        static member inline op_Implicit(x: 'b): U5<'a, 'b, 'c, 'd, 'e> = Case2 x
        static member inline op_Implicit(x: 'c): U5<'a, 'b, 'c, 'd, 'e> = Case3 x
        static member inline op_Implicit(x: 'd): U5<'a, 'b, 'c, 'd, 'e> = Case4 x
        static member inline op_Implicit(x: 'e): U5<'a, 'b, 'c, 'd, 'e> = Case5 x

    /// Erased union type to represent one of six possible values.
    /// More info: https://fable.io/docs/communicate/js-from-fable.html#erase-attribute
    [<Erase>]
    type U6<'a, 'b, 'c, 'd, 'e, 'f> =
        | Case1 of 'a
        | Case2 of 'b
        | Case3 of 'c
        | Case4 of 'd
        | Case5 of 'e
        | Case6 of 'f

        static member op_ErasedCast(x: 'a) = Case1 x
        static member op_ErasedCast(x: 'b) = Case2 x
        static member op_ErasedCast(x: 'c) = Case3 x
        static member op_ErasedCast(x: 'd) = Case4 x
        static member op_ErasedCast(x: 'e) = Case5 x
        static member op_ErasedCast(x: 'f) = Case6 x
        static member inline op_Implicit(x: 'a): U6<'a, 'b, 'c, 'd, 'e, 'f> = Case1 x
        static member inline op_Implicit(x: 'b): U6<'a, 'b, 'c, 'd, 'e, 'f> = Case2 x
        static member inline op_Implicit(x: 'c): U6<'a, 'b, 'c, 'd, 'e, 'f> = Case3 x
        static member inline op_Implicit(x: 'd): U6<'a, 'b, 'c, 'd, 'e, 'f> = Case4 x
        static member inline op_Implicit(x: 'e): U6<'a, 'b, 'c, 'd, 'e, 'f> = Case5 x
        static member inline op_Implicit(x: 'f): U6<'a, 'b, 'c, 'd, 'e, 'f> = Case6 x

    /// Erased union type to represent one of seven possible values.
    /// More info: https://fable.io/docs/communicate/js-from-fable.html#erase-attribute
    [<Erase>]
    type U7<'a, 'b, 'c, 'd, 'e, 'f, 'g> =
        | Case1 of 'a
        | Case2 of 'b
        | Case3 of 'c
        | Case4 of 'd
        | Case5 of 'e
        | Case6 of 'f
        | Case7 of 'g

        static member op_ErasedCast(x: 'a) = Case1 x
        static member op_ErasedCast(x: 'b) = Case2 x
        static member op_ErasedCast(x: 'c) = Case3 x
        static member op_ErasedCast(x: 'd) = Case4 x
        static member op_ErasedCast(x: 'e) = Case5 x
        static member op_ErasedCast(x: 'f) = Case6 x
        static member op_ErasedCast(x: 'g) = Case7 x
        static member inline op_Implicit(x: 'a): U7<'a, 'b, 'c, 'd, 'e, 'f, 'g> = Case1 x
        static member inline op_Implicit(x: 'b): U7<'a, 'b, 'c, 'd, 'e, 'f, 'g> = Case2 x
        static member inline op_Implicit(x: 'c): U7<'a, 'b, 'c, 'd, 'e, 'f, 'g> = Case3 x
        static member inline op_Implicit(x: 'd): U7<'a, 'b, 'c, 'd, 'e, 'f, 'g> = Case4 x
        static member inline op_Implicit(x: 'e): U7<'a, 'b, 'c, 'd, 'e, 'f, 'g> = Case5 x
        static member inline op_Implicit(x: 'f): U7<'a, 'b, 'c, 'd, 'e, 'f, 'g> = Case6 x
        static member inline op_Implicit(x: 'g): U7<'a, 'b, 'c, 'd, 'e, 'f, 'g> = Case7 x

    /// Erased union type to represent one of eight possible values.
    /// More info: https://fable.io/docs/communicate/js-from-fable.html#erase-attribute
    [<Erase>]
    type U8<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h> =
        | Case1 of 'a
        | Case2 of 'b
        | Case3 of 'c
        | Case4 of 'd
        | Case5 of 'e
        | Case6 of 'f
        | Case7 of 'g
        | Case8 of 'h

        static member op_ErasedCast(x: 'a) = Case1 x
        static member op_ErasedCast(x: 'b) = Case2 x
        static member op_ErasedCast(x: 'c) = Case3 x
        static member op_ErasedCast(x: 'd) = Case4 x
        static member op_ErasedCast(x: 'e) = Case5 x
        static member op_ErasedCast(x: 'f) = Case6 x
        static member op_ErasedCast(x: 'g) = Case7 x
        static member op_ErasedCast(x: 'h) = Case8 x
        static member inline op_Implicit(x: 'a): U8<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h> = Case1 x
        static member inline op_Implicit(x: 'b): U8<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h> = Case2 x
        static member inline op_Implicit(x: 'c): U8<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h> = Case3 x
        static member inline op_Implicit(x: 'd): U8<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h> = Case4 x
        static member inline op_Implicit(x: 'e): U8<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h> = Case5 x
        static member inline op_Implicit(x: 'f): U8<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h> = Case6 x
        static member inline op_Implicit(x: 'g): U8<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h> = Case7 x
        static member inline op_Implicit(x: 'h): U8<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h> = Case8 x

    /// Erased union type to represent one of nine or more possible values.
    /// More info: https://fable.io/docs/communicate/js-from-fable.html#erase-attribute
    [<Erase>]
    type U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> =
        | Case1 of 'a
        | Case2 of 'b
        | Case3 of 'c
        | Case4 of 'd
        | Case5 of 'e
        | Case6 of 'f
        | Case7 of 'g
        | Case8 of 'h
        | Case9 of 'i

        static member op_ErasedCast(x: 'a) = Case1 x
        static member op_ErasedCast(x: 'b) = Case2 x
        static member op_ErasedCast(x: 'c) = Case3 x
        static member op_ErasedCast(x: 'd) = Case4 x
        static member op_ErasedCast(x: 'e) = Case5 x
        static member op_ErasedCast(x: 'f) = Case6 x
        static member op_ErasedCast(x: 'g) = Case7 x
        static member op_ErasedCast(x: 'h) = Case8 x
        static member op_ErasedCast(x: 'i) = Case9 x
        static member inline op_Implicit(x: 'a): U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> = Case1 x
        static member inline op_Implicit(x: 'b): U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> = Case2 x
        static member inline op_Implicit(x: 'c): U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> = Case3 x
        static member inline op_Implicit(x: 'd): U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> = Case4 x
        static member inline op_Implicit(x: 'e): U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> = Case5 x
        static member inline op_Implicit(x: 'f): U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> = Case6 x
        static member inline op_Implicit(x: 'g): U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> = Case7 x
        static member inline op_Implicit(x: 'h): U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> = Case8 x
        static member inline op_Implicit(x: 'i): U9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i> = Case9 x

        static member inline op_ErasedCast(x: 't) : U9<_, _, _, _, _, _, _, _, ^U> =
            Case9(^U: (static member op_ErasedCast: 't -> ^U) x)