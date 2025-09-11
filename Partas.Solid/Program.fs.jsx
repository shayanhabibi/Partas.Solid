
import { Router } from "@solidjs/router";
import { FileRoutes } from "@solidjs/start/router";
import { SolidBaseRoot } from "@kobalte/solidbase/client";

export function Program() {
    return <Router root={SolidBaseRoot}>
        <FileRoutes />
    </Router>;
}

