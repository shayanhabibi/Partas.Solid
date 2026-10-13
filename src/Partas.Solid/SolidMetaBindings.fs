namespace Partas.Solid.Meta

open Fable.Core
open Partas.Solid

/// <summary>
/// Bindings for <c>@solidjs/meta</c> 1.0 (Solid 2), built on <c>useHead</c> from <c>@solidjs/web</c>. No provider is
/// needed: render the tag components anywhere and they manage <c>document.head</c>. Tags reach the head in a
/// microtask after the update that renders them.
/// </summary>
[<AutoOpen>]
module Bindings =
    [<EB(EBState.Never)>]
    module Spec =
        [<Literal>]
        let path = "@solidjs/meta"

    /// <summary>
    /// Groups its child head tags into one replacement set. Tags with the same identity inside one group coexist
    /// (several <c>og:image</c>s); a later group replaces an earlier group's tags per identity, and restores them
    /// when it is disposed. A <c>Head</c> inside another <c>Head</c> forms its own group: render bare tags instead to
    /// add to the surrounding set.
    /// </summary>
    [<Import("Head", Spec.path)>]
    type Head() =
        interface FragmentNode

    [<Import("Title", Spec.path)>]
    type Title() =
        inherit title()
        /// <summary>Identity of the tag, so a later tag with the same key replaces it.</summary>
        [<Erase; DefaultValue>]
        val mutable key: string

    [<Import("Style", Spec.path)>]
    type Style() =
        inherit style()
        /// <summary>Identity of the tag, so a later tag with the same key replaces it.</summary>
        [<Erase; DefaultValue>]
        val mutable key: string

    [<Import("Script", Spec.path)>]
    type Script() =
        inherit script()
        /// <summary>Identity of the tag, so a later tag with the same key replaces it.</summary>
        [<Erase; DefaultValue>]
        val mutable key: string

    [<Import("Link", Spec.path)>]
    type Link() =
        inherit link()
        /// <summary>Identity of the tag, so a later tag with the same key replaces it.</summary>
        [<Erase; DefaultValue>]
        val mutable key: string

    /// <summary>
    /// A <c>link</c> with <c>rel="stylesheet"</c> already set. Solid 2 warms it with a style preload and holds the update
    /// until the sheet has loaded, so the page never paints unstyled.
    /// </summary>
    [<Import("Stylesheet", Spec.path)>]
    type Stylesheet() =
        inherit link()
        /// <summary>Identity of the tag, so a later tag with the same key replaces it.</summary>
        [<Erase; DefaultValue>]
        val mutable key: string

    [<Import("Meta", Spec.path)>]
    type Meta() =
        inherit meta()
        /// <summary>Identity of the tag, so a later tag with the same key replaces it.</summary>
        [<Erase; DefaultValue>]
        val mutable key: string

    /// <summary>
    /// Shell-only: applied when the page is rendered on the server. Solid 2 ignores it on the client, with a warning.
    /// </summary>
    [<Import("Base", Spec.path)>]
    type Base() =
        inherit base'()
