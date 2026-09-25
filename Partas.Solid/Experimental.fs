/// <summary>
/// <b>WARNING</b><br/>
/// This module contains syntax sugars that are not yet battle tested.<br/><br/>
/// Contains the following builders:<br/>lambda<br/>effect<br/>memo<br/>mount<br/>cleanup<br/>lazyload<br/>children<br/>reaction
/// </summary>
namespace Partas.Solid.Experimental

open Partas.Solid
open Fable.Core

#nowarn 64 49

/// <summary>
/// Lambdas that are of the signature <c>unit -> unit</c> are common enough that we can
/// build this as a base for things like <c>createEffect</c> to inherit
/// </summary>
[<EB(EBState.Never)>]
[<Erase>]
type NullLambdaBuilder() =
    member inline _.Return(x) =
        fun () -> x

    member inline _.Bind(m, f) =
        fun () -> f (m ()) ()

    member inline _.Zero() = ignore
    member inline _.Delay(f) = f

    member inline _.Combine([<InlineIfLambda>] PARTAS_FIRST: 'T -> unit, [<InlineIfLambda>] PARTAS_SECOND) =
        fun () ->
            PARTAS_FIRST ()
            PARTAS_SECOND ()

/// <summary>
/// Lambdas that take null parameters and return a type are common enough to build
/// a sugar wrapper that things like <c>createMemo</c> can inherit
/// </summary>
[<EB(EBState.Never)>]
[<Erase>]
type BaseLambdaBuilder() =
    member inline _.Return(x) =
        fun () -> x

    member inline _.Bind(m, f) =
        fun () -> f (m ()) ()

    member inline _.Delay(f) = f

    member inline _.Combine([<InlineIfLambda>] PARTAS_FIRST: 'T -> unit, [<InlineIfLambda>] PARTAS_SECOND) =
        ignore PARTAS_FIRST
        PARTAS_SECOND ()

    member inline _.Yield(PARTAS_VALUE) =
        PARTAS_VALUE

[<EB(EBState.Never)>]
[<Erase>]
type LambdaBuilder() =
    inherit BaseLambdaBuilder()
    member inline _.Zero() = ignore

    member inline _.Run(code: unit -> 'T) : unit -> 'T =
        fun () -> code ()

/// <summary>
/// Returned by the <c>let!</c> inside an <c>effect { }</c>. Forces that computation to name
/// its tracked source, since Solid 2 no longer has a single-phase <c>createEffect</c>.
/// </summary>
[<EB(EBState.Never)>]
type EffectDeclaration = interface end

/// <summary>
/// Splits the computation into Solid 2's two phases: the <c>let!</c> source is the tracked compute
/// phase, and everything after it is the untracked effect phase.
/// </summary>
[<EB(EBState.Never)>]
[<Erase>]
type CreateEffectBuilder() =
    member inline _.Zero() = ()
    member inline _.Delay(f: unit -> EffectDeclaration) = f

    /// <c>createEffect</c> typed to return the declaration, so the builder never has to unbox a
    /// <c>unit</c> call (which Fable emits as <c>(createEffect(...), undefined)</c> at module level).
    [<Import("createEffect", "solid-js"); EB(EBState.Never)>]
    static member Declare<'T>(compute: 'T option -> 'T, effectFn: 'T -> unit) : EffectDeclaration = jsNative

    member inline _.Bind(compute: Accessor<'T>, effectFn: 'T -> unit) : EffectDeclaration =
        CreateEffectBuilder.Declare ((fun (_: 'T option) -> compute ()), effectFn)

    member inline _.Run(effect: unit -> EffectDeclaration) : unit =
        effect ()
        |> ignore

[<EB(EBState.Never)>]
[<Erase>]
type OnSettledBuilder() =
    inherit NullLambdaBuilder()

    member inline _.Run(effect) =
        onSettled (fun () ->
            effect ()
            |> ignore)

[<EB(EBState.Never)>]
[<Erase>]
type OnCleanupBuilder() =
    inherit NullLambdaBuilder()

    member inline _.Run(effect) =
        onCleanup (fun () ->
            effect ()
            |> ignore)

[<EB(EBState.Never)>]
[<Erase>]
type CreateMemoBuilder() =
    inherit BaseLambdaBuilder()

    member inline _.Run(computation: unit -> 'T) : Accessor<'T> =
        createMemo (fun (_: 'T option) -> computation ())

[<EB(EBState.Never)>]
[<Erase>]
type CreateReactionBuilder() =
    inherit BaseLambdaBuilder()

    member inline _.Run(computation: unit -> unit) : (unit -> objnull) -> unit =
        createReaction (fun () -> computation ())

    member inline _.Zero(value: unit) =
        ignore value

[<EB(EBState.Never)>]
[<Erase>]
type ChildrenBuilder() =
    inherit BaseLambdaBuilder()

    member inline _.Run<'T when 'T :> HtmlElement>(computation: unit -> 'T) : ChildrenReturn<'T> =
        children (fun () -> computation ())

[<EB(EBState.Never)>]
[<Erase>]
type LazyBuilder() =
    inherit BaseLambdaBuilder()

    member inline _.Run<'T when 'T :> HtmlElement>(computation: unit -> JS.Promise<'T>) : LazyComponent<'T> =
        lazy' (fun () -> computation ())

[<AutoOpen; Erase>]
module Builders =
    /// <summary>
    /// Wraps the computation in <c>fun () -> ...</c>
    /// </summary>
    /// <example><code>
    /// let config = lambda { Data.config.data }
    /// console.log (config())
    /// </code></example>
    [<Erase>]
    let lambda = LambdaBuilder ()

    /// <summary>
    /// Wraps the computation in <c>createEffect(compute, effectFn)</c>. The <c>let!</c> binds the
    /// tracked compute phase; the rest of the body is the untracked effect phase, and receives its value.
    /// </summary>
    /// <remarks>
    /// Exactly one <c>let!</c> is required. Track several sources by binding one accessor that reads them all,
    /// e.g. <c>let! a, b = lambda { first (), second () }</c>. Anything written before the <c>let!</c> runs
    /// immediately, not as part of the effect.
    /// </remarks>
    /// <example><code>
    /// effect {
    ///     let! result = Data.Onboarding.accessor
    ///     match result with
    ///     | Some result -> setNavigation (navigation result.config.IsOk)
    ///     | _ -> ()
    /// }
    /// </code></example>
    [<Erase>]
    let effect = CreateEffectBuilder ()

    /// <summary>
    /// Wraps the computation in <c>onSettled(fun () -> ...)</c>
    /// </summary>
    /// <example><code>
    /// mount {
    ///     configStore.Update Data.Config.data
    /// }
    /// </code></example>
    [<Erase>]
    let mount = OnSettledBuilder ()

    /// <summary>
    /// Wraps the computation in <c>onCleanup(fun () -> ...)</c>
    /// </summary>
    /// <example><code>
    /// cleanup {
    ///     configStore.Update Data.Config.data
    /// }
    /// </code></example>
    [<Erase>]
    let cleanup = OnCleanupBuilder ()

    /// <summary>
    /// Wraps the computation in <c>createMemo(fun _ -> ...)</c>
    /// </summary>
    /// <example><code>
    /// memo {
    ///     // code here that returns
    ///     // some expensive value
    /// }
    /// </code></example>
    [<Erase>]
    let memo = CreateMemoBuilder ()

    /// <summary>
    /// Wraps the computation in <c>lazy(fun () -> ...)</c>
    /// </summary>
    /// <example><code>
    /// let comp = lazyload {
    ///     importDynamic "./MyComponent.fs.jsx"
    /// }
    /// </code></example>
    [<Erase>]
    let lazyload = LazyBuilder ()

    /// <summary>
    /// Wraps the computation in <c>children(fun () -> ...)</c>
    /// </summary>
    /// <example>
    /// A common pattern to optimise conditional child expressions:
    /// <code>
    /// let resolvedChildren = children { props.children }
    /// let hasChildren = lambda { resolvedChildren.toArray().Length > 0 }
    /// // ...
    /// if hasChildren() then resolvedChildren.Invoke()
    /// </code></example>
    [<Erase>]
    let children = ChildrenBuilder ()

    /// <summary>
    /// Wraps the computation in <c>createReaction(fun () -> ...)</c>
    /// </summary>
    /// <example><code>
    /// let isOpen,setOpen = createSignal false
    /// let onFirstOpen = reaction {
    ///   // ... do something the first time
    ///   // ... isOpen changes
    /// }
    /// onFirstOpen (fun () -> isOpen())
    /// // This will run the first time `isOpen` changes
    /// </code></example>
    [<Erase>]
    let reaction = CreateReactionBuilder ()
