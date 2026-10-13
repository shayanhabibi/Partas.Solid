module Partas.Solid.Tests.Runtime.Primitives.Reactivity.Lists

open System
open Partas.Solid
open Fable.Core

/// Keyed mapArray (identity keys, `(item, index accessor)`): rows are created once per item;
/// indexes are reactive accessors.
type KeyedList =
    {
        setItems: Setter<string array>
        mapped: Accessor<string array>
        mapCalls: ResizeArray<string>
        /// index accessor per row, read now
        indexes: unit -> int array
        disposed: ResizeArray<string>
        dispose: unit -> unit
    }

type private Row = { label: string; index: Accessor<int> }

let makeKeyedList (initial: string array) : KeyedList =
    createRoot (fun (dispose: unit -> unit) ->
        let mapCalls = ResizeArray<string> ()
        let disposed = ResizeArray<string> ()
        let items, setItems = createSignal initial

        let rows =
            mapArray (
                items,
                Func<string, Accessor<int>, Row> (fun item index ->
                    mapCalls.Add item
                    onCleanup (fun () -> disposed.Add item)
                    { label = item; index = index })
            )

        let mapped =
            createMemo (fun _ ->
                rows ()
                |> Array.map (fun r -> r.label.ToUpper ()))

        { setItems = setItems
          mapped = mapped
          mapCalls = mapCalls
          indexes =
            fun () ->
                rows ()
                |> Array.map (fun r -> r.index ())
          disposed = disposed
          dispose = dispose })

/// Unkeyed mapArray (`keyed = false`): rows are per index, the item is an accessor that updates in place.
type UnkeyedList =
    { setItems: Setter<string array>
      mapped: Accessor<string array>
      mapCalls: unit -> int
      dispose: unit -> unit }

let makeUnkeyedList (initial: string array) : UnkeyedList =
    createRoot (fun (dispose: unit -> unit) ->
        let mapCalls = ref 0
        let items, setItems = createSignal initial

        let rows =
            mapArrayUnkeyed (
                items,
                Func<Accessor<string>, int, Accessor<string>> (fun item index ->
                    mapCalls.Value <-
                        mapCalls.Value
                        + 1

                    fun () -> $"{index}:{item ()}")
            )

        let mapped =
            createMemo (fun _ ->
                rows ()
                |> Array.map (fun row -> row ()))

        { setItems = setItems
          mapped = mapped
          mapCalls = fun () -> mapCalls.Value
          dispose = dispose })

/// repeat(count, map): entries are reused when only the count changes.
type Repeated =
    { setCount: Setter<int>
      values: Accessor<string array>
      mapCalls: ResizeArray<int>
      dispose: unit -> unit }

let makeRepeated (initial: int) : Repeated =
    createRoot (fun (dispose: unit -> unit) ->
        let mapCalls = ResizeArray<int> ()
        let count, setCount = createSignal initial

        let values =
            repeat (
                count,
                fun (i: int) ->
                    mapCalls.Add i
                    $"#{i}"
            )

        { setCount = setCount
          values = values
          mapCalls = mapCalls
          dispose = dispose })
