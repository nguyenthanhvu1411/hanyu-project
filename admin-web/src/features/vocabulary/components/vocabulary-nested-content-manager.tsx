"use client";

import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";
import { Pencil, Plus, RefreshCw, Trash2, X } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";

import { vocabularyApi } from "../vocabulary.api";
import {
  AdminVocabularyExample,
  AdminVocabularyMeaning,
  AdminVocabularyRelation,
  VocabularyContentStatus,
  VOCABULARY_CONTENT_STATUS_LABELS,
  VocabularyExampleRequest,
  VocabularyMeaningRequest,
  VOCABULARY_RELATION_LABELS,
  VocabularyRelationRequest,
  VocabularyRelationType,
} from "../vocabulary.types";

type Mode = "meanings" | "examples" | "relations";

export function VocabularyNestedContentManager({ vocabularyId, mode }: { vocabularyId: number; mode: Mode }) {
  if (mode === "meanings") return <MeaningsManager vocabularyId={vocabularyId} />;
  if (mode === "examples") return <ExamplesManager vocabularyId={vocabularyId} />;
  return <RelationsManager vocabularyId={vocabularyId} />;
}

function MeaningsManager({ vocabularyId }: { vocabularyId: number }) {
  const [items, setItems] = useState<AdminVocabularyMeaning[]>([]);
  const [form, setForm] = useState<VocabularyMeaningRequest>({ meaningVi: "", senseOrder: 0, usageNoteVi: null });
  const [editingId, setEditingId] = useState<number | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    try { setItems(await vocabularyApi.meanings.list(vocabularyId)); }
    catch (caught) { appToast.error("Không thể tải nghĩa từ vựng", normalizeApiError(caught).message); setItems([]); }
    finally { setLoading(false); }
  }, [vocabularyId]);

  useEffect(() => { void load(); }, [load]);

  function reset() { setEditingId(null); setForm({ meaningVi: "", senseOrder: items.length, usageNoteVi: null }); }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!form.meaningVi.trim()) return appToast.error("Thiếu nghĩa", "Vui lòng nhập nghĩa tiếng Việt.");
    setSaving(true);
    try {
      const payload = { ...form, meaningVi: form.meaningVi.trim(), usageNoteVi: form.usageNoteVi?.trim() || null };
      if (editingId) await vocabularyApi.meanings.update(vocabularyId, editingId, payload);
      else await vocabularyApi.meanings.create(vocabularyId, payload);
      appToast.success(editingId ? "Đã cập nhật nghĩa." : "Đã thêm nghĩa.");
      reset();
      await load();
    } catch (caught) { appToast.error("Không thể lưu nghĩa", normalizeApiError(caught).message); }
    finally { setSaving(false); }
  }

  async function remove(id: number) {
    if (!window.confirm(`Xóa nghĩa #${id}?`)) return;
    try { await vocabularyApi.meanings.remove(vocabularyId, id); appToast.success("Đã xóa nghĩa."); await load(); }
    catch (caught) { appToast.error("Không thể xóa nghĩa", normalizeApiError(caught).message); }
  }

  return (
    <Workspace title={editingId ? "Sửa nghĩa" : "Thêm nghĩa"} form={
      <form className="space-y-3" onSubmit={submit}>
        <label className="block space-y-1"><span className="text-[11px] font-medium">Nghĩa tiếng Việt *</span><Textarea value={form.meaningVi} onChange={(e) => setForm((v) => ({ ...v, meaningVi: e.target.value }))} /></label>
        <label className="block space-y-1"><span className="text-[11px] font-medium">Thứ tự nghĩa</span><Input type="number" min={0} value={form.senseOrder} onChange={(e) => setForm((v) => ({ ...v, senseOrder: Number(e.target.value) }))} /></label>
        <label className="block space-y-1"><span className="text-[11px] font-medium">Ghi chú sử dụng</span><Textarea value={form.usageNoteVi ?? ""} onChange={(e) => setForm((v) => ({ ...v, usageNoteVi: e.target.value || null }))} /></label>
        <Button className="w-full gap-2" type="submit" loading={saving}><Plus size={14} />{editingId ? "Lưu nghĩa" : "Thêm nghĩa"}</Button>
        {editingId ? <Button className="w-full gap-2" type="button" variant="outline" onClick={reset}><X size={14} />Hủy sửa</Button> : null}
      </form>
    } onRefresh={() => void load()} loading={loading}>
      {items.map((item) => <ContentRow key={item.id} title={item.meaningVi} subtitle={`Thứ tự ${item.senseOrder}${item.usageNoteVi ? ` · ${item.usageNoteVi}` : ""}`} actions={<><Button size="icon" variant="outline" onClick={() => { setEditingId(item.id); setForm({ meaningVi: item.meaningVi, senseOrder: item.senseOrder, usageNoteVi: item.usageNoteVi }); }}><Pencil size={13} /></Button><Button size="icon" variant="outline" onClick={() => void remove(item.id)}><Trash2 size={13} /></Button></>} />)}
      {!loading && items.length === 0 ? <EmptyText text="Chưa có nghĩa bổ sung." /> : null}
    </Workspace>
  );
}

function ExamplesManager({ vocabularyId }: { vocabularyId: number }) {
  const EMPTY: VocabularyExampleRequest = { sentenceZh: "", sentencePinyin: "", sentenceVi: "", difficulty: 1, audioAssetId: null, sourceNote: null };
  const [items, setItems] = useState<AdminVocabularyExample[]>([]);
  const [form, setForm] = useState<VocabularyExampleRequest>(EMPTY);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [workingId, setWorkingId] = useState<number | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    try { setItems(await vocabularyApi.examples.list(vocabularyId)); }
    catch (caught) { appToast.error("Không thể tải ví dụ", normalizeApiError(caught).message); setItems([]); }
    finally { setLoading(false); }
  }, [vocabularyId]);
  useEffect(() => { void load(); }, [load]);

  function reset() { setEditingId(null); setForm(EMPTY); }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!form.sentenceZh.trim() || !form.sentencePinyin.trim() || !form.sentenceVi.trim()) return appToast.error("Thiếu nội dung", "Cần nhập câu Trung, Pinyin và nghĩa Việt.");
    setSaving(true);
    try {
      const payload = { ...form, sentenceZh: form.sentenceZh.trim(), sentencePinyin: form.sentencePinyin.trim(), sentenceVi: form.sentenceVi.trim(), sourceNote: form.sourceNote?.trim() || null, audioAssetId: form.audioAssetId || null };
      if (editingId) await vocabularyApi.examples.update(vocabularyId, editingId, payload);
      else await vocabularyApi.examples.create(vocabularyId, payload);
      appToast.success(editingId ? "Đã cập nhật ví dụ." : "Đã thêm ví dụ.");
      reset(); await load();
    } catch (caught) { appToast.error("Không thể lưu ví dụ", normalizeApiError(caught).message); }
    finally { setSaving(false); }
  }

  async function action(item: AdminVocabularyExample, execute: () => Promise<void>, message: string) {
    setWorkingId(item.id);
    try { await execute(); appToast.success(message); await load(); }
    catch (caught) { appToast.error("Không thể cập nhật ví dụ", normalizeApiError(caught).message); }
    finally { setWorkingId(null); }
  }

  const workflow = (item: AdminVocabularyExample) => {
    if (item.status === VocabularyContentStatus.Draft) return <Button size="sm" variant="outline" onClick={() => void action(item, () => vocabularyApi.examples.submitReview(vocabularyId, item.id), "Đã gửi duyệt.")}>Gửi duyệt</Button>;
    if (item.status === VocabularyContentStatus.Review) return <Button size="sm" variant="outline" onClick={() => void action(item, () => vocabularyApi.examples.approve(vocabularyId, item.id), "Đã duyệt.")}>Duyệt</Button>;
    if (item.status === VocabularyContentStatus.Approved) return <Button size="sm" onClick={() => void action(item, () => vocabularyApi.examples.publish(vocabularyId, item.id), "Đã xuất bản.")}>Xuất bản</Button>;
    if (item.status === VocabularyContentStatus.Published) return <Button size="sm" variant="outline" onClick={() => void action(item, () => vocabularyApi.examples.archive(vocabularyId, item.id), "Đã lưu trữ.")}>Lưu trữ</Button>;
    return <Button size="sm" variant="outline" onClick={() => void action(item, () => vocabularyApi.examples.restore(vocabularyId, item.id), "Đã khôi phục.")}>Khôi phục</Button>;
  };

  return (
    <Workspace title={editingId ? "Sửa ví dụ" : "Thêm ví dụ"} form={<form className="space-y-3" onSubmit={submit}>
      <label className="block space-y-1"><span className="text-[11px] font-medium">Câu tiếng Trung *</span><Textarea value={form.sentenceZh} onChange={(e) => setForm((v) => ({ ...v, sentenceZh: e.target.value }))} /></label>
      <label className="block space-y-1"><span className="text-[11px] font-medium">Pinyin *</span><Textarea value={form.sentencePinyin} onChange={(e) => setForm((v) => ({ ...v, sentencePinyin: e.target.value }))} /></label>
      <label className="block space-y-1"><span className="text-[11px] font-medium">Nghĩa tiếng Việt *</span><Textarea value={form.sentenceVi} onChange={(e) => setForm((v) => ({ ...v, sentenceVi: e.target.value }))} /></label>
      <div className="grid grid-cols-2 gap-2"><label className="space-y-1"><span className="text-[10px] font-medium">Độ khó</span><Input type="number" min={1} value={form.difficulty} onChange={(e) => setForm((v) => ({ ...v, difficulty: Number(e.target.value) }))} /></label><label className="space-y-1"><span className="text-[10px] font-medium">Audio Asset ID</span><Input type="number" min={1} value={form.audioAssetId ?? ""} onChange={(e) => setForm((v) => ({ ...v, audioAssetId: e.target.value ? Number(e.target.value) : null }))} /></label></div>
      <label className="block space-y-1"><span className="text-[11px] font-medium">Nguồn / ghi chú</span><Textarea value={form.sourceNote ?? ""} onChange={(e) => setForm((v) => ({ ...v, sourceNote: e.target.value || null }))} /></label>
      <Button className="w-full" type="submit" loading={saving}>{editingId ? "Lưu ví dụ" : "Thêm ví dụ"}</Button>{editingId ? <Button className="w-full" type="button" variant="outline" onClick={reset}>Hủy sửa</Button> : null}
    </form>} onRefresh={() => void load()} loading={loading}>
      {items.map((item) => <ContentRow key={item.id} title={item.sentenceZh} subtitle={`${item.sentencePinyin} · ${item.sentenceVi}`} badge={<Badge>{VOCABULARY_CONTENT_STATUS_LABELS[item.status]}</Badge>} extra={workflow(item)} actions={<><Button size="icon" variant="outline" disabled={workingId === item.id} onClick={() => { setEditingId(item.id); setForm({ sentenceZh: item.sentenceZh, sentencePinyin: item.sentencePinyin, sentenceVi: item.sentenceVi, difficulty: item.difficulty, audioAssetId: item.audioAssetId, sourceNote: item.sourceNote }); }}><Pencil size={13} /></Button><Button size="icon" variant="outline" disabled={workingId === item.id} onClick={() => void action(item, () => vocabularyApi.examples.remove(vocabularyId, item.id), "Đã xóa ví dụ.")}><Trash2 size={13} /></Button></>} />)}
      {!loading && items.length === 0 ? <EmptyText text="Chưa có ví dụ." /> : null}
    </Workspace>
  );
}

function RelationsManager({ vocabularyId }: { vocabularyId: number }) {
  const EMPTY: VocabularyRelationRequest = { relatedVocabularyId: 0, relationType: VocabularyRelationType.Related, noteVi: null };
  const [items, setItems] = useState<AdminVocabularyRelation[]>([]);
  const [form, setForm] = useState<VocabularyRelationRequest>(EMPTY);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => { setLoading(true); try { setItems(await vocabularyApi.relations.list(vocabularyId)); } catch (caught) { appToast.error("Không thể tải quan hệ", normalizeApiError(caught).message); setItems([]); } finally { setLoading(false); } }, [vocabularyId]);
  useEffect(() => { void load(); }, [load]);
  function reset() { setEditingId(null); setForm(EMPTY); }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!form.relatedVocabularyId || form.relatedVocabularyId === vocabularyId) return appToast.error("Vocabulary liên quan không hợp lệ", "Hãy nhập ID của một Vocabulary khác.");
    setSaving(true);
    try { if (editingId) await vocabularyApi.relations.update(vocabularyId, editingId, form); else await vocabularyApi.relations.create(vocabularyId, form); appToast.success(editingId ? "Đã cập nhật quan hệ." : "Đã thêm quan hệ."); reset(); await load(); }
    catch (caught) { appToast.error("Không thể lưu quan hệ", normalizeApiError(caught).message); }
    finally { setSaving(false); }
  }

  async function remove(id: number) { if (!window.confirm(`Xóa quan hệ #${id}?`)) return; try { await vocabularyApi.relations.remove(vocabularyId, id); appToast.success("Đã xóa quan hệ."); await load(); } catch (caught) { appToast.error("Không thể xóa quan hệ", normalizeApiError(caught).message); } }

  return <Workspace title={editingId ? "Sửa quan hệ" : "Thêm quan hệ"} form={<form className="space-y-3" onSubmit={submit}>
    <label className="block space-y-1"><span className="text-[11px] font-medium">Related Vocabulary ID *</span><Input type="number" min={1} value={form.relatedVocabularyId || ""} onChange={(e) => setForm((v) => ({ ...v, relatedVocabularyId: Number(e.target.value) }))} /></label>
    <label className="block space-y-1"><span className="text-[11px] font-medium">Loại quan hệ</span><select className="h-10 w-full rounded-md border px-3 text-[11px]" value={form.relationType} onChange={(e) => setForm((v) => ({ ...v, relationType: Number(e.target.value) as VocabularyRelationType }))}>{Object.entries(VOCABULARY_RELATION_LABELS).map(([k, label]) => <option key={k} value={k}>{label}</option>)}</select></label>
    <label className="block space-y-1"><span className="text-[11px] font-medium">Ghi chú</span><Textarea value={form.noteVi ?? ""} onChange={(e) => setForm((v) => ({ ...v, noteVi: e.target.value || null }))} /></label>
    <Button className="w-full" type="submit" loading={saving}>{editingId ? "Lưu quan hệ" : "Thêm quan hệ"}</Button>{editingId ? <Button className="w-full" type="button" variant="outline" onClick={reset}>Hủy sửa</Button> : null}
  </form>} onRefresh={() => void load()} loading={loading}>
    {items.map((item) => <ContentRow key={item.id} title={`${item.relatedSimplified} · ${item.relatedPinyin}`} subtitle={item.relatedMeaningVi} badge={<Badge>{VOCABULARY_RELATION_LABELS[item.relationType]}</Badge>} actions={<><Button size="icon" variant="outline" onClick={() => { setEditingId(item.id); setForm({ relatedVocabularyId: item.relatedVocabularyId, relationType: item.relationType, noteVi: item.noteVi }); }}><Pencil size={13} /></Button><Button size="icon" variant="outline" onClick={() => void remove(item.id)}><Trash2 size={13} /></Button></>} />)}
    {!loading && items.length === 0 ? <EmptyText text="Chưa có quan hệ từ vựng." /> : null}
  </Workspace>;
}

function Workspace({ title, form, children, onRefresh, loading }: { title: string; form: React.ReactNode; children: React.ReactNode; onRefresh: () => void; loading: boolean }) {
  return <div className="grid gap-5 xl:grid-cols-[390px_minmax(0,1fr)]"><Card className="h-fit"><CardHeader><CardTitle>{title}</CardTitle></CardHeader><CardContent>{form}</CardContent></Card><Card><CardHeader className="flex flex-row items-center justify-between"><CardTitle>Danh sách</CardTitle><Button size="sm" variant="outline" className="gap-2" onClick={onRefresh}><RefreshCw size={13} />Làm mới</Button></CardHeader><CardContent className="space-y-3">{loading ? <div className="py-8 text-center text-[11px] text-muted-foreground">Đang tải...</div> : children}</CardContent></Card></div>;
}

function ContentRow({ title, subtitle, badge, actions, extra }: { title: string; subtitle?: string; badge?: React.ReactNode; actions: React.ReactNode; extra?: React.ReactNode }) {
  return <div className="rounded-[9px] border p-3"><div className="flex items-start justify-between gap-3"><div className="min-w-0 flex-1"><div className="flex flex-wrap items-center gap-2"><div className="text-[12px] font-semibold text-[#333]">{title}</div>{badge}</div>{subtitle ? <div className="mt-1 text-[10px] leading-4 text-muted-foreground">{subtitle}</div> : null}</div><div className="flex gap-1">{actions}</div></div>{extra ? <div className="mt-3 border-t pt-3">{extra}</div> : null}</div>;
}

function EmptyText({ text }: { text: string }) { return <div className="rounded-md border border-dashed p-8 text-center text-[11px] text-muted-foreground">{text}</div>; }
