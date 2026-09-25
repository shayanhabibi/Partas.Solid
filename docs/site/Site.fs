module Docs.Site

open Nacara.Core
open Nacara.Plugins
open Partas.Nacara.Theme

let versions = [ SiteVersion.root "3.0" ]

let theme =
    Theme.defaults
    |> Theme.navbar
        [
            NavbarSection("Guide", "guide", "/guide/")
            NavbarSection("Examples", "examples", "/examples/data-table/")
            NavbarSection("Ecosystem", "ecosystem", "/ecosystem/")
            NavbarSection("About", "about", "/about/")
            NavbarSection("Contributing", "contributing", "/contributing/")
        ]
    |> Theme.menu
        "guide"
        [
            Menu.section
                "Getting started"
                [
                    Menu.page "guide/index.md"
                    Menu.page "guide/installation.md"
                    Menu.page "guide/overview.md"
                    Menu.page "guide/migrating-to-solid-2.md" |> Menu.badge "New"
                ]
            Menu.section
                "Writing components"
                [
                    Menu.page "guide/oxpecker-dsl.md"
                    Menu.page "guide/building-the-dom.md"
                    Menu.page "guide/api-differences.md"
                    Menu.page "guide/common-issues.md"
                ]
            Menu.section
                "Attributes"
                [
                    Menu.page "guide/solid-component-attribute.md"
                    Menu.page "guide/solid-type-attribute.md"
                    Menu.page "guide/attribute-flags.md"
                ]
            Menu.section
                "Tags"
                [
                    Menu.page "guide/tag-interfaces.md"
                    Menu.page "guide/tag-values.md"
                    Menu.page "guide/extension-methods.md"
                    Menu.page "guide/aria-attributes.md"
                    Menu.page "guide/svg-elements.md"
                ]
            Menu.section
                "Solid APIs"
                [
                    Menu.page "guide/solid-js.md"
                    Menu.page "guide/experimental.md"
                    Menu.page "guide/solid-router.md"
                    Menu.page "guide/solid-meta.md"
                    Menu.page "guide/solid-start.md"
                    Menu.page "guide/storybook.md"
                ]
        ]
    |> Theme.menu
        "examples"
        [
            Menu.section
                "Data table"
                [
                    Menu.page "examples/data-table/index.md"
                    Menu.page "examples/data-table/utils.md"
                    Menu.page "examples/data-table/table-component.md"
                    Menu.page "examples/data-table/datatable-component.md"
                    Menu.page "examples/data-table/column-defs.md"
                    Menu.page "examples/data-table/table-render.md"
                    Menu.page "examples/data-table/selectable-rows.md"
                ]
        ]
    |> Theme.menu
        "ecosystem"
        [
            Menu.page "ecosystem/index.md"
            Menu.section
                "Writing bindings"
                [
                    Menu.page "ecosystem/bindings.md"
                    Menu.page "ecosystem/polymorphism.md"
                ]
            Menu.section
                "Libraries"
                [
                    Menu.page "ecosystem/partas-solid-ui.md"
                    Menu.page "ecosystem/kobalte.md"
                    Menu.page "ecosystem/tan-stack-table.md"
                    Menu.page "ecosystem/apexcharts.md"
                    Menu.page "ecosystem/cmdk.md"
                    Menu.page "ecosystem/lucide.md"
                    Menu.page "ecosystem/modular-forms.md"
                    Menu.page "ecosystem/motion.md"
                    Menu.page "ecosystem/neodrag.md"
                    Menu.page "ecosystem/storybook.md"
                ]
            Menu.section
                "Primitives"
                [
                    Menu.page "ecosystem/primitives/index.md"
                    Menu.page "ecosystem/primitives/active-element.md"
                    Menu.page "ecosystem/primitives/autofocus.md"
                    Menu.page "ecosystem/primitives/bounds.md"
                    Menu.page "ecosystem/primitives/broadcast-channel.md"
                    Menu.page "ecosystem/primitives/clipboard.md"
                    Menu.page "ecosystem/primitives/devices.md"
                    Menu.page "ecosystem/primitives/event-bus.md"
                    Menu.page "ecosystem/primitives/event-listener.md"
                    Menu.page "ecosystem/primitives/idle.md"
                    Menu.page "ecosystem/primitives/input-mask.md"
                    Menu.page "ecosystem/primitives/jsx-tokenizer.md"
                    Menu.page "ecosystem/primitives/keyboard.md"
                    Menu.page "ecosystem/primitives/media.md"
                    Menu.page "ecosystem/primitives/mouse.md"
                    Menu.page "ecosystem/primitives/permission.md"
                    Menu.page "ecosystem/primitives/raf.md"
                    Menu.page "ecosystem/primitives/rootless.md"
                    Menu.page "ecosystem/primitives/scheduled.md"
                    Menu.page "ecosystem/primitives/scroll.md"
                    Menu.page "ecosystem/primitives/spring.md"
                    Menu.page "ecosystem/primitives/storage.md"
                    Menu.page "ecosystem/primitives/timer.md"
                    Menu.page "ecosystem/primitives/trigger.md"
                    Menu.page "ecosystem/primitives/tween.md"
                    Menu.page "ecosystem/primitives/websocket.md"
                ]
        ]
    |> Theme.menu
        "about"
        [
            Menu.page "about/index.md"
            Menu.page "about/jsx-output.md"
            Menu.page "about/oxpecker-fork.md"
        ]
    |> Theme.menu
        "contributing"
        [
            Menu.page "contributing/index.md"
            Menu.page "contributing/submit-issues.md"
            Menu.section
                "Plugin internals"
                [
                    Menu.page "contributing/dev/file-structure.md"
                    Menu.page "contributing/dev/spec.md"
                    Menu.page "contributing/dev/types.md"
                    Menu.page "contributing/dev/utilities.md"
                    Menu.page "contributing/dev/patterns.md"
                    Menu.page "contributing/dev/transformation.md"
                ]
        ]
    |> Theme.navbarEnd
        [
            NavbarDynamicWidget(Versions.switcher (Versions.versions versions Versions.defaults))
            NavbarIcon("GitHub", "https://github.com/shayanhabibi/Partas.Solid", Icons.github)
        ]
    |> Theme.editUrl "https://github.com/shayanhabibi/Partas.Solid/edit/master/docs/site"
    |> Theme.footer (Feliz.ViewEngine.Html.p [ Feliz.ViewEngine.Html.text "Built with Nacara" ])

/// The Partas.Solid the live examples compile against. Until Partas.Solid 3 is on NuGet, a
/// prebuilt pair is committed under docs/site/feed. PARTAS_SOLID_FEED and PARTAS_SOLID_VERSION
/// override them, for trying the docs against another local build.
let private solidExamples (options: SolidExamplesOptions) =
    let fromEnvironment name =
        match System.Environment.GetEnvironmentVariable name with
        | null
        | "" -> None
        | value -> Some value

    options
    |> SolidExamples.partasVersion (
        fromEnvironment "PARTAS_SOLID_VERSION"
        |> Option.defaultValue "3.0.0-local.cd6d4e2"
    )
    |> SolidExamples.feed (
        fromEnvironment "PARTAS_SOLID_FEED"
        |> Option.defaultValue (System.IO.Path.Combine(__SOURCE_DIRECTORY__, "feed"))
    )

let site =
    Site.create "Partas.Solid"
    |> Site.baseUrl "/Partas.Solid/"
    |> Site.origin "https://shayanhabibi.github.io"
    |> Site.output "output"
    |> Markdown.register
    |> TreeSitter.register
    |> Sitemap.register
    |> LinkValidator.register
    |> SolidExamples.registerWith solidExamples
    |> Esbuild.register
    |> Nuglify.minifyHtml
    |> Versions.register versions
    |> GitHubPages.register
    |> Theme.register theme
    |> Site.collection (Theme.docs theme "content")

[<EntryPoint>]
let main argv = Nacara.run site argv
