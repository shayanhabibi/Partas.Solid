module Partas.Solid.Tests.Runtime.Primitives.StoresAsync.CoreHelpers

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

// ---------------------------------------------------------------------------------------------
// isWrappable, isEqual
// ---------------------------------------------------------------------------------------------

/// isWrappable over a plain object, an array, a numeric array, primitives, null, a class instance and a frozen
/// object. The suites compile with `--typedArrays false`, so `int[]` is a plain array a store wraps; with Fable's
/// default it would be an `Int32Array`, which a store leaves unwrapped.
let wrappable () =
    [| isWrappable (createObj [ "a" ==> 1 ])
       isWrappable [| "a"; "b" |]
       isWrappable [| 1; 2 |]
       isWrappable 1
       isWrappable "s"
       isWrappable null
       isWrappable (JS.Constructors.Date.Create())
       isWrappable (JS.Constructors.Object.freeze (createObj [ "a" ==> 1 ])) |]

/// isEqual is strict equality: equal primitives, different primitives, one object twice, two equal-looking objects.
let equality () =
    let o = createObj []
    [| isEqual (1, 1); isEqual ("a", "b"); isEqual (o, o); isEqual (createObj [], createObj []) |]

// ---------------------------------------------------------------------------------------------
// NotReadyError
// ---------------------------------------------------------------------------------------------

type NotReadyHarness =
    { /// Reads the pending memo untracked and classifies what it throws with an F# type test.
      readUntracked: unit -> string
      /// A memo that reads the pending memo and catches the throw itself.
      readInMemo: Accessor<string>
      /// Whether the caught error carries the pending source.
      hasSource: unit -> bool
      dispose: unit -> unit }

let makeNotReady () : NotReadyHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let pending = createMemo (fun (_: string option) -> emitJsExpr () "new Promise(() => {})" : JS.Promise<string>)

        let classify () =
            try
                pending () |> ignore
                "ready"
            with
            | :? NotReadyError -> "not-ready"
            | e -> "other: " + e.Message

        { readUntracked = classify
          readInMemo = createMemo (fun (_: string option) -> classify ())
          hasSource =
            fun () ->
                try
                    pending () |> ignore
                    false
                with :? NotReadyError as e ->
                    not (isNull e.source)
          dispose = dispose })
