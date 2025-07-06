namespace Partas.Solid.Kobalte

open Fable.Core
open Partas.Solid

[<AllowNullLiteral>]
[<Interface>]
type ColorModeStorageManager =
    /// <summary>
    /// The type of storage.
    /// </summary>
    abstract member ``type``: Theme.Config.Storage with get, set
    /// <summary>
    /// Whether it's an SSR environment.
    /// </summary>
    abstract member ssr: bool with get, set
    /// <summary>
    /// Get the color mode from the storage.
    /// </summary>
    abstract member get: ?fallback: Theme.Config.Mode -> Theme.Config.Mode option with get, set
    /// <summary>
    /// Save the color mode in the storage.
    /// </summary>
    abstract member set: (Theme.Config.Mode -> unit) with get, set


[<AllowNullLiteral>]
[<Interface>]
type ColorModeContextType =
    abstract member colorMode: Accessor<Theme.Mode> with get, set
    abstract member setColorMode: (Theme.Config.Mode -> unit) with get, set
    abstract member toggleColorMode: (unit -> unit) with get, set

type ColorModeProvider() =
    interface RegularNode
    /// <summary>
    /// The initial color mode to use.
    /// </summary>
    [<DefaultValue>] val mutable initialColorMode: Theme.Mode
    /// <summary>
    /// Whether css transitions should be disabled during the color mode changes.
    /// </summary>
    [<DefaultValue>] val mutable disableTransitionOnChange: bool
    /// <summary>
    /// The color mode storage manager, either localStorage or cookie.
    /// </summary>
    [<DefaultValue>] val mutable storageManager: ColorModeStorageManager
    
type ColorModeScript() =
    interface RegularNode
    /// <summary>
    /// The initial color mode to use.
    /// </summary>
    [<DefaultValue>] val mutable initialColorMode: Theme.Config.Mode
    /// <summary>
    /// The type of the color mode storage manager, either localStorage or cookie.
    /// </summary>
    [<DefaultValue>] val mutable storageType: Theme.Config.Storage
    /// <summary>
    /// The key used to store color mode preference in localStorage or cookie.
    /// </summary>
    [<DefaultValue>] val mutable storageKey: string
    [<DefaultValue>] val mutable nonce: string

[<AutoOpen; Erase>]
type ColorMode =
    /// <summary>
    /// Primitive that reads from <c>ColorModeProvider</c> context,
    /// Returns the color mode and function to toggle it.
    /// </summary>
    [<Import("useColorMode", "@kobalte/core/color-mode")>]
    static member useColorMode () : ColorModeContextType = nativeOnly
    /// <summary>
    /// Change value based on color mode.
    /// </summary>
    /// <example>
    /// <code lang="js">
    /// const Icon = useColorModeValue(MoonIcon, SunIcon)
    /// </code>
    /// </example>
    /// <param name="light">
    /// the light mode value
    /// </param>
    /// <param name="dark">
    /// the dark mode value
    /// </param>
    /// <returns>
    /// A memoized value based on the color mode.
    /// </returns>
    [<Import("useColorModeValue", "@kobalte/core/color-mode")>]
    static member useColorModeValue<'TLight, 'TDark> (light: 'TLight, dark: 'TDark) : Accessor<U2<'TLight, 'TDark>> = nativeOnly
    /// <summary>
    /// Change value based on color mode.
    /// </summary>
    /// <example>
    /// <code lang="js">
    /// const Icon = useColorModeValue(MoonIcon, SunIcon)
    /// </code>
    /// </example>
    /// <param name="light">
    /// the light mode value
    /// </param>
    /// <param name="dark">
    /// the dark mode value
    /// </param>
    /// <returns>
    /// A memoized value based on the color mode.
    /// </returns>
    [<Import("useColorModeValue", "@kobalte/core/color-mode")>]
    static member useColorModeValue<'T> (light: 'T, dark: 'T) : Accessor<'T> = nativeOnly
    
    [<Import("createLocalStorageManager", "@kobalte/core/color-mode")>]
    static member createLocalStorageManager (key: string) : ColorModeStorageManager = nativeOnly
    [<Import("createCookieStorageManager", "@kobalte/core/color-mode")>]
    static member createCookieStorageManager (key: string, ?cookie: string) : ColorModeStorageManager = nativeOnly
    [<Import("cookieStorageManagerSSR", "@kobalte/core/color-mode")>]
    static member cookieStorageManagerSSR (cookie: string) : ColorModeStorageManager = nativeOnly





