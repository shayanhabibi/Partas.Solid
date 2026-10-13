module Partas.Solid.Tests.Runtime.Primitives.Reactivity.Memos

open Partas.Solid
open Fable.Core

/// a -> doubled -> plusOne, with a run log per memo body.
type Chain =
    { setA: Setter<int>
      doubled: Accessor<int>
      plusOne: Accessor<int>
      log: ResizeArray<string>
      dispose: unit -> unit }

let makeChain () : Chain =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let a, setA = createSignal 1

        let doubled =
            createMemo (fun _ ->
                log.Add "doubled"
                a () * 2)

        let plusOne =
            createMemo (fun _ ->
                log.Add "plusOne"

                doubled ()
                + 1)

        { setA = setA
          doubled = doubled
          plusOne = plusOne
          log = log
          dispose = dispose })

/// Eager vs `lazy = true` memo. `runs` counts compute executions.
type LazyProbe =
    { setSource: Setter<int>
      eager: Accessor<int>
      lazyMemo: Accessor<int>
      eagerRuns: unit -> int
      lazyRuns: unit -> int
      dispose: unit -> unit }

let makeLazyProbe () : LazyProbe =
    createRoot (fun (dispose: unit -> unit) ->
        let eagerRuns = ref 0
        let lazyRuns = ref 0
        let source, setSource = createSignal 3

        let eager =
            createMemo (fun _ ->
                eagerRuns.Value <-
                    eagerRuns.Value
                    + 1

                source ()
                + 100)

        let lazyMemo =
            createMemo (
                (fun _ ->
                    lazyRuns.Value <-
                        lazyRuns.Value
                        + 1

                    source ()
                    + 200),
                ``lazy`` = true
            )

        { setSource = setSource
          eager = eager
          lazyMemo = lazyMemo
          eagerRuns = fun () -> eagerRuns.Value
          lazyRuns = fun () -> lazyRuns.Value
          dispose = dispose })

/// A memo whose value often stays the same (parity). Downstream work must not re-run when
/// the memo recomputes to an equal value.
type Dedupe =
    { setN: Setter<int>
      parity: Accessor<string>
      parityRuns: unit -> int
      downstreamRuns: unit -> int
      effectSeen: ResizeArray<string>
      dispose: unit -> unit }

let makeDedupe () : Dedupe =
    createRoot (fun (dispose: unit -> unit) ->
        let parityRuns = ref 0
        let downstreamRuns = ref 0
        let effectSeen = ResizeArray<string> ()
        let n, setN = createSignal 1

        let parity =
            createMemo (fun _ ->
                parityRuns.Value <-
                    parityRuns.Value
                    + 1

                if n () % 2 = 0 then "even" else "odd")

        let upper =
            createMemo (fun _ ->
                downstreamRuns.Value <-
                    downstreamRuns.Value
                    + 1

                parity().ToUpper ())

        createEffect ((fun _ -> upper ()), (fun (v: string) -> effectSeen.Add v))

        { setN = setN
          parity = parity
          parityRuns = fun () -> parityRuns.Value
          downstreamRuns = fun () -> downstreamRuns.Value
          effectSeen = effectSeen
          dispose = dispose })

/// Memo with a custom `equals` (MemoOptions POJO): values within 5 of the previous value are treated as equal,
/// so a downstream effect only sees "significant" changes.
type Coarse =
    { setRaw: Setter<int>
      coarse: Accessor<int>
      effectSeen: ResizeArray<int>
      dispose: unit -> unit }

let makeCoarse () : Coarse =
    createRoot (fun (dispose: unit -> unit) ->
        let effectSeen = ResizeArray<int> ()
        let raw, setRaw = createSignal 0

        let coarse =
            createMemo (
                (fun _ -> raw ()),
                MemoOptions<int> (
                    equals =
                        EqualityFunc (fun prev next ->
                            abs (
                                next
                                - prev
                            ) < 5)
                )
            )

        createEffect ((fun _ -> coarse ()), (fun (v: int) -> effectSeen.Add v))

        { setRaw = setRaw
          coarse = coarse
          effectSeen = effectSeen
          dispose = dispose })

/// Diamond: a -> (left, right) -> sum. `sum` must compute once per change of `a`, never with a torn view.
type Diamond =
    { setA: Setter<int>
      sum: Accessor<int>
      log: ResizeArray<string>
      sumSeen: ResizeArray<string>
      dispose: unit -> unit }

let makeDiamond () : Diamond =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let sumSeen = ResizeArray<string> ()
        let a, setA = createSignal 1

        let left =
            createMemo (fun _ ->
                log.Add "left"
                a () + 1)

        let right =
            createMemo (fun _ ->
                log.Add "right"
                a () * 10)

        let sum =
            createMemo (fun _ ->
                log.Add "sum"
                let l = left ()
                let r = right ()
                sumSeen.Add $"{l}+{r}"
                l + r)

        createEffect ((fun _ -> sum ()), (fun (_: int) -> log.Add "effect"))

        { setA = setA
          sum = sum
          log = log
          sumSeen = sumSeen
          dispose = dispose })

/// Conditional dependency switching: the memo reads `useLeft` then *either* left or right.
type Switching =
    { setUseLeft: Setter<bool>
      setLeft: Setter<string>
      setRight: Setter<string>
      picked: Accessor<string>
      runs: unit -> int
      dispose: unit -> unit }

let makeSwitching () : Switching =
    createRoot (fun (dispose: unit -> unit) ->
        let runs = ref 0
        let useLeft, setUseLeft = createSignal true
        let left, setLeft = createSignal "L1"
        let right, setRight = createSignal "R1"

        let picked =
            createMemo (fun _ ->
                runs.Value <-
                    runs.Value
                    + 1

                if useLeft () then left () else right ())

        // keep the memo observed so it recomputes on flush rather than on read
        createEffect ((fun _ -> picked ()), (fun (_: string) -> ()))

        { setUseLeft = setUseLeft
          setLeft = setLeft
          setRight = setRight
          picked = picked
          runs = fun () -> runs.Value
          dispose = dispose })

/// The compute function receives its previous value (None on the first run): a running total.
type Accumulator =
    { add: Setter<int>
      total: Accessor<int>
      prevSeen: ResizeArray<string>
      dispose: unit -> unit }

let makeAccumulator () : Accumulator =
    createRoot (fun (dispose: unit -> unit) ->
        let prevSeen = ResizeArray<string> ()
        let step, setStep = createSignal 5

        let total =
            createMemo (fun (prev: int option) ->
                match prev with
                | None -> prevSeen.Add "none"
                | Some p -> prevSeen.Add (string p)

                defaultArg prev 0
                + step ())

        { add = setStep
          total = total
          prevSeen = prevSeen
          dispose = dispose })

/// Same as `makeCoarse`, with the compute's `prev` parameter annotated so overload resolution is unambiguous.
let makeCoarseAnnotated () : Coarse =
    createRoot (fun (dispose: unit -> unit) ->
        let effectSeen = ResizeArray<int> ()
        let raw, setRaw = createSignal 0

        let coarse =
            createMemo (
                (fun (_: int option) -> raw ()),
                MemoOptions<int> (
                    equals =
                        EqualityFunc (fun prev next ->
                            abs (
                                next
                                - prev
                            ) < 5)
                )
            )

        createEffect ((fun _ -> coarse ()), (fun (v: int) -> effectSeen.Add v))

        { setRaw = setRaw
          coarse = coarse
          effectSeen = effectSeen
          dispose = dispose })

/// Custom memo `equals` through the named optional argument (ParamObject overload).
let makeCoarseNamed () : Coarse =
    createRoot (fun (dispose: unit -> unit) ->
        let effectSeen = ResizeArray<int> ()
        let raw, setRaw = createSignal 0

        let coarse =
            createMemo (
                (fun _ -> raw ()),
                equals =
                    EqualityFunc (fun prev next ->
                        abs (
                            next
                            - prev
                        ) < 5)
            )

        createEffect ((fun _ -> coarse ()), (fun (v: int) -> effectSeen.Add v))

        { setRaw = setRaw
          coarse = coarse
          effectSeen = effectSeen
          dispose = dispose })
