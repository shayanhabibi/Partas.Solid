// @refresh reload
import { mount, StartClient } from "@solidjs/start/client";
import { attachDevtoolsOverlay } from "@solid-devtools/overlay";
import 'solid-devtools';

attachDevtoolsOverlay();

mount(() => <StartClient />, document.getElementById("app"));
