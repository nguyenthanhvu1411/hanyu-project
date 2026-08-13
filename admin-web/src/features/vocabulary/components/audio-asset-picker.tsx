"use client";

import { UploadCloud, Volume2 } from "lucide-react";
import { useCallback, useEffect, useMemo, useState } from "react";

import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Select } from "@/components/ui/select";
import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";
import { getContentStatusLabel } from "@/lib/constants/content-status";

export type AudioAssetKind = 0 | 1;

interface AudioAssetDto {
  id: number;
  storagePath: string;
  publicUrl: string | null;
  kind: number;
  mimeType: string;
  fileSizeBytes: number | null;
  durationMs: number | null;
  voice: string | null;
  provider: string | null;
  languageCode: string | null;
  checksum: string | null;
  status: number;
}

interface UploadAudioResult {
  objectKey: string;
  publicUrl: string;
  originalFileName: string;
  contentType: string;
  fileSizeBytes: number;
}

interface PagedResult<T> {
  items: T[];
}

interface AudioAssetPickerProps {
  value: number | null;
  onChange: (audioAssetId: number | null) => void | Promise<void>;
  kind: AudioAssetKind;
  title?: string;
  description?: string;
  disabled?: boolean;
}

const MAX_AUDIO_BYTES = 50 * 1024 * 1024;

export function AudioAssetPicker({
  value,
  onChange,
  kind,
  title = "Audio",
  description = "Chọn AudioAsset có sẵn hoặc upload file mới lên Backblaze B2.",
  disabled = false,
}: AudioAssetPickerProps) {
  const [assets, setAssets] = useState<AudioAssetDto[]>([]);
  const [selectedId, setSelectedId] = useState(value ? String(value) : "");
  const [uploadFile, setUploadFile] = useState<File | null>(null);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const page = await apiClient<PagedResult<AudioAssetDto>>(
        `${API_ENDPOINTS.VOCABULARY.AUDIO_ASSETS}?page=1&pageSize=100`,
      );
      setAssets(page.items.filter((item) => item.kind === kind));
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Không thể tải danh sách audio.");
    } finally {
      setLoading(false);
    }
  }, [kind]);

  useEffect(() => {
    void load();
  }, [load]);

  useEffect(() => {
    setSelectedId(value ? String(value) : "");
  }, [value]);

  const selectedAsset = useMemo(
    () => assets.find((item) => item.id === value) ?? null,
    [assets, value],
  );

  const options = useMemo(
    () =>
      assets.map((asset) => ({
        value: String(asset.id),
        label: `Audio #${asset.id}`,
        description: `${asset.storagePath} · ${getContentStatusLabel(asset.status)}`,
      })),
    [assets],
  );

  async function applySelection() {
    setBusy(true);
    setError(null);
    try {
      await onChange(selectedId ? Number(selectedId) : null);
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Không thể gắn audio.");
    } finally {
      setBusy(false);
    }
  }

  async function uploadAndSelect() {
    if (!uploadFile) {
      setError("Hãy chọn file audio trước khi tải lên.");
      return;
    }

    if (uploadFile.size > MAX_AUDIO_BYTES) {
      setError("File audio không được vượt quá 50 MB.");
      return;
    }

    setBusy(true);
    setError(null);
    try {
      const formData = new FormData();
      formData.append("file", uploadFile);

      const uploaded = await apiClient<UploadAudioResult>(API_ENDPOINTS.ADMIN.UPLOAD_AUDIO, {
        method: "POST",
        body: formData,
      });

      const created = await apiClient<AudioAssetDto>(API_ENDPOINTS.VOCABULARY.AUDIO_ASSETS, {
        method: "POST",
        body: {
          storagePath: uploaded.objectKey,
          kind,
          mimeType: uploaded.contentType,
        },
      });

      await apiClient<AudioAssetDto>(API_ENDPOINTS.VOCABULARY.AUDIO_ASSET(created.id), {
        method: "PUT",
        body: {
          storagePath: uploaded.objectKey,
          mimeType: uploaded.contentType,
          fileSizeBytes: uploaded.fileSizeBytes,
          durationMs: null,
          checksum: null,
          voice: null,
          provider: "admin-upload",
          languageCode: "zh-CN",
          publicUrl: uploaded.publicUrl,
        },
      });

      setUploadFile(null);
      await load();
      setSelectedId(String(created.id));
      await onChange(created.id);
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Không thể upload audio.");
    } finally {
      setBusy(false);
    }
  }

  async function publishSelected() {
    if (!selectedAsset) return;
    setBusy(true);
    setError(null);
    try {
      await apiClient(API_ENDPOINTS.VOCABULARY.AUDIO_ASSET_PUBLISH(selectedAsset.id), {
        method: "POST",
      });
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Không thể xuất bản audio.");
    } finally {
      setBusy(false);
    }
  }

  return (
    <Card>
      <CardContent className="space-y-4 p-4">
        <div className="flex items-start gap-3">
          <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-[8px] bg-[#fff0ee] text-[#ef241c]">
            <Volume2 size={18} />
          </div>
          <div>
            <div className="text-[14px] font-semibold text-[#333]">{title}</div>
            <div className="mt-1 text-[13px] leading-5 text-[#777]">{description}</div>
          </div>
        </div>

        {error && (
          <div className="rounded-[8px] border border-[#f0cfcb] bg-[#fff5f4] px-3 py-2 text-[13px] text-[#b9433d]">
            {error}
          </div>
        )}

        <div className="rounded-[8px] border border-dashed border-[#ddd8d1] bg-[#faf9f7] p-4">
          <div className="flex items-center gap-2 text-[13px] font-medium text-[#555]">
            <UploadCloud size={16} className="text-[#ef241c]" /> Upload audio mới
          </div>
          <div className="mt-1 text-[12px] text-[#888]">
            MP3, M4A, WAV, OGG, WEBM, AAC hoặc FLAC · tối đa 50 MB.
          </div>
          <div className="mt-3 flex flex-col gap-3 lg:flex-row lg:items-center">
            <input
              type="file"
              accept="audio/*,.mp3,.m4a,.wav,.ogg,.webm,.aac,.flac"
              disabled={disabled || busy}
              onChange={(event) => setUploadFile(event.target.files?.[0] ?? null)}
              className="min-w-0 flex-1 text-[13px] text-[#666] file:mr-3 file:rounded-[7px] file:border file:border-[#dedede] file:bg-white file:px-3 file:py-2 file:text-[13px] file:font-medium file:text-[#444]"
            />
            <Button type="button" size="md" loading={busy} disabled={disabled || !uploadFile} onClick={() => void uploadAndSelect()}>
              Upload & chọn
            </Button>
          </div>
          {uploadFile && (
            <div className="mt-2 text-[12px] text-[#777]">
              {uploadFile.name} · {(uploadFile.size / 1024 / 1024).toFixed(2)} MB
            </div>
          )}
        </div>

        <div className="grid gap-3 lg:grid-cols-[1fr_auto]">
          <Select
            value={selectedId}
            onValueChange={setSelectedId}
            options={options}
            placeholder={loading ? "Đang tải audio..." : "Chọn AudioAsset"}
            disabled={disabled || loading || busy}
            searchable
            clearable
            searchPlaceholder="Tìm theo ID hoặc storage path..."
          />
          <Button type="button" variant="outline" size="md" disabled={disabled || busy || loading} onClick={() => void applySelection()}>
            Lưu lựa chọn
          </Button>
        </div>

        {selectedAsset ? (
          <div className="rounded-[8px] border border-[#e8e3dc] p-4">
            <div className="flex flex-col gap-3 md:flex-row md:items-start md:justify-between">
              <div className="min-w-0">
                <div className="text-[14px] font-semibold text-[#333]">Audio #{selectedAsset.id}</div>
                <div className="mt-1 break-all text-[12px] text-[#777]">{selectedAsset.storagePath}</div>
                <div className="mt-2 text-[12px] text-[#777]">
                  {selectedAsset.mimeType} · {selectedAsset.fileSizeBytes ? `${(selectedAsset.fileSizeBytes / 1024 / 1024).toFixed(2)} MB` : "chưa có kích thước"} · {getContentStatusLabel(selectedAsset.status)}
                </div>
              </div>
              {selectedAsset.status !== 3 && selectedAsset.status !== 4 && (
                <Button type="button" variant="outline" size="sm" disabled={disabled || busy} onClick={() => void publishSelected()}>
                  Xuất bản Audio
                </Button>
              )}
            </div>
            {selectedAsset.publicUrl && <audio controls src={selectedAsset.publicUrl} className="mt-3 h-10 w-full" />}
          </div>
        ) : (
          <div className="rounded-[8px] border border-dashed border-[#ddd8d1] px-4 py-6 text-center text-[13px] text-[#999]">
            Chưa gắn audio.
          </div>
        )}
      </CardContent>
    </Card>
  );
}
