module Partas.Solid.examples.checkbox

open Partas.Solid.Lucide
open Partas.Solid
open Partas.Solid.Aria
open Partas.Solid.TanStack.Table
open Partas.Solid.Kobalte
open Fable.Core
open Fable.Core.JsInterop

type [<Erase>] Lib =
    [<Import("twMerge", "tailwind-merge")>]
    static member twMerge (classes: string) : string = jsNative
    [<Import("clsx", "clsx")>]
    static member clsx(classes: obj): string = jsNative
    static member cn (classes: string array): string = classes |> Lib.clsx |> Lib.twMerge

[<Erase>]
type Checkbox() =
    inherit Kobalte.Checkbox()
    [<SolidTypeComponent>]
    member props.checkbox =
        Kobalte.Checkbox(
            indeterminate = props.indeterminate,
            class' = Lib.cn [| "items-top group relative flex space-x-2"; props.class' |]
            ).spread(props) { yield fun _ -> Fragment() {
            
            Checkbox.Input(class'="peer")
            Checkbox.Control(
                class' = "size-4 shrink-0 rounded-sm border border-primary
                ring-offset-background disabled:cursor-not-allowed disabled:opacity-50
                peer-focus-visible:outline-none peer-focus-visible:ring-2 peer-focus-visible:ring-ring
                peer-focus-visible:ring-offset-2 data-[checked]:border-none
                data-[indeterminate]:border-none data-[checked]:bg-primary
                data-[indeterminate]:bg-primary data-[checked]:text-primary-foreground
                data-[indeterminate]:text-primary-foreground"
                ) {
                Checkbox.Indicator() {
                    if props.indeterminate then
                        Minus(class' = "size-4", strokeWidth = 2)
                    else
                        Check(class' = "size-4", strokeWidth = 2)
                }
            }
        }
        }

[<Erase>]
type TableCaption() =
    inherit caption()
    [<SolidTypeComponent>]
    member props.__ =
        caption(class' = Lib.cn [|
                "mt-4 text-sm text-muted-foreground"
                props.class'
            |]).spread props
[<Erase>]
type Table() =
    inherit table()
    [<SolidTypeComponent>]
    member props.__ =
        div(class' = "relative w-full overflow-auto") {
            table(class' = Lib.cn [|
                    "w-full caption-bottom text-sm"
                    props.class'
                |]).spread props
        }
[<Erase>]
type TableHeader() =
    inherit thead()
    [<SolidTypeComponent>]
    member props.constructor =
        thead(class' = Lib.cn [| "[&_tr]:border-b"
                                 props.class' |])
            .spread props

[<Erase>]
type TableBody() =
    inherit tbody()
    [<SolidTypeComponent>]
    member props.constructor =
        tbody(class' = Lib.cn [|
            "[&_tr:last-child]:border-0"
            props.class'
        |]).spread props

[<Erase>]
type TableFooter() =
    inherit tfoot()
    [<SolidTypeComponent>]
    member props.constructor =
        tfoot(class' = Lib.cn [|
            "bg-primary font-medium text-primary-foreground"
            props.class'
        |]).spread props

[<Erase>]
type TableRow() =
    inherit tr()
    [<SolidTypeComponent>]
    member props.constructor =
        tr(
            class' = Lib.cn [|
                "border-b transition-colors hover:bg-muted/50 data-[state=selected]:bg-muted"
                props.class'
            |]
        ).spread props

[<Erase>]
type TableHead() =
    inherit th()
    [<SolidTypeComponent>]
    member props.constructor =
        th(
            class' = Lib.cn [|
                "h-10 px-2 text-left align-middle font-medium
                text-muted-foreground [&:has([role=checkbox])]:pr-0"
                props.class'
            |]
        ).spread props

[<Erase>]
type TableCell() =
    inherit td()
    [<SolidTypeComponentAttribute>]
    member props.constructor =
        td(class' = Lib.cn [| "p-2 align-middle [&:has([role=checkbox])]:pr-0"; props.class' |]).spread props
[<Erase>]
type DataTable<'T>() =
    interface VoidNode
    [<DefaultValue>] val mutable table: Table<'T>
    [<SolidTypeComponent>]
    member props.__ =
        let table = props.table
        Table() {
            TableHeader() {
                For(each = table.getHeaderGroups()) {
                    yield fun headerGroup _ ->
                        TableRow() {
                            For(each = headerGroup.headers) {
                                yield fun header _ ->
                                    TableHead(colspan = header.colSpan) {
                                        Show(when' = not header.isPlaceholder) {
                                            flexRender(header.column.columnDef.header, header.getContext())
                                        }
                                    }
                            }
                        }
                }
            }
            TableBody() {
                Show(
                    when' = unbox (table.getRowModel().rows.Length)
                    ,fallback = (TableRow() { TableCell(colspan = (8), class' = "h-24 text-center") { "No Results." } })
                    ) {
                    For(each = table.getRowModel().rows) {
                        yield fun row _ ->
                            TableRow().data("state", !!(row.getIsSelected() && !!"selected")) {
                                For(each = row.getVisibleCells()) {
                                    yield fun cell _ ->
                                        TableCell() {
                                            flexRender(cell.column.columnDef.cell, cell.getContext())
                                        }
                                }
                            }
                    }
                }
            }
        }
type User = {
    Code: string
    Name: string
    Color: string
}
[<SolidComponent>]
let codeColumn = ColumnDef<User>(
    accessorFn = fun user _ -> user.Code
    ,id = "code"
    ,header = !!"Code"
    ,cell = fun props ->
        div(class' = "w-14 hover:scale-102 flex justify-center bg-black text-white") {
                props.getValue() :?> string
            }
)
[<SolidComponent>]
let nameColumn = ColumnDef<User>(
    accessorFn = fun user _ -> user.Name
    ,header = !!"Name"
    ,cell = fun props ->
        div(class' = "w-14 hover:scale-102 flex justify-center bg-black text-white") {
                props.getValue() :?> string
            }
)
[<SolidComponent>]
let colorColumn = ColumnDef<User>(
    accessorFn = fun user _ -> user.Color
    ,header = !!"Color"
    ,cell = fun props ->
        div(class' = "w-14 hover:scale-102 flex justify-center bg-black text-white") {
                props.getValue() :?> string
            }
)
[<SolidComponent>]
let selectColumn =
    ColumnDef<User>(
        id = "select"
        ,enableHiding = false
        ,cell = fun cellProps ->
            Checkbox(
                checked' = cellProps.row.getIsSelected(),
                onChange = fun value -> cellProps.row.toggleSelected(!!value)
                ,ariaLabel = "Select row",
                class' = "translate-y-[2px]"
                )
        ,header = fun headerProps ->
            Checkbox(
                checked' = (headerProps.table.getIsAllPageRowsSelected()),
                indeterminate = (headerProps.table.getIsSomePageRowsSelected()),
                onChange = fun value -> headerProps.table.toggleAllPageRowsSelected(!!value)
                ,ariaLabel = "Select all"
                ,class' = "translate-y-[2px]"
                )
    )
let columnDefs = [|
    selectColumn
    codeColumn
    nameColumn
    colorColumn
|]

let userData = [|
        { Name = "Name"; Color = "Color"; Code = "Code" }
        { Name = "Name1"; Color = "Color1"; Code = "Code1" }
        { Name = "Name2"; Color = "Color2"; Code = "Code2" }
    |]

[<SolidComponent>]
let TestSelectableTable () =
    let selection,setSelection =
        createSignal <| RowSelectionState.init()
    let table = createTable(
            TableOptions<User>(
                getCoreRowModel = getCoreRowModel(),
                enableRowSelection = !!true,
                onRowSelectionChange = !!setSelection
            )   .data(fun _ -> userData)
                .columns(fun _ -> columnDefs)
                .stateFn(fun state ->
                    state.rowSelection(selection)
                    )
        )
    DataTable(table = table)
