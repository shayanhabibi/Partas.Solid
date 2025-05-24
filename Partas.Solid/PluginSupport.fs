namespace Partas.Solid

// Attributes that are matched in Partas.Solid.FablePlugin to support special transformations

open System

/// Used for types that have inbuilt Builder computation support but are an imported
/// component from an external library.
/// This prevents the obfuscation/generalisation of property names which lets us be
/// more confident in less strict patterns matching the appropriate targets with discrimination.
/// We will add the import attribute afterwards
// [<AttributeUsage(AttributeTargets.Class)>]
type PartasImportAttribute(selector: string, path: string) =
    inherit Attribute()
/// <summary>
/// Used for importing components or types from a library which serves precomposed
/// types via a proxy such as Motion. This is not required, since we can construct our own
/// proxies, but there is no reason to increase bundle sizes doing so.
/// </summary>
/// <param name="key">The extra parameter 'key' is '.' appended to the selector</param>
type PartasProxyImportAttribute(key: string, selector: string, path: string) =
    inherit Attribute()

