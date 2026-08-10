"use client";

import {
  FileSpreadsheet,
  Trash2,
} from "lucide-react";

import {
  formatFileSize,
} from "@/utils/file.util";

interface ExcelFileInfoProps {
  file: File;

  sheetName?: string;

  totalRows?: number;

  onRemove: () => void;
}

export function ExcelFileInfo({
  file,
  sheetName,
  totalRows,
  onRemove,
}: ExcelFileInfoProps) {
  return (
    <div
      className="
        flex
        items-center
        gap-3
        rounded-[10px]
        border
        border-[#e6e1da]
        bg-[#faf9f7]
        p-3
      "
    >
      <div
        className="
          flex
          h-11
          w-11
          shrink-0
          items-center
          justify-center
          rounded-[9px]
          bg-[#edf8f2]
          text-[#16975b]
        "
      >
        <FileSpreadsheet
          size={22}
        />
      </div>

      <div
        className="
          min-w-0
          flex-1
        "
      >
        <div
          className="
            truncate
            text-[13px]
            font-medium
            text-[#333]
          "
        >
          {file.name}
        </div>

        <div
          className="
            mt-[3px]
            flex
            flex-wrap
            gap-x-3
            gap-y-1
            text-[10px]
            text-[#8a8a8a]
          "
        >
          <span>
            {formatFileSize(
              file.size,
            )}
          </span>

          {sheetName && (
            <span>
              Sheet:{" "}
              {sheetName}
            </span>
          )}

          {totalRows !==
            undefined && (
            <span>
              {
                totalRows
              }{" "}
              dòng dữ liệu
            </span>
          )}
        </div>
      </div>

      <button
        type="button"
        onClick={
          onRemove
        }
        className="
          flex
          h-8
          w-8
          shrink-0
          items-center
          justify-center
          rounded-[7px]
          text-[#999]
          transition
          hover:bg-[#fff0ee]
          hover:text-[#ef241c]
        "
        title="Xóa file"
      >
        <Trash2
          size={16}
        />
      </button>
    </div>
  );
}
