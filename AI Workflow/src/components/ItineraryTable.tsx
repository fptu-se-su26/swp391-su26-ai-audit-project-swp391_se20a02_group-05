"use client";

import {
  createColumnHelper,
  flexRender,
  getCoreRowModel,
  useReactTable,
} from "@tanstack/react-table";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";

type ItineraryRow = {
  day: number;
  time: string;
  activity: string;
  location: string;
  cost: number;
  type: string;
};

const columnHelper = createColumnHelper<ItineraryRow>();

const columns = [
  columnHelper.accessor("day", {
    header: "Day",
    cell: (info) => <span className="font-medium">Day {info.getValue()}</span>,
  }),
  columnHelper.accessor("time", {
    header: "Time",
    cell: (info) => info.getValue(),
  }),
  columnHelper.accessor("activity", {
    header: "Activity",
    cell: (info) => info.getValue(),
  }),
  columnHelper.accessor("location", {
    header: "Location",
    cell: (info) => <span className="text-muted-foreground">{info.getValue()}</span>,
  }),
  columnHelper.accessor("type", {
    header: "Category",
    cell: (info) => {
      const type = info.getValue();
      const variant = type === "food" ? "default" : type === "activity" ? "secondary" : "outline";
      return <Badge variant={variant as any} className="capitalize">{type}</Badge>;
    },
  }),
  columnHelper.accessor("cost", {
    header: () => <div className="text-right">Est. Cost</div>,
    cell: (info) => <div className="text-right font-medium">\${info.getValue()}</div>,
  }),
];

export function ItineraryTable({ data }: { data: ItineraryRow[] }) {
  const table = useReactTable({
    data,
    columns,
    getCoreRowModel: getCoreRowModel(),
  });

  return (
    <div className="rounded-md border border-border/50 overflow-hidden">
      <Table>
        <TableHeader className="bg-muted/50">
          {table.getHeaderGroups().map((headerGroup) => (
            <TableRow key={headerGroup.id}>
              {headerGroup.headers.map((header) => (
                <TableHead key={header.id}>
                  {header.isPlaceholder
                    ? null
                    : flexRender(
                        header.column.columnDef.header,
                        header.getContext()
                      )}
                </TableHead>
              ))}
            </TableRow>
          ))}
        </TableHeader>
        <TableBody>
          {table.getRowModel().rows?.length ? (
            table.getRowModel().rows.map((row) => (
              <TableRow
                key={row.id}
                data-state={row.getIsSelected() && "selected"}
              >
                {row.getVisibleCells().map((cell) => (
                  <TableCell key={cell.id}>
                    {flexRender(cell.column.columnDef.cell, cell.getContext())}
                  </TableCell>
                ))}
              </TableRow>
            ))
          ) : (
            <TableRow>
              <TableCell colSpan={columns.length} className="h-24 text-center">
                No results.
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
    </div>
  );
}
