"use client";

import {
  ChevronLeft,
  ChevronRight,
} from "lucide-react";

import {
  Pagination,
} from "@/components/ui/pagination";

interface DataTablePaginationProps {
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;

  pageSizeOptions?: number[];

  onPageChange: (
    page: number,
  ) => void;

  onPageSizeChange: (
    size: number,
  ) => void;
}

export function DataTablePagination({
  page,
  pageSize,
  totalItems,
  totalPages,
  pageSizeOptions = [
    10,
    20,
    50,
    100,
  ],
  onPageChange,
  onPageSizeChange,
}: DataTablePaginationProps) {
  const start =
    totalItems === 0
      ? 0
      : (page -
          1) *
          pageSize +
        1;

  const end =
    Math.min(
      page *
        pageSize,
      totalItems,
    );

  return (
    <div
      className="
        flex
        flex-col
        gap-3
        border-t
        border-[#eee9e2]
        px-4
        py-3
        text-[11px]
        text-[#747474]
        sm:flex-row
        sm:items-center
        sm:justify-between
      "
    >
      <div>
        Hiển thị{" "}
        <strong>
          {start}
        </strong>
        {" - "}
        <strong>
          {end}
        </strong>
        {" / "}
        <strong>
          {totalItems}
        </strong>{" "}
        bản ghi
      </div>

      <div className="flex flex-wrap items-center gap-3">
        <div className="flex items-center gap-2">
          <span>
            Số dòng:
          </span>

          <select
            value={
              pageSize
            }
            onChange={(
              event,
            ) =>
              onPageSizeChange(
                Number(
                  event
                    .target
                    .value,
                ),
              )
            }
            className="
              h-8
              rounded-[6px]
              border
              border-[#ddd8d1]
              bg-white
              px-2
              text-[11px]
              outline-none
            "
          >
            {pageSizeOptions.map(
              (
                size,
              ) => (
                <option
                  key={
                    size
                  }
                  value={
                    size
                  }
                >
                  {
                    size
                  }
                </option>
              ),
            )}
          </select>
        </div>

        <Pagination
          page={page}
          totalPages={
            Math.max(
              1,
              totalPages,
            )
          }
          onPageChange={
            onPageChange
          }
        />
      </div>
    </div>
  );
}
