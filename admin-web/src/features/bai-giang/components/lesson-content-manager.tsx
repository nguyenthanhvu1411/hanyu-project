"use client";

import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";
import { BookOpen, FileText, Image, Link2, Plus, RefreshCw, Trash2 } from "lucide-react";
import { toast } from "sonner";

import { Button } from "@/components/ui/button";
import { baiGiangApi } from "../api/bai-giang.api";
import {
  AdminLessonAsset,
  AdminLessonPrerequisite,
  AdminLessonSection,
  AdminLessonVocabulary,
  LessonAssetType,
  LessonSectionType,
  lessonAssetTypeLabels,
  lessonSectionTypeLabels,
} from "../types/bai-giang.types";

type TabKey = "sections" | "vocabulary" | "assets" | "prerequisites";

interface LessonContentManagerProps {
  lessonId: number;
}

const fieldClass =
  "h-10 w-full rounded-md border border-border bg-background px-3 text-[13px] outline-none transition focus:border-primary";
const textareaClass =
  "min-h-28 w-full rounded-md border border-border bg-background px-3 py-2 text-[13px] outline-none transition focus:border-primary";

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
        baiGiangApi.danhSachPhan(lessonId),
        baiGiangApi.danhSachTuVung(lessonId),
        baiGiangApi.danhSachTaiNguyen(lessonId),
        baiGiangApi.danhSachTienQuyet(lessonId),
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

  const tabs = useMemo(
    () => [
      { key: "sections" as const, label: "Nội dung", count: sections.length, icon: BookOpen },
      { key: "vocabulary" as const, label: "Từ vựng", count: vocabulary.length, icon: Link2 },
      { key: "assets" as const, label: "Tài nguyên", count: assets.length, icon: Image },
      { key: "prerequisites" as const, label: "Tiên quyết", count: prerequisites.length, icon: FileText },
    ],
    [assets.length, prerequisites.length, sections.length, vocabulary.length],
  );

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
    return <div className="rounded-lg border border-border bg-card p-8 text-center text-[13px] text-muted-foreground">Đang tải nội dung bài giảng...</div>;
  }

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3 rounded-lg border border-border bg-card p-3">
        <div className="flex flex-wrap gap-2">
          {tabs.map((item) => {
            const Icon = item.icon;
            return (
              <button
                key={item.key}
                type="button"
                onClick={() => setTab(item.key)}
                className={`flex h-9 items-center gap-2 rounded-md px-3 text-[12px] font-medium transition ${
                  tab === item.key ? "bg-primary text-primary-foreground" : "bg-muted text-muted-foreground hover:text-foreground"
                }`}
              >
                <Icon size={14} />
                {item.label}
                <span className="rounded bg-background/20 px-1.5 py-0.5 text-[10px]">{item.count}</span>
              </button>
            );
          })}
        </div>
        <Button variant="outline" className="h-9 gap-2 text-[11px]" onClick={() => void loadAll()} disabled={busy}>
          <RefreshCw size={13} /> Làm mới
        </Button>
      </div>

      {tab === "sections" && (
        <SectionsPanel lessonId={lessonId} items={sections} busy={busy} run={run} />
      )}
      {tab === "vocabulary" && (
        <VocabularyPanel lessonId={lessonId} items={vocabulary} busy={busy} run={run} />
      )}
      {tab === "assets" && (
        <AssetsPanel lessonId={lessonId} items={assets} busy={busy} run={run} />
      )}
      {tab === "prerequisites" && (
        <PrerequisitesPanel lessonId={lessonId} items={prerequisites} busy={busy} run={run} />
      )}
    </div>
  );
}

interface PanelProps<T> {
  lessonId: number;
  items: T[];
  busy: boolean;
  run: (action: () => Promise<unknown>, success: string) => Promise<void>;
}

function SectionsPanel({ lessonId, items, busy, run }: PanelProps<AdminLessonSection>) {
  const [sectionType, setSectionType] = useState(LessonSectionType.Introduction);
  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [sortOrder, setSortOrder] = useState(items.length);
  const [estimatedSeconds, setEstimatedSeconds] = useState(120);
  const [required, setRequired] = useState(true);

  async function submit(event: FormEvent) {
    event.preventDefault();
    await run(
      () => baiGiangApi.taoPhan(lessonId, {
        sectionType,
        titleVi: title || null,
        contentVi: content || null,
        sortOrder,
        estimatedSeconds: estimatedSeconds || null,
        isRequired: required,
      }),
      "Đã thêm phần nội dung.",
    );
    setTitle("");
    setContent("");
    setSortOrder(items.length + 1);
  }

  return (
    <PanelShell title="LessonSection" description="Chia bài giảng thành các phần học có thứ tự, thời lượng và trạng thái bắt buộc.">
      <form onSubmit={submit} className="grid gap-3 rounded-lg border border-border bg-muted/30 p-4 md:grid-cols-12">
        <Field label="Loại" className="md:col-span-3">
          <select className={fieldClass} value={sectionType} onChange={(e) => setSectionType(Number(e.target.value) as LessonSectionType)}>
            {Object.entries(lessonSectionTypeLabels).map(([value, label]) => <option key={value} value={value}>{label}</option>)}
          </select>
        </Field>
        <Field label="Tiêu đề" className="md:col-span-5"><input className={fieldClass} value={title} onChange={(e) => setTitle(e.target.value)} placeholder="Ví dụ: Từ vựng trọng tâm" /></Field>
        <Field label="Thứ tự" className="md:col-span-2"><input className={fieldClass} type="number" min={0} value={sortOrder} onChange={(e) => setSortOrder(Number(e.target.value))} /></Field>
        <Field label="Thời lượng (giây)" className="md:col-span-2"><input className={fieldClass} type="number" min={0} value={estimatedSeconds} onChange={(e) => setEstimatedSeconds(Number(e.target.value))} /></Field>
        <Field label="Nội dung" className="md:col-span-12"><textarea className={textareaClass} value={content} onChange={(e) => setContent(e.target.value)} placeholder="Nội dung tiếng Việt của section..." /></Field>
        <label className="flex items-center gap-2 text-[12px] md:col-span-8"><input type="checkbox" checked={required} onChange={(e) => setRequired(e.target.checked)} /> Bắt buộc hoàn thành</label>
        <div className="flex justify-end md:col-span-4"><Button className="h-9 gap-2 text-[11px]" disabled={busy}><Plus size={13} /> Thêm section</Button></div>
      </form>

      <div className="space-y-2">
        {items.length === 0 ? <EmptyState text="Chưa có section nào." /> : items.sort((a, b) => a.sortOrder - b.sortOrder).map((item, index) => (
          <div key={item.id} className="flex gap-3 rounded-lg border border-border bg-card p-4">
            <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-md bg-muted text-[12px] font-semibold">{index + 1}</div>
            <div className="min-w-0 flex-1">
              <div className="flex flex-wrap items-center gap-2">
                <strong className="text-[13px]">{item.titleVi || lessonSectionTypeLabels[item.sectionType]}</strong>
                <Badge>{lessonSectionTypeLabels[item.sectionType]}</Badge>
                {item.isRequired && <Badge>Bắt buộc</Badge>}
                <span className="text-[11px] text-muted-foreground">#{item.sortOrder} · {item.estimatedSeconds ?? 0}s</span>
              </div>
              {item.contentVi && <p className="mt-2 line-clamp-3 whitespace-pre-wrap text-[12px] leading-5 text-muted-foreground">{item.contentVi}</p>}
            </div>
            <IconDelete disabled={busy} onClick={() => void run(() => baiGiangApi.xoaPhan(lessonId, item.id), "Đã xóa section.")} />
          </div>
        ))}
      </div>
    </PanelShell>
  );
}

function VocabularyPanel({ lessonId, items, busy, run }: PanelProps<AdminLessonVocabulary>) {
  const [vocabularyId, setVocabularyId] = useState(0);
  const [sortOrder, setSortOrder] = useState(items.length);
  const [required, setRequired] = useState(true);

  async function submit(event: FormEvent) {
    event.preventDefault();
    if (vocabularyId <= 0) return toast.error("Nhập Vocabulary ID hợp lệ.");
    await run(() => baiGiangApi.ganTuVung(lessonId, { vocabularyId, sortOrder, isRequired: required }), "Đã gắn từ vựng vào bài giảng.");
    setVocabularyId(0);
    setSortOrder(items.length + 1);
  }

  return (
    <PanelShell title="LessonVocabulary" description="Gắn từ vựng đã có trong kho Vocabulary vào bài giảng và xác định thứ tự học.">
      <form onSubmit={submit} className="grid gap-3 rounded-lg border border-border bg-muted/30 p-4 md:grid-cols-12">
        <Field label="Vocabulary ID" className="md:col-span-5"><input className={fieldClass} type="number" min={1} value={vocabularyId || ""} onChange={(e) => setVocabularyId(Number(e.target.value))} placeholder="ID từ vựng" /></Field>
        <Field label="Thứ tự" className="md:col-span-3"><input className={fieldClass} type="number" min={0} value={sortOrder} onChange={(e) => setSortOrder(Number(e.target.value))} /></Field>
        <label className="flex items-end gap-2 pb-2 text-[12px] md:col-span-2"><input type="checkbox" checked={required} onChange={(e) => setRequired(e.target.checked)} /> Bắt buộc</label>
        <div className="flex items-end justify-end md:col-span-2"><Button className="h-9 gap-2 text-[11px]" disabled={busy}><Plus size={13} /> Gắn</Button></div>
      </form>
      <div className="overflow-hidden rounded-lg border border-border">
        <table className="w-full text-left text-[12px]">
          <thead className="bg-muted/60 text-muted-foreground"><tr><th className="px-3 py-2">STT</th><th className="px-3 py-2">Hán tự</th><th className="px-3 py-2">Pinyin</th><th className="px-3 py-2">Nghĩa</th><th className="px-3 py-2">Bắt buộc</th><th className="w-12 px-3 py-2" /></tr></thead>
          <tbody>{items.length === 0 ? <tr><td colSpan={6} className="p-6 text-center text-muted-foreground">Chưa gắn từ vựng.</td></tr> : [...items].sort((a,b)=>a.sortOrder-b.sortOrder).map((item, index) => (
            <tr key={item.vocabularyId} className="border-t border-border"><td className="px-3 py-2">{index + 1}</td><td className="px-3 py-2 text-[15px] font-semibold">{item.simplified}</td><td className="px-3 py-2">{item.pinyin}</td><td className="px-3 py-2">{item.primaryMeaningVi}</td><td className="px-3 py-2">{item.isRequired ? "Có" : "Không"}</td><td className="px-3 py-2"><IconDelete disabled={busy} onClick={() => void run(() => baiGiangApi.goTuVung(lessonId, item.vocabularyId), "Đã gỡ từ vựng.")} /></td></tr>
          ))}</tbody>
        </table>
      </div>
    </PanelShell>
  );
}

function AssetsPanel({ lessonId, items, busy, run }: PanelProps<AdminLessonAsset>) {
  const [assetType, setAssetType] = useState(LessonAssetType.Image);
  const [url, setUrl] = useState("");
  const [caption, setCaption] = useState("");
  const [audioAssetId, setAudioAssetId] = useState<number | null>(null);
  const [sortOrder, setSortOrder] = useState(items.length);

  async function submit(event: FormEvent) {
    event.preventDefault();
    await run(() => baiGiangApi.taoTaiNguyen(lessonId, { assetType, url: url || null, captionVi: caption || null, audioAssetId, sortOrder }), "Đã thêm tài nguyên.");
    setUrl(""); setCaption(""); setAudioAssetId(null); setSortOrder(items.length + 1);
  }

  return (
    <PanelShell title="LessonAsset" description="Quản lý hình ảnh, audio và tài liệu gắn trực tiếp với bài giảng.">
      <form onSubmit={submit} className="grid gap-3 rounded-lg border border-border bg-muted/30 p-4 md:grid-cols-12">
        <Field label="Loại" className="md:col-span-2"><select className={fieldClass} value={assetType} onChange={(e) => setAssetType(Number(e.target.value) as LessonAssetType)}>{Object.entries(lessonAssetTypeLabels).map(([value,label])=><option key={value} value={value}>{label}</option>)}</select></Field>
        <Field label="URL" className="md:col-span-4"><input className={fieldClass} value={url} onChange={(e)=>setUrl(e.target.value)} placeholder="https://..." /></Field>
        <Field label="Caption" className="md:col-span-3"><input className={fieldClass} value={caption} onChange={(e)=>setCaption(e.target.value)} placeholder="Mô tả tài nguyên" /></Field>
        <Field label="Audio Asset ID" className="md:col-span-2"><input className={fieldClass} type="number" min={1} value={audioAssetId ?? ""} onChange={(e)=>setAudioAssetId(e.target.value ? Number(e.target.value) : null)} /></Field>
        <Field label="Thứ tự" className="md:col-span-1"><input className={fieldClass} type="number" min={0} value={sortOrder} onChange={(e)=>setSortOrder(Number(e.target.value))} /></Field>
        <div className="flex justify-end md:col-span-12"><Button className="h-9 gap-2 text-[11px]" disabled={busy}><Plus size={13}/> Thêm tài nguyên</Button></div>
      </form>
      <div className="grid gap-3 md:grid-cols-2 xl:grid-cols-3">{items.length === 0 ? <div className="md:col-span-2 xl:col-span-3"><EmptyState text="Chưa có tài nguyên." /></div> : [...items].sort((a,b)=>a.sortOrder-b.sortOrder).map((item)=>(
        <div key={item.id} className="rounded-lg border border-border bg-card p-4"><div className="flex items-start justify-between gap-3"><div><Badge>{lessonAssetTypeLabels[item.assetType]}</Badge><p className="mt-2 text-[13px] font-medium">{item.captionVi || "Không có caption"}</p></div><IconDelete disabled={busy} onClick={() => void run(() => baiGiangApi.xoaTaiNguyen(lessonId,item.id), "Đã xóa tài nguyên.")} /></div>{item.url && <a href={item.url} target="_blank" rel="noreferrer" className="mt-3 block truncate text-[11px] text-primary underline">{item.url}</a>}<p className="mt-2 text-[11px] text-muted-foreground">Thứ tự: {item.sortOrder}{item.audioAssetId ? ` · Audio #${item.audioAssetId}` : ""}</p></div>
      ))}</div>
    </PanelShell>
  );
}

function PrerequisitesPanel({ lessonId, items, busy, run }: PanelProps<AdminLessonPrerequisite>) {
  const [requiredLessonId, setRequiredLessonId] = useState(0);

  async function submit(event: FormEvent) {
    event.preventDefault();
    if (requiredLessonId <= 0 || requiredLessonId === lessonId) return toast.error("Lesson ID tiên quyết không hợp lệ.");
    await run(() => baiGiangApi.themTienQuyet(lessonId, { requiredLessonId }), "Đã thêm bài học tiên quyết.");
    setRequiredLessonId(0);
  }

  return (
    <PanelShell title="LessonPrerequisite" description="Xác định các bài học cần hoàn thành trước khi học bài hiện tại.">
      <form onSubmit={submit} className="flex flex-wrap items-end gap-3 rounded-lg border border-border bg-muted/30 p-4">
        <Field label="Required Lesson ID" className="min-w-64 flex-1"><input className={fieldClass} type="number" min={1} value={requiredLessonId || ""} onChange={(e)=>setRequiredLessonId(Number(e.target.value))} placeholder="ID bài học tiên quyết" /></Field>
        <Button className="h-10 gap-2 text-[11px]" disabled={busy}><Plus size={13}/> Thêm tiên quyết</Button>
      </form>
      <div className="space-y-2">{items.length === 0 ? <EmptyState text="Bài giảng chưa có điều kiện tiên quyết." /> : items.map((item,index)=>(
        <div key={item.requiredLessonId} className="flex items-center gap-3 rounded-lg border border-border bg-card p-3"><div className="flex h-8 w-8 items-center justify-center rounded-md bg-muted text-[12px] font-semibold">{index+1}</div><div className="min-w-0 flex-1"><p className="truncate text-[13px] font-medium">{item.titleVi}</p><p className="text-[11px] text-muted-foreground">#{item.requiredLessonId} · {item.slug}</p></div><IconDelete disabled={busy} onClick={() => void run(() => baiGiangApi.xoaTienQuyet(lessonId,item.requiredLessonId), "Đã xóa điều kiện tiên quyết.")} /></div>
      ))}</div>
    </PanelShell>
  );
}

function PanelShell({ title, description, children }: { title: string; description: string; children: React.ReactNode }) {
  return <section className="space-y-4 rounded-lg border border-border bg-card p-4"><div><h2 className="text-[14px] font-semibold">{title}</h2><p className="mt-1 text-[11px] text-muted-foreground">{description}</p></div>{children}</section>;
}

function Field({ label, className = "", children }: { label: string; className?: string; children: React.ReactNode }) {
  return <label className={`space-y-1.5 ${className}`}><span className="block text-[11px] font-medium text-muted-foreground">{label}</span>{children}</label>;
}

function Badge({ children }: { children: React.ReactNode }) {
  return <span className="rounded-md bg-muted px-2 py-1 text-[10px] font-medium text-muted-foreground">{children}</span>;
}

function IconDelete({ onClick, disabled }: { onClick: () => void; disabled?: boolean }) {
  return <button type="button" onClick={onClick} disabled={disabled} className="flex h-8 w-8 shrink-0 items-center justify-center rounded-md text-muted-foreground transition hover:bg-destructive/10 hover:text-destructive disabled:opacity-50" aria-label="Xóa"><Trash2 size={14}/></button>;
}

function EmptyState({ text }: { text: string }) {
  return <div className="rounded-lg border border-dashed border-border p-8 text-center text-[12px] text-muted-foreground">{text}</div>;
}
