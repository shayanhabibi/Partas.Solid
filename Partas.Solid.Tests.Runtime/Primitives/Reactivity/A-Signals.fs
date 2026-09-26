module Partas.Solid.Tests.Runtime.Primitives.Reactivity.Signals

open Partas.Solid
open Fable.Core

/// A signal observed by a user effect. `seen` records every value the effect phase received.
type Observed<'T> =
    { value: Accessor<'T>
      set: Setter<'T>
      seen: ResizeArray<'T>
      dispose: unit -> unit }

let private observe (value: Accessor<'T>) (set: Setter<'T>) (dispose: unit -> unit) =
    let seen = ResizeArray<'T> ()
    createEffect ((fun _ -> value ()), (fun (v: 'T) -> seen.Add v))

    { value = value
      set = set
      seen = seen
      dispose = dispose }

/// Plain `createSignal value` with the default (reference) equality.
let makeIntSignal (initial: int) : Observed<int> =
    createRoot (fun (dispose: unit -> unit) ->
        let value, setValue = createSignal initial
        observe value setValue dispose)

/// Updater-function writes through the `Setter.Invoke` / `InvokeAndGet` extensions.
type Counter =
    { count: Accessor<int>
      increment: unit -> unit
      addAndGet: int -> int
      setAndGet: int -> int
      dispose: unit -> unit }

let makeCounter (initial: int) : Counter =
    createRoot (fun (dispose: unit -> unit) ->
        let count, setCount = createSignal initial

        { count = count
          increment = fun () -> setCount.Invoke (fun c -> c + 1)
          addAndGet = fun n -> setCount.InvokeAndGet (fun c -> c + n)
          setAndGet = fun n -> setCount.InvokeAndGet n
          dispose = dispose })

/// `createSignal()` with no initial value is `Signal<'T option>`: undefined until written.
type OptionalSignal =
    { current: Accessor<string option>
      isSet: unit -> bool
      set: string -> unit
      clear: unit -> unit
      dispose: unit -> unit }

let makeOptionalSignal () : OptionalSignal =
    createRoot (fun (dispose: unit -> unit) ->
        let current, setCurrent = createSignal<string> ()

        { current = current
          isSet = fun () -> current().IsSome
          set = fun s -> setCurrent (Some s)
          clear = fun () -> setCurrent None
          dispose = dispose })

/// Custom comparator passed as a named optional argument (ParamObject overload):
/// values are "equal" when they share the same last digit.
let makeLastDigitSignal (initial: int) : Observed<int> =
    createRoot (fun (dispose: unit -> unit) ->
        let value, setValue =
            createSignal (initial, equals = EqualityFunc (fun prev next -> prev % 10 = next % 10))

        observe value setValue dispose)

/// Comparator that never reports equality: every write notifies, even the same value.
let makeAlwaysNotifySignal (initial: int) : Observed<int> =
    createRoot (fun (dispose: unit -> unit) ->
        let value, setValue =
            createSignal (initial, SignalOptions<int> (equals = EqualityFunc (fun _ _ -> false)))

        observe value setValue dispose)

type Point = { x: int; y: int }

/// Records compared structurally through a custom comparator (default equality is by reference).
let makePointSignal (structural: bool) : Observed<Point> =
    createRoot (fun (dispose: unit -> unit) ->
        let value, setValue =
            if structural then
                createSignal ({ x = 0; y = 0 }, equals = EqualityFunc (fun (a: Point) (b: Point) -> a = b))
            else
                createSignal { x = 0; y = 0 }

        observe value setValue dispose)

/// Writable derived signal: `createSignal (fun () -> ...)` tracks its source, but can be overwritten locally
/// until the source changes again.
type WritableDerived =
    { source: Accessor<int>
      setSource: Setter<int>
      derived: Accessor<int>
      setDerived: Setter<int>
      dispose: unit -> unit }

let makeWritableDerived () : WritableDerived =
    createRoot (fun (dispose: unit -> unit) ->
        let source, setSource = createSignal 1

        let derived, setDerived =
            createSignal<int> (fun () ->
                source ()
                * 10)

        { source = source
          setSource = setSource
          derived = derived
          setDerived = setDerived
          dispose = dispose })
