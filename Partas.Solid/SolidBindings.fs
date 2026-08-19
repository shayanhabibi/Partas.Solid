namespace Partas.Solid

open System.Runtime.CompilerServices
open Fable.Core
open System
open Fable.Core.JsInterop

#nowarn 49
#nowarn 1182

/// <summary>A function that compares two values for equality. Failing this, reactive effects downstream are pushed with the next value.</summary>
/// <param name="prev">Previous value</param>
/// <param name="next">Next value</param>
type EqualityFunc<'T> = delegate of prev: 'T * next: 'T -> bool
/// <summary>Alias for a unit call signature which disposes of resources when run.</summary>
type DisposalFunc = unit -> unit
/// <summary>Alias for a unit call signature which resets some state run.</summary>
type ResetFunc = unit -> unit

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

    [<Erase>]
    type Store<'T> = private Store of 'T with
        [<Emit "$0">]
        member inline this.Value = unbox<'T> this
        static member inline op_Implicit(store: Store<'T>): 'T = store.Value

    [<Erase>]
    type StoreSetter<'T> = 'T -> 'T
    type StoreReturn<'T> = Store<'T> * StoreSetter<'T>

    [<Erase>]
    type Refreshable<'T> = private Refreshable of 'T with
        [<Emit "$0">]
        member inline this.Value = unbox<'T> this
        static member inline op_Implicit(refreshable: Refreshable<'T>): 'T = refreshable.Value
        static member inline op_Implicit(refreshable: Refreshable<Store<'T>>): 'T = refreshable.Value.Value

    [<Erase>]
    type RefreshableStore<'T> = private RefreshableStore of 'T with
        [<Emit "$0">]
        member inline this.Value = unbox<'T> this
        [<Emit "$0">]
        member inline this.AsStore: Store<'T> = unbox this
        [<Emit "$0">]
        member inline this.AsRefreshable: Refreshable<'T> = unbox this
        static member inline op_Implicit(refreshable: RefreshableStore<'T>): Store<'T> = refreshable.AsStore
        static member inline op_Implicit(refreshable: RefreshableStore<'T>): 'T = refreshable.Value
        static member inline op_Implicit(refreshable: RefreshableStore<'T>): Refreshable<'T> = refreshable.AsRefreshable

    type RefreshableStoreReturn<'T> = RefreshableStore<'T> * StoreSetter<'T>

    type ContextProvider =
        inherit HtmlContainer

    /// <summary>
    /// Created by passing a <c>JS.Pojo</c> (recommended) to a createContext call as a type arg or value. Defaults
    /// are set by calling the constructor with any default values wanted.<br/><br/>
    /// The plugin transpiles the identifier with a <c>.Provider</c> suffix in the tag as per the SolidJS documentation.
    /// </summary>
    type Context<'T> = 'T -> ContextProvider
    type ContextNotFoundError() = inherit exn()


    module ErrorBoundary =
        type Fallback = delegate of err: Accessor<obj> * reset: (unit -> unit) -> HtmlElement

    [<Import("Errored", "solid-js")>]
    [<Erase>]
    type Errored() =
        interface HtmlContainer

        [<Erase; DefaultValue>]
        val mutable fallback: U2<HtmlElement, ErrorBoundary.Fallback>

        [<Erase>]
        member inline this.fallbackEle
            with inline set (value: HtmlElement) = this.fallback <- !^value
        [<Erase>]
        member inline this.fallbackFn
            with inline set (value: ErrorBoundary.Fallback) = this.fallback <- !^value
    module For =
        [<Import("For", "solid-js")>]
        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        type For() =
            interface HtmlElement
            interface ChildLambdaProvider2<U2<obj, Accessor<obj>>, U2<int, Accessor<int>>>
            [<Erase; DefaultValue>]
            val mutable keyed: U2<bool, obj -> objnull>

            [<Erase; DefaultValue>]
            val mutable each: obj[]

            /// Fallback element to render while the list is loading.
            [<DefaultValue; Erase>]
            val mutable fallback: HtmlElement

        [<Import("For", "solid-js")>]
        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        type For<'T>() =
            interface HtmlElement
            interface ChildLambdaProvider2<U2<'T, Accessor<'T>>, U2<int, Accessor<int>>>
            [<Erase; DefaultValue>]
            val mutable keyed: U2<bool, 'T -> objnull>

            [<Erase; DefaultValue>]
            val mutable each: 'T[]

            /// Fallback element to render while the list is loading.
            [<DefaultValue; Erase>]
            val mutable fallback: HtmlElement

        [<Erase; CompiledName("KeyedFor")>]
        type Keyed<'T>() =
            interface HtmlElement
            interface ChildLambdaProvider2<'T, Accessor<int>>
            [<Erase; DefaultValue>]
            val mutable each: 'T[]
            /// Fallback element to render while the list is loading.
            [<DefaultValue; Erase>]
            val mutable fallback: HtmlElement
            [<SolidTypeComponent(ComponentFlag.SkipOmit ||| ComponentFlag.SpreadProps); EditorBrowsable(EditorBrowsableState.Never)>]
            member props.comp = For(keyed = !^true).spread(props)

        type Component<'T> = Keyed<'T>

        [<Erase; CompiledName("NonKeyedFor")>]
        type NonKeyed<'T>() =
            interface HtmlElement
            interface ChildLambdaProvider2<Accessor<'T>, int>
            [<Erase; DefaultValue>]
            val mutable each: 'T[]
            /// Fallback element to render while the list is loading.
            [<DefaultValue; Erase>]
            val mutable fallback: HtmlElement
            [<SolidTypeComponent(ComponentFlag.SkipOmit ||| ComponentFlag.SpreadProps)>]
            member props.comp = For(keyed = !^false).spread(props)

        [<Erase; CompiledName("KeyedFnFor")>]
        type KeyedFn<'T>() =
            interface HtmlElement
            interface ChildLambdaProvider2<Accessor<'T>, Accessor<int>>
            [<Erase; DefaultValue>]
            val mutable keyed: 'T -> obj
            [<Erase; DefaultValue>]
            val mutable each: 'T[]
            /// Fallback element to render while the list is loading.
            [<DefaultValue; Erase>]
            val mutable fallback: HtmlElement
            [<SolidTypeComponent(ComponentFlag.SkipOmit ||| ComponentFlag.SpreadProps)>]
            member props.comp = For().spread(props)

    [<Import("Loading", "solid-js")>]
    [<Erase>]
    type Loading() =
        interface HtmlContainer
        [<Erase; DefaultValue>]
        val mutable fallback: HtmlElement
        [<Erase; DefaultValue>]
        val mutable on: objnull

    [<Import("Repeat", "solid-js")>]
    [<Erase>]
    type Repeat<'T when 'T :> HtmlElement>() =
        interface HtmlElement
        interface FlowContainer<'T>
        interface ChildLambdaProviderStrict<int, 'T>
        [<Erase; DefaultValue>] val mutable count: int
        [<Erase; DefaultValue>] val mutable from: int
        [<Erase; DefaultValue>] val mutable fallback: HtmlElement

    module Reveal =
        [<StringEnum; RequireQualifiedAccess>]
        type Order =
            | Sequential
            | Together
            | Natural

    [<Import("Reveal", "solid-js")>]
    type Reveal() =
        interface HtmlContainer
        [<Erase; DefaultValue>] val mutable order: Reveal.Order
        [<Erase; DefaultValue>] val mutable collapsed: bool

    [<Import("Show", "solid-js")>]
    type Show() =
        interface HtmlContainer

        [<Erase; DefaultValue>]
        val mutable when': bool

        [<Erase; DefaultValue>]
        val mutable fallback: HtmlElement

        [<Erase; DefaultValue>]
        val mutable keyed: bool

    /// <summary>
    /// See Show.Keyed and Show.NonKeyed for stronger typed versions
    /// </summary>
    [<Import("Show", "solid-js")>]
    [<Erase>]
    type Show<'T>() =
        interface HtmlElement
        interface ChildLambdaProvider<U2<'T, Accessor<'T>>>

        [<Erase; DefaultValue>]
        val mutable when': 'T

        [<Erase; DefaultValue>]
        val mutable fallback: HtmlElement

        /// <summary>
        /// When keyed is false, the child is wrapped in an accessor
        /// </summary>
        [<Erase; DefaultValue>]
        val mutable keyed: bool

    module Show =
        [<Erase>]
        type Keyed<'T>() =
            interface HtmlElement
            interface ChildLambdaProvider<'T>
            [<Erase; DefaultValue>] val mutable when': 'T
            [<Erase; DefaultValue>] val mutable fallback: HtmlElement
            [<SolidTypeComponent(ComponentFlag.SkipOmit ||| ComponentFlag.SpreadProps); EditorBrowsable(EditorBrowsableState.Never);>]
            member props.comp = Show<'T>(keyed = true).spread(props)
        [<Erase>]
        type NonKeyed<'T>() =
            interface HtmlElement
            interface ChildLambdaProvider<Accessor<'T>>
            [<Erase; DefaultValue>] val mutable when': 'T
            [<Erase; DefaultValue>] val mutable fallback: HtmlElement
            [<SolidTypeComponent(ComponentFlag.SkipOmit ||| ComponentFlag.SpreadProps); EditorBrowsable(EditorBrowsableState.Never)>]
            member props.comp = Show<'T>(keyed = false).spread(props)

    [<AllowNullLiteral; EditorBrowsable(EditorBrowsableState.Never)>]
    type IMatch = inherit HtmlElement


    [<Import("Match", "solid-js")>]
    [<Erase>]
    type Match() =
        interface IMatch
        interface HtmlContainer

        [<Erase; DefaultValue>]
        val mutable when': bool
    [<Import("Match", "solid-js")>]
    [<Erase>]
    type Match<'T>() =
        interface IMatch
        interface HtmlContainer
        interface ChildLambdaProvider<U2<'T, Accessor<'T>>>
        [<Erase; DefaultValue>]
        val mutable when': 'T
        [<Erase>]
        member inline this.when'option with set(value: 'T option) = this.when' <- unbox value
        [<Erase; DefaultValue>]
        val mutable keyed: bool

    module Match =
        [<CompiledName("KeyedMatch"); Erase>]
        type Keyed<'T>() =
            interface IMatch
            interface ChildLambdaProvider<'T>
            [<Erase; DefaultValue>] val mutable when': 'T
            [<Erase>]
            member inline this.when'option with set(value: 'T option) = this.when' <- unbox value
            [<SolidTypeComponent(ComponentFlag.SkipOmit ||| ComponentFlag.SpreadProps); EditorBrowsable(EditorBrowsableState.Never)>]
            member props.comp = Match<'T>(keyed = true).spread(props)
        [<CompiledName("NonKeyedMatch"); Erase>]
        type NonKeyed<'T>() =
            interface IMatch
            interface ChildLambdaProvider<Accessor<'T>>
            [<Erase; DefaultValue>] val mutable when': 'T
            [<Erase>]
            member inline this.when'option with set(value: 'T option) = this.when' <- unbox value
            [<SolidTypeComponent(ComponentFlag.SkipOmit ||| ComponentFlag.SpreadProps); EditorBrowsable(EditorBrowsableState.Never)>]
            member props.comp = Match<'T>(keyed = false).spread(props)


    [<PartasImport("Switch", "solid-js")>]
    [<Erase>]
    type Switch() =
        interface HtmlElement
        interface FlowContainer<IMatch>
        [<Erase; DefaultValue>]
        val mutable fallback: HtmlElement

    [<Erase>]
    type Extensions =
        /// <summary>
        /// Replace a signals value. This is synonymous with using the Setters as normal.
        /// </summary>
        /// <param name="setter">The signal setter</param>
        /// <param name="value">The next value</param>
        [<Extension; Erase>]
        static member inline Invoke(setter: Setter<'T>, value: 'T) : unit =
            setter value

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
            setter value
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
    type LazyComponent<'T when 'T :> HtmlElement> =
        inherit HtmlElement
        abstract preload: unit -> JS.Promise<'T>
        abstract moduleUrl: string option

    type LazyComponent<'T when 'T :> HtmlElement> with
        static member inline op_Implicit(lazyComponent: LazyComponent<'T>): 'T = unbox lazyComponent
        member inline this.Value: 'T = unbox this

    [<Import("Hydration", "solid-js")>]
    [<Erase>]
    type Hydration() =
        interface HtmlContainer
        [<Erase; DefaultValue>]
        val mutable id: string

    [<Import("NoHydration", "solid-js")>]
    [<Erase>]
    type NoHydration() =
        interface HtmlContainer

    type Owner =
        abstract id: string option
        abstract _parent: Owner option
        abstract _childCount: int
        abstract _firstChild: Owner option
        abstract _nextSibling: Owner option
        abstract _prevSibling: Owner option

    type Root =
        inherit Owner
        abstract dispose: bool option -> unit

    type Root with
        static member inline FromOwner(owner: Owner) =
            if owner?_root
            then owner :?> Root |> Some
            else None
    type Owner with
        member inline this.ToRoot() = Root.FromOwner this

[<JS.Pojo; System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
type EffectOptions(
    ?defer: bool,
    ?schedule: bool,
    ?sync: bool,
    ?transparent: bool
    ) =
    [<Erase>] member val defer = defer with get,set
    [<Erase>] member val schedule = schedule with get,set
    [<Erase>] member val sync = sync with get,set
    [<Erase>] member val transparent = transparent with get,set

[<JS.Pojo; System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
type MemoOptions<'T>(
    ?id: string,
    ?name: string,
    ?transparent: bool,
    ?equals: EqualityFunc<'T>,
    ?unobserved: unit -> unit,
    ?``lazy``: bool,
    ?sync: bool,
    ?loadingValue: 'T
    ) =
    [<Erase>] member val id = id with get,set
    [<Erase>] member val name = name with get,set
    [<Erase>] member val transparent = transparent with get,set
    [<Erase>] member val equals = equals with get,set
    [<Erase>] member val unobserved = unobserved with get,set
    [<Erase>] member val ``lazy`` = ``lazy`` with get,set
    [<Erase>] member val sync = sync with get,set
    [<Erase>] member val loadingValue = loadingValue with get,set

[<JS.Pojo; System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
type SignalOptions<'T>(
    ?name: string,
    ?equals: EqualityFunc<'T>,
    ?ownedWrite: bool,
    ?unobserved: unit -> unit
    ) =
    [<Erase>] member val name = name with get,set
    [<Erase>] member val equals = equals with get,set
    [<Erase>] member val ownedWrite = ownedWrite with get,set
    [<Erase>] member val unobserved = unobserved with get,set

[<JS.Pojo; System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
type MixedSignalMemoOptions<'T>(
    ?id: string,
    ?name: string,
    ?transparent: bool,
    ?equals: EqualityFunc<'T>,
    ?unobserved: unit -> unit,
    ?``lazy``: bool,
    ?sync: bool,
    ?loadingValue: 'T,
    ?ownedWrite: bool
    ) =
    [<Erase>] member val id = id with get,set
    [<Erase>] member val name = name with get,set
    [<Erase>] member val transparent = transparent with get,set
    [<Erase>] member val equals = equals with get,set
    [<Erase>] member val unobserved = unobserved with get,set
    [<Erase>] member val ``lazy`` = ``lazy`` with get,set
    [<Erase>] member val sync = sync with get,set
    [<Erase>] member val loadingValue = loadingValue with get,set
    [<Erase>] member val ownedWrite = ownedWrite with get,set


[<JS.Pojo; System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
type StoreOptions(?name: string) =
    [<Erase>] member val name = name with get,set

[<JS.Pojo; System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
type ProjectionOptions<'T>(
    ?name: string,
    ?key: U3<string, 'T -> obj, unit>,
    ?shallow: bool,
    ?seedLoadingValue: bool
    ) =
    [<Erase>] member val name = name with get,set
    [<Erase>] member val key = key with get,set
    [<Erase>] member val shallow = shallow with get,set
    [<Erase>] member val seedLoadingValue = seedLoadingValue with get,set

type ChildrenReturn<'T when 'T :> HtmlElement> =
    [<Emit("$0()")>]
    abstract Invoke: unit -> 'T when 'T :> HtmlElement
    [<Emit("$0.toArray()")>]
    abstract toArray: unit -> 'T[] when 'T :> HtmlElement



type ExternalSource =
    abstract track: (obj -> obj) with get,set
    abstract dispose: (unit -> unit) with get,set

type ExternalSourceFactory = delegate of fn: (obj -> obj) * trigger: (unit -> unit) -> ExternalSource

type ExternalSourceConfig =
    abstract factory: ExternalSourceFactory with get,set
    abstract untrack: ((unit -> obj) -> obj) with get,set

module Dev =
    type OnStoreNodeUpdateDelegate = delegate of state: obj * property: string * value: obj * prev: obj -> unit

    [<JS.Pojo>]
    type DevHooks(
        ?onOwner: Owner -> unit,
        ?onGraph: objnull * Owner option -> unit,
        ?onUpdate: unit -> unit,
        ?onStoreNodeUpdate: OnStoreNodeUpdateDelegate
        ) =
        [<Erase>] member val onOwner = onOwner with get,set
        [<Erase>] member val onGraph = onGraph with get,set
        [<Erase>] member val onUpdate = onUpdate with get,set
        [<Erase>] member val onStoreNodeUpdate = onStoreNodeUpdate with get,set

    [<StringEnum(CaseRules.SnakeCaseAllCaps); RequireQualifiedAccess>]
    type DiagnosticCode =
        | StrictReadUntracked
        | PendingAsyncUntrackedRead
        | PendingAsyncForbiddenScope
        | ReactiveWriteInOwnedScope
        | ActionCalledInOwnedScope
        | RunWithDisposedOwner
        | NoOwnerCleanup
        | CleanupInForbiddenScope
        | SettledCleanupUnowned
        | PrimitiveInForbiddenScope
        | NoOwnerEffect
        | NoOwnerBoundary
        | AsyncOutsideLoadingBoundary
        | InvalidRefreshTarget
        | InvalidAffectsTarget
        | MissingEffectFn
        | SyncNodeReceivedAsync
        | ReactivityHalted
        | InvariantViolation
    [<StringEnum(CaseRules.KebabCase); RequireQualifiedAccess>]
    type DiagnosticKind =
        | StrictRead
        | Async
        | Write
        | Lifecycle
        | Owner
        | Error

    [<StringEnum>]
    type DiagnosticSeverity =
        | Warn
        | Error

    type DiagnosticEvent =
        abstract sequence: int
        abstract code: DiagnosticCode
        abstract kind: DiagnosticKind
        abstract severity: DiagnosticSeverity
        abstract message: string
        abstract ownerId: string option
        abstract ownerName: string option
        abstract nodeName: string option
        abstract data: Map<string, obj> option

    type DiagnosticCapture =
        abstract events: DiagnosticEvent[]
        abstract clear: unit -> unit
        abstract stop: unit -> DiagnosticEvent[]

type Dev =
    abstract hooks: Dev.DevHooks with get,set

[<AutoOpen>]
[<Erase>]
type Bindings =
    (*
    Create effect can return a cleanup function in the effect fn.
    The compute function receives the previous value of the effect.
    When the create effect returns a cleanup function, the error branch will receive this cleanup function.
    *)
    [<ImportMember("solid-js"); ParamObject(2)>]
    static member createEffect<'T>(compute: 'T option -> 'T, effectFn: 'T -> unit, ?defer: bool, ?schedule: bool, ?sync: bool, ?transparent: bool): unit = jsNative
    [<ImportMember("solid-js")>]
    static member createEffect<'T>(compute: 'T option -> 'T, effectFn: 'T -> unit): unit = jsNative
    [<ImportMember("solid-js")>]
    static member createEffect<'T>(compute: 'T option -> 'T, effectFn: 'T -> unit, options: EffectOptions): unit = jsNative
    [<ImportMember("solid-js"); ParamObject(2)>]
    static member createEffect<'T>(compute: 'T option -> 'T, effectFn: 'T -> DisposalFunc, ?defer: bool, ?schedule: bool, ?sync: bool, ?transparent: bool): unit = jsNative
    [<ImportMember("solid-js")>]
    static member createEffect<'T>(compute: 'T option -> 'T, effectFn: 'T -> DisposalFunc, options: EffectOptions): unit = jsNative
    [<ImportMember("solid-js"); ParamObject(1)>]
    static member createEffect<'T>(compute: 'T option -> 'T, effect: 'T -> unit, error: obj -> unit): unit = jsNative

    [<ImportMember("solid-js")>]
    static member createEffect<'T>(compute: 'T option -> 'T, effectFn: 'T -> DisposalFunc): unit = jsNative
    [<ImportMember("solid-js"); ParamObject(1)>]
    static member createEffect<'T>(compute: 'T option -> 'T, effect: 'T -> DisposalFunc, error: obj * ResetFunc -> unit): unit = jsNative

    (*
    Create Memo
    *)
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member createMemo<'T>(compute: 'T -> 'T, loadingValue: 'T): Accessor<'T> = jsNative
    [<ImportMember "solid-js">]
    static member createMemo<'T>(compute: 'T option -> 'T, options: MemoOptions<'T>): Accessor<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member createMemo<'T>(compute: 'T option -> 'T, ?name: string, ?transparent: bool, ?equals: EqualityFunc<'T>, ?unobserved: unit -> unit, ?``lazy``: bool, ?sync: bool, ?loadingValue: 'T): Accessor<'T> = jsNative

    (*
    Create Optimistic
    *)
    [<ImportMember "solid-js">]
    static member createOptimistic<'T>(): Signal<'T option> = jsNative
    [<ImportMember "solid-js">]
    static member createOptimistic<'T>(value: 'T): Signal<'T> = jsNative
    [<ImportMember "solid-js">]
    static member createOptimistic<'T>(value: 'T, options: SignalOptions<'T>): Signal<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member createOptimistic<'T>(value: 'T, ?name: string, ?equals: EqualityFunc<'T>, ?ownedWrite: bool, ?unobserved: unit -> unit): Signal<'T> = jsNative
    // Creates a memo like optimistic signal
    [<ImportMember "solid-js">]
    static member createOptimistic<'T>(fn: unit -> 'T): Signal<'T> = jsNative
    [<ImportMember "solid-js">]
    static member createOptimistic<'T>(fn: unit -> 'T, options: MixedSignalMemoOptions<'T>): Signal<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member createOptimistic<'T>(fn: unit -> 'T, ?id: string, ?name: string, ?transparent: bool, ?equals: EqualityFunc<'T>, ?unobserved: unit -> unit, ?``lazy``: bool, ?sync: bool, ?loadingValue: 'T, ?ownedWrite: bool): Signal<'T> = jsNative

    (*
    Create signal
    *)
    [<ImportMember "solid-js">]
    static member createSignal<'T>(): Signal<'T option> = jsNative
    [<ImportMember "solid-js">]
    static member createSignal<'T>(value: 'T): Signal<'T> = jsNative
    [<ImportMember "solid-js">]
    static member createSignal<'T>(fn: unit -> 'T): Signal<'T> = jsNative
    [<ImportMember "solid-js">]
    static member createSignal<'T>(value: 'T, options: SignalOptions<'T>): Signal<'T> = jsNative
    [<ImportMember "solid-js">]
    static member createSignal<'T>(fn: unit -> 'T, options: MixedSignalMemoOptions<'T>): Signal<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member createSignal<'T>(value: 'T, ?name: string, ?equals: EqualityFunc<'T>, ?ownedWrite: bool, ?unobserved: unit -> unit): Signal<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member createSignal<'T>(fn: unit -> 'T, ?id: string, ?name: string, ?transparent: bool, ?equals: EqualityFunc<'T>, ?unobserved: unit -> unit, ?``lazy``: bool, ?sync: bool, ?loadingValue: 'T, ?ownedWrite: bool): Signal<'T> = jsNative
    [<ImportMember "solid-js">]
    static member flush(): unit = jsNative
    [<ImportMember "solid-js">]
    static member flush<'T>(fn: unit -> 'T): 'T = jsNative
    [<ImportMember "solid-js">]
    static member isPending(fn: unit -> objnull): bool = jsNative
    [<ImportMember "solid-js">]
    static member latest<'T>(fn: unit -> 'T): 'T = jsNative
    [<ImportMember "solid-js">]
    static member untrack<'T>(fn: unit -> 'T, ?strictReadLabel: string): 'T = jsNative
    [<ImportMember "solid-js">]
    static member inline createOptimisticStore<'T, 'I when 'T:(member id: 'I)>(store: 'T): StoreReturn<'T> = jsNative
    [<ImportMember "solid-js">]
    static member createOptimisticStore<'T>(store: 'T, options: ProjectionOptions<'T>): StoreReturn<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member inline createOptimisticStore<'T, 'I when 'T:(member id: 'I)>(store: 'T, ?name: string, ?shallow: bool, ?seedLoadingValue: bool): StoreReturn<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member inline createOptimisticStore<'T>(store: 'T, key: 'T -> objnull, ?name: string, ?shallow: bool, ?seedLoadingValue: bool): StoreReturn<'T> = jsNative
    [<ImportMember "solid-js">]
    static member createOptimisticStore<'T>(fn: 'T -> U3<'T, JS.Promise<'T>, JS.Promise<unit>>, store: 'T): RefreshableStoreReturn<'T> = jsNative
    [<ImportMember "solid-js">]
    static member createOptimisticStore<'T>(fn: 'T -> U3<'T, JS.Promise<'T>, JS.Promise<unit>>, store: 'T, options: ProjectionOptions<'T>): RefreshableStoreReturn<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(2)>]
    static member inline createOptimisticStore<'T, 'I when 'T:(member id: 'I)>(fn: 'T -> U3<'T, JS.Promise<'T>, JS.Promise<unit>>, store: 'T, ?name: string, ?shallow: bool, ?seedLoadingValue: bool): RefreshableStoreReturn<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(2)>]
    static member inline createOptimisticStore<'T>(fn: 'T -> U3<'T, JS.Promise<'T>, JS.Promise<unit>>, store: 'T, key: 'T -> objnull, ?name: string, ?shallow: bool, ?seedLoadingValue: bool): RefreshableStoreReturn<'T> = jsNative
    [<ImportMember "solid-js">]
    static member inline createProjection<'T, 'I when 'T:(member id: 'I)>(fn: 'T -> U2<'T option, JS.Promise<'T option>>, seed: 'T): RefreshableStoreReturn<'T> = jsNative
    [<ImportMember "solid-js">]
    static member inline createProjection<'T, 'I when 'T:(member id: 'I)>(fn: 'T -> U2<'T option, JS.Promise<'T option>>, seed: Store<'T>): RefreshableStoreReturn<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(2)>]
    static member inline createProjection<'T>(fn: 'T -> U2<'T option, JS.Promise<'T option>>, seed: 'T, key: 'T -> objnull): RefreshableStoreReturn<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(2)>]
    static member inline createProjection<'T>(fn: 'T -> U2<'T option, JS.Promise<'T option>>, seed: Store<'T>, key: 'T -> objnull): RefreshableStoreReturn<'T> = jsNative
    [<ImportMember "solid-js">]
    static member createProjection<'T>(fn: 'T -> U2<'T option, JS.Promise<'T option>>, seed: 'T, options: ProjectionOptions<'T>): RefreshableStoreReturn<'T> = jsNative
    [<ImportMember "solid-js">]
    static member createProjection<'T>(fn: 'T -> U2<'T option, JS.Promise<'T option>>, seed: Store<'T>, options: ProjectionOptions<'T>): RefreshableStoreReturn<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(2)>]
    static member inline createProjection<'T, 'I when 'T:(member id: 'I)>(fn: 'T -> U2<'T option, JS.Promise<'T option>>, seed: 'T, ?shallow: bool, ?seedLoadingValue: bool, ?name: string): RefreshableStoreReturn<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(2)>]
    static member inline createProjection<'T, 'I when 'T:(member id: 'I)>(fn: 'T -> U2<'T option, JS.Promise<'T option>>, seed: Store<'T>, ?shallow: bool, ?seedLoadingValue: bool, ?name: string): RefreshableStoreReturn<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(2)>]
    static member inline createProjection<'T>(fn: 'T -> U2<'T option, JS.Promise<'T option>>, seed: 'T, key: 'T -> objnull, ?shallow: bool, ?seedLoadingValue: bool, ?name: string): RefreshableStoreReturn<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(2)>]
    static member inline createProjection<'T>(fn: 'T -> U2<'T option, JS.Promise<'T option>>, seed: Store<'T>, key: 'T -> objnull, ?shallow: bool, ?seedLoadingValue: bool, ?name: string): RefreshableStoreReturn<'T> = jsNative

    [<ImportMember "solid-js">]
    static member inline createStore<'T>(store: 'T): StoreReturn<'T> = jsNative
    [<ImportMember "solid-js">]
    static member inline createStore<'T>(store: Store<'T>): StoreReturn<'T> = jsNative
    [<ImportMember "solid-js">]
    static member inline createStore<'T, 'I when 'T:(member id: 'I)>(fn: 'T -> U2<'T option, JS.Promise<'T option>>): RefreshableStoreReturn<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member inline createStore<'T>(fn: 'T -> U2<'T option, JS.Promise<'T option>>, key: 'T -> objnull): RefreshableStoreReturn<'T> = jsNative

    [<ImportMember "solid-js"; ParamObject(1)>]
    static member inline createStore<'T>(store: 'T, ?name: string, ?shallow: bool): StoreReturn<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member inline createStore<'T>(store: Store<'T>, ?name: string, ?shallow: bool): StoreReturn<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member inline createStore<'T, 'I when 'T:(member id: 'I)>(fn: 'T -> U2<'T option, JS.Promise<'T option>>, ?name: string, ?shallow: bool, ?seedLoadingValue: bool, ?key: 'T -> objnull): RefreshableStoreReturn<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member inline createStore<'T>(fn: 'T -> U2<'T option, JS.Promise<'T option>>, key: 'T -> objnull, ?name: string, ?shallow: bool, ?seedLoadingValue: bool): RefreshableStoreReturn<'T> = jsNative

    [<ImportMember "solid-js">]
    static member merge<'T>([<ParamArray>] sources: obj[]): 'T = jsNative
    [<ImportMember "solid-js">]
    static member omit<'T>(obj: 'T, [<ParamArray>] props: string[]): 'T = jsNative

    [<ImportMember "solid-js">]
    static member reconcile<'T>(value: 'T): StoreSetter<'T> = jsNative
    [<ImportMember "solid-js">]
    static member reconcile<'T>(value: 'T, key: 'T -> objnull): StoreSetter<'T> = jsNative

    [<ImportMember "solid-js">]
    static member action<'Args, 'R>(genFn: 'Args -> 'R): JS.Promise<'R> = jsNative
    [<ImportMember "solid-js">]
    static member action<'Args, 'R>(genFn: 'Args -> JS.Promise<'R>): JS.Promise<'R> = jsNative

    [<ImportMember "solid-js">]
    static member affects<'T>(target: Accessor<'T>): unit = jsNative
    [<ImportMember "solid-js">]
    static member affects<'T>(target: Store<'T>, ?key: string): unit = jsNative
    static member inline affects<'T>(target: Store<'T>, path: 'T -> objnull): unit = affects(target, Experimental.nameofLambda path)

    [<ImportMember "solid-js">]
    static member onSettled(callback: unit -> unit): unit = jsNative
    [<ImportMember "solid-js">]
    static member onSettled(callback: unit -> DisposalFunc): unit = jsNative

    [<ImportMember "solid-js">]
    static member refresh<'T>(target: Refreshable<'T>): unit = jsNative

    [<ImportMember "solid-js">]
    static member children(fn: Accessor<#HtmlElement>): ChildrenReturn<#HtmlElement> = jsNative

    // todo - need to change how context works; its a fn atm from solid 1.*
    [<ImportMember "solid-js">]
    static member createContext<'T>(): Context<'T> = jsNative
    [<ImportMember "solid-js">]
    static member createContext<'T>(defaultValue: 'T): Context<'T> = jsNative
    [<ImportMember "solid-js">]
    static member createContext<'T>(defaultValue: 'T, options: EffectOptions): Context<'T> = jsNative
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member createContext<'T>(defaultValue: 'T, ?defer: bool, ?schedule: bool, ?sync: bool, ?transparent: bool, ?name: string): Context<'T> = jsNative

    [<ImportMember "solid-js">]
    static member createUniqueId(): string = jsNative
    [<Import("lazy", "solid-js")>]
    static member lazy'<'T when 'T :> HtmlElement>(fn: unit -> JS.Promise<'T>, ?moduleUrl: string): LazyComponent<'T> = jsNative

    [<ImportMember "solid-js">]
    static member useContext<'T>(context: Context<'T>): 'T = jsNative
    static member inline tryUseContext<'T>(context: Context<'T>): Result<'T, ContextNotFoundError> =
        try useContext context |> Ok with e -> unbox<ContextNotFoundError> e |> Error
    // TODO - Switch & Match

    [<ImportMember "solid-js">]
    static member createRoot<'T>(init: unit -> 'T): 'T = jsNative
    [<ImportMember "solid-js">]
    static member createRoot<'T>(init: DisposalFunc -> 'T): 'T = jsNative
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member createRoot<'T>(init: unit -> 'T, ?id: string, ?transparent: bool): 'T = jsNative
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member createRoot<'T>(init: DisposalFunc -> 'T, ?id: string, ?transparent: bool): 'T = jsNative

    // todo - Owner type
    [<ImportMember "solid-js">]
    static member getObserver(): Owner option = jsNative
    [<ImportMember "solid-js">]
    static member getOwner(): Owner option = jsNative
    [<ImportMember "solid-js">]
    static member isDisposed(node: Owner): bool = jsNative

    [<ImportMember "solid-js">]
    static member runWithOwner<'T>(owner: Owner, fn: unit -> 'T): 'T = jsNative
    [<ImportMember "solid-js">]
    static member runWithOwner<'T>(owner: Owner option, fn: unit -> 'T): 'T = jsNative

    [<ImportMember "solid-js">]
    static member createReaction(effectFn: unit -> unit): (unit -> objnull) -> unit = jsNative
    [<ImportMember "solid-js">]
    static member createReaction(effectFn: unit -> DisposalFunc): (unit -> objnull) -> unit = jsNative
    [<ImportMember "solid-js"; ParamObject(0)>]
    static member createReaction(effect: unit -> unit, error: Accessor<obj> -> unit): (unit -> objnull) -> unit = jsNative
    [<ImportMember "solid-js"; ParamObject(0)>]
    static member createReaction(effect: unit -> DisposalFunc, error: Accessor<obj> * DisposalFunc -> unit): (unit -> objnull) -> unit = jsNative

    [<ImportMember "solid-js">]
    static member createRenderEffect<'T>(compute: 'T option -> 'T, effectFn: 'T -> unit): unit = jsNative
    [<ImportMember "solid-js">]
    static member createRenderEffect<'T>(compute: 'T option -> 'T, effectFn: 'T -> DisposalFunc): unit = jsNative
    [<ImportMember "solid-js"; ParamObject(2)>]
    static member createRenderEffect<'T>(compute: 'T option -> 'T, effectFn: 'T -> unit, ?defer: bool, ?schedule: bool, ?sync: bool, ?transparent: bool): unit = jsNative
    [<ImportMember "solid-js"; ParamObject(2)>]
    static member createRenderEffect<'T>(compute: 'T option -> 'T, effectFn: 'T -> DisposalFunc, ?defer: bool, ?schedule: bool, ?sync: bool, ?transparent: bool): unit = jsNative
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member createRenderEffect<'T>(compute: 'T option -> 'T, effect: 'T -> unit, error: Accessor<obj> -> unit): unit = jsNative
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member createRenderEffect<'T>(compute: 'T option -> 'T, effect: 'T -> DisposalFunc, error: Accessor<obj> * DisposalFunc -> unit): unit = jsNative

    [<ImportMember "solid-js">]
    static member createTrackedEffect(compute: unit -> unit): unit = jsNative
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member createTrackedEffect(compute: unit -> unit, ?name: string): unit = jsNative
    [<ImportMember "solid-js">]
    static member createTrackedEffect(compute: unit -> DisposalFunc): unit = jsNative
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member createTrackedEffect(compute: unit -> DisposalFunc, ?name: string): unit = jsNative

    [<ImportMember "solid-js">]
    static member onCleanup(fn: unit -> unit): unit = jsNative

    [<ImportMember "solid-js">]
    static member deep<'T>(store: Store<'T>): 'T = jsNative
    // todo - isWrappable

    [<ImportMember "solid-js">]
    static member snapshot<'T>(item: Store<'T>): 'T = jsNative
    [<ImportMember "solid-js">]
    static member snapshot<'T>(item: Store<'T>, ?map: System.Collections.IDictionary, ?lookup: System.Collections.IDictionary): 'T = jsNative

    [<ImportMember "solid-js">]
    static member createErrorBoundary<'T, 'U>(fn: unit -> 'T, fallback: Accessor<objnull> * DisposalFunc -> 'U): Accessor<U2<'T, 'U>> = jsNative
    [<ImportMember "solid-js"; ParamObject(2)>]
    static member createLoadingBoundary<'T, 'U>(fn: unit -> 'T, fallback: unit -> 'U, ?on: unit -> objnull): Accessor<U2<'T, 'U>> = jsNative
    [<ImportMember "solid-js">]
    static member createRevealOrder<'T>(fn: unit -> 'T): 'T = jsNative
    [<ImportMember "solid-js"; ParamObject(1)>]
    static member createRevealOrder<'T>(fn: unit -> 'T, ?order: Accessor<Reveal.Order>, ?collapsed: Accessor<bool>): 'T = jsNative
    [<Import("mapArray","solid-js"); ParamObject(2)>]
    static member mapArray'<'Item, 'MappedItem>(list: Accessor<'Item array>, map: U3<'Item * Accessor<int>, Accessor<'Item> * int, Accessor<'Item> * Accessor<int>> -> 'MappedItem, ?keyed: U2<bool, 'Item -> objnull>, ?fallback: Accessor<objnull>, ?name: string): Accessor<'MappedItem[]> = jsNative
    [<ImportMember "solid-js"; ParamObject(2)>]
    static member mapArray<'Item, 'MappedItem>(list: Accessor<'Item array>, map: 'Item * Accessor<int> -> 'MappedItem, ?fallback: Accessor<objnull>, ?name: string): Accessor<'MappedItem[]> = jsNative
    static member inline mapArrayKeyed<'Item, 'MappedItem>(list: Accessor<'Item array>, map: 'Item * Accessor<int> -> 'MappedItem, ?fallback: Accessor<objnull>, ?name: string): Accessor<'MappedItem[]> =
        mapArray'(list, unbox map, ?fallback = fallback, ?name = name)
    static member inline mapArrayUnkeyed<'Item, 'MappedItem>(list: Accessor<'Item array>, map: Accessor<'Item> * int -> 'MappedItem, ?fallback: Accessor<objnull>, ?name: string): Accessor<'MappedItem[]> =
        mapArray'(list, unbox map, keyed = !^false, ?fallback = fallback, ?name = name)
    static member inline mapArrayKeyedFn<'Item, 'MappedItem>(list: Accessor<'Item array>, map: Accessor<'Item> * Accessor<int> -> 'MappedItem, keyed: 'Item -> objnull, ?fallback: Accessor<objnull>, ?name: string): Accessor<'MappedItem[]> =
        mapArray'(list, unbox map, keyed = !^keyed, ?fallback = fallback, ?name = name)
    static member inline mapArrayKeyed<'Item, 'MappedItem>(list: Accessor<'Item array>, map: 'Item * Accessor<int> -> 'MappedItem): Accessor<'MappedItem[]> =
        mapArray'(list, unbox map)
    static member inline mapArrayUnkeyed<'Item, 'MappedItem>(list: Accessor<'Item array>, map: Accessor<'Item> * int -> 'MappedItem): Accessor<'MappedItem[]> =
        mapArray'(list, unbox map, keyed = !^false)
    static member inline mapArrayKeyedFn<'Item, 'MappedItem>(list: Accessor<'Item array>, map: Accessor<'Item> * Accessor<int> -> 'MappedItem, keyed: 'Item -> objnull): Accessor<'MappedItem[]> =
        mapArray'(list, unbox map, keyed = !^keyed)
    [<ImportMember "solid-js"; ParamObject(2)>]
    static member repeat(count: Accessor<int>, map: int -> obj, ?from: Accessor<int option>, ?fallback: Accessor<objnull>, ?name: string): Accessor<obj[]> = jsNative
    [<ImportMember "solid-js"; ParamObject(2)>]
    static member repeat<'T>(count: Accessor<int>, map: int -> 'T, ?from: Accessor<int option>, ?fallback: Accessor<U3<objnull, 'T, 'T[]>>, ?name: string): Accessor<'T[]> = jsNative

    [<ImportMember "solid-js">]
    static member enableExternalSource(config: ExternalSourceConfig): unit = jsNative

    [<ImportMember "solid-js"; ParamObject(1)>]
    static member flatten(children: obj, ?skipNonRendered: bool, ?doNotUnwrap: bool): obj = jsNative
    [<ImportMember "solid-js">]
    static member resolve(fn: unit -> 'T): JS.Promise<'T> = jsNative
    [<ImportMember "solid-js">]
    static member DEV: Dev option = jsNative

    [<ImportMember "solid-js"; ParamObject(0)>]
    static member createOwner(?id: string, ?transparent: bool): Root = jsNative

    /// <summary>
    /// Invokes a component, wrapping the call in <c>untrack</c> so that reactive reads
    /// inside the component body don't subscribe the parent computation. Compiled JSX uses this
    /// internally; manual calls are rarely needed unless authoring a custom JSX factory or renderer.
    /// </summary>
    [<ImportMember "solid-js">]
    static member createComponent(fn: FSharpFunc<_, #HtmlElement>, props: obj): #HtmlElement = jsNative

    /// <summary>
    /// Homebrew helper that boiler plates the typical Solid directive pattern.
    /// <example><code lang="fsharp">
    /// let listen (typ: string) (listener: EventListener) (options: obj) =
    ///     createDirective &lt;| fun el ->
    ///         el.addEventListener(typ, listener, options)
    ///         fun () -> target.removeEventListener(typ, listener, options)
    /// // ...
    /// div().ref(listen "click" listener {||})
    /// </code></example>
    /// </summary>
    /// <remarks>
    /// This is a basic utility wrapper which sets up a mutable variable, which is handed to the directive
    /// function within <c>onSettled</c> (after pattern matching for nullability).
    /// We then return a ref callback which sets the element for the settled work, and creates a <c>Ref.Callback</c>.
    /// </remarks>
    /// <param name="directive">A lambda which performs the actions on the target element, and returns a disposal/cleanup function</param>
    static member inline createDirectiveFactory<^DomType when ^DomType :> Browser.Types.HTMLElement>([<InlineIfLambda>] directive: ^DomType -> DisposalFunc): Ref<^DomType> =
        let mutable el: ^DomType option = None
        onSettled(fun () ->
            match el with
            | None -> unbox<DisposalFunc> ()
            | Some target -> directive target)
        fun (element: ^DomType) -> el <- Some element
        |> Ref.Callback
