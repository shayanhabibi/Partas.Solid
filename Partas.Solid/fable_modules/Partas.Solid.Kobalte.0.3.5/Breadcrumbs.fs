namespace Partas.Solid.Kobalte

open Fable.Core
open Browser.Types
open Partas.Solid

[<Erase; Import("Root", Spec.breadcrumbs)>]
type Breadcrumbs() =
    inherit nav()
    interface Polymorph // the custom is that if a property takes an element or string, then the apostrophised version takes the element
    [<DefaultValue>] val mutable separator' : HtmlElement  //v0.13.9
    [<DefaultValue>] val mutable separator : string   //v0.13.9
    // [<DefaultValue>] val mutable translations : string  // todo- support  //v0.13.9

[<Erase; RequireQualifiedAccess>]
module Breadcrumbs =
    /// <summary>
    /// data-current: whether the breadcrumb link represents the current page
    /// </summary>
    [<Erase; Import("Link", Spec.breadcrumbs)>]
    type Link() =
        inherit Kobalte.Link()
        interface Polymorph
        [<DefaultValue>] val mutable current : bool  //v0.13.9
        [<DefaultValue>] val mutable disabled : bool  //v0.13.9
    [<Erase; Import("Separator", Spec.breadcrumbs)>]
    type Separator() = //v0.13.9
        inherit span()
        interface Polymorph

[<Erase; AutoOpen>]
module BreadcrumbContext =
    [<AllowNullLiteral>]
    [<Interface>]
    type BreadcrumbsContext =
        abstract member separator: (unit -> U2<string, Element>) with get, set
        
    [<Import("useBreadcrumbsContext", Spec.breadcrumbs)>]
    let useBreadcrumbsContext () : BreadcrumbsContext = nativeOnly


