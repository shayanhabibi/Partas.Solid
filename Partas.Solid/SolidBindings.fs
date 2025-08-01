namespace Partas.Solid

open System.Runtime.CompilerServices
open Browser.Types
open Fable.Core
open System

#nowarn 49

[<AutoOpen>]
module Bindings =

    /// <summary>
    /// Calling the setter updates the Signal (triggering dependents to rerun) if the value actually changed.
    /// <br/>The setter takes either the new value for the signal or a function that maps the previous value of the signal to a new value as its only argument. The updated value is also returned by the setter.
    /// </summary>
    /// <remarks>
    /// To pass a handler that maps the previous value, call Invoke on the setter.
    /// <code>
    /// let index,setIndex = createSignal(0)
    /// setIndex.Invoke(fun x -> x + 1)
    /// </code>
    /// To access the returned value, use <c>.InvokeGet</c>
    /// </remarks>
    type Setter<'T> = 'T -> unit
    type Accessor<'T> = unit -> 'T
    type Signal<'T> = Accessor<'T> * Setter<'T>

    type ContextProvider =
        inherit HtmlContainer

    /// <summary>
    /// Created by passing a <c>JS.Pojo</c> (recommended) to a createContext call as a type arg or value. Defaults
    /// are set by calling the constructor with any default values wanted.<br/><br/>
    /// The plugin transpiles the identifier with a <c>.Provider</c> suffix in the tag as per the SolidJS documentation.
    /// </summary>
    type Context<'T> = 'T -> ContextProvider

    [<PartasImport("For", "solid-js")>]
    [<Erase>]
    type For<'T>() =
        interface HtmlElement

        [<Erase>]
        member this.each
            with set (value: 'T[]) = ()

        [<Erase>]
        member inline _.Zero() : HtmlContainerFun = ignore

        [<Erase>]
        member inline _.Yield(PARTAS_VALUE: 'T -> Accessor<int> -> #HtmlElement) : HtmlContainerFun =
            fun PARTAS_CONT -> ignore PARTAS_VALUE

    [<PartasImport("Index", "solid-js")>]
    [<Erase>]
    type Index<'T>() =
        interface HtmlElement

        [<Erase>]
        member this.each
            with set (value: 'T[]) = ()

        [<Erase>]
        member inline _.Zero() : HtmlContainerFun = ignore

        [<Erase>]
        member inline _.Yield(PARTAS_VALUE: Accessor<'T> -> int -> #HtmlElement) : HtmlContainerFun =
            fun PARTAS_CONT -> ignore PARTAS_VALUE

    [<PartasImport("Show", "solid-js")>]
    [<Erase>]
    type Show() =
        interface HtmlContainer

        [<Erase>]
        member this.when'
            with set (value: bool) = ()

        [<Erase>]
        member this.fallback
            with set (value: HtmlElement) = ()

        [<Erase>]
        member this.keyed
            with set (value: bool) = ()

    [<PartasImport("Match", "solid-js")>]
    [<Erase>]
    type Match() =
        interface HtmlContainer

        [<Erase>]
        member this.when'
            with set (value: bool) = ()

    [<PartasImport("Switch", "solid-js")>]
    [<Erase>]
    type Switch() =
        interface HtmlElement

        [<Erase>]
        member this.fallback
            with set (value: HtmlElement) = ()

        [<Erase>]
        member inline _.Combine
            ([<InlineIfLambda>] PARTAS_FIRST: HtmlContainerFun, [<InlineIfLambda>] PARTAS_SECOND: HtmlContainerFun)
            : HtmlContainerFun =
            fun PARTAS_BUILDER ->
                PARTAS_FIRST PARTAS_BUILDER
                PARTAS_SECOND PARTAS_BUILDER

        [<Erase>]
        member inline _.Delay([<InlineIfLambda>] PARTAS_DELAY: unit -> HtmlContainerFun) : HtmlContainerFun =
            PARTAS_DELAY ()

        [<Erase>]
        member inline _.Zero() : HtmlContainerFun = ignore

        [<Erase>]
        member inline _.Yield(PARTAS_ELEMENT: Match) : HtmlContainerFun =
            fun PARTAS_CONT -> ignore PARTAS_ELEMENT

    [<PartasImport("Suspense", "solid-js")>]
    [<Erase>]
    type Suspense() =
        interface HtmlContainer

        [<Erase>]
        member this.fallback
            with set (value: HtmlElement) = ()

    [<PartasImport("SuspenseList", "solid-js")>]
    [<Erase>]
    type SuspenseList() =
        interface HtmlContainer

        [<Erase>]
        member this.revealOrder
            with set (value: string) = ()

        [<Erase>]
        member this.tail
            with set (value: string) = ()

        [<Erase>]
        member this.fallback
            with set (value: HtmlElement) = ()

    [<PartasImport("Portal", "solid-js/web")>]
    [<Erase>]
    type Portal() =
        interface HtmlContainer

        [<Erase>]
        member this.mount
            with set (value: Element) = ()

        [<Erase>]
        member this.useShadow
            with set (value: bool) = ()

    module ErrorBoundary =
        type Fallback = delegate of err: obj * reset: (unit -> unit) -> HtmlElement

    [<PartasImport("ErrorBoundary", "solid-js")>]
    [<Erase>]
    type ErrorBoundary() =
        interface HtmlContainer

        [<Erase>]
        member this.fallback
            with set (value: ErrorBoundary.Fallback) = ()

    [<Erase>]
    type Extensions =

        [<Extension; Erase>]
        static member Run(PARTAS_THIS: For<'T>, PARTAS_RUNEXPR: HtmlContainerFun) =
            PARTAS_RUNEXPR Unchecked.defaultof<_>
            PARTAS_THIS

        [<Extension; Erase>]
        static member Run(PARTAS_THIS: Index<'T>, PARTAS_RUNEXPR: HtmlContainerFun) =
            PARTAS_RUNEXPR Unchecked.defaultof<_>
            PARTAS_THIS

        [<Extension; Erase>]
        static member Run(PARTAS_THIS: Switch, PARTAS_RUNEXPR: HtmlContainerFun) =
            PARTAS_RUNEXPR Unchecked.defaultof<_>
            PARTAS_THIS

        /// <summary>
        /// Replace a signals value. This is synonymous with using the Setters as normal.
        /// </summary>
        /// <param name="setter">The signal setter</param>
        /// <param name="value">The next value</param>
        [<Extension; Erase>]
        static member inline Invoke(setter: Setter<'T>, value: 'T) : unit =
            setter (value)
        // In the case of calling Invoke on a setter, we want the alternate behaviour to be suggested first.
        /// <summary>
        /// Modify a signal value by performing computation on its previous value
        /// </summary>
        /// <param name="setter">The signal setter</param>
        /// <param name="handler">The handler that takes the previous value and returns the next.</param>
        [<Extension; Erase>]
        static member inline Invoke(setter: Setter<'T>, handler: 'T -> 'T) : unit =
            setter (unbox<'T> handler)

        /// <summary>
        /// Modify a signal value by replacing it with a new value.
        /// </summary>
        /// <param name="setter">The signal setter</param>
        /// <param name="value">The new value</param>
        /// <returns>The new value</returns>
        [<Extension; Erase>]
        static member inline InvokeAndGet(setter: Setter<'T>, value: 'T) : 'T =
            setter (value)
            |> unbox<'T>

        /// <summary>
        /// Modify a signal value by performing computation its previous value.
        /// </summary>
        /// <param name="setter">The signal setter</param>
        /// <param name="handler">The handler that takes the previous value and returns the next.</param>
        /// <returns>The new value</returns>
        [<Extension; Erase>]
        static member inline InvokeAndGet(setter: Setter<'T>, handler: 'T -> 'T) : 'T =
            setter (unbox<'T> handler)
            |> unbox<'T>

    [<Interface; AllowNullLiteral>]
    type ResourceFetcherInfo<'T> =
        /// Previous value
        abstract value: 'T
        /// <summary>
        /// Is true when the <c>fetcher</c> was triggered using the refetch function.
        /// </summary>
        /// <remarks>
        /// If <c>refetch</c> is called with an argument, that argument is supplied instead.
        /// You can use the <c>_.refetchingWith</c> helper which takes a type argument and returns
        /// an erased union of the type and bool.
        /// </remarks>
        abstract refetching: bool


    /// <summary>
    /// Arguments that are available to consume within the handler. The first is the source signal
    /// if any was provided (else null), and the second is an object with two properties, the previous
    /// value via <c>_.value</c>, and whether the fetcher was initiated by a <c>refetch</c> via <c>_.refetching</c>.
    /// </summary>
    type ResourceFetcher<'U, 'T> = 'U -> ResourceFetcherInfo<'T> -> JS.Promise<'T>

    [<RequireQualifiedAccess; StringEnum>]
    type SolidResourceState =
        /// Hasn't started loading, no value yet
        | Unresolved
        /// It's loading, no value yet
        | Pending
        /// Finished loading, has value
        | Ready
        /// It's re-loading, `latest` has value
        | Refreshing
        /// Finished loading with an error, no value
        | Errored

    type SolidResource<'T> =
        /// Attention, will be undefined while loading
        [<Emit("$0()")>]
        abstract current: 'T

        abstract state: SolidResourceState
        abstract loading: bool
        abstract error: exn option
        /// Unlike `current`, it keeps the latest value while re-loading
        /// Attention, will be undefined until first value has been loaded
        abstract latest: 'T

    type SolidResourceManager<'T> =
        abstract mutate: 'T -> 'T
        abstract refetch: unit -> JS.Promise<'T>

    [<Erase; AutoOpen; Extension>]
    type ResourceExtensions =
        /// <summary>
        /// Alias for the second property of the second argument provided in a resource fetching function
        /// to be provided as an erased union of a provided type and bool instead of just bool.
        /// </summary>
        [<Extension; Erase>]
        static member inline refetchingWith<'T>(this: ResourceFetcherInfo<_>) : U2<'T, bool> =
            unbox<U2<'T, bool>> this.refetching

        [<Extension; Erase>]
        static member inline refetchingWith<'T, 'U>(this: ResourceFetcherInfo<'U>) : U2<'T, bool> =
            unbox<U2<'T, bool>> this.refetching

        [<Emit("$0.refetch($1)")>]
        [<Extension; Erase>]
        static member inline refetchWith<'U>(this: SolidResourceManager<obj>, input: 'U) : JS.Promise<obj> = jsNative

        [<Emit("$0.refetch($1)")>]
        [<Extension; Erase>]
        static member inline refetchWith<'U, 'T>(this: SolidResourceManager<'T>, input: 'U) : JS.Promise<'T> = jsNative

    type SolidStoreSetter<'T> =
        /// Replace old store value with new
        [<Emit("$0($1)")>]
        abstract Update: newValue: 'T -> unit

        /// Update store specifying updater function from old value to new value
        [<Emit("$0($1)")>]
        abstract Update: updater: ('T -> 'T) -> unit

        /// Update store using native solid path syntax
        [<Emit("$0(...$1)")>]
        abstract UpdatePath: pathArgs: obj[] -> unit

    type SolidStorePath<'T, 'Value>(setter: SolidStoreSetter<'T>, path: obj[]) =
        member _.Setter = setter
        member _.Path = path

        /// Choose the store item that should be updated
        member inline this.Map(map: 'Value -> 'Value2) =
            SolidStorePath<'T, 'Value2> (
                this.Setter,
                Experimental.namesofLambda map
                |> Array.map box
                |> Array.append this.Path
            )

        /// Update store item using new value
        member this.Update(value: 'Value) : unit =
            this.Setter.UpdatePath (Array.append this.Path [| value |])

        /// Update store item specifying updater function from old value to new value
        member this.Update(updater: 'Value -> 'Value) : unit =
            this.Setter.UpdatePath (Array.append this.Path [| updater |])

    [<AutoOpen>]
    module SolidExtensions =

        type SolidStoreSetter<'T> with
            /// Access more convenient way of updating store items
            member this.Path = SolidStorePath<'T, 'T> (this, [||])

    [<Extension; Erase>]
    type SolidStorePathExtensions =

        /// Select store item by index
        [<Extension; Erase>]
        static member inline Item(this: SolidStorePath<'T, 'Value array>, index: int) =
            SolidStorePath<'T, 'Value> (this.Setter, Array.append this.Path [| index |])

        /// Select store item by predicate
        [<Extension; Erase>]
        static member inline Find(this: SolidStorePath<'T, 'Value array>, predicate: 'Value -> bool) =
            SolidStorePath<'T, 'Value> (this.Setter, Array.append this.Path [| predicate |])

[<AutoOpen>]
[<Erase>]
type Bindings =
    /// Returns a memo evaluating to the resolved children which updates whenever the children change.
    [<ImportMember("solid-js")>]
    static member children(value: unit -> #HtmlElement) : unit -> #HtmlElement = jsNative

    [<ImportMember("solid-js")>]
    static member mergeProps([<ParamList>] values: 'T[]) : 'T = jsNative

    [<ImportMember("solid-js")>]
    static member splitProps(o: 'T, properties: string array, otherProperties: string array) : 'T * 'T = jsNative

    [<ImportMember("solid-js")>]
    static member splitProps(o: 'T, properties: string array) : 'T * 'T = jsNative

    [<ImportMember("solid-js/web")>]
    static member render(code: unit -> #HtmlElement, element: #Element) : unit = jsNative

    [<ImportMember("solid-js/web")>]
    static member renderToString(fn: unit -> #HtmlElement) : string = jsNative

    [<ImportMember("solid-js")>]
    static member createSignal(value: 'T) : Signal<'T> = jsNative

    [<ImportMember("solid-js"); ParamObject(1)>]
    static member createSignal(value: 'T, ?equals: ('T -> 'T -> bool), ?name: string, ?``internal``: bool) : Signal<'T> = jsNative

    [<ImportMember("solid-js")>]
    static member createMemo(value: unit -> 'T) : (unit -> 'T) = jsNative

    [<ImportMember("solid-js")>]
    static member createEffect(effect: unit -> unit) : unit = jsNative

    [<ImportMember("solid-js")>]
    static member createEffect(effect: 'T -> 'T, initialValue: 'T) : unit = jsNative

    [<ImportMember("solid-js")>]
    static member createContext<'T>(?value: 'T) : Context<'T> = jsNative

    [<ImportMember("solid-js")>]
    static member useContext(context: Context<'T>) : 'T = jsNative

    [<ImportMember("solid-js"); ParamObject(fromIndex = 2)>]
    static member createResource
        (
            source: unit -> 'U option,
            fetcher: ResourceFetcher<'U option, 'T>,
            ?initialValue: 'T,
            ?name: string,
            ?deferStream: bool,
            ?onHydrated: (unit -> unit),
            ?ssrLoadFrom: string,
            ?storage: Signal<'T>
        ) : SolidResource<'T> * SolidResourceManager<'T> =
        jsNative

    [<ImportMember("solid-js"); ParamObject(fromIndex = 1)>]
    static member createResource
        (
            fetcher: ResourceFetcher<unit, 'T>,
            ?initialValue: 'T,
            ?name: string,
            ?deferStream: bool,
            ?onHydrated: (unit -> unit),
            ?ssrLoadFrom: string,
            ?storage: Signal<'T>
        ) : SolidResource<'T> * SolidResourceManager<'T> =
        jsNative

    [<ImportMember("solid-js")>]
    static member createResource(fetcher: ResourceFetcher<unit, 'T>) : SolidResource<'T> * SolidResourceManager<'T> = jsNative

    [<ImportMember("solid-js")>]
    static member createResource(source: unit -> 'U option, fetcher: ResourceFetcher<'U option, 'T>) : SolidResource<'T> * SolidResourceManager<'T> =
        jsNative

    /// Fetcher will be called immediately
    [<ImportMember("solid-js"); ParamObject(fromIndex = 1)>]
    static member createResource(fetcher: unit -> JS.Promise<'T>, ?initialValue: 'T) : SolidResource<'T> * SolidResourceManager<'T> = jsNative

    /// Injects Async.StartAsPromise to the fetcher
    static member inline createResource(fetcher: unit -> Async<'T>, ?initialValue: 'T) : SolidResource<'T> * SolidResourceManager<'T> =
        createResource (
            fetcher
            >> Async.StartAsPromise,
            ?initialValue = initialValue
        )

    /// Fetcher will be called only when source signal returns `Some('U)`
    [<ImportMember("solid-js"); ParamObject(fromIndex = 2)>]
    static member createResource
        (source: unit -> 'U option, fetcher: 'U -> JS.Promise<'T>, ?initialValue: 'T)
        : SolidResource<'T> * SolidResourceManager<'T> =
        jsNative

    /// Injects Async.StartAsPromise to the fetcher
    static member inline createResource
        (source: unit -> 'U option, fetcher: 'U -> Async<'T>, ?initialValue: 'T)
        : SolidResource<'T> * SolidResourceManager<'T> =
        createResource (
            source,
            fetcher
            >> Async.StartAsPromise,
            ?initialValue = initialValue
        )


    [<ImportMember("solid-js")>]
    static member createRoot(fn (* dispose *) : Action -> 'T) : 'T = jsNative

    [<ImportMember("solid-js")>]
    static member createUniqueId() : string = jsNative

    [<ImportMember("solid-js/store")>]
    static member createStore(store: 'T) : 'T * SolidStoreSetter<'T> = jsNative

    [<ImportMember("solid-js/store")>]
    static member reconcile<'T, 'U>(value: 'T) : ('U -> 'T) = jsNative

    [<ImportMember("solid-js/store")>]
    static member produce<'T>(fn: 'T -> unit) : ('T -> 'T) = jsNative

    [<ImportMember("solid-js/store")>]
    static member unwrap<'T>(item: 'T) : 'T = jsNative

    [<ImportMember("solid-js")>]
    static member batch<'T>(fn: unit -> 'T) : 'T = jsNative

    [<ImportMember("solid-js")>]
    static member catchError<'T>(tryFn: unit -> 'T, onError: obj -> unit) : 'T = jsNative

    [<ImportMember("solid-js")>]
    static member onCleanup(fn: unit -> unit) : unit = jsNative

    [<ImportMember("solid-js")>]
    static member onMount(fn: unit -> unit) : unit = jsNative

    [<ImportMember("solid-js")>]
    static member useTransition() : (unit -> bool) * ((unit -> unit) -> JS.Promise<unit>) = jsNative

    [<ImportMember("solid-js")>]
    static member startTransition() : ((unit -> unit) -> JS.Promise<unit>) = jsNative

    [<ImportMember("solid-js")>]
    static member untrack<'T>(fn: Accessor<'T>) : 'T = jsNative

    /// Component should be decorated by `ExportDefaultAttribute`. Use in combination with `lazy'`.
    [<Emit("import($0)")>]
    static member importComponent(path: string) : JS.Promise<HtmlElement> = jsNative

    /// Component lazy loading. Use in combination with `importComponent`
    [<Import("lazy", "solid-js")>]
    static member lazy'(import: unit -> JS.Promise<HtmlElement>) : HtmlElement = jsNative

    /// <summary>
    /// <c>createComputed</c> creates a new computation that immediately runs the given function in a tracking,
    /// thus automatically tracking its dependencies, and automatically reruns the function whenever the dependencies
    /// changes. The function gets called with an argument equal to the value returned from the function's last
    /// execution, or on the first call, equal to the optional second argument. Note that the return value of the
    /// function is not otherwise exposed; in particular, createComputed has no return value.<br/><br/>
    /// <c>createComputed</c> is the most immediate form of reactivity in Solid, and is most useful for building
    /// other reactive primitives. For example, some other Solid primitives are built from <c>createComputed</c>.
    /// However, it should be used with care, as <c>createComputed</c> can easily cause more unnecessary updates
    /// than other reactive primitives. Before using it, consider the closely related primitives <c>createMemo</c>
    /// and <c>createRenderEffect</c>.
    /// </summary>
    /// <param name="fn">The function to run in a tracking scope.</param>
    /// <param name="value">The initial value to pass to the function.</param>
    [<ImportMember("solid-js")>]
    static member createComputed<'T>(fn: 'T -> 'T, ?value: 'T) : unit = jsNative

    /// <summary>
    /// Creates a readonly that only notifies downstream changes when the browser is idle. <c>timeoutMs</c> is the
    /// maximum time to wait before forcing the update.
    /// </summary>
    [<ImportMember("solid-js"); ParamObject(1)>]
    static member createDeferred<'T>(source: unit -> 'T, ?timeoutMs: int, ?equals: 'T -> 'T -> bool, ?name: string) : unit -> 'T = jsNative

    /// <summary>
    /// Sometimes it is useful to separate tracking from re-execution. This primitive registers a side-effect
    /// that is run the first time the expression wrapped by the returned tracking is notified of a change.
    /// </summary>
    [<ImportMember("solid-js")>]
    static member createReaction(onInvalidate: unit -> unit) : (unit -> unit) -> unit = jsNative

    /// <summary>
    /// A render effect is a computation similar to a regular effect, but differs in when Solid schedules
    /// the first execution of the effect function. While createEffect waits for the current rendering
    /// phase to be complete, createRenderEffect immediately calls the function. Thus the effect runs as
    /// DOM elements are being created and updated, but possibly before specific elements of interest have
    /// been created, and probably before those elements have been connected to the document. In particular, refs
    /// will not be set before the initial effect call. Indeed, Solid uses <c>createRenderEffect</c> to implement
    /// the rendering phase of itself, including setting of <b>refs</b>
    /// </summary>
    [<ImportMember("solid-js")>]
    static member createRenderEffect<'T>(fn: 'T -> 'T, ?value: 'T) : unit = jsNative

    /// <summary>
    /// Creates a parameterised derived boolean signal <c>selector(key)</c> that indicates whether <c>key</c>
    /// is equal to the current value of the <c>source</c> signal. These signals are optimised to notify
    /// each subscriber only when their <c>key</c> starts or stops matching the reactive <c>source</c> value
    /// (instead of every time <c>key</c> changes). If you have <i>n</i> different subscribers with different
    /// keys, and the <c>source</c> value changes from <c>a</c> to <c>b</c>, then instead of all <i>n</i> subscribers
    /// updating, at most two subscribers will update: the signal with key <c>a</c> will change to <c>false</c>, and
    /// the signal with key <c>b</c> will change to <c>true</c>. Thus it reduces from <i>n</i> updates to 2 updates.<br/>
    /// <br/>Useful for defining the selection state of several selectable elements.
    /// </summary>
    /// <param name="source">The source signal to get the value from and compare with keys.</param>
    /// <param name="fn">A function to compare the key and the value, returning whether they should be treated as equal. Default: <c>=</c></param>
    [<ImportMember("solid-js")>]
    static member createSelector<'T, 'U>(source: unit -> 'T, ?fn: 'U -> 'T -> bool) : 'U -> bool = jsNative

    /// <summary>
    /// Reactive map helper that caches each item by reference to avoid unnecessary recomputations.
    /// It only runs the mapping function once per value, and then moves or removes it as needed.
    /// The index argument is a signal. The map function itself is not tracking.
    /// <br/><br/>
    /// This is the underlying helper for the <c>For</c> component, which is used to render lists of items.
    /// </summary>
    [<ImportMember("solid-js")>]
    static member mapArray<'T, 'U>(source: Accessor<'T[]>, fn: 'T -> Accessor<int> -> 'U) : Accessor<'U[]> = jsNative

    /// <summary>
    /// Reactive helper that maps by index, similar to <c>mapArray</c>, but uses the index of the item in the array
    /// as the key for the mapping function. This is useful when you want to map over an array and use the index
    /// as a key for each item, such as when rendering a list of items with unique keys.
    /// <br/><br/>
    /// This is the underlying helper for the <c>Index</c> component, which is used to render lists of items
    /// </summary>
    [<ImportMember("solid-js")>]
    static member indexArray<'T, 'U>(source: Accessor<'T[]>, fn: Accessor<'T> -> int -> 'U) : Accessor<'U[]> = jsNative
