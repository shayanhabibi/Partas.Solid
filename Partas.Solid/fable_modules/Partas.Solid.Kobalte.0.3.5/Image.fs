namespace Partas.Solid.Kobalte

open Fable.Core
open Partas.Solid

[<Erase; Import("Root", Spec.image)>]
type Image() =
    interface RegularNode
    interface Polymorph
    [<DefaultValue>] val mutable fallbackDelay : int 
    [<DefaultValue>] val mutable onLoadingStatusChange : LoadingStatus -> unit 

[<Erase; RequireQualifiedAccess>]
module Image =
    [<Erase; Import("Fallback", Spec.image)>]
    type Fallback() =
        inherit span()
        interface Polymorph
    [<Erase; Import("Img", Spec.image)>]
    type Img() =
        inherit img()
        interface Polymorph

[<Erase; AutoOpen>]
module ImageContext =
    [<AllowNullLiteral>]
    [<Interface>]
    type ImageContext =
        abstract member fallbackDelay: Accessor<float option> with get, set
        abstract member imageLoadingStatus: Accessor<LoadingStatus> with get, set
        abstract member onImageLoadingStatusChange: (LoadingStatus -> unit) with get, set
    
    [<ImportMember(Spec.image)>]
    let useImageContext (): ImageContext = jsNative
