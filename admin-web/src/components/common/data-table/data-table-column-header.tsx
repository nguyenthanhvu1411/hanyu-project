"use client";

import {
  ArrowDown,
  ArrowUp,
  ArrowUpDown,
} from "lucide-react";

import type {
  SortDirection,
} from "@/types/table.types";

interface DataTableColumnHeaderProps {
  title: string;
  sortable?: boolean;

  direction?: SortDirection;

  onSort?: () => void;
}

export function DataTableColumnHeader({
  title,
  sortable = false,
  direction,
  onSort,
}: DataTableColumnHeaderProps) {
  if (
    !sortable
  ) {
    return title;
  }

  return (
    <button
      type="button"
      onClick={
        onSort
      }
      className="
        inline-flex
        items-center
        gap-1
        transition
        hover:text-[#ef241c]
      "
    >
      {title}

      {direction ===
      "asc" ? (
        <ArrowUp
          size={13}
        />
      ) : direction ===
        "desc" ? (
        <ArrowDown
          size={13}
        />
      ) : (
        <ArrowUpDown
          size={13}
          className="text-[#aaa]"
        />
      )}
    </button>
  );
}
