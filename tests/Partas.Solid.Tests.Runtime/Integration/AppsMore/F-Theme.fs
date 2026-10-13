module Partas.Solid.Tests.Runtime.Integration.AppsMore.Theme

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

[<StringEnum>]
type ThemeName =
    | Light
    | Dark
    | Sepia

[<JS.Pojo>]
type ThemeApi(theme: Accessor<ThemeName>, setTheme: ThemeName -> unit, toggle: unit -> unit) =
    member val theme = theme with get, set
    member val setTheme = setTheme with get, set
    member val toggle = toggle with get, set

/// Default (no provider): a fixed light theme whose setters do nothing.
let ThemeContext =
    createContext<ThemeApi> (ThemeApi((fun () -> Light), ignore, ignore))

let useTheme () = useContext ThemeContext

/// Provides a reactive theme to its subtree and mirrors it to data-theme on its wrapper.
[<Erase>]
type ThemeProvider() =
    inherit div()

    [<Erase>]
    member val initial: ThemeName = unbox null with get, set

    [<Erase>]
    member val onThemeChange: ThemeName -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let theme, setThemeRaw = createSignal (if isNull (box props.initial) then Light else props.initial)

        let setTheme (t: ThemeName) =
            if t <> theme () then
                setThemeRaw t

                if not (isNull (box props.onThemeChange)) then
                    props.onThemeChange t

        let api =
            ThemeApi(theme, setTheme, (fun () -> setTheme (if theme () = Dark then Light else Dark)))

        ThemeContext api {
            div(class' = "theme-root").data ("theme", unbox<string> (theme ())) { props.children }
        }

/// Consumer: a button whose class follows the theme.
[<SolidComponent>]
let ThemedButton () =
    let api = useTheme ()

    button (class' = $"themed-btn btn-{unbox<string> (api.theme ())}") { $"Theme is {unbox<string> (api.theme ())}" }

/// Consumer: toggles between light and dark.
[<SolidComponent>]
let ThemeToggle () =
    let api = useTheme ()
    button (class' = "toggle-theme", onClick = fun _ -> api.toggle ()) { "Toggle" }

/// Consumer: a select that sets any theme.
[<SolidComponent>]
let ThemePicker () =
    let api = useTheme ()

    select (
        class' = "theme-picker",
        value = unbox<string> (api.theme ()),
        onChange = fun e -> api.setTheme (!!e.currentTarget?value)
    ) {
        option' (value = "light") { "Light" }
        option' (value = "dark") { "Dark" }
        option' (value = "sepia") { "Sepia" }
    }

/// A full page: toolbar + content, with a nested section forced to sepia by an inner provider.
[<Erase>]
type ThemedPage() =
    inherit div()

    [<Erase>]
    member val onThemeChange: ThemeName -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        ThemeProvider(initial = Light, onThemeChange = props.onThemeChange) {
            header (class' = "toolbar") {
                ThemeToggle()
                ThemePicker()
            }

            main (class' = "content") { ThemedButton() }

            aside (class' = "forced") { ThemeProvider(initial = Sepia) { ThemedButton() } }
        }

/// Consumer used with no provider at all.
[<SolidComponent>]
let LonelyButton () =
    div (class' = "lonely") {
        ThemedButton()
        ThemeToggle()
    }
