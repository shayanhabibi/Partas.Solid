namespace Partas.Solid.Kobalte

open Fable.Core
open Partas.Solid
open Partas.Solid.Experimental.U

// ================================================== Interfaces
[<JS.Pojo>]
type CollectionItem(?``type``: CollectionType, ?key: string, ?textValue: string, ?disabled: bool) =
    [<DefaultValue>]
    val mutable ``type``: CollectionType
    [<DefaultValue>]
    val mutable key: string
    [<DefaultValue>]
    val mutable textValue: string
    [<DefaultValue>]
    val mutable disabled: bool

[<Interface; AllowNullLiteral>]
type CollectionNode<'T> =
    abstract ``type``: CollectionType
    abstract key: string
    abstract rawValue: 'T
    abstract textValue: string
    abstract disabled: bool
    abstract level: int
    abstract index: int
    abstract prevKey: string option
    abstract nextKey: string option

[<Interface; AllowNullLiteral>]
type ItemComponentProps<'T> =
    abstract item: CollectionNode<'T>
[<Interface; AllowNullLiteral>]
type SectionComponentProps<'T> =
    abstract section: CollectionNode<'T>

[<JS.Pojo>]
type PosXY(?x: float, ?y: float) =
    member val x = x with get,set
    member val y = y with get,set

[<AllowNullLiteral>]
[<Interface>]
type ColorAxes =
    abstract member xChannel: ColorChannel with get, set
    abstract member yChannel: ColorChannel with get, set
    abstract member zChannel: ColorChannel with get, set

[<AllowNullLiteral>]
[<Interface>]
type ColorChannelRange =
    /// <summary>
    /// The minimum value of the color channel.
    /// </summary>
    abstract member minValue: float with get, set
    /// <summary>
    /// The maximum value of the color channel.
    /// </summary>
    abstract member maxValue: float with get, set
    /// <summary>
    /// The step value of the color channel, used when incrementing and decrementing.
    /// </summary>
    abstract member step: float with get, set
    /// <summary>
    /// The page step value of the color channel, used when incrementing and decrementing.
    /// </summary>
    abstract member pageSize: float with get, set

[<Global>]
[<AllowNullLiteral>]
type XyChannels
    [<ParamObject; Emit("$0")>]
    (
        ?xChannel: ColorChannel,
        ?yChannel: ColorChannel
    ) =

    member val xChannel : ColorChannel option = nativeOnly with get, set
    member val yChannel : ColorChannel option = nativeOnly with get, set
[<Global>]
[<AllowNullLiteral>]
type XyzChannels
    [<ParamObject; Emit("$0")>]
    (
        xChannel: ColorChannel,
        yChannel: ColorChannel,
        zChannel: ColorChannel
    ) =

    member val xChannel : ColorChannel = nativeOnly with get, set
    member val yChannel : ColorChannel = nativeOnly with get, set
    member val zChannel : ColorChannel = nativeOnly with get, set
/// <summary>
/// You can create a Color object using <c>Color.parseColor</c>
/// </summary>
[<AllowNullLiteral>]
[<Interface>]
type Color =
    /// <summary>
    /// Parses a color from a string value. Throws an error if the string could not be parsed.
    /// </summary>
    // BUG: Despite documentation and examples on site, the path to these color utilities is not in @kobalte/utils
    // https://github.com/kobaltedev/kobalte/issues/586
    [<Import("parseColor", "@kobalte/core/colors")>]
    static member parseColor(value: string): Color = jsNative
    // BUG: Despite documentation and examples on site, the path to these color utilities is not in @kobalte/utils
    // https://github.com/kobaltedev/kobalte/issues/586
    [<Import("normalizeColor", "@kobalte/core/colors")>]
    static member normalizeColor(value: string): Color = jsNative
    // BUG: Despite documentation and examples on site, the path to these color utilities is not in @kobalte/utils
    // https://github.com/kobaltedev/kobalte/issues/586
    [<Import("normalizeColor", "@kobalte/core/colors")>]
    static member normalizeColor(value: Color): Color = jsNative
    /// <summary>
    /// Returns a list of color channels for a given color space.
    /// </summary>
    // BUG: Despite documentation and examples on site, the path to these color utilities is not in @kobalte/utils
    // https://github.com/kobaltedev/kobalte/issues/586
    [<Import("getColorChannels", "@kobalte/core/colors")>]
    static member getColorChannels(colorSpace: ColorSpace): ColorChannel * ColorChannel * ColorChannel = jsNative
    /// <summary>
    /// Returns the hue value normalized to the range of 0 to 360.
    /// </summary>
    // BUG: Despite documentation and examples on site, the path to these color utilities is not in @kobalte/utils
    // https://github.com/kobaltedev/kobalte/issues/586
    [<Import("normalizeHue", "@kobalte/core/colors")>]
    static member normalizeHue(value: int): Color = jsNative
    /// <summary>
    /// Converts the color to the given color format, and returns a new Color object.
    /// </summary>
    abstract member toFormat: format: Color.Format -> Color
    /// <summary>
    /// Converts the color to a string in the given format.
    /// </summary>
    abstract member toString: ?format: Color.ToString.Format -> string
    /// <summary>
    /// Returns a duplicate of the color value.
    /// </summary>
    abstract member clone: unit -> Color
    /// <summary>
    /// Converts the color to hex, and returns an integer representation.
    /// </summary>
    abstract member toHexInt: unit -> float
    /// <summary>
    /// Returns the numeric value for a given channel.
    /// Throws an error if the channel is unsupported in the current color format.
    /// </summary>
    abstract member getChannelValue: channel: Color.Channel -> float
    /// <summary>
    /// Sets the numeric value for a given channel, and returns a new Color object.
    /// Throws an error if the channel is unsupported in the current color format.
    /// </summary>
    abstract member withChannelValue: channel: Color.Channel * value: float -> Color
    /// <summary>
    /// Returns the minimum, maximum, and step values for a given channel.
    /// </summary>
    abstract member getChannelRange: channel: Color.Channel -> ColorChannelRange
    /// <summary>
    /// Returns a localized color channel name for a given channel and locale,
    /// for use in visual or accessibility labels.
    /// </summary>
    abstract member getChannelName: channel: Color.Channel * translations: obj -> string
    /// <summary>
    /// Returns the number formatting options for the given channel.
    /// </summary>
    abstract member getChannelFormatOptions: channel: Color.Channel -> obj
    /// <summary>
    /// Formats the numeric value for a given channel for display according to the provided locale.
    /// </summary>
    abstract member formatChannelValue: channel: Color.Channel -> string
    /// <summary>
    /// Returns the color space, 'rgb', 'hsb' or 'hsl', for the current color.
    /// </summary>
    abstract member getColorSpace: unit -> ColorSpace
    /// <summary>
    /// Returns the color space axes, xChannel, yChannel, zChannel.
    /// </summary>
    abstract member getColorSpaceAxes: xyChannels: XyChannels -> ColorAxes
    /// <summary>
    /// Returns an array of the color channels within the current color space space.
    /// </summary>
    abstract member getColorChannels: unit -> Color.Channel * Color.Channel * ColorChannel
    /// <summary>
    /// Returns a localized name for the color, for use in visual or accessibility labels.
    /// </summary>
    abstract member getColorName: translations: obj -> string
    /// <summary>
    /// Returns a localized name for the hue, for use in visual or accessibility labels.
    /// </summary>
    abstract member getHueName: translations: obj -> string
[<AllowNullLiteral>]
[<Interface>]
type SelectionManager =
    abstract member selectionMode: unit -> SelectionMode
    abstract member disallowEmptySelection: unit -> bool
    abstract member selectionBehavior: unit -> SelectionBehavior
    abstract member setSelectionBehavior: selectionBehavior: SelectionBehavior -> unit
    abstract member isFocused: unit -> bool
    abstract member setFocused: isFocused: bool -> unit
    abstract member focusedKey: unit -> string option
    abstract member setFocusedKey: ?key: string -> unit
    abstract member selectedKeys: unit -> Set<string>
    abstract member isSelected: key: string -> bool
    abstract member isEmpty: unit -> bool
    abstract member isSelectAll: unit -> bool
    abstract member firstSelectedKey: unit -> string option
    abstract member lastSelectedKey: unit -> string option
    abstract member extendSelection: toKey: string -> unit
    abstract member toggleSelection: key: string -> unit
    abstract member replaceSelection: key: string -> unit
    abstract member setSelectedKeys: keys: System.Collections.Generic.IEnumerable<string> -> unit
    abstract member selectAll: unit -> unit
    abstract member clearSelection: unit -> unit
    abstract member toggleSelectAll: unit -> unit
    abstract member select: key: string * ?e: Browser.Types.PointerEvent -> unit
    abstract member isSelectionEqual: selection: Set<string> -> bool
    abstract member canSelectItem: key: string -> bool
    abstract member isDisabled: key: string -> bool

[<AllowNullLiteral>]
[<Interface>]
type Collection<'T> =
    inherit System.Collections.Generic.IEnumerable<'T>
    /// <summary>
    /// The number of items in the collection.
    /// </summary>
    abstract member getSize: (unit -> float) with get, set
    /// <summary>
    /// Iterate over all keys in the collection.
    /// </summary>
    abstract member getKeys: (unit -> System.Collections.Generic.IEnumerable<string>) with get, set
    /// <summary>
    /// Get an item by its key.
    /// </summary>
    abstract member getItem: (string -> 'T option) with get, set
    /// <summary>
    /// Get an item by the index of its key.
    /// </summary>
    abstract member at: (float -> 'T option) with get, set
    /// <summary>
    /// Get the key that comes before the given key in the collection.
    /// </summary>
    abstract member getKeyBefore: (string -> string option) with get, set
    /// <summary>
    /// Get the key that comes after the given key in the collection.
    /// </summary>
    abstract member getKeyAfter: (string -> string option) with get, set
    /// <summary>
    /// Get the first key in the collection.
    /// </summary>
    abstract member getFirstKey: (unit -> string option) with get, set
    /// <summary>
    /// Get the last key in the collection.
    /// </summary>
    abstract member getLastKey: (unit -> string option) with get, set

[<AllowNullLiteral>]
[<Interface>]
type ListState<'Value> =
    /// <summary>
    /// A collection of items in the list.
    /// </summary>
    abstract member collection: Accessor<Collection<CollectionNode<'Value>>> with get, set
    /// <summary>
    /// A selection manager to read and update multiple selection state.
    /// </summary>
    abstract member selectionManager: Accessor<SelectionManager> with get, set

[<Erase>]
module KeyboardDelegate =

    type getFirstKey =
        delegate of ?key: string * ?``global``: bool -> string option

    type getLastKey =
        delegate of ?key: string * ?``global``: bool -> string option

    type getKeyForSearch =
        delegate of search: string * ?fromKey: string -> string option


[<AllowNullLiteral>]
[<Interface>]
type KeyboardDelegate =
    /// <summary>
    /// Returns the key visually below the given one, or <c>undefined</c> for none.
    /// </summary>
    abstract member getKeyBelow: (string -> string option) option with get, set
    /// <summary>
    /// Returns the key visually above the given one, or <c>undefined</c> for none.
    /// </summary>
    abstract member getKeyAbove: (string -> string option) option with get, set
    /// <summary>
    /// Returns the key visually to the left of the given one, or <c>undefined</c> for none.
    /// </summary>
    abstract member getKeyLeftOf: (string -> string option) option with get, set
    /// <summary>
    /// Returns the key visually to the right of the given one, or <c>undefined</c> for none.
    /// </summary>
    abstract member getKeyRightOf: (string -> string option) option with get, set
    /// <summary>
    /// Returns the key visually one page below the given one, or <c>undefined</c> for none.
    /// </summary>
    abstract member getKeyPageBelow: (string -> string option) option with get, set
    /// <summary>
    /// Returns the key visually one page above the given one, or <c>undefined</c> for none.
    /// </summary>
    abstract member getKeyPageAbove: (string -> string option) option with get, set
    /// <summary>
    /// Returns the first key, or <c>undefined</c> for none.
    /// </summary>
    abstract member getFirstKey: KeyboardDelegate.getFirstKey option with get, set
    /// <summary>
    /// Returns the last key, or <c>undefined</c> for none.
    /// </summary>
    abstract member getLastKey: KeyboardDelegate.getLastKey option with get, set
    /// <summary>
    /// Returns the next key after <c>fromKey</c> that matches the given search string, or <c>undefined</c> for none.
    /// </summary>
    abstract member getKeyForSearch: KeyboardDelegate.getKeyForSearch option with get, set

[<AllowNullLiteral>]
[<Interface>]
type EventDetails<'T> =
    abstract member originalEvent: 'T with get, set
    abstract member isContextMenu: bool with get, set

[<AllowNullLiteral>]
[<Interface>]
type FormControlDataSet =
    abstract member ``data-valid``: string option with get, set
    abstract member ``data-invalid``: string option with get, set
    abstract member ``data-required``: string option with get, set
    abstract member ``data-disabled``: string option with get, set
    abstract member ``data-readonly``: string option with get, set
    
