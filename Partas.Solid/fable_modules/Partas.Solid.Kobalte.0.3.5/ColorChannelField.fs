namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core


[<Import("Root", Spec.colorChannelField)>]
type ColorChannelField() =
    inherit NumberField()
    interface Polymorph
    /// <summary>
    /// The controlled formatted value of the field.
    /// </summary>
    [<DefaultValue>] val mutable value: Color
    /// <summary>
    /// The default formatted value when initially rendered.
    /// </summary>
    [<DefaultValue>] val mutable defaultValue: Color
    /// <summary>
    /// Event handler called when the value of the field changes.
    /// </summary>
    [<DefaultValue>] val mutable onChange: (Color -> unit)
    /// <summary>
    /// The color channel that the field manipulates.
    /// </summary>
    [<DefaultValue>] val mutable channel: ColorChannel
    /// <summary>
    /// The color space that the field operates in. The <c>channel</c> must be in this color space.
    /// </summary>
    [<DefaultValue>] val mutable colorSpace: ColorSpace
    [<Erase; System.Obsolete("Omitted")>] member val rawValue: unit = ()
    [<Erase; System.Obsolete("Omitted")>] member val onRawValueChange: unit = ()
    [<Erase; System.Obsolete("Omitted")>] member val formatOptions: unit = ()
    [<Erase; System.Obsolete("Omitted")>] member val allowedInput: unit = ()

[<Erase; RequireQualifiedAccess>]
module ColorChannelField =
    [<Import("Description", Spec.colorChannelField)>]
    type Description() =
        inherit div()
        interface Polymorph
    [<Import("ErrorMessage", Spec.colorChannelField)>]
    type ErrorMessage() =
        inherit div()
        interface Polymorph
    [<Import("HiddenInput", Spec.colorChannelField)>]
    type HiddenInput() =
        inherit NumberField.HiddenInput()
        interface Polymorph
    [<Import("Input", Spec.colorChannelField)>]
    type Input() =
        inherit NumberField.Input()
        interface Polymorph
    [<Import("IncrementTrigger", Spec.colorChannelField)>]
    type IncrementTrigger() =
        inherit NumberField.IncrementTrigger()
        interface Polymorph
    [<Import("DecrementTrigger", Spec.colorChannelField)>]
    type DecrementTrigger() =
        inherit NumberField.DecrementTrigger()
        interface Polymorph
    [<Import("Label", Spec.colorChannelField)>]
    type Label() =
        inherit NumberField.Label()
        interface Polymorph
