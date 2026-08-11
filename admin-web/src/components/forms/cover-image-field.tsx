"use client";

import { ChangeEvent, useEffect, useMemo, useRef, useState } from "react";
import { ImageIcon, Link2, Loader2, Trash2, Upload } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { appToast } from "@/components/ui/toast";
import {
  getStorageObjectKey,
  mediaApi,
  toStorageReference,
} from "@/features/system/api/media.api";

interface CoverImageFieldProps {
  value?: string | null;
  onChange: (value: string) => void;
  disabled?: boolean;
}

const ACCEPTED_TYPES = ["image/jpeg", "image/png", "image/webp", "image/gif"];
const MAX_FILE_SIZE = 5 * 1024 * 1024;

export function CoverImageField({
  value,
  onChange,
  disabled = false,
}: CoverImageFieldProps) {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [uploading, setUploading] = useState(false);
  const [localPreview, setLocalPreview] = useState<string | null>(null);
  const [resolvedPreview, setResolvedPreview] = useState<string | null>(null);
  const [previewFailed, setPreviewFailed] = useState(false);

  const storageObjectKey = useMemo(() => getStorageObjectKey(value), [value]);
  const directValue = storageObjectKey ? "" : value?.trim() || "";
  const previewUrl = localPreview || resolvedPreview || directValue;
  const hasImage = previewUrl.length > 0 && !previewFailed;

  const displayName = useMemo(() => {
    if (storageObjectKey) {
      return storageObjectKey.split("/").filter(Boolean).pop() ?? storageObjectKey;
    }
    if (!value) return "";
    try {
      const parsed = new URL(value);
      return parsed.pathname.split("/").filter(Boolean).pop() ?? value;
    } catch {
      return value.split("/").filter(Boolean).pop() ?? value;
    }
  }, [storageObjectKey, value]);

  useEffect(() => {
    setPreviewFailed(false);
  }, [value, localPreview, resolvedPreview]);

  useEffect(() => {
    let active = true;

    if (!storageObjectKey || localPreview) {
      setResolvedPreview(null);
      return () => {
        active = false;
      };
    }

    void mediaApi
      .getReadUrl(storageObjectKey)
      .then((result) => {
        if (active) setResolvedPreview(result.url);
      })
      .catch(() => {
        if (active) {
          setResolvedPreview(null);
          setPreviewFailed(true);
        }
      });

    return () => {
      active = false;
    };
  }, [localPreview, storageObjectKey]);

  useEffect(() => {
    return () => {
      if (localPreview) URL.revokeObjectURL(localPreview);
    };
  }, [localPreview]);

  function replaceLocalPreview(next: string | null) {
    setLocalPreview((current) => {
      if (current) URL.revokeObjectURL(current);
      return next;
    });
  }

  async function selectFile(event: ChangeEvent<HTMLInputElement>) {
    const file = event.target.files?.[0];
    event.target.value = "";

    if (!file) return;

    if (!ACCEPTED_TYPES.includes(file.type)) {
      appToast.error("Ảnh không hợp lệ", "Chỉ hỗ trợ JPG, PNG, WEBP hoặc GIF.");
      return;
    }

    if (file.size > MAX_FILE_SIZE) {
      appToast.error("Ảnh quá lớn", "Dung lượng ảnh tối đa là 5 MB.");
      return;
    }

    const objectUrl = URL.createObjectURL(file);
    replaceLocalPreview(objectUrl);
    setUploading(true);

    try {
      const uploaded = await mediaApi.uploadImage(file);
      // Persist a stable canonical reference. Read URLs may expire and must never be saved.
      onChange(toStorageReference(uploaded.objectKey));
      setResolvedPreview(uploaded.url);
      replaceLocalPreview(null);
      appToast.success("Tải ảnh lên thành công.");
    } catch (error) {
      replaceLocalPreview(null);
      appToast.error(
        "Không thể tải ảnh lên",
        error instanceof Error ? error.message : "Vui lòng thử lại.",
      );
    } finally {
      setUploading(false);
    }
  }

  function clearImage() {
    replaceLocalPreview(null);
    setResolvedPreview(null);
    onChange("");
    setPreviewFailed(false);
  }

  return (
    <div className="space-y-3">
      <div className="grid gap-3 lg:grid-cols-[minmax(0,1fr)_180px]">
        <div className="space-y-2">
          <div className="relative">
            <Link2
              size={15}
              className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-[#999]"
            />
            <Input
              type="text"
              value={storageObjectKey ? "" : value ?? ""}
              onChange={(event) => {
                replaceLocalPreview(null);
                setResolvedPreview(null);
                onChange(event.target.value);
              }}
              disabled={disabled || uploading}
              placeholder={
                storageObjectKey
                  ? `Đã lưu trên Storage: ${storageObjectKey}`
                  : "https://example.com/cover.jpg"
              }
              className="pl-9"
            />
          </div>

          <p className="text-[10px] leading-4 text-[#8f8f8f]">
            Có thể dán URL ngoài hoặc tải file lên Storage. Ảnh upload được lưu bằng object key ổn định; URL đọc sẽ được tạo lại mỗi khi cần hiển thị.
          </p>
        </div>

        <div className="flex gap-2 lg:justify-end">
          <input
            ref={fileInputRef}
            type="file"
            accept="image/jpeg,image/png,image/webp,image/gif"
            className="hidden"
            onChange={(event) => void selectFile(event)}
            disabled={disabled || uploading}
          />

          <Button
            type="button"
            variant="outline"
            className="gap-2"
            disabled={disabled || uploading}
            onClick={() => fileInputRef.current?.click()}
          >
            {uploading ? <Loader2 size={15} className="animate-spin" /> : <Upload size={15} />}
            {uploading ? "Đang tải..." : "Chọn file"}
          </Button>

          {value ? (
            <Button
              type="button"
              variant="outline"
              aria-label="Xóa ảnh bìa"
              disabled={disabled || uploading}
              onClick={clearImage}
            >
              <Trash2 size={15} />
            </Button>
          ) : null}
        </div>
      </div>

      <div className="overflow-hidden rounded-[10px] border border-[#e4dfd8] bg-[#faf9f7]">
        {hasImage ? (
          <div className="grid min-h-[220px] place-items-center bg-[linear-gradient(45deg,#f4f2ee_25%,transparent_25%),linear-gradient(-45deg,#f4f2ee_25%,transparent_25%),linear-gradient(45deg,transparent_75%,#f4f2ee_75%),linear-gradient(-45deg,transparent_75%,#f4f2ee_75%)] bg-[length:20px_20px] bg-[position:0_0,0_10px,10px_-10px,-10px_0px] p-4">
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img
              src={previewUrl}
              alt="Xem trước ảnh bìa"
              className="max-h-[320px] w-auto max-w-full rounded-[8px] object-contain shadow-sm"
              onError={() => setPreviewFailed(true)}
            />
          </div>
        ) : (
          <div className="flex min-h-[180px] flex-col items-center justify-center gap-2 px-4 text-center">
            <div className="flex h-11 w-11 items-center justify-center rounded-[10px] bg-[#fff0ee] text-[#ef241c]">
              <ImageIcon size={20} />
            </div>
            <div className="text-[12px] font-medium text-[#555]">
              {previewFailed ? "Không thể tải ảnh từ Storage/URL này" : "Chưa có ảnh bìa"}
            </div>
            <div className="text-[10px] text-[#999]">
              Preview sẽ hiển thị ngay sau khi nhập URL hoặc chọn file.
            </div>
          </div>
        )}

        {value ? (
          <div className="border-t border-[#e9e4dd] bg-white px-3 py-2 text-[10px] text-[#888]">
            <span className="font-medium text-[#555]">Đang dùng:</span> {displayName}
          </div>
        ) : null}
      </div>
    </div>
  );
}
