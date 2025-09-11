namespace Partas.Solid

open Fable.Core
open Fable.Core.JS
open JetBrains.Annotations

module Aria =
    type HtmlTag with
        // aria attributes
        [<Erase>]
        member this.ariaActiveDescendant
            with set (_: string) = ()
            and get (): string = undefined

        [<Erase>]
        member this.ariaBrailleLabel
            with set (_: string) = ()
            and get (): string = undefined

        [<Erase>]
        member this.ariaBrailleRoleDescription
            with set (_: string) = ()
            and get (): string = undefined

        [<Erase>]
        member this.ariaDescription
            with set (_: string) = ()
            and get (): string = undefined

        /// <summary>
        /// Identifies the currently active element when DOM focus is on a composite widget, textbox,
        /// group, or application.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-descendant='", Suffix = "'>")>]
        member _.ariaDescendant
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Indicates whether assistive technologies will present all, or only parts of, the changed
        /// region based on the change notifications defined by the aria-relevant attribute.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-atomic='", Suffix = "'>")>]
        member _.ariaAtomic
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Indicates whether inputting text could trigger display of one or more predictions of the
        /// user's intended value for an input and specifies how predictions would be presented if they
        /// are made.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-autocomplete='", Suffix = "'>")>]
        member _.ariaAutoComplete
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Indicates an element is being modified and that assistive technologies MAY want to wait until
        /// the modifications are complete before exposing them to the user.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-busy='", Suffix = "'>")>]
        member _.ariaBusy
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Indicates the current "checked" state of checkboxes, radio buttons, and other widgets.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-checked='", Suffix = "'>")>]
        member _.ariaChecked
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Defines the total number of columns in a table, grid, or treegrid.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-colcount='", Suffix = "'>")>]
        member _.ariaColCount
            with set (_: U2<int, string>) = ()
            and [<Erase>] get (): U2<int, string> = unbox ()

        /// <summary>
        /// Defines an element's column index or position with respect to the total number of columns
        /// within a table, grid, or treegrid.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-colindex='", Suffix = "'>")>]
        member _.ariaColIndex
            with set (_: U2<int, string>) = ()
            and [<Erase>] get (): U2<int, string> = unbox ()

        /// <summary>
        /// Defines the number of columns spanned by a cell or gridcell within a table, grid, or
        /// treegrid.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-colspan='", Suffix = "'>")>]
        member _.ariaColSpan
            with set (_: U2<int, string>) = ()
            and [<Erase>] get (): U2<int, string> = unbox ()

        /// <summary>
        /// Identifies the element (or elements) whose contents or presence are controlled by the current
        /// element.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-controls='", Suffix = "'>")>]
        member _.ariaControls
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Indicates the element that represents the current item within a container or set of related
        /// elements.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-current='", Suffix = "'>")>]
        member _.ariaCurrent
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Identifies the element (or elements) that describes the object.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-describedby='", Suffix = "'>")>]
        member _.ariaDescribedBy
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Identifies the element that provides a detailed, extended description for the object.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-details='", Suffix = "'>")>]
        member _.ariaDetails
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Indicates that the element is perceivable but disabled, so it is not editable or otherwise
        /// operable.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-disabled='", Suffix = "'>")>]
        member _.ariaDisabled
            with set (_: bool) = ()
            and [<Erase>] get (): bool = unbox ()

        /// <summary>
        /// Indicates what functions can be performed when a dragged object is released on the drop
        /// target.
        /// </summary>
        [<System.Obsolete("In ARIA 1.1")>]
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-dropeffect='", Suffix = "'>")>]
        member _.ariaDropeffect
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Identifies the element that provides an error message for the object.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-errormessage='", Suffix = "'>")>]
        member _.ariaErrorMessage
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Indicates whether the element, or another grouping element it controls, is currently expanded
        /// or collapsed.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-expanded='", Suffix = "'>")>]
        member _.ariaExpanded
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Identifies the next element (or elements) in an alternate reading order of content which, at
        /// the user's discretion, allows assistive technology to override the general default of reading
        /// in document source order.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-flowto='", Suffix = "'>")>]
        member _.ariaFlowTo
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Indicates an element's "grabbed" state in a drag-and-drop operation.
        /// </summary>
        [<System.Obsolete("In ARIA 1.1")>]
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-grabbed='", Suffix = "'>")>]
        member _.ariaGrabbed
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Indicates the availability and type of interactive popup element, such as menu or dialog,
        /// that can be triggered by an element.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-haspopup='", Suffix = "'>")>]
        member _.ariaHasPopup
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Indicates whether the element is exposed to an accessibility API.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-hidden='", Suffix = "'>")>]
        member _.ariaHidden
            with set (_: bool) = ()
            and [<Erase>] get (): bool = unbox ()

        /// <summary>
        /// Indicates the entered value does not conform to the format expected by the application.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-invalid='", Suffix = "'>")>]
        member _.ariaInvalid
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Indicates keyboard shortcuts that an author has implemented to activate or give focus to an
        /// element.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-keyshortcuts='", Suffix = "'>")>]
        member _.ariaKeyShortcuts
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Defines a string value that labels the current element.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-label='", Suffix = "'>")>]
        member _.ariaLabel
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Identifies the element (or elements) that labels the current element.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-labelledby='", Suffix = "'>")>]
        member _.ariaLabelledBy
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Defines the hierarchical level of an element within a structure.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-level='", Suffix = "'>")>]
        member _.ariaLevel
            with set (_: U2<float, string>) = ()
            and [<Erase>] get (): U2<float, string> = unbox ()

        /// <summary>
        /// Indicates that an element will be updated, and describes the types of updates the user
        /// agents, assistive technologies, and user can expect from the live region.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-live='", Suffix = "'>")>]
        member _.ariaLive
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Indicates whether an element is modal when displayed.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-modal='", Suffix = "'>")>]
        member _.ariaModal
            with set (_: bool) = ()
            and [<Erase>] get (): bool = unbox ()

        /// <summary>
        /// Indicates whether a text box accepts multiple lines of input or only a single line.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-multiline='", Suffix = "'>")>]
        member _.ariaMultiLine
            with set (_: bool) = ()
            and [<Erase>] get (): bool = unbox ()

        /// <summary>
        /// Indicates that the user may select more than one item from the current selectable
        /// descendants.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-multiselectable='", Suffix = "'>")>]
        member _.ariaMultiSelectable
            with set (_: bool) = ()
            and [<Erase>] get (): bool = unbox ()

        /// <summary>
        /// Indicates whether the element's orientation is horizontal, vertical, or unknown/ambiguous.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-orientation='", Suffix = "'>")>]
        member _.ariaOrientation
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Identifies an element (or elements) in order to define a visual, functional, or contextual
        /// parent/child relationship between DOM elements where the DOM hierarchy cannot be used to
        /// represent the relationship.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-owns='", Suffix = "'>")>]
        member _.ariaOwns
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Defines a short hint (a word or short phrase) intended to aid the user with data entry when
        /// the control has no value. A hint could be a sample value or a brief description of the
        /// expected format.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-placeholder='", Suffix = "'>")>]
        member _.ariaPlaceholder
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Defines an element's number or position in the current set of listitems or treeitems. Not
        /// required if all elements in the set are present in the DOM.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-posinset='", Suffix = "'>")>]
        member _.ariaPosInSet
            with set (_: U2<int, string>) = ()
            and [<Erase>] get (): U2<int, string> = unbox ()

        /// <summary>
        /// Indicates the current "pressed" state of toggle buttons.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-pressed='", Suffix = "'>")>]
        member _.ariaPressed
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Indicates that the element is not editable, but is otherwise operable.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-readonly='", Suffix = "'>")>]
        member _.ariaReadOnly
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Indicates what notifications the user agent will trigger when the accessibility tree within a
        /// live region is modified.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-relevant='", Suffix = "'>")>]
        member _.ariaRelevant
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Indicates that user input is required on the element before a form may be submitted.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-required='", Suffix = "'>")>]
        member _.ariaRequired
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Defines a human-readable, author-localized description for the role of an element.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-roledescription='", Suffix = "'>")>]
        member _.ariaRoleDescription
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Defines the total number of rows in a table, grid, or treegrid.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-rowcount='", Suffix = "'>")>]
        member _.ariaRowCount
            with set (_: U2<int, string>) = ()
            and [<Erase>] get (): U2<int, string> = unbox ()

        /// <summary>
        /// Defines an element's row index or position with respect to the total number of rows within a
        /// table, grid, or treegrid.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-rowindex='", Suffix = "'>")>]
        member _.ariaRowIndex
            with set (_: U2<int, string>) = ()
            and [<Erase>] get (): U2<int, string> = unbox ()

        /// <summary>
        /// Defines the number of rows spanned by a cell or gridcell within a table, grid, or treegrid.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-rowspan='", Suffix = "'>")>]
        member _.ariaRowSpan
            with set (_: U2<float, string>) = ()
            and [<Erase>] get (): U2<float, string> = unbox ()

        /// <summary>
        /// Indicates the current "selected" state of various widgets.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-selected='", Suffix = "'>")>]
        member _.ariaSelected
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Defines the number of items in the current set of listitems or treeitems. Not required if all
        /// elements in the set are present in the DOM.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-setsize='", Suffix = "'>")>]
        member _.ariaSetSize
            with set (_: U2<float, string>) = ()
            and [<Erase>] get (): U2<float, string> = unbox ()

        /// <summary>
        /// Indicates if items in a table or grid are sorted in ascending or descending order.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-sort='", Suffix = "'>")>]
        member _.ariaSort
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        /// <summary>
        /// Defines the maximum allowed value for a range widget.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-valuemax='", Suffix = "'>")>]
        member _.ariaValueMax
            with set (_: U2<float, string>) = ()
            and [<Erase>] get (): U2<float, string> = unbox ()

        /// <summary>
        /// Defines the minimum allowed value for a range widget.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-valuemin='", Suffix = "'>")>]
        member _.ariaValueMin
            with set (_: U2<float, string>) = ()
            and [<Erase>] get (): U2<float, string> = unbox ()

        /// <summary>
        /// Defines the current value for a range widget.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-valuenow='", Suffix = "'>")>]
        member _.ariaValueNow
            with set (_: U2<float, string>) = ()
            and [<Erase>] get (): U2<float, string> = unbox ()

        /// <summary>
        /// Defines the human readable text alternative of aria-valuenow for a range widget.
        /// </summary>
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div aria-valuetext='", Suffix = "'>")>]
        member _.ariaValueText
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()

        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div role='", Suffix = "'>")>]
        member _.role
            with set (_: string) = ()
            and [<Erase>] get (): string = unbox ()
