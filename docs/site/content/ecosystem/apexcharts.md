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
`apexcharts` directly.
:::
