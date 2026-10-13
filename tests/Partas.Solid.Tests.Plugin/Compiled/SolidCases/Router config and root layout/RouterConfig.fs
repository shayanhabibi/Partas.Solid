module Partas.Solid.Tests.SolidCases.RouterConfig

open Partas.Solid
open Partas.Solid.Router
open Fable.Core

[<SolidComponent>]
let Home () = h1 () { "Home" }

[<SolidComponent>]
let User (props: RouteSectionProps) =
    h1 () { props.``params``["id"] |> Option.defaultValue "" }

[<SolidComponent>]
let Layout (props: RouteSectionProps) = section () { props.children }

let Router =
    createRouter (
        RouterConfig(
            [| route ("/", Home)
               route ("/users", Layout, children = [| route ("/:id", User, matchFilters = {| id = intFilter |}) |])
               routePaths ([| "/login"; "/register" |], Home)
               routeGroup ("/admin", [| route ("/", Home) |]) |],
            ``base`` = "/app",
            history = hashHistory (),
            links = pendingLinks,
            preloadLinks = [| intentPreload (delay = 50) |]
        )
    )

[<SolidComponent>]
let App () =
    Router % RouterRoot() {
        yield fun props ->
            div () {
                nav () {
                    a (href = "/") { "Home" }
                    a (href = "/users/1", replace = true, noscroll = true, preload = "false") { "User 1" }
                }
                main () { props.children }
            }
    }

[<SolidComponent>]
let Bare () = Router.render ()

[<SolidComponent>]
let Hooks () =
    let navigate = useNavigate ()
    let isRouting = useIsRouting ()
    useBeforeLeave (fun e -> if not e.defaultPrevented then e.preventDefault ())

    button (
        disabled = isRouting (),
        onClick =
            fun _ ->
                revalidate "users"
                navigate.Invoke("/users", NavigateOptions(replace = true, scroll = false))
    ) {
        "Users"
    }
