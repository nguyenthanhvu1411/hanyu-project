"use client";

import { ArrowDown, ArrowUp, FileText, Headphones, ImageIcon, Pencil, Plus, RefreshCw, Trash2 } from "lucide-react";
import { useCallback, useEffect, useMemo, useState } from "react";
import { toast } from "sonner";

import { Alert } from "@/components/ui/alert";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";
import { Skeleton } from "@/components/ui/skeleton";

import { lessonSectionAssetsApi } from "../api/lesson-section-assets.api";
import { lessonApi } from "../api/lesson.api";
import type { AdminLessonSectionAsset, AttachLessonSectionAssetRequest, UpdateLessonSectionAssetRequest } from "../types/lesson-section-asset.types";
import { LessonAssetType, lessonAssetTypeLabels, lessonSectionTypeLabels, type AdminLessonAsset, type AdminLessonSection, type CreateLessonAssetRequest, type CreateLessonSectionRequest } from "../types/lesson.types";
import { LessonAssetModal } from "./lesson-media-asset-modal";
import { LessonSectionMediaModal } from "./lesson-section-media-modal";
import { LessonSectionModal } from "./lesson-section-modal";

export function LessonSectionStudio({ lessonId }: { lessonId: number }) {
  const [sections, setSections] = useState<AdminLessonSection[]>([]);
  const [assets, setAssets] = useState<AdminLessonAsset[]>([]);
  const [mediaBySection, setMediaBySection] = useState<Record<number, AdminLessonSectionAsset[]>>({});
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [editingSection, setEditingSection] = useState<AdminLessonSection | null>(null);
  const [sectionModalOpen, setSectionModalOpen] = useState(false);
  const [deletingSection, setDeletingSection] = useState<AdminLessonSection | null>(null);
  const [mediaSection, setMediaSection] = useState<AdminLessonSection | null>(null);
  const [editingMedia, setEditingMedia] = useState<AdminLessonSectionAsset | null>(null);
  const [editingAsset, setEditingAsset] = useState<AdminLessonAsset | null>(null);
  const [deletingAsset, setDeletingAsset] = useState<AdminLessonAsset | null>(null);
  const [assetModalOpen, setAssetModalOpen] = useState(false);

  const orderedSections = useMemo(() => [...sections].sort((a, b) => a.sortOrder - b.sortOrder || a.id - b.id), [sections]);
  const orderedAssets = useMemo(() => [...assets].sort((a, b) => a.sortOrder - b.sortOrder || a.id - b.id), [assets]);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [nextSections, nextAssets] = await Promise.all([lessonApi.listSections(lessonId), lessonApi.listAssets(lessonId)]);
      const entries = await Promise.all(nextSections.map(async (section) => [section.id, await lessonSectionAssetsApi.list(lessonId, section.id)] as const));
      setSections(nextSections);
      setAssets(nextAssets);
      setMediaBySection(Object.fromEntries(entries));
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể tải Lesson Content Editor.");
    } finally {
      setLoading(false);
    }
  }, [lessonId]);

  useEffect(() => { void load(); }, [load]);

  async function saveSection(request: CreateLessonSectionRequest) {
    setBusy(true);
    try {
      if (editingSection) await lessonApi.updateSection(lessonId, editingSection.id, request);
      else await lessonApi.createSection(lessonId, request);
      toast.success(editingSection ? "Đã cập nhật section." : "Đã thêm section.");
      setEditingSection(null);
      setSectionModalOpen(false);
      await load();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể lưu section.");
    } finally { setBusy(false); }
  }

  async function moveSection(index: number, direction: -1 | 1) {
    const targetIndex = index + direction;
    if (targetIndex < 0 || targetIndex >= orderedSections.length) return;
    const current = orderedSections[index];
    const target = orderedSections[targetIndex];
    const temporary = Math.max(...orderedSections.map((item) => item.sortOrder), 0) + 100;
    setBusy(true);
    try {
      await lessonApi.updateSection(lessonId, current.id, sectionRequest(current, temporary));
      await lessonApi.updateSection(lessonId, target.id, sectionRequest(target, current.sortOrder));
      await lessonApi.updateSection(lessonId, current.id, sectionRequest(current, target.sortOrder));
      await load();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể đổi thứ tự section.");
    } finally { setBusy(false); }
  }

  async function removeSection() {
    if (!deletingSection) return;
    setBusy(true);
    try {
      await lessonApi.deleteSection(lessonId, deletingSection.id);
      setDeletingSection(null);
      await load();
      toast.success("Đã xóa section.");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể xóa section.");
    } finally { setBusy(false); }
  }

  async function attachMedia(request: AttachLessonSectionAssetRequest) {
    if (!mediaSection) return;
    setBusy(true);
    try {
      await lessonSectionAssetsApi.attach(lessonId, mediaSection.id, request);
      setMediaSection(null);
      setEditingMedia(null);
      await load();
      toast.success("Đã gắn media vào section.");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể gắn media.");
    } finally { setBusy(false); }
  }

  async function updateMedia(linkId: number, request: UpdateLessonSectionAssetRequest) {
    if (!mediaSection) return;
    setBusy(true);
    try {
      await lessonSectionAssetsApi.update(lessonId, mediaSection.id, linkId, request);
      setMediaSection(null);
      setEditingMedia(null);
      await load();
      toast.success("Đã cập nhật media.");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể cập nhật media.");
    } finally { setBusy(false); }
  }

  async function saveAsset(request: CreateLessonAssetRequest) {
    setBusy(true);
    try {
      if (editingAsset) {
        await lessonApi.updateAsset(lessonId, editingAsset.id, {
          url: request.url,
          captionVi: request.captionVi,
          audioAssetId: request.audioAssetId,
          sortOrder: request.sortOrder,
        });
      } else {
        await lessonApi.createAsset(lessonId, request);
      }
      toast.success(editingAsset ? "Đã cập nhật tài nguyên." : "Đã thêm tài nguyên.");
      setEditingAsset(null);
      setAssetModalOpen(false);
      await load();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể lưu tài nguyên.");
    } finally { setBusy(false); }
  }

  async function removeAsset() {
    if (!deletingAsset) return;
    setBusy(true);
    try {
      await lessonApi.deleteAsset(lessonId, deletingAsset.id);
      setDeletingAsset(null);
      if (editingAsset?.id === deletingAsset.id) {
        setEditingAsset(null);
        setAssetModalOpen(false);
      }
      await load();
      toast.success("Đã xóa tài nguyên khỏi Lesson.");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể xóa tài nguyên. Hãy gỡ các liên kết section nếu backend yêu cầu.");
    } finally { setBusy(false); }
  }

  if (loading) {
    return <div className="grid gap-4 xl:grid-cols-[minmax(0,1fr)_360px]"><Skeleton className="h-[620px] rounded-[12px]" /><Skeleton className="h-[620px] rounded-[12px]" /></div>;
  }

  return (
    <>
      <div className="grid gap-4 xl:grid-cols-[minmax(0,1fr)_360px]">
        <Card>
          <CardHeader className="flex flex-row items-start justify-between gap-3">
            <div>
              <CardTitle>Cấu trúc bài học ({orderedSections.length})</CardTitle>
              <p className="mt-1 text-[13px] leading-5 text-[#777]">Thêm/sửa section và media bằng modal, không kéo dài form trên trang.</p>
            </div>
            <div className="flex flex-wrap gap-2">
              <Button type="button" variant="outline" size="sm" disabled={busy} onClick={() => void load()} className="gap-2"><RefreshCw size={14} /> Làm mới</Button>
              <Button type="button" size="sm" onClick={() => { setEditingSection(null); setSectionModalOpen(true); }} className="gap-2"><Plus size={14} /> Thêm section</Button>
            </div>
          </CardHeader>
          <CardContent>
            {orderedSections.length === 0 ? <Alert variant="info">Chưa có section.</Alert> : (
              <div className="space-y-3">
                {orderedSections.map((section, index) => {
                  const media = [...(mediaBySection[section.id] ?? [])].sort((a, b) => a.sortOrder - b.sortOrder || a.id - b.id);
                  return (
                    <div key={section.id} className="rounded-[10px] border border-[#e8e3dc] bg-white p-4">
                      <div className="flex flex-col gap-3 lg:flex-row lg:items-start lg:justify-between">
                        <div className="min-w-0 flex-1">
                          <div className="flex flex-wrap items-center gap-2">
                            <span className="text-[14px] font-semibold text-[#333]">{index + 1}. {section.titleVi || lessonSectionTypeLabels[section.sectionType]}</span>
                            <Badge>{lessonSectionTypeLabels[section.sectionType]}</Badge>
                            {section.isRequired && <Badge variant="warning">Bắt buộc</Badge>}
                            <Badge variant="info">{media.length} media</Badge>
                          </div>
                          <div className="mt-1 text-[12px] text-[#888]">Thứ tự {section.sortOrder} · {section.estimatedSeconds ?? 0} giây</div>
                          {section.contentVi && <p className="mt-2 line-clamp-3 whitespace-pre-wrap text-[13px] leading-5 text-[#666]">{section.contentVi}</p>}
                        </div>
                        <div className="flex shrink-0 flex-wrap gap-1.5">
                          <Button type="button" variant="ghost" size="icon" disabled={busy || index === 0} onClick={() => void moveSection(index, -1)} aria-label="Đưa section lên"><ArrowUp size={14} /></Button>
                          <Button type="button" variant="ghost" size="icon" disabled={busy || index === orderedSections.length - 1} onClick={() => void moveSection(index, 1)} aria-label="Đưa section xuống"><ArrowDown size={14} /></Button>
                          <Button type="button" variant="outline" size="sm" onClick={() => { setEditingSection(section); setSectionModalOpen(true); }} className="gap-1.5"><Pencil size={13} /> Sửa</Button>
                          <Button type="button" variant="outline" size="sm" onClick={() => { setMediaSection(section); setEditingMedia(null); }} className="gap-1.5"><Plus size={13} /> Media</Button>
                          <Button type="button" variant="dangerGhost" size="sm" onClick={() => setDeletingSection(section)} className="gap-1.5"><Trash2 size={13} /> Xóa</Button>
                        </div>
                      </div>

                      {media.length > 0 && <div className="mt-4 grid gap-2 border-t border-[#eee9e2] pt-3 md:grid-cols-2">{media.map((item) => (
                        <button key={item.id} type="button" className="rounded-[9px] border border-[#e8e3dc] bg-[#fcfbf9] p-3 text-left" onClick={() => { setMediaSection(section); setEditingMedia(item); }}>
                          <MediaPreview item={item} />
                        </button>
                      ))}</div>}
                    </div>
                  );
                })}
              </div>
            )}
          </CardContent>
        </Card>

        <aside className="xl:sticky xl:top-4 xl:self-start">
          <Card>
            <CardHeader className="flex flex-row items-start justify-between gap-3">
              <div><CardTitle>Kho media Lesson ({orderedAssets.length})</CardTitle><p className="mt-1 text-[13px] leading-5 text-[#777]">Upload, chỉnh sửa hoặc xóa tài nguyên; sau đó gắn vào section bằng modal.</p></div>
              <Button type="button" size="icon" onClick={() => { setEditingAsset(null); setAssetModalOpen(true); }} aria-label="Thêm tài nguyên"><Plus size={14} /></Button>
            </CardHeader>
            <CardContent>
              {orderedAssets.length === 0 ? <Alert variant="info">Lesson chưa có tài nguyên.</Alert> : <div className="space-y-2">{orderedAssets.map((asset) => (
                <div key={asset.id} className="flex items-start gap-1 rounded-[9px] border border-[#e8e3dc] p-2">
                  <button type="button" className="min-w-0 flex-1 rounded-[7px] p-1 text-left" onClick={() => { setEditingAsset(asset); setAssetModalOpen(true); }}>
                    <AssetPreview asset={asset} />
                  </button>
                  <Button type="button" variant="dangerGhost" size="icon" disabled={busy} aria-label="Xóa tài nguyên" title="Xóa tài nguyên" onClick={() => setDeletingAsset(asset)}>
                    <Trash2 size={14} />
                  </Button>
                </div>
              ))}</div>}
            </CardContent>
          </Card>
        </aside>
      </div>

      <LessonSectionModal open={sectionModalOpen} section={editingSection} defaultSortOrder={orderedSections.length} loading={busy} onClose={() => { setSectionModalOpen(false); setEditingSection(null); }} onSubmit={saveSection} />
      <LessonSectionMediaModal open={Boolean(mediaSection)} section={mediaSection} assets={assets} existingLinks={mediaSection ? mediaBySection[mediaSection.id] ?? [] : []} editing={editingMedia} loading={busy} onClose={() => { setMediaSection(null); setEditingMedia(null); }} onAttach={attachMedia} onUpdate={updateMedia} />
      <LessonAssetModal open={assetModalOpen} asset={editingAsset} defaultSortOrder={orderedAssets.length} loading={busy} onClose={() => { setAssetModalOpen(false); setEditingAsset(null); }} onSubmit={saveAsset} />
      <ConfirmDialog open={Boolean(deletingSection)} title="Xóa section?" description={`Section “${deletingSection?.titleVi || (deletingSection ? lessonSectionTypeLabels[deletingSection.sectionType] : "") }” sẽ bị xóa khỏi bài giảng.`} confirmLabel="Xóa section" loading={busy} onClose={() => setDeletingSection(null)} onConfirm={removeSection} />
      <ConfirmDialog open={Boolean(deletingAsset)} title="Xóa tài nguyên Lesson?" description={`Tài nguyên “${deletingAsset?.captionVi || (deletingAsset ? lessonAssetTypeLabels[deletingAsset.assetType] : "") }” sẽ bị xóa khỏi Lesson. Nếu đang được section sử dụng, backend có thể yêu cầu gỡ liên kết trước.`} confirmLabel="Xóa tài nguyên" loading={busy} onClose={() => setDeletingAsset(null)} onConfirm={removeAsset} />
    </>
  );
}

function MediaPreview({ item }: { item: AdminLessonSectionAsset }) {
  const type = item.assetType.toLowerCase();
  const caption = item.captionVi || item.assetCaptionVi || "Tài nguyên";
  if (type.includes("image") && item.url) return <div>{/* eslint-disable-next-line @next/next/no-img-element */}<img src={item.url} alt={caption} className="max-h-[150px] w-full rounded-[7px] object-contain" /><div className="mt-2 text-[12px] text-[#777]">{caption}</div></div>;
  return <div><div className="flex items-center gap-2 text-[13px] font-medium text-[#444]">{type.includes("audio") ? <Headphones size={14} /> : <FileText size={14} />}{caption}</div>{item.isRequired && <Badge variant="warning" className="mt-2">Bắt buộc</Badge>}</div>;
}

function AssetPreview({ asset }: { asset: AdminLessonAsset }) {
  const caption = asset.captionVi || `${lessonAssetTypeLabels[asset.assetType]} #${asset.id}`;
  return <div><div className="flex items-center gap-2 text-[13px] font-medium text-[#444]">{asset.assetType === LessonAssetType.Audio ? <Headphones size={14} /> : asset.assetType === LessonAssetType.Document ? <FileText size={14} /> : <ImageIcon size={14} />}{caption}</div><div className="mt-1 break-all text-[12px] text-[#888]">{asset.url || (asset.audioAssetId ? `AudioAsset #${asset.audioAssetId}` : "Chưa có nguồn media")}</div></div>;
}

function sectionRequest(item: AdminLessonSection, sortOrder: number): CreateLessonSectionRequest {
  return {
    sectionType: item.sectionType,
    titleVi: item.titleVi ?? null,
    contentVi: item.contentVi ?? null,
    sortOrder,
    isRequired: item.isRequired,
    estimatedSeconds: item.estimatedSeconds ?? null,
  };
}
