namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core
open Browser.Types

/// <summary>
/// <param name="data-valid">Present when the slider is valid according to the validation rules.</param>
/// <param name="data-invalid">Present when the slider is invalid according to the validation rules.</param>
/// <param name="data-required">Present when the user must slider an item before the owning form can be submitted.</param>
/// <param name="data-disabled">Present when the slider is disabled.</param>
/// <param name="data-readonly">Present when the slider is read only.</param>
/// </summary>
[<Import("Root", Spec.colorArea)>]
type ColorArea() =
    inherit div()
    interface Polymorph
    /// <summary>
    /// The localized strings of the component.
    /// </summary>
    [<DefaultValue>] val mutable translations: obj
    /// <summary>
    /// The controlled value of the color area.
    /// </summary>
    [<DefaultValue>] val mutable value: Color
    /// <summary>
    /// The value of the color area when initially rendered.
    /// </summary>
    [<DefaultValue>] val mutable defaultValue: Color
    /// <summary>
    /// Event handler called when the value changes.
    /// </summary>
    [<DefaultValue>] val mutable onChange: (Color -> unit)
    /// <summary>
    /// Called when the value changes at the end of an interaction.
    /// </summary>
    [<DefaultValue>] val mutable onChangeEnd: (Color -> unit)
    /// <summary>
    /// Color channel for the horizontal axis.
    /// </summary>
    [<DefaultValue>] val mutable xChannel: ColorChannel
    /// <summary>
    /// Color channel for the vertical axis.
    /// </summary>
    [<DefaultValue>] val mutable yChannel: ColorChannel
    /// <summary>
    /// The color space that the color area operates in. The <c>xChannel</c> and <c>yChannel</c> must be in this color space.
    /// </summary>
    [<DefaultValue>] val mutable colorSpace: ColorSpace
    /// <summary>
    /// A unique identifier for the component.
    /// The id is used to generate id attributes for nested components.
    /// If no id prop is provided, a generated id will be used.
    /// </summary>
    [<DefaultValue>] val mutable id: string
    /// <summary>
    /// The name of the color area, used when submitting an HTML form.
    /// See <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input#htmlattrdefname">MDN</a>.
    /// </summary>
    [<DefaultValue>] val mutable name: string
    /// <summary>
    /// The name of the x channel input element, used when submitting an HTML form. See [MDN](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input#htmlattrdefname).
    /// </summary>
    [<DefaultValue>] val mutable xName: string
    /// <summary>
    /// The name of the y channel input element, used when submitting an HTML form. See [MDN](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input#htmlattrdefname).
    /// </summary>
    [<DefaultValue>] val mutable yName: string
    /// <summary>
    /// Whether the color area should display its "valid" or "invalid" visual styling.
    /// </summary>
    [<DefaultValue>] val mutable validationState: ValidationState
    /// <summary>
    /// Whether the user must select an item before the owning form can be submitted.
    /// </summary>
    [<DefaultValue>] val mutable required: bool
    /// <summary>
    /// Whether the color area is disabled.
    /// </summary>
    [<DefaultValue>] val mutable disabled: bool
    /// <summary>
    /// Whether the color area is read only.
    /// </summary>
    [<DefaultValue>] val mutable readOnly: bool

[<Erase; RequireQualifiedAccess>]
module ColorArea =
    /// <summary>
    /// <param name="data-valid">Present when the slider is valid according to the validation rules.</param>
    /// <param name="data-invalid">Present when the slider is invalid according to the validation rules.</param>
    /// <param name="data-required">Present when the user must slider an item before the owning form can be submitted.</param>
    /// <param name="data-disabled">Present when the slider is disabled.</param>
    /// <param name="data-readonly">Present when the slider is read only.</param>
    /// </summary>
    [<Import("Background", Spec.colorArea)>]
    type Background() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable onPointerDown: PointerEvent -> unit
        [<DefaultValue>] val mutable onPointerMove: PointerEvent -> unit
        [<DefaultValue>] val mutable onPointerUp: PointerEvent -> unit

    /// <summary>
    /// The current color is available on the thumb using the custom css property <c>--kb-color-current</c>
    /// <param name="data-valid">Present when the slider is valid according to the validation rules.</param>
    /// <param name="data-invalid">Present when the slider is invalid according to the validation rules.</param>
    /// <param name="data-required">Present when the user must slider an item before the owning form can be submitted.</param>
    /// <param name="data-disabled">Present when the slider is disabled.</param>
    /// <param name="data-readonly">Present when the slider is read only.</param>
    /// </summary>
    [<Import("Thumb", Spec.colorArea)>]
    type Thumb() =
        inherit span()
        interface Polymorph
        [<DefaultValue>] val mutable onPointerDown: PointerEvent -> unit
        [<DefaultValue>] val mutable onPointerMove: PointerEvent -> unit
        [<DefaultValue>] val mutable onPointerUp: PointerEvent -> unit
        [<DefaultValue>] val mutable onKeyDown: KeyboardEvent -> unit
    
    /// <summary>
    /// <param name="data-valid">Present when the slider is valid according to the validation rules.</param>
    /// <param name="data-invalid">Present when the slider is invalid according to the validation rules.</param>
    /// <param name="data-required">Present when the user must slider an item before the owning form can be submitted.</param>
    /// <param name="data-disabled">Present when the slider is disabled.</param>
    /// <param name="data-readonly">Present when the slider is read only.</param>
    /// <param name="data-orientation='horizontal'">Always present</param>
    /// </summary>
    [<Import("HiddenInputX", Spec.colorArea)>]
    type HiddenInputX() =
        inherit input()
        interface Polymorph
    /// <summary>
    /// <param name="data-valid">Present when the slider is valid according to the validation rules.</param>
    /// <param name="data-invalid">Present when the slider is invalid according to the validation rules.</param>
    /// <param name="data-required">Present when the user must slider an item before the owning form can be submitted.</param>
    /// <param name="data-disabled">Present when the slider is disabled.</param>
    /// <param name="data-readonly">Present when the slider is read only.</param>
    /// <param name="data-orientation='vertical'">Always present</param>
    /// </summary>
    [<Import("HiddenInputY", Spec.colorArea)>]
    type HiddenInputY() =
        inherit input()
        interface Polymorph
    /// <summary>
    /// <param name="data-valid">Present when the slider is valid according to the validation rules.</param>
    /// <param name="data-invalid">Present when the slider is invalid according to the validation rules.</param>
    /// <param name="data-required">Present when the user must slider an item before the owning form can be submitted.</param>
    /// <param name="data-disabled">Present when the slider is disabled.</param>
    /// <param name="data-readonly">Present when the slider is read only.</param>
    /// </summary>
    [<Import("Label", Spec.colorArea)>]
    type Label() =
        inherit label()
        interface Polymorph
    [<Import("Description", Spec.colorArea)>]
    type Description() =
        inherit div()
        interface Polymorph
    [<Import("ErrorMessage", Spec.colorArea)>]
    type ErrorMessage() =
        inherit div()
        interface Polymorph
    

[<Erase; AutoOpen>]
module ColorAreaContext =
    [<AllowNullLiteral>]
    [<Interface>]
    type ColorAreaState =
        abstract member value: Accessor<Color> with get
        abstract member setValue: (Color -> unit) with get, set
        abstract member xValue: Accessor<float> with get, set
        abstract member yValue: Accessor<float> with get, set
        abstract member setXValue: (float -> unit) with get, set
        abstract member setYValue: (float -> unit) with get, set
        abstract member xStep: Accessor<float> with get, set
        abstract member yStep: Accessor<float> with get, set
        abstract member xPageSize: Accessor<float> with get, set
        abstract member yPageSize: Accessor<float> with get, set
        abstract member xMaxValue: Accessor<float> with get, set
        abstract member yMaxValue: Accessor<float> with get, set
        abstract member xMinValue: Accessor<float> with get, set
        abstract member yMinValue: Accessor<float> with get, set
        abstract member incrementX: (float -> unit) with get, set
        abstract member incrementY: (float -> unit) with get, set
        abstract member decrementX: (float -> unit) with get, set
        abstract member decrementY: (float -> unit) with get, set
        abstract member getThumbPosition: (unit -> PosXY) with get, set
        abstract member isDragging: Accessor<bool> with get
        abstract member setIsDragging: (bool -> unit) with get, set
        abstract member channels: Accessor<XyzChannels> with get, set
        abstract member resetValue: (unit -> unit) with get, set
        abstract member getThumbPercent: (unit -> PosXY) with get, set
        abstract member setThumbPercent: (PosXY -> unit) with get, set
        abstract member setThumbValue: (PosXY -> unit) with get, set
        abstract member isDisabled: Accessor<bool> with get

    [<AllowNullLiteral>]
    [<Interface>]
    type DragDelta =
        abstract member deltaX: float with get
        abstract member deltaY: float with get

    [<AllowNullLiteral>]
    [<Interface>]
    type ColorAreaContext =
        abstract member state: ColorAreaState with get, set
        abstract member xName: Accessor<string option> with get, set
        abstract member yName: Accessor<string option> with get, set
        abstract member onDragStart: (float array -> unit) with get, set
        abstract member onDrag: (DragDelta -> unit) with get, set
        abstract member onDragEnd: (unit -> unit) with get, set
        abstract member translations: Accessor<obj> with get, set
        abstract member getDisplayColor: (unit -> Color) with get, set
        abstract member onStepKeyDown: (KeyboardEvent -> unit) with get, set
        abstract member thumbRef: Accessor<HTMLElement option> with get, set
        abstract member setThumbRef: (HTMLElement -> unit) with get, set
        abstract member backgroundRef: Accessor<HTMLElement option> with get, set
        abstract member setBackgroundRef: (HTMLElement -> unit) with get, set
        abstract member generateId: (string -> string) with get, set

    [<Import("useColorAreaContext", Spec.colorArea)>]
    let useColorAreaContext(): ColorAreaContext = jsNative
