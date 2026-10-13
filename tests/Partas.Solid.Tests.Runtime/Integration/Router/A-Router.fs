module Partas.Solid.Tests.Runtime.Integration.Router.App

open Partas.Solid
open Partas.Solid.Router
open Fable.Core
open Fable.Core.JsInterop

[<SolidComponent>]
let Home () = h1 () { "Home" }

[<SolidComponent>]
let About () = h1 () { "About" }

/// Reads its parameter from the props the router passes.
[<SolidComponent>]
let User (props: RouteSectionProps) =
    h1 (id = "user") { "User " + (props.``params``["id"] |> Option.defaultValue "?") }

/// A layout: the matched child renders where it places `props.children`.
[<SolidComponent>]
let UsersLayout (props: RouteSectionProps) =
    section (id = "users") {
        h2 () { "Users" }
        props.children
    }

[<SolidComponent>]
let UsersIndex () = p (id = "users-index") { "Pick a user" }

/// Reads its parameter through `useParams`, from inside a nested route.
[<SolidComponent>]
let UserSettings () =
    let ps = useParams ()
    p (id = "settings") { "Settings of " + (ps["id"] |> Option.defaultValue "?") }

/// Query string: read a key, merge one in.
[<SolidComponent>]
let Search () =
    let search, setSearch = useSearchParams ()

    div () {
        span (id = "q") { search["q"] |> Option.defaultValue "" }
        button (id = "set-q", onClick = fun _ -> setSearch.Invoke {| q = "solid" |}) { "Set q" }
    }

/// A query read through an async memo, with the route's `preload` warming it.
let getGreeting = query ((fun (name: string) -> JS.Constructors.Promise.resolve ("Hello " + name)), "greeting")

[<SolidComponent>]
let Greet (props: RouteSectionProps) =
    let greeting =
        createMemo (fun (_: string option) -> getGreeting.Invoke(props.``params``["name"] |> Option.defaultValue "nobody"))

    Loading (fallback = p (id = "greet-loading") { "Loading" }) { p (id = "greet") { greeting () } }

/// An action called directly through `useAction`; its settled submissions are listed.
let addItem =
    action ((fun (text: string) -> JS.Constructors.Promise.resolve ("added " + text)), "add-item")

/// An action a form posts its `FormData` to, with a bound first argument.
let saveTitle =
    action ((fun (id: int) (data: obj) -> JS.Constructors.Promise.resolve (string id + ":" + unbox<string> (data?get ("title")))), "save-title")

[<SolidComponent>]
let Actions () =
    let add = useAction addItem
    let added = useSubmissions addItem
    let saved = useSubmissions saveTitle

    div () {
        button (id = "add", onClick = fun _ -> add "x" |> ignore) { "Add" }
        span (id = "added") { added |> Array.map (fun s -> s.result |> Option.defaultValue "") |> String.concat "," }
        form (id = "save", action = (saveTitle.with' 3).url, method = Attr.Method.Post) {
            input (name = "title", value = "Draft")
            button (type' = Attr.ButtonType.Submit) { "Save" }
        }
        span (id = "saved") { saved |> Array.map (fun s -> s.result |> Option.defaultValue "") |> String.concat "," }
    }

[<SolidComponent>]
let NotFound () = h1 (id = "not-found") { "Not found" }

let history = memoryHistory "/"

let Router =
    createRouter (
        RouterConfig(
            [| route ("/", Home)
               route ("/about", About)
               route (
                   "/users",
                   UsersLayout,
                   children =
                       [| route ("/", UsersIndex)
                          route ("/:id", User, matchFilters = {| id = intFilter |})
                          route ("/:id/settings", UserSettings) |]
               )
               route ("/search", Search)
               route ("/greet/:name", Greet, preload = fun args -> box (getGreeting.Invoke(args.``params``["name"].Value)))
               route ("/actions", Actions)
               route ("*404", NotFound) |],
            history = history
        )
    )

/// Navigation: hooks must run under the router, so this renders inside its root layout.
[<SolidComponent>]
let Nav () =
    let navigate = useNavigate ()
    let location = useLocation ()
    let usersLink = useLinkState ((fun () -> "/users"))
    let userMatch = useMatch (fun () -> "/users/:id")
    let matches = useRouteMatches ()

    nav () {
        a (id = "to-home", href = "/") { "Home" }
        a (id = "to-about", href = "/about") { "About" }
        a (id = "to-user", href = "/users/42", replace = true) { "User 42" }
        a (id = "to-paths", href = Router.paths["users"].Invoke(7).href) { "User 7" }
        a (id = "with-state", href = "/about", state = "{\"from\":\"nav\"}", noscroll = true) { "About with state" }
        a (id = "no-preload", href = "/about", preload = "false") { "About, never preloaded" }
        span (id = "state") { location.state |> Option.map (fun st -> string st?from) |> Option.defaultValue "" }
        span (id = "path") { location.pathname }
        span (id = "users-active") { if usersLink.active () then "active" else "inactive" }
        span (id = "users-current") { if usersLink.current () then "current" else "not current" }
        span (id = "user-match") { userMatch () |> Option.map (fun m -> m.``params``["id"] |> Option.defaultValue "") |> Option.defaultValue "none" }
        span (id = "depth") { string (matches().Length) }
        button (id = "go", onClick = fun _ -> navigate.Invoke "/users/7/settings") { "Settings of 7" }
        button (id = "back", onClick = fun _ -> navigate.Invoke -1) { "Back" }
    }

/// The root layout: `Nav` stays mounted, the matched route renders in `main`.
[<SolidComponent>]
let App () =
    Router % RouterRoot() {
        yield fun props ->
            div () {
                Nav ()
                main () { props.children }
            }
    }

/// The router with no root layout.
[<SolidComponent>]
let Bare () = Router.render ()
