namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core
open Browser.Types

[<Erase>]
type CheckboxRenderProp =
    abstract checked': Accessor<bool> //v0.13.9
    abstract indeterminate: Accessor<bool> //v0.13.9
/// <summary>
/// 
/// </summary>
/// <param name="data-valid">Present when the checkbox is valid according to validation rules</param>
/// <param name="data-invalid">Present when the checkbox is invalid according to the validation rules</param>
/// <param name="data-required">Present when the checkbox is required</param>
/// <param name="data-disabled">Present when the checkbox is disabled</param>
/// <param name="data-readonly">Present when the checkbox is readonly</param>
/// <param name="data-checked">Present when the checkbox is checked</param>
/// <param name="data-indeterminate">Present when the checkbox is indeterminate</param>
[<Erase; Import("Root", Spec.checkbox)>]
type Checkbox() =
    interface HtmlTag
    interface Polymorph
    interface ChildLambdaProvider<CheckboxRenderProp>
    [<DefaultValue>] val mutable checked' : bool  //v0.13.9
    [<DefaultValue>] val mutable defaultChecked : bool  //v0.13.9
    [<DefaultValue>] val mutable onChange : bool -> unit  //v0.13.9
    [<DefaultValue>] val mutable indeterminate : bool //v0.13.9
    [<DefaultValue>] val mutable name : string  //v0.13.9
    [<DefaultValue>] val mutable value : string  //v0.13.9
    [<DefaultValue>] val mutable validationState : ValidationState  //v0.13.9
    [<DefaultValue>] val mutable required : bool  //v0.13.9
    [<DefaultValue>] val mutable disabled : bool  //v0.13.9
    [<DefaultValue>] val mutable readOnly : bool  //v0.13.9
    [<DefaultValue>] val mutable children : CheckboxRenderProp -> HtmlElement   //v0.13.9

[<Erase; RequireQualifiedAccess>]
module Checkbox = //v0.13.9
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data-valid">Present when the checkbox is valid according to validation rules</param>
    /// <param name="data-invalid">Present when the checkbox is invalid according to the validation rules</param>
    /// <param name="data-required">Present when the checkbox is required</param>
    /// <param name="data-disabled">Present when the checkbox is disabled</param>
    /// <param name="data-readonly">Present when the checkbox is readonly</param>
    /// <param name="data-checked">Present when the checkbox is checked</param>
    /// <param name="data-indeterminate">Present when the checkbox is indeterminate</param>
    [<Erase; Import("Indicator", Spec.checkbox)>]
    type Indicator() = //v0.13.9
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable forceMount : bool  //v0.13.9
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data-valid">Present when the checkbox is valid according to validation rules</param>
    /// <param name="data-invalid">Present when the checkbox is invalid according to the validation rules</param>
    /// <param name="data-required">Present when the checkbox is required</param>
    /// <param name="data-disabled">Present when the checkbox is disabled</param>
    /// <param name="data-readonly">Present when the checkbox is readonly</param>
    /// <param name="data-checked">Present when the checkbox is checked</param>
    /// <param name="data-indeterminate">Present when the checkbox is indeterminate</param>
    [<Erase; Import("ErrorMessage", Spec.checkbox)>]
    type ErrorMessage() = //v0.13.9
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable forceMount : bool  //v0.13.9
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data-valid">Present when the checkbox is valid according to validation rules</param>
    /// <param name="data-invalid">Present when the checkbox is invalid according to the validation rules</param>
    /// <param name="data-required">Present when the checkbox is required</param>
    /// <param name="data-disabled">Present when the checkbox is disabled</param>
    /// <param name="data-readonly">Present when the checkbox is readonly</param>
    /// <param name="data-checked">Present when the checkbox is checked</param>
    /// <param name="data-indeterminate">Present when the checkbox is indeterminate</param>
    [<Erase; Import("Label", Spec.checkbox)>]
    type Label() = //v0.13.9
        inherit label()
        interface Polymorph
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data-valid">Present when the checkbox is valid according to validation rules</param>
    /// <param name="data-invalid">Present when the checkbox is invalid according to the validation rules</param>
    /// <param name="data-required">Present when the checkbox is required</param>
    /// <param name="data-disabled">Present when the checkbox is disabled</param>
    /// <param name="data-readonly">Present when the checkbox is readonly</param>
    /// <param name="data-checked">Present when the checkbox is checked</param>
    /// <param name="data-indeterminate">Present when the checkbox is indeterminate</param>
    [<Erase; Import("Description", Spec.checkbox)>]
    type Description() =
        inherit div() //v0.13.9
        interface Polymorph
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data-valid">Present when the checkbox is valid according to validation rules</param>
    /// <param name="data-invalid">Present when the checkbox is invalid according to the validation rules</param>
    /// <param name="data-required">Present when the checkbox is required</param>
    /// <param name="data-disabled">Present when the checkbox is disabled</param>
    /// <param name="data-readonly">Present when the checkbox is readonly</param>
    /// <param name="data-checked">Present when the checkbox is checked</param>
    /// <param name="data-indeterminate">Present when the checkbox is indeterminate</param>
    [<Erase; Import("Control", Spec.checkbox)>]
    type Control() = //v0.13.9
        inherit div()
        interface Polymorph
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data-valid">Present when the checkbox is valid according to validation rules</param>
    /// <param name="data-invalid">Present when the checkbox is invalid according to the validation rules</param>
    /// <param name="data-required">Present when the checkbox is required</param>
    /// <param name="data-disabled">Present when the checkbox is disabled</param>
    /// <param name="data-readonly">Present when the checkbox is readonly</param>
    /// <param name="data-checked">Present when the checkbox is checked</param>
    /// <param name="data-indeterminate">Present when the checkbox is indeterminate</param>
    [<Erase; Import("Input", Spec.checkbox)>]
    type Input() = //v0.13.9
        inherit input()
        interface Polymorph

[<Erase; AutoOpen>]
module CheckboxContext =
    [<AllowNullLiteral; Interface>]
    type CheckboxContext =
        abstract member value: Accessor<string> with get
        abstract member dataset: Accessor<{|``data-checked``:string option; ``data-indeterminate``: string option|}> with get
        abstract member ``checked``: Accessor<bool> with get
        abstract member indeterminate: Accessor<bool> with get
        abstract member inputRef: Accessor<HTMLInputElement option> with get
        abstract member generateId: (string -> string) with get
        abstract member toggle: (unit -> unit) with get
        abstract member setIsChecked: (bool -> unit) with get
        abstract member setIsFocused: (bool -> unit) with get
        abstract member setInputRef: (HTMLInputElement -> unit) with get
    [<Import("useCheckboxContext", Spec.checkbox)>]
    let useCheckboxContext (): CheckboxContext = jsNative


