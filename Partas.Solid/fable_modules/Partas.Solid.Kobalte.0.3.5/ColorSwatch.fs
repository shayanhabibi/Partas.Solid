namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core

[<Import("Root", Spec.colorSwatch)>]
type ColorSwatch() =
    inherit div()
    interface Polymorph
    /// <summary>
    /// The color value to display in the swatch.
    /// </summary>
    [<DefaultValue>] val mutable value: Color
    /// <summary>
    /// A localized accessible name for the color.
    /// By default, a description is generated from the color value,
    /// but this can be overridden if you have a more specific color
    /// name (e.g. Pantone colors).
    /// </summary>
    [<DefaultValue>] val mutable colorName: string
    /// <summary>
    /// The localized strings of the component.
    /// </summary>
    [<DefaultValue>] val mutable translations: obj
