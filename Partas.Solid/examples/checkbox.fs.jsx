
import { twMerge } from "tailwind-merge";
import { clsx } from "clsx";
import { createSignal, Show, For, splitProps } from "solid-js";
import { Indicator, Control, Input, Root } from "@kobalte/core/checkbox";
import Minus from "lucide-solid/icons/minus";
import Check from "lucide-solid/icons/check";
import { getCoreRowModel, createSolidTable, flexRender } from "@tanstack/solid-table";
import { Record } from "../fable_modules/fable-library-js.5.0.0-alpha.14/Types.js";
import { record_type, string_type } from "../fable_modules/fable-library-js.5.0.0-alpha.14/Reflection.js";

export function Lib_cn_Z35CD86D0(classes) {
    return twMerge(clsx(classes));
}

export function Checkbox(props) {
    const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["indeterminate", "class"]);
    return <Root indeterminate={PARTAS_LOCAL.indeterminate}
        class={Lib_cn_Z35CD86D0(["items-top group relative flex space-x-2", PARTAS_LOCAL.class])}
        {...PARTAS_OTHERS} bool:n$={false}>
        {(_arg) => <>
            <Input class="peer" />
            <Control class="size-4 shrink-0 rounded-sm border border-primary
                ring-offset-background disabled:cursor-not-allowed disabled:opacity-50
                peer-focus-visible:outline-none peer-focus-visible:ring-2 peer-focus-visible:ring-ring
                peer-focus-visible:ring-offset-2 data-[checked]:border-none
                data-[indeterminate]:border-none data-[checked]:bg-primary
                data-[indeterminate]:bg-primary data-[checked]:text-primary-foreground
                data-[indeterminate]:text-primary-foreground">
                <Indicator>
                    {PARTAS_LOCAL.indeterminate ? (<Minus class="size-4"
                        strokeWidth={2} />) : (<Check class="size-4"
                        strokeWidth={2} />)}
                </Indicator>
            </Control>
        </>}
    </Root>;
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
    return record_type("Partas.Solid.examples.checkbox.User", [], User, () => [["Code", string_type], ["Name", string_type], ["Color", string_type]]);
}

export const codeColumn = {
    id: "code",
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

export const selectColumn = {
    id: "select",
    header: (headerProps) => <Checkbox checked={headerProps.table.getIsAllPageRowsSelected()}
        indeterminate={headerProps.table.getIsSomePageRowsSelected()}
        onChange={(value) => {
            headerProps.table.toggleAllPageRowsSelected(value);
        }}
        ariaLabel="Select all"
        class="translate-y-[2px]" />,
    cell: (cellProps) => <Checkbox checked={cellProps.row.getIsSelected()}
        onChange={(value_1) => {
            cellProps.row.toggleSelected(value_1);
        }}
        ariaLabel="Select row"
        class="translate-y-[2px]" />,
    enableHiding: false,
};

export const columnDefs = [selectColumn, codeColumn, nameColumn, colorColumn];

export const userData = [new User("Code", "Name", "Color"), new User("Code1", "Name1", "Color1"), new User("Code2", "Name2", "Color2")];

export function TestSelectableTable() {
    let this$_6, this$_3, this$_1, this$_5;
    const patternInput = createSignal({});
    const table = createSolidTable((this$_6 = ((this$_3 = ((this$_1 = {
        getCoreRowModel: getCoreRowModel(),
        enableRowSelection: true,
        onRowSelectionChange: patternInput[1],
    }, (Object.defineProperty(this$_1, "data", {
        get: () => userData,
    }), this$_1))), (Object.defineProperty(this$_3, "columns", {
        get: () => columnDefs,
    }), this$_3))), (this$_6.state = ((this$_5 = {}, (void Object.defineProperty(this$_5, "rowSelection", {
        get: patternInput[0],
    }), this$_5))), this$_6)));
    return <DataTable table={table} />;
}

