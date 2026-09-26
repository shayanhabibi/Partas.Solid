module Partas.Solid.Tests.Runtime.Primitives.StoresDeep.Tracking

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

// ---------------------------------------------------------------------------------------------
// A deeply nested organisation store. Every write goes through the F# StoreSetter
// (`('T -> 'T) -> unit`): mutate the draft and return it, or return a replacement.
// Memos read one leaf each and bump a per-memo counter, so the spec can assert fine-grained
// tracking (a memo over `a.b.c` only reruns when `a.b.c` changes).
// ---------------------------------------------------------------------------------------------

type Geo = { mutable lat: float; mutable lng: float }

type Address =
    { mutable street: string
      mutable city: string
      mutable geo: Geo }

type Company = { mutable name: string; mutable address: Address }

type Person =
    { id: int
      mutable name: string
      mutable age: int
      mutable tags: ResizeArray<string>
      mutable company: Company }

type Org =
    { mutable title: string
      mutable people: ResizeArray<Person>
      mutable scores: int[]
      mutable version: int }

let mkPerson id name age city =
    { id = id
      name = name
      age = age
      tags = ResizeArray [ "t" + string id ]
      company =
        { name = "Co" + string id
          address =
            { street = "Main " + string id
              city = city
              geo = { lat = float id; lng = 0.0 } } } }

let initialOrg () =
    { title = "Acme"
      people = ResizeArray [ mkPerson 1 "Ada" 36 "London"; mkPerson 2 "Brian" 28 "Paris"; mkPerson 3 "Cleo" 41 "Rome" ]
      scores = [| 10; 20; 30 |]
      version = 0 }

type OrgHarness =
    { state: Store<Org>
      setState: StoreSetter<Org>
      /// Compute counters per memo name.
      runs: obj
      /// Values the observing effects saw, as "name:value".
      log: ResizeArray<string>
      memos: obj
      // --- nested leaf writes ---
      setTitle: string -> unit
      setCity: int -> string -> unit
      setLat: int -> float -> unit
      replaceGeo: int -> float -> float -> unit
      /// `p.company <- { p.company with name = n }`: copy-update of a nested record in the draft.
      copyUpdateCompanyName: int -> string -> unit
      /// Replace the company with a structurally-equal copy.
      replaceCompanySame: int -> unit
      birthday: int -> unit
      // --- array updates on the draft ---
      addPerson: int -> string -> unit
      insertPersonAt: int -> int -> string -> unit
      removePersonById: int -> unit
      removeAllOlderThan: int -> unit
      sortByName: unit -> unit
      sortByAgeDesc: unit -> unit
      reversePeople: unit -> unit
      swapFirstTwo: unit -> unit
      clearPeople: unit -> unit
      addRange: string array -> unit
      /// Updater that returns a NEW filtered list for `people`.
      keepWhere: (Person -> bool) -> unit
      /// `people <- people |> map { p with age = p.age + 1 }` (copy-updated records).
      ageEveryone: unit -> unit
      addTag: int -> string -> unit
      removeTag: int -> string -> unit
      // --- F# array (int[]) field ---
      setScore: int -> int -> unit
      mapScores: (int -> int) -> unit
      sortScoresInPlace: unit -> unit
      /// Several separate setter calls, one flush.
      bulk: unit -> unit
      /// A setter whose updater returns a completely new Org.
      resetTo: Org -> unit
      snap: unit -> Org
      dispose: unit -> unit }

let private bump (runs: obj) (key: string) =
    let current: int = runs?(key)
    runs?(key) <- current + 1

let private findPerson (people: ResizeArray<Person>) (id: int) = people |> Seq.find (fun p -> p.id = id)

let makeOrg () : OrgHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let state, setState = createStore (initialOrg ())
        let log = ResizeArray<string>()

        let runs =
            createObj
                [ "title" ==> 0
                  "city0" ==> 0
                  "lat0" ==> 0
                  "company0" ==> 0
                  "count" ==> 0
                  "names" ==> 0
                  "tags0" ==> 0
                  "score1" ==> 0
                  "totalAge" ==> 0
                  "name1" ==> 0 ]

        let title =
            createMemo (fun (_: string option) ->
                bump runs "title"
                state.Value.title)

        let city0 =
            createMemo (fun (_: string option) ->
                bump runs "city0"
                state.Value.people[0].company.address.city)

        let lat0 =
            createMemo (fun (_: float option) ->
                bump runs "lat0"
                state.Value.people[0].company.address.geo.lat)

        let company0 =
            createMemo (fun (_: string option) ->
                bump runs "company0"
                state.Value.people[0].company.name)

        let count =
            createMemo (fun (_: int option) ->
                bump runs "count"
                state.Value.people.Count)

        let names =
            createMemo (fun (_: string option) ->
                bump runs "names"
                state.Value.people |> Seq.map (fun p -> p.name) |> String.concat ",")

        let tags0 =
            createMemo (fun (_: string option) ->
                bump runs "tags0"
                state.Value.people[0].tags |> String.concat ",")

        let score1 =
            createMemo (fun (_: int option) ->
                bump runs "score1"
                state.Value.scores[1])

        let totalAge =
            createMemo (fun (_: int option) ->
                bump runs "totalAge"
                state.Value.people |> Seq.sumBy (fun p -> p.age))

        /// Person at index 1's name (by position).
        let name1 =
            createMemo (fun (_: string option) ->
                bump runs "name1"
                if state.Value.people.Count > 1 then state.Value.people[1].name else "-")

        createEffect ((fun (_: string option) -> city0 ()), (fun (v: string) -> log.Add $"city0:{v}"))
        createEffect ((fun (_: string option) -> company0 ()), (fun (v: string) -> log.Add $"company0:{v}"))
        createEffect ((fun (_: string option) -> names ()), (fun (v: string) -> log.Add $"names:{v}"))
        createEffect ((fun (_: int option) -> totalAge ()), (fun (v: int) -> log.Add $"totalAge:{v}"))

        let memos =
            createObj
                [ "title" ==> title
                  "city0" ==> city0
                  "lat0" ==> lat0
                  "company0" ==> company0
                  "count" ==> count
                  "names" ==> names
                  "tags0" ==> tags0
                  "score1" ==> score1
                  "totalAge" ==> totalAge
                  "name1" ==> name1 ]

        { state = state
          setState = setState
          runs = runs
          log = log
          memos = memos
          setTitle =
            fun t ->
                setState (fun s ->
                    s.title <- t
                    s)
          setCity =
            fun id city ->
                setState (fun s ->
                    (findPerson s.people id).company.address.city <- city
                    s)
          setLat =
            fun id lat ->
                setState (fun s ->
                    (findPerson s.people id).company.address.geo.lat <- lat
                    s)
          replaceGeo =
            fun id lat lng ->
                setState (fun s ->
                    (findPerson s.people id).company.address.geo <- { lat = lat; lng = lng }
                    s)
          copyUpdateCompanyName =
            fun id name ->
                setState (fun s ->
                    let p = findPerson s.people id
                    p.company <- { p.company with name = name }
                    s)
          replaceCompanySame =
            fun id ->
                setState (fun s ->
                    let p = findPerson s.people id
                    let c = p.company

                    p.company <-
                        { name = c.name
                          address =
                            { street = c.address.street
                              city = c.address.city
                              geo = { lat = c.address.geo.lat; lng = c.address.geo.lng } } }

                    s)
          birthday =
            fun id ->
                setState (fun s ->
                    let p = findPerson s.people id
                    p.age <- p.age + 1
                    s)
          addPerson =
            fun id name ->
                setState (fun s ->
                    s.people.Add(mkPerson id name 20 "Oslo")
                    s)
          insertPersonAt =
            fun index id name ->
                setState (fun s ->
                    s.people.Insert(index, mkPerson id name 20 "Oslo")
                    s)
          removePersonById =
            fun id ->
                setState (fun s ->
                    let i = s.people.FindIndex(fun p -> p.id = id)
                    s.people.RemoveAt i
                    s)
          removeAllOlderThan =
            fun age ->
                setState (fun s ->
                    s.people.RemoveAll(fun p -> p.age > age) |> ignore
                    s)
          sortByName =
            fun () ->
                setState (fun s ->
                    s.people.Sort(fun a b -> compare a.name b.name)
                    s)
          sortByAgeDesc =
            fun () ->
                setState (fun s ->
                    s.people <- ResizeArray(s.people |> Seq.sortByDescending (fun p -> p.age))
                    s)
          reversePeople =
            fun () ->
                setState (fun s ->
                    s.people.Reverse()
                    s)
          swapFirstTwo =
            fun () ->
                setState (fun s ->
                    let a = s.people[0]
                    s.people[0] <- s.people[1]
                    s.people[1] <- a
                    s)
          clearPeople =
            fun () ->
                setState (fun s ->
                    s.people.Clear()
                    s)
          addRange =
            fun names ->
                setState (fun s ->
                    let start = s.people.Count + 100
                    s.people.AddRange(names |> Array.mapi (fun i n -> mkPerson (start + i) n 30 "Berlin"))
                    s)
          keepWhere =
            fun pred ->
                setState (fun s ->
                    s.people <- ResizeArray(s.people |> Seq.filter pred)
                    s)
          ageEveryone =
            fun () ->
                setState (fun s ->
                    s.people <- ResizeArray(s.people |> Seq.map (fun p -> { p with age = p.age + 1 }))
                    s)
          addTag =
            fun id tag ->
                setState (fun s ->
                    (findPerson s.people id).tags.Add tag
                    s)
          removeTag =
            fun id tag ->
                setState (fun s ->
                    (findPerson s.people id).tags.Remove tag |> ignore
                    s)
          setScore =
            fun i v ->
                setState (fun s ->
                    s.scores[i] <- v
                    s)
          mapScores =
            fun f ->
                setState (fun s ->
                    s.scores <- s.scores |> Array.map f
                    s)
          sortScoresInPlace =
            fun () ->
                setState (fun s ->
                    Array.sortInPlaceWith (fun a b -> compare b a) s.scores
                    s)
          bulk =
            fun () ->
                setState (fun s ->
                    s.version <- s.version + 1
                    s)

                setState (fun s ->
                    s.title <- s.title + "!"
                    s)

                setState (fun s ->
                    s.version <- s.version + 1
                    s)
          resetTo = fun org -> setState (fun _ -> org)
          snap = fun () -> snapshot state
          dispose = dispose })

// ---------------------------------------------------------------------------------------------
// F# structural equality over proxies and snapshots.
// ---------------------------------------------------------------------------------------------

type EqualityReport =
    { proxyCompanyEqualsLiteral: bool
      proxyGeoEqualsLiteral: bool
      proxyGeoNotEqualOther: bool
      snapshotEqualsInitial: bool
      snapshotPersonEqualsLiteral: bool
      snapshotAfterWriteDiffers: bool
      proxyEqualsSnapshot: bool
      hashesMatch: bool
      compareSnapshots: int }

/// Unowned on purpose: the store write below must not happen inside an owned scope.
let equalityReport () =
        let state, setState = createStore (initialOrg ())
        let first = mkPerson 1 "Ada" 36 "London"
        let before = snapshot state

        let r1 =
            {| proxyCompanyEqualsLiteral = (state.Value.people[0].company = first.company)
               proxyGeoEqualsLiteral = (state.Value.people[0].company.address.geo = { lat = 1.0; lng = 0.0 })
               proxyGeoNotEqualOther = (state.Value.people[0].company.address.geo <> { lat = 2.0; lng = 0.0 })
               snapshotEqualsInitial = (before = initialOrg ())
               snapshotPersonEqualsLiteral = (before.people[0] = first)
               proxyEqualsSnapshot = (state.Value.people[0].company.address.geo = before.people[0].company.address.geo)
               hashesMatch = (hash before.people[0].company.address.geo = hash { lat = 1.0; lng = 0.0 }) |}

        setState (fun s ->
            s.people[0].age <- 99
            s)

        flush ()
        let after = snapshot state
        let afterAge = after.people[0].age
        let beforeAge = before.people[0].age

        let report =
            { proxyCompanyEqualsLiteral = r1.proxyCompanyEqualsLiteral
              proxyGeoEqualsLiteral = r1.proxyGeoEqualsLiteral
              proxyGeoNotEqualOther = r1.proxyGeoNotEqualOther
              snapshotEqualsInitial = r1.snapshotEqualsInitial
              snapshotPersonEqualsLiteral = r1.snapshotPersonEqualsLiteral
              snapshotAfterWriteDiffers = (after <> before)
              proxyEqualsSnapshot = r1.proxyEqualsSnapshot
              hashesMatch = r1.hashesMatch
              compareSnapshots = compare afterAge beforeAge }

        report

// ---------------------------------------------------------------------------------------------
// reconcile over arrays of records, at the root and on a nested path.
// ---------------------------------------------------------------------------------------------

type Line = { id: string; mutable qty: int; mutable sku: string }

type Cart =
    { mutable owner: string
      mutable lines: ResizeArray<Line> }

type CartHarness =
    { cart: Store<Cart>
      runs: obj
      qtyOf: string -> int
      /// reconcile the whole cart at the root (default key "id").
      reconcileRoot: Cart -> unit
      /// reconcile only `lines`, from inside an updater, by applying `reconcile next` to the nested draft.
      reconcileLines: ResizeArray<Line> -> unit
      /// reconcile keyed by `sku` (named key).
      reconcileBySku: ResizeArray<Line> -> unit
      dispose: unit -> unit }

let line id qty sku = { id = id; qty = qty; sku = sku }
let lines (xs: Line array) = ResizeArray xs

let cart owner (xs: Line array) = { owner = owner; lines = ResizeArray xs }

let makeCart () : CartHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let state, setState =
            createStore (cart "ann" [| line "a" 1 "A-1"; line "b" 2 "B-1"; line "c" 3 "C-1" |])

        let runs = createObj [ "a" ==> 0; "b" ==> 0; "c" ==> 0; "owner" ==> 0; "count" ==> 0 ]

        let qtyMemo (id: string) =
            createMemo (fun (_: int option) ->
                bump runs id

                match state.Value.lines |> Seq.tryFind (fun l -> l.id = id) with
                | Some l -> l.qty
                | None -> -1)

        let a = qtyMemo "a"
        let b = qtyMemo "b"
        let c = qtyMemo "c"

        let owner =
            createMemo (fun (_: string option) ->
                bump runs "owner"
                state.Value.owner)

        let count =
            createMemo (fun (_: int option) ->
                bump runs "count"
                state.Value.lines.Count)

        createEffect ((fun (_: int option) -> a () + b () + c ()), ignore)
        createEffect ((fun (_: string option) -> owner ()), ignore)
        createEffect ((fun (_: int option) -> count ()), ignore)

        { cart = state
          runs = runs
          qtyOf =
            fun id ->
                match id with
                | "a" -> a ()
                | "b" -> b ()
                | _ -> c ()
          reconcileRoot = fun next -> setState (reconcile next)
          reconcileLines =
            fun next ->
                setState (fun s ->
                    reconcile next s.lines |> ignore
                    s)
          reconcileBySku = fun next -> setState (fun s ->
                    reconcile (next, "sku") s.lines |> ignore
                    s)
          dispose = dispose })
