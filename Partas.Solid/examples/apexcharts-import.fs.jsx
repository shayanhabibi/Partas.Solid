
import { clientOnly } from "@solidjs/start";

const ClientOnly = clientOnly(() => (import("./apexcharts.fs.jsx")));

export function ApexChartEg() {
    return <ClientOnly />;
}

