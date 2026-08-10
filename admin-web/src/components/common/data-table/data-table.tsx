"use client";

import {
  useMemo,
  useState,
} from "react";

import {
  Checkbox,
} from "@/components/ui/checkbox";

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

import type {
  DataTableColumn,
  SortDirection,
} from "@/types/table.types";

import {
  cn,
} from "@/lib/utils/cn";

import {
  DataTableColumnHeader,
} from "./data-table-column-header";

import {
  DataTableEmpty,
} from "./data-table-empty";

import {
  DataTablePagination,
} from "./data-table-pagination";

import {
  DataTableSkeleton,
} from "./data-table-skeleton";

interface DataTableProps<T> {
  data: T[];

  columns: DataTableColumn<T>[];

  rowKey: (
    item: T,
  ) => string | number;

  loading?: boolean;

  selectable?: boolean;

  page: number;

  pageSize: number;

  totalItems: number;

  totalPages: number;

  onPageChange: (
    page: number,
  ) => void;

  onPageSizeChange: (
    size: number,
  ) => void;

  selectedIds?: Array<
    string | number
  >;

  onSelectionChange?: (
    ids: Array<
      string | number
    >,
  ) => void;
}

export function DataTable<T>({
  data,
  columns,
  rowKey,
  loading = false,
  selectable = true,
  page,
  pageSize,
  totalItems,
  totalPages,
  onPageChange,
  onPageSizeChange,
  selectedIds,
  onSelectionChange,
}: DataTableProps<T>) {
  const [
    internalSelectedIds,
    setInternalSelectedIds,
  ] = useState<
    Array<
      string | number
    >
  >([]);

  const currentSelectedIds =
    selectedIds ??
    internalSelectedIds;

  const [
    sortField,
    setSortField,
  ] = useState<
    string | undefined
  >();

  const [
    sortDirection,
    setSortDirection,
  ] = useState<
    SortDirection | undefined
  >();

  const allIds =
    useMemo(
      () =>
        data.map(
          rowKey,
        ),
      [
        data,
        rowKey,
      ],
    );

  const allSelected =
    data.length >
      0 &&
    allIds.every(
      (id) =>
        currentSelectedIds.includes(
          id,
        ),
    );

  function updateSelection(
    ids: Array<
      string | number
    >,
  ) {
    if (
      selectedIds ===
      undefined
    ) {
      setInternalSelectedIds(
        ids,
      );
    }

    onSelectionChange?.(
      ids,
    );
  }

  function toggleAll() {
    if (
      allSelected
    ) {
      const currentSet =
        new Set(
          allIds,
        );

      updateSelection(
        currentSelectedIds.filter(
          (id) =>
            !currentSet.has(
              id,
            ),
        ),
      );

      return;
    }

    const next =
      new Set([
        ...currentSelectedIds,
        ...allIds,
      ]);

    updateSelection(
      Array.from(
        next,
      ),
    );
  }

  function toggleRow(
    id:
      | string
      | number,
  ) {
    if (
      currentSelectedIds.includes(
        id,
      )
    ) {
      updateSelection(
        currentSelectedIds.filter(
          (
            item,
          ) =>
            item !==
            id,
        ),
      );
    } else {
      updateSelection([
        ...currentSelectedIds,
        id,
      ]);
    }
  }

  function handleSort(
    field: string,
  ) {
    if (
      sortField !==
      field
    ) {
      setSortField(
        field,
      );

      setSortDirection(
        "asc",
      );

      return;
    }

    if (
      sortDirection ===
      "asc"
    ) {
      setSortDirection(
        "desc",
      );
    } else if (
      sortDirection ===
      "desc"
    ) {
      setSortField(
        undefined,
      );

      setSortDirection(
        undefined,
      );
    } else {
      setSortDirection(
        "asc",
      );
    }
  }

  return (
    <div
      className="
        overflow-visible
        rounded-[11px]
        border
        border-[#e8e3dc]
        bg-white
        shadow-[0_2px_10px_rgba(0,0,0,0.02)]
      "
    >
      {loading ? (
        <DataTableSkeleton
          columns={
            columns.length +
            2
          }
        />
      ) : data.length ===
        0 ? (
        <DataTableEmpty />
      ) : (
        <>
          <Table>
            <TableHeader>
              <TableRow
                className="
                  bg-[#faf9f7]
                  hover:bg-[#faf9f7]
                "
              >
                {selectable && (
                  <TableHead className="w-[46px] text-center">
                    <div className="flex justify-center">
                      <Checkbox
                        checked={
                          allSelected
                        }
                        onCheckedChange={
                          toggleAll
                        }
                      />
                    </div>
                  </TableHead>
                )}

                <TableHead className="w-[64px] text-center">
                  STT
                </TableHead>

                {columns.map(
                  (
                    column,
                  ) => (
                    <TableHead
                      key={
                        column.id
                      }
                      style={{
                        width:
                          column.width,
                      }}
                      className={cn(
                        column.align ===
                          "center" &&
                          "text-center",

                        column.align ===
                          "right" &&
                          "text-right",
                      )}
                    >
                      <DataTableColumnHeader
                        title={
                          column.header
                        }
                        sortable={
                          column.sortable
                        }
                        direction={
                          sortField ===
                          column.id
                            ? sortDirection
                            : undefined
                        }
                        onSort={() =>
                          handleSort(
                            column.id,
                          )
                        }
                      />
                    </TableHead>
                  ),
                )}
              </TableRow>
            </TableHeader>

            <TableBody>
              {data.map(
                (
                  item,
                  index,
                ) => {
                  const id =
                    rowKey(
                      item,
                    );

                  const selected =
                    currentSelectedIds.includes(
                      id,
                    );

                  return (
                    <TableRow
                      key={
                        id
                      }
                      className={cn(
                        selected &&
                          "bg-[#fffaf8]",
                      )}
                    >
                      {selectable && (
                        <TableCell className="w-[46px] text-center">
                          <div className="flex justify-center">
                            <Checkbox
                              checked={
                                selected
                              }
                              onCheckedChange={() =>
                                toggleRow(
                                  id,
                                )
                              }
                            />
                          </div>
                        </TableCell>
                      )}

                      <TableCell className="w-[64px] text-center text-[#777]">
                        {(page -
                          1) *
                          pageSize +
                          index +
                          1}
                      </TableCell>

                      {columns.map(
                        (
                          column,
                        ) => {
                          let content:
                            React.ReactNode;

                          if (
                            column.cell
                          ) {
                            content =
                              column.cell(
                                item,
                                index,
                              );
                          } else if (
                            typeof column.accessor ===
                            "function"
                          ) {
                            content =
                              column.accessor(
                                item,
                              );
                          } else if (
                            column.accessor
                          ) {
                            content =
                              String(
                                item[
                                  column
                                    .accessor
                                ] ??
                                  "",
                              );
                          } else {
                            content =
                              null;
                          }

                          return (
                            <TableCell
                              key={
                                column.id
                              }
                              className={cn(
                                column.align ===
                                  "center" &&
                                  "text-center",

                                column.align ===
                                  "right" &&
                                  "text-right",
                              )}
                            >
                              {
                                content
                              }
                            </TableCell>
                          );
                        },
                      )}
                    </TableRow>
                  );
                },
              )}
            </TableBody>
          </Table>

          <DataTablePagination
            page={
              page
            }
            pageSize={
              pageSize
            }
            totalItems={
              totalItems
            }
            totalPages={
              totalPages
            }
            onPageChange={
              onPageChange
            }
            onPageSizeChange={
              onPageSizeChange
            }
          />
        </>
      )}
    </div>
  );
}
