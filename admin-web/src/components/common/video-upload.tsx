"use client";

import {
  PlayCircle,
  Trash2,
} from "lucide-react";

import {
  useEffect,
  useState,
} from "react";

import {
  UploadDropZone,
} from "./upload-drop-zone";

import {
  UploadFileInfo,
} from "./upload-file-info";

interface VideoUploadProps {
  value?: File | string | null;

  onChange?: (
    value: File | null,
  ) => void;

  maxSizeMb?: number;

  disabled?: boolean;
}

export function VideoUpload({
  value,
  onChange,
  maxSizeMb = 100,
  disabled = false,
}: VideoUploadProps) {
  const [
    previewUrl,
    setPreviewUrl,
  ] =
    useState<string | null>(
      typeof value ===
        "string"
        ? value
        : null,
    );

  useEffect(() => {
    if (
      typeof value ===
      "string"
    ) {
      setPreviewUrl(
        value,
      );

      return;
    }

    if (!value) {
      setPreviewUrl(
        null,
      );

      return;
    }

    const url =
      URL.createObjectURL(
        value,
      );

    setPreviewUrl(
      url,
    );

    return () =>
      URL.revokeObjectURL(
        url,
      );
  }, [value]);

  if (!previewUrl) {
    return (
      <UploadDropZone
        mediaType="video"
        accept="video/mp4,video/webm,video/quicktime"
        maxSizeMb={
          maxSizeMb
        }
        disabled={
          disabled
        }
        onFiles={(
          files,
        ) =>
          onChange?.(
            files[0],
          )
        }
      />
    );
  }

  return (
    <div
      className="
        overflow-hidden
        rounded-[10px]
        border
        border-[#e6e1da]
        bg-[#faf9f7]
      "
    >
      <div
        className="
          relative
          aspect-video
          bg-black
        "
      >
        <video
          src={
            previewUrl
          }
          controls
          preload="metadata"
          className="
            h-full
            w-full
            object-contain
          "
        />

        {!disabled && (
          <button
            type="button"
            title="Xóa video"
            onClick={() =>
              onChange?.(
                null,
              )
            }
            className="
              absolute
              right-3 top-3
              flex h-9 w-9
              items-center
              justify-center
              rounded-[7px]
              bg-white/95
              text-[#ef241c]
              shadow
              hover:bg-white
            "
          >
            <Trash2
              size={16}
            />
          </button>
        )}
      </div>

      <div
        className="
          flex
          items-center
          gap-2
          px-3
          py-2
          text-[10px]
          text-[#777]
        "
      >
        <PlayCircle
          size={14}
        />

        Xem trước video
      </div>

      {value instanceof
        File && (
        <div
          className="
            border-t
            border-[#e7e2db]
            p-3
          "
        >
          <UploadFileInfo
            name={
              value.name
            }
            size={
              value.size
            }
            mediaType="video"
          />
        </div>
      )}
    </div>
  );
}
