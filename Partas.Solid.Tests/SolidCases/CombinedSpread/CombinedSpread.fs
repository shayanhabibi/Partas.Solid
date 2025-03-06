module Partas.Solid.Tests.SolidCases.CombinedSpread

open Partas.Solid
open Fable.Core

type [<Erase>] SimpleCombination() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.CombinedSpread =
        props.class' <- "ClassDefault"
        div() {
            "Some text"
            span(class' = props.class') { "More text" }
            div().spread(props) {
                "All but class have been spread into this tag!"
            }
            
        }

type [<Erase>] Lib =
    [<Import("twMerge","tailwind-merge")>]
    static member inline twMerge ( classes : string ) : string = jsNative
    [<Import("clsx","clsx")>]
    static member inline clsx ( classes : obj ) : string = jsNative
    static member inline cn ( classes : string * bool ) : string = classes |> Lib.clsx |> Lib.twMerge
    static member inline cn ( classes : string array ) : string = classes |> Lib.clsx |> Lib.twMerge
    static member inline cn ( classes : string ) : string = classes |> Lib.clsx |> Lib.twMerge
    [<Import("cva","class-variance-authority")>]
    static member inline cva ( orig : string ) ( object : 'T) : obj -> string = jsNative

[<AutoOpen; Erase>]
module Operators =
    [<Emit("$0 && $1")>]
    let (&&=) (conditional: 'T) (output: 'M): 'M = jsNative
    
[<AutoOpen>]
module [<Erase>] button =
    let variants =
        Lib.cva
            "inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium transition-colors focus-visible:outline-hidden focus-visible:ring-1 focus-visible:ring-ring disabled:pointer-events-none disabled:opacity-50 [&_svg]:pointer-events-none [&_svg]:size-4 [&_svg]:shrink-0"
            {|
             variants = {|
                          variant = {|
                                      ``default`` = "bg-primary text-primary-foreground shadow-sm hover:bg-primary/90"
                                      destructive = "bg-destructive text-destructive-foreground shadow-xs hover:bg-destructive/90"
                                      outline = "border border-input bg-background shadow-xs hover:bg-accent hover:text-accent-foreground"
                                      secondary = "bg-secondary text-secondary-foreground shadow-xs hover:bg-secondary/80"
                                      ghost = "hover:bg-accent hover:text-accent-foreground"
                                      link = "text-primary underline-offset-4 hover:underline"
                                      |}
                          size = {|
                                   ``default`` = "h-9 px-4 py-2"
                                   sm = "h-8 rounded-md px-3 text-xs"
                                   lg = "h-10 rounded-md px-8"
                                   icon = "h-9 w-9"
                                   |}
                          |}
             defaultVariants = {|
                                 variant = "default"
                                 size = "default"
                                |}
            |}
            
    [<Erase>]
    type size =
        static member inline default' : size = unbox "default"
        static member inline sm : size = unbox "sm"
        static member inline lg : size = unbox "lg"
        static member inline icon : size = unbox "icon"
    [<Erase>]
    type variant =
        static member inline default' : variant = unbox "default"
        static member inline outline : variant = unbox "outline"
        static member inline ghost : variant = unbox "ghost"
        static member inline link : variant = unbox "link"
        static member inline destructive : variant = unbox "destructive"
        static member inline secondary : variant = unbox "secondary"

[<Erase>]
type Button() =
    inherit button()
    [<Erase>]
    member val size: size = unbox null with get,set
    [<Erase>]
    member val variant: variant = unbox null with get,set
    [<SolidTypeComponent>]
    member props.constructor =
        button(class' = button.variants({|size = props.size; variant = props.variant|}))
            .spread props