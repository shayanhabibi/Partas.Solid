module Partas.Solid.Tests.IssueCases.ThisArgTransformsTypes

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop
open Fable.Core.JS
open System.Runtime.CompilerServices

type private ChangeStack<'T> =
    { data: SolidResource<'T[]>
      resource: SolidResourceManager<'T[]>
      source: Signal<'T[] option> }

type private AddStack<'T> =
    { data: SolidResource<Result<unit, string>[]>
      resource: SolidResourceManager<Result<unit, string>[]>
      source: Signal<'T[] option> }

type private EditStack<'T> =
    { data: SolidResource<unit>
      resource: SolidResourceManager<unit>
      source: Signal<'T[] option> }

type DataStack<'T> =
    private
        { changeStack: ChangeStack<'T>
          editStack: EditStack<'T>
          addStack: AddStack<'T> }

    member this.data = this.changeStack.data.latest

[<JS.Pojo>]
type TableOptions<'Data>(?data: 'Data[]) =
    /// <summary>
    /// The data for the table to display. This array should match the type you provided to <c>table.setRowType[...]</c>, but in theory could be an array of anything. It's common for each item in the array to be an object of key/values but this is not required. Columns can access this data via string/index or a functional accessor to return anything they want.<br/><br/>
    /// When the data option changes reference (compared via Object.is), the table will reprocess the data. Any other data processing that relies on the core data model (such as grouping, sorting, filtering, etc) will also be reprocessed.<br/><br/>
    /// 🧠 Make sure your <c>data</c> option is only changing when you want the table to reprocess. Providing an inline [] or constructing the data array as a new object every time you want to render the table will result in a lot of unnecessary re-processing. This can easily go unnoticed in smaller tables, but you will likely notice it in larger tables.
    /// </summary>
    /// <remarks>For reactivity, you should use the helper function <c>TableOptions.dataGetter</c> to set a `get` property</remarks>
    member val data: 'Data[] option = data with get, set


let inline private mkProperty this name getter =
    Constructors.Object.defineProperty (
        this,
        name,
        unbox<PropertyDescriptor> (
            createObj
                [ "get"
                  ==> getter ]
        )
    )
    |> ignore

    this

[<AutoOpen; Erase>]
type TableOptionsExtensions =
    /// Properly provides reactivity by making the data key a property
    [<System.Runtime.CompilerServices.Extension; Erase>]
    static member inline data(this: TableOptions<'Data>, value: unit -> 'Data[]) =
        mkProperty this "data" value
