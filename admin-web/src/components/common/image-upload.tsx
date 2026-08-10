"use client";

import {
  Eye,
  ImageIcon,
  Trash2,
} from "lucide-react";

import {
  useEffect,
  useState,
} from "react";

import {
  UploadDropZone,
} from "./upload-drop-zone";

interface ImageUploadProps {
  value?: File | string | null;

  onChange?: (
    value: File | null,
  ) => void;

  maxSizeMb?: number;

  disabled?: boolean;

  aspectRatio?: string;

  description?: string;
}

export function ImageUpload({
  value,
  onChange,
  maxSizeMb = 5,
  disabled = false,
  aspectRatio = "aspect-[16/9]",
  description,
}: ImageUploadProps) {
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

  function handleFile(
    file: File,
  ) {
    onChange?.(
      file,
    );
  }

  function remove() {
    onChange?.(
      null,
    );

    setPreviewUrl(
      null,
    );
  }

  if (!previewUrl) {
    return (
      <UploadDropZone
        mediaType="image"
        accept="image/jpeg,image/png,image/webp"
        maxSizeMb={
          maxSizeMb
        }
        disabled={
          disabled
        }
        description={
          description ??
          "Kéo thả hoặc chọn JPG, PNG, WEBP"
        }
        onFiles={(
          files,
        ) =>
          handleFile(
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
        className={`
          relative
          w-full
          overflow-hidden
          bg-[#f3f2ef]
          ${aspectRatio}
        `}
      >
        <img
          src={
            previewUrl
          }
          alt="Ảnh xem trước"
          className="
            h-full
            w-full
            object-cover
          "
        />

        <div
          className="
            absolute
            inset-0
            flex items-center
            justify-center
            gap-2
            bg-black/0
            opacity-0
            transition
            hover:bg-black/30
            hover:opacity-100
          "
        >
          <a
            href={
              previewUrl
            }
            target="_blank"
            rel="noreferrer"
            className="
              flex h-9 w-9
              items-center
              justify-center
              rounded-[7px]
              bg-white
              text-[#555]
              shadow
            "
          >
            <Eye
              size={16}
            />
          </a>

          {!disabled && (
            <button
              type="button"
              onClick={
                remove
              }
              className="
                flex h-9 w-9
                items-center
                justify-center
                rounded-[7px]
                bg-white
                text-[#ef241c]
                shadow
              "
            >
              <Trash2
                size={16}
              />
            </button>
          )}
        </div>
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
        <ImageIcon
          size={13}
        />

        Ảnh xem trước
      </div>
    </div>
  );
}
