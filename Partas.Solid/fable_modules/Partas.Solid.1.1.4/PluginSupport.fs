namespace Partas.Solid

// Attributes that are matched in Partas.Solid.FablePlugin to support special transformations

open System
open Fable.Core

/// Used for types that have inbuilt Builder computation support but are an imported
/// component from an external library.
/// This prevents the obfuscation/generalisation of property names which lets us be
/// more confident in less strict patterns matching the appropriate targets with discrimination.
/// We will add the import attribute afterwards
// [<AttributeUsage(AttributeTargets.Class)>]
type PartasImportAttribute(selector: string, path: string) =
    inherit Attribute()
