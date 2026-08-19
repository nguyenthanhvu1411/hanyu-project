"use client";

import { FormEvent, ReactNode, useCallback, useEffect, useState } from "react";
import { Pencil, Plus, RefreshCw, Trash2, X } from "lucide-react";

import { AudioAssetSelector, VocabularySelector } from "@/components/admin/entity-selectors";
import { EmptyState } from "@/components/common/empty-state";
import { ErrorState } from "@/components/common/error-state";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { Textarea } from "@/components/ui/textarea";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";

import { vocabularyApi } from "../vocabulary.api";
import {
  type AdminVocabularyExample,
  type AdminVocabularyMeaning,
  type AdminVocabularyRelation,
  VocabularyContentStatus,
  VOCABULARY_CONTENT_STATUS_LABELS,
  type VocabularyExampleRequest,
  type VocabularyMeaningRequest,
  VOCABULARY_RELATION_LABELS,
  type VocabularyRelationRequest,
  VocabularyRelationType,
} from "../vocabulary.types";

type Mode = "meanings" | "examples" | "relations";

type DeleteTarget = { id: number; label: string } | null;

const relationOptions = Object.entries(VOCABULARY_RELATION_LABELS).map(([value, label]) => ({ value, label }));

export function VocabularyNestedContentManager({ vocabularyId, mode }: { vocabularyId: number; mode: Mode }) {
  if (mode === "meanings") return <MeaningsManager vocabularyId={vocabularyId} />;
  if (mode === "examples") return <ExamplesManager vocabularyId={vocabularyId} />;
  return <RelationsManager vocabularyId={vocabularyId} />;
}

function Workspace({ title, editing, onCancelEdit, onRefresh, form, loading, error, children }: {
  title: string;
  editing: boolean;
  onCancelEdit: () => void;
  onRefresh: () => void;
  form: ReactNode;
  loading: boolean;
  error: string | null;
  children: ReactNode;
}) {
  return (
    <div className="grid gap-5 xl:grid-cols-[380px_minmax(0,1fr)]">
      <Card className="h-fit">
        <CardHeader className="flex flex-row items-center justify-between gap-3">
          <CardTitle>{title}</CardTitle>
          {editing ? <Button type="button" size="icon" variant="ghost" aria-label="Hủy chỉnh sửa" onClick={onCancelEdit}><X size={14} /></Button> : null}
        </CardHeader>
        <CardContent>{form}</CardContent>
      </Card>

      <Card>
        <CardHeader className="flex flex-row items-center justify-between gap-3">
          <div>
            <CardTitle>Danh sách</CardTitle>
            <p className="mt-1 text-[11px] text-muted-foreground">Dữ liệu thuộc từ vựng đang mở.</p>
          </div>
          <Button type="button" size="sm" variant="outline" className="gap-2" onClick={onRefresh}><RefreshCw size={14} />Làm mới</Button>
        </CardHeader>
        <CardContent>
          {loading ? (
            <div className="space-y-3">{Array.from({ length: 4 }).map((_, index) => <Skeleton key={index} className="h-20 w-full" />)}</div>
          ) : error ? (
            <ErrorState description={error} onRetry={onRefresh} />
          ) : children}
        </CardContent>
      </Card>
    </div>
  );
}

function ContentRow({ title, subtitle, badge, extra, actions }: { title: string; subtitle?: string; badge?: ReactNode; extra?: ReactNode; actions?: ReactNode }) {
  return (
    <div className="flex flex-wrap items-center justify-between gap-3 rounded-[10px] border border-[#e8e3dc] bg-white p-4">
      <div className="min-w-0 flex-1">
        <div className="flex flex-wrap items-center gap-2">
          <span className="text-[13px] font-semibold text-[#343434]">{title}</span>
          {badge}
        </div>
        {subtitle ? <p className="mt-1 text-[11px] leading-5 text-muted-foreground">{subtitle}</p> : null}
      </div>
      <div className="flex shrink-0 flex-wrap items-center gap-2">{extra}{actions}</div>
    </div>
  );
}

function MeaningsManager({ vocabularyId }: { vocabularyId: number }) {
  const emptyForm: VocabularyMeaningRequest = { meaningVi: "", senseOrder: 0, usageNoteVi: null };
  const [items, setItems] = useState<AdminVocabularyMeaning[]>([]);
  const [form, setForm] = useState<VocabularyMeaningRequest>(emptyForm);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<DeleteTarget>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true); setError(null);
    try { setItems(await vocabularyApi.meanings.list(vocabularyId)); }
    catch (caught) { setItems([]); setError(normalizeApiError(caught).message); }
    finally { setLoading(false); }
  }, [vocabularyId]);

  useEffect(() => { void load(); }, [load]);

  function reset() { setEditingId(null); setForm({ ...emptyForm, senseOrder: items.length }); }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!form.meaningVi.trim()) { appToast.error("Thiếu nghĩa", "Vui lòng nhập nghĩa tiếng Việt."); return; }
    setSaving(true);
    try {
      const payload = { ...form, meaningVi: form.meaningVi.trim(), usageNoteVi: form.usageNoteVi?.trim() || null };
      if (editingId) await vocabularyApi.meanings.update(vocabularyId, editingId, payload);
      else await vocabularyApi.meanings.create(vocabularyId, payload);
      appToast.success(editingId ? "Đã cập nhật nghĩa." : "Đã thêm nghĩa.");
      reset(); await load();
    } catch (caught) { appToast.error("Không thể lưu nghĩa", normalizeApiError(caught).message); }
    finally { setSaving(false); }
  }

  async function confirmDelete() {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await vocabularyApi.meanings.remove(vocabularyId, deleteTarget.id);
      appToast.success("Đã xóa nghĩa."); setDeleteTarget(null); await load();
    } catch (caught) { appToast.error("Không thể xóa nghĩa", normalizeApiError(caught).message); }
    finally { setDeleting(false); }
  }

  return (
    <>
      <Workspace title={editingId ? "Sửa nghĩa" : "Thêm nghĩa"} editing={Boolean(editingId)} onCancelEdit={reset} onRefresh={() => void load()} loading={loading} error={error} form={(
        <form className="space-y-3" onSubmit={submit}>
          <label className="block space-y-1"><span className="text-[11px] font-medium">Nghĩa tiếng Việt *</span><Textarea value={form.meaningVi} onChange={(event) => setForm((current) => ({ ...current, meaningVi: event.target.value }))} /></label>
          <label className="block space-y-1"><span className="text-[11px] font-medium">Thứ tự nghĩa</span><Input type="number" min={0} value={form.senseOrder} onChange={(event) => setForm((current) => ({ ...current, senseOrder: Number(event.target.value) }))} /></label>
          <label className="block space-y-1"><span className="text-[11px] font-medium">Ghi chú sử dụng</span><Textarea value={form.usageNoteVi ?? ""} onChange={(event) => setForm((current) => ({ ...current, usageNoteVi: event.target.value || null }))} /></label>
          <Button className="w-full gap-2" type="submit" loading={saving}><Plus size={14} />{editingId ? "Lưu thay đổi" : "Thêm nghĩa"}</Button>
        </form>
      )}>
        {items.length === 0 ? <EmptyState title="Chưa có nghĩa bổ sung" description="Thêm nghĩa tiếng Việt và ghi chú sử dụng cho từ vựng này." /> : (
          <div className="space-y-3">{items.map((item) => <ContentRow key={item.id} title={item.meaningVi} subtitle={`Thứ tự ${item.senseOrder}${item.usageNoteVi ? ` · ${item.usageNoteVi}` : ""}`} actions={<><Button size="icon" variant="outline" aria-label="Sửa nghĩa" onClick={() => { setEditingId(item.id); setForm({ meaningVi: item.meaningVi, senseOrder: item.senseOrder, usageNoteVi: item.usageNoteVi }); }}><Pencil size={13} /></Button><Button size="icon" variant="dangerGhost" aria-label="Xóa nghĩa" onClick={() => setDeleteTarget({ id: item.id, label: item.meaningVi })}><Trash2 size={13} /></Button></>} />)}</div>
        )}
      </Workspace>
      <ConfirmDialog open={Boolean(deleteTarget)} title="Xóa nghĩa từ vựng?" description={deleteTarget ? `Nghĩa “${deleteTarget.label}” sẽ bị xóa.` : ""} confirmLabel="Xóa nghĩa" loading={deleting} onClose={() => setDeleteTarget(null)} onConfirm={confirmDelete} />
    </>
  );
}

function ExamplesManager({ vocabularyId }: { vocabularyId: number }) {
  const emptyForm: VocabularyExampleRequest = { sentenceZh: "", sentencePinyin: "", sentenceVi: "", difficulty: 1, audioAssetId: null, sourceNote: null };
  const [items, setItems] = useState<AdminVocabularyExample[]>([]);
  const [form, setForm] = useState<VocabularyExampleRequest>(emptyForm);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<DeleteTarget>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [workingId, setWorkingId] = useState<number | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true); setError(null);
    try { setItems(await vocabularyApi.examples.list(vocabularyId)); }
    catch (caught) { setItems([]); setError(normalizeApiError(caught).message); }
    finally { setLoading(false); }
  }, [vocabularyId]);
  useEffect(() => { void load(); }, [load]);

  function reset() { setEditingId(null); setForm(emptyForm); }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!form.sentenceZh.trim() || !form.sentencePinyin.trim() || !form.sentenceVi.trim()) { appToast.error("Thiếu nội dung", "Cần nhập câu Trung, Pinyin và nghĩa Việt."); return; }
    setSaving(true);
    try {
      const payload = { ...form, sentenceZh: form.sentenceZh.trim(), sentencePinyin: form.sentencePinyin.trim(), sentenceVi: form.sentenceVi.trim(), sourceNote: form.sourceNote?.trim() || null };
      if (editingId) await vocabularyApi.examples.update(vocabularyId, editingId, payload);
      else await vocabularyApi.examples.create(vocabularyId, payload);
      appToast.success(editingId ? "Đã cập nhật ví dụ." : "Đã thêm ví dụ."); reset(); await load();
    } catch (caught) { appToast.error("Không thể lưu ví dụ", normalizeApiError(caught).message); }
    finally { setSaving(false); }
  }

  async function workflow(item: AdminVocabularyExample, action: "review" | "approve" | "publish" | "archive" | "restore") {
    setWorkingId(item.id);
    try {
      if (action === "review") await vocabularyApi.examples.submitReview(vocabularyId, item.id);
      if (action === "approve") await vocabularyApi.examples.approve(vocabularyId, item.id);
      if (action === "publish") await vocabularyApi.examples.publish(vocabularyId, item.id);
      if (action === "archive") await vocabularyApi.examples.archive(vocabularyId, item.id);
      if (action === "restore") await vocabularyApi.examples.restore(vocabularyId, item.id);
      appToast.success("Đã cập nhật trạng thái ví dụ."); await load();
    } catch (caught) { appToast.error("Không thể cập nhật ví dụ", normalizeApiError(caught).message); }
    finally { setWorkingId(null); }
  }

  async function confirmDelete() {
    if (!deleteTarget) return;
    setDeleting(true);
    try { await vocabularyApi.examples.remove(vocabularyId, deleteTarget.id); appToast.success("Đã xóa ví dụ."); setDeleteTarget(null); await load(); }
    catch (caught) { appToast.error("Không thể xóa ví dụ", normalizeApiError(caught).message); }
    finally { setDeleting(false); }
  }

  function workflowButton(item: AdminVocabularyExample) {
    if (item.status === VocabularyContentStatus.Draft) return <Button size="sm" variant="outline" disabled={workingId === item.id} onClick={() => void workflow(item, "review")}>Gửi duyệt</Button>;
    if (item.status === VocabularyContentStatus.Review) return <Button size="sm" variant="outline" disabled={workingId === item.id} onClick={() => void workflow(item, "approve")}>Duyệt</Button>;
    if (item.status === VocabularyContentStatus.Approved) return <Button size="sm" disabled={workingId === item.id} onClick={() => void workflow(item, "publish")}>Xuất bản</Button>;
    if (item.status === VocabularyContentStatus.Published) return <Button size="sm" variant="outline" disabled={workingId === item.id} onClick={() => void workflow(item, "archive")}>Lưu trữ</Button>;
    return <Button size="sm" variant="outline" disabled={workingId === item.id} onClick={() => void workflow(item, "restore")}>Khôi phục</Button>;
  }

  return (
    <>
      <Workspace title={editingId ? "Sửa ví dụ" : "Thêm ví dụ"} editing={Boolean(editingId)} onCancelEdit={reset} onRefresh={() => void load()} loading={loading} error={error} form={(
        <form className="space-y-3" onSubmit={submit}>
          <label className="block space-y-1"><span className="text-[11px] font-medium">Câu tiếng Trung *</span><Textarea value={form.sentenceZh} onChange={(event) => setForm((current) => ({ ...current, sentenceZh: event.target.value }))} /></label>
          <label className="block space-y-1"><span className="text-[11px] font-medium">Pinyin *</span><Textarea value={form.sentencePinyin} onChange={(event) => setForm((current) => ({ ...current, sentencePinyin: event.target.value }))} /></label>
          <label className="block space-y-1"><span className="text-[11px] font-medium">Nghĩa tiếng Việt *</span><Textarea value={form.sentenceVi} onChange={(event) => setForm((current) => ({ ...current, sentenceVi: event.target.value }))} /></label>
          <label className="block space-y-1"><span className="text-[11px] font-medium">Audio</span><AudioAssetSelector value={form.audioAssetId ? String(form.audioAssetId) : ""} onValueChange={(value) => setForm((current) => ({ ...current, audioAssetId: value ? Number(value) : null }))} /></label>
          <label className="block space-y-1"><span className="text-[11px] font-medium">Độ khó</span><Input type="number" min={1} value={form.difficulty} onChange={(event) => setForm((current) => ({ ...current, difficulty: Number(event.target.value) }))} /></label>
          <label className="block space-y-1"><span className="text-[11px] font-medium">Nguồn / ghi chú</span><Textarea value={form.sourceNote ?? ""} onChange={(event) => setForm((current) => ({ ...current, sourceNote: event.target.value || null }))} /></label>
          <Button className="w-full gap-2" type="submit" loading={saving}><Plus size={14} />{editingId ? "Lưu thay đổi" : "Thêm ví dụ"}</Button>
        </form>
      )}>
        {items.length === 0 ? <EmptyState title="Chưa có ví dụ" description="Tạo câu ví dụ và gắn audio bằng đối tượng có sẵn thay vì nhập ID." /> : (
          <div className="space-y-3">{items.map((item) => <ContentRow key={item.id} title={item.sentenceZh} subtitle={`${item.sentencePinyin} · ${item.sentenceVi}`} badge={<Badge>{VOCABULARY_CONTENT_STATUS_LABELS[item.status]}</Badge>} extra={workflowButton(item)} actions={<><Button size="icon" variant="outline" aria-label="Sửa ví dụ" onClick={() => { setEditingId(item.id); setForm({ sentenceZh: item.sentenceZh, sentencePinyin: item.sentencePinyin, sentenceVi: item.sentenceVi, difficulty: item.difficulty, audioAssetId: item.audioAssetId, sourceNote: item.sourceNote }); }}><Pencil size={13} /></Button><Button size="icon" variant="dangerGhost" aria-label="Xóa ví dụ" onClick={() => setDeleteTarget({ id: item.id, label: item.sentenceZh })}><Trash2 size={13} /></Button></>} />)}</div>
        )}
      </Workspace>
      <ConfirmDialog open={Boolean(deleteTarget)} title="Xóa ví dụ?" description={deleteTarget ? `Câu “${deleteTarget.label}” sẽ bị xóa.` : ""} confirmLabel="Xóa ví dụ" loading={deleting} onClose={() => setDeleteTarget(null)} onConfirm={confirmDelete} />
    </>
  );
}

function RelationsManager({ vocabularyId }: { vocabularyId: number }) {
  const emptyForm: VocabularyRelationRequest = { relatedVocabularyId: 0, relationType: VocabularyRelationType.Related, noteVi: null };
  const [items, setItems] = useState<AdminVocabularyRelation[]>([]);
  const [form, setForm] = useState<VocabularyRelationRequest>(emptyForm);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<DeleteTarget>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true); setError(null);
    try { setItems(await vocabularyApi.relations.list(vocabularyId)); }
    catch (caught) { setItems([]); setError(normalizeApiError(caught).message); }
    finally { setLoading(false); }
  }, [vocabularyId]);
  useEffect(() => { void load(); }, [load]);

  function reset() { setEditingId(null); setForm(emptyForm); }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!form.relatedVocabularyId || form.relatedVocabularyId === vocabularyId) { appToast.error("Từ vựng liên quan không hợp lệ", "Hãy chọn một từ vựng khác."); return; }
    setSaving(true);
    try {
      if (editingId) await vocabularyApi.relations.update(vocabularyId, editingId, form);
      else await vocabularyApi.relations.create(vocabularyId, form);
      appToast.success(editingId ? "Đã cập nhật quan hệ." : "Đã thêm quan hệ."); reset(); await load();
    } catch (caught) { appToast.error("Không thể lưu quan hệ", normalizeApiError(caught).message); }
    finally { setSaving(false); }
  }

  async function confirmDelete() {
    if (!deleteTarget) return;
    setDeleting(true);
    try { await vocabularyApi.relations.remove(vocabularyId, deleteTarget.id); appToast.success("Đã xóa quan hệ."); setDeleteTarget(null); await load(); }
    catch (caught) { appToast.error("Không thể xóa quan hệ", normalizeApiError(caught).message); }
    finally { setDeleting(false); }
  }

  return (
    <>
      <Workspace title={editingId ? "Sửa quan hệ" : "Thêm quan hệ"} editing={Boolean(editingId)} onCancelEdit={reset} onRefresh={() => void load()} loading={loading} error={error} form={(
        <form className="space-y-3" onSubmit={submit}>
          <label className="block space-y-1"><span className="text-[11px] font-medium">Từ vựng liên quan *</span><VocabularySelector value={form.relatedVocabularyId ? String(form.relatedVocabularyId) : ""} onValueChange={(value) => setForm((current) => ({ ...current, relatedVocabularyId: value ? Number(value) : 0 }))} /></label>
          <label className="block space-y-1"><span className="text-[11px] font-medium">Loại quan hệ</span><Select value={String(form.relationType)} options={relationOptions} onValueChange={(value) => setForm((current) => ({ ...current, relationType: Number(value) as VocabularyRelationType }))} /></label>
          <label className="block space-y-1"><span className="text-[11px] font-medium">Ghi chú</span><Textarea value={form.noteVi ?? ""} onChange={(event) => setForm((current) => ({ ...current, noteVi: event.target.value || null }))} /></label>
          <Button className="w-full gap-2" type="submit" loading={saving}><Plus size={14} />{editingId ? "Lưu thay đổi" : "Thêm quan hệ"}</Button>
        </form>
      )}>
        {items.length === 0 ? <EmptyState title="Chưa có quan hệ từ vựng" description="Chọn từ liên quan bằng Hanzi, Pinyin hoặc nghĩa; không cần biết Vocabulary ID." /> : (
          <div className="space-y-3">{items.map((item) => <ContentRow key={item.id} title={`${item.relatedSimplified} · ${item.relatedPinyin}`} subtitle={`${item.relatedMeaningVi}${item.noteVi ? ` · ${item.noteVi}` : ""}`} badge={<Badge>{VOCABULARY_RELATION_LABELS[item.relationType]}</Badge>} actions={<><Button size="icon" variant="outline" aria-label="Sửa quan hệ" onClick={() => { setEditingId(item.id); setForm({ relatedVocabularyId: item.relatedVocabularyId, relationType: item.relationType, noteVi: item.noteVi }); }}><Pencil size={13} /></Button><Button size="icon" variant="dangerGhost" aria-label="Xóa quan hệ" onClick={() => setDeleteTarget({ id: item.id, label: item.relatedSimplified })}><Trash2 size={13} /></Button></>} />)}</div>
        )}
      </Workspace>
      <ConfirmDialog open={Boolean(deleteTarget)} title="Xóa quan hệ từ vựng?" description={deleteTarget ? `Quan hệ với “${deleteTarget.label}” sẽ bị xóa.` : ""} confirmLabel="Xóa quan hệ" loading={deleting} onClose={() => setDeleteTarget(null)} onConfirm={confirmDelete} />
    </>
  );
}
