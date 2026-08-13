"use client";

import { FileText, Headphones, ImageIcon, Save, UploadCloud } from "lucide-react";
import { useEffect, useMemo, useState } from "react";

import { AudioAssetPicker } from "@/features/vocabulary/components/audio-asset-picker";
import { Alert } from "@/components/ui/alert";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Modal } from "@/components/ui/modal";
import { Select } from "@/components/ui/select";
import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";

import {
  LessonAssetType,
  lessonAssetTypeLabels,
  type AdminLessonAsset,
  type CreateLessonAssetRequest,
} from "../types/lesson.types";

interface LessonAssetModalProps {
  open: boolean;
  asset?: AdminLessonAsset | null;
  defaultSortOrder: number;
  loading?: boolean;
  onClose: () => void;
  onSubmit: (request: CreateLessonAssetRequest) => void | Promise<void>;
}

interface UploadResult {
  objectKey: string;
  publicUrl: string;
  originalFileName: string;
  contentType: string;
  fileSizeBytes: number;
}

const typeOptions = Object.entries(lessonAssetTypeLabels).map(([value, label]) => ({ value, label }));

export function LessonAssetModal({
  open,
  asset,
  defaultSortOrder,
  loading = false,
  onClose,
  onSubmit,
}: LessonAssetModalProps) {
  const [assetType, setAssetType] = useState<LessonAssetType>(LessonAssetType.Image);
  const [url, setUrl] = useState("");
  const [captionVi, setCaptionVi] = useState("");
  const [audioAssetId, setAudioAssetId] = useState<number | null>(null);
  const [sortOrder, setSortOrder] = useState(defaultSortOrder);
  const [uploadFile, setUploadFile] = useState<File | null>(null);
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!open) return;
    setAssetType(asset?.assetType ?? LessonAssetType.Image);
    setUrl(asset?.url ?? "");
    setCaptionVi(asset?.captionVi ?? "");
    setAudioAssetId(asset?.audioAssetId ?? null);
    setSortOrder(asset?.sortOrder ?? defaultSortOrder);
    setUploadFile(null);
    setError(null);
  }, [open, asset, defaultSortOrder]);

  const uploadConfig = useMemo(() => {
    if (assetType === LessonAssetType.Image) {
      return { endpoint: API_ENDPOINTS.ADMIN.UPLOAD_IMAGE, accept: "image/*", label: "hình ảnh" };
    }
    if (assetType === LessonAssetType.Document) {
      return { endpoint: API_ENDPOINTS.ADMIN.UPLOAD_DOCUMENT, accept: ".pdf,.doc,.docx,.ppt,.pptx,.txt,application/pdf", label: "tài liệu" };
    }
    return null;
  }, [assetType]);

  async function upload() {
    if (!uploadFile || !uploadConfig) return;
    setUploading(true);
    setError(null);
    try {
      const formData = new FormData();
      formData.append("file", uploadFile);
      const result = await apiClient<UploadResult>(uploadConfig.endpoint, { method: "POST", body: formData });
      setUrl(result.publicUrl);
      setUploadFile(null);
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Không thể upload tài nguyên.");
    } finally {
      setUploading(false);
    }
  }

  async function submit() {
    if (assetType === LessonAssetType.Audio && !audioAssetId && !url.trim()) {
      setError("Audio LessonAsset phải có AudioAsset hoặc URL.");
      return;
    }
    if (assetType !== LessonAssetType.Audio && !url.trim()) {
      setError("Hãy upload file hoặc nhập URL tài nguyên.");
      return;
    }

    setError(null);
    await onSubmit({
      assetType,
      url: url.trim() || null,
      captionVi: captionVi.trim() || null,
      audioAssetId: assetType === LessonAssetType.Audio ? audioAssetId : null,
      sortOrder: Math.max(0, sortOrder),
    });
  }

  const busy = loading || uploading;

  return (
    <Modal
      open={open}
      onClose={onClose}
      size="lg"
      title={asset ? "Chỉnh sửa tài nguyên Lesson" : "Thêm tài nguyên Lesson"}
      description="Quản lý Image, Audio và Document dùng chung cho các LessonSection."
      footer={
        <div className="flex justify-end gap-2">
          <Button type="button" variant="outline" size="md" disabled={busy} onClick={onClose}>Hủy</Button>
          <Button type="button" size="md" loading={loading} disabled={uploading} onClick={() => void submit()} className="gap-2">
            <Save size={14} /> {asset ? "Lưu thay đổi" : "Thêm tài nguyên"}
          </Button>
        </div>
      }
    >
      <div className="space-y-4">
        {error && <Alert variant="error">{error}</Alert>}

        <div className="grid gap-4 md:grid-cols-2">
          <Field label="Loại tài nguyên">
            <Select
              value={String(assetType)}
              onValueChange={(value) => {
                setAssetType(Number(value) as LessonAssetType);
                setUrl("");
                setAudioAssetId(null);
                setUploadFile(null);
              }}
              options={typeOptions}
              disabled={Boolean(asset)}
            />
          </Field>
          <Field label="Thứ tự">
            <Input type="number" min={0} value={sortOrder} onChange={(event) => setSortOrder(Number(event.target.value) || 0)} />
          </Field>
        </div>

        <Field label="Caption">
          <Input value={captionVi} onChange={(event) => setCaptionVi(event.target.value)} placeholder="Mô tả ngắn cho tài nguyên" />
        </Field>

        {assetType === LessonAssetType.Audio ? (
          <AudioAssetPicker
            value={audioAssetId}
            onChange={setAudioAssetId}
            kind={2}
            title="Audio Lesson"
            description="Chọn AudioAsset Kind=Lesson hoặc upload audio mới trực tiếp lên Backblaze B2."
            disabled={busy}
          />
        ) : (
          <>
            <div className="rounded-[9px] border border-dashed border-[#ddd8d1] bg-[#faf9f7] p-4">
              <div className="flex items-center gap-2 text-[13px] font-medium text-[#555]">
                <UploadCloud size={16} className="text-[#ef241c]" /> Upload {uploadConfig?.label}
              </div>
              <div className="mt-3 flex flex-col gap-3 md:flex-row md:items-center">
                <input
                  type="file"
                  accept={uploadConfig?.accept}
                  disabled={busy}
                  onChange={(event) => setUploadFile(event.target.files?.[0] ?? null)}
                  className="min-w-0 flex-1 text-[13px] text-[#666] file:mr-3 file:rounded-[7px] file:border file:border-[#dedede] file:bg-white file:px-3 file:py-2 file:text-[13px] file:font-medium file:text-[#444]"
                />
                <Button type="button" variant="outline" size="md" loading={uploading} disabled={!uploadFile || loading} onClick={() => void upload()}>
                  Upload
                </Button>
              </div>
            </div>

            <Field label="URL tài nguyên">
              <Input value={url} onChange={(event) => setUrl(event.target.value)} placeholder="https://... hoặc URL sau khi upload" />
            </Field>
          </>
        )}

        <div>
          <div className="mb-2 text-[13px] font-medium text-[#555]">Preview</div>
          <AssetPreview assetType={assetType} url={url} audioAssetId={audioAssetId} caption={captionVi} />
        </div>
      </div>
    </Modal>
  );
}

function AssetPreview({
  assetType,
  url,
  audioAssetId,
  caption,
}: {
  assetType: LessonAssetType;
  url: string;
  audioAssetId: number | null;
  caption: string;
}) {
  if (assetType === LessonAssetType.Image && url) {
    return (
      <figure className="overflow-hidden rounded-[9px] border border-[#e8e3dc] bg-[#faf9f7]">
        {/* eslint-disable-next-line @next/next/no-img-element */}
        <img src={url} alt={caption || "Lesson asset"} className="max-h-[320px] w-full object-contain" />
        {caption && <figcaption className="px-3 py-2 text-[12px] text-[#777]">{caption}</figcaption>}
      </figure>
    );
  }

  if (assetType === LessonAssetType.Audio) {
    return (
      <div className="rounded-[9px] border border-[#e8e3dc] bg-[#faf9f7] p-4">
        <div className="flex items-center gap-2"><Headphones size={16} /><Badge>Âm thanh</Badge></div>
        <div className="mt-2 text-[12px] text-[#777]">{audioAssetId ? `AudioAsset #${audioAssetId}` : url || "Chưa chọn audio"}</div>
      </div>
    );
  }

  return (
    <div className="rounded-[9px] border border-[#e8e3dc] bg-[#faf9f7] p-4">
      <div className="flex items-center gap-2">
        {assetType === LessonAssetType.Document ? <FileText size={16} /> : <ImageIcon size={16} />}
        <Badge>{lessonAssetTypeLabels[assetType]}</Badge>
      </div>
      <div className="mt-2 break-all text-[12px] text-[#777]">{url || "Chưa có URL"}</div>
    </div>
  );
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <label className="block space-y-1.5">
      <span className="text-[13px] font-medium text-[#555]">{label}</span>
      {children}
    </label>
  );
}
