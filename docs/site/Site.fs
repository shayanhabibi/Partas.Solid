module Docs.Site

open System.IO
open Nacara.Core
open Nacara.Plugins
open Partas.Nacara.Theme

/// 2.x has no build here: its prefix is only static/v2/index.html, which forwards to the old site.
let versions = [ SiteVersion.root "3.0 (unreleased)"; SiteVersion.create "2.x" "v2" ]

/// A stylesheet from docs/site/theme. Read once at startup, so under `nacara watch` an edit to
/// one of these files only shows after a restart.
let private themeCss name =
    File.ReadAllText(Path.Combine(__SOURCE_DIRECTORY__, "theme", name))

/// Non-colour tokens. Applied to both schemes: the dark record is independent of the light one,
/// so any value left at its default would be re-emitted under :root[data-theme="dark"].
let private sizing (t: Tokens) =
    { t with
        FontSans = "\"Geist\", ui-sans-serif, system-ui, -apple-system, \"Segoe UI\", Roboto, sans-serif"
        FontMono = "\"Geist Mono\", ui-monospace, \"JetBrains Mono\", SFMono-Regular, Menlo, monospace"
        Radius = "0.75rem"
        RadiusSm = "0.375rem"
        ContentWidth = "46rem"
        SidebarWidth = "16.5rem"
        TocWidth = "14rem"
        NavbarOpacity = "72%"
        NavbarBlur = "12px"
    }

let theme =
    Theme.defaults
    |> Theme.brandIcon (BrandIcon.Image "logo.png")
    |> Theme.favIcon "favicon.ico"
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
    |> Theme.lightTokens (fun t ->
        { (sizing t) with
            Bg = "#ffffff"
            BgSubtle = "#f8f9fb"
            BgRaised = "#ffffff"
            Border = "#e6e7ec"
            Text = "#1c2024"
            TextMuted = "#5f6570"
            Heading = "#0f1115"
            Primary = "#4f5bd5"
            PrimaryContrast = "#ffffff"
            PrimarySubtle = "#eef0fe"
            Note = "#4f5bd5"
            Tip = "#12805c"
            Warning = "#a15c07"
            Danger = "#d1344e"
            CodeInlineBg = "#f2f3f7"
            CodeInlineBorder = "#e3e5eb"
            CodeInlineText = "#23262f"
            Shadow = "0 1px 2px rgb(16 18 27 / 5%), 0 2px 8px rgb(16 18 27 / 4%)"
            ShadowFloating = "0 12px 32px -8px rgb(16 18 27 / 16%), 0 2px 6px rgb(16 18 27 / 6%)"
        }
    )
    |> Theme.darkTokens (fun t ->
        { (sizing t) with
            Bg = "#0b0c10"
            BgSubtle = "#111318"
            BgRaised = "#16181e"
            Border = "#23262e"
            Text = "#ecedf0"
            TextMuted = "#9ba1ad"
            Heading = "#f7f8fa"
            Primary = "#8b97ff"
            PrimaryContrast = "#0b0c10"
            PrimarySubtle = "#1a1d3a"
            Note = "#8b97ff"
            Tip = "#4cc38a"
            Warning = "#f1a10d"
            Danger = "#ff6b81"
            CodeInlineBg = "#1b1e26"
            CodeInlineBorder = "#2a2e38"
            CodeInlineText = "#e2e4ea"
            Shadow = "0 1px 2px rgb(0 0 0 / 50%), 0 4px 16px rgb(0 0 0 / 35%)"
            ShadowFloating = "0 16px 40px -8px rgb(0 0 0 / 60%), 0 0 0 1px #23262e"
        }
    )
    |> Theme.lightSyntax (fun s ->
        { s with
            Comment = "#6e737e"
            String = "#0f7b5f"
            Number = "#b35900"
            Constant = "#b35900"
            Constructor = "#4f5bd5"
            Property = "#c2410c"
            Escape = "#0e7490"
            Keyword = "#8a3ffc"
            Operator = "#5f6570"
            Function = "#1a5fd0"
            Type = "#4f5bd5"
            Namespace = "#4f5bd5"
            Variable = "#1c2024"
            Parameter = "#1c2024"
            Punctuation = "#5f6570"
            Tag = "#0e7490"
            Attribute = "#c2410c"
            Preprocessor = "#8a3ffc"
            Invalid = "#d1344e"
            Inserted = "#0f7b5f"
            Deleted = "#d1344e"
        }
    )
    |> Theme.darkSyntax (fun s ->
        { s with
            Comment = "#7d838f"
            String = "#7ee2b8"
            Number = "#ffb86b"
            Constant = "#ffb86b"
            Constructor = "#8b97ff"
            Property = "#ff9e64"
            Escape = "#67e8f9"
            Keyword = "#c4a1ff"
            Operator = "#9ba1ad"
            Function = "#79b8ff"
            Type = "#8b97ff"
            Namespace = "#8b97ff"
            Variable = "#ecedf0"
            Parameter = "#ecedf0"
            Punctuation = "#9ba1ad"
            Tag = "#67e8f9"
            Attribute = "#ff9e64"
            Preprocessor = "#c4a1ff"
            Invalid = "#ff6b81"
            Inserted = "#7ee2b8"
            Deleted = "#ff6b81"
        }
    )
    |> Theme.layerAfter "responsive" "brand" (themeCss "brand.css")
    |> Theme.layerAfter "brand" "landing" (themeCss "landing.css")
    |> Theme.css (themeCss "solid-cells.css")
    |> Theme.headExtra
        [
            Feliz.ViewEngine.Html.link
                [
                    Feliz.ViewEngine.prop.rel "preconnect"
                    Feliz.ViewEngine.prop.href "https://fonts.googleapis.com"
                ]
            Feliz.ViewEngine.Html.link
                [
                    Feliz.ViewEngine.prop.rel "preconnect"
                    Feliz.ViewEngine.prop.href "https://fonts.gstatic.com"
                    Feliz.ViewEngine.prop.custom ("crossorigin", "")
                ]
            Feliz.ViewEngine.Html.link
                [
                    Feliz.ViewEngine.prop.rel "stylesheet"
                    Feliz.ViewEngine.prop.href
                        "https://fonts.googleapis.com/css2?family=Geist:wght@400..700&family=Geist+Mono:wght@400..600&display=swap"
                ]
        ]
    |> Theme.footer (
        Feliz.ViewEngine.Html.p
            [
                Feliz.ViewEngine.Html.text "Partas.Solid · MIT licensed · "
                Feliz.ViewEngine.Html.a
                    [
                        Feliz.ViewEngine.prop.href "https://github.com/shayanhabibi/Partas.Solid"
                        Feliz.ViewEngine.prop.text "GitHub"
                    ]
                Feliz.ViewEngine.Html.text " · Built with Nacara"
            ]
    )

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
        |> Option.defaultValue (Path.Combine(__SOURCE_DIRECTORY__, "feed"))
    )
    // Libraries the live examples import. Each change re-runs npm install in .nacara/partas-solid.
    |> SolidExamples.npm "animejs" "4.5.0"
    |> SolidExamples.npm "apexcharts" "7.6.0"
    |> SolidExamples.npm "motion-dom" "13.4.4"
    |> SolidExamples.npm "@neodrag/vanilla" "2.3.1"
    |> SolidExamples.npm "@floating-ui/dom" "1.8.0"
    |> SolidExamples.npm "canvas-confetti" "1.9.4"
    |> SolidExamples.npm "fuse.js" "7.5.0"
    |> SolidExamples.npm "lucide" "1.48.0"
    |> SolidExamples.npm "@tanstack/table-core" "8.21.3"

let site =
    Site.create "Partas.Solid"
    |> Site.baseUrl "/Partas.Solid/"
    |> Site.origin "https://shayanhabibi.github.io"
    |> Site.output "output"
    |> Site.staticFiles "static"
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
