"use client";

import {
  File,
  Paperclip,
  Trash2,
  UploadCloud,
} from "lucide-react";

import {
  useRef,
  useState,
} from "react";

import {
  cn,
} from "@/lib/utils/cn";

import {
  formatFileSize,
} from "@/utils/file.util";

interface FileUploadProps {
  value?: File[];

  onChange: (
    files: File[],
  ) => void;

  accept?: string;

  maxSizeMb?: number;

  maxFiles?: number;

  multiple?: boolean;

  disabled?: boolean;

  title?: string;

  description?: string;
}

export function FileUpload({
  value = [],
  onChange,
  accept = "*",
  maxSizeMb = 20,
  maxFiles = 10,
  multiple = true,
  disabled = false,
  title = "Tải tệp lên",
  description =
    "Kéo thả file hoặc nhấn để chọn từ máy tính.",
}: FileUploadProps) {
  const inputRef =
    useRef<HTMLInputElement>(
      null,
    );

  const [
    dragging,
    setDragging,
  ] = useState(false);

  function processFiles(
    files:
      | FileList
      | File[],
  ) {
    const maxBytes =
      maxSizeMb *
      1024 *
      1024;

    const incoming =
      Array.from(files)
        .filter(
          (file) =>
            file.size <=
            maxBytes,
        )
        .slice(
          0,
          multiple
            ? maxFiles
            : 1,
        );

    if (!multiple) {
      onChange(
        incoming.slice(
          0,
          1,
        ),
      );

      return;
    }

    const next = [
      ...value,
      ...incoming,
    ].slice(
      0,
      maxFiles,
    );

    onChange(
      next,
    );
  }

  function remove(
    index: number,
  ) {
    onChange(
      value.filter(
        (_, i) =>
          i !== index,
      ),
    );
  }

  return (
    <div>
      <input
        ref={inputRef}
        hidden
        type="file"
        accept={accept}
        multiple={
          multiple
        }
        disabled={
          disabled
        }
        onChange={(
          event,
        ) => {
          if (
            event.target
              .files
          ) {
            processFiles(
              event.target
                .files,
            );
          }

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
        }}
        onDragLeave={() =>
          setDragging(
            false,
          )
        }
        onDrop={(
          event,
        ) => {
          event.preventDefault();

          setDragging(
            false,
          );

          processFiles(
            event
              .dataTransfer
              .files,
          );
        }}
        className={cn(
          "flex",
          "min-h-[150px]",
          "w-full",
          "flex-col",
          "items-center",
          "justify-center",
          "rounded-[10px]",
          "border-2",
          "border-dashed",
          "px-5",
          "py-6",
          "text-center",
          "transition",

          dragging
            ? "border-[#ef241c] bg-[#fff5f3]"
            : "border-[#ddd7cf] bg-[#fdfcf9] hover:border-[#cbc3b6]",

          disabled &&
            "cursor-not-allowed opacity-50",
        )}
      >
        <div
          className="
            flex h-11 w-11
            items-center
            justify-center
            rounded-[10px]
            bg-[#fff0ee]
            text-[#ef241c]
          "
        >
          <UploadCloud
            size={21}
          />
        </div>

        <div
          className="
            mt-3
            text-[12px]
            font-semibold
            text-[#444]
          "
        >
          {title}
        </div>

        <div
          className="
            mt-1
            text-[10px]
            text-[#909090]
          "
        >
          {description}
        </div>

        <div
          className="
            mt-3
            rounded-full
            border
            border-[#e4ded6]
            bg-white
            px-3
            py-1
            text-[9px]
            text-[#999]
          "
        >
          Tối đa {maxSizeMb} MB/file
          {multiple &&
            ` · ${maxFiles} file`}
        </div>
      </button>

      {value.length >
        0 && (
        <div className="mt-3 space-y-2">
          {value.map(
            (
              file,
              index,
            ) => (
              <div
                key={`${file.name}-${index}`}
                className="
                  flex
                  items-center
                  gap-3
                  rounded-[8px]
                  border
                  border-[#e8e3dc]
                  bg-[#faf9f7]
                  px-3
                  py-[9px]
                "
              >
                <div
                  className="
                    flex h-9 w-9
                    shrink-0
                    items-center
                    justify-center
                    rounded-[7px]
                    bg-white
                    text-[#777]
                  "
                >
                  <File
                    size={16}
                  />
                </div>

                <div className="min-w-0 flex-1">
                  <div
                    className="
                      truncate
                      text-[11px]
                      font-medium
                      text-[#444]
                    "
                  >
                    {file.name}
                  </div>

                  <div
                    className="
                      mt-[2px]
                      text-[9px]
                      text-[#999]
                    "
                  >
                    {formatFileSize(
                      file.size,
                    )}
                  </div>
                </div>

                <button
                  type="button"
                  onClick={() =>
                    remove(
                      index,
                    )
                  }
                  className="
                    flex h-8 w-8
                    items-center
                    justify-center
                    rounded-[6px]
                    text-[#999]
                    hover:bg-[#fff0ee]
                    hover:text-[#ef241c]
                  "
                >
                  <Trash2
                    size={14}
                  />
                </button>
              </div>
            ),
          )}
        </div>
      )}
    </div>
  );
}
