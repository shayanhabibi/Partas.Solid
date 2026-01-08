namespace Partas.Solid.TanStack.Store

open Fable.Core
open Fable.Core.JS
open Partas.Solid
open System

[<AutoOpen>]
module Bindings =

    /// Store options - use anonymous record with !! operator:
    /// new Store(0, !!{| updateFn = fun prev updater -> ... |})
    [<Interface>]
    type StoreOptions<'T> =
        abstract member updateFn: ('T -> ('T -> 'T) -> 'T) option with get
        abstract member onUpdate: (unit -> unit) option with get

    [<Import("Store", Spec.PackageName)>]
    type Store<'T>(initialValue: 'T, ?options: StoreOptions<'T>) =
        member _.state: 'T = jsNative
        member _.setState(updater: 'T -> 'T) : unit = jsNative
        member _.setState(newState: 'T) : unit = jsNative
        member _.subscribe(callback: unit -> unit) : (unit -> unit) = jsNative

    [<ImportMember(Spec.PackageName)>]
    let batch (fn: unit -> unit) : unit = jsNative

    [<Interface>]
    type DerivedFnProps<'T> =
        abstract member prevVal: 'T option with get
        abstract member prevDepVals: obj[] option with get
        abstract member currDepVals: obj[] with get

    [<Pojo>]
    type DerivedOptions<'T>(fn: DerivedFnProps<'T> -> 'T, deps: obj[]) =
        member val fn = fn with get, set
        member val deps = deps with get, set
        member val onSubscribe: (Action<'T> * Derived<'T> -> (unit -> unit)) option = None with get, set
        member val onUpdate: (unit -> unit) option = None with get, set

    and [<Import("Derived", Spec.PackageName)>] Derived<'T>(options: DerivedOptions<'T>) =
        member _.state: 'T = jsNative
        member _.mount() : (unit -> unit) = jsNative

    [<Pojo>]
    type EffectOptions(fn: unit -> unit, deps: obj[]) =
        member val fn = fn with get, set
        member val deps = deps with get, set
        member val eager = false with get, set

    [<Import("Effect", Spec.PackageName)>]
    type Effect(options: EffectOptions) =
        member _.mount() : (unit -> unit) = jsNative

    [<ImportMember(Spec.PackageName)>]
    let useStore (store: Store<'T>) (selector: 'T -> 'U) : Accessor<'U> = jsNative

    [<ImportMember(Spec.PackageName)>]
    let useStoreFull (store: Store<'T>) : Accessor<'T> = jsNative
