namespace Fable.Core

open Fable.Core.TS

/// <summary>
/// Global values that Xantham.Fable.Core.TS types but does not yet bind. Opening <c>Fable.Core</c> brings them in.
/// </summary>
/// <remarks>
/// This shadows the <c>globalThis</c> binding of Xantham's <c>TSExtensions</c>, which is not in a published release
/// yet. When it is, <c>globalThis</c> here can be removed, or left: the two bindings are the same global.
/// </remarks>
[<AutoOpen>]
module PartasGlobals =
    /// <summary>The JavaScript <c>globalThis</c>.</summary>
    [<Global>]
    let globalThis: GlobalThis = jsNative

    /// <summary>The page's <c>document</c>.</summary>
    [<Global>]
    let document: Dom.Document = jsNative

    /// <summary>The browser <c>window</c>.</summary>
    [<Global>]
    let window: Dom.Window = jsNative
