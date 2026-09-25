module Partas.Solid.Tests.Runtime.Integration.Apps.Sidebar

open System
open Partas.Solid
open Partas.Solid.Aria
open Fable.Core
open Fable.Core.JsInterop

// Kobalte / shadcn-solid style sidebar, modelled on
// Partas.Solid.Tests.Plugin/Compiled/SolidCases/Property getters mixed with Operands are transformed/OperatorsInProps.fs:
// a context provider owns the open state, consumers read it through useSidebar, and the
// visual variants are chosen by StringEnum props with defaults set through `props.x <- ...`.

[<Erase>]
module sidebar =
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
        | [<CompiledName("none")>] NonCollapsible

    [<StringEnum>]
    type Size =
        | Sm
        | Md
        | Lg

type SidebarContext =
    {| state: Accessor<sidebar.State>
       open': Accessor<bool>
       setOpen: bool -> unit
       toggle: unit -> unit |}

/// Local clsx/twMerge stand-in: drops empty/undefined entries.
let cn (classes: string array) =
    classes
    |> Array.filter (fun c -> not (String.IsNullOrEmpty c))
    |> String.concat " "

/// cva-like variant resolver for the menu button.
let menuButtonVariants (variant: sidebar.Variant) (size: sidebar.Size) =
    let v =
        match variant with
        | sidebar.Sidebar -> "mb-default"
        | sidebar.Floating -> "mb-floating"
        | sidebar.Inset -> "mb-inset"

    let s =
        match size with
        | sidebar.Sm -> "h-7 text-xs"
        | sidebar.Md -> "h-8 text-sm"
        | sidebar.Lg -> "h-12 text-base"

    cn [| "menu-button"; v; s |]

[<Erase>]
module Context =
    let SidebarContext = createContext<SidebarContext> ()

    let useSidebar () = useContext SidebarContext

[<Erase>]
type SidebarProvider() =
    inherit div()

    [<Erase>]
    member val defaultOpen: bool = unbox null with get, set

    [<Erase>]
    member val onOpenChange: bool -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.defaultOpen <- true

        let isOpen, setIsOpen = createSignal props.defaultOpen

        let setOpen (value: bool) =
            if value <> isOpen () then
                setIsOpen value

                if not (isNull (box props.onOpenChange)) then
                    props.onOpenChange value

        let state =
            createMemo (fun (_: sidebar.State option) -> if isOpen () then sidebar.Expanded else sidebar.Collapsed)

        let ctx: SidebarContext =
            {| state = state
               open' = isOpen
               setOpen = setOpen
               toggle = fun () -> setOpen (not (isOpen ())) |}

        Context.SidebarContext(ctx) {
            div(class' = cn [| "sidebar-wrapper"; props.class' |])
                .data("state", unbox<string> (state ()))
                .spread props {
                props.children
            }
        }

[<Erase>]
type Sidebar() =
    inherit div()

    [<Erase>]
    member val side: sidebar.Side = unbox null with get, set

    [<Erase>]
    member val variant: sidebar.Variant = unbox null with get, set

    [<Erase>]
    member val collapsible: sidebar.Collapsible = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.side <- sidebar.Left
        props.variant <- sidebar.Sidebar
        props.collapsible <- sidebar.Offcanvas
        let ctx = Context.useSidebar ()

        Switch () {
            Match (when' = (props.collapsible = sidebar.NonCollapsible)) {
                div(class' = cn [| "sidebar static"; props.class' |]).spread props { props.children }
            }

            Match (when' = true) {
                div(
                    class' =
                        cn
                            [| "sidebar"
                               (if props.side = sidebar.Left then "left" else "right")
                               (if props.variant = sidebar.Floating || props.variant = sidebar.Inset then
                                    "raised"
                                else
                                    "flat")
                               props.class' |]
                )
                    .data("state", unbox<string> (ctx.state ()))
                    .data("variant", unbox<string> props.variant)
                    .data("collapsible", (if ctx.state () = sidebar.Collapsed then unbox<string> props.collapsible else ""))
                    .spread props {
                    div (class' = "sidebar-inner") { props.children }
                }
            }
        }

[<Erase>]
type SidebarTrigger() =
    inherit button()

    [<SolidTypeComponent>]
    member props.View =
        let ctx = Context.useSidebar ()

        button(
            class' = cn [| "sidebar-trigger"; props.class' |],
            ariaExpanded = (if ctx.open' () then "true" else "false"),
            onClick = fun _ -> ctx.toggle ()
        )
            .spread props {
            "Toggle Sidebar"
        }

[<Erase>]
type SidebarMenuButton() =
    inherit button()

    [<Erase>]
    member val isActive: bool = unbox null with get, set

    [<Erase>]
    member val variant: sidebar.Variant = unbox null with get, set

    [<Erase>]
    member val size: sidebar.Size = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.isActive <- false
        props.variant <- sidebar.Sidebar
        props.size <- sidebar.Md

        button(class' = cn [| menuButtonVariants props.variant props.size; props.class' |])
            .data("active", (if props.isActive then "true" else "false"))
            .spread props {
            props.children
        }

/// A whole page composed from the pieces above.
[<Erase>]
type SidebarApp() =
    inherit div()

    [<Erase>]
    member val startOpen: bool = unbox null with get, set

    [<Erase>]
    member val look: sidebar.Variant = unbox null with get, set

    [<Erase>]
    member val onToggle: bool -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let active, setActive = createSignal "home"

        SidebarProvider(defaultOpen = props.startOpen, onOpenChange = props.onToggle, class' = "app") {
          Sidebar (variant = props.look, side = sidebar.Right, collapsible = sidebar.Icon, id = "main-sidebar") {
              For.Keyed (each = [| "home"; "inbox"; "settings" |]) {
                  yield
                      fun item _ ->
                          SidebarMenuButton(
                              isActive = (active () = item),
                              size = (if item = "settings" then sidebar.Sm else sidebar.Md),
                              onClick = fun _ -> setActive item
                          )
                              .data ("item", item) {
                              item
                          }
              }
          }
  
          main (class' = "content") {
              SidebarTrigger (class' = "extra")
              p (class' = "active-item") { active () }
          }
        }

/// A consumer mounted with no provider: default-less contexts throw ContextNotFoundError.
[<SolidComponent>]
let OrphanTrigger () = SidebarTrigger ()
