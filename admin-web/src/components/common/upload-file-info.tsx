"use client";

import {
  FileAudio,
  FileVideo,
  ImageIcon,
  Trash2,
} from "lucide-react";

import {
  formatFileSize,
} from "@/utils/file.util";

import type {
  UploadMediaType,
} from "@/types/upload.types";

interface UploadFileInfoProps {
  name: string;

  size?: number;

  mediaType: UploadMediaType;

  onRemove?: () => void;
}

export function UploadFileInfo({
  name,
  size,
  mediaType,
  onRemove,
}: UploadFileInfoProps) {
  const Icon =
    mediaType ===
    "image"
      ? ImageIcon
      : mediaType ===
          "audio"
        ? FileAudio
        : FileVideo;

  return (
    <div
      className="
        flex items-center
        gap-3
        rounded-[8px]
        border
        border-[#e7e2db]
        bg-[#faf9f7]
        p-3
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
          text-[#ef241c]
        "
      >
        <Icon size={17} />
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
          {name}
        </div>

        {size !==
          undefined && (
          <div
            className="
              mt-[2px]
              text-[9px]
              text-[#999]
            "
          >
            {formatFileSize(
              size,
            )}
          </div>
        )}
      </div>

      {onRemove && (
        <button
          type="button"
          onClick={
            onRemove
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
            size={15}
          />
        </button>
      )}
    </div>
  );
}
