import DefaultLayout from "@kobalte/solidbase/default-theme/Layout.jsx";
import { DefaultThemeComponentsProvider } from "@kobalte/solidbase/default-theme/context.jsx";
import VersionSelector from "./VersionSelector";

// The site has one locale, so the header's LocaleSelector slot is empty. The version
// switcher takes it, which puts it beside the theme toggle on desktop and in the mobile menu.
export default (props) => (
    <DefaultThemeComponentsProvider components={{ LocaleSelector: VersionSelector }}>
        <DefaultLayout>{props.children}</DefaultLayout>
    </DefaultThemeComponentsProvider>
);
