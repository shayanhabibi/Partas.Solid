---
title: ARIA Attributes
---

The ARIA attributes are typed properties on every HTML tag, kept in their own module so they do not crowd the
completion list. `open Partas.Solid.Aria` to use them:

```fsharp solid setup
open Partas.Solid.Aria
```

```fsharp solid jsx
button (ariaLabel = "Close dialog", ariaHasPopup = Attr.AriaHaspopup.Dialog) { "x" }
```

The plugin writes each one under its attribute name: `ariaLabel` becomes `aria-label`. `role` is in the same module,
typed `Attr.AriaRole`.

## Booleans

ARIA's `true`/`false` states are typed `bool`, and the plugin writes them as the strings `"true"` and `"false"`. Solid 2
removes an attribute whose value is `false`, and ARIA reads a missing attribute differently from `"false"`, so
`ariaHidden = false` has to become `aria-hidden="false"`. Toggle the switch and inspect it:

```fsharp solid render=DetailsSwitch jsx
[<SolidComponent>]
let DetailsSwitch () =
    let on, setOn = createSignal false

    button (
        role = Attr.AriaRole.Switch,
        ariaChecked = on (),
        ariaLabel = "Details",
        onClick = fun _ -> setOn (not (on ()))
    ) {
        if on () then "Details: on" else "Details: off"
    }
```

## Types

| Type | Attributes |
| --- | --- |
| `bool` | `ariaAtomic`, `ariaBusy`, `ariaDisabled`, `ariaExpanded`, `ariaGrabbed`, `ariaHidden`, `ariaModal`, `ariaMultiLine`, `ariaMultiSelectable`, `ariaReadOnly`, `ariaRequired`, `ariaSelected` |
| `U2<bool, Attr.Aria*>` | `ariaChecked`, `ariaCurrent`, `ariaHasPopup`, `ariaInvalid`, `ariaPressed`: a `bool`, or one of the other values (`Attr.AriaChecked.Mixed`) |
| `Attr.Aria*` | `role`, `ariaAutoComplete`, `ariaDropeffect`, `ariaLive`, `ariaOrientation`, `ariaRelevant`, `ariaSort` |
| `U2<int, string>` | `ariaColCount`, `ariaColIndex`, `ariaColIndexText`, `ariaColSpan`, `ariaPosInSet`, `ariaRowCount`, `ariaRowIndex`, `ariaRowIndexText` |
| `U2<float, string>` | `ariaLevel`, `ariaRowSpan`, `ariaSetSize`, `ariaValueMax`, `ariaValueMin`, `ariaValueNow` |
| `string` | the rest: `ariaLabel`, `ariaLabelledBy`, `ariaDescribedBy`, `ariaControls`, `ariaOwns`, `ariaValueText` and so on |

The `U2` cases convert implicitly, so `ariaChecked = true`, `ariaChecked = Attr.AriaChecked.Mixed` and
`ariaLevel = 2.0` all compile.

SVG elements do not use this module. Their ARIA attributes are on the elements themselves, as
``` ``aria-hidden`` ``` with a camelCase alias (`ariaHidden`), typed with the `Attr.SvgAria*` unions. See
[SVG elements](svg-elements.md#attributes).
