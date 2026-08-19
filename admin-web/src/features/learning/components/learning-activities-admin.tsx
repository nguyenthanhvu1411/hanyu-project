"use client";

import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";
import { Pencil, Plus, RefreshCw, Trash2, X } from "lucide-react";

import { DataTable } from "@/components/common/data-table/data-table";
import { DataTableSearch } from "@/components/common/data-table/data-table-search";
import { DataTableToolbar } from "@/components/common/data-table/data-table-toolbar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";
import type { DataTableColumn } from "@/types/table.types";

import { learningApi } from "../learning.api";
import {
  LEARNING_ACTIVITY_TYPE_LABELS,
  LearningActivityType,
  type AdminLearningActivity,
  type CreateLearningActivityRequest,
} from "../learning.types";

const EMPTY_FORM: CreateLearningActivityRequest = {
  userId: "",
  activityType: LearningActivityType.Other,
  lessonId: null,
  vocabularyId: null,
  quizAttemptId: null,
  flashcardSessionId: null,
  durationSeconds: 0,
  xpEarned: 0,
  isCompleted: true,
  metadataJson: null,
};

export function LearningActivitiesAdmin() {
  const [items, setItems] = useState<AdminLearningActivity[]>([]);
  const [total, setTotal] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [userId, setUserId] = useState("");
  const [typeFilter, setTypeFilter] = useState("");
  const [completedFilter, setCompletedFilter] = useState("");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [workingId, setWorkingId] = useState<number | null>(null);
  const [editing, setEditing] = useState<AdminLearningActivity | null>(null);
  const [form, setForm] = useState<CreateLearningActivityRequest>(EMPTY_FORM);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const result = await learningApi.activities.list({
        userId: userId.trim() || undefined,
        activityType: typeFilter === "" ? undefined : Number(typeFilter) as LearningActivityType,
        isCompleted: completedFilter === "" ? undefined : completedFilter === "true",
        page,
        pageSize,
      });
      const count = result.total ?? result.totalCount ?? 0;
      setItems(result.items ?? []);
      setTotal(count);
      setTotalPages(result.totalPages ?? Math.max(1, Math.ceil(count / Math.max(1, result.pageSize ?? pageSize))));
    } catch (caught) {
      appToast.error("Không thể tải hoạt động học tập", normalizeApiError(caught).message);
      setItems([]);
      setTotal(0);
      setTotalPages(1);
    } finally {
      setLoading(false);
    }
  }, [completedFilter, page, pageSize, typeFilter, userId]);

  useEffect(() => {
    const timer = window.setTimeout(() => void load(), userId ? 250 : 0);
    return () => window.clearTimeout(timer);
  }, [load, userId]);

  function resetForm() { setEditing(null); setForm(EMPTY_FORM); }

  function edit(item: AdminLearningActivity) {
    setEditing(item);
    setForm({
      userId: item.userId,
      activityType: item.activityType,
      lessonId: item.lessonId,
      vocabularyId: item.vocabularyId,
      quizAttemptId: item.quizAttemptId,
      flashcardSessionId: item.flashcardSessionId,
      durationSeconds: item.durationSeconds,
      xpEarned: item.xpEarned,
      isCompleted: item.isCompleted,
      metadataJson: item.metadataJson,
    });
  }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!form.userId.trim()) { appToast.error("Thiếu UserId", "Vui lòng nhập UserId."); return; }
    setSaving(true);
    try {
      if (editing) {
        const { userId: _userId, ...payload } = form;
        void _userId;
        await learningApi.activities.update(editing.id, payload);
        appToast.success("Đã cập nhật hoạt động học tập.");
      } else {
        await learningApi.activities.create({ ...form, userId: form.userId.trim() });
        appToast.success("Đã tạo hoạt động học tập.");
      }
      resetForm();
      await load();
    } catch (caught) {
      appToast.error(editing ? "Không thể cập nhật hoạt động" : "Không thể tạo hoạt động", normalizeApiError(caught).message);
    } finally { setSaving(false); }
  }

  async function remove(item: AdminLearningActivity) {
    if (!window.confirm(`Xóa hoạt động #${item.id}?`)) return;
    setWorkingId(item.id);
    try { await learningApi.activities.remove(item.id); appToast.success("Đã xóa hoạt động."); await load(); }
    catch (caught) { appToast.error("Không thể xóa hoạt động", normalizeApiError(caught).message); }
    finally { setWorkingId(null); }
  }

  function numberField(key: "lessonId" | "vocabularyId" | "quizAttemptId" | "flashcardSessionId", label: string) {
    return <label className="space-y-1"><span className="text-[10px] font-medium">{label}</span><Input type="number" min={1} value={form[key] ?? ""} onChange={(event) => setForm((v) => ({ ...v, [key]: event.target.value ? Number(event.target.value) : null }))} /></label>;
  }

  const columns = useMemo<DataTableColumn<AdminLearningActivity>[]>(() => [
    { id: "type", header: "Hoạt động", cell: (item) => <div><div className="text-[12px] font-semibold">{LEARNING_ACTIVITY_TYPE_LABELS[item.activityType]}</div><div className="text-[10px] text-muted-foreground">#{item.id}</div></div> },
    { id: "user", header: "User", width: "190px", cell: (item) => <span className="font-mono text-[10px]">{item.userId}</span> },
    { id: "metrics", header: "Thời lượng / XP", width: "130px", cell: (item) => <div className="text-[10px]"><div>{item.durationSeconds}s</div><div className="text-muted-foreground">{item.xpEarned} XP</div></div> },
    { id: "refs", header: "Liên kết", width: "180px", cell: (item) => <div className="text-[10px] text-muted-foreground">Lesson {item.lessonId ?? "—"} · Vocab {item.vocabularyId ?? "—"}</div> },
    { id: "status", header: "Trạng thái", width: "110px", cell: (item) => <Badge variant={item.isCompleted ? "success" : "default"}>{item.isCompleted ? "Hoàn tất" : "Đang làm"}</Badge> },
    { id: "time", header: "Bắt đầu", width: "150px", cell: (item) => <span className="text-[10px]">{new Date(item.startedAt).toLocaleString("vi-VN")}</span> },
    { id: "actions", header: "Thao tác", width: "100px", align: "center", cell: (item) => <div className="flex justify-center gap-1"><Button size="icon" variant="outline" onClick={() => edit(item)}><Pencil size={13} /></Button><Button size="icon" variant="outline" disabled={workingId === item.id} onClick={() => void remove(item)}><Trash2 size={13} /></Button></div> },
  ], [workingId]);

  return (
    <div className="grid gap-5 xl:grid-cols-[390px_minmax(0,1fr)]">
      <Card className="h-fit"><CardHeader className="flex flex-row items-center justify-between"><CardTitle>{editing ? "Sửa hoạt động" : "Thêm hoạt động"}</CardTitle>{editing ? <Button size="icon" variant="ghost" onClick={resetForm}><X size={14} /></Button> : null}</CardHeader><CardContent><form className="space-y-3" onSubmit={submit}>
        <label className="block space-y-1"><span className="text-[11px] font-medium">UserId *</span><Input value={form.userId} disabled={Boolean(editing)} onChange={(event) => setForm((v) => ({ ...v, userId: event.target.value }))} /></label>
        <label className="block space-y-1"><span className="text-[11px] font-medium">Loại hoạt động</span><select className="h-10 w-full rounded-md border px-3 text-[11px]" value={form.activityType} onChange={(event) => setForm((v) => ({ ...v, activityType: Number(event.target.value) as LearningActivityType }))}>{Object.entries(LEARNING_ACTIVITY_TYPE_LABELS).map(([key, label]) => <option key={key} value={key}>{label}</option>)}</select></label>
        <div className="grid grid-cols-2 gap-2">{numberField("lessonId", "Lesson ID")}{numberField("vocabularyId", "Vocabulary ID")}{numberField("quizAttemptId", "Quiz Attempt ID")}{numberField("flashcardSessionId", "Flashcard Session ID")}</div>
        <div className="grid grid-cols-2 gap-2"><label className="space-y-1"><span className="text-[10px] font-medium">Thời lượng (giây)</span><Input type="number" min={0} value={form.durationSeconds} onChange={(event) => setForm((v) => ({ ...v, durationSeconds: Number(event.target.value) }))} /></label><label className="space-y-1"><span className="text-[10px] font-medium">XP</span><Input type="number" min={0} value={form.xpEarned} onChange={(event) => setForm((v) => ({ ...v, xpEarned: Number(event.target.value) }))} /></label></div>
        <label className="flex items-center gap-2 text-[11px]"><input type="checkbox" checked={form.isCompleted} onChange={(event) => setForm((v) => ({ ...v, isCompleted: event.target.checked }))} /> Đã hoàn thành</label>
        <label className="block space-y-1"><span className="text-[11px] font-medium">Metadata JSON</span><Textarea value={form.metadataJson ?? ""} onChange={(event) => setForm((v) => ({ ...v, metadataJson: event.target.value || null }))} /></label>
        <Button className="w-full gap-2" type="submit" loading={saving}><Plus size={14} />{editing ? "Lưu thay đổi" : "Tạo hoạt động"}</Button>
      </form></CardContent></Card>

      <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white"><DataTableToolbar left={<><DataTableSearch value={userId} onChange={(value) => { setUserId(value); setPage(1); }} placeholder="Lọc UserId..." /><select className="h-[38px] rounded-md border px-3 text-[11px]" value={typeFilter} onChange={(event) => { setTypeFilter(event.target.value); setPage(1); }}><option value="">Tất cả hoạt động</option>{Object.entries(LEARNING_ACTIVITY_TYPE_LABELS).map(([key, label]) => <option key={key} value={key}>{label}</option>)}</select><select className="h-[38px] rounded-md border px-3 text-[11px]" value={completedFilter} onChange={(event) => { setCompletedFilter(event.target.value); setPage(1); }}><option value="">Tất cả trạng thái</option><option value="true">Hoàn tất</option><option value="false">Chưa hoàn tất</option></select></>} right={<Button variant="outline" className="h-[38px] gap-2 text-[11px]" onClick={() => void load()}><RefreshCw size={14} />Làm mới</Button>} /><DataTable data={items} columns={columns} rowKey={(item) => item.id} loading={loading} selectable={false} page={page} pageSize={pageSize} totalItems={total} totalPages={totalPages} onPageChange={setPage} onPageSizeChange={(value) => { setPageSize(value); setPage(1); }} /></div>
    </div>
  );
}
