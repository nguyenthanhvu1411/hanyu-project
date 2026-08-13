"use client";

import { FileText, Headphones, ImageIcon, Save } from "lucide-react";
import { useEffect, useMemo, useState } from "react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Modal } from "@/components/ui/modal";
import { Select } from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";

import type {
  AdminLessonSectionAsset,
  AttachLessonSectionAssetRequest,
  UpdateLessonSectionAssetRequest,
} from "../types/lesson-section-asset.types";
import {
  LessonAssetType,
  lessonAssetTypeLabels,
  type AdminLessonAsset,
  type AdminLessonSection,
} from "../types/lesson.types";

interface LessonSectionMediaModalProps {
  open: boolean;
  section: AdminLessonSection | null;
  assets: AdminLessonAsset[];
  existingLinks: AdminLessonSectionAsset[];
  editing?: AdminLessonSectionAsset | null;
  loading?: boolean;
  onClose: () => void;
  onAttach: (request: AttachLessonSectionAssetRequest) => void | Promise<void>;
  onUpdate: (linkId: number, request: UpdateLessonSectionAssetRequest) => void | Promise<void>;
}

export function LessonSectionMediaModal({
  open,
  section,
  assets,
  existingLinks,
  editing,
  loading = false,
  onClose,
  onAttach,
  onUpdate,
}: LessonSectionMediaModalProps) {
  const linkedIds = useMemo(() => new Set(existingLinks.map((item) => item.lessonAssetId)), [existingLinks]);
  const availableAssets = useMemo(
    () => assets.filter((asset) => editing?.lessonAssetId === asset.id || !linkedIds.has(asset.id)),
    [assets, editing?.lessonAssetId, linkedIds],
  );

  const [assetId, setAssetId] = useState("");
  const [captionVi, setCaptionVi] = useState("");
  const [sortOrder, setSortOrder] = useState(0);
  const [isRequired, setIsRequired] = useState(false);

  useEffect(() => {
    if (!open) return;
    setAssetId(editing ? String(editing.lessonAssetId) : "");
    setCaptionVi(editing?.captionVi ?? "");
    setSortOrder(editing?.sortOrder ?? existingLinks.length);
    setIsRequired(editing?.isRequired ?? false);
  }, [open, editing, existingLinks.length]);

  const selectedAsset = useMemo(
    () => assets.find((asset) => asset.id === Number(assetId)) ?? null,
    [assets, assetId],
  );

  async function submit() {
    if (!section) return;

    if (editing) {
      await onUpdate(editing.id, {
        sortOrder: Math.max(0, sortOrder),
        captionVi: captionVi.trim() || null,
        isRequired,
      });
      return;
    }

    const lessonAssetId = Number(assetId);
    if (!lessonAssetId) return;

    await onAttach({
      lessonAssetId,
      sortOrder: Math.max(0, sortOrder),
      captionVi: captionVi.trim() || null,
      isRequired,
    });
  }

  return (
    <Modal
      open={open}
      onClose={onClose}
      size="md"
      title={editing ? "Chỉnh sửa media của section" : "Gắn media vào section"}
      description={section ? `Section: ${section.titleVi || `#${section.id}`}` : undefined}
      footer={
        <div className="flex justify-end gap-2">
          <Button type="button" variant="outline" size="md" disabled={loading} onClick={onClose}>Hủy</Button>
          <Button
            type="button"
            size="md"
            loading={loading}
            disabled={!editing && !assetId}
            onClick={() => void submit()}
            className="gap-2"
          >
            <Save size={14} /> {editing ? "Lưu thay đổi" : "Gắn media"}
          </Button>
        </div>
      }
    >
      <div className="space-y-4">
        <Field label="Tài nguyên Lesson">
          <Select
            value={assetId}
            onValueChange={setAssetId}
            disabled={Boolean(editing)}
            searchable
            options={availableAssets.map((asset) => ({
              value: String(asset.id),
              label: asset.captionVi || `${lessonAssetTypeLabels[asset.assetType]} #${asset.id}`,
              description: asset.url || (asset.audioAssetId ? `AudioAsset #${asset.audioAssetId}` : "Chưa có nguồn media"),
            }))}
            placeholder={availableAssets.length ? "Chọn tài nguyên" : "Không còn tài nguyên để gắn"}
          />
        </Field>

        <div className="grid gap-4 md:grid-cols-2">
          <Field label="Caption riêng cho section">
            <Input value={captionVi} onChange={(event) => setCaptionVi(event.target.value)} placeholder="Tùy chọn" />
          </Field>
          <Field label="Thứ tự media">
            <Input type="number" min={0} value={sortOrder} onChange={(event) => setSortOrder(Number(event.target.value) || 0)} />
          </Field>
        </div>

        <div className="flex items-center justify-between rounded-[9px] border border-[#e7e2db] bg-[#faf9f7] px-3 py-2.5">
          <div>
            <div className="text-[13px] font-medium text-[#444]">Media bắt buộc</div>
            <div className="mt-0.5 text-[12px] text-[#888]">Dùng cho rule hoàn thành section ở giai đoạn Learning.</div>
          </div>
          <Switch checked={isRequired} onCheckedChange={setIsRequired} />
        </div>

        <div>
          <div className="mb-2 text-[13px] font-medium text-[#555]">Preview</div>
          {selectedAsset ? <AssetPreview asset={selectedAsset} caption={captionVi.trim() || selectedAsset.captionVi || undefined} /> : (
            <div className="rounded-[9px] border border-dashed border-[#ddd8d1] bg-[#faf9f7] px-4 py-8 text-center text-[13px] text-[#888]">
              Chọn một tài nguyên để xem trước.
            </div>
          )}
        </div>
      </div>
    </Modal>
  );
}

function AssetPreview({ asset, caption }: { asset: AdminLessonAsset; caption?: string }) {
  if (asset.assetType === LessonAssetType.Image && asset.url) {
    return (
      <figure className="overflow-hidden rounded-[9px] border border-[#e8e3dc] bg-[#faf9f7]">
        {/* eslint-disable-next-line @next/next/no-img-element */}
        <img src={asset.url} alt={caption || "Lesson media"} className="max-h-[320px] w-full object-contain" />
        {caption && <figcaption className="px-3 py-2 text-[12px] text-[#777]">{caption}</figcaption>}
      </figure>
    );
  }

  if (asset.assetType === LessonAssetType.Audio) {
    return (
      <div className="rounded-[9px] border border-[#e8e3dc] bg-[#faf9f7] p-4">
        <div className="mb-3 flex items-center gap-2"><Headphones size={16} /><Badge>Âm thanh</Badge></div>
        {asset.url ? <audio controls className="w-full" src={asset.url} /> : <div className="text-[13px] text-[#888]">AudioAsset #{asset.audioAssetId ?? "-"}</div>}
        {caption && <div className="mt-2 text-[12px] text-[#777]">{caption}</div>}
      </div>
    );
  }

  return (
    <div className="rounded-[9px] border border-[#e8e3dc] bg-[#faf9f7] p-4">
      <div className="flex items-center gap-2">
        {asset.assetType === LessonAssetType.Document ? <FileText size={16} /> : <ImageIcon size={16} />}
        <Badge>{lessonAssetTypeLabels[asset.assetType]}</Badge>
      </div>
      <div className="mt-2 break-all text-[12px] text-[#777]">{asset.url || "Chưa có URL"}</div>
      {caption && <div className="mt-2 text-[12px] text-[#777]">{caption}</div>}
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
