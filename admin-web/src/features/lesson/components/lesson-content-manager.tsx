"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { BookOpen, FileText, ImageIcon, Link2, Plus, RefreshCw, Trash2 } from "lucide-react";
import { toast } from "sonner";

import { EmptyState } from "@/components/common/empty-state";
import { FormField } from "@/components/forms/form-field";
import { FormRow } from "@/components/forms/form-row";
import { FormSection } from "@/components/forms/form-section";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
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

interface LessonContentManagerProps { lessonId: number; }
interface PanelProps<T> {
  lessonId: number;
  items: T[];
  busy: boolean;
  run: (action: () => Promise<unknown>, success: string) => Promise<void>;
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

  useEffect(() => { void loadAll(); }, [loadAll]);

  async function run(action: () => Promise<unknown>, success: string) {
    setBusy(true);
    try {
      await action();
      toast.success(success);
      await loadAll();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Thao tác thất bại.");
    } finally {
      setBusy(false);
    }
  }

  if (loading) {
    return <div className="space-y-3"><Skeleton className="h-12 w-full rounded-[11px]" /><Skeleton className="h-72 w-full rounded-[11px]" /></div>;
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

      {tab === "sections" ? <SectionsPanel lessonId={lessonId} items={sections} busy={busy} run={run} /> : null}
      {tab === "vocabulary" ? <VocabularyPanel lessonId={lessonId} items={vocabulary} busy={busy} run={run} /> : null}
      {tab === "assets" ? <AssetsPanel lessonId={lessonId} items={assets} busy={busy} run={run} /> : null}
      {tab === "prerequisites" ? <PrerequisitesPanel lessonId={lessonId} items={prerequisites} busy={busy} run={run} /> : null}
    </div>
  );
}

function SectionsPanel({ lessonId, items, busy, run }: PanelProps<AdminLessonSection>) {
  const [sectionType, setSectionType] = useState(LessonSectionType.Introduction);
  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [sortOrder, setSortOrder] = useState(items.length);
  const [estimatedSeconds, setEstimatedSeconds] = useState(120);
  const [required, setRequired] = useState(true);

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    await run(() => lessonApi.createSection(lessonId, {
      sectionType,
      titleVi: title.trim() || null,
      contentVi: content.trim() || null,
      sortOrder,
      estimatedSeconds: estimatedSeconds || null,
      isRequired: required,
    }), "Đã thêm phần nội dung.");
    setTitle(""); setContent(""); setSortOrder(items.length + 1);
  }

  return (
    <div className="space-y-4">
      <form onSubmit={submit}>
        <FormSection title="Thêm LessonSection" description="Chia bài giảng thành các phần học có thứ tự, thời lượng và trạng thái bắt buộc." icon={<BookOpen size={18} />}>
          <FormRow columns={2}>
            <FormField label="Loại section" required>
              <Select
                value={String(sectionType)}
                onValueChange={(value) => setSectionType(Number(value) as LessonSectionType)}
                options={Object.entries(lessonSectionTypeLabels).map(([value, label]) => ({ value, label }))}
              />
            </FormField>
            <FormField label="Tiêu đề"><Input value={title} onChange={(e) => setTitle(e.target.value)} /></FormField>
          </FormRow>
          <FormRow columns={2}>
            <FormField label="Thứ tự"><Input type="number" min={0} value={sortOrder} onChange={(e) => setSortOrder(Number(e.target.value))} /></FormField>
            <FormField label="Thời lượng (giây)"><Input type="number" min={0} value={estimatedSeconds} onChange={(e) => setEstimatedSeconds(Number(e.target.value))} /></FormField>
          </FormRow>
          <FormField label="Nội dung"><Textarea value={content} onChange={(e) => setContent(e.target.value)} rows={6} /></FormField>
          <FormRow columns={2}>
            <FormField label="Yêu cầu hoàn thành"><Switch checked={required} onCheckedChange={setRequired} label={required ? "Bắt buộc" : "Không bắt buộc"} /></FormField>
            <div className="flex items-end justify-end"><Button type="submit" disabled={busy} className="gap-2"><Plus size={14} /> Thêm section</Button></div>
          </FormRow>
        </FormSection>
      </form>

      {items.length === 0 ? <EmptyState title="Chưa có section" description="Bài giảng chưa có phần nội dung nào." /> : (
        <div className="space-y-2">
          {[...items].sort((a, b) => a.sortOrder - b.sortOrder).map((item, index) => (
            <Card key={item.id}>
              <CardHeader className="flex flex-row items-start justify-between gap-3 py-3">
                <div className="min-w-0"><CardTitle>{index + 1}. {item.titleVi || lessonSectionTypeLabels[item.sectionType]}</CardTitle><div className="mt-2 flex flex-wrap gap-2"><Badge>{lessonSectionTypeLabels[item.sectionType]}</Badge>{item.isRequired ? <Badge variant="warning">Bắt buộc</Badge> : null}<Badge variant="info">#{item.sortOrder} · {item.estimatedSeconds ?? 0}s</Badge></div></div>
                <DeleteButton disabled={busy} onClick={() => void run(() => lessonApi.deleteSection(lessonId, item.id), "Đã xóa section.")} />
              </CardHeader>
              {item.contentVi ? <CardContent><p className="whitespace-pre-wrap text-[12px] leading-5 text-[#666]">{item.contentVi}</p></CardContent> : null}
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}

function VocabularyPanel({ lessonId, items, busy, run }: PanelProps<AdminLessonVocabulary>) {
  const [vocabularyId, setVocabularyId] = useState("");
  const [sortOrder, setSortOrder] = useState(items.length);
  const [required, setRequired] = useState(true);

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const id = Number(vocabularyId);
    if (!Number.isSafeInteger(id) || id <= 0) return toast.error("Vocabulary ID không hợp lệ.");
    await run(() => lessonApi.attachVocabulary(lessonId, { vocabularyId: id, sortOrder, isRequired: required }), "Đã gắn từ vựng.");
    setVocabularyId(""); setSortOrder(items.length + 1);
  }

  return (
    <div className="space-y-4">
      <form onSubmit={submit}>
        <FormSection title="Gắn LessonVocabulary" description="Gắn từ vựng đã tồn tại trong kho Vocabulary vào bài giảng." icon={<Link2 size={18} />}>
          <FormRow columns={3}>
            <FormField label="Vocabulary ID" required><Input type="number" min={1} value={vocabularyId} onChange={(e) => setVocabularyId(e.target.value)} /></FormField>
            <FormField label="Thứ tự"><Input type="number" min={0} value={sortOrder} onChange={(e) => setSortOrder(Number(e.target.value))} /></FormField>
            <FormField label="Yêu cầu"><Switch checked={required} onCheckedChange={setRequired} label={required ? "Bắt buộc" : "Không bắt buộc"} /></FormField>
          </FormRow>
          <div className="flex justify-end"><Button type="submit" disabled={busy} className="gap-2"><Plus size={14} /> Gắn từ vựng</Button></div>
        </FormSection>
      </form>

      {items.length === 0 ? <EmptyState title="Chưa có từ vựng" description="Bài giảng chưa gắn từ vựng nào." /> : (
        <div className="space-y-2">
          {[...items].sort((a, b) => a.sortOrder - b.sortOrder).map((item) => (
            <Card key={item.vocabularyId}><CardContent className="flex items-center justify-between gap-3 py-3"><div className="min-w-0"><div className="flex flex-wrap items-center gap-2"><span className="text-[16px] font-semibold text-[#333]">{item.simplified}</span><Badge>{item.pinyin}</Badge>{item.isRequired ? <Badge variant="warning">Bắt buộc</Badge> : null}</div><p className="mt-1 text-[11px] text-[#666]">{item.primaryMeaningVi} · ID {item.vocabularyId} · PublicId {item.vocabularyPublicId.slice(0, 8)}…</p></div><DeleteButton disabled={busy} onClick={() => void run(() => lessonApi.detachVocabulary(lessonId, item.vocabularyId), "Đã gỡ từ vựng.")} /></CardContent></Card>
          ))}
        </div>
      )}
    </div>
  );
}

function AssetsPanel({ lessonId, items, busy, run }: PanelProps<AdminLessonAsset>) {
  const [assetType, setAssetType] = useState(LessonAssetType.Image);
  const [url, setUrl] = useState("");
  const [caption, setCaption] = useState("");
  const [audioAssetId, setAudioAssetId] = useState("");
  const [sortOrder, setSortOrder] = useState(items.length);

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    await run(() => lessonApi.createAsset(lessonId, {
      assetType,
      url: url.trim() || null,
      captionVi: caption.trim() || null,
      audioAssetId: audioAssetId ? Number(audioAssetId) : null,
      sortOrder,
    }), "Đã thêm tài nguyên.");
    setUrl(""); setCaption(""); setAudioAssetId(""); setSortOrder(items.length + 1);
  }

  return (
    <div className="space-y-4">
      <form onSubmit={submit}>
        <FormSection title="Thêm LessonAsset" description="Quản lý hình ảnh, audio và tài liệu gắn trực tiếp với bài giảng." icon={<ImageIcon size={18} />}>
          <FormRow columns={2}>
            <FormField label="Loại tài nguyên"><Select value={String(assetType)} onValueChange={(value) => setAssetType(Number(value) as LessonAssetType)} options={Object.entries(lessonAssetTypeLabels).map(([value, label]) => ({ value, label }))} /></FormField>
            <FormField label="Thứ tự"><Input type="number" min={0} value={sortOrder} onChange={(e) => setSortOrder(Number(e.target.value))} /></FormField>
          </FormRow>
          <FormRow columns={2}>
            <FormField label="URL"><Input value={url} onChange={(e) => setUrl(e.target.value)} placeholder="https://..." /></FormField>
            <FormField label="AudioAsset ID"><Input type="number" min={1} value={audioAssetId} onChange={(e) => setAudioAssetId(e.target.value)} placeholder="Chỉ dùng khi asset là audio nội bộ" /></FormField>
          </FormRow>
          <FormField label="Chú thích"><Textarea value={caption} onChange={(e) => setCaption(e.target.value)} rows={3} /></FormField>
          <div className="flex justify-end"><Button type="submit" disabled={busy} className="gap-2"><Plus size={14} /> Thêm tài nguyên</Button></div>
        </FormSection>
      </form>

      {items.length === 0 ? <EmptyState title="Chưa có tài nguyên" description="Bài giảng chưa có hình ảnh, audio hoặc tài liệu." /> : (
        <div className="space-y-2">{[...items].sort((a, b) => a.sortOrder - b.sortOrder).map((item) => (
          <Card key={item.id}><CardContent className="flex items-center justify-between gap-3 py-3"><div className="min-w-0"><div className="flex flex-wrap gap-2"><Badge variant="info">{lessonAssetTypeLabels[item.assetType]}</Badge><Badge>#{item.sortOrder}</Badge></div><p className="mt-1 truncate text-[11px] text-[#666]">{item.captionVi || item.url || `AudioAsset #${item.audioAssetId ?? "—"}`}</p><p className="mt-1 text-[10px] text-[#999]">ID {item.id} · PublicId {item.publicId.slice(0, 8)}…</p></div><DeleteButton disabled={busy} onClick={() => void run(() => lessonApi.deleteAsset(lessonId, item.id), "Đã xóa tài nguyên.")} /></CardContent></Card>
        ))}</div>
      )}
    </div>
  );
}

function PrerequisitesPanel({ lessonId, items, busy, run }: PanelProps<AdminLessonPrerequisite>) {
  const [requiredLessonId, setRequiredLessonId] = useState("");

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const id = Number(requiredLessonId);
    if (!Number.isSafeInteger(id) || id <= 0 || id === lessonId) return toast.error("Required Lesson ID không hợp lệ.");
    await run(() => lessonApi.addPrerequisite(lessonId, { requiredLessonId: id }), "Đã thêm bài giảng tiên quyết.");
    setRequiredLessonId("");
  }

  return (
    <div className="space-y-4">
      <form onSubmit={submit}>
        <FormSection title="Thêm LessonPrerequisite" description="Yêu cầu người học hoàn thành bài giảng khác trước bài giảng hiện tại." icon={<FileText size={18} />}>
          <FormRow columns={2}>
            <FormField label="Required Lesson ID" required><Input type="number" min={1} value={requiredLessonId} onChange={(e) => setRequiredLessonId(e.target.value)} /></FormField>
            <div className="flex items-end justify-end"><Button type="submit" disabled={busy || !requiredLessonId} className="gap-2"><Plus size={14} /> Thêm tiên quyết</Button></div>
          </FormRow>
        </FormSection>
      </form>

      {items.length === 0 ? <EmptyState title="Chưa có bài giảng tiên quyết" description="Bài giảng này chưa yêu cầu hoàn thành bài giảng nào trước đó." /> : (
        <div className="space-y-2">{items.map((item) => (
          <Card key={item.requiredLessonId}><CardContent className="flex items-center justify-between gap-3 py-3"><div className="min-w-0"><div className="text-[12px] font-semibold text-[#333]">{item.titleVi}</div><p className="mt-1 text-[10px] text-[#888]">/{item.slug} · ID {item.requiredLessonId} · PublicId {item.requiredLessonPublicId.slice(0, 8)}…</p></div><DeleteButton disabled={busy} onClick={() => void run(() => lessonApi.removePrerequisite(lessonId, item.requiredLessonId), "Đã gỡ bài giảng tiên quyết.")} /></CardContent></Card>
        ))}</div>
      )}
    </div>
  );
}

function DeleteButton({ disabled, onClick }: { disabled: boolean; onClick: () => void }) {
  return <Button type="button" variant="ghost" size="icon" disabled={disabled} onClick={onClick} aria-label="Xóa"><Trash2 size={15} /></Button>;
}
