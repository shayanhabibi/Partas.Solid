module Partas.Solid.Tests.Runtime.Integration.Composition.Lazy

open Partas.Solid
open Partas.Solid.Web
open Partas.Solid.Experimental
open Fable.Core
open Fable.Core.JsInterop

/// `lazy'` with a named export, importing the sibling module.
let LazyPanelNamed: LazyComponent<HtmlElement> =
    lazy' ((fun () -> importDynamic "./F-LazyTarget.fs.jsx"), LazyOptions(``export`` = "LazyPanel"))

/// `lazyload { }` builder: maps the module to a `{ default }` shape.
let LazyPanelBuilder: LazyComponent<HtmlElement> =
    lazyload {
        (importDynamic "./F-LazyTarget.fs.jsx": JS.Promise<obj>)
            .``then``(fun m -> createObj [ "default" ==> m?LazyPanel ])
        |> unbox<JS.Promise<HtmlElement>>
    }

/// A lazy component whose loader is supplied by the caller (so the test controls resolution).
let makeLazy (loader: unit -> JS.Promise<obj>) : LazyComponent<HtmlElement> =
    lazy' ((fun () -> unbox (loader ())), LazyOptions(``export`` = "LazyPanel"))

/// Renders a lazy component through <Dynamic> under <Loading>.
[<Erase>]
type LazyHost() =
    inherit div()

    [<Erase>]
    member val comp: obj = unbox null with get, set

    [<Erase>]
    member val title: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let dynProps = createObj [ "title" ==> props.title; "children" ==> "lazy body" ]

        div (class' = "lazy-host") {
            Loading(fallback = span (class' = "lazy-fallback") { "loading..." }) {
                Dynamic<obj>(component' = unbox props.comp).spread dynProps
            }
        }

[<SolidComponent>]
let NamedLazyHost () =
    LazyHost(comp = box LazyPanelNamed, title = "named")

[<SolidComponent>]
let BuilderLazyHost () =
    LazyHost(comp = box LazyPanelBuilder, title = "builder")

/// The lazy component is only mounted after a toggle, then preloaded state is reused.
[<SolidComponent>]
let ToggleLazyHost (comp: obj) =
    let show, setShow = createSignal false

    div (class' = "toggle-lazy") {
        button (class' = "show", onClick = fun _ -> setShow (not (show ()))) { "show" }
        Show(when' = show ()) { LazyHost(comp = comp, title = "toggled") }
    }
