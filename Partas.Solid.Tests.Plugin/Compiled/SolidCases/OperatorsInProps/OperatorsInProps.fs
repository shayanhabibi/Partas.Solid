module Partas.Solid.Tests.SolidCases.OperatorsInProps

open Partas.Solid
open Fable.Core
open Browser.Types
open Browser.Dom

[<Erase>]
type Lib =
    [<Import("twMerge", "tailwind-merge")>]
    static member inline twMerge(classes: string) : string = jsNative

    [<Import("clsx", "clsx")>]
    static member inline clsx(classes: obj) : string = jsNative

    static member inline cn(classes: string * bool) : string =
        classes
        |> Lib.clsx
        |> Lib.twMerge

    static member inline cn(classes: string array) : string =
        classes
        |> Lib.clsx
        |> Lib.twMerge

    static member inline cn(classes: string) : string =
        classes
        |> Lib.clsx
        |> Lib.twMerge

    [<Import("cva", "class-variance-authority")>]
    static member inline cva (orig: string) (object: 'T) : obj -> string = jsNative

[<AutoOpen; Erase>]
module Operators =
    [<Emit("$0 && $1")>]
    let (&&=) (conditional: 'T) (output: 'M) : 'M = jsNative

[<Erase>]
module sidebar =
    let mobileBreakpoint = 768
    let sidebarCookieName = "sidebar:state"

    let sidebarCookieMaxAge =
        60
        * 60
        * 24
        * 7

    let sidebarWidth = "16rem"
    let sidebarWidthMobile = "18rem"
    let sidebarWidthIcon = "3rem"
    let sidebarKeyboardShortcut = "b"

    [<StringEnum>]
    type State =
        | Expanded
        | Collapsed

    [<StringEnum>]
    type Side =
        | Left
        | Right

    [<StringEnum>]
    type Variant =
        | Sidebar
        | Floating
        | Inset

    [<StringEnum>]
    type Collapsible =
        | Offcanvas
        | Icon
        | None

    type SidebarContext =
        {| state: State Accessor
           open': bool Accessor
           setOpen: bool -> unit
           openMobile: bool Accessor
           setOpenMobile: bool -> unit
           isMobile: bool Accessor
           toggleSidebar: unit -> unit |}

open sidebar
open Fable.Core.JsInterop

[<Erase>]
type Sheet() =
    inherit dialog()

    [<Erase>]
    member val onOpenChange: bool -> unit = unbox null with get, set

[<Erase>]
type SheetContent() =
    inherit dialog()

    [<Erase>]
    member val position: string = unbox null with get, set

[<Erase>]
module Context =
    let SidebarContext = createContext<SidebarContext> ()

    let useSidebar () =
        let context = useContext (SidebarContext)

        if not (unbox context) then
            failwith "useSidebar can only be used within a Sidebar"

        context

    let useIsMobile (fallback: bool) =
        let (isMobile, setIsMobile) = createSignal (fallback)

        createEffect (fun () ->
            let mql =
                window?matchMedia (
                    $"(max-width:{mobileBreakpoint
                                  - 1}px)"
                )

            let onChange = fun (e) -> setIsMobile (e?matches)
            mql?addEventListener ("change", onChange)
            onChange (mql)
            onCleanup (fun () -> mql?removeEventListener ("change", onChange)))

        isMobile

[<Erase>]
type Sidebar() =
    inherit div()

    [<Erase>]
    member val side: sidebar.Side = unbox null with get, set

    [<Erase>]
    member val variant: sidebar.Variant = unbox null with get, set

    [<Erase>]
    member val collapsible: sidebar.Collapsible = unbox null with get, set

    [<SolidTypeComponentAttribute>]
    member props.constructor =
        props.side <- Left
        props.variant <- sidebar.Sidebar
        props.collapsible <- Offcanvas
        let ctx = Context.useSidebar ()

        let (isMobile, state, openMobile, setOpenMobile) =
            (ctx.isMobile, ctx.state, ctx.openMobile, ctx.openMobile)

        Switch () {
            Match (when' = (props.collapsible = sidebar.None)) {

                div(
                    class' =
                        Lib.cn
                            [| "test flex h-full w-[--sidebar-width] flex-col bg-sidebar text-sidebar-foreground"
                               props.class' |]
                )
                    .spread
                    props {
                    props.children
                }

            }

            Match (when' = isMobile ()) {

                Sheet(open' = openMobile (), onOpenChange = !!setOpenMobile).spread props {
                    SheetContent(class' = "w-[--sidebar-width] bg-sidebar p-0 text-sidebar-foreground [&>button]:hidden", position = !!props.side)
                        .data("sidebar", !!sidebar.Sidebar)
                        .data("mobile", "true")
                        .style' (
                            createObj
                                [ "--sidebar-width"
                                  ==> sidebarWidthMobile ]
                        ) {
                        div (class' = "flex size-full flex-col") { props.children }
                    }
                }

            }

            Match (
                when' =
                    (isMobile ()
                     |> not)
            ) {
                // gap handler on desktop
                div (
                    class' =
                        Lib.cn
                            [| "a1"
                               "a2"
                               "a3"
                               if
                                   (props.variant = sidebar.Floating
                                    || props.variant = sidebar.Inset)
                               then
                                   "?a1"
                               else
                                   "?a2" |]
                )

                div(
                    class' =
                        Lib.cn
                            [| "b1"
                               if props.side = sidebar.Left then "?b1" else "?b2"
                               // Adjust the padding for floating and inset variants.
                               if
                                   props.variant = sidebar.Floating
                                   || props.variant = sidebar.Inset
                               then
                                   "?b3"
                               else
                                   "?b4"
                               props.class' |]
                )
                    .spread
                    props {
                    div(class' = "classend").data ("sidebar", !!sidebar.Sidebar) { props.children }
                }
            }

        }
