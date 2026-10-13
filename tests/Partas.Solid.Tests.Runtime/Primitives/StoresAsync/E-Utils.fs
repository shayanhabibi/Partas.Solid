module Partas.Solid.Tests.Runtime.Primitives.StoresAsync.Utils

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

// ---------------------------------------------------------------------------------------------
// omit / merge / isStatic
// ---------------------------------------------------------------------------------------------

type ButtonProps =
    { label: string
      size: string
      variant: string
      disabled: bool }

let buttonProps () =
    { label = "Save"
      size = "lg"
      variant = "primary"
      disabled = false }

/// omit with the ParamArray (key list) overload.
let omitSizeVariant (props: ButtonProps) : obj = box (omit (props, "size", "variant"))

/// omit with the predicate overload: hide every "$"-prefixed key.
let omitDollarKeys () : obj =
    let props = createObj [ "$theme" ==> "dark"; "$slot" ==> 1; "id" ==> "x"; "label" ==> "L" ]
    box (omit (props, fun (k: string) -> k.StartsWith "$"))

/// Predicate omit over an object that carries a symbol-keyed prop (as Solid props can).
let omitWithSymbolKey (sym: obj) : obj =
    let props = createObj [ "$hidden" ==> 1; "id" ==> "x" ]
    props?(sym) <- "symbol-value"
    box (omit (props, fun (k: string) -> k.StartsWith "$"))

type Counter = { mutable n: int }

type OmitStoreHarness =
    { rest: obj
      bump: unit -> unit
      dispose: unit -> unit }

/// omit over a store is a live view: store writes are visible through it after flush.
let omitOverStore () : OmitStoreHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let store, setStore = createStore { n = 1 }

        { rest = box (omit (store.Value, "missing"))
          bump =
            fun () ->
                setStore (fun s ->
                    s.n <- s.n + 1
                    s)
          dispose = dispose })

/// merge: later sources win; falsy sources are skipped.
let mergeDefaults (props: obj) : obj =
    merge (box {| size = "md"; variant = "ghost"; label = "default" |}, props)

let mergeSingle (props: obj) : obj = merge props

let mergeWithFalsy (props: obj) : obj = merge (props, null, box false)

/// merge a store with a plain defaults object; returns the merged view and a writer.
let mergeOverStore () =
    createRoot (fun (dispose: unit -> unit) ->
        let store, setStore = createStore { n = 1 }
        let merged: obj = merge (box {| n = 0; extra = "e" |}, box store.Value)

        {| merged = merged
           setN =
            fun v ->
                setStore (fun s ->
                    s.n <- v
                    s)
           dispose = dispose |})

type IDynamicProps =
    abstract ``as``: string
    abstract dyn: string

/// isStatic over plain records, F# object expressions (getters), stores and merge/omit views.
let isStaticReport () =
    createRoot (fun (dispose: unit -> unit) ->
        let sig', _ = createSignal "a"
        let plain = {| ``as`` = "button"; label = "x" |}

        let withGetter =
            { new IDynamicProps with
                member _.``as`` = "button"
                member _.dyn = sig' () }

        let store, _ = createStore { n = 1 }

        let report =
            {| plainAs = isStatic (plain, "as")
               plainMissing = isStatic (plain, "missing")
               recordField = isStatic (buttonProps (), "label")
               getterDyn = isStatic (withGetter, "dyn")
               storeKey = isStatic (store.Value, "n")
               mergePlainOverStore = isStatic (merge (box store.Value, box {| n = 5 |}), "n")
               mergeStoreOverPlain = isStatic (merge (box {| n = 5 |}, box store.Value), "n")
               omitPlain = isStatic (omit (plain, "label"), "as")
               omitStore = isStatic (omit (store.Value, "missing"), "n") |}

        dispose ()
        report)
