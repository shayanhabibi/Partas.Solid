namespace Partas.Solid.Kobalte

open Fable.Core
open Partas.Solid

[<Import("Root", Spec.colorSlider)>]
type ColorSlider() =
    inherit div()
    interface Polymorph
    /// <summary>
    /// The controlled values of the slider.
    /// </summary>
    [<DefaultValue>] val mutable value: Color
    /// <summary>
    /// The value of the slider when initially rendered.
    /// </summary>
    [<DefaultValue>] val mutable defaultValue: Color
    /// <summary>
    /// Called when the value changes.
    /// </summary>
    [<DefaultValue>] val mutable onChange: (Color -> unit)
    /// <summary>
    /// Called when the value changes at the end of an interaction.
    /// </summary>
    [<DefaultValue>] val mutable onChangeEnd: (Color -> unit)
    /// <summary>
    /// The color channel that the slider manipulates.
    /// </summary>
    [<DefaultValue>] val mutable channel: ColorChannel
    /// <summary>
    /// The color space that the slider operates in. The <c>channel</c> must be in this color space.
    /// </summary>
    [<DefaultValue>] val mutable colorSpace: ColorSpace
    /// <summary>
    /// The orientation of the slider.
    /// </summary>
    [<DefaultValue>] val mutable orientation: Enums.Orientation
    /// <summary>
    /// A function to get the accessible label text representing the current value in a human-readable format.
    /// </summary>
    [<DefaultValue>] val mutable getValueLabel: (Color -> string)
    /// <summary>
    /// A unique identifier for the component.
    /// The id is used to generate id attributes for nested components.
    /// If no id prop is provided, a generated id will be used.
    /// </summary>
    [<DefaultValue>] val mutable id: string
    /// <summary>
    /// The name of the slider, used when submitting an HTML form.
    /// See <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input#htmlattrdefname">MDN</a>.
    /// </summary>
    [<DefaultValue>] val mutable name: string
    /// <summary>
    /// Whether the slider should display its "valid" or "invalid" visual styling.
    /// </summary>
    [<DefaultValue>] val mutable validationState: ValidationState
    /// <summary>
    /// Whether the user must fill the slider before the owning form can be submitted.
    /// </summary>
    [<DefaultValue>] val mutable required: bool
    /// <summary>
    /// Whether the slider is disabled.
    /// </summary>
    [<DefaultValue>] val mutable disabled: bool
    /// <summary>
    /// Whether the slider is read only.
    /// </summary>
    [<DefaultValue>] val mutable readOnly: bool
    /// <summary>
    /// The localized strings of the component.
    /// </summary>
    [<DefaultValue>] val mutable translations: obj

[<Erase; RequireQualifiedAccess>]
module ColorSlider =
    [<Import("Description", Spec.colorSlider)>]
    type Description() =
        inherit ColorArea.Description()
        interface Polymorph
    [<Import("ErrorMessage", Spec.colorSlider)>]
    type ErrorMessage() =
        inherit ColorArea.ErrorMessage()
        interface Polymorph
    [<Import("Input", Spec.colorSlider)>]
    type Input() =
        inherit Slider.Input()
        interface Polymorph
    [<Import("Label", Spec.colorSlider)>]
    type Label() =
        inherit ColorArea.Label()
        interface Polymorph
    ///<inheritdoc cref="Partas.Solid.Kobalte.ColorAreaModule.Thumb"/>
    [<Import("Thumb", Spec.colorSlider)>]
    type Thumb() =
        inherit ColorArea.Thumb()
        interface Polymorph
    [<Import("Track", Spec.colorSlider)>]
    type Track() =
        inherit div()
        interface Polymorph
    [<Import("ValueLabel", Spec.colorSlider)>]
    type ValueLabel() =
        inherit Slider.ValueLabel()
        interface Polymorph
        
