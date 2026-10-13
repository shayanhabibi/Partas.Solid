module Partas.Solid.Tests.Runtime.Integration.Composition.LazyTarget

open Partas.Solid
open Fable.Core

/// Target of the lazy import (named export).
[<Erase>]
type LazyPanel() =
    inherit div()

    [<Erase>]
    member val title: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "lazy-panel") {
            h4 (class' = "lazy-title") { props.title }
            div (class' = "lazy-body") { props.children }
        }
