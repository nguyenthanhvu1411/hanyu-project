"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import {
  ArrowDown,
  ArrowUp,
  FileText,
  Headphones,
  ImageIcon,
  Pencil,
  Plus,
  RefreshCw,
  Save,
  Trash2,
  X,
} from "lucide-react";
import { toast } from "sonner";

import { Alert } from "@/components/ui/alert";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";

import { lessonApi } from "../api/lesson.api";
import { lessonSectionAssetsApi } from "../api/lesson-section-assets.api";
import {
  lessonAssetTypeLabels,
  lessonSectionTypeLabels,
  type AdminLessonAsset,
  type AdminLessonSection,
} from "../types/lesson.types";
import type { AdminLessonSectionAsset } from "../types/lesson-section-asset.types";

interface EditState {
  id: number;
  captionVi: string;
  isRequired: boolean;
}

export function LessonSectionAssetsPanel({ lessonId }: { lessonId: number }) {
  const [sections, setSections] = useState<AdminLessonSection[]>([]);
  const [assets, setAssets] = useState<AdminLessonAsset[]>([]);
  const [links, setLinks] = useState<AdminLessonSectionAsset[]>([]);
  const [sectionId, setSectionId] = useState("");
  const [assetId, setAssetId] = useState("");
  const [captionVi, setCaptionVi] = useState("");
  const [sortOrder, setSortOrder] = useState(0);
  const [required, setRequired] = useState(false);
  const [busy, setBusy] = useState(false);
  const [deleting, setDeleting] = useState<AdminLessonSectionAsset | null>(null);
  const [editing, setEditing] = useState<EditState | null>(null);

  const selectedSectionId = Number(sectionId) || 0;
  const orderedLinks = useMemo(
    () => [...links].sort((a, b) => a.sortOrder - b.sortOrder || a.id - b.id),
    [links],
  );

  const loadBase = useCallback(async () => {
    try {
      const [sectionData, assetData] = await Promise.all([
        lessonApi.listSections(lessonId),
        lessonApi.listAssets(lessonId),
      ]);
      const ordered = [...sectionData].sort((a, b) => a.sortOrder - b.sortOrder || a.id - b.id);
      setSections(ordered);
      setAssets(assetData);
      setSectionId((current) => current || (ordered[0] ? String(ordered[0].id) : ""));
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể tải section và media.");
    }
  }, [lessonId]);

  const loadLinks = useCallback(async () => {
    if (!selectedSectionId) {
      setLinks([]);
      return;
    }
    try {
      const data = await lessonSectionAssetsApi.list(lessonId, selectedSectionId);
      setLinks(data);
      setSortOrder(data.length);
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể tải media của section.");
    }
  }, [lessonId, selectedSectionId]);

  useEffect(() => { void loadBase(); }, [loadBase]);
  useEffect(() => {
    setEditing(null);
    void loadLinks();
  }, [loadLinks]);

  const linkedAssetIds = useMemo(() => new Set(links.map((item) => item.lessonAssetId)), [links]);
  const availableAssets = useMemo(() => assets.filter((item) => !linkedAssetIds.has(item.id)), [assets, linkedAssetIds]);

  async function attach() {
    const nextAssetId = Number(assetId);
    if (!selectedSectionId || !nextAssetId) {
      toast.error("Hãy chọn section và tài nguyên cần gắn.");
      return;
    }

    setBusy(true);
    try {
      await lessonSectionAssetsApi.attach(lessonId, selectedSectionId, {
        lessonAssetId: nextAssetId,
        sortOrder: Math.max(0, sortOrder),
        captionVi: captionVi.trim() || null,
        isRequired: required,
      });
      setAssetId("");
      setCaptionVi("");
      setRequired(false);
      await loadLinks();
      toast.success("Đã gắn media vào section.");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể gắn media vào section.");
    } finally {
      setBusy(false);
    }
  }

  async function saveEdit(item: AdminLessonSectionAsset) {
    if (!editing || editing.id !== item.id) return;
    setBusy(true);
    try {
      await lessonSectionAssetsApi.update(lessonId, selectedSectionId, item.id, {
        sortOrder: item.sortOrder,
        captionVi: editing.captionVi.trim() || null,
        isRequired: editing.isRequired,
      });
      setEditing(null);
      await loadLinks();
      toast.success("Đã cập nhật media của section.");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể cập nhật media.");
    } finally {
      setBusy(false);
    }
  }

  async function move(index: number, direction: -1 | 1) {
    const targetIndex = index + direction;
    if (targetIndex < 0 || targetIndex >= orderedLinks.length) return;

    const current = orderedLinks[index];
    const target = orderedLinks[targetIndex];
    const temporaryOrder = Math.max(...orderedLinks.map((item) => item.sortOrder), 0) + 100;

    setBusy(true);
    try {
      await lessonSectionAssetsApi.update(lessonId, selectedSectionId, current.id, {
        sortOrder: temporaryOrder,
        captionVi: current.captionVi ?? null,
        isRequired: current.isRequired,
      });
      await lessonSectionAssetsApi.update(lessonId, selectedSectionId, target.id, {
        sortOrder: current.sortOrder,
        captionVi: target.captionVi ?? null,
        isRequired: target.isRequired,
      });
      await lessonSectionAssetsApi.update(lessonId, selectedSectionId, current.id, {
        sortOrder: target.sortOrder,
        captionVi: current.captionVi ?? null,
        isRequired: current.isRequired,
      });
      await loadLinks();
      toast.success("Đã cập nhật thứ tự media.");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể đổi thứ tự media.");
    } finally {
      setBusy(false);
    }
  }

  async function remove() {
    if (!deleting || !selectedSectionId) return;
    setBusy(true);
    try {
      await lessonSectionAssetsApi.remove(lessonId, selectedSectionId, deleting.id);
      setDeleting(null);
      await loadLinks();
      toast.success("Đã gỡ media khỏi section.");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể gỡ media khỏi section.");
    } finally {
      setBusy(false);
    }
  }

  return (
    <Card>
      <CardHeader className="flex flex-row items-start justify-between gap-3">
        <div>
          <CardTitle>Media theo section</CardTitle>
          <p className="mt-1 text-[13px] leading-5 text-[#777]">
            Gắn, chỉnh caption, đánh dấu bắt buộc, sắp xếp và xem trước Image / Audio / Document ngay trong từng section.
          </p>
        </div>
        <Button type="button" variant="outline" size="sm" className="gap-2" onClick={() => void Promise.all([loadBase(), loadLinks()])} disabled={busy}>
          <RefreshCw size={14} /> Làm mới
        </Button>
      </CardHeader>

      <CardContent className="space-y-4">
        {sections.length === 0 ? (
          <Alert variant="info">Hãy tạo ít nhất một LessonSection trước khi gắn media.</Alert>
        ) : (
          <>
            <div className="grid gap-3 lg:grid-cols-2">
              <Field label="Section">
                <Select
                  value={sectionId}
                  onValueChange={setSectionId}
                  options={sections.map((item, index) => ({
                    value: String(item.id),
                    label: `${index + 1}. ${item.titleVi || lessonSectionTypeLabels[item.sectionType]}`,
                    description: lessonSectionTypeLabels[item.sectionType],
                  }))}
                />
              </Field>
              <Field label="Tài nguyên chưa gắn">
                <Select
                  value={assetId}
                  onValueChange={setAssetId}
                  options={availableAssets.map((item) => ({
                    value: String(item.id),
                    label: item.captionVi || `${lessonAssetTypeLabels[item.assetType]} #${item.id}`,
                    description: item.url || (item.audioAssetId ? `AudioAsset #${item.audioAssetId}` : "Không có URL"),
                  }))}
                  placeholder={availableAssets.length ? "Chọn tài nguyên" : "Không còn tài nguyên để gắn"}
                />
              </Field>
              <Field label="Caption riêng cho section">
                <Input value={captionVi} onChange={(event) => setCaptionVi(event.target.value)} placeholder="Tùy chọn" />
              </Field>
              <Field label="Thứ tự media">
                <Input type="number" min={0} value={sortOrder} onChange={(event) => setSortOrder(Number(event.target.value) || 0)} />
              </Field>
            </div>

            <div className="flex flex-wrap items-center justify-between gap-3 rounded-[9px] border border-[#e7e2db] bg-[#faf9f7] px-3 py-2.5">
              <div>
                <div className="text-[13px] font-medium text-[#444]">Media bắt buộc</div>
                <div className="text-[12px] text-[#888]">Dùng cho rule hoàn thành section ở bước Learning sau.</div>
              </div>
              <div className="flex items-center gap-3">
                <Switch checked={required} onCheckedChange={setRequired} />
                <Button type="button" size="sm" className="gap-2" onClick={() => void attach()} loading={busy} disabled={!assetId}>
                  <Plus size={14} /> Gắn media
                </Button>
              </div>
            </div>

            <div className="space-y-3">
              {orderedLinks.length === 0 ? (
                <Alert variant="default">Section này chưa được gắn media.</Alert>
              ) : orderedLinks.map((item, index) => {
                const isEditing = editing?.id === item.id;
                return (
                  <div key={item.id} className="rounded-[10px] border border-[#e7e2db] bg-white p-3">
                    <div className="flex flex-col gap-3 lg:flex-row lg:items-start lg:justify-between">
                      <div className="min-w-0 flex-1">
                        {isEditing ? (
                          <div className="grid gap-3 md:grid-cols-[minmax(0,1fr)_auto] md:items-end">
                            <Field label="Caption">
                              <Input
                                value={editing.captionVi}
                                onChange={(event) => setEditing({ ...editing, captionVi: event.target.value })}
                                placeholder={item.assetCaptionVi || "Caption media"}
                              />
                            </Field>
                            <div className="flex h-10 items-center gap-2 rounded-[8px] border border-[#e7e2db] px-3">
                              <Switch
                                checked={editing.isRequired}
                                onCheckedChange={(value) => setEditing({ ...editing, isRequired: value })}
                              />
                              <span className="text-[13px] text-[#555]">Bắt buộc</span>
                            </div>
                          </div>
                        ) : (
                          <div className="flex flex-wrap items-center gap-2">
                            <span className="text-[14px] font-medium text-[#333]">
                              {index + 1}. {item.captionVi || item.assetCaptionVi || `${item.assetType} #${item.lessonAssetId}`}
                            </span>
                            <Badge>{item.assetType}</Badge>
                            {item.isRequired && <Badge variant="warning">Bắt buộc</Badge>}
                            <Badge variant="info">#{item.sortOrder}</Badge>
                          </div>
                        )}

                        <div className="mt-3">
                          <MediaPreview item={item} />
                        </div>
                      </div>

                      <div className="flex shrink-0 flex-wrap gap-1.5">
                        <Button type="button" variant="outline" size="icon" aria-label="Đưa lên" title="Đưa lên" onClick={() => void move(index, -1)} disabled={busy || index === 0}>
                          <ArrowUp size={14} />
                        </Button>
                        <Button type="button" variant="outline" size="icon" aria-label="Đưa xuống" title="Đưa xuống" onClick={() => void move(index, 1)} disabled={busy || index === orderedLinks.length - 1}>
                          <ArrowDown size={14} />
                        </Button>
                        {isEditing ? (
                          <>
                            <Button type="button" variant="secondary" size="icon" aria-label="Lưu" title="Lưu" onClick={() => void saveEdit(item)} disabled={busy}>
                              <Save size={14} />
                            </Button>
                            <Button type="button" variant="ghost" size="icon" aria-label="Hủy" title="Hủy" onClick={() => setEditing(null)} disabled={busy}>
                              <X size={14} />
                            </Button>
                          </>
                        ) : (
                          <Button
                            type="button"
                            variant="outline"
                            size="icon"
                            aria-label="Chỉnh sửa"
                            title="Chỉnh sửa"
                            onClick={() => setEditing({ id: item.id, captionVi: item.captionVi ?? "", isRequired: item.isRequired })}
                            disabled={busy}
                          >
                            <Pencil size={14} />
                          </Button>
                        )}
                        <Button type="button" variant="dangerGhost" size="icon" aria-label="Gỡ media" title="Gỡ media" onClick={() => setDeleting(item)} disabled={busy}>
                          <Trash2 size={14} />
                        </Button>
                      </div>
                    </div>
                  </div>
                );
              })}
            </div>
          </>
        )}
      </CardContent>

      <ConfirmDialog
        open={Boolean(deleting)}
        title="Gỡ media khỏi section?"
        description="Tài nguyên gốc vẫn còn trong Lesson; chỉ liên kết với section hiện tại bị xóa."
        confirmLabel="Gỡ media"
        loading={busy}
        onClose={() => setDeleting(null)}
        onConfirm={remove}
      />
    </Card>
  );
}

function MediaPreview({ item }: { item: AdminLessonSectionAsset }) {
  const type = item.assetType.toLowerCase();
  const caption = item.captionVi || item.assetCaptionVi || "Tài nguyên";

  if (type.includes("image")) {
    return item.url ? (
      <figure className="overflow-hidden rounded-[9px] border border-[#ece7e0] bg-[#faf9f7]">
        {/* eslint-disable-next-line @next/next/no-img-element */}
        <img src={item.url} alt={caption} className="max-h-[320px] w-full object-contain" />
        <figcaption className="flex items-center gap-2 border-t border-[#ece7e0] px-3 py-2 text-[12px] text-[#777]">
          <ImageIcon size={13} /> {caption}
        </figcaption>
      </figure>
    ) : <Alert variant="default">Image chưa có URL public để preview.</Alert>;
  }

  if (type.includes("audio")) {
    return item.url ? (
      <div className="rounded-[9px] border border-[#ece7e0] bg-[#faf9f7] p-3">
        <div className="mb-2 flex items-center gap-2 text-[13px] font-medium text-[#555]"><Headphones size={14} /> {caption}</div>
        <audio controls preload="metadata" className="w-full" src={item.url}>Trình duyệt không hỗ trợ audio.</audio>
      </div>
    ) : (
      <Alert variant="default">AudioAsset #{item.audioAssetId ?? "?"} chưa trả public URL cho admin preview.</Alert>
    );
  }

  return (
    <div className="flex items-center justify-between gap-3 rounded-[9px] border border-[#ece7e0] bg-[#faf9f7] px-3 py-3">
      <div className="flex min-w-0 items-center gap-2 text-[13px] text-[#555]"><FileText size={15} /><span className="truncate">{caption}</span></div>
      {item.url ? <a href={item.url} target="_blank" rel="noreferrer" className="text-[13px] font-medium text-[#ef241c] hover:underline">Mở tài liệu</a> : <span className="text-[12px] text-[#999]">Chưa có URL</span>}
    </div>
  );
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return <label className="block space-y-1.5"><span className="text-[13px] font-medium text-[#555]">{label}</span>{children}</label>;
}
