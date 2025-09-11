
import { createSignal } from "solid-js";
import { toArray } from "../fable_modules/fable-library-js.5.0.0-alpha.14/Seq.js";
import { rangeDouble } from "../fable_modules/fable-library-js.5.0.0-alpha.14/Range.js";
import { SolidApexCharts } from "solid-apexcharts";

function ApexChartExample() {
    const options = createSignal({
        theme: {
            monochrome: {
                enabled: true,
                color: "#1e293b",
                shadeTo: "light",
            },
        },
        tooltip: {
            followCursor: true,
        },
        xaxis: {
            categories: toArray(rangeDouble(1991, 1, 1998)),
        },
    })[0];
    const series = createSignal([{
        data: [30, 40, 35, 50, 49, 60, 70, 91],
        name: "series-1",
    }])[0];
    return <div class="flex w-full items-center justify-center p-8 gap-8">
        <SolidApexCharts width="500"
            type="bar"
            options={options()}
            series={series()} />
        <SolidApexCharts width="500"
            type="line"
            options={options()}
            series={series()} />
    </div>;
}

export default ApexChartExample;

