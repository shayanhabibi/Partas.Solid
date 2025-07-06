/// Provides scaffolding plugin support for Kobalte
/// polymorphism
[<AutoOpen>]
module Partas.Solid.Polymorphism

open System.Runtime.CompilerServices
open Partas.Solid
open Fable.Core

#nowarn 64

/// <summary>
/// Support for polymorphism is explicitly provided by interfacing with the Polymorph type.<br/>
/// By default, Partas.Solid provides support for <c>Kobalte</c> polymorphism via the <c>.as'</c> attribute.<br/><br/>
/// Custom attributes to support polymorphism can be added by providing an alias to a member which is prefaced with
/// <c>__PARTAS_POLYMORPHIC__</c>.
/// </summary>
/// <remarks>
/// Different libraries accept different values for polymorphic attributes. Some accept the function identifier itself such
/// as Kobalte, while others such as ArkUI will take a string instead.<br/>
/// Partas.Solid polymorphic support transpiles the TagValues into identifiers.
/// </remarks>
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