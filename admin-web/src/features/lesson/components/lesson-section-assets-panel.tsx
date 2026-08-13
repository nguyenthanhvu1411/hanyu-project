"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { ImageIcon, Link2, Plus, RefreshCw, Trash2 } from "lucide-react";
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
import { lessonAssetTypeLabels, lessonSectionTypeLabels, type AdminLessonAsset, type AdminLessonSection } from "../types/lesson.types";
import type { AdminLessonSectionAsset } from "../types/lesson-section-asset.types";

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

  const selectedSectionId = Number(sectionId) || 0;

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
  useEffect(() => { void loadLinks(); }, [loadLinks]);

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
            Gắn hình ảnh, audio hoặc tài liệu của Lesson vào từng section bằng quan hệ dữ liệu thật, không cần nhúng URL thủ công.
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

            <div className="space-y-2">
              {links.length === 0 ? (
                <Alert variant="default">Section này chưa được gắn media.</Alert>
              ) : links.map((item, index) => (
                <div key={item.id} className="flex flex-col gap-3 rounded-[9px] border border-[#e7e2db] p-3 md:flex-row md:items-center md:justify-between">
                  <div className="min-w-0">
                    <div className="flex flex-wrap items-center gap-2">
                      <span className="text-[14px] font-medium text-[#333]">{index + 1}. {item.captionVi || item.assetCaptionVi || `${item.assetType} #${item.lessonAssetId}`}</span>
                      <Badge>{item.assetType}</Badge>
                      {item.isRequired && <Badge variant="warning">Bắt buộc</Badge>}
                    </div>
                    <div className="mt-1 flex items-center gap-1.5 text-[12px] text-[#888]">
                      {item.assetType.toLowerCase().includes("image") ? <ImageIcon size={13} /> : <Link2 size={13} />}
                      <span className="truncate">{item.url || (item.audioAssetId ? `AudioAsset #${item.audioAssetId}` : "Không có URL")}</span>
                    </div>
                  </div>
                  <Button type="button" variant="dangerGhost" size="sm" className="gap-1.5" onClick={() => setDeleting(item)} disabled={busy}>
                    <Trash2 size={13} /> Gỡ
                  </Button>
                </div>
              ))}
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

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return <label className="block space-y-1.5"><span className="text-[13px] font-medium text-[#555]">{label}</span>{children}</label>;
}
