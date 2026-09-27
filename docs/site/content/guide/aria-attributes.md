---
title: ARIA Attributes
---

The ARIA attributes are typed properties on every tag, like the other HTML attributes. They are kept in their own
module so they do not crowd the completion list. Open it to use them:

```fsharp
open Partas.Solid.Aria

button (ariaLabel = "Close dialog", ariaControls = "menu-1") { "x" }
```

The bindings come from [Oxpecker.Solid](https://github.com/Lanayx/Oxpecker), by Lanayx.

The plugin writes each one under its attribute name, so `ariaLabel = "Close"` becomes `aria-label="Close"`. `role`
is in the same module.

```fsharp solid render=DetailsSwitch jsx
open Partas.Solid.Aria

[<SolidComponent>]
let DetailsSwitch () =
    let on, setOn = createSignal false

    button (
        role = "switch",
        ariaChecked = string (on ()),
        ariaLabel = "Details",
        onClick = fun _ -> setOn (not (on ()))
    ) {
        if on () then "Details: on" else "Details: off"
    }
```

## Boolean attributes

`ariaDisabled`, `ariaHidden`, `ariaModal`, `ariaMultiLine` and `ariaMultiSelectable` are typed `bool`. The plugin
writes them as the strings `"true"` and `"false"`, so `ariaHidden = false` becomes `aria-hidden="false"`. This matters
because Solid 2 removes an attribute whose value is `false`, and ARIA reads a missing attribute differently from
`"false"`. The other states, such as `ariaExpanded` and `ariaChecked`, are typed `string`: pass `"true"` or `"false"`.

## Attributes

| Property | Type |
| --- | --- |
| `role` | `string` |
| `ariaActiveDescendant` | `string` |
| `ariaAtomic` | `string` |
| `ariaAutoComplete` | `string` |
| `ariaBrailleLabel` | `string` |
| `ariaBrailleRoleDescription` | `string` |
| `ariaBusy` | `string` |
| `ariaChecked` | `string` |
| `ariaColCount` | `U2<int, string>` |
| `ariaColIndex` | `U2<int, string>` |
| `ariaColSpan` | `U2<int, string>` |
| `ariaControls` | `string` |
| `ariaCurrent` | `string` |
| `ariaDescendant` | `string` |
| `ariaDescribedBy` | `string` |
| `ariaDescription` | `string` |
| `ariaDetails` | `string` |
| `ariaDisabled` | `bool` |
| `ariaDropeffect` | `string` |
| `ariaErrorMessage` | `string` |
| `ariaExpanded` | `string` |
| `ariaFlowTo` | `string` |
| `ariaGrabbed` | `string` |
| `ariaHasPopup` | `string` |
| `ariaHidden` | `bool` |
| `ariaInvalid` | `string` |
| `ariaKeyShortcuts` | `string` |
| `ariaLabel` | `string` |
| `ariaLabelledBy` | `string` |
| `ariaLevel` | `U2<float, string>` |
| `ariaLive` | `string` |
| `ariaModal` | `bool` |
| `ariaMultiLine` | `bool` |
| `ariaMultiSelectable` | `bool` |
| `ariaOrientation` | `string` |
| `ariaOwns` | `string` |
| `ariaPlaceholder` | `string` |
| `ariaPosInSet` | `U2<int, string>` |
| `ariaPressed` | `string` |
| `ariaReadOnly` | `string` |
| `ariaRelevant` | `string` |
| `ariaRequired` | `string` |
| `ariaRoleDescription` | `string` |
| `ariaRowCount` | `U2<int, string>` |
| `ariaRowIndex` | `U2<int, string>` |
| `ariaRowSpan` | `U2<float, string>` |
| `ariaSelected` | `string` |
| `ariaSetSize` | `U2<float, string>` |
| `ariaSort` | `string` |
| `ariaValueMax` | `U2<float, string>` |
| `ariaValueMin` | `U2<float, string>` |
| `ariaValueNow` | `U2<float, string>` |
| `ariaValueText` | `string` |
