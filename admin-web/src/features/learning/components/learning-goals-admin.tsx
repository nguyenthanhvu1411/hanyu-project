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
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";
import type { DataTableColumn } from "@/types/table.types";

import { learningApi } from "../learning.api";
import {
  LEARNING_GOAL_STATUS_LABELS,
  LearningGoalStatus,
  type AdminLearningGoal,
  type CreateLearningGoalRequest,
} from "../learning.types";

const EMPTY_FORM: CreateLearningGoalRequest = {
  userId: "",
  targetHskLevel: 1,
  targetDate: null,
  dailyGoalMinutes: 30,
  dailyVocabularyGoal: 10,
  weeklyLessonGoal: 3,
};

export function LearningGoalsAdmin() {
  const [items, setItems] = useState<AdminLearningGoal[]>([]);
  const [total, setTotal] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [userId, setUserId] = useState("");
  const [status, setStatus] = useState<string>("");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [workingId, setWorkingId] = useState<number | null>(null);
  const [editing, setEditing] = useState<AdminLearningGoal | null>(null);
  const [form, setForm] = useState<CreateLearningGoalRequest>(EMPTY_FORM);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const result = await learningApi.goals.list({
        userId: userId.trim() || undefined,
        status: status === "" ? undefined : Number(status) as LearningGoalStatus,
        page,
        pageSize,
      });
      const count = result.total ?? result.totalCount ?? 0;
      setItems(result.items ?? []);
      setTotal(count);
      setTotalPages(result.totalPages ?? Math.max(1, Math.ceil(count / Math.max(1, result.pageSize ?? pageSize))));
    } catch (caught) {
      appToast.error("Không thể tải mục tiêu học tập", normalizeApiError(caught).message);
      setItems([]);
      setTotal(0);
      setTotalPages(1);
    } finally {
      setLoading(false);
    }
  }, [page, pageSize, status, userId]);

  useEffect(() => {
    const timer = window.setTimeout(() => void load(), userId ? 250 : 0);
    return () => window.clearTimeout(timer);
  }, [load, userId]);

  function resetForm() {
    setEditing(null);
    setForm(EMPTY_FORM);
  }

  function edit(item: AdminLearningGoal) {
    setEditing(item);
    setForm({
      userId: item.userId,
      targetHskLevel: item.targetHskLevel,
      targetDate: item.targetDate,
      dailyGoalMinutes: item.dailyGoalMinutes,
      dailyVocabularyGoal: item.dailyVocabularyGoal,
      weeklyLessonGoal: item.weeklyLessonGoal,
    });
  }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!form.userId.trim()) {
      appToast.error("Thiếu UserId", "Vui lòng nhập UserId của học viên.");
      return;
    }
    setSaving(true);
    try {
      if (editing) {
        await learningApi.goals.update(editing.id, {
          targetHskLevel: form.targetHskLevel,
          targetDate: form.targetDate || null,
          dailyGoalMinutes: form.dailyGoalMinutes,
          dailyVocabularyGoal: form.dailyVocabularyGoal || null,
          weeklyLessonGoal: form.weeklyLessonGoal || null,
          status: editing.status,
        });
        appToast.success("Đã cập nhật mục tiêu học tập.");
      } else {
        await learningApi.goals.create({ ...form, userId: form.userId.trim() });
        appToast.success("Đã tạo mục tiêu học tập.");
      }
      resetForm();
      await load();
    } catch (caught) {
      appToast.error(editing ? "Không thể cập nhật mục tiêu" : "Không thể tạo mục tiêu", normalizeApiError(caught).message);
    } finally {
      setSaving(false);
    }
  }

  async function remove(item: AdminLearningGoal) {
    if (!window.confirm(`Xóa mục tiêu #${item.id}?`)) return;
    setWorkingId(item.id);
    try {
      await learningApi.goals.remove(item.id);
      appToast.success("Đã xóa mục tiêu học tập.");
      await load();
    } catch (caught) {
      appToast.error("Không thể xóa mục tiêu", normalizeApiError(caught).message);
    } finally {
      setWorkingId(null);
    }
  }

  const columns = useMemo<DataTableColumn<AdminLearningGoal>[]>(() => [
    { id: "user", header: "Học viên", cell: (item) => <span className="font-mono text-[10px]">{item.userId}</span> },
    { id: "hsk", header: "Mục tiêu HSK", width: "110px", align: "center", cell: (item) => <Badge>HSK {item.targetHskLevel}</Badge> },
    { id: "daily", header: "Hàng ngày", width: "160px", cell: (item) => <div className="text-[10px]"><div>{item.dailyGoalMinutes} phút</div><div className="text-muted-foreground">{item.dailyVocabularyGoal ?? 0} từ/ngày</div></div> },
    { id: "weekly", header: "Tuần", width: "100px", align: "center", cell: (item) => <span className="text-[11px]">{item.weeklyLessonGoal ?? 0} bài</span> },
    { id: "targetDate", header: "Hạn", width: "120px", cell: (item) => <span className="text-[10px]">{item.targetDate ?? "—"}</span> },
    { id: "status", header: "Trạng thái", width: "130px", cell: (item) => <Badge variant={item.status === LearningGoalStatus.Active ? "success" : "default"}>{LEARNING_GOAL_STATUS_LABELS[item.status]}</Badge> },
    {
      id: "actions", header: "Thao tác", width: "100px", align: "center",
      cell: (item) => <div className="flex justify-center gap-1"><Button size="icon" variant="outline" onClick={() => edit(item)}><Pencil size={13} /></Button><Button size="icon" variant="outline" disabled={workingId === item.id} onClick={() => void remove(item)}><Trash2 size={13} /></Button></div>,
    },
  ], [workingId]);

  return (
    <div className="grid gap-5 xl:grid-cols-[360px_minmax(0,1fr)]">
      <Card className="h-fit">
        <CardHeader className="flex flex-row items-center justify-between"><CardTitle>{editing ? "Sửa mục tiêu" : "Thêm mục tiêu"}</CardTitle>{editing ? <Button size="icon" variant="ghost" onClick={resetForm}><X size={14} /></Button> : null}</CardHeader>
        <CardContent>
          <form className="space-y-3" onSubmit={submit}>
            <label className="block space-y-1"><span className="text-[11px] font-medium">UserId *</span><Input value={form.userId} disabled={Boolean(editing)} onChange={(event) => setForm((v) => ({ ...v, userId: event.target.value }))} /></label>
            <div className="grid grid-cols-2 gap-2"><label className="space-y-1"><span className="text-[10px] font-medium">HSK mục tiêu</span><Input type="number" min={1} max={9} value={form.targetHskLevel} onChange={(event) => setForm((v) => ({ ...v, targetHskLevel: Number(event.target.value) }))} /></label><label className="space-y-1"><span className="text-[10px] font-medium">Phút/ngày</span><Input type="number" min={1} value={form.dailyGoalMinutes} onChange={(event) => setForm((v) => ({ ...v, dailyGoalMinutes: Number(event.target.value) }))} /></label></div>
            <div className="grid grid-cols-2 gap-2"><label className="space-y-1"><span className="text-[10px] font-medium">Từ/ngày</span><Input type="number" min={0} value={form.dailyVocabularyGoal ?? ""} onChange={(event) => setForm((v) => ({ ...v, dailyVocabularyGoal: event.target.value ? Number(event.target.value) : null }))} /></label><label className="space-y-1"><span className="text-[10px] font-medium">Bài/tuần</span><Input type="number" min={0} value={form.weeklyLessonGoal ?? ""} onChange={(event) => setForm((v) => ({ ...v, weeklyLessonGoal: event.target.value ? Number(event.target.value) : null }))} /></label></div>
            <label className="block space-y-1"><span className="text-[11px] font-medium">Ngày mục tiêu</span><Input type="date" value={form.targetDate ?? ""} onChange={(event) => setForm((v) => ({ ...v, targetDate: event.target.value || null }))} /></label>
            <Button className="w-full gap-2" type="submit" loading={saving}><Plus size={14} />{editing ? "Lưu thay đổi" : "Tạo mục tiêu"}</Button>
          </form>
        </CardContent>
      </Card>

      <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
        <DataTableToolbar
          left={<><DataTableSearch value={userId} onChange={(value) => { setUserId(value); setPage(1); }} placeholder="Lọc UserId..." /><select className="h-[38px] rounded-md border px-3 text-[11px]" value={status} onChange={(event) => { setStatus(event.target.value); setPage(1); }}><option value="">Tất cả trạng thái</option>{Object.entries(LEARNING_GOAL_STATUS_LABELS).map(([key, label]) => <option key={key} value={key}>{label}</option>)}</select></>}
          right={<Button variant="outline" className="h-[38px] gap-2 text-[11px]" onClick={() => void load()}><RefreshCw size={14} />Làm mới</Button>}
        />
        <DataTable data={items} columns={columns} rowKey={(item) => item.id} loading={loading} selectable={false} page={page} pageSize={pageSize} totalItems={total} totalPages={totalPages} onPageChange={setPage} onPageSizeChange={(value) => { setPageSize(value); setPage(1); }} />
      </div>
    </div>
  );
}
