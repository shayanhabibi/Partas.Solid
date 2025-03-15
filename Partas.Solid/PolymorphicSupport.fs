/// Provides scaffolding plugin support for Kobalte
/// polymorphism
module Partas.Solid.Polymorphism

open System.Runtime.CompilerServices
open Partas.Solid
open Fable.Core

#nowarn 64

/// If a tag has some capacity for polymorphism, then that must be explicitly
/// labeled by interfacing with Polymorph. All Kobalte base elements do this.
[<Erase>]
type Polymorph =
    inherit HtmlTag

[<Erase; Extension>]
type PolymorphicExtensions =
    /// For Kobalte or other supporting libraries
    [<Erase; Extension>]
    static member as'<'Base when 'Base :> Polymorph> (
            this: 'Base,
            morph: #HtmlTag
        ): 'Base = this
    /// For Kobalte or other supporting libraries
    [<Erase; Extension>]
    static member as'<'Base when 'Base :> Polymorph > (
            this: 'Base,
            morph: TagValue
        ): 'Base = this