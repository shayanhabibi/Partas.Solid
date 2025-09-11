
import { twMerge } from "tailwind-merge";
import { clsx } from "clsx";
import { Show, For, splitProps } from "solid-js";
import { getCoreRowModel, createSolidTable, flexRender } from "@tanstack/solid-table";
import { Record } from "../fable_modules/fable-library-js.5.0.0-alpha.14/Types.js";
import { record_type, string_type } from "../fable_modules/fable-library-js.5.0.0-alpha.14/Reflection.js";
import "~/app.css";


export function Lib_cn_Z35CD86D0(classes) {
    return twMerge(clsx(classes));
}

export function TableCaption(props) {
    const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["class"]);
    return <caption class={Lib_cn_Z35CD86D0(["mt-4 text-sm text-muted-foreground", PARTAS_LOCAL.class])}
        {...PARTAS_OTHERS} bool:n$={false} />;
}

export function Table(props) {
    const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["class"]);
    return <div class="relative w-full overflow-auto">
        <table class={Lib_cn_Z35CD86D0(["w-full caption-bottom text-sm", PARTAS_LOCAL.class])}
            {...PARTAS_OTHERS} bool:n$={false} />
    </div>;
}

export function TableHeader(props) {
    const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["class"]);
    return <thead class={Lib_cn_Z35CD86D0(["[&_tr]:border-b", PARTAS_LOCAL.class])}
        {...PARTAS_OTHERS} bool:n$={false} />;
}

export function TableBody(props) {
    const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["class"]);
    return <tbody class={Lib_cn_Z35CD86D0(["[&_tr:last-child]:border-0", PARTAS_LOCAL.class])}
        {...PARTAS_OTHERS} bool:n$={false} />;
}

export function TableFooter(props) {
    const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["class"]);
    return <tfoot class={Lib_cn_Z35CD86D0(["bg-primary font-medium text-primary-foreground", PARTAS_LOCAL.class])}
        {...PARTAS_OTHERS} bool:n$={false} />;
}

export function TableRow(props) {
    const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["class"]);
    return <tr class={Lib_cn_Z35CD86D0(["border-b transition-colors hover:bg-muted/50 data-[state=selected]:bg-muted", PARTAS_LOCAL.class])}
        {...PARTAS_OTHERS} bool:n$={false} />;
}

export function TableHead(props) {
    const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["class"]);
    return <th class={Lib_cn_Z35CD86D0(["h-10 px-2 text-left align-middle font-medium\r\n                text-muted-foreground [&:has([role=checkbox])]:pr-0", PARTAS_LOCAL.class])}
        {...PARTAS_OTHERS} bool:n$={false} />;
}

export function TableCell(props) {
    const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["class"]);
    return <td class={Lib_cn_Z35CD86D0(["p-2 align-middle [&:has([role=checkbox])]:pr-0", PARTAS_LOCAL.class])}
        {...PARTAS_OTHERS} bool:n$={false} />;
}

export function DataTable(props) {
    const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["table"]);
    const table = PARTAS_LOCAL.table;
    return <Table>
        <TableHeader>
            <For each={table.getHeaderGroups()}>
                {(headerGroup, _arg) => <TableRow>
                    <For each={headerGroup.headers}>
                        {(header, _arg_1) => <TableHead colspan={header.colSpan}>
                            <Show when={!header.isPlaceholder}>
                                {flexRender(header.column.columnDef.header, header.getContext())}
                            </Show>
                        </TableHead>}
                    </For>
                </TableRow>}
            </For>
        </TableHeader>
        <TableBody>
            <Show when={table.getRowModel().rows.length}
                fallback={<TableRow>
                    <TableCell colspan={8}
                        class="h-24 text-center">
                        No Results.
                    </TableCell>
                </TableRow>}>
                <For each={table.getRowModel().rows}>
                    {(row, _arg_2) => <TableRow data-state={row.getIsSelected() && "selected"}>
                        <For each={row.getVisibleCells()}>
                            {(cell, _arg_3) => <TableCell>
                                {flexRender(cell.column.columnDef.cell, cell.getContext())}
                            </TableCell>}
                        </For>
                    </TableRow>}
                </For>
            </Show>
        </TableBody>
    </Table>;
}

export class User extends Record {
    constructor(Code, Name, Color) {
        super();
        this.Code = Code;
        this.Name = Name;
        this.Color = Color;
    }
}

export function User_$reflection() {
    return record_type("Partas.Solid.examples.datatable.User", [], User, () => [["Code", string_type], ["Name", string_type], ["Color", string_type]]);
}

export const codeColumn = {
    accessorFn: (user, _arg) => user.Code,
    header: "Code",
    cell: (props) => <div class="w-14 hover:scale-102 flex justify-center bg-black text-white">
        {props.getValue()}
    </div>,
};

export const nameColumn = {
    accessorFn: (user, _arg) => user.Name,
    header: "Name",
    cell: (props) => <div class="w-14 hover:scale-102 flex justify-center bg-black text-white">
        {props.getValue()}
    </div>,
};

export const colorColumn = {
    accessorFn: (user, _arg) => user.Color,
    header: "Color",
    cell: (props) => <div class="w-14 hover:scale-102 flex justify-center bg-black text-white">
        {props.getValue()}
    </div>,
};

export const columnDefs = [codeColumn, nameColumn, colorColumn];

export const userData = [new User("Code", "Name", "Color"), new User("Code1", "Name1", "Color1"), new User("Code2", "Name2", "Color2")];

export function Test() {
    let this$_3, this$_1;
    const table = createSolidTable((this$_3 = ((this$_1 = {
        getCoreRowModel: getCoreRowModel(),
    }, (Object.defineProperty(this$_1, "data", {
        get: () => userData,
    }), this$_1))), (Object.defineProperty(this$_3, "columns", {
        get: () => columnDefs,
    }), this$_3)));
    return <DataTable table={table} />;
}

