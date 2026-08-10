"use client";

import {
  FileSpreadsheet,
  UploadCloud,
} from "lucide-react";

import {
  useRef,
  useState,
} from "react";

import {
  cn,
} from "@/lib/utils/cn";

interface ExcelUploadZoneProps {
  disabled?: boolean;

  onFileSelect: (
    file: File,
  ) => void;
}

export function ExcelUploadZone({
  disabled = false,
  onFileSelect,
}: ExcelUploadZoneProps) {
  const inputRef =
    useRef<HTMLInputElement>(
      null,
    );

  const [
    dragging,
    setDragging,
  ] = useState(false);

  function validateFile(
    file?: File,
  ) {
    if (!file) {
      return;
    }

    const extension =
      file.name
        .split(".")
        .pop()
        ?.toLowerCase();

    if (
      extension !== "xlsx" &&
      extension !== "xls"
    ) {
      alert(
        "Chỉ hỗ trợ file .xlsx hoặc .xls",
      );

      return;
    }

    onFileSelect(
      file,
    );
  }

  return (
    <>
      <input
        ref={
          inputRef
        }
        type="file"
        accept=".xlsx,.xls"
        hidden
        disabled={
          disabled
        }
        onChange={(
          event,
        ) => {
          validateFile(
            event
              .target
              .files?.[0],
          );

          event.target.value =
            "";
        }}
      />

      <button
        type="button"
        disabled={
          disabled
        }
        onClick={() =>
          inputRef.current?.click()
        }
        onDragEnter={(
          event,
        ) => {
          event.preventDefault();

          setDragging(
            true,
          );
        }}
        onDragOver={(
          event,
        ) => {
          event.preventDefault();

          setDragging(
            true,
          );
        }}
        onDragLeave={(
          event,
        ) => {
          event.preventDefault();

          setDragging(
            false,
          );
        }}
        onDrop={(
          event,
        ) => {
          event.preventDefault();

          setDragging(
            false,
          );

          validateFile(
            event
              .dataTransfer
              .files?.[0],
          );
        }}
        className={cn(
          "group",
          "flex",
          "min-h-[205px]",
          "w-full",
          "flex-col",
          "items-center",
          "justify-center",
          "rounded-[12px]",
          "border-2",
          "border-dashed",
          "px-6",
          "py-8",
          "text-center",
          "transition-all",

          dragging
            ? "border-[#ef241c] bg-[#fff6f4]"
            : "border-[#ded8ce] bg-[#fdfcf9] hover:border-[#d0c5b7] hover:bg-[#fffaf5]",

          disabled &&
            "cursor-not-allowed opacity-60",
        )}
      >
        <div
          className={cn(
            "flex",
            "h-[58px]",
            "w-[58px]",
            "items-center",
            "justify-center",
            "rounded-[14px]",
            "bg-[#fff0ee]",
            "text-[#ef241c]",
            "transition-transform",

            !disabled &&
              "group-hover:-translate-y-1",
          )}
        >
          {dragging ? (
            <FileSpreadsheet
              size={27}
              strokeWidth={
                1.7
              }
            />
          ) : (
            <UploadCloud
              size={28}
              strokeWidth={
                1.7
              }
            />
          )}
        </div>

        <div
          className="
            mt-4
            text-[14px]
            font-semibold
            text-[#333]
          "
        >
          Kéo và thả file Excel vào đây
        </div>

        <div
          className="
            mt-1
            text-[12px]
            text-[#888]
          "
        >
          hoặc nhấn để chọn file từ máy tính
        </div>

        <div
          className="
            mt-4
            rounded-full
            border
            border-[#e4ded4]
            bg-white
            px-3
            py-[5px]
            text-[10px]
            text-[#888]
          "
        >
          Hỗ trợ .xlsx, .xls
          · Tối đa 10 MB
        </div>
      </button>
    </>
  );
}
