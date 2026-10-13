namespace Partas.Solid.Router

open System
open Fable.Core
open Fable.Core.JS
open Fable.Core.TS
open Partas.Solid

/// <summary>
/// Bindings for <c>@solidjs/router</c> 2 (Solid 2). The router is configured, not composed: declare the route tree
/// with <c>route</c>, create the router with <c>createRouter</c> at module level, and render the instance it
/// returns as a tag. Links are plain <c>a</c> elements.
/// </summary>
[<AutoOpen>]
module Bindings =
    [<EB(EBState.Never)>]
    module Spec =
        [<Literal>]
        let path = "@solidjs/router"

    /// <summary>Why a preload function is running.</summary>
    [<RequireQualifiedAccess; StringEnum>]
    type Intent =
        /// <summary>The first render.</summary>
        | Initial
        /// <summary>Browser back or forward.</summary>
        | Native
        /// <summary>A router navigation.</summary>
        | Navigate
        /// <summary>A link preload, not a navigation.</summary>
        | Preload

    /// <summary>
    /// The current route's path parameters: a reactive, store-like object. <c>params["id"]</c> reads one, and is
    /// <c>None</c> when the route has no such parameter.
    /// </summary>
    [<AllowNullLiteral>]
    type Params =
        [<EmitIndexer>]
        abstract Item: name: string -> string option with get

    /// <summary>
    /// The current query string as a reactive, store-like object. Read a key to subscribe to it.
    /// </summary>
    [<AllowNullLiteral>]
    type SearchParams =
        /// <summary>The value of a key. A key repeated in the query string is an array at runtime: use <c>getAll</c>.</summary>
        [<EmitIndexer>]
        abstract Item: key: string -> string option with get

        /// <summary>Every value of a key, whether it appears once, several times or not at all.</summary>
        [<Emit("[].concat($0[$1] ?? [])")>]
        abstract getAll: key: string -> string[]

    /// <summary>The router's reactive location.</summary>
    type Location =
        abstract pathname: string
        abstract search: string
        abstract hash: string
        abstract query: SearchParams
        abstract state: obj option
        abstract key: string

    /// <summary>Options for a navigation. Every field is optional.</summary>
    [<JS.Pojo>]
    type NavigateOptions(?resolve: bool, ?replace: bool, ?scroll: bool, ?state: obj) =
        /// <summary>Resolve the path against the current route. Defaults to <c>true</c>.</summary>
        member val resolve: bool = JS.undefined with get, set
        /// <summary>Replace the history entry instead of pushing one. Defaults to <c>false</c>.</summary>
        member val replace: bool = JS.undefined with get, set
        /// <summary>Scroll to the top after navigating. Defaults to <c>true</c>.</summary>
        member val scroll: bool = JS.undefined with get, set
        /// <summary>Custom state for <c>location.state</c>, serialised with the structured clone algorithm.</summary>
        member val state: obj = JS.undefined with get, set

    /// <summary>
    /// A node of the router's typed path proxy, <c>Router.paths</c>. Index it by segment and call it to bind
    /// parameters: <c>Router.paths["users"].Invoke(2)</c> is <c>/users/2</c>. Pass it to <c>navigate</c>, or use
    /// <c>.href</c> where a string is needed.
    /// </summary>
    [<AllowNullLiteral>]
    type RoutePath =
        /// <summary>The child segment <c>segment</c>.</summary>
        [<EmitIndexer>]
        abstract Item: segment: string -> RoutePath with get

        /// <summary>
        /// Binds parameters, then optionally a search object and a hash, in that order. With no arguments it ends
        /// the path.
        /// </summary>
        [<Emit("$0($1...)")>]
        abstract Invoke: [<ParamArray>] args: obj[] -> RoutePath

        /// <summary>The URL this node stands for.</summary>
        [<Emit("String($0)")>]
        abstract href: string

    /// <summary>Navigates. Returned by <c>useNavigate</c>.</summary>
    type Navigator =
        [<Emit("$0($1...)")>]
        abstract Invoke: ``to``: string * ?options: NavigateOptions -> unit

        [<Emit("$0($1...)")>]
        abstract Invoke: ``to``: RoutePath * ?options: NavigateOptions -> unit

        /// <summary>Moves through the history by <c>delta</c> entries: <c>-1</c> is back.</summary>
        [<Emit("$0($1)")>]
        abstract Invoke: delta: int -> unit

    /// <summary>Merges keys into the query string. <c>""</c>, <c>null</c> and <c>undefined</c> values remove a key.</summary>
    type SearchParamsSetter =
        [<Emit("$0($1...)")>]
        abstract Invoke: values: obj * ?options: NavigateOptions -> unit

    /// <summary>Passed to a <c>useBeforeLeave</c> listener.</summary>
    type BeforeLeaveEventArgs =
        /// <summary>The location being left.</summary>
        abstract from: Location
        /// <summary>The path, or history delta, passed to <c>navigate</c>.</summary>
        abstract ``to``: U2<string, int>
        abstract options: NavigateOptions option
        /// <summary><c>true</c> when an earlier listener called <c>preventDefault</c>.</summary>
        abstract defaultPrevented: bool
        /// <summary>Blocks the navigation.</summary>
        abstract preventDefault: unit -> unit
        /// <summary>Retries the navigation; <c>force</c> skips the leave listeners.</summary>
        abstract retry: ?force: bool -> unit

    /// <summary>Passed to a route's <c>preload</c> function.</summary>
    type RoutePreloadFuncArgs =
        abstract ``params``: Params
        abstract location: Location
        abstract intent: Intent

    /// <summary>
    /// The props a route component receives, and the argument of the router's root layout
    /// (<c>RouterRoot</c>). <c>children</c> is the matched content.
    /// </summary>
    type RouteSectionProps =
        abstract ``params``: Params
        abstract location: Location
        /// <summary>What the route's <c>preload</c> returned (the router config's <c>preload</c>, for the root).</summary>
        abstract data: obj
        abstract children: HtmlElement

    /// <summary>A route: build one with <c>route</c>.</summary>
    type RouteDefinition = interface end

    /// <summary>Route metadata, as resolved by the router. See <c>useRouteMatches</c>.</summary>
    type RouteDescription =
        abstract originalPath: string
        abstract pattern: string
        abstract info: obj option

    /// <summary>A path matched against the location.</summary>
    type PathMatch =
        abstract ``params``: Params
        abstract path: string

    /// <summary>One route of the chain matching the location.</summary>
    type RouteMatch =
        inherit PathMatch
        abstract route: RouteDescription

    /// <summary>One route of the chain matching a URL. See <c>RouterInstance.matchUrl</c>.</summary>
    type OutputMatch =
        abstract path: string
        abstract pattern: string
        abstract ``match``: string
        abstract ``params``: Params
        abstract info: obj option

    /// <summary>
    /// Where the router reads the URL and writes navigations: <c>browserHistory</c>, <c>hashHistory</c> or
    /// <c>memoryHistory</c>.
    /// </summary>
    type RouterHistory = interface end

    /// <summary>An in-memory history, for tests and environments without a URL bar.</summary>
    type MemoryHistory =
        inherit RouterHistory
        abstract get: unit -> string
        abstract go: delta: int -> unit
        abstract back: unit -> unit
        abstract forward: unit -> unit
        /// <summary>Calls <c>listener</c> on each change; returns the function that stops it.</summary>
        abstract listen: listener: (string -> unit) -> (unit -> unit)

    /// <summary>A link state plugin, such as <c>pendingLinks</c>.</summary>
    type LinksPlugin = interface end

    /// <summary>A link preload strategy, such as <c>intentPreload ()</c>.</summary>
    type LinkPreload = interface end

    /// <summary>A parameter constraint for a route's <c>matchFilters</c>, such as <c>intFilter</c>.</summary>
    type MatchFilter = interface end

    /// <summary>
    /// The router's configuration. Only <c>routes</c> is required. The tree is fixed once the router is created.
    /// </summary>
    [<JS.Pojo>]
    type RouterConfig
        (
            routes: RouteDefinition[],
            ?``base``: string,
            ?preload: RoutePreloadFuncArgs -> obj,
            ?history: RouterHistory,
            ?singleFlight: bool,
            ?actionBase: string,
            ?explicitLinks: bool,
            ?links: LinksPlugin,
            ?preloadLinks: LinkPreload[],
            ?scrollRestoration: bool,
            ?transformUrl: string -> string
        ) =
        member val routes: RouteDefinition[] = routes with get, set

    /// <summary>
    /// A router: the value <c>createRouter</c> returns. It is a component, so render it as a tag:
    /// <c>Router.render ()</c>, or <c>Router % RouterRoot() { yield fun props -> ... }</c> to wrap the matched
    /// content in a root layout. Bind it to a capitalised name, as the Solid compiler only treats capitalised
    /// tags as components.
    /// </summary>
    [<Erase>]
    type RouterInstance() =
        inherit TagValue(unbox<obj -> HtmlElement> null)

        /// <summary>The typed path proxy: <c>Router.paths["users"].Invoke(2)</c> is <c>/users/2</c>.</summary>
        [<Emit("$0.paths")>]
        member _.paths: RoutePath = jsNative

        /// <summary>The route tree.</summary>
        [<Emit("$0.routes")>]
        member _.routes: RouteDefinition[] = jsNative

        /// <summary>The routes matching <c>url</c>, outermost first; empty when none matches. Nothing renders.</summary>
        [<Emit("$0.match($1)")>]
        member _.matchUrl(url: string) : OutputMatch[] = jsNative

        /// <summary>The query keys of the server component routes <c>url</c> shows, outermost first.</summary>
        [<Emit("$0.keysFor($1)")>]
        member _.keysFor(url: string) : string[] = jsNative

    /// <summary>
    /// The root layout of a router. It stays mounted across navigations, and its child lambda receives the matched
    /// content as <c>props.children</c>: <c>Router % RouterRoot() { yield fun props -> main () { props.children } }</c>.
    /// </summary>
    [<Erase>]
    type RouterRoot() =
        interface ChildLambdaProvider<RouteSectionProps>

        /// <summary>Server only: the location to render when no request is in scope. Ignored on the client.</summary>
        [<Erase; DefaultValue>]
        val mutable url: string

    /// <summary>A link's state. See <c>useLinkState</c>.</summary>
    type LinkState =
        /// <summary>The location's path is the link's path or lies under it; the query is ignored.</summary>
        abstract active: unit -> bool
        /// <summary>The location matches the link exactly, query included (parameter order and hash aside).</summary>
        abstract current: unit -> bool
        /// <summary>The link is the target of an in-flight navigation.</summary>
        abstract pending: unit -> bool

    /// <summary>Preloads a route's code, and optionally its data. Returned by <c>usePreloadRoute</c>.</summary>
    type RoutePreloader =
        [<Emit("$0($1, { preloadData: $2 })")>]
        abstract Invoke: url: string * preloadData: bool -> unit

        [<Emit("$0($1)")>]
        abstract Invoke: url: string -> unit

    /// <summary>
    /// A query: a fetching function whose calls are deduplicated and cached under its key. Call it with
    /// <c>Invoke</c>, typically inside <c>createMemo</c>.
    /// </summary>
    type Query<'Arg, 'Result> =
        [<Emit("$0($1...)")>]
        abstract Invoke: arg: 'Arg -> 'Result

        /// <summary>The query's key, which revalidates every entry.</summary>
        abstract key: string

        /// <summary>The key of the entry for <c>arg</c>, which revalidates only that entry.</summary>
        abstract keyFor: arg: 'Arg -> string

    /// <summary>A settled submission of an action. See <c>useSubmissions</c>.</summary>
    type Submission<'Input, 'Result> =
        abstract input: 'Input
        abstract result: 'Result option
        abstract error: obj
        abstract url: string
        abstract clear: unit -> unit
        abstract retry: unit -> JS.Promise<'Result>

    /// <summary>
    /// A router action: a mutation with a URL. Bind a form to it with <c>form (action = act.url, method = Attr.Method.Post)</c>,
    /// or call it through <c>useAction</c>.
    /// </summary>
    type Action<'Input, 'Result> =
        /// <summary>The URL that identifies the action, for a form's <c>action</c> or a button's <c>formaction</c>.</summary>
        abstract url: string

        /// <summary>
        /// Registers a hook, owned by the current reactive owner, that runs when a submission starts. Use it to
        /// apply optimistic updates.
        /// </summary>
        abstract onSubmit: hook: ('Input -> unit) -> Action<'Input, 'Result>

        /// <summary>Registers a hook, owned by the current reactive owner, that runs when a submission settles.</summary>
        abstract onSettled: hook: (Submission<'Input, 'Result> -> unit) -> Action<'Input, 'Result>

    /// <summary>An action of two arguments. <c>with'</c> binds the first, as for a form's hidden id.</summary>
    type Action<'Arg, 'Input, 'Result> =
        inherit Action<'Arg * 'Input, 'Result>

        /// <summary>The action with its first argument bound to <c>arg</c>.</summary>
        [<Emit("$0.with($1)")>]
        abstract with': arg: 'Arg -> Action<'Input, 'Result>

/// <summary>Functions of <c>@solidjs/router</c> 2.</summary>
[<AutoOpen; Erase>]
type Bindings =
    /// <summary>
    /// A route. <c>component</c> is a component taking <c>RouteSectionProps</c> (or <c>unit</c>), such as a
    /// <c>[&lt;SolidComponent&gt;]</c> let binding. A parent with a component wraps its children, which render
    /// where it places <c>props.children</c>. <c>preload</c> warms the route's data; what it returns reaches the
    /// component as <c>props.data</c>. <c>matchFilters</c> is an object of filters per parameter:
    /// <c>{| id = intFilter |}</c>.
    /// </summary>
    [<Emit("$0"); ParamObject>]
    static member route
        (
            path: string,
            ``component``: 'Props -> #HtmlElement,
            ?children: RouteDefinition[],
            ?preload: RoutePreloadFuncArgs -> obj,
            ?matchFilters: obj,
            ?info: obj
        ) : RouteDefinition =
        jsNative

    /// <summary>A route matching several paths, which stays mounted when moving between them.</summary>
    [<Emit("$0"); ParamObject>]
    static member routePaths
        (
            path: string[],
            ``component``: 'Props -> #HtmlElement,
            ?children: RouteDefinition[],
            ?preload: RoutePreloadFuncArgs -> obj,
            ?matchFilters: obj,
            ?info: obj
        ) : RouteDefinition =
        jsNative

    /// <summary>A route that only groups its children under <c>path</c>, without a component of its own.</summary>
    [<Emit("$0"); ParamObject>]
    static member routeGroup(path: string, children: RouteDefinition[], ?preload: RoutePreloadFuncArgs -> obj, ?info: obj) : RouteDefinition =
        jsNative

    /// <summary>
    /// Creates the router. Call it once, at module level, and bind the result to a capitalised name: it is the
    /// component to render.
    /// </summary>
    [<ImportMember(Spec.path)>]
    static member createRouter(config: RouterConfig) : RouterInstance = jsNative

    /// <summary>Reads and writes the browser's URL. The default.</summary>
    [<ImportMember(Spec.path)>]
    static member browserHistory() : RouterHistory = jsNative

    /// <summary>Keeps the route in the URL's hash.</summary>
    [<ImportMember(Spec.path)>]
    static member hashHistory() : RouterHistory = jsNative

    /// <summary>Keeps the location in memory, starting at <c>initial</c>.</summary>
    [<ImportMember(Spec.path)>]
    static member memoryHistory(?initial: string) : MemoryHistory = jsNative

    /// <summary>Adds <c>data-pending</c> to links covering the destination of an in-flight navigation.</summary>
    [<ImportMember(Spec.path)>]
    static member pendingLinks: LinksPlugin = jsNative

    /// <summary>Preloads a link when the pointer rests on it for <c>delay</c> ms (default 20), it is focused, or a touch starts on it.</summary>
    [<ImportMember(Spec.path); ParamObject>]
    static member intentPreload(?delay: int, ?data: bool) : LinkPreload = jsNative

    /// <summary>Preloads a link when a pointer goes down on it.</summary>
    [<ImportMember(Spec.path); ParamObject>]
    static member tapPreload(?data: bool) : LinkPreload = jsNative

    /// <summary>
    /// Preloads a link once it has stayed in the viewport for <c>delay</c> ms and the browser is idle. Applies to
    /// links with <c>preload = "viewport"</c>, or to every link with <c>all = true</c>.
    /// </summary>
    [<ImportMember(Spec.path); ParamObject>]
    static member viewportPreload(?all: bool, ?data: bool, ?delay: int, ?rootMargin: string) : LinkPreload = jsNative

    /// <summary>
    /// Preloads a link once the page has loaded and the browser is idle. Applies to links with
    /// <c>preload = "eager"</c>, or to every link with <c>all = true</c>.
    /// </summary>
    [<ImportMember(Spec.path); ParamObject>]
    static member eagerPreload(?all: bool, ?data: bool) : LinkPreload = jsNative

    /// <summary>A match filter that accepts integers only.</summary>
    [<Import("int", Spec.path)>]
    static member intFilter: MatchFilter = jsNative

    /// <summary>The current route's path parameters.</summary>
    [<ImportMember(Spec.path)>]
    static member useParams() : Params = jsNative

    /// <summary>The function that navigates.</summary>
    [<ImportMember(Spec.path)>]
    static member useNavigate() : Navigator = jsNative

    /// <summary>The reactive location.</summary>
    [<ImportMember(Spec.path)>]
    static member useLocation() : Location = jsNative

    /// <summary>The query string, and the function that merges keys into it. Setting it does not scroll.</summary>
    [<ImportMember(Spec.path)>]
    static member useSearchParams() : SearchParams * SearchParamsSetter = jsNative

    /// <summary><c>true</c> while a navigation, and the data it waits on, is settling.</summary>
    [<ImportMember(Spec.path)>]
    static member useIsRouting() : (unit -> bool) = jsNative

    /// <summary>
    /// Matches the pattern <c>path</c> returns against the location, without consulting the route tree.
    /// </summary>
    [<ImportMember(Spec.path)>]
    static member useMatch(path: unit -> string, ?matchFilters: obj) : (unit -> PathMatch option) = jsNative

    /// <summary>The routes matching the location, outermost first.</summary>
    [<ImportMember(Spec.path)>]
    static member useRouteMatches() : (unit -> RouteMatch[]) = jsNative

    /// <summary>The href for a path, resolved against the current route.</summary>
    [<ImportMember(Spec.path)>]
    static member useHref(``to``: unit -> string) : (unit -> string) = jsNative

    /// <summary>A path resolved against the current route.</summary>
    [<ImportMember(Spec.path)>]
    static member useResolvedPath(path: unit -> string) : (unit -> string option) = jsNative

    /// <summary>The function that preloads a route by hand, as link preloading does.</summary>
    [<ImportMember(Spec.path)>]
    static member usePreloadRoute() : RoutePreloader = jsNative

    /// <summary>
    /// The state of a link to <c>href</c>, for links that need more than the attributes the router sets on
    /// <c>a</c> elements. <c>end</c> makes <c>active</c> an exact match.
    /// </summary>
    [<ImportMember(Spec.path); ParamObject(1)>]
    static member useLinkState(href: unit -> string, ?``end``: bool) : LinkState = jsNative

    /// <summary>Calls <c>listener</c> before the router leaves the current route; it can block the navigation.</summary>
    [<ImportMember(Spec.path)>]
    static member useBeforeLeave(listener: BeforeLeaveEventArgs -> unit) : unit = jsNative

    /// <summary>
    /// Wraps a fetching function so calls are deduplicated and cached under <c>name</c> and the argument. Read a
    /// query's result with Solid's primitives, such as <c>createMemo</c>. Pass several values as a tuple or record.
    /// </summary>
    [<ImportMember(Spec.path)>]
    static member query(fn: 'Arg -> 'Result, name: string) : Query<'Arg, 'Result> = jsNative

    /// <summary>Refetches every query.</summary>
    [<ImportMember(Spec.path)>]
    static member revalidate() : unit = jsNative

    /// <summary>Refetches the queries under <c>key</c>; <c>force</c> skips the cache.</summary>
    [<ImportMember(Spec.path)>]
    static member revalidate(key: string, ?force: bool) : unit = jsNative

    /// <summary>Refetches the queries under each of <c>keys</c>; <c>force</c> skips the cache.</summary>
    [<ImportMember(Spec.path)>]
    static member revalidate(keys: string[], ?force: bool) : unit = jsNative

    /// <summary>
    /// A router action. A form posts its <c>FormData</c> to it. An action that is not a server function needs a
    /// stable <c>name</c>.
    /// </summary>
    [<ImportMember(Spec.path)>]
    static member action(fn: 'Input -> JS.Promise<'Result>, ?name: string) : Action<'Input, 'Result> = jsNative

    /// <summary>A router action of two arguments: bind the first with <c>with'</c>.</summary>
    [<ImportMember(Spec.path)>]
    static member action(fn: 'Arg -> 'Input -> JS.Promise<'Result>, ?name: string) : Action<'Arg, 'Input, 'Result> = jsNative

    /// <summary>The function that calls <c>action</c> directly, in the current router.</summary>
    [<ImportMember(Spec.path)>]
    static member useAction(action: Action<'Input, 'Result>) : ('Input -> JS.Promise<'Result>) = jsNative

    /// <summary>The settled submissions of <c>action</c>, oldest first; <c>filter</c> selects them by input.</summary>
    [<ImportMember(Spec.path)>]
    static member useSubmissions(action: Action<'Input, 'Result>, ?filter: 'Input -> bool) : Submission<'Input, 'Result>[] =
        jsNative
