---
title: SVG Elements
---

The SVG elements live in the `Svg` module, so names such as `set`, `path`, `text` and `filter` do not shadow F#
functions (`set`) or your own bindings when you open `Partas.Solid`. Use them qualified, as below, or
`open Partas.Solid.Svg` in a file that is mostly SVG. Click the target:

```fsharp solid render=Target jsx
[<SolidComponent>]
let Target () =
    let big, setBig = createSignal false
    let radius () = if big () then 11.0 else 7.0

    Svg.svg (
        viewBox = "0 0 24 24",
        width = 96.0,
        height = "96",
        fill = "none",
        onClick = fun _ -> setBig (not (big ()))
    ) {
        Svg.circle (cx = 12.0, cy = 12.0, r = radius (), stroke = "currentColor", ``stroke-width`` = 1.5)
        Svg.path (
            d = "M12 2V22M2 12H22",
            stroke = "crimson",
            strokeWidth = 1.0,
            strokeLinecap = Attr.SvgStrokeLinecap.Round
        )
        Svg.text (x = 1.0, y = 23.0, fill = "currentColor") { "click" }
    }
```

## Attributes

The attributes are typed properties, like the HTML ones:

- Length and number attributes, such as `cx`, `r`, `width` and `stroke-width`, take a `U2<float, string>`, so you can
  pass `12.0` or `"12"`. This `U2` converts implicitly from either case (see
  [Erased union implicit casting](experimental.md#erased-union-implicit-casting)).
- An attribute with a fixed set of values takes a union from `Attr` whose name starts with `Svg`:
  `strokeLinecap = Attr.SvgStrokeLinecap.Round`.
- Hyphenated names are written in double backticks, ``` ``stroke-width`` ```, and have a camelCase alias that sets the
  same attribute: `strokeWidth`. The ARIA attributes are among them: ``` ``aria-hidden`` ``` and `ariaHidden`.

For anything the properties do not cover, use [`.attr`](extension-methods.md#attr-data-and-bool). For example, the
old `xlink:href`:

```fsharp
Svg.use'().attr("xlink:href", "#icon")
```

## Components

An SVG element can be the root of its own component, and be rendered inside an `svg` in another:

```fsharp solid
[<SolidComponent>]
let Dot (x: float) =
    Svg.circle (cx = x, cy = 5.0, r = 4.0, fill = "teal")
```

```fsharp solid jsx
Svg.svg (viewBox = "0 0 40 10", width = 160.0) {
    Dot 5.0
    Dot 20.0
    Dot 35.0
}
```

## Elements

`animate`, `animateMotion`, `animateTransform`, `circle`, `clipPath`, `defs`, `desc`, `ellipse`, `feBlend`,
`feColorMatrix`, `feComponentTransfer`, `feComposite`, `feConvolveMatrix`, `feDiffuseLighting`, `feDisplacementMap`,
`feDistantLight`, `feDropShadow`, `feFlood`, `feFuncA`, `feFuncB`, `feFuncG`, `feFuncR`, `feGaussianBlur`, `feImage`,
`feMerge`, `feMergeNode`, `feMorphology`, `feOffset`, `fePointLight`, `feSpecularLighting`, `feSpotLight`, `feTile`,
`feTurbulence`, `filter`, `foreignObject`, `g`, `image`, `line`, `linearGradient`, `marker`, `mask`, `metadata`,
`mpath`, `path`, `pattern`, `polygon`, `polyline`, `radialGradient`, `rect`, `set`, `stop`, `svg`, `switch`,
`symbol`, `text`, `textPath`, `tspan`, `use'` (`use` is an F# keyword), `view`.

MathML elements are in the `MathML` module the same way: `MathML.math () { MathML.mi () { "x" } }`.
