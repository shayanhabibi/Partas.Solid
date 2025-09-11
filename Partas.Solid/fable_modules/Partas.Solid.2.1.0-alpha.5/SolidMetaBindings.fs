namespace Partas.Solid.Meta

open Fable.Core
open Partas.Solid

[<AutoOpen>]
module Bindings =
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    module Spec =
        [<Literal>]
        let path = "@solidjs/meta"

    [<Import("MetaProvider", Spec.path)>]
    type MetaProvider() =
        interface FragmentNode

    [<Import("Title", Spec.path)>]
    type Title() =
        inherit title()

    [<Import("Style", Spec.path)>]
    type Style() =
        inherit style()

    [<Import("Link", Spec.path)>]
    type Link() =
        inherit link()

    [<Import("Meta", Spec.path)>]
    type Meta() =
        inherit meta()

    [<Import("Base", Spec.path)>]
    type Base() =
        inherit base'()
