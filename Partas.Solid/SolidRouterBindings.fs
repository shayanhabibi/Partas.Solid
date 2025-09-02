namespace Partas.Solid.Router

open System.Runtime.CompilerServices
open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open System
open Partas.Solid

#nowarn 49
#nowarn 64

[<AutoOpen>]
module Bindings =

    [<RequireQualifiedAccess>]
    [<StringEnum>]
    type Intent =
        | Initial
        | Native
        | Navigate
        | Preload

    [<Pojo>]
    type NavigateOptions(?resolve: bool, ?replace: bool, ?scroll: bool, ?state: obj) =
        member val resolve: bool =
            resolve
            |> Option.defaultValue !!null with get, set

        member val replace: bool =
            replace
            |> Option.defaultValue !!null with get, set

        member val scroll: bool =
            scroll
            |> Option.defaultValue !!null with get, set

        member val state: obj =
            state
            |> Option.defaultValue !!null with get, set

    type Navigator =
        [<Emit("$0($1...)")>]
        abstract member Invoke: ``to``: string * ?options: NavigateOptions -> unit

        /// Replaces NavigateOptions
        [<Emit("$0($1...)"); ParamObject(1)>]
        abstract member InvokeOptions: ``to``: string * ?resolve: bool * ?replace: bool * ?scroll: bool * ?state: obj -> unit

        [<Emit("$0($1...)")>]
        abstract member Invoke: delta: float -> unit

    type Path =
        abstract member pathname: string with get, set
        abstract member search: string with get, set
        abstract member hash: string with get, set

    type Location =
        inherit Path
        abstract member query: obj with get, set
        abstract member state: obj option with get, set
        abstract member key: string with get, set

    type BeforeLeaveEventArgs =
        abstract member from: Location with get, set
        abstract member ``to``: U2<string, float> with get, set
        abstract member options: NavigateOptions option with get, set
        abstract member defaultPrevented: bool with get
        abstract member preventDefault: unit -> unit
        abstract member retry: ?force: bool -> unit

    type RoutePreloadFuncArgs =
        abstract member ``params``: obj with get, set
        abstract member location: Location with get, set
        abstract member intent: Intent with get, set

    type RoutePreloadFunc = RoutePreloadFuncArgs -> unit

    type PathMatch =
        abstract member ``params``: obj with get, set
        abstract member path: string with get, set

    type RouteDescription =
        abstract member key: obj with get, set
        abstract member originalPath: string with get, set
        abstract member pattern: string with get, set
        abstract member preload: RoutePreloadFunc option with get, set
        abstract member matcher: (string -> PathMatch option) with get, set
        abstract member matchFilters: obj option with get, set
        abstract member info: obj option with get, set

    type RouteMatch =
        inherit PathMatch
        abstract member route: RouteDescription with get, set

    [<PartasImport("Route", "@solidjs/router")>]
    type Route() =
        interface HtmlElement

        [<Erase; DefaultValue>]
        val mutable path: string

        [<Erase>]
        member this.paths
            with inline get (): string array = unbox this.path
            and inline set (value: string array) = this.path <- unbox<string> value

        [<Erase; DefaultValue>]
        val mutable component': TagValue

        [<Erase; DefaultValue>]
        val mutable matchFilters: obj

        [<Erase; DefaultValue>]
        val mutable preload: RoutePreloadFunc

        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Combine
            ([<InlineIfLambda>] PARTAS_FIRST: HtmlContainerFun, [<InlineIfLambda>] PARTAS_SECOND: HtmlContainerFun)
            : HtmlContainerFun =
            fun PARTAS_BUILDER ->
                PARTAS_FIRST PARTAS_BUILDER
                PARTAS_SECOND PARTAS_BUILDER

        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Delay([<InlineIfLambda>] PARTAS_DELAY: unit -> HtmlContainerFun) : HtmlContainerFun =
            PARTAS_DELAY ()

        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Zero() : HtmlContainerFun = ignore

        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Yield(PARTAS_ELEMENT: Route) : HtmlContainerFun =
            fun PARTAS_CONT -> ignore PARTAS_ELEMENT

    [<Pojo>]
    type RootConfig(path: string, ``component``: HtmlElement) =
        member val path: string = jsNative with get, set

        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member val ``component``: HtmlElement = jsNative with get, set

        /// <summary> Alias for <c>_.``component``</c></summary>
        member this.component'
            with inline set (value: HtmlElement) = this.``component`` <- value
            and inline get (): HtmlElement = this.``component``

    [<PartasImport("Router", "@solidjs/router")>] // Replaces Import as it doesn't impact the builder param names
    type Router() =
        interface HtmlElement

        [<Erase; DefaultValue>]
        val mutable root: TagValue

        [<Erase; DefaultValue>]
        val mutable base': string

        [<Erase; DefaultValue>]
        val mutable actionBase: string

        [<Erase; DefaultValue>]
        val mutable preload: bool

        [<Erase; DefaultValue>]
        val mutable explicitLinks: bool

        [<Erase; DefaultValue>]
        val mutable url: string

        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Combine
            ([<InlineIfLambda>] PARTAS_FIRST: HtmlContainerFun, [<InlineIfLambda>] PARTAS_SECOND: HtmlContainerFun)
            : HtmlContainerFun =
            fun PARTAS_BUILDER ->
                PARTAS_FIRST PARTAS_BUILDER
                PARTAS_SECOND PARTAS_BUILDER

        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Delay([<InlineIfLambda>] PARTAS_DELAY: unit -> HtmlContainerFun) : HtmlContainerFun =
            PARTAS_DELAY ()

        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Zero() : HtmlContainerFun = ignore

        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Yield(PARTAS_ELEMENT: Route) : HtmlContainerFun =
            fun PARTAS_CONT -> ignore PARTAS_ELEMENT

        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Yield(PARTAS_ELEMENT: RootConfig[]) : HtmlContainerFun =
            fun PARTAS_CONT -> ignore PARTAS_ELEMENT

    [<PartasImport("HashRouter", "@solidjs/router")>]
    type HashRouter() =
        inherit Router()

    [<PartasImport("MemoryRouter", "@solidjs/router")>]
    type MemoryRouter() =
        inherit Router()

        [<Erase; DefaultValue>]
        val mutable history: MemoryHistory

    and MemoryHistory private () =
        [<Erase>]
        member this.get() : string = jsNative

        [<Erase; ParamObject(0)>]
        member this.set(value: string, ?scroll: bool, ?replace: bool) : unit = jsNative

        [<Erase>]
        member this.back() = jsNative

        [<Erase>]
        member this.forward() = jsNative

        [<Erase>]
        member this.go(n: int) : unit = jsNative

        [<Erase>]
        member this.listen(listener: string -> unit) : unit -> unit = jsNative

    [<Pojo>]
    type PreloadData(preloadData: bool) =
        member val preloadData: bool = jsNative with get, set

    [<Erase>]
    type Extensions =
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        [<Extension; Erase>]
        static member Run(PARTAS_THIS: Router, PARTAS_RUN: HtmlContainerFun) =
            PARTAS_RUN Unchecked.defaultof<_>
            PARTAS_THIS

        [<EditorBrowsable(EditorBrowsableState.Never)>]
        [<Extension; Erase>]
        static member Run(PARTAS_THIS: Route, PARTAS_RUN: HtmlContainerFun) =
            PARTAS_RUN Unchecked.defaultof<_>
            PARTAS_THIS

    [<Import("A", "@solidjs/router")>]
    type A() =
        interface RegularNode

        [<Erase; DefaultValue>]
        val mutable href: string

        [<Erase; DefaultValue>]
        val mutable noScroll: bool

        [<Erase; DefaultValue>]
        val mutable replace: bool

        [<Erase; DefaultValue>]
        val mutable state: obj

        [<Erase; DefaultValue>]
        val mutable activeClass: string

        [<Erase; DefaultValue>]
        val mutable inactiveClass: string

        [<Erase; DefaultValue>]
        val mutable end': bool

    [<Import("Navigate", "@solidjs/router")>]
    type Navigate() =
        interface RegularNode

        [<Erase; DefaultValue>]
        val mutable href: string

        [<Erase; DefaultValue>]
        val mutable state: obj

    type SolidAction<'Input, 'Result> =
        abstract url: string with get, set
        abstract member with': 'Input -> string

    type Submission<'Input, 'Result> =
        abstract input: 'Input with get
        abstract result: 'Result option with get
        abstract error: obj with get
        abstract pending: bool with get
        abstract url: string with get
        abstract clear: (unit -> unit)
        abstract retry: (unit -> unit)

    type Extensions with
        [<Extension>]
        static member pending(this: Submission<'Input, unit>[]) : bool = undefined

    type AsyncAccessor<'T> =
        [<Emit "$0()">]
        abstract Invoke: unit -> 'T

        abstract latest: 'T

    type Query<'Input, 'Output> =
        [<Emit("$0($1)")>]
        abstract Invoke: 'Input -> 'Output

        [<Emit "$0.key">]
        abstract key: string

        [<Emit "$0.keyFor($1)">]
        abstract keyFor: 'Input -> string

    type Extensions with

        [<EditorBrowsable(EditorBrowsableState.Never)>]
        [<Extension>]
        [<Emit "$0.latest">]
        static member latest<'T>(this: unit -> 'T) : 'T = jsNative

        [<EditorBrowsable(EditorBrowsableState.Never)>]
        [<Extension>]
        [<Emit "$0.key">]
        static member key(this: FSharpFunc<_, _>) : string = jsNative

        [<EditorBrowsable(EditorBrowsableState.Never)>]
        [<Extension>]
        [<Emit "$0.keyFor($1)">]
        static member keyFor(this: FSharpFunc<_, _>, value: string) : string = jsNative

[<AutoOpen>]
[<Erase>]
type Bindings =
    /// Actions only work with POST requests.
    [<ImportMember("@solidjs/router"); ParamObject(1)>]
    static member action<'Input, 'Output>
        (handler: 'Input -> Promise<'Output>, ?name: string, ?onComplete: Submission<'Input, 'Output> -> unit)
        : SolidAction<'Input, 'Output> =
        jsNative

    /// This can avoid the use of formData etc. However, it requires client-side javascript
    /// and is not progressively enhanceable like forms are.
    [<ImportMember("@solidjs/router")>]
    static member useAction<'Input, 'Result>(action: SolidAction<'Input, 'Result>) : 'Input -> 'Result = jsNative

    [<ImportMember("@solidjs/router")>]
    static member useSubmission<'Input>(action: SolidAction<'Input, unit>, ?filter: 'Input -> bool) : Submission<'Input, unit> = jsNative

    [<ImportMember("@solidjs/router")>]
    static member useSubmissions<'Input>(action: SolidAction<'Input, unit>, ?filter: 'Input -> bool) : Submission<'Input, unit>[] = jsNative

    [<ImportMember("@solidjs/router")>]
    static member createAsync<'T>
        (
            fn: 'T option -> Promise<'T>,
            ?name: string,
            ?initialValue: 'T,
            ?deferStream: bool,
            ?onHydrated: unit -> unit,
            ?ssrLoadFrom: string,
            ?storage: unit -> Signal<'T>
        ) : Accessor<'T> =
        jsNative

    [<Import("createAsync", "@solidjs/router")>]
    static member createAsyncWithLatest<'T>
        (
            fn: 'T option -> Promise<'T>,
            ?name: string,
            ?initialValue: 'T,
            ?deferStream: bool,
            ?onHydrated: unit -> unit,
            ?ssrLoadFrom: string,
            ?storage: unit -> Signal<'T>
        ) : AsyncAccessor<'T> =
        jsNative

    [<ImportMember("@solidjs/router")>]
    static member createAsyncStore<'T>
        (
            fn: 'T option -> Promise<'T>,
            ?name: string,
            ?initialValue: 'T,
            ?deferStream: bool,
            ?reconcile: obj,
            ?onHydrated: unit -> unit,
            ?ssrLoadFrom: string
        ) : Accessor<'T> =
        jsNative

    [<Import("createAsyncStore", "@solidjs/router")>]
    static member createAsyncStoreWithLatest<'T>
        (
            fn: 'T option -> Promise<'T>,
            ?name: string,
            ?initialValue: 'T,
            ?deferStream: bool,
            ?reconcile: obj,
            ?onHydrated: unit -> unit,
            ?ssrLoadFrom: string
        ) : AsyncAccessor<'T> =
        jsNative

    [<ImportMember("@solidjs/router")>]
    static member query<'Input, 'Output>(fn: FSharpFunc<'Input, 'Output>, ?name: string) : FSharpFunc<'Input, 'Output> = jsNative

    [<Import("query", "@solidjs/router")>]
    static member query'<'Input, 'Output>(fn: FSharpFunc<'Input, 'Output>, ?name: string) : Query<'Input, 'Output> = jsNative

    [<ImportMember("@solidjs/router")>]
    static member revalidate(key: string, ?force: bool) : unit = jsNative

    [<ImportMember("@solidjs/router")>]
    static member useNavigate() : Navigator = jsNative

    [<ImportMember("@solidjs/router")>]
    static member useLocation() : Location = jsNative

    [<ImportMember("@solidjs/router")>]
    static member useIsRouting() : (unit -> bool) = jsNative

    [<ImportMember("@solidjs/router")>]
    static member useMatch(fn: unit -> string, ?matchFilters: obj) : (unit -> bool) = jsNative

    [<ImportMember("@solidjs/router")>]
    static member useParams() : obj = jsNative

    [<ImportMember("@solidjs/router")>]
    static member useBeforeLeave(listener: BeforeLeaveEventArgs -> unit) : unit = jsNative

    [<ImportMember("@solidjs/router")>]
    static member useCurrentMatches() : unit -> RouteMatch[] = jsNative

    [<ImportMember("@solidjs/router")>]
    static member usePreloadRoute() : (string -> PreloadData) -> unit = jsNative

    [<ImportMember("@solidjs/router")>]
    static member useSearchParams() : Signal<obj> = jsNative

    [<ImportMember("@solidjs/router")>]
    static member createMemoryHistory() : MemoryHistory = jsNative
// TODO preload, json, redirect and reload
