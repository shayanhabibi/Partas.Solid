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
    