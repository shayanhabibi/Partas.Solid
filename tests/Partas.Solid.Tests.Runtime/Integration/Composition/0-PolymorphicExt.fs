module Partas.Solid.Tests.Runtime.Integration.Composition.PolymorphicExt

open Partas.Solid
open Fable.Core

/// Custom polymorphic attribute through the `__PARTAS_POLYMORPHIC__` prefix.
[<Erase; System.Runtime.CompilerServices.Extension>]
type CustomPolymorphicExtensions =
    [<Erase; System.Runtime.CompilerServices.Extension>]
    static member __PARTAS_POLYMORPHIC__render<'Base when 'Base :> Polymorph>(this: 'Base, morph: #HtmlTag) : 'Base = this

