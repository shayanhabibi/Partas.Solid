import { Select } from "@kobalte/core/select";
import styles from "@kobalte/solidbase/default-theme/components/ThemeSelector.module.css";

// The 3.0 documentation is a separate site, so choosing it leaves this one.
const VERSIONS = [
    { label: "2.x", href: null },
    { label: "3.0", href: "https://shayanhabibi.github.io/Partas.Solid/" },
];

export default function VersionSelector() {
    return (
        <Select
            class={styles.root}
            options={VERSIONS}
            optionValue="label"
            optionTextValue="label"
            value={VERSIONS[0]}
            onChange={(option) => {
                if (option?.href) location.href = option.href;
            }}
            gutter={8}
            sameWidth={false}
            placement="bottom"
            itemComponent={(props) => (
                <Select.Item class={styles.item} item={props.item}>
                    <Select.ItemLabel>{props.item.rawValue.label}</Select.ItemLabel>
                </Select.Item>
            )}
        >
            <Select.Trigger class={styles.trigger} aria-label="Documentation version">
                <Select.Value>{(state) => state.selectedOption().label}</Select.Value>
            </Select.Trigger>
            <Select.Portal>
                <Select.Content class={styles.content}>
                    <Select.Listbox class={styles.list} />
                </Select.Content>
            </Select.Portal>
        </Select>
    );
}
