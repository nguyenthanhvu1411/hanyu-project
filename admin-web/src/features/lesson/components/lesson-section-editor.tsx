"use client";

import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import {
  ArrowDown,
  ArrowUp,
  Bold,
  BookOpenText,
  Eye,
  FileText,
  Heading2,
  Headphones,
  ImageIcon,
  Italic,
  Link2,
  List,
  Pencil,
  Plus,
  Quote,
  RefreshCw,
  Save,
  Trash2,
} from "lucide-react";
import { toast } from "sonner";

import { Alert } from "@/components/ui/alert";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { Switch } from "@/components/ui/switch";
import { Textarea } from "@/components/ui/textarea";
import { cn } from "@/lib/utils/cn";

import { lessonApi } from "../api/lesson.api";
import { lessonSectionAssetsApi } from "../api/lesson-section-assets.api";
import type { AdminLessonSectionAsset } from "../types/lesson-section-asset.types";
import {
  LessonAssetType,
  LessonSectionType,
  lessonAssetTypeLabels,
  lessonSectionTypeLabels,
  type AdminLessonAsset,
  type AdminLessonSection,
  type CreateLessonSectionRequest,
} from "../types/lesson.types";

interface LessonSectionEditorProps {
  lessonId: number;
}

interface DraftState {
  sectionType: LessonSectionType;
  titleVi: string;
  contentVi: string;
  sortOrder: number;
  isRequired: boolean;
  estimatedSeconds: number;
}

const EMPTY_DRAFT: DraftState = {
  sectionType: LessonSectionType.Introduction,
  titleVi: "",
  contentVi: "",
  sortOrder: 0,
  isRequired: true,
  estimatedSeconds: 120,
};

const sectionTypeOptions = Object.entries(lessonSectionTypeLabels).map(([value, label]) => ({
  value,
  label,
}));

export function LessonSectionEditor({ lessonId }: LessonSectionEditorProps) {
  const [sections, setSections] = useState<AdminLessonSection[]>([]);
  const [assets, setAssets] = useState<AdminLessonAsset[]>([]);
  const [sectionMedia, setSectionMedia] = useState<Record<number, AdminLessonSectionAsset[]>>({});
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [deleting, setDeleting] = useState<AdminLessonSection | null>(null);
  const [draft, setDraft] = useState<DraftState>(EMPTY_DRAFT);
  const [previewMode, setPreviewMode] = useState(false);
  const textareaRef = useRef<HTMLTextAreaElement | null>(null);

  const ordered = useMemo(
    () => [...sections].sort((a, b) => a.sortOrder - b.sortOrder || a.id - b.id),
    [sections],
  );

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [sectionData, assetData] = await Promise.all([
        lessonApi.listSections(lessonId),
        lessonApi.listAssets(lessonId),
      ]);

      const mediaEntries = await Promise.all(
        sectionData.map(async (section) => [
          section.id,
          await lessonSectionAssetsApi.list(lessonId, section.id),
        ] as const),
      );

      setSections(sectionData);
      setAssets(assetData);
      setSectionMedia(Object.fromEntries(mediaEntries));
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể tải nội dung bài giảng.");
    } finally {
      setLoading(false);
    }
  }, [lessonId]);

  useEffect(() => {
    void load();
  }, [load]);

  useEffect(() => {
    if (editingId !== null) return;
    setDraft((current) => ({ ...current, sortOrder: ordered.length }));
  }, [editingId, ordered.length]);

  function updateDraft<K extends keyof DraftState>(key: K, value: DraftState[K]) {
    setDraft((current) => ({ ...current, [key]: value }));
  }

  function resetDraft() {
    setEditingId(null);
    setPreviewMode(false);
    setDraft({ ...EMPTY_DRAFT, sortOrder: ordered.length });
  }

  function editSection(item: AdminLessonSection) {
    setEditingId(item.id);
    setDraft({
      sectionType: item.sectionType,
      titleVi: item.titleVi ?? "",
      contentVi: item.contentVi ?? "",
      sortOrder: item.sortOrder,
      isRequired: item.isRequired,
      estimatedSeconds: item.estimatedSeconds ?? 120,
    });
    setPreviewMode(false);
    window.requestAnimationFrame(() => textareaRef.current?.focus());
  }

  function toRequest(): CreateLessonSectionRequest {
    return {
      sectionType: draft.sectionType,
      titleVi: draft.titleVi.trim() || null,
      contentVi: draft.contentVi.trim() || null,
      sortOrder: Math.max(0, draft.sortOrder),
      isRequired: draft.isRequired,
      estimatedSeconds: draft.estimatedSeconds > 0 ? draft.estimatedSeconds : null,
    };
  }

  async function save() {
    setBusy(true);
    try {
      const request = toRequest();
      if (editingId) {
        await lessonApi.updateSection(lessonId, editingId, request);
        toast.success("Đã cập nhật section.");
      } else {
        await lessonApi.createSection(lessonId, request);
        toast.success("Đã thêm section.");
      }
      resetDraft();
      await load();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể lưu section.");
    } finally {
      setBusy(false);
    }
  }

  async function move(index: number, direction: -1 | 1) {
    const targetIndex = index + direction;
    if (targetIndex < 0 || targetIndex >= ordered.length) return;

    const current = ordered[index];
    const target = ordered[targetIndex];
    const temporaryOrder = Math.max(...ordered.map((item) => item.sortOrder), 0) + 100;

    setBusy(true);
    try {
      await lessonApi.updateSection(lessonId, current.id, sectionRequest(current, temporaryOrder));
      await lessonApi.updateSection(lessonId, target.id, sectionRequest(target, current.sortOrder));
      await lessonApi.updateSection(lessonId, current.id, sectionRequest(current, target.sortOrder));
      await load();
      toast.success("Đã cập nhật thứ tự section.");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể đổi thứ tự section.");
    } finally {
      setBusy(false);
    }
  }

  async function deleteSection() {
    if (!deleting) return;
    setBusy(true);
    try {
      await lessonApi.deleteSection(lessonId, deleting.id);
      if (editingId === deleting.id) resetDraft();
      setDeleting(null);
      await load();
      toast.success("Đã xóa section.");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể xóa section.");
    } finally {
      setBusy(false);
    }
  }

  function insertMarkup(before: string, after = "", placeholder = "nội dung") {
    const textarea = textareaRef.current;
    const value = draft.contentVi;
    if (!textarea) {
      updateDraft("contentVi", `${value}${before}${placeholder}${after}`);
      return;
    }

    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const selected = value.slice(start, end) || placeholder;
    const next = `${value.slice(0, start)}${before}${selected}${after}${value.slice(end)}`;
    updateDraft("contentVi", next);

    window.requestAnimationFrame(() => {
      textarea.focus();
      const cursor = start + before.length + selected.length + after.length;
      textarea.setSelectionRange(cursor, cursor);
    });
  }

  function insertAsset(asset: AdminLessonAsset) {
    const source = asset.url?.trim();
    if (!source) {
      toast.error("Tài nguyên này chưa có URL để chèn vào nội dung.");
      return;
    }

    const caption = asset.captionVi?.trim() || lessonAssetTypeLabels[asset.assetType];
    const markup = asset.assetType === LessonAssetType.Image
      ? `\n![${caption}](${source})\n`
      : `\n[${caption}](${source})\n`;

    const textarea = textareaRef.current;
    const start = textarea?.selectionStart ?? draft.contentVi.length;
    const next = `${draft.contentVi.slice(0, start)}${markup}${draft.contentVi.slice(start)}`;
    updateDraft("contentVi", next);
    toast.success("Đã chèn tài nguyên vào nội dung section.");
  }

  if (loading) {
    return (
      <div className="grid gap-4 xl:grid-cols-[minmax(0,1.45fr)_minmax(320px,.55fr)]">
        <Skeleton className="h-[620px] rounded-[12px]" />
        <Skeleton className="h-[620px] rounded-[12px]" />
      </div>
    );
  }

  return (
    <div className="grid gap-4 xl:grid-cols-[minmax(0,1.45fr)_minmax(320px,.55fr)]">
      <div className="space-y-4">
        <Card>
          <CardHeader className="flex flex-row items-start justify-between gap-3">
            <div>
              <CardTitle>{editingId ? "Chỉnh sửa section" : "Thêm section mới"}</CardTitle>
              <p className="mt-1 text-[13px] leading-5 text-[#777]">
                Soạn nội dung bằng Markdown nhẹ, xem trước trực tiếp và chèn tài nguyên đã có của Lesson.
              </p>
            </div>
            <Button type="button" variant="outline" size="sm" onClick={() => void load()} disabled={busy} className="gap-2">
              <RefreshCw size={14} /> Làm mới
            </Button>
          </CardHeader>

          <CardContent className="space-y-4">
            <div className="grid gap-4 md:grid-cols-2">
              <Field label="Loại section">
                <Select
                  value={String(draft.sectionType)}
                  onValueChange={(value) => updateDraft("sectionType", Number(value) as LessonSectionType)}
                  options={sectionTypeOptions}
                />
              </Field>
              <Field label="Tiêu đề">
                <Input
                  value={draft.titleVi}
                  onChange={(event) => updateDraft("titleVi", event.target.value)}
                  placeholder="Ví dụ: Mẫu câu chào hỏi"
                />
              </Field>
              <Field label="Thứ tự">
                <Input
                  type="number"
                  min={0}
                  value={draft.sortOrder}
                  onChange={(event) => updateDraft("sortOrder", Number(event.target.value) || 0)}
                />
              </Field>
              <Field label="Thời lượng ước tính (giây)">
                <Input
                  type="number"
                  min={1}
                  value={draft.estimatedSeconds}
                  onChange={(event) => updateDraft("estimatedSeconds", Number(event.target.value) || 0)}
                />
              </Field>
            </div>

            <div className="flex items-center justify-between rounded-[9px] border border-[#e7e2db] bg-[#faf9f7] px-3 py-2.5">
              <div>
                <div className="text-[13px] font-medium text-[#444]">Yêu cầu hoàn thành</div>
                <div className="mt-0.5 text-[12px] text-[#888]">Section bắt buộc sẽ được tính vào tiến độ Lesson.</div>
              </div>
              <Switch checked={draft.isRequired} onCheckedChange={(value) => updateDraft("isRequired", value)} />
            </div>

            <div>
              <div className="mb-2 flex flex-wrap items-center justify-between gap-2">
                <span className="text-[13px] font-medium text-[#555]">Nội dung section</span>
                <div className="flex gap-1">
                  <Button type="button" variant={!previewMode ? "secondary" : "ghost"} size="sm" onClick={() => setPreviewMode(false)}>
                    Soạn thảo
                  </Button>
                  <Button type="button" variant={previewMode ? "secondary" : "ghost"} size="sm" onClick={() => setPreviewMode(true)} className="gap-1.5">
                    <Eye size={13} /> Xem trước
                  </Button>
                </div>
              </div>

              {!previewMode ? (
                <>
                  <div className="mb-2 flex flex-wrap gap-1 rounded-[8px] border border-[#e7e2db] bg-[#faf9f7] p-1.5">
                    <ToolbarButton label="Tiêu đề" onClick={() => insertMarkup("## ", "", "Tiêu đề")}><Heading2 size={14} /></ToolbarButton>
                    <ToolbarButton label="Đậm" onClick={() => insertMarkup("**", "**")}><Bold size={14} /></ToolbarButton>
                    <ToolbarButton label="Nghiêng" onClick={() => insertMarkup("*", "*")}><Italic size={14} /></ToolbarButton>
                    <ToolbarButton label="Danh sách" onClick={() => insertMarkup("- ", "", "Mục nội dung")}><List size={14} /></ToolbarButton>
                    <ToolbarButton label="Trích dẫn" onClick={() => insertMarkup("> ", "", "Ghi chú quan trọng")}><Quote size={14} /></ToolbarButton>
                    <ToolbarButton label="Liên kết" onClick={() => insertMarkup("[", "](https://)", "văn bản liên kết")}><Link2 size={14} /></ToolbarButton>
                  </div>
                  <Textarea
                    ref={textareaRef}
                    value={draft.contentVi}
                    onChange={(event) => updateDraft("contentVi", event.target.value)}
                    className="min-h-[300px] font-mono text-[13px] leading-6"
                    placeholder={"## Mục tiêu\n\nNội dung bài học...\n\n- Ý chính 1\n- Ý chính 2"}
                  />
                </>
              ) : (
                <div className="min-h-[300px] rounded-[9px] border border-[#e7e2db] bg-white p-5">
                  <SectionContentPreview content={draft.contentVi} assets={assets} />
                </div>
              )}
            </div>

            <div className="flex flex-wrap justify-end gap-2 border-t border-[#eee9e2] pt-4">
              {editingId && (
                <Button type="button" variant="outline" size="md" onClick={resetDraft} disabled={busy}>
                  Hủy chỉnh sửa
                </Button>
              )}
              <Button type="button" size="md" onClick={() => void save()} loading={busy} className="gap-2">
                {editingId ? <Save size={14} /> : <Plus size={14} />}
                {editingId ? "Lưu thay đổi" : "Thêm section"}
              </Button>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Cấu trúc bài học ({ordered.length})</CardTitle>
          </CardHeader>
          <CardContent>
            {ordered.length === 0 ? (
              <Alert variant="info">Chưa có section. Hãy tạo phần nội dung đầu tiên cho bài giảng.</Alert>
            ) : (
              <div className="space-y-2">
                {ordered.map((item, index) => {
                  const media = [...(sectionMedia[item.id] ?? [])]
                    .sort((a, b) => a.sortOrder - b.sortOrder || a.id - b.id);

                  return (
                    <div
                      key={item.id}
                      className={cn(
                        "rounded-[10px] border px-3 py-3 transition",
                        editingId === item.id ? "border-[#efaca8] bg-[#fff8f7]" : "border-[#e8e3dc] bg-white",
                      )}
                    >
                      <div className="flex flex-col gap-3 md:flex-row md:items-start md:justify-between">
                        <div className="min-w-0 flex-1">
                          <div className="flex flex-wrap items-center gap-2">
                            <span className="text-[14px] font-semibold text-[#333]">
                              {index + 1}. {item.titleVi || lessonSectionTypeLabels[item.sectionType]}
                            </span>
                            <Badge>{lessonSectionTypeLabels[item.sectionType]}</Badge>
                            {item.isRequired && <Badge variant="warning">Bắt buộc</Badge>}
                            {media.length > 0 && <Badge variant="info">{media.length} media</Badge>}
                          </div>
                          <div className="mt-1 text-[12px] text-[#888]">
                            Thứ tự {item.sortOrder} · {item.estimatedSeconds ?? 0} giây
                          </div>
                          {item.contentVi && (
                            <p className="mt-2 line-clamp-2 whitespace-pre-wrap text-[13px] leading-5 text-[#666]">
                              {item.contentVi}
                            </p>
                          )}

                          {media.length > 0 && (
                            <div className="mt-3 grid gap-2 border-t border-[#eee9e2] pt-3 md:grid-cols-2">
                              {media.map((mediaItem) => (
                                <SectionMediaPreview key={mediaItem.id} item={mediaItem} />
                              ))}
                            </div>
                          )}
                        </div>

                        <div className="flex shrink-0 flex-wrap gap-1.5">
                          <Button type="button" variant="ghost" size="sm" disabled={busy || index === 0} onClick={() => void move(index, -1)} aria-label="Đưa section lên">
                            <ArrowUp size={14} />
                          </Button>
                          <Button type="button" variant="ghost" size="sm" disabled={busy || index === ordered.length - 1} onClick={() => void move(index, 1)} aria-label="Đưa section xuống">
                            <ArrowDown size={14} />
                          </Button>
                          <Button type="button" variant="outline" size="sm" disabled={busy} onClick={() => editSection(item)} className="gap-1.5">
                            <Pencil size={13} /> Sửa
                          </Button>
                          <Button type="button" variant="dangerGhost" size="sm" disabled={busy} onClick={() => setDeleting(item)} className="gap-1.5">
                            <Trash2 size={13} /> Xóa
                          </Button>
                        </div>
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </CardContent>
        </Card>
      </div>

      <aside className="space-y-4 xl:sticky xl:top-4 xl:self-start">
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2"><ImageIcon size={16} /> Kho media Lesson</CardTitle>
            <p className="mt-1 text-[13px] leading-5 text-[#777]">
              Chèn tài nguyên đã quản lý ở Lesson vào vị trí con trỏ trong nội dung section.
            </p>
          </CardHeader>
          <CardContent>
            {assets.length === 0 ? (
              <Alert variant="info">Lesson chưa có tài nguyên. Hãy vào phần Tài nguyên & liên kết để upload hoặc khai báo media.</Alert>
            ) : (
              <div className="space-y-2">
                {[...assets].sort((a, b) => a.sortOrder - b.sortOrder).map((asset) => (
                  <div key={asset.id} className="rounded-[9px] border border-[#e8e3dc] p-3">
                    <div className="flex items-start justify-between gap-3">
                      <div className="min-w-0">
                        <div className="text-[13px] font-semibold text-[#444]">
                          {asset.captionVi || `${lessonAssetTypeLabels[asset.assetType]} #${asset.id}`}
                        </div>
                        <div className="mt-1 break-all text-[12px] leading-4 text-[#888]">
                          {asset.url || (asset.audioAssetId ? `AudioAsset #${asset.audioAssetId}` : "Chưa có URL")}
                        </div>
                      </div>
                      <Badge variant="info">{lessonAssetTypeLabels[asset.assetType]}</Badge>
                    </div>
                    <Button
                      type="button"
                      variant="outline"
                      size="sm"
                      className="mt-3 w-full gap-2"
                      onClick={() => insertAsset(asset)}
                      disabled={!asset.url}
                    >
                      <Plus size={13} /> Chèn vào section
                    </Button>
                  </div>
                ))}
              </div>
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2"><BookOpenText size={16} /> Preview nhanh</CardTitle>
          </CardHeader>
          <CardContent>
            <SectionContentPreview content={draft.contentVi} assets={assets} compact />
          </CardContent>
        </Card>
      </aside>

      <ConfirmDialog
        open={Boolean(deleting)}
        title="Xóa section?"
        description={`Section “${deleting?.titleVi || (deleting ? lessonSectionTypeLabels[deleting.sectionType] : "") }” sẽ bị xóa khỏi bài giảng.`}
        confirmLabel="Xóa section"
        loading={busy}
        onClose={() => setDeleting(null)}
        onConfirm={deleteSection}
      />
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

function ToolbarButton({ label, onClick, children }: { label: string; onClick: () => void; children: React.ReactNode }) {
  return (
    <Button type="button" variant="ghost" size="sm" title={label} aria-label={label} onClick={onClick}>
      {children}
    </Button>
  );
}

function SectionMediaPreview({ item }: { item: AdminLessonSectionAsset }) {
  const type = item.assetType.toLowerCase();
  const caption = item.captionVi || item.assetCaptionVi || "Tài nguyên";

  if (type.includes("image")) {
    return (
      <div className="overflow-hidden rounded-[8px] border border-[#e8e3dc] bg-[#faf9f7]">
        {item.url ? (
          // eslint-disable-next-line @next/next/no-img-element
          <img src={item.url} alt={caption} className="h-[120px] w-full object-cover" />
        ) : (
          <div className="flex h-[90px] items-center justify-center text-[12px] text-[#999]">Image chưa có URL</div>
        )}
        <div className="flex items-center gap-2 px-2.5 py-2 text-[12px] text-[#666]">
          <ImageIcon size={13} /> <span className="truncate">{caption}</span>
        </div>
      </div>
    );
  }

  if (type.includes("audio")) {
    return (
      <div className="rounded-[8px] border border-[#e8e3dc] bg-[#faf9f7] p-2.5">
        <div className="mb-2 flex items-center gap-2 text-[12px] font-medium text-[#666]"><Headphones size={13} /> {caption}</div>
        {item.url ? (
          <audio controls preload="metadata" className="h-9 w-full" src={item.url}>Trình duyệt không hỗ trợ audio.</audio>
        ) : (
          <div className="text-[12px] text-[#999]">AudioAsset #{item.audioAssetId ?? "?"}</div>
        )}
      </div>
    );
  }

  return (
    <div className="flex min-h-[74px] items-center justify-between gap-2 rounded-[8px] border border-[#e8e3dc] bg-[#faf9f7] p-2.5">
      <div className="flex min-w-0 items-center gap-2 text-[12px] text-[#666]"><FileText size={14} /><span className="truncate">{caption}</span></div>
      {item.url && <a href={item.url} target="_blank" rel="noreferrer" className="shrink-0 text-[12px] font-medium text-[#ef241c] hover:underline">Mở</a>}
    </div>
  );
}

function SectionContentPreview({
  content,
  assets,
  compact = false,
}: {
  content: string;
  assets: AdminLessonAsset[];
  compact?: boolean;
}) {
  if (!content.trim()) {
    return <div className="text-[13px] text-[#999]">Chưa có nội dung để xem trước.</div>;
  }

  const lines = content.split("\n");

  return (
    <div className={cn("space-y-2 text-[14px] leading-6 text-[#444]", compact && "max-h-[420px] overflow-auto pr-1 text-[13px]")}>
      {lines.map((line, index) => {
        const trimmed = line.trim();
        const image = trimmed.match(/^!\[([^\]]*)\]\(([^)]+)\)$/);
        const link = trimmed.match(/^\[([^\]]+)\]\(([^)]+)\)$/);

        if (!trimmed) return <div key={index} className="h-2" />;
        if (image) {
          const [, alt, src] = image;
          return (
            <figure key={index} className="overflow-hidden rounded-[9px] border border-[#e8e3dc] bg-[#faf9f7]">
              {/* eslint-disable-next-line @next/next/no-img-element */}
              <img src={src} alt={alt} className="max-h-[360px] w-full object-contain" />
              {alt && <figcaption className="px-3 py-2 text-[12px] text-[#777]">{alt}</figcaption>}
            </figure>
          );
        }
        if (link) {
          const [, label, href] = link;
          return <a key={index} href={href} target="_blank" rel="noreferrer" className="text-[#d92720] underline underline-offset-2">{label}</a>;
        }
        if (trimmed.startsWith("## ")) return <h3 key={index} className="pt-2 text-[18px] font-semibold text-[#2f2f2f]">{trimmed.slice(3)}</h3>;
        if (trimmed.startsWith("### ")) return <h4 key={index} className="pt-1 text-[16px] font-semibold text-[#333]">{trimmed.slice(4)}</h4>;
        if (trimmed.startsWith("- ")) return <div key={index} className="flex gap-2"><span>•</span><span>{renderInline(trimmed.slice(2))}</span></div>;
        if (trimmed.startsWith("> ")) return <blockquote key={index} className="border-l-3 border-[#efaca8] bg-[#fff8f7] px-3 py-2 text-[#666]">{renderInline(trimmed.slice(2))}</blockquote>;
        return <p key={index}>{renderInline(trimmed)}</p>;
      })}
      {assets.length > 0 && !compact && (
        <div className="pt-3 text-[11px] text-[#aaa]">Preview dùng Markdown nhẹ; media liên kết section được hiển thị trực tiếp trong Section Card.</div>
      )}
    </div>
  );
}

function renderInline(text: string) {
  const parts = text.split(/(\*\*[^*]+\*\*|\*[^*]+\*)/g).filter(Boolean);
  return parts.map((part, index) => {
    if (part.startsWith("**") && part.endsWith("**")) {
      return <strong key={index}>{part.slice(2, -2)}</strong>;
    }
    if (part.startsWith("*") && part.endsWith("*")) {
      return <em key={index}>{part.slice(1, -1)}</em>;
    }
    return <span key={index}>{part}</span>;
  });
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
