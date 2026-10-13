module Partas.Solid.Tests.Runtime.Primitives.StoresDeep.Shapes

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

let private bump (runs: obj) (key: string) =
    let current: int = runs?(key)
    runs?(key) <- current + 1

[<Emit("delete $0[$1]")>]
let private jsDelete (o: obj) (key: string) : unit = jsNative

// ---------------------------------------------------------------------------------------------
// Map-like stores: a string-keyed pojo dictionary (dynamic keys), and an F# Map field.
// ---------------------------------------------------------------------------------------------

type Entry = { mutable label: string; mutable hits: int }

type Registry =
    { mutable byId: obj
      mutable lookup: Map<string, int> }

type RegistryHarness =
    { registry: Store<Registry>
      runs: obj
      /// memo: byId["x"].label, or "missing".
      labelOfX: Accessor<string>
      /// memo: number of keys in byId.
      keyCount: Accessor<int>
      /// memo: Map.tryFind "k" lookup.
      lookupK: Accessor<int>
      put: string -> string -> unit
      hit: string -> unit
      remove: string -> unit
      mapAdd: string -> int -> unit
      mapRemove: string -> unit
      dispose: unit -> unit }

let makeRegistry () : RegistryHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let registry, setRegistry =
            createStore
                { byId = createObj [ "a" ==> { label = "A"; hits = 0 } ]
                  lookup = Map.ofList [ "j", 1 ] }

        let runs = createObj [ "labelOfX" ==> 0; "keyCount" ==> 0; "lookupK" ==> 0 ]

        let labelOfX =
            createMemo (fun (_: string option) ->
                bump runs "labelOfX"
                let e: Entry option = registry.Value.byId?x
                match e with
                | Some e -> e.label
                | None -> "missing")

        let keyCount =
            createMemo (fun (_: int option) ->
                bump runs "keyCount"
                (JS.Constructors.Object.keys registry.Value.byId).Count)

        let lookupK =
            createMemo (fun (_: int option) ->
                bump runs "lookupK"
                registry.Value.lookup |> Map.tryFind "k" |> Option.defaultValue -1)

        createEffect ((fun (_: string option) -> labelOfX ()), ignore)
        createEffect ((fun (_: int option) -> keyCount ()), ignore)
        createEffect ((fun (_: int option) -> lookupK ()), ignore)

        { registry = registry
          runs = runs
          labelOfX = labelOfX
          keyCount = keyCount
          lookupK = lookupK
          put =
            fun key label ->
                setRegistry (fun r ->
                    r.byId?(key) <- { label = label; hits = 0 }
                    r)
          hit =
            fun key ->
                setRegistry (fun r ->
                    let e: Entry = r.byId?(key)
                    e.hits <- e.hits + 1
                    r)
          remove =
            fun key ->
                setRegistry (fun r ->
                    jsDelete r.byId key
                    r)
          mapAdd =
            fun key v ->
                setRegistry (fun r ->
                    r.lookup <- r.lookup.Add(key, v)
                    r)
          mapRemove =
            fun key ->
                setRegistry (fun r ->
                    r.lookup <- r.lookup.Remove key
                    r)
          dispose = dispose })

// ---------------------------------------------------------------------------------------------
// Unions and options as store values.
// ---------------------------------------------------------------------------------------------

type Status =
    | Idle
    | Busy of progress: int
    | Failed of reason: string

let busy (p: int) = Busy p
let failed (r: string) = Failed r
let idle () = Idle

type Profile = { mutable nick: string; mutable avatar: string option }

type Session =
    { mutable status: Status
      mutable selected: int option
      mutable profile: Profile option }

type SessionHarness =
    { session: Store<Session>
      describe: Accessor<string>
      selectedText: Accessor<string>
      nick: Accessor<string>
      log: ResizeArray<string>
      setStatus: Status -> unit
      select: int option -> unit
      login: string -> unit
      logout: unit -> unit
      setAvatar: string option -> unit
      isBusy: unit -> bool
      dispose: unit -> unit }

let makeSession () : SessionHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let session, setSession =
            createStore
                { status = Idle
                  selected = None
                  profile = None }

        let log = ResizeArray<string>()

        let describe =
            createMemo (fun (_: string option) ->
                match session.Value.status with
                | Idle -> "idle"
                | Busy p -> $"busy {p}%%"
                | Failed r -> "failed: " + r)

        let selectedText =
            createMemo (fun (_: string option) ->
                match session.Value.selected with
                | Some i -> $"#{i}"
                | None -> "none")

        let nick =
            createMemo (fun (_: string option) ->
                match session.Value.profile with
                | Some p ->
                    match p.avatar with
                    | Some a -> p.nick + "@" + a
                    | None -> p.nick
                | None -> "anon")

        createEffect ((fun (_: string option) -> describe ()), (fun (v: string) -> log.Add v))
        createEffect ((fun (_: string option) -> nick ()), (fun (v: string) -> log.Add v))

        { session = session
          describe = describe
          selectedText = selectedText
          nick = nick
          log = log
          setStatus =
            fun st ->
                setSession (fun s ->
                    s.status <- st
                    s)
          select =
            fun i ->
                setSession (fun s ->
                    s.selected <- i
                    s)
          login =
            fun n ->
                setSession (fun s ->
                    s.profile <- Some { nick = n; avatar = None }
                    s)
          logout =
            fun () ->
                setSession (fun s ->
                    s.profile <- None
                    s)
          setAvatar =
            fun a ->
                setSession (fun s ->
                    match s.profile with
                    | Some p -> p.avatar <- a
                    | None -> ()

                    s)
          isBusy =
            fun () ->
                match session.Value.status with
                | Busy _ -> true
                | _ -> false
          dispose = dispose })

// ---------------------------------------------------------------------------------------------
// Stores holding signals (accessor functions as values).
// ---------------------------------------------------------------------------------------------

type Widget =
    { name: string
      mutable value: Accessor<int> }

type Board = { mutable widgets: ResizeArray<Widget> }

type BoardHarness =
    { total: Accessor<int>
      runs: unit -> int
      setA: int -> unit
      setB: int -> unit
      /// Swap widget 0's accessor for one reading signal C.
      rewire: unit -> unit
      setC: int -> unit
      firstIsFunction: unit -> bool
      dispose: unit -> unit }

let makeBoard () : BoardHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let a, setA = createSignal 1
        let b, setB = createSignal 10
        let c, setC = createSignal 100

        let board, setBoard =
            createStore
                { widgets =
                    ResizeArray
                        [ { name = "a"; value = a }
                          { name = "b"; value = b } ] }

        let mutable runs = 0

        let total =
            createMemo (fun (_: int option) ->
                runs <- runs + 1
                board.Value.widgets |> Seq.sumBy (fun w -> w.value ()))

        createEffect ((fun (_: int option) -> total ()), ignore)

        { total = total
          runs = fun () -> runs
          setA = setA
          setB = setB
          rewire =
            fun () ->
                setBoard (fun d ->
                    d.widgets[0].value <- c
                    d)
          setC = setC
          firstIsFunction = fun () -> jsTypeof board.Value.widgets[0].value = "function"
          dispose = dispose })
