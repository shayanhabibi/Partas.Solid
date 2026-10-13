namespace Partas.Solid

open System.Runtime.CompilerServices
open JetBrains.Annotations
open Fable.Core
open Fable.Core.JsInterop
open Partas.Solid.Experimental.U
open Fable.Core.TS
open Fable.Core.TS.Dom

/// Hand-written element members the JSX generator leaves alone: children and the structured `style`. Attributes and
/// event handlers are generated into HtmlAttributes.fs by tools/JsxGen.
[<AutoOpen>]
module HtmlEvents =
    type HtmlContainer with
        [<Erase>]
        member _.children
            with set (value: HtmlElement) = JS.undefined
            and get (): HtmlElement = JS.undefined

    type FlowContainer<'A> with
        [<Erase>]
        member _.children
            with set(value: 'A) = JS.undefined
            and get (): 'A = JS.undefined

    type HTMLAttributes with
        [<EB(EBState.Never); Erase>]
        member _.style''
            with set (_: obj) = ()
            and get (): obj = unbox ()

        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<div style='", Suffix = "' />")>]
        member this.style
            with inline set (value: string) = this.style'' <- value
            and inline [<Erase>] get (): string = !!this.style''

        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<div inlist='", Suffix = "' >")>]
        member _.inlist
            with set (_: obj) = ()
            and [<Erase>] get () = unbox ()
