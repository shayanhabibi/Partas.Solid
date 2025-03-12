namespace Partas.Solid.Router

open System.Runtime.CompilerServices
open Fable.Core
open Fable.Core.JsInterop
open System
open Partas.Solid

#nowarn 49
#nowarn 64

[<AutoOpen>]
module Bindings =

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type Intent =
        | initial
        | native
        | navigate
        | preload

    type NavigateOptions =
        abstract member resolve: bool with get, set
        abstract member replace: bool with get, set
        abstract member scroll: bool with get, set
        abstract member state: obj with get, set

    type Navigator =
        [<Emit("$0($1...)")>]
        abstract member Invoke: ``to``: string * ?options: NavigateOptions -> unit
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

    // [<Import("Route", "@solidjs/router")>] <--- this is injected by the plugin
    type Route() =
        interface HtmlElement
        [<Erase>]
        member this.path
            with set (value: string) = ()
        [<Erase>]
        member this.component'
            with set (value: TagValue) = ()
        [<Erase>]
        member this.matchFilters
            with set (value: obj) = ()
        [<Erase>]
        member this.preload
            with set (value: RoutePreloadFunc) = ()
        [<Erase>]
        member inline _.Combine
            ([<InlineIfLambda>] PARTAS_FIRST: HtmlContainerFun, [<InlineIfLambda>] PARTAS_SECOND: HtmlContainerFun)
            : HtmlContainerFun =
            fun PARTAS_BUILDER ->
                PARTAS_FIRST PARTAS_BUILDER
                PARTAS_SECOND PARTAS_BUILDER
        [<Erase>]
        member inline _.Delay([<InlineIfLambda>] PARTAS_DELAY: unit -> HtmlContainerFun) : HtmlContainerFun = PARTAS_DELAY()
        [<Erase>]
        member inline _.Zero() : HtmlContainerFun = ignore
        [<Erase>]
        member inline _.Yield(PARTAS_ELEMENT: Route) : HtmlContainerFun = fun PARTAS_CONT -> ignore PARTAS_ELEMENT

    [<AllowNullLiteral>]
    [<Global>]
    type RootConfig [<ParamObject; Emit("$0")>] (path: string, ``component``: HtmlElement) =
        member val path: string = jsNative with get, set
        member val ``component``: HtmlElement = jsNative with get, set

    // [<Import("Router", "@solidjs/router")>] // <--- this is injected by the plugin
    type Router() =
        interface HtmlElement
        [<Erase>]
        member this.root
            with set (value: TagValue) = ()
        [<Erase>]
        member this.base'
            with set (value: string) = ()
        [<Erase>]
        member this.actionBase
            with set (value: string) = ()
        [<Erase>]
        member this.preload
            with set (value: bool) = ()
        [<Erase>]
        member this.explicitLinks
            with set (value: bool) = ()
        [<Erase>]
        member this.url
            with set (value: string) = ()
        [<Erase>]
        member inline _.Combine
            ([<InlineIfLambda>] PARTAS_FIRST: HtmlContainerFun, [<InlineIfLambda>] PARTAS_SECOND: HtmlContainerFun)
            : HtmlContainerFun =
            fun PARTAS_BUILDER ->
                PARTAS_FIRST PARTAS_BUILDER
                PARTAS_SECOND PARTAS_BUILDER
        [<Erase>]
        member inline _.Delay([<InlineIfLambda>] PARTAS_DELAY: unit -> HtmlContainerFun) : HtmlContainerFun = PARTAS_DELAY()
        [<Erase>]
        member inline _.Zero() : HtmlContainerFun = ignore
        [<Erase>]
        member inline _.Yield(PARTAS_ELEMENT: Route) : HtmlContainerFun = fun PARTAS_CONT -> ignore PARTAS_ELEMENT
        [<Erase>]
        member inline _.Yield(PARTAS_ELEMENT: RootConfig[]) : HtmlContainerFun = fun PARTAS_CONT -> ignore PARTAS_ELEMENT

    // [<Import("HashRouter", "@solidjs/router")>] <-- injected by plugin
    type HashRouter() =
        inherit Router()

    [<AllowNullLiteral>]
    [<Global>]
    type PreloadData [<ParamObject; Emit("$0")>] (preloadData: bool) =
        member val preloadData: bool = jsNative with get, set

    [<Erase>]
    type Extensions =
        [<Extension; Erase>]
        static member Run(PARTAS_THIS: Router, PARTAS_RUN: HtmlContainerFun) =
            PARTAS_RUN Unchecked.defaultof<_>
            PARTAS_THIS

        [<Extension; Erase>]
        static member Run(PARTAS_THIS: Route, PARTAS_RUN: HtmlContainerFun) =
            PARTAS_RUN Unchecked.defaultof<_>
            PARTAS_THIS

    [<Import("A", "@solidjs/router")>]
    type A() =
        inherit RegularNode()
        [<Erase>]
        member this.href
            with set (value: string) = ()
        [<Erase>]
        member this.noScroll
            with set (value: bool) = ()
        [<Erase>]
        member this.replace
            with set (value: bool) = ()
        [<Erase>]
        member this.state
            with set (value: obj) = ()
        [<Erase>]
        member this.activeClass
            with set (value: string) = ()
        [<Erase>]
        member this.inactiveClass
            with set (value: string) = ()
        [<Erase>]
        member this.end'
            with set (value: bool) = ()

    [<Import("Navigate", "@solidjs/router")>]
    type Navigate() =
        inherit RegularNode()
        [<Erase>]
        member this.href
            with set (value: string) = ()
        [<Erase>]
        member this.state
            with set (value: obj) = ()


[<AutoOpen>]
[<Erase>]
type Bindings =

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
