---
title: ApexCharts
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

`Partas.Solid.ApexCharts` binds [solid-apexcharts](https://github.com/wobsoriano/solid-apexcharts). See its docs for
usage and options.

The chart options are built with Pojo constructors. [Writing bindings](bindings.md) shows what those compile to.

## Example

```fsharp
[<SolidComponent>]
let ApexChartExample () =
    let options,_ = createSignal(Options(
        theme = Theme(
            monochrome = Theme.Monochrome(
                enabled = true, color = "#1e293b",
                shadeTo = Enums.Theme.Monochrome.ShadeTo.Light
                )
            ),
        tooltip = Tooltip(followCursor = true),
        xaxis = XAxis(categories = [| 1991 .. 1998 |])
    ))
    let chartSeries = AxisChartSeries(
        name = "series-1",
        data = !^[|30;40;35;50;49;60;70;91|]
        )
    let series,_ = createSignal([|chartSeries|])
    div(class' = "flex w-full items-center justify-center p-8 gap-8") {
        SolidApexCharts(
            width = "500",
            type' = Chart.Type.Bar,
            options = options(),
            series = series()
        )
        SolidApexCharts(
            width = "500",
            type' = Chart.Type.Line,
            options = options(),
            series = series()
        )
    }
```

This renders a bar chart and a line chart of the same series, side by side, in a monochrome theme.

:::note
ApexCharts itself does not depend on Solid; only the `solid-apexcharts` wrapper does. A Solid 2 port could bind
`apexcharts` directly, as the next section does.
:::

## Binding apexcharts directly on Solid 2

`apexcharts` is plain JavaScript, so you can use it on Solid 2 without a wrapper. Declare the class with
`[<Import("default", "apexcharts")>]` and give it only the members you call. `MutationObserver` is a browser global,
so it uses `[<Global>]` instead. Create the chart through a plain function such as `newChart`: inside a component,
the plugin treats a call to an imported constructor as a JSX tag. Options are anonymous records, which Fable compiles
to plain objects.

```fsharp solid
[<Import("default", "apexcharts")>]
type ApexCharts(element: Browser.Types.HTMLElement, options: obj) =
    member _.render(): JS.Promise<unit> = jsNative
    member _.updateSeries(series: obj): JS.Promise<unit> = jsNative
    member _.destroy(): unit = jsNative

/// Inside a component, the plugin reads a call to an imported constructor as a JSX tag,
/// so construct the chart from a plain function.
let newChart (element: Browser.Types.HTMLElement, options: obj) =
    ApexCharts(element, options)

[<Global>]
type MutationObserver(callback: obj -> unit) =
    member _.observe(target: Browser.Types.Node, options: obj): unit = jsNative
    member _.disconnect(): unit = jsNative

/// "light" or "dark", read from the attribute the site's theme switch sets.
let siteScheme () =
    match Browser.Dom.document.documentElement.getAttribute "data-theme" with
    | "dark" -> "dark"
    | _ -> "light"
```

Effects run after the first render, so the host `div` exists when the first effect builds the chart. That effect
tracks the theme: the cleanup it returns destroys the chart, and the effect builds a new one in the other scheme.
The cleanup also runs when the component is disposed. A second, deferred effect sends new data to `updateSeries`,
which animates the bars. The `--apx-*` variables on the host are ApexCharts design tokens that point at the site's
colours, so the bars use the accent colour in both themes.

```fsharp solid render=DownloadsChart jsx
[<SolidComponent>]
let DownloadsChart () =
    let data, setData = createSignal [| 42; 58; 51; 73; 66; 88; 97 |]
    let total = createMemo (fun (_: int option) -> Array.sum (data ()))
    let scheme, setScheme = createSignal (siteScheme ())
    let mutable host: Browser.Types.HTMLDivElement = JS.undefined
    let mutable chart: ApexCharts option = None
    let chartTokens =
        "--apx-accent: var(--nacara-primary); --apx-fore: var(--nacara-text-muted);"
        + " --apx-grid: var(--nacara-border)"

    onSettled (fun () ->
        let observer = MutationObserver(fun _ -> setScheme (siteScheme ()))
        observer.observe (
            Browser.Dom.document.documentElement,
            {| attributes = true; attributeFilter = [| "data-theme" |] |}
        )
        fun () -> observer.disconnect ())

    createEffect (
        (fun (_: string option) -> scheme ()),
        fun (mode: string) ->
            let c =
                newChart (
                    host,
                    {| chart =
                        {| ``type`` = "bar"; height = 240; background = "transparent"
                           fontFamily = "inherit"; toolbar = {| show = false |} |}
                       theme = {| mode = mode |}
                       series = [| {| name = "Downloads"; data = data () |} |]
                       xaxis = {| categories = [| "Mon"; "Tue"; "Wed"; "Thu"; "Fri"; "Sat"; "Sun" |] |}
                       plotOptions = {| bar = {| borderRadius = 4; columnWidth = "50%" |} |}
                       dataLabels = {| enabled = false |}
                       grid = {| strokeDashArray = 4 |} |}
                )
            c.render () |> ignore
            chart <- Some c
            fun () -> c.destroy ()
    )

    createEffect (
        (fun (_: int array option) -> data ()),
        (fun (d: int array) ->
            chart
            |> Option.iter (fun c -> c.updateSeries [| {| name = "Downloads"; data = d |} |] |> ignore)),
        defer = true
    )

    div (style = "width: 100%; display: grid; gap: .25rem") {
        div (style = "display: flex; align-items: end; justify-content: space-between") {
            div () {
                div (style = "font-size: .8125rem; color: var(--nacara-text-muted)") { "Downloads this week" }
                div (style = "font-size: 1.75rem; font-weight: 600; line-height: 1.2") {
                    $"{total ()}k"
                }
            }
            button (
                class' = "p-btn p-btn--secondary",
                onClick = fun _ -> setData (Array.init 7 (fun _ -> 20 + int (JS.Math.random () * 80.)))
            ) { "Randomise" }
        }
        div(style = chartTokens).ref (host)
    }
```

Click **Randomise** to animate the bars to new values, or switch the site theme to see the chart follow it.
