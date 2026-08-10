"use client";

import {
  CheckCircle2,
  Download,
  MoreHorizontal,
  Trash2,
  X,
} from "lucide-react";

import {
  Button,
} from "@/components/ui/button";

interface DataTableBulkActionsProps {
  selectedCount: number;

  onClear: () => void;

  onDelete?: () => void;

  onExport?: () => void;

  onActivate?: () => void;

  customActions?: React.ReactNode;
}

export function DataTableBulkActions({
  selectedCount,
  onClear,
  onDelete,
  onExport,
  onActivate,
  customActions,
}: DataTableBulkActionsProps) {
  if (
    selectedCount <= 0
  ) {
    return null;
  }

  return (
    <div
      className="
        flex
        flex-col
        gap-2
        border-b
        border-[#f0d7d4]
        bg-[#fff6f4]
        px-4
        py-[10px]
        sm:flex-row
        sm:items-center
        sm:justify-between
      "
    >
      <div
        className="
          flex
          items-center
          gap-2
        "
      >
        <span
          className="
            flex h-6
            min-w-6
            items-center
            justify-center
            rounded-full
            bg-[#ef241c]
            px-2
            text-[10px]
            font-semibold
            text-white
          "
        >
          {selectedCount}
        </span>

        <span
          className="
            text-[11px]
            font-medium
            text-[#555]
          "
        >
          mục đã chọn
        </span>

        <button
          type="button"
          onClick={
            onClear
          }
          className="
            ml-1
            flex
            items-center
            gap-1
            text-[10px]
            text-[#888]
            hover:text-[#ef241c]
          "
        >
          <X size={12} />

          Bỏ chọn
        </button>
      </div>

      <div
        className="
          flex
          flex-wrap
          items-center
          gap-2
        "
      >
        {onExport && (
          <Button
            type="button"
            size="sm"
            variant="outline"
            onClick={
              onExport
            }
            className="gap-2"
          >
            <Download
              size={13}
            />

            Xuất
          </Button>
        )}

        {onActivate && (
          <Button
            type="button"
            size="sm"
            variant="outline"
            onClick={
              onActivate
            }
            className="
              gap-2
              text-[#16975b]
            "
          >
            <CheckCircle2
              size={13}
            />

            Kích hoạt
          </Button>
        )}

        {customActions}

        {onDelete && (
          <Button
            type="button"
            size="sm"
            variant="outline"
            onClick={
              onDelete
            }
            className="
              gap-2
              border-[#f0c8c5]
              text-[#ef241c]
            "
          >
            <Trash2
              size={13}
            />

            Xóa
          </Button>
        )}
      </div>
    </div>
  );
}
