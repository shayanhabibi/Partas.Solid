namespace Partas.Solid

open Fable.Core
open Fable.Core.JS

module Aria =

    type HtmlTag with
        // aria role
        [<Erase>]
        member this.role
            with set (value: string) = ()
            and get(): string = undefined
        // aria attributes
        [<Erase>]
        member this.ariaActiveDescendant
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaAtomic
            with set (value: bool) = ()
            and get(): bool = undefined
        [<Erase>]
        member this.ariaAutoComplete
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaBrailleLabel
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaBrailleRoleDescription
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaBusy
            with set (value: bool) = ()
            and get(): bool = undefined
        [<Erase>]
        member this.ariaChecked
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaColCount
            with set (value: int) = ()
            and get(): int = undefined
        [<Erase>]
        member this.ariaColIndex
            with set (value: int) = ()
            and get(): int = undefined
        [<Erase>]
        member this.ariaColIndexText
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaControls
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaCurrent
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaDescribedBy
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaDescription
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaDetails
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaDisabled
            with set (value: bool) = ()
            and get(): bool = undefined
        [<Erase>]
        member this.ariaErrorMessage
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaExpanded
            with set (value: bool) = ()
            and get(): bool = undefined
        [<Erase>]
        member this.ariaFlowTo
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaHasPopup
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaHidden
            with set (value: bool) = ()
            and get(): bool = undefined
        [<Erase>]
        member this.ariaInvalid
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaKeyShortcuts
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaLabel
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaLabelledBy
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaLevel
            with set (value: int) = ()
            and get(): int = undefined
        [<Erase>]
        member this.ariaLive
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaModal
            with set (bool: bool) = ()
            and get(): bool = undefined
        [<Erase>]
        member this.ariaMultiLine
            with set (bool: bool) = ()
            and get(): bool = undefined
        [<Erase>]
        member this.ariaMultiSelectable
            with set (bool: bool) = ()
            and get(): bool = undefined
        [<Erase>]
        member this.ariaOrientation
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaOwns
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaPlaceholder
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaPosInSet
            with set (value: int) = ()
            and get(): int = undefined
        [<Erase>]
        member this.ariaPressed
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaReadOnly
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaRelevant
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaRequired
            with set (bool: bool) = ()
            and get(): bool = undefined
        [<Erase>]
        member this.ariaRoleDescription
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaRowCount
            with set (value: int) = ()
            and get(): int = undefined
        [<Erase>]
        member this.ariaRowIndex
            with set (value: int) = ()
            and get(): int = undefined
        [<Erase>]
        member this.ariaRowIndexText
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaRowSpan
            with set (value: int) = ()
            and get(): int = undefined
        [<Erase>]
        member this.ariaSelected
            with set (bool: bool) = ()
            and get(): bool = undefined
        [<Erase>]
        member this.ariaSetSize
            with set (value: int) = ()
            and get(): int = undefined
        [<Erase>]
        member this.ariaSort
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaValueMax
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaValueMin
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaValueNow
            with set (value: string) = ()
            and get(): string = undefined
        [<Erase>]
        member this.ariaValueText
            with set (value: string) = ()
            and get(): string = undefined
