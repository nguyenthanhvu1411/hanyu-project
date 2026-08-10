"use client";

import {
  ImagePlus,
  Music,
  UploadCloud,
  Video,
} from "lucide-react";

import {
  useRef,
  useState,
} from "react";

import {
  cn,
} from "@/lib/utils/cn";

import type {
  UploadMediaType,
} from "@/types/upload.types";

interface UploadDropZoneProps {
  mediaType: UploadMediaType;

  accept: string;

  maxSizeMb: number;

  multiple?: boolean;

  disabled?: boolean;

  title?: string;

  description?: string;

  onFiles: (
    files: File[],
  ) => void;
}

const typeConfig = {
  image: {
    icon: ImagePlus,

    defaultTitle:
      "Tải hình ảnh",

    description:
      "JPG, PNG, WEBP",
  },

  audio: {
    icon: Music,

    defaultTitle:
      "Tải tệp âm thanh",

    description:
      "MP3, WAV, M4A",
  },

  video: {
    icon: Video,

    defaultTitle:
      "Tải video",

    description:
      "MP4, WEBM, MOV",
  },
};

export function UploadDropZone({
  mediaType,
  accept,
  maxSizeMb,
  multiple = false,
  disabled = false,
  title,
  description,
  onFiles,
}: UploadDropZoneProps) {
  const inputRef =
    useRef<HTMLInputElement>(
      null,
    );

  const [
    dragging,
    setDragging,
  ] = useState(false);

  const config =
    typeConfig[
      mediaType
    ];

  const Icon =
    config.icon;

  function selectFiles(
    files:
      | FileList
      | File[],
  ) {
    if (disabled) {
      return;
    }

    const list =
      Array.from(files);

    const maxBytes =
      maxSizeMb *
      1024 *
      1024;

    const valid =
      list.filter(
        (file) =>
          file.size <=
          maxBytes,
      );

    if (
      valid.length === 0
    ) {
      return;
    }

    onFiles(
      multiple
        ? valid
        : [valid[0]],
    );
  }

  return (
    <>
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
            selectFiles(
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

          selectFiles(
            event
              .dataTransfer
              .files,
          );
        }}
        className={cn(
          "group",
          "flex min-h-[155px]",
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
          "transition-all",

          dragging
            ? "border-[#ef241c] bg-[#fff5f3]"
            : "border-[#ddd7cf] bg-[#fdfcf9] hover:border-[#cfc6b8] hover:bg-[#fffaf5]",

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
            transition
            group-hover:-translate-y-[2px]
          "
        >
          {dragging ? (
            <UploadCloud
              size={22}
            />
          ) : (
            <Icon
              size={21}
            />
          )}
        </div>

        <div
          className="
            mt-3
            text-[12px]
            font-semibold
            text-[#444]
          "
        >
          {title ??
            config.defaultTitle}
        </div>

        <div
          className="
            mt-1
            text-[10px]
            leading-[16px]
            text-[#8f8f8f]
          "
        >
          {description ??
            `Kéo thả file hoặc nhấn để chọn · ${config.description}`}
        </div>

        <div
          className="
            mt-3
            rounded-full
            border
            border-[#e3ded6]
            bg-white
            px-3
            py-[4px]
            text-[9px]
            text-[#999]
          "
        >
          Tối đa {maxSizeMb} MB
          {multiple &&
            " / file"}
        </div>
      </button>
    </>
  );
}
