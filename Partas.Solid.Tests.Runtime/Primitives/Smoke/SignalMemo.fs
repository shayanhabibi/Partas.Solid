module Partas.Solid.Tests.Runtime.Primitives.Smoke.SignalMemo

open Partas.Solid
open Fable.Core

/// A signal, a memo derived from it, and a counter of how often the memo body ran.
/// Everything is created inside a reactive root so memo ownership is well defined.
type Counter =
    { count: Accessor<int>
      setCount: Setter<int>
      doubled: Accessor<int>
      memoRuns: unit -> int
      dispose: unit -> unit }

let makeCounter (initial: int) : Counter =
    createRoot (fun (dispose: unit -> unit) ->
        let mutable runs = 0
        let count, setCount = createSignal initial

        let doubled =
            createMemo (fun _ ->
                runs <- runs + 1
                count () * 2)

        { count = count
          setCount = setCount
          doubled = doubled
          memoRuns = fun () -> runs
          dispose = dispose })
