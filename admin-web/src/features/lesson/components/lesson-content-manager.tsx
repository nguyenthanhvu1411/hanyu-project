"use client";

import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";
import {
  ArrowDown,
  ArrowUp,
  BookOpen,
  ImageIcon,
  Link2,
  Pencil,
  Plus,
  RefreshCw,
  Trash2,
} from "lucide-react";
import { toast } from "sonner";

import { EmptyState } from "@/components/common/empty-state";
import { FormField } from "@/components/forms/form-field";
import { FormRow } from "@/components/forms/form-row";
import { FormSection } from "@/components/forms/form-section";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Dialog } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { Switch } from "@/components/ui/switch";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Textarea } from "@/components/ui/textarea";

import { lessonApi } from "../api/lesson.api";
import {
  LessonAssetType,
  LessonSectionType,
  lessonAssetTypeLabels,
  lessonSectionTypeLabels,
  type AdminLessonAsset,
  type AdminLessonPrerequisite,
  type AdminLessonSection,
  type AdminLessonVocabulary,
} from "../types/lesson.types";

type TabKey = "sections" | "vocabulary" | "assets" | "prerequisites";
type RunAction = (action: () => Promise<unknown>, success: string) => Promise<boolean>;

interface LessonContentManagerProps {
  lessonId: number;
}

interface PanelProps<T> {
  lessonId: number;
  items: T[];
  busy: boolean;
  run: RunAction;
}

export function LessonContentManager({ lessonId }: LessonContentManagerProps) {
  const [tab, setTab] = useState<TabKey>("sections");
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [sections, setSections] = useState<AdminLessonSection[]>([]);
  const [vocabulary, setVocabulary] = useState<AdminLessonVocabulary[]>([]);
  const [assets, setAssets] = useState<AdminLessonAsset[]>([]);
  const [prerequisites, setPrerequisites] = useState<AdminLessonPrerequisite[]>([]);

  const loadAll = useCallback(async () => {
    setLoading(true);
    try {
      const [sectionData, vocabularyData, assetData, prerequisiteData] = await Promise.all([
        lessonApi.listSections(lessonId),
        lessonApi.listVocabulary(lessonId),
        lessonApi.listAssets(lessonId),
        lessonApi.listPrerequisites(lessonId),
      ]);
      setSections(sectionData);
      setVocabulary(vocabularyData);
      setAssets(assetData);
      setPrerequisites(prerequisiteData);
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không tải được nội dung bài giảng.");
    } finally {
      setLoading(false);
    }
  }, [lessonId]);

  useEffect(() => {
    void loadAll();
  }, [loadAll]);

  async function run(action: () => Promise<unknown>, success: string) {
    setBusy(true);
    try {
      await action();
      toast.success(success);
      await loadAll();
      return true;
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Thao tác thất bại.");
      return false;
    } finally {
      setBusy(false);
    }
  }

  if (loading) {
    return (
      <div className="space-y-3">
        <Skeleton className="h-12 w-full rounded-[11px]" />
        <Skeleton className="h-72 w-full rounded-[11px]" />
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <Card>
        <CardContent className="flex flex-col gap-3 p-3 md:flex-row md:items-center md:justify-between">
          <Tabs value={tab} onValueChange={(value) => setTab(value as TabKey)}>
            <TabsList className="border-0">
              <TabsTrigger value="sections">Nội dung ({sections.length})</TabsTrigger>
              <TabsTrigger value="vocabulary">Từ vựng ({vocabulary.length})</TabsTrigger>
              <TabsTrigger value="assets">Tài nguyên ({assets.length})</TabsTrigger>
              <TabsTrigger value="prerequisites">Tiên quyết ({prerequisites.length})</TabsTrigger>
            </TabsList>
          </Tabs>
          <Button variant="outline" className="gap-2" onClick={() => void loadAll()} disabled={busy}>
            <RefreshCw size={14} /> Làm mới
          </Button>
        </CardContent>
      </Card>

      {tab === "sections" && <SectionsPanel lessonId={lessonId} items={sections} busy={busy} run={run} />}
      {tab === "vocabulary" && <VocabularyPanel lessonId={lessonId} items={vocabulary} busy={busy} run={run} />}
      {tab === "assets" && <AssetsPanel lessonId={lessonId} items={assets} busy={busy} run={run} />}
      {tab === "prerequisites" && <PrerequisitesPanel lessonId={lessonId} items={prerequisites} busy={busy} run={run} />}
    </div>
  );
}

function SectionsPanel({ lessonId, items, busy, run }: PanelProps<AdminLessonSection>) {
  const ordered = useMemo(() => [...items].sort((a, b) => a.sortOrder - b.sortOrder), [items]);
  const [sectionType, setSectionType] = useState(LessonSectionType.Introduction);
  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [sortOrder, setSortOrder] = useState(items.length);
  const [estimatedSeconds, setEstimatedSeconds] = useState(120);
  const [required, setRequired] = useState(true);
  const [editing, setEditing] = useState<AdminLessonSection | null>(null);
  const [deleting, setDeleting] = useState<AdminLessonSection | null>(null);

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const ok = await run(
      () => lessonApi.createSection(lessonId, {
        sectionType,
        titleVi: title.trim() || null,
        contentVi: content.trim() || null,
        sortOrder,
        estimatedSeconds: estimatedSeconds || null,
        isRequired: required,
      }),
      "Đã thêm phần nội dung.",
    );
    if (ok) {
      setTitle("");
      setContent("");
      setSortOrder(items.length + 1);
    }
  }

  async function move(index: number, direction: -1 | 1) {
    const targetIndex = index + direction;
    if (targetIndex < 0 || targetIndex >= ordered.length) return;
    const current = ordered[index];
    const target = ordered[targetIndex];
    const temporaryOrder = Math.max(...ordered.map((x) => x.sortOrder), 0) + 1;

    await run(async () => {
      await lessonApi.updateSection(lessonId, current.id, sectionRequest(current, temporaryOrder));
      await lessonApi.updateSection(lessonId, target.id, sectionRequest(target, current.sortOrder));
      await lessonApi.updateSection(lessonId, current.id, sectionRequest(current, target.sortOrder));
    }, "Đã cập nhật thứ tự section.");
  }

  return (
    <div className="space-y-4">
      <form onSubmit={submit}>
        <FormSection
          title="Thêm LessonSection"
          description="Chia bài giảng thành các phần học có thứ tự, thời lượng và trạng thái bắt buộc."
          icon={<BookOpen size={18} />}
        >
          <FormRow columns={2}>
            <FormField label="Loại section" required>
              <Select
                value={String(sectionType)}
                onValueChange={(value) => setSectionType(Number(value) as LessonSectionType)}
                options={Object.entries(lessonSectionTypeLabels).map(([value, label]) => ({ value, label }))}
              />
            </FormField>
            <FormField label="Tiêu đề">
              <Input value={title} onChange={(event) => setTitle(event.target.value)} />
            </FormField>
          </FormRow>
          <FormRow columns={2}>
            <FormField label="Thứ tự">
              <Input type="number" min={0} value={sortOrder} onChange={(event) => setSortOrder(Number(event.target.value))} />
            </FormField>
            <FormField label="Thời lượng (giây)">
              <Input type="number" min={0} value={estimatedSeconds} onChange={(event) => setEstimatedSeconds(Number(event.target.value))} />
            </FormField>
          </FormRow>
          <FormField label="Nội dung">
            <Textarea value={content} onChange={(event) => setContent(event.target.value)} rows={6} />
          </FormField>
          <FormRow columns={2}>
            <FormField label="Yêu cầu hoàn thành">
              <Switch checked={required} onCheckedChange={setRequired} label={required ? "Bắt buộc" : "Không bắt buộc"} />
            </FormField>
            <div className="flex items-end justify-end">
              <Button type="submit" disabled={busy} className="gap-2"><Plus size={14} /> Thêm section</Button>
            </div>
          </FormRow>
        </FormSection>
      </form>

      {ordered.length === 0 ? (
        <EmptyState title="Chưa có section" description="Bài giảng chưa có phần nội dung nào." />
      ) : (
        <div className="space-y-2">
          {ordered.map((item, index) => (
            <Card key={item.id}>
              <CardHeader className="flex flex-row items-start justify-between gap-3 py-3">
                <div className="min-w-0">
                  <CardTitle>{index + 1}. {item.titleVi || lessonSectionTypeLabels[item.sectionType]}</CardTitle>
                  <div className="mt-2 flex flex-wrap gap-2">
                    <Badge>{lessonSectionTypeLabels[item.sectionType]}</Badge>
                    {item.isRequired && <Badge variant="warning">Bắt buộc</Badge>}
                    <Badge variant="info">#{item.sortOrder} · {item.estimatedSeconds ?? 0}s</Badge>
                  </div>
                </div>
                <ContentActions
                  busy={busy}
                  canMoveUp={index > 0}
                  canMoveDown={index < ordered.length - 1}
                  onMoveUp={() => void move(index, -1)}
                  onMoveDown={() => void move(index, 1)}
                  onEdit={() => setEditing(item)}
                  onDelete={() => setDeleting(item)}
                />
              </CardHeader>
              {item.contentVi && (
                <CardContent><p className="whitespace-pre-wrap text-[12px] leading-5 text-[#666]">{item.contentVi}</p></CardContent>
              )}
            </Card>
          ))}
        </div>
      )}

      <SectionEditDialog
        item={editing}
        busy={busy}
        onClose={() => setEditing(null)}
        onSave={async (item, request) => {
          const ok = await run(() => lessonApi.updateSection(lessonId, item.id, request), "Đã cập nhật section.");
          if (ok) setEditing(null);
        }}
      />
      <ConfirmDeleteDialog
        open={Boolean(deleting)}
        title="Xóa section?"
        description="Section sẽ bị xóa khỏi bài giảng. Thao tác có thể bị backend từ chối nếu đã phát sinh dữ liệu học."
        busy={busy}
        onClose={() => setDeleting(null)}
        onConfirm={async () => {
          if (!deleting) return;
          const ok = await run(() => lessonApi.deleteSection(lessonId, deleting.id), "Đã xóa section.");
          if (ok) setDeleting(null);
        }}
      />
    </div>
  );
}

function VocabularyPanel({ lessonId, items, busy, run }: PanelProps<AdminLessonVocabulary>) {
  const ordered = useMemo(() => [...items].sort((a, b) => a.sortOrder - b.sortOrder), [items]);
  const [vocabularyId, setVocabularyId] = useState("");
  const [sortOrder, setSortOrder] = useState(items.length);
  const [required, setRequired] = useState(true);
  const [editing, setEditing] = useState<AdminLessonVocabulary | null>(null);
  const [deleting, setDeleting] = useState<AdminLessonVocabulary | null>(null);

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const id = Number(vocabularyId);
    if (!Number.isSafeInteger(id) || id <= 0) {
      toast.error("Vocabulary ID không hợp lệ.");
      return;
    }
    const ok = await run(
      () => lessonApi.attachVocabulary(lessonId, { vocabularyId: id, sortOrder, isRequired: required }),
      "Đã gắn từ vựng.",
    );
    if (ok) {
      setVocabularyId("");
      setSortOrder(items.length + 1);
    }
  }

  async function move(index: number, direction: -1 | 1) {
    const targetIndex = index + direction;
    if (targetIndex < 0 || targetIndex >= ordered.length) return;
    const current = ordered[index];
    const target = ordered[targetIndex];
    const temporaryOrder = Math.max(...ordered.map((x) => x.sortOrder), 0) + 1;

    await run(async () => {
      await lessonApi.updateVocabulary(lessonId, current.vocabularyId, { sortOrder: temporaryOrder, isRequired: current.isRequired });
      await lessonApi.updateVocabulary(lessonId, target.vocabularyId, { sortOrder: current.sortOrder, isRequired: target.isRequired });
      await lessonApi.updateVocabulary(lessonId, current.vocabularyId, { sortOrder: target.sortOrder, isRequired: current.isRequired });
    }, "Đã cập nhật thứ tự từ vựng.");
  }

  return (
    <div className="space-y-4">
      <form onSubmit={submit}>
        <FormSection title="Gắn LessonVocabulary" description="Gắn từ vựng đã tồn tại trong kho Vocabulary vào bài giảng." icon={<Link2 size={18} />}>
          <FormRow columns={3}>
            <FormField label="Vocabulary ID" required>
              <Input type="number" min={1} value={vocabularyId} onChange={(event) => setVocabularyId(event.target.value)} />
            </FormField>
            <FormField label="Thứ tự">
              <Input type="number" min={0} value={sortOrder} onChange={(event) => setSortOrder(Number(event.target.value))} />
            </FormField>
            <FormField label="Yêu cầu">
              <Switch checked={required} onCheckedChange={setRequired} label={required ? "Bắt buộc" : "Không bắt buộc"} />
            </FormField>
          </FormRow>
          <div className="flex justify-end"><Button type="submit" disabled={busy} className="gap-2"><Plus size={14} /> Gắn từ vựng</Button></div>
        </FormSection>
      </form>

      {ordered.length === 0 ? (
        <EmptyState title="Chưa có từ vựng" description="Bài giảng chưa gắn từ vựng nào." />
      ) : (
        <div className="space-y-2">
          {ordered.map((item, index) => (
            <Card key={item.vocabularyId}>
              <CardContent className="flex items-center justify-between gap-3 py-3">
                <div className="min-w-0">
                  <div className="flex flex-wrap items-center gap-2">
                    <span className="text-[16px] font-semibold text-[#333]">{item.simplified}</span>
                    <Badge>{item.pinyin}</Badge>
                    {item.isRequired && <Badge variant="warning">Bắt buộc</Badge>}
                    <Badge variant="info">#{item.sortOrder}</Badge>
                  </div>
                  <p className="mt-1 text-[11px] text-[#666]">{item.primaryMeaningVi} · ID {item.vocabularyId}</p>
                </div>
                <ContentActions
                  busy={busy}
                  canMoveUp={index > 0}
                  canMoveDown={index < ordered.length - 1}
                  onMoveUp={() => void move(index, -1)}
                  onMoveDown={() => void move(index, 1)}
                  onEdit={() => setEditing(item)}
                  onDelete={() => setDeleting(item)}
                />
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      <VocabularyEditDialog
        item={editing}
        busy={busy}
        onClose={() => setEditing(null)}
        onSave={async (item, nextOrder, nextRequired) => {
          const ok = await run(
            () => lessonApi.updateVocabulary(lessonId, item.vocabularyId, { sortOrder: nextOrder, isRequired: nextRequired }),
            "Đã cập nhật từ vựng trong bài giảng.",
          );
          if (ok) setEditing(null);
        }}
      />
      <ConfirmDeleteDialog
        open={Boolean(deleting)}
        title="Gỡ từ vựng?"
        description="Từ vựng chỉ được gỡ khỏi bài giảng, dữ liệu Vocabulary gốc không bị xóa."
        busy={busy}
        onClose={() => setDeleting(null)}
        onConfirm={async () => {
          if (!deleting) return;
          const ok = await run(() => lessonApi.detachVocabulary(lessonId, deleting.vocabularyId), "Đã gỡ từ vựng.");
          if (ok) setDeleting(null);
        }}
      />
    </div>
  );
}

function AssetsPanel({ lessonId, items, busy, run }: PanelProps<AdminLessonAsset>) {
  const ordered = useMemo(() => [...items].sort((a, b) => a.sortOrder - b.sortOrder), [items]);
  const [assetType, setAssetType] = useState(LessonAssetType.Image);
  const [url, setUrl] = useState("");
  const [caption, setCaption] = useState("");
  const [audioAssetId, setAudioAssetId] = useState("");
  const [sortOrder, setSortOrder] = useState(items.length);
  const [editing, setEditing] = useState<AdminLessonAsset | null>(null);
  const [deleting, setDeleting] = useState<AdminLessonAsset | null>(null);

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const audioId = audioAssetId ? Number(audioAssetId) : null;
    if (audioId !== null && (!Number.isSafeInteger(audioId) || audioId <= 0)) {
      toast.error("AudioAsset ID không hợp lệ.");
      return;
    }
    const ok = await run(
      () => lessonApi.createAsset(lessonId, {
        assetType,
        url: url.trim() || null,
        captionVi: caption.trim() || null,
        audioAssetId: audioId,
        sortOrder,
      }),
      "Đã thêm tài nguyên.",
    );
    if (ok) {
      setUrl("");
      setCaption("");
      setAudioAssetId("");
      setSortOrder(items.length + 1);
    }
  }

  async function move(index: number, direction: -1 | 1) {
    const targetIndex = index + direction;
    if (targetIndex < 0 || targetIndex >= ordered.length) return;
    const current = ordered[index];
    const target = ordered[targetIndex];
    const temporaryOrder = Math.max(...ordered.map((x) => x.sortOrder), 0) + 1;

    await run(async () => {
      await lessonApi.updateAsset(lessonId, current.id, assetRequest(current, temporaryOrder));
      await lessonApi.updateAsset(lessonId, target.id, assetRequest(target, current.sortOrder));
      await lessonApi.updateAsset(lessonId, current.id, assetRequest(current, target.sortOrder));
    }, "Đã cập nhật thứ tự tài nguyên.");
  }

  return (
    <div className="space-y-4">
      <form onSubmit={submit}>
        <FormSection title="Thêm LessonAsset" description="Quản lý hình ảnh, audio và tài liệu gắn trực tiếp với bài giảng." icon={<ImageIcon size={18} />}>
          <FormRow columns={2}>
            <FormField label="Loại tài nguyên">
              <Select value={String(assetType)} onValueChange={(value) => setAssetType(Number(value) as LessonAssetType)} options={Object.entries(lessonAssetTypeLabels).map(([value, label]) => ({ value, label }))} />
            </FormField>
            <FormField label="Thứ tự"><Input type="number" min={0} value={sortOrder} onChange={(event) => setSortOrder(Number(event.target.value))} /></FormField>
          </FormRow>
          <FormRow columns={2}>
            <FormField label="URL"><Input value={url} onChange={(event) => setUrl(event.target.value)} placeholder="https://..." /></FormField>
            <FormField label="AudioAsset ID"><Input type="number" min={1} value={audioAssetId} onChange={(event) => setAudioAssetId(event.target.value)} /></FormField>
          </FormRow>
          <FormField label="Chú thích"><Textarea value={caption} onChange={(event) => setCaption(event.target.value)} rows={3} /></FormField>
          <div className="flex justify-end"><Button type="submit" disabled={busy} className="gap-2"><Plus size={14} /> Thêm tài nguyên</Button></div>
        </FormSection>
      </form>

      {ordered.length === 0 ? (
        <EmptyState title="Chưa có tài nguyên" description="Bài giảng chưa có hình ảnh, audio hoặc tài liệu." />
      ) : (
        <div className="space-y-2">
          {ordered.map((item, index) => (
            <Card key={item.id}>
              <CardContent className="flex items-center justify-between gap-3 py-3">
                <div className="min-w-0">
                  <div className="flex flex-wrap gap-2"><Badge variant="info">{lessonAssetTypeLabels[item.assetType]}</Badge><Badge>#{item.sortOrder}</Badge></div>
                  <p className="mt-1 truncate text-[11px] text-[#666]">{item.captionVi || item.url || `Asset #${item.id}`}</p>
                </div>
                <ContentActions
                  busy={busy}
                  canMoveUp={index > 0}
                  canMoveDown={index < ordered.length - 1}
                  onMoveUp={() => void move(index, -1)}
                  onMoveDown={() => void move(index, 1)}
                  onEdit={() => setEditing(item)}
                  onDelete={() => setDeleting(item)}
                />
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      <AssetEditDialog
        item={editing}
        busy={busy}
        onClose={() => setEditing(null)}
        onSave={async (item, request) => {
          const ok = await run(() => lessonApi.updateAsset(lessonId, item.id, request), "Đã cập nhật tài nguyên.");
          if (ok) setEditing(null);
        }}
      />
      <ConfirmDeleteDialog
        open={Boolean(deleting)}
        title="Xóa tài nguyên?"
        description="Tài nguyên sẽ bị gỡ khỏi bài giảng."
        busy={busy}
        onClose={() => setDeleting(null)}
        onConfirm={async () => {
          if (!deleting) return;
          const ok = await run(() => lessonApi.deleteAsset(lessonId, deleting.id), "Đã xóa tài nguyên.");
          if (ok) setDeleting(null);
        }}
      />
    </div>
  );
}

function PrerequisitesPanel({ lessonId, items, busy, run }: PanelProps<AdminLessonPrerequisite>) {
  const [requiredLessonId, setRequiredLessonId] = useState("");
  const [deleting, setDeleting] = useState<AdminLessonPrerequisite | null>(null);

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const id = Number(requiredLessonId);
    if (!Number.isSafeInteger(id) || id <= 0 || id === lessonId) {
      toast.error("Lesson tiên quyết không hợp lệ.");
      return;
    }
    const ok = await run(() => lessonApi.addPrerequisite(lessonId, { requiredLessonId: id }), "Đã thêm bài học tiên quyết.");
    if (ok) setRequiredLessonId("");
  }

  return (
    <div className="space-y-4">
      <form onSubmit={submit}>
        <FormSection title="Thêm LessonPrerequisite" description="Backend sẽ kiểm tra trùng lặp, self-reference và vòng lặp prerequisite." icon={<Link2 size={18} />}>
          <FormRow columns={2}>
            <FormField label="Required Lesson ID" required><Input type="number" min={1} value={requiredLessonId} onChange={(event) => setRequiredLessonId(event.target.value)} /></FormField>
            <div className="flex items-end justify-end"><Button type="submit" disabled={busy} className="gap-2"><Plus size={14} /> Thêm tiên quyết</Button></div>
          </FormRow>
        </FormSection>
      </form>

      {items.length === 0 ? (
        <EmptyState title="Không có bài học tiên quyết" description="Người học có thể học bài này mà không cần hoàn thành bài khác trước." />
      ) : (
        <div className="space-y-2">
          {items.map((item) => (
            <Card key={item.requiredLessonId}>
              <CardContent className="flex items-center justify-between gap-3 py-3">
                <div className="min-w-0"><p className="text-[12px] font-semibold text-[#333]">{item.titleVi}</p><p className="mt-1 text-[10px] text-[#888]">{item.slug} · ID {item.requiredLessonId}</p></div>
                <Button variant="outline" disabled={busy} onClick={() => setDeleting(item)} className="gap-2 text-[#e2372f]"><Trash2 size={14} /> Gỡ</Button>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      <ConfirmDeleteDialog
        open={Boolean(deleting)}
        title="Gỡ bài học tiên quyết?"
        description="Quan hệ prerequisite sẽ bị xóa, bài giảng gốc vẫn được giữ nguyên."
        busy={busy}
        onClose={() => setDeleting(null)}
        onConfirm={async () => {
          if (!deleting) return;
          const ok = await run(() => lessonApi.removePrerequisite(lessonId, deleting.requiredLessonId), "Đã gỡ bài học tiên quyết.");
          if (ok) setDeleting(null);
        }}
      />
    </div>
  );
}

function ContentActions({
  busy,
  canMoveUp,
  canMoveDown,
  onMoveUp,
  onMoveDown,
  onEdit,
  onDelete,
}: {
  busy: boolean;
  canMoveUp: boolean;
  canMoveDown: boolean;
  onMoveUp: () => void;
  onMoveDown: () => void;
  onEdit: () => void;
  onDelete: () => void;
}) {
  return (
    <div className="flex shrink-0 flex-wrap items-center justify-end gap-1">
      <Button variant="outline" disabled={busy || !canMoveUp} onClick={onMoveUp} aria-label="Đưa lên"><ArrowUp size={14} /></Button>
      <Button variant="outline" disabled={busy || !canMoveDown} onClick={onMoveDown} aria-label="Đưa xuống"><ArrowDown size={14} /></Button>
      <Button variant="outline" disabled={busy} onClick={onEdit} className="gap-2"><Pencil size={14} /> Sửa</Button>
      <Button variant="outline" disabled={busy} onClick={onDelete} className="gap-2 text-[#e2372f]"><Trash2 size={14} /> Xóa</Button>
    </div>
  );
}

function SectionEditDialog({
  item,
  busy,
  onClose,
  onSave,
}: {
  item: AdminLessonSection | null;
  busy: boolean;
  onClose: () => void;
  onSave: (item: AdminLessonSection, request: Parameters<typeof lessonApi.updateSection>[2]) => Promise<void>;
}) {
  const [sectionType, setSectionType] = useState(LessonSectionType.Introduction);
  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [sortOrder, setSortOrder] = useState(0);
  const [seconds, setSeconds] = useState(0);
  const [required, setRequired] = useState(false);

  useEffect(() => {
    if (!item) return;
    setSectionType(item.sectionType);
    setTitle(item.titleVi ?? "");
    setContent(item.contentVi ?? "");
    setSortOrder(item.sortOrder);
    setSeconds(item.estimatedSeconds ?? 0);
    setRequired(item.isRequired);
  }, [item]);

  return (
    <Dialog
      open={Boolean(item)}
      onOpenChange={(open) => { if (!open) onClose(); }}
      title="Chỉnh sửa section"
      description="Cập nhật nội dung và thứ tự section."
      size="lg"
      footer={
        <div className="flex justify-end gap-2">
          <Button variant="outline" onClick={onClose} disabled={busy}>Hủy</Button>
          <Button disabled={busy || !item} onClick={() => item && void onSave(item, { sectionType, titleVi: title.trim() || null, contentVi: content.trim() || null, sortOrder, estimatedSeconds: seconds || null, isRequired: required })}>Lưu thay đổi</Button>
        </div>
      }
    >
      <div className="space-y-4">
        <FormRow columns={2}>
          <FormField label="Loại section"><Select value={String(sectionType)} onValueChange={(value) => setSectionType(Number(value) as LessonSectionType)} options={Object.entries(lessonSectionTypeLabels).map(([value, label]) => ({ value, label }))} /></FormField>
          <FormField label="Tiêu đề"><Input value={title} onChange={(event) => setTitle(event.target.value)} /></FormField>
        </FormRow>
        <FormRow columns={2}>
          <FormField label="Thứ tự"><Input type="number" min={0} value={sortOrder} onChange={(event) => setSortOrder(Number(event.target.value))} /></FormField>
          <FormField label="Thời lượng (giây)"><Input type="number" min={0} value={seconds} onChange={(event) => setSeconds(Number(event.target.value))} /></FormField>
        </FormRow>
        <FormField label="Nội dung"><Textarea rows={7} value={content} onChange={(event) => setContent(event.target.value)} /></FormField>
        <FormField label="Yêu cầu"><Switch checked={required} onCheckedChange={setRequired} label={required ? "Bắt buộc" : "Không bắt buộc"} /></FormField>
      </div>
    </Dialog>
  );
}

function VocabularyEditDialog({ item, busy, onClose, onSave }: {
  item: AdminLessonVocabulary | null;
  busy: boolean;
  onClose: () => void;
  onSave: (item: AdminLessonVocabulary, sortOrder: number, required: boolean) => Promise<void>;
}) {
  const [sortOrder, setSortOrder] = useState(0);
  const [required, setRequired] = useState(false);

  useEffect(() => {
    if (!item) return;
    setSortOrder(item.sortOrder);
    setRequired(item.isRequired);
  }, [item]);

  return (
    <Dialog
      open={Boolean(item)}
      onOpenChange={(open) => { if (!open) onClose(); }}
      title="Chỉnh sửa từ vựng trong bài giảng"
      description={item ? `${item.simplified} · ${item.pinyin}` : undefined}
      footer={<div className="flex justify-end gap-2"><Button variant="outline" onClick={onClose} disabled={busy}>Hủy</Button><Button disabled={busy || !item} onClick={() => item && void onSave(item, sortOrder, required)}>Lưu thay đổi</Button></div>}
    >
      <FormRow columns={2}>
        <FormField label="Thứ tự"><Input type="number" min={0} value={sortOrder} onChange={(event) => setSortOrder(Number(event.target.value))} /></FormField>
        <FormField label="Yêu cầu"><Switch checked={required} onCheckedChange={setRequired} label={required ? "Bắt buộc" : "Không bắt buộc"} /></FormField>
      </FormRow>
    </Dialog>
  );
}

function AssetEditDialog({ item, busy, onClose, onSave }: {
  item: AdminLessonAsset | null;
  busy: boolean;
  onClose: () => void;
  onSave: (item: AdminLessonAsset, request: Parameters<typeof lessonApi.updateAsset>[2]) => Promise<void>;
}) {
  const [url, setUrl] = useState("");
  const [caption, setCaption] = useState("");
  const [audioAssetId, setAudioAssetId] = useState("");
  const [sortOrder, setSortOrder] = useState(0);

  useEffect(() => {
    if (!item) return;
    setUrl(item.url ?? "");
    setCaption(item.captionVi ?? "");
    setAudioAssetId(item.audioAssetId ? String(item.audioAssetId) : "");
    setSortOrder(item.sortOrder);
  }, [item]);

  const save = () => {
    if (!item) return;
    const audioId = audioAssetId ? Number(audioAssetId) : null;
    if (audioId !== null && (!Number.isSafeInteger(audioId) || audioId <= 0)) {
      toast.error("AudioAsset ID không hợp lệ.");
      return;
    }
    void onSave(item, { url: url.trim() || null, captionVi: caption.trim() || null, audioAssetId: audioId, sortOrder });
  };

  return (
    <Dialog
      open={Boolean(item)}
      onOpenChange={(open) => { if (!open) onClose(); }}
      title="Chỉnh sửa tài nguyên"
      description={item ? lessonAssetTypeLabels[item.assetType] : undefined}
      footer={<div className="flex justify-end gap-2"><Button variant="outline" onClick={onClose} disabled={busy}>Hủy</Button><Button disabled={busy || !item} onClick={save}>Lưu thay đổi</Button></div>}
    >
      <div className="space-y-4">
        <FormRow columns={2}>
          <FormField label="Thứ tự"><Input type="number" min={0} value={sortOrder} onChange={(event) => setSortOrder(Number(event.target.value))} /></FormField>
          <FormField label="AudioAsset ID"><Input type="number" min={1} value={audioAssetId} onChange={(event) => setAudioAssetId(event.target.value)} /></FormField>
        </FormRow>
        <FormField label="URL"><Input value={url} onChange={(event) => setUrl(event.target.value)} /></FormField>
        <FormField label="Chú thích"><Textarea rows={4} value={caption} onChange={(event) => setCaption(event.target.value)} /></FormField>
      </div>
    </Dialog>
  );
}

function ConfirmDeleteDialog({ open, title, description, busy, onClose, onConfirm }: {
  open: boolean;
  title: string;
  description: string;
  busy: boolean;
  onClose: () => void;
  onConfirm: () => Promise<void>;
}) {
  return (
    <Dialog
      open={open}
      onOpenChange={(next) => { if (!next) onClose(); }}
      title={title}
      description={description}
      size="sm"
      footer={
        <div className="flex justify-end gap-2">
          <Button variant="outline" onClick={onClose} disabled={busy}>Hủy</Button>
          <Button onClick={() => void onConfirm()} disabled={busy} className="gap-2"><Trash2 size={14} /> Xác nhận</Button>
        </div>
      }
    >
      <p className="text-[12px] leading-5 text-[#666]">Hãy kiểm tra đúng nội dung trước khi xác nhận thao tác.</p>
    </Dialog>
  );
}

function sectionRequest(item: AdminLessonSection, sortOrder: number) {
  return {
    sectionType: item.sectionType,
    titleVi: item.titleVi ?? null,
    contentVi: item.contentVi ?? null,
    sortOrder,
    isRequired: item.isRequired,
    estimatedSeconds: item.estimatedSeconds ?? null,
  };
}

function assetRequest(item: AdminLessonAsset, sortOrder: number) {
  return {
    url: item.url ?? null,
    captionVi: item.captionVi ?? null,
    audioAssetId: item.audioAssetId ?? null,
    sortOrder,
  };
}
