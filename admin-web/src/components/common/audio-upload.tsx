"use client";

import {
  Music,
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

interface AudioUploadProps {
  value?: File | string | null;

  onChange?: (
    value: File | null,
  ) => void;

  maxSizeMb?: number;

  disabled?: boolean;
}

export function AudioUpload({
  value,
  onChange,
  maxSizeMb = 15,
  disabled = false,
}: AudioUploadProps) {
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
        mediaType="audio"
        accept="audio/mpeg,audio/wav,audio/mp4,audio/x-m4a"
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
          items-center
          gap-3
        "
      >
        <div
          className="
            flex h-10 w-10
            shrink-0
            items-center
            justify-center
            rounded-full
            bg-[#edf8f2]
            text-[#16975b]
          "
        >
          <Music
            size={18}
          />
        </div>

        <audio
          src={
            previewUrl
          }
          controls
          className="
            h-9
            min-w-0
            flex-1
          "
        />

        {!disabled && (
          <button
            type="button"
            onClick={() =>
              onChange?.(
                null,
              )
            }
            className="
              flex h-8 w-8
              shrink-0
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

      {value instanceof
        File && (
        <div className="mt-3">
          <UploadFileInfo
            name={
              value.name
            }
            size={
              value.size
            }
            mediaType="audio"
          />
        </div>
      )}
    </div>
  );
}
