namespace Partas.Solid.Style
open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Partas.Solid.Experimental.U

[<AutoOpen>]
module Types =
    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)>]
    type AtRules =
        | [<CompiledName("@charset")>] CharSet
        | [<CompiledName("@counter-style")>] CounterStyle
        | [<CompiledName("@document")>] Document
        | [<CompiledName("@font-face")>] FontFace
        | [<CompiledName("@font-feature-values")>] FontFeatureValues
        | [<CompiledName("@font-palette-values")>] FontPaletteValues
        | [<CompiledName("@import")>] Import
        | [<CompiledName("@keyframes")>] Keyframes
        | [<CompiledName("@layer")>] Layer
        | [<CompiledName("@media")>] Media
        | [<CompiledName("@namespace")>] Namespace
        | [<CompiledName("@page")>] Page
        | [<CompiledName("@property")>] Property
        | [<CompiledName("@scope")>] Scope
        | [<CompiledName("@scroll-timeline")>] ScrollTimeline
        | [<CompiledName("@starting-style")>] StartingStyle
        | [<CompiledName("@supports")>] Supports
        | [<CompiledName("@viewport")>] Viewport
    
    [<RequireQualifiedAccess>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Globals =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
    
    [<RequireQualifiedAccess>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)>]
    [<StringEnum(CaseRules.KebabCase)>]
    type AccentColor =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type AlignContent =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | SpaceAround
        | SpaceBetween
        | SpaceEvenly
        | Stretch
        | Center
        | End
        | FlexEnd
        | FlexStart
        | Start
        | Baseline
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type AlignItems =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Center
        | End
        | FlexEnd
        | FlexStart
        | SelfEnd
        | SelfStart
        | Start
        | Baseline
        | Normal
        | Stretch

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type AlignSelf =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Center
        | End
        | FlexEnd
        | FlexStart
        | SelfEnd
        | SelfStart
        | Start
        | Auto
        | Baseline
        | Normal
        | Stretch

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type AlignTracks =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | SpaceAround
        | SpaceBetween
        | SpaceEvenly
        | Stretch
        | Center
        | End
        | FlexEnd
        | FlexStart
        | Start
        | Baseline
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Animation =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Ease
        | EaseIn
        | EaseInOut
        | EaseOut
        | StepEnd
        | StepStart
        | Linear
        | Alternate
        | AlternateReverse
        | Normal
        | Reverse
        | Backwards
        | Both
        | Forwards
        | None
        | Auto
        | Infinite
        | Paused
        | Running

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type AnimationComposition =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Accumulate
        | Add
        | Replace

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type AnimationDirection =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Alternate
        | AlternateReverse
        | Normal
        | Reverse

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type AnimationFillMode =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Backwards
        | Both
        | Forwards
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type AnimationIterationCount =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Infinite

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type AnimationPlayState =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Paused
        | Running

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type AnimationRange =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Contain
        | Cover
        | Entry
        | EntryCrossing
        | Exit
        | ExitCrossing
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type AnimationRangeEnd =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Contain
        | Cover
        | Entry
        | EntryCrossing
        | Exit
        | ExitCrossing
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type AnimationRangeStart =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Contain
        | Cover
        | Entry
        | EntryCrossing
        | Exit
        | ExitCrossing
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type AnimationTimeline =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type AnimationTimingFunction =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Ease
        | EaseIn
        | EaseInOut
        | EaseOut
        | StepEnd
        | StepStart
        | Linear

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Appearance =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Button
        | Checkbox
        | Listbox
        | Menulist
        | Meter
        | ProgressBar
        | PushButton
        | Radio
        | Searchfield
        | SliderHorizontal
        | SquareButton
        | Textarea
        | Auto
        | MenulistButton
        | None
        | Textfield

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type AspectRatio =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Azimuth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Behind
        | Center
        | CenterLeft
        | CenterRight
        | FarLeft
        | FarRight
        | Left
        | LeftSide
        | Leftwards
        | Right
        | RightSide
        | Rightwards

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BackfaceVisibility =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Hidden
        | Visible

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Background =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor
        | Bottom
        | Center
        | Left
        | Right
        | Top
        | NoRepeat
        | Repeat
        | RepeatX
        | RepeatY
        | Round
        | Space
        | Fixed
        | Local
        | Scroll
        | BorderBox
        | ContentBox
        | PaddingBox
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BackgroundAttachment =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Fixed
        | Local
        | Scroll

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BackgroundBlendMode =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Color
        | ColorBurn
        | ColorDodge
        | Darken
        | Difference
        | Exclusion
        | HardLight
        | Hue
        | Lighten
        | Luminosity
        | Multiply
        | Normal
        | Overlay
        | Saturation
        | Screen
        | SoftLight

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BackgroundClip =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | BorderBox
        | ContentBox
        | PaddingBox

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BackgroundOrigin =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | BorderBox
        | ContentBox
        | PaddingBox

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BackgroundPosition =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Bottom
        | Center
        | Left
        | Right
        | Top

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BackgroundPositionX =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Center
        | Left
        | Right
        | XEnd
        | XStart

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BackgroundPositionY =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Bottom
        | Center
        | Top
        | YEnd
        | YStart

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BackgroundRepeat =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | NoRepeat
        | Repeat
        | RepeatX
        | RepeatY
        | Round
        | Space

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BackgroundSize =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Contain
        | Cover

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BlockOverflow =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Clip
        | Ellipsis

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BlockSize =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-fit-content")>] MozFitContent
        | [<CompiledName("-moz-max-content")>] MozMaxContent
        | [<CompiledName("-moz-min-content")>] MozMinContent
        | Auto
        | FitContent
        | MaxContent
        | MinContent

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Border =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderBlock =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderBlockEnd =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderBlockEndStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderBlockEndWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderBlockStart =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderBlockStartStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderBlockStartWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderBlockStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderBlockWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderBottom =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor
        
    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderBottomStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderBottomWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderCollapse =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Collapse
        | Separate

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderImage =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | None
        | Repeat
        | Round
        | Space
        | Stretch

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderImageRepeat =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Repeat
        | Round
        | Space
        | Stretch

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderImageWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderInline =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderInlineEnd =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderInlineEndStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderInlineEndWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderInlineStart =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderInlineStartStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderInlineStartWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderInlineStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderInlineWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderLeft =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderLeftStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderLeftWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderRight =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderRightStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderRightWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderTop =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderTopStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderTopWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BorderWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Bottom =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BoxAlign =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Baseline
        | Center
        | End
        | Start
        | Stretch

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BoxDecorationBreak =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Clone
        | Slice

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BoxDirection =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Normal
        | Reverse

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BoxLines =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Multiple
        | Single

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BoxOrient =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | BlockAxis
        | Horizontal
        | InlineAxis
        | Vertical

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BoxPack =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Center
        | End
        | Justify
        | Start

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BoxSizing =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | BorderBox
        | ContentBox

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BreakAfter =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | All
        | Always
        | Auto
        | Avoid
        | AvoidColumn
        | AvoidPage
        | AvoidRegion
        | Column
        | Left
        | Page
        | Recto
        | Region
        | Right
        | Verso

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BreakBefore =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | All
        | Always
        | Auto
        | Avoid
        | AvoidColumn
        | AvoidPage
        | AvoidRegion
        | Column
        | Left
        | Page
        | Recto
        | Region
        | Right
        | Verso

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BreakInside =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Avoid
        | AvoidColumn
        | AvoidPage
        | AvoidRegion

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type CaptionSide =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | BlockEnd
        | BlockStart
        | Bottom
        | InlineEnd
        | InlineStart
        | Top

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Caret =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor
        | Auto
        | Bar
        | Block
        | Underscore

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type CaretColor =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type CaretShape =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Bar
        | Block
        | Underscore

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Clear =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Both
        | InlineEnd
        | InlineStart
        | Left
        | None
        | Right

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Clip =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ClipPath =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | BorderBox
        | ContentBox
        | PaddingBox
        | FillBox
        | MarginBox
        | StrokeBox
        | ViewBox
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Color =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type PrintColorAdjust =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Economy
        | Exact

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ColorScheme =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Dark
        | Light
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ColumnCount =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ColumnFill =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Balance
        | BalanceAll

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ColumnGap =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ColumnRule =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ColumnRuleStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ColumnRuleWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ColumnSpan =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | All
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ColumnWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Columns =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Contain =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Content
        | InlineSize
        | Layout
        | None
        | Paint
        | Size
        | Strict
        | Style

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ContainerType =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | InlineSize
        | Normal
        | Size

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Content =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | CloseQuote
        | NoCloseQuote
        | NoOpenQuote
        | OpenQuote
        | Contents
        | None
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ContentVisibility =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Hidden
        | Visible

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Cursor =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-grab")>] MozGrab
        | WebkitGrab
        | Alias
        | AllScroll
        | Auto
        | Cell
        | ColResize
        | ContextMenu
        | Copy
        | Crosshair
        | Default
        | EResize
        | EwResize
        | Grab
        | Grabbing
        | Help
        | Move
        | NResize
        | NeResize
        | NeswResize
        | NoDrop
        | None
        | NotAllowed
        | NsResize
        | NwResize
        | NwseResize
        | Pointer
        | Progress
        | RowResize
        | SResize
        | SeResize
        | SwResize
        | Text
        | VerticalText
        | WResize
        | Wait
        | ZoomIn
        | ZoomOut

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Direction =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Ltr
        | Rtl

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Display =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Block
        | Inline
        | RunIn
        | MsFlexbox
        | MsGrid
        | WebkitFlex
        | Flex
        | Flow
        | FlowRoot
        | Grid
        | Ruby
        | Table
        | RubyBase
        | RubyBaseContainer
        | RubyText
        | RubyTextContainer
        | TableCaption
        | TableCell
        | TableColumn
        | TableColumnGroup
        | TableFooterGroup
        | TableHeaderGroup
        | TableRow
        | TableRowGroup
        | MsInlineFlexbox
        | MsInlineGrid
        | WebkitInlineFlex
        | InlineBlock
        | InlineFlex
        | InlineGrid
        | InlineListItem
        | InlineTable
        | Contents
        | ListItem
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type EmptyCells =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Hide
        | Show

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Flex =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Content
        | FitContent
        | MaxContent
        | MinContent
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FlexBasis =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-fit-content")>] MozFitContent
        | [<CompiledName("-moz-max-content")>] MozMaxContent
        | [<CompiledName("-moz-min-content")>] MozMinContent
        | WebkitAuto
        | Auto
        | Content
        | FitContent
        | MaxContent
        | MinContent

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FlexDirection =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Column
        | ColumnReverse
        | Row
        | RowReverse

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FlexFlow =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Column
        | ColumnReverse
        | Nowrap
        | Row
        | RowReverse
        | Wrap
        | WrapReverse

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FlexWrap =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Nowrap
        | Wrap
        | WrapReverse

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Float =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | InlineEnd
        | InlineStart
        | Left
        | None
        | Right

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Font =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Caption
        | Icon
        | Menu
        | MessageBox
        | SmallCaption
        | StatusBar

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontFamily =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Cursive
        | Fantasy
        | Monospace
        | SansSerif
        | Serif

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontFeatureSettings =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontKerning =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | None
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontLanguageOverride =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontOpticalSizing =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontPalette =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Dark
        | Light
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontSize =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Large
        | Medium
        | Small
        | XLarge
        | XSmall
        | XxLarge
        | XxSmall
        | XxxLarge
        | Larger
        | Smaller

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontSizeAdjust =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | FromFont
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontSmooth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Large
        | Medium
        | Small
        | XLarge
        | XSmall
        | XxLarge
        | XxSmall
        | XxxLarge
        | Always
        | Auto
        | Never

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontStretch =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Condensed
        | Expanded
        | ExtraCondensed
        | ExtraExpanded
        | Normal
        | SemiCondensed
        | SemiExpanded
        | UltraCondensed
        | UltraExpanded

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Italic
        | Normal
        | Oblique

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontSynthesis =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | None
        | Position
        | SmallCaps
        | Style
        | Weight

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontSynthesisPosition =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontSynthesisSmallCaps =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontSynthesisStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontSynthesisWeight =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontVariant =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Jis04
        | Jis78
        | Jis83
        | Jis90
        | Simplified
        | Traditional
        | AllPetiteCaps
        | AllSmallCaps
        | CommonLigatures
        | Contextual
        | DiagonalFractions
        | DiscretionaryLigatures
        | FullWidth
        | HistoricalForms
        | HistoricalLigatures
        | LiningNums
        | NoCommonLigatures
        | NoContextual
        | NoDiscretionaryLigatures
        | NoHistoricalLigatures
        | None
        | Normal
        | OldstyleNums
        | Ordinal
        | PetiteCaps
        | ProportionalNums
        | ProportionalWidth
        | Ruby
        | SlashedZero
        | SmallCaps
        | StackedFractions
        | TabularNums
        | TitlingCaps
        | Unicase

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontVariantAlternates =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | HistoricalForms
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontVariantCaps =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | AllPetiteCaps
        | AllSmallCaps
        | Normal
        | PetiteCaps
        | SmallCaps
        | TitlingCaps
        | Unicase

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontVariantEastAsian =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Jis04
        | Jis78
        | Jis83
        | Jis90
        | Simplified
        | Traditional
        | FullWidth
        | Normal
        | ProportionalWidth
        | Ruby

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontVariantEmoji =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Emoji
        | Normal
        | Text
        | Unicode

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontVariantLigatures =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | CommonLigatures
        | Contextual
        | DiscretionaryLigatures
        | HistoricalLigatures
        | NoCommonLigatures
        | NoContextual
        | NoDiscretionaryLigatures
        | NoHistoricalLigatures
        | None
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontVariantNumeric =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | DiagonalFractions
        | LiningNums
        | Normal
        | OldstyleNums
        | Ordinal
        | ProportionalNums
        | SlashedZero
        | StackedFractions
        | TabularNums

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontVariantPosition =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Normal
        | Sub
        | Super

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontVariationSettings =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FontWeight =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Bold
        | Normal
        | Bolder
        | Lighter

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ForcedColorAdjust =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Gap =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type GridArea =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type GridAutoColumns =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | MaxContent
        | MinContent

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type GridAutoFlow =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Column
        | Dense
        | Row

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type GridAutoRows =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | MaxContent
        | MinContent

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type GridColumn =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type GridColumnEnd =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type GridColumnStart =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type GridRow =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type GridRowEnd =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type GridRowStart =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        
    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type GridTemplateColumns =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | MaxContent
        | MinContent
        | None
        | Subgrid

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type GridTemplateRows =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | MaxContent
        | MinContent
        | None
        | Subgrid

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type HangingPunctuation =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | AllowEnd
        | First
        | ForceEnd
        | Last
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Height =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-max-content")>] MozMaxContent
        | [<CompiledName("-moz-min-content")>] MozMinContent
        | WebkitFitContent
        | Auto
        | FitContent
        | MaxContent
        | MinContent

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type HyphenateCharacter =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type HyphenateLimitChars =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Hyphens =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Manual
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ImageOrientation =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Flip
        | FromImage

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ImageRendering =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-crisp-edges")>] MozCrispEdges
        | WebkitOptimizeContrast
        | Auto
        | CrispEdges
        | Pixelated

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ImageResolution =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | FromImage

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ImeMode =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Active
        | Auto
        | Disabled
        | Inactive
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type InitialLetter =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type InlineSize =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-fit-content")>] MozFitContent
        | [<CompiledName("-moz-max-content")>] MozMaxContent
        | [<CompiledName("-moz-min-content")>] MozMinContent
        | WebkitFillAvailable
        | Auto
        | FitContent
        | MaxContent
        | MinContent

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type InputSecurity =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Inset =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type InsetBlock =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type InsetBlockEnd =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type InsetBlockStart =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type InsetInline =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type InsetInlineEnd =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type InsetInlineStart =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Isolation =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Isolate

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type JustifyContent =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | SpaceAround
        | SpaceBetween
        | SpaceEvenly
        | Stretch
        | Center
        | End
        | FlexEnd
        | FlexStart
        | Start
        | Left
        | Normal
        | Right

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type JustifyItems =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Center
        | End
        | FlexEnd
        | FlexStart
        | SelfEnd
        | SelfStart
        | Start
        | Baseline
        | Left
        | Legacy
        | Normal
        | Right
        | Stretch

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type JustifySelf =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Center
        | End
        | FlexEnd
        | FlexStart
        | SelfEnd
        | SelfStart
        | Start
        | Auto
        | Baseline
        | Left
        | Normal
        | Right
        | Stretch

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type JustifyTracks =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | SpaceAround
        | SpaceBetween
        | SpaceEvenly
        | Stretch
        | Center
        | End
        | FlexEnd
        | FlexStart
        | Start
        | Left
        | Normal
        | Right

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Left =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type LetterSpacing =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type LineBreak =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Anywhere
        | Auto
        | Loose
        | Normal
        | Strict

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type LineHeight =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ListStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Inside
        | None
        | Outside

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ListStylePosition =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Inside
        | Outside

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Margin =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MarginBlock =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MarginBlockEnd =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MarginBlockStart =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MarginBottom =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MarginInline =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MarginInlineEnd =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MarginInlineStart =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MarginLeft =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MarginRight =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MarginTop =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MarginTrim =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | All
        | InFlow
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Mask =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Bottom
        | Center
        | Left
        | Right
        | Top
        | NoRepeat
        | Repeat
        | RepeatX
        | RepeatY
        | Round
        | Space
        | BorderBox
        | ContentBox
        | PaddingBox
        | FillBox
        | MarginBox
        | StrokeBox
        | ViewBox
        | Add
        | Exclude
        | Intersect
        | Subtract
        | Alpha
        | Luminance
        | MatchSource
        | NoClip
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MaskBorder =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Alpha
        | Luminance
        | None
        | Repeat
        | Round
        | Space
        | Stretch

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MaskBorderMode =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Alpha
        | Luminance

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MaskBorderRepeat =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Repeat
        | Round
        | Space
        | Stretch

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MaskBorderWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MaskClip =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | BorderBox
        | ContentBox
        | PaddingBox
        | FillBox
        | MarginBox
        | StrokeBox
        | ViewBox
        | NoClip

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MaskComposite =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Add
        | Exclude
        | Intersect
        | Subtract

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MaskMode =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Alpha
        | Luminance
        | MatchSource

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MaskOrigin =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | BorderBox
        | ContentBox
        | PaddingBox
        | FillBox
        | MarginBox
        | StrokeBox
        | ViewBox

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MaskPosition =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Bottom
        | Center
        | Left
        | Right
        | Top

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MaskRepeat =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | NoRepeat
        | Repeat
        | RepeatX
        | RepeatY
        | Round
        | Space

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MaskSize =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Contain
        | Cover

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MaskType =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Alpha
        | Luminance

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MasonryAutoFlow =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | DefiniteFirst
        | Next
        | Ordered
        | Pack

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MathDepth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | AutoAdd

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MathShift =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Compact
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MathStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Compact
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MaxBlockSize =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-max-content")>] MozMaxContent
        | [<CompiledName("-moz-min-content")>] MozMinContent
        | WebkitFillAvailable
        | FitContent
        | MaxContent
        | MinContent
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MaxHeight =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-fit-content")>] MozFitContent
        | [<CompiledName("-moz-max-content")>] MozMaxContent
        | [<CompiledName("-moz-min-content")>] MozMinContent
        | WebkitFitContent
        | WebkitMaxContent
        | WebkitMinContent
        | FitContent
        | Intrinsic
        | MaxContent
        | MinContent
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MaxInlineSize =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-fit-content")>] MozFitContent
        | [<CompiledName("-moz-max-content")>] MozMaxContent
        | [<CompiledName("-moz-min-content")>] MozMinContent
        | WebkitFillAvailable
        | FitContent
        | MaxContent
        | MinContent
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MaxWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-fit-content")>] MozFitContent
        | [<CompiledName("-moz-max-content")>] MozMaxContent
        | [<CompiledName("-moz-min-content")>] MozMinContent
        | WebkitFitContent
        | WebkitMaxContent
        | WebkitMinContent
        | FitContent
        | Intrinsic
        | MaxContent
        | MinContent
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MinBlockSize =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-max-content")>] MozMaxContent
        | [<CompiledName("-moz-min-content")>] MozMinContent
        | WebkitFillAvailable
        | Auto
        | FitContent
        | MaxContent
        | MinContent

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MinHeight =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-fit-content")>] MozFitContent
        | [<CompiledName("-moz-max-content")>] MozMaxContent
        | [<CompiledName("-moz-min-content")>] MozMinContent
        | WebkitFitContent
        | WebkitMaxContent
        | WebkitMinContent
        | Auto
        | FitContent
        | Intrinsic
        | MaxContent
        | MinContent

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MinInlineSize =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-fit-content")>] MozFitContent
        | [<CompiledName("-moz-max-content")>] MozMaxContent
        | [<CompiledName("-moz-min-content")>] MozMinContent
        | WebkitFillAvailable
        | Auto
        | FitContent
        | MaxContent
        | MinContent

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MinWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-fit-content")>] MozFitContent
        | [<CompiledName("-moz-max-content")>] MozMaxContent
        | [<CompiledName("-moz-min-content")>] MozMinContent
        | WebkitFillAvailable
        | WebkitFitContent
        | WebkitMaxContent
        | WebkitMinContent
        | Auto
        | FitContent
        | Intrinsic
        | MaxContent
        | MinContent
        | MinIntrinsic

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MixBlendMode =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Color
        | ColorBurn
        | ColorDodge
        | Darken
        | Difference
        | Exclusion
        | HardLight
        | Hue
        | Lighten
        | Luminosity
        | Multiply
        | Normal
        | Overlay
        | Saturation
        | Screen
        | SoftLight
        | PlusLighter

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Offset =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Bottom
        | Center
        | Left
        | Right
        | Top
        | Auto
        | None
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type OffsetRotate =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Reverse

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ObjectFit =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Contain
        | Cover
        | Fill
        | None
        | ScaleDown

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ObjectPosition =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Bottom
        | Center
        | Left
        | Right
        | Top

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type OffsetAnchor =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Bottom
        | Center
        | Left
        | Right
        | Top
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type OffsetPosition =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Bottom
        | Center
        | Left
        | Right
        | Top
        | Auto
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Outline =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid
        | Medium
        | Thick
        | Thin
        | Auto
        | Invert

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type OutlineColor =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor
        | Invert

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type OutlineStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type OutlineWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Overflow =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-hidden-unscrollable")>] MozHiddenUnscrollable
        | Auto
        | Clip
        | Hidden
        | Scroll
        | Visible

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type OverflowAnchor =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type OverflowBlock =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Clip
        | Hidden
        | Scroll
        | Visible

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type OverflowClipBox =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | ContentBox
        | PaddingBox

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type OverflowClipMargin =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | BorderBox
        | ContentBox
        | PaddingBox

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type OverflowInline =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Clip
        | Hidden
        | Scroll
        | Visible

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type OverflowWrap =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Anywhere
        | BreakWord
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type OverflowX =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-hidden-unscrollable")>] MozHiddenUnscrollable
        | Auto
        | Clip
        | Hidden
        | Scroll
        | Visible

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type OverflowY =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-hidden-unscrollable")>] MozHiddenUnscrollable
        | Auto
        | Clip
        | Hidden
        | Scroll
        | Visible

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Overlay =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type OverscrollBehavior =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Contain
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type OverscrollBehaviorBlock =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Contain
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type OverscrollBehaviorInline =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Contain
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type OverscrollBehaviorX =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Contain
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type OverscrollBehaviorY =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Contain
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Page =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type PageBreakAfter =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Always
        | Auto
        | Avoid
        | Left
        | Recto
        | Right
        | Verso

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type PageBreakBefore =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Always
        | Auto
        | Avoid
        | Left
        | Recto
        | Right
        | Verso

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type PageBreakInside =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Avoid

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type PaintOrder =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Fill
        | Markers
        | Normal
        | Stroke

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type PerspectiveOrigin =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Bottom
        | Center
        | Left
        | Right
        | Top

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type PlaceContent =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | SpaceAround
        | SpaceBetween
        | SpaceEvenly
        | Stretch
        | Center
        | End
        | FlexEnd
        | FlexStart
        | Start
        | Baseline
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type PlaceItems =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Center
        | End
        | FlexEnd
        | FlexStart
        | SelfEnd
        | SelfStart
        | Start
        | Baseline
        | Normal
        | Stretch

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type PlaceSelf =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Center
        | End
        | FlexEnd
        | FlexStart
        | SelfEnd
        | SelfStart
        | Start
        | Auto
        | Baseline
        | Normal
        | Stretch

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type PointerEvents =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | All
        | Auto
        | Fill
        | None
        | Painted
        | Stroke
        | Visible
        | VisibleFill
        | VisiblePainted
        | VisibleStroke

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Position =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | WebkitSticky
        | Absolute
        | Fixed
        | Relative
        | Static
        | Sticky

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Quotes =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Resize =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Block
        | Both
        | Horizontal
        | Inline
        | None
        | Vertical

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Right =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type RowGap =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type RubyAlign =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Center
        | SpaceAround
        | SpaceBetween
        | Start

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type RubyMerge =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Collapse
        | Separate

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type RubyPosition =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Alternate
        | InterCharacter
        | Over
        | Under

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollBehavior =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Smooth

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollPadding =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollPaddingBlock =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollPaddingBlockEnd =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollPaddingBlockStart =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollPaddingBottom =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollPaddingInline =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollPaddingInlineEnd =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollPaddingInlineStart =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollPaddingLeft =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollPaddingRight =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollPaddingTop =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollSnapAlign =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Center
        | End
        | None
        | Start

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollSnapCoordinate =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Bottom
        | Center
        | Left
        | Right
        | Top
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollSnapDestination =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Bottom
        | Center
        | Left
        | Right
        | Top

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollSnapStop =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Always
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollSnapType =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Block
        | Both
        | Inline
        | None
        | X
        | Y

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollSnapTypeX =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Mandatory
        | None
        | Proximity

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollSnapTypeY =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Mandatory
        | None
        | Proximity

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollTimelineAxis =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Block
        | Inline
        | X
        | Y

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollbarColor =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollbarGutter =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Stable

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ScrollbarWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | None
        | Thin

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ShapeOutside =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | BorderBox
        | ContentBox
        | PaddingBox
        | MarginBox
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TableLayout =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Fixed

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextAlign =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | WebkitMatchParent
        | Center
        | End
        | Justify
        | Left
        | MatchParent
        | Right
        | Start

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextAlignLast =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Center
        | End
        | Justify
        | Left
        | Right
        | Start

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextCombineUpright =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | All
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextDecoration =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor
        | Auto
        | Blink
        | Dashed
        | Dotted
        | Double
        | FromFont
        | GrammarError
        | LineThrough
        | None
        | Overline
        | Solid
        | SpellingError
        | Underline
        | Wavy

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextDecorationLine =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Blink
        | GrammarError
        | LineThrough
        | None
        | Overline
        | SpellingError
        | Underline

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextDecorationSkip =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | BoxDecoration
        | Edges
        | LeadingSpaces
        | None
        | Objects
        | Spaces
        | TrailingSpaces

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextDecorationSkipInk =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | All
        | Auto
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextDecorationStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Dashed
        | Dotted
        | Double
        | Solid
        | Wavy

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextDecorationThickness =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | FromFont

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextEmphasis =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor
        | Circle
        | Dot
        | DoubleCircle
        | Filled
        | None
        | Open
        | Sesame
        | Triangle

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextEmphasisStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Circle
        | Dot
        | DoubleCircle
        | Filled
        | None
        | Open
        | Sesame
        | Triangle

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextJustify =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | InterCharacter
        | InterWord
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextOrientation =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Mixed
        | Sideways
        | Upright

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextOverflow =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Clip
        | Ellipsis

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextRendering =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | GeometricPrecision
        | OptimizeLegibility
        | OptimizeSpeed

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextSizeAdjust =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextTransform =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Capitalize
        | FullSizeKana
        | FullWidth
        | Lowercase
        | None
        | Uppercase

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextUnderlineOffset =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextUnderlinePosition =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | FromFont
        | Left
        | Right
        | Under

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextWrap =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Balance
        | Nowrap
        | Pretty
        | Stable
        | Wrap

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Top =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TouchAction =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | MsManipulation
        | MsNone
        | MsPinchZoom
        | Auto
        | Manipulation
        | None
        | PanDown
        | PanLeft
        | PanRight
        | PanUp
        | PanX
        | PanY
        | PinchZoom

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TransformBox =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | BorderBox
        | ContentBox
        | FillBox
        | StrokeBox
        | ViewBox

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TransformOrigin =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Bottom
        | Center
        | Left
        | Right
        | Top

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TransformStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Flat
        | Preserve3d

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Transition =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Ease
        | EaseIn
        | EaseInOut
        | EaseOut
        | StepEnd
        | StepStart
        | Linear
        | All
        | AllowDiscrete
        | None
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TransitionBehavior =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | AllowDiscrete
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TransitionProperty =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | All
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TransitionTimingFunction =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Ease
        | EaseIn
        | EaseInOut
        | EaseOut
        | StepEnd
        | StepStart
        | Linear

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type UnicodeBidi =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-isolate")>] MozIsolate
        | [<CompiledName("-moz-isolate-override")>] MozIsolateOverride
        | [<CompiledName("-moz-plain-text")>] MozPlaintext
        | WebkitIsolate
        | WebkitIsolateOverride
        | WebkitPlaintext
        | BidiOverride
        | Embed
        | Isolate
        | IsolateOverride
        | Normal
        | Plaintext

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type UserSelect =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-none")>] MozNone
        | All
        | Auto
        | Contain
        | Element
        | None
        | Text

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type VerticalAlign =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Baseline
        | Bottom
        | Middle
        | Sub
        | Super
        | TextBottom
        | TextTop
        | Top

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ViewTimelineAxis =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Block
        | Inline
        | X
        | Y

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ViewTimelineInset =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Visibility =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Collapse
        | Hidden
        | Visible

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WhiteSpace =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-pre-wrap")>] MozPreWrap
        | Balance
        | BreakSpaces
        | Collapse
        | Discard
        | DiscardAfter
        | DiscardBefore
        | DiscardInner
        | None
        | Normal
        | Nowrap
        | Pre
        | PreLine
        | PreWrap
        | Preserve
        | PreserveBreaks
        | PreserveSpaces
        | Pretty
        | Stable
        | Wrap

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WhiteSpaceCollapse =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | BreakSpaces
        | Collapse
        | Discard
        | Preserve
        | PreserveBreaks
        | PreserveSpaces

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WhiteSpaceTrim =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | DiscardAfter
        | DiscardBefore
        | DiscardInner
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Width =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | [<CompiledName("-moz-fit-content")>] MozFitContent
        | [<CompiledName("-moz-max-content")>] MozMaxContent
        | [<CompiledName("-moz-min-content")>] MozMinContent
        | WebkitFitContent
        | WebkitMaxContent
        | Auto
        | FitContent
        | Intrinsic
        | MaxContent
        | MinContent
        | MinIntrinsic

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WillChange =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Contents
        | ScrollPosition
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WordBreak =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | BreakAll
        | BreakWord
        | KeepAll
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WordSpacing =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WordWrap =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | BreakWord
        | Normal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WritingMode =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | HorizontalTb
        | SidewaysLr
        | SidewaysRl
        | VerticalLr
        | VerticalRl

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ZIndex =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Zoom =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Normal
        | Reset
    
    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsAccelerator =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | False
        | True

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsBlockProgression =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Bt
        | Lr
        | Rl
        | Tb

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsContentZoomChaining =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Chained
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsContentZoomSnap =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Mandatory
        | None
        | Proximity

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsContentZoomSnapType =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Mandatory
        | None
        | Proximity

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsContentZooming =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | None
        | Zoom

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsGridColumns =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | MaxContent
        | MinContent
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsGridRows =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | MaxContent
        | MinContent
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsHighContrastAdjust =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsHyphenateLimitChars =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsHyphenateLimitLines =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | NoLimit

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsImeAlign =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | After
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsOverflowStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | MsAutohidingScrollbar
        | Auto
        | None
        | Scrollbar

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsScrollChaining =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Chained
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsScrollLimitXMax =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsScrollLimitYMax =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsScrollRails =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | None
        | Railed

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsScrollSnapType =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Mandatory
        | None
        | Proximity

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsScrollTranslation =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | None
        | VerticalToHorizontal

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsTextAutospace =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | IdeographAlpha
        | IdeographNumeric
        | IdeographParenthesis
        | IdeographSpace
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsTouchSelect =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Grippers
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsUserSelect =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Element
        | None
        | Text

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsWrapFlow =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Both
        | Clear
        | End
        | Maximum
        | Start

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type MsWrapThrough =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | None
        | Wrap

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitAppearance =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | ApplePayButton
        | Button
        | ButtonBevel
        | Caret
        | Checkbox
        | DefaultButton
        | InnerSpinButton
        | Listbox
        | Listitem
        | MediaControlsBackground
        | MediaControlsFullscreenBackground
        | MediaCurrentTimeDisplay
        | MediaEnterFullscreenButton
        | MediaExitFullscreenButton
        | MediaFullscreenButton
        | MediaMuteButton
        | MediaOverlayPlayButton
        | MediaPlayButton
        | MediaSeekBackButton
        | MediaSeekForwardButton
        | MediaSlider
        | MediaSliderthumb
        | MediaTimeRemainingDisplay
        | MediaToggleClosedCaptionsButton
        | MediaVolumeSlider
        | MediaVolumeSliderContainer
        | MediaVolumeSliderthumb
        | Menulist
        | MenulistButton
        | MenulistText
        | MenulistTextfield
        | Meter
        | None
        | ProgressBar
        | ProgressBarValue
        | PushButton
        | Radio
        | Searchfield
        | SearchfieldCancelButton
        | SearchfieldDecoration
        | SearchfieldResultsButton
        | SearchfieldResultsDecoration
        | SliderHorizontal
        | SliderVertical
        | SliderthumbHorizontal
        | SliderthumbVertical
        | SquareButton
        | Textarea
        | Textfield

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitBorderBefore =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitBorderBeforeStyle =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Dashed
        | Dotted
        | Double
        | Groove
        | Hidden
        | Inset
        | None
        | Outset
        | Ridge
        | Solid

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitBorderBeforeWidth =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Medium
        | Thick
        | Thin

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitBoxReflect =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Above
        | Below
        | Left
        | Right

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitMask =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Bottom
        | Center
        | Left
        | Right
        | Top
        | NoRepeat
        | Repeat
        | RepeatX
        | RepeatY
        | Round
        | Space
        | BorderBox
        | ContentBox
        | PaddingBox
        | Border
        | Content
        | None
        | Padding
        | Text

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitMaskAttachment =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Fixed
        | Local
        | Scroll

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitMaskClip =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | BorderBox
        | ContentBox
        | PaddingBox
        | Border
        | Content
        | Padding
        | Text

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitMaskComposite =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Clear
        | Copy
        | DestinationAtop
        | DestinationIn
        | DestinationOut
        | DestinationOver
        | SourceAtop
        | SourceIn
        | SourceOut
        | SourceOver
        | Xor

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitMaskOrigin =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | BorderBox
        | ContentBox
        | PaddingBox
        | Border
        | Content
        | Padding

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitMaskPosition =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Bottom
        | Center
        | Left
        | Right
        | Top

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitMaskPositionX =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Center
        | Left
        | Right

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitMaskPositionY =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Bottom
        | Center
        | Top

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitMaskRepeat =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | NoRepeat
        | Repeat
        | RepeatX
        | RepeatY
        | Round
        | Space

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitMaskRepeatX =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | NoRepeat
        | Repeat
        | Round
        | Space

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitMaskRepeatY =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | NoRepeat
        | Repeat
        | Round
        | Space

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitMaskSize =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Contain
        | Cover

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitOverflowScrolling =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | Touch

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitTouchCallout =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Default
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type WebkitUserModify =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | ReadOnly
        | ReadWrite
        | ReadWritePlaintextOnly

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type AlignmentBaseline =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | AfterEdge
        | Alphabetic
        | Auto
        | Baseline
        | BeforeEdge
        | Central
        | Hanging
        | Ideographic
        | Mathematical
        | Middle
        | TextAfterEdge
        | TextBeforeEdge

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type BaselineShift =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Baseline
        | Sub
        | Super

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ClipRule =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Evenodd
        | Nonzero

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ColorInterpolation =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | LinearRGB
        | SRGB

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ColorRendering =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | OptimizeQuality
        | OptimizeSpeed

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type DominantBaseline =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Alphabetic
        | Auto
        | Central
        | Hanging
        | Ideographic
        | Mathematical
        | Middle
        | NoChange
        | ResetSize
        | TextAfterEdge
        | TextBeforeEdge
        | UseScript

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Fill =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor
        | Child
        | ContextFill
        | ContextStroke
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type FillRule =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Evenodd
        | Nonzero

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type GlyphOrientationVertical =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type ShapeRendering =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Auto
        | CrispEdges
        | GeometricPrecision
        | OptimizeSpeed

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type Stroke =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Aliceblue
        | Antiquewhite
        | Aqua
        | Aquamarine
        | Azure
        | Beige
        | Bisque
        | Black
        | Blanchedalmond
        | Blue
        | Blueviolet
        | Brown
        | Burlywood
        | Cadetblue
        | Chartreuse
        | Chocolate
        | Coral
        | Cornflowerblue
        | Cornsilk
        | Crimson
        | Cyan
        | Darkblue
        | Darkcyan
        | Darkgoldenrod
        | Darkgray
        | Darkgreen
        | Darkgrey
        | Darkkhaki
        | Darkmagenta
        | Darkolivegreen
        | Darkorange
        | Darkorchid
        | Darkred
        | Darksalmon
        | Darkseagreen
        | Darkslateblue
        | Darkslategray
        | Darkslategrey
        | Darkturquoise
        | Darkviolet
        | Deeppink
        | Deepskyblue
        | Dimgray
        | Dimgrey
        | Dodgerblue
        | Firebrick
        | Floralwhite
        | Forestgreen
        | Fuchsia
        | Gainsboro
        | Ghostwhite
        | Gold
        | Goldenrod
        | Gray
        | Green
        | Greenyellow
        | Grey
        | Honeydew
        | Hotpink
        | Indianred
        | Indigo
        | Ivory
        | Khaki
        | Lavender
        | Lavenderblush
        | Lawngreen
        | Lemonchiffon
        | Lightblue
        | Lightcoral
        | Lightcyan
        | Lightgoldenrodyellow
        | Lightgray
        | Lightgreen
        | Lightgrey
        | Lightpink
        | Lightsalmon
        | Lightseagreen
        | Lightskyblue
        | Lightslategray
        | Lightslategrey
        | Lightsteelblue
        | Lightyellow
        | Lime
        | Limegreen
        | Linen
        | Magenta
        | Maroon
        | Mediumaquamarine
        | Mediumblue
        | Mediumorchid
        | Mediumpurple
        | Mediumseagreen
        | Mediumslateblue
        | Mediumspringgreen
        | Mediumturquoise
        | Mediumvioletred
        | Midnightblue
        | Mintcream
        | Mistyrose
        | Moccasin
        | Navajowhite
        | Navy
        | Oldlace
        | Olive
        | Olivedrab
        | Orange
        | Orangered
        | Orchid
        | Palegoldenrod
        | Palegreen
        | Paleturquoise
        | Palevioletred
        | Papayawhip
        | Peachpuff
        | Peru
        | Pink
        | Plum
        | Powderblue
        | Purple
        | Rebeccapurple
        | Red
        | Rosybrown
        | Royalblue
        | Saddlebrown
        | Salmon
        | Sandybrown
        | Seagreen
        | Seashell
        | Sienna
        | Silver
        | Skyblue
        | Slateblue
        | Slategray
        | Slategrey
        | Snow
        | Springgreen
        | Steelblue
        | Tan
        | Teal
        | Thistle
        | Tomato
        | Transparent
        | Turquoise
        | Violet
        | Wheat
        | White
        | Whitesmoke
        | Yellow
        | Yellowgreen
        | ActiveBorder
        | ActiveCaption
        | AppWorkspace
        | Background
        | ButtonFace
        | ButtonHighlight
        | ButtonShadow
        | ButtonText
        | CaptionText
        | GrayText
        | Highlight
        | HighlightText
        | InactiveBorder
        | InactiveCaption
        | InactiveCaptionText
        | InfoBackground
        | InfoText
        | Menu
        | MenuText
        | Scrollbar
        | ThreeDDarkShadow
        | ThreeDFace
        | ThreeDHighlight
        | ThreeDLightShadow
        | ThreeDShadow
        | Window
        | WindowFrame
        | WindowText
        | Currentcolor
        | Child
        | ContextFill
        | ContextStroke
        | None

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type StrokeLinecap =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Butt
        | Round
        | Square

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type StrokeLinejoin =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | Bevel
        | Miter
        | Round

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type TextAnchor =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | End
        | Middle
        | Start

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.KebabCase)>]
    type VectorEffect =
        | [<CompiledName("-moz-initial")>] MozInitial
        | Inherit
        | Initial
        | Revert
        | RevertLayer
        | Unset
        | NonScalingStroke
        | None
    
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)>]
    module AtRule =
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type SpeakAs =
            | Auto
            | Bullets
            | Numbers
            | SpellOut
            | Words
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type System =
            | Additive
            | Alphabetic
            | Cyclic
            | Fixed
            | Numeric
            | Symbolic
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type FontDisplay =
            | Auto
            | Block
            | Fallback
            | Optional
            | Swap
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type FontStretch =
            | Condensed
            | Expanded
            | ExtraCondensed
            | ExtraExpanded
            | Normal
            | SemiCondensed
            | SemiExpanded
            | UltraCondensed
            | UltraExpanded
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type FontStyle =
            | Italic
            | Normal
            | Oblique
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type FontVariant =
            | Jis04
            | Jis78
            | Jis83
            | Jis90
            | Simplified
            | Traditional
            | AllPetiteCaps
            | AllSmallCaps
            | CommonLigatures
            | Contextual
            | DiagonalFractions
            | DiscretionaryLigatures
            | FullWidth
            | HistoricalForms
            | HistoricalLigatures
            | LiningNums
            | NoCommonLigatures
            | NoContextual
            | NoDiscretionaryLigatures
            | NoHistoricalLigatures
            | None
            | Normal
            | OldstyleNums
            | Ordinal
            | PetiteCaps
            | ProportionalNums
            | ProportionalWidth
            | Ruby
            | SlashedZero
            | SmallCaps
            | StackedFractions
            | TabularNums
            | TitlingCaps
            | Unicase

        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type FontWeight =
            | Bold
            | Normal
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type LineGapOverride =
            | Normal
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type BasePalette =
            | Dark
            | Light
    
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type Marks =
            | Crop
            | Cross
            | None
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type PageOrientation =
            | RotateLeft
            | RotateRight
            | Upright
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type Size =
            | A3
            | A4
            | A5
            | B4
            | B5
            | JISB4
            | JISB5
            | Ledger
            | Legal
            | Letter
            | Auto
            | Landscape
            | Portrait
    
    
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)>]
    module DataType =
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type AbsoluteSize =
            | Large
            | Medium
            | Small
            | XLarge
            | XSmall
            | XxLarge
            | XxSmall
            | XxxLarge
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type AnimateableFeature =
            | Contents
            | ScrollPosition
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type Attachment =
            | Fixed
            | Local
            | Scroll
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type BgPosition =
            | Bottom
            | Center
            | Left
            | Right
            | Top
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type BgSize =
            | Auto
            | Contain
            | Cover
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type BlendMode =
            | Color
            | ColorBurn
            | ColorDodge
            | Darken
            | Difference
            | Exclusion
            | HardLight
            | Hue
            | Lighten
            | Luminosity
            | Multiply
            | Normal
            | Overlay
            | Saturation
            | Screen
            | SoftLight
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type Box =
            | BorderBox
            | ContentBox
            | PaddingBox
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type Color =
            | Aliceblue
            | Antiquewhite
            | Aqua
            | Aquamarine
            | Azure
            | Beige
            | Bisque
            | Black
            | Blanchedalmond
            | Blue
            | Blueviolet
            | Brown
            | Burlywood
            | Cadetblue
            | Chartreuse
            | Chocolate
            | Coral
            | Cornflowerblue
            | Cornsilk
            | Crimson
            | Cyan
            | Darkblue
            | Darkcyan
            | Darkgoldenrod
            | Darkgray
            | Darkgreen
            | Darkgrey
            | Darkkhaki
            | Darkmagenta
            | Darkolivegreen
            | Darkorange
            | Darkorchid
            | Darkred
            | Darksalmon
            | Darkseagreen
            | Darkslateblue
            | Darkslategray
            | Darkslategrey
            | Darkturquoise
            | Darkviolet
            | Deeppink
            | Deepskyblue
            | Dimgray
            | Dimgrey
            | Dodgerblue
            | Firebrick
            | Floralwhite
            | Forestgreen
            | Fuchsia
            | Gainsboro
            | Ghostwhite
            | Gold
            | Goldenrod
            | Gray
            | Green
            | Greenyellow
            | Grey
            | Honeydew
            | Hotpink
            | Indianred
            | Indigo
            | Ivory
            | Khaki
            | Lavender
            | Lavenderblush
            | Lawngreen
            | Lemonchiffon
            | Lightblue
            | Lightcoral
            | Lightcyan
            | Lightgoldenrodyellow
            | Lightgray
            | Lightgreen
            | Lightgrey
            | Lightpink
            | Lightsalmon
            | Lightseagreen
            | Lightskyblue
            | Lightslategray
            | Lightslategrey
            | Lightsteelblue
            | Lightyellow
            | Lime
            | Limegreen
            | Linen
            | Magenta
            | Maroon
            | Mediumaquamarine
            | Mediumblue
            | Mediumorchid
            | Mediumpurple
            | Mediumseagreen
            | Mediumslateblue
            | Mediumspringgreen
            | Mediumturquoise
            | Mediumvioletred
            | Midnightblue
            | Mintcream
            | Mistyrose
            | Moccasin
            | Navajowhite
            | Navy
            | Oldlace
            | Olive
            | Olivedrab
            | Orange
            | Orangered
            | Orchid
            | Palegoldenrod
            | Palegreen
            | Paleturquoise
            | Palevioletred
            | Papayawhip
            | Peachpuff
            | Peru
            | Pink
            | Plum
            | Powderblue
            | Purple
            | Rebeccapurple
            | Red
            | Rosybrown
            | Royalblue
            | Saddlebrown
            | Salmon
            | Sandybrown
            | Seagreen
            | Seashell
            | Sienna
            | Silver
            | Skyblue
            | Slateblue
            | Slategray
            | Slategrey
            | Snow
            | Springgreen
            | Steelblue
            | Tan
            | Teal
            | Thistle
            | Tomato
            | Transparent
            | Turquoise
            | Violet
            | Wheat
            | White
            | Whitesmoke
            | Yellow
            | Yellowgreen
            | ActiveBorder
            | ActiveCaption
            | AppWorkspace
            | Background
            | ButtonFace
            | ButtonHighlight
            | ButtonShadow
            | ButtonText
            | CaptionText
            | GrayText
            | Highlight
            | HighlightText
            | InactiveBorder
            | InactiveCaption
            | InactiveCaptionText
            | InfoBackground
            | InfoText
            | Menu
            | MenuText
            | Scrollbar
            | ThreeDDarkShadow
            | ThreeDFace
            | ThreeDHighlight
            | ThreeDLightShadow
            | ThreeDShadow
            | Window
            | WindowFrame
            | WindowText
            | Currentcolor
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type CompatAuto =
            | Button
            | Checkbox
            | Listbox
            | Menulist
            | Meter
            | ProgressBar
            | PushButton
            | Radio
            | Searchfield
            | SliderHorizontal
            | SquareButton
            | Textarea
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type CompositeStyle =
            | Clear
            | Copy
            | DestinationAtop
            | DestinationIn
            | DestinationOut
            | DestinationOver
            | SourceAtop
            | SourceIn
            | SourceOut
            | SourceOver
            | Xor
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type CompositingOperator =
            | Add
            | Exclude
            | Intersect
            | Subtract
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type ContentDistribution =
            | SpaceAround
            | SpaceBetween
            | SpaceEvenly
            | Stretch
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type ContentList =
            | CloseQuote
            | NoCloseQuote
            | NoOpenQuote
            | OpenQuote
            | Contents
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type ContentPosition =
            | Center
            | End
            | FlexEnd
            | FlexStart
            | Start
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type CubicBezierTimingFunction =
            | Ease
            | EaseIn
            | EaseInOut
            | EaseOut
    
        type Dasharray<'TLength> =
            U2<'TLength, obj>
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type DeprecatedSystemColor =
            | ActiveBorder
            | ActiveCaption
            | AppWorkspace
            | Background
            | ButtonFace
            | ButtonHighlight
            | ButtonShadow
            | ButtonText
            | CaptionText
            | GrayText
            | Highlight
            | HighlightText
            | InactiveBorder
            | InactiveCaption
            | InactiveCaptionText
            | InfoBackground
            | InfoText
            | Menu
            | MenuText
            | Scrollbar
            | ThreeDDarkShadow
            | ThreeDFace
            | ThreeDHighlight
            | ThreeDLightShadow
            | ThreeDShadow
            | Window
            | WindowFrame
            | WindowText
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type DisplayInside =
            | MsFlexbox
            | MsGrid
            | WebkitFlex
            | Flex
            | Flow
            | FlowRoot
            | Grid
            | Ruby
            | Table
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type DisplayInternal =
            | RubyBase
            | RubyBaseContainer
            | RubyText
            | RubyTextContainer
            | TableCaption
            | TableCell
            | TableColumn
            | TableColumnGroup
            | TableFooterGroup
            | TableHeaderGroup
            | TableRow
            | TableRowGroup
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type DisplayLegacy =
            | MsInlineFlexbox
            | MsInlineGrid
            | WebkitInlineFlex
            | InlineBlock
            | InlineFlex
            | InlineGrid
            | InlineListItem
            | InlineTable
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type DisplayOutside =
            | Block
            | Inline
            | RunIn
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type EasingFunction =
            | Ease
            | EaseIn
            | EaseInOut
            | EaseOut
            | StepEnd
            | StepStart
            | Linear
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type EastAsianVariantValues =
            | Jis04
            | Jis78
            | Jis83
            | Jis90
            | Simplified
            | Traditional
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type FinalBgLayer =
            | Aliceblue
            | Antiquewhite
            | Aqua
            | Aquamarine
            | Azure
            | Beige
            | Bisque
            | Black
            | Blanchedalmond
            | Blue
            | Blueviolet
            | Brown
            | Burlywood
            | Cadetblue
            | Chartreuse
            | Chocolate
            | Coral
            | Cornflowerblue
            | Cornsilk
            | Crimson
            | Cyan
            | Darkblue
            | Darkcyan
            | Darkgoldenrod
            | Darkgray
            | Darkgreen
            | Darkgrey
            | Darkkhaki
            | Darkmagenta
            | Darkolivegreen
            | Darkorange
            | Darkorchid
            | Darkred
            | Darksalmon
            | Darkseagreen
            | Darkslateblue
            | Darkslategray
            | Darkslategrey
            | Darkturquoise
            | Darkviolet
            | Deeppink
            | Deepskyblue
            | Dimgray
            | Dimgrey
            | Dodgerblue
            | Firebrick
            | Floralwhite
            | Forestgreen
            | Fuchsia
            | Gainsboro
            | Ghostwhite
            | Gold
            | Goldenrod
            | Gray
            | Green
            | Greenyellow
            | Grey
            | Honeydew
            | Hotpink
            | Indianred
            | Indigo
            | Ivory
            | Khaki
            | Lavender
            | Lavenderblush
            | Lawngreen
            | Lemonchiffon
            | Lightblue
            | Lightcoral
            | Lightcyan
            | Lightgoldenrodyellow
            | Lightgray
            | Lightgreen
            | Lightgrey
            | Lightpink
            | Lightsalmon
            | Lightseagreen
            | Lightskyblue
            | Lightslategray
            | Lightslategrey
            | Lightsteelblue
            | Lightyellow
            | Lime
            | Limegreen
            | Linen
            | Magenta
            | Maroon
            | Mediumaquamarine
            | Mediumblue
            | Mediumorchid
            | Mediumpurple
            | Mediumseagreen
            | Mediumslateblue
            | Mediumspringgreen
            | Mediumturquoise
            | Mediumvioletred
            | Midnightblue
            | Mintcream
            | Mistyrose
            | Moccasin
            | Navajowhite
            | Navy
            | Oldlace
            | Olive
            | Olivedrab
            | Orange
            | Orangered
            | Orchid
            | Palegoldenrod
            | Palegreen
            | Paleturquoise
            | Palevioletred
            | Papayawhip
            | Peachpuff
            | Peru
            | Pink
            | Plum
            | Powderblue
            | Purple
            | Rebeccapurple
            | Red
            | Rosybrown
            | Royalblue
            | Saddlebrown
            | Salmon
            | Sandybrown
            | Seagreen
            | Seashell
            | Sienna
            | Silver
            | Skyblue
            | Slateblue
            | Slategray
            | Slategrey
            | Snow
            | Springgreen
            | Steelblue
            | Tan
            | Teal
            | Thistle
            | Tomato
            | Transparent
            | Turquoise
            | Violet
            | Wheat
            | White
            | Whitesmoke
            | Yellow
            | Yellowgreen
            | ActiveBorder
            | ActiveCaption
            | AppWorkspace
            | Background
            | ButtonFace
            | ButtonHighlight
            | ButtonShadow
            | ButtonText
            | CaptionText
            | GrayText
            | Highlight
            | HighlightText
            | InactiveBorder
            | InactiveCaption
            | InactiveCaptionText
            | InfoBackground
            | InfoText
            | Menu
            | MenuText
            | Scrollbar
            | ThreeDDarkShadow
            | ThreeDFace
            | ThreeDHighlight
            | ThreeDLightShadow
            | ThreeDShadow
            | Window
            | WindowFrame
            | WindowText
            | Currentcolor
            | Bottom
            | Center
            | Left
            | Right
            | Top
            | NoRepeat
            | Repeat
            | RepeatX
            | RepeatY
            | Round
            | Space
            | Fixed
            | Local
            | Scroll
            | BorderBox
            | ContentBox
            | PaddingBox
            | None
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type FontStretchAbsolute =
            | Condensed
            | Expanded
            | ExtraCondensed
            | ExtraExpanded
            | Normal
            | SemiCondensed
            | SemiExpanded
            | UltraCondensed
            | UltraExpanded
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type FontWeightAbsolute =
            | Bold
            | Normal
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type GenericFamily =
            | Cursive
            | Fantasy
            | Monospace
            | SansSerif
            | Serif
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type GeometryBox =
            | BorderBox
            | ContentBox
            | PaddingBox
            | FillBox
            | MarginBox
            | StrokeBox
            | ViewBox
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type GridLine =
            | Auto
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type LineStyle =
            | Dashed
            | Dotted
            | Double
            | Groove
            | Hidden
            | Inset
            | None
            | Outset
            | Ridge
            | Solid
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type LineWidth =
            | Medium
            | Thick
            | Thin
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type MaskLayer =
            | Bottom
            | Center
            | Left
            | Right
            | Top
            | NoRepeat
            | Repeat
            | RepeatX
            | RepeatY
            | Round
            | Space
            | BorderBox
            | ContentBox
            | PaddingBox
            | FillBox
            | MarginBox
            | StrokeBox
            | ViewBox
            | Add
            | Exclude
            | Intersect
            | Subtract
            | Alpha
            | Luminance
            | MatchSource
            | NoClip
            | None
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type MaskingMode =
            | Alpha
            | Luminance
            | MatchSource
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type NamedColor =
            | Aliceblue
            | Antiquewhite
            | Aqua
            | Aquamarine
            | Azure
            | Beige
            | Bisque
            | Black
            | Blanchedalmond
            | Blue
            | Blueviolet
            | Brown
            | Burlywood
            | Cadetblue
            | Chartreuse
            | Chocolate
            | Coral
            | Cornflowerblue
            | Cornsilk
            | Crimson
            | Cyan
            | Darkblue
            | Darkcyan
            | Darkgoldenrod
            | Darkgray
            | Darkgreen
            | Darkgrey
            | Darkkhaki
            | Darkmagenta
            | Darkolivegreen
            | Darkorange
            | Darkorchid
            | Darkred
            | Darksalmon
            | Darkseagreen
            | Darkslateblue
            | Darkslategray
            | Darkslategrey
            | Darkturquoise
            | Darkviolet
            | Deeppink
            | Deepskyblue
            | Dimgray
            | Dimgrey
            | Dodgerblue
            | Firebrick
            | Floralwhite
            | Forestgreen
            | Fuchsia
            | Gainsboro
            | Ghostwhite
            | Gold
            | Goldenrod
            | Gray
            | Green
            | Greenyellow
            | Grey
            | Honeydew
            | Hotpink
            | Indianred
            | Indigo
            | Ivory
            | Khaki
            | Lavender
            | Lavenderblush
            | Lawngreen
            | Lemonchiffon
            | Lightblue
            | Lightcoral
            | Lightcyan
            | Lightgoldenrodyellow
            | Lightgray
            | Lightgreen
            | Lightgrey
            | Lightpink
            | Lightsalmon
            | Lightseagreen
            | Lightskyblue
            | Lightslategray
            | Lightslategrey
            | Lightsteelblue
            | Lightyellow
            | Lime
            | Limegreen
            | Linen
            | Magenta
            | Maroon
            | Mediumaquamarine
            | Mediumblue
            | Mediumorchid
            | Mediumpurple
            | Mediumseagreen
            | Mediumslateblue
            | Mediumspringgreen
            | Mediumturquoise
            | Mediumvioletred
            | Midnightblue
            | Mintcream
            | Mistyrose
            | Moccasin
            | Navajowhite
            | Navy
            | Oldlace
            | Olive
            | Olivedrab
            | Orange
            | Orangered
            | Orchid
            | Palegoldenrod
            | Palegreen
            | Paleturquoise
            | Palevioletred
            | Papayawhip
            | Peachpuff
            | Peru
            | Pink
            | Plum
            | Powderblue
            | Purple
            | Rebeccapurple
            | Red
            | Rosybrown
            | Royalblue
            | Saddlebrown
            | Salmon
            | Sandybrown
            | Seagreen
            | Seashell
            | Sienna
            | Silver
            | Skyblue
            | Slateblue
            | Slategray
            | Slategrey
            | Snow
            | Springgreen
            | Steelblue
            | Tan
            | Teal
            | Thistle
            | Tomato
            | Transparent
            | Turquoise
            | Violet
            | Wheat
            | White
            | Whitesmoke
            | Yellow
            | Yellowgreen
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type PageSize =
            | A3
            | A4
            | A5
            | B4
            | B5
            | JISB4
            | JISB5
            | Ledger
            | Legal
            | Letter
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type Paint =
            | Aliceblue
            | Antiquewhite
            | Aqua
            | Aquamarine
            | Azure
            | Beige
            | Bisque
            | Black
            | Blanchedalmond
            | Blue
            | Blueviolet
            | Brown
            | Burlywood
            | Cadetblue
            | Chartreuse
            | Chocolate
            | Coral
            | Cornflowerblue
            | Cornsilk
            | Crimson
            | Cyan
            | Darkblue
            | Darkcyan
            | Darkgoldenrod
            | Darkgray
            | Darkgreen
            | Darkgrey
            | Darkkhaki
            | Darkmagenta
            | Darkolivegreen
            | Darkorange
            | Darkorchid
            | Darkred
            | Darksalmon
            | Darkseagreen
            | Darkslateblue
            | Darkslategray
            | Darkslategrey
            | Darkturquoise
            | Darkviolet
            | Deeppink
            | Deepskyblue
            | Dimgray
            | Dimgrey
            | Dodgerblue
            | Firebrick
            | Floralwhite
            | Forestgreen
            | Fuchsia
            | Gainsboro
            | Ghostwhite
            | Gold
            | Goldenrod
            | Gray
            | Green
            | Greenyellow
            | Grey
            | Honeydew
            | Hotpink
            | Indianred
            | Indigo
            | Ivory
            | Khaki
            | Lavender
            | Lavenderblush
            | Lawngreen
            | Lemonchiffon
            | Lightblue
            | Lightcoral
            | Lightcyan
            | Lightgoldenrodyellow
            | Lightgray
            | Lightgreen
            | Lightgrey
            | Lightpink
            | Lightsalmon
            | Lightseagreen
            | Lightskyblue
            | Lightslategray
            | Lightslategrey
            | Lightsteelblue
            | Lightyellow
            | Lime
            | Limegreen
            | Linen
            | Magenta
            | Maroon
            | Mediumaquamarine
            | Mediumblue
            | Mediumorchid
            | Mediumpurple
            | Mediumseagreen
            | Mediumslateblue
            | Mediumspringgreen
            | Mediumturquoise
            | Mediumvioletred
            | Midnightblue
            | Mintcream
            | Mistyrose
            | Moccasin
            | Navajowhite
            | Navy
            | Oldlace
            | Olive
            | Olivedrab
            | Orange
            | Orangered
            | Orchid
            | Palegoldenrod
            | Palegreen
            | Paleturquoise
            | Palevioletred
            | Papayawhip
            | Peachpuff
            | Peru
            | Pink
            | Plum
            | Powderblue
            | Purple
            | Rebeccapurple
            | Red
            | Rosybrown
            | Royalblue
            | Saddlebrown
            | Salmon
            | Sandybrown
            | Seagreen
            | Seashell
            | Sienna
            | Silver
            | Skyblue
            | Slateblue
            | Slategray
            | Slategrey
            | Snow
            | Springgreen
            | Steelblue
            | Tan
            | Teal
            | Thistle
            | Tomato
            | Transparent
            | Turquoise
            | Violet
            | Wheat
            | White
            | Whitesmoke
            | Yellow
            | Yellowgreen
            | ActiveBorder
            | ActiveCaption
            | AppWorkspace
            | Background
            | ButtonFace
            | ButtonHighlight
            | ButtonShadow
            | ButtonText
            | CaptionText
            | GrayText
            | Highlight
            | HighlightText
            | InactiveBorder
            | InactiveCaption
            | InactiveCaptionText
            | InfoBackground
            | InfoText
            | Menu
            | MenuText
            | Scrollbar
            | ThreeDDarkShadow
            | ThreeDFace
            | ThreeDHighlight
            | ThreeDLightShadow
            | ThreeDShadow
            | Window
            | WindowFrame
            | WindowText
            | Currentcolor
            | Child
            | ContextFill
            | ContextStroke
            | None
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type Position =
            | Bottom
            | Center
            | Left
            | Right
            | Top
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type Quote =
            | CloseQuote
            | NoCloseQuote
            | NoOpenQuote
            | OpenQuote
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type RepeatStyle =
            | NoRepeat
            | Repeat
            | RepeatX
            | RepeatY
            | Round
            | Space
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type SelfPosition =
            | Center
            | End
            | FlexEnd
            | FlexStart
            | SelfEnd
            | SelfStart
            | Start
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type SingleAnimation =
            | Ease
            | EaseIn
            | EaseInOut
            | EaseOut
            | StepEnd
            | StepStart
            | Linear
            | Alternate
            | AlternateReverse
            | Normal
            | Reverse
            | Backwards
            | Both
            | Forwards
            | None
            | Auto
            | Infinite
            | Paused
            | Running
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type SingleAnimationComposition =
            | Accumulate
            | Add
            | Replace
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type SingleAnimationDirection =
            | Alternate
            | AlternateReverse
            | Normal
            | Reverse
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type SingleAnimationFillMode =
            | Backwards
            | Both
            | Forwards
            | None
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type SingleAnimationTimeline =
            | Auto
            | None
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type SingleTransition =
            | Ease
            | EaseIn
            | EaseInOut
            | EaseOut
            | StepEnd
            | StepStart
            | Linear
            | All
            | AllowDiscrete
            | None
            | Normal
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type StepTimingFunction =
            | StepEnd
            | StepStart
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type TimelineRangeName =
            | Contain
            | Cover
            | Entry
            | EntryCrossing
            | Exit
            | ExitCrossing
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type TrackBreadth =
            | Auto
            | MaxContent
            | MinContent
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type ViewportLength =
            | Auto
    
        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.KebabCase)>]
        type VisualBox =
            | BorderBox
            | ContentBox
            | PaddingBox
