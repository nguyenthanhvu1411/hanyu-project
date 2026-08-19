"use client";

import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";
import { Pencil, Plus, RefreshCw, Trash2, X } from "lucide-react";

import { LessonDisplay, UserDisplay, VocabularyDisplay } from "@/components/admin/entity-display";
import {
  FlashcardSessionSelector,
  LessonSelector,
  QuizAttemptSelector,
  UserSelector,
  VocabularySelector,
} from "@/components/admin/entity-selectors";
import { DataTable } from "@/components/common/data-table/data-table";
import { DataTableToolbar } from "@/components/common/data-table/data-table-toolbar";
import { ErrorState } from "@/components/common/error-state";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";
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

const activityTypeOptions = Object.entries(LEARNING_ACTIVITY_TYPE_LABELS).map(([value, label]) => ({ value, label }));
const completedOptions = [
  { value: "true", label: "Hoàn tất" },
  { value: "false", label: "Chưa hoàn tất" },
];

export function LearningActivitiesAdmin() {
  const [items, setItems] = useState<AdminLearningActivity[]>([]);
  const [total, setTotal] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [selectedUserId, setSelectedUserId] = useState("");
  const [typeFilter, setTypeFilter] = useState("");
  const [completedFilter, setCompletedFilter] = useState("");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [workingId, setWorkingId] = useState<number | null>(null);
  const [editing, setEditing] = useState<AdminLearningActivity | null>(null);
  const [deleting, setDeleting] = useState<AdminLearningActivity | null>(null);
  const [form, setForm] = useState<CreateLearningActivityRequest>(EMPTY_FORM);
  const [loadError, setLoadError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setLoadError(null);
    try {
      const result = await learningApi.activities.list({
        userId: selectedUserId || undefined,
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
      setLoadError(normalizeApiError(caught).message);
      setItems([]);
      setTotal(0);
      setTotalPages(1);
    } finally {
      setLoading(false);
    }
  }, [completedFilter, page, pageSize, selectedUserId, typeFilter]);

  useEffect(() => { void load(); }, [load]);

  function resetForm() {
    setEditing(null);
    setForm(EMPTY_FORM);
  }

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
    if (!form.userId) {
      appToast.error("Chưa chọn học viên", "Vui lòng chọn học viên cho hoạt động học tập.");
      return;
    }

    setSaving(true);
    try {
      if (editing) {
        const { userId: _userId, ...payload } = form;
        void _userId;
        await learningApi.activities.update(editing.id, payload);
        appToast.success("Đã cập nhật hoạt động học tập.");
      } else {
        await learningApi.activities.create(form);
        appToast.success("Đã tạo hoạt động học tập.");
      }
      resetForm();
      await load();
    } catch (caught) {
      appToast.error(editing ? "Không thể cập nhật hoạt động" : "Không thể tạo hoạt động", normalizeApiError(caught).message);
    } finally {
      setSaving(false);
    }
  }

  async function confirmRemove() {
    if (!deleting) return;
    setWorkingId(deleting.id);
    try {
      await learningApi.activities.remove(deleting.id);
      appToast.success("Đã xóa hoạt động.");
      setDeleting(null);
      await load();
    } catch (caught) {
      appToast.error("Không thể xóa hoạt động", normalizeApiError(caught).message);
    } finally {
      setWorkingId(null);
    }
  }

  const columns = useMemo<DataTableColumn<AdminLearningActivity>[]>(() => [
    { id: "type", header: "Hoạt động", cell: (item) => <div className="text-[12px] font-semibold">{LEARNING_ACTIVITY_TYPE_LABELS[item.activityType]}</div> },
    { id: "user", header: "Học viên", width: "220px", cell: (item) => <UserDisplay id={item.userId} label={item.userDisplayName} description={item.userEmail} /> },
    {
      id: "reference", header: "Nội dung liên quan", width: "240px",
      cell: (item) => {
        if (item.lessonId) return <LessonDisplay id={item.lessonId} label={item.lessonTitleVi} />;
        if (item.vocabularyId) return <VocabularyDisplay id={item.vocabularyId} label={item.vocabularySimplified ? `${item.vocabularySimplified}${item.vocabularyPinyin ? ` · ${item.vocabularyPinyin}` : ""}` : null} />;
        if (item.flashcardSessionId) return <span className="text-[11px] text-[#777]">Phiên flashcard</span>;
        if (item.quizAttemptId) return <span className="text-[11px] text-[#777]">Lượt làm bài</span>;
        return <span className="text-[#aaa]">—</span>;
      },
    },
    { id: "metrics", header: "Thời lượng / XP", width: "130px", cell: (item) => <div className="text-[11px]"><div>{item.durationSeconds}s</div><div className="text-muted-foreground">{item.xpEarned} XP</div></div> },
    { id: "status", header: "Trạng thái", width: "110px", cell: (item) => <Badge variant={item.isCompleted ? "success" : "default"}>{item.isCompleted ? "Hoàn tất" : "Đang làm"}</Badge> },
    { id: "time", header: "Bắt đầu", width: "150px", cell: (item) => <span className="text-[10px]">{new Date(item.startedAt).toLocaleString("vi-VN")}</span> },
    {
      id: "actions", header: "Thao tác", width: "100px", align: "center",
      cell: (item) => <div className="flex justify-center gap-1"><Button size="icon" variant="outline" aria-label="Sửa hoạt động" onClick={() => edit(item)}><Pencil size={13} /></Button><Button size="icon" variant="dangerGhost" aria-label="Xóa hoạt động" disabled={workingId === item.id} onClick={() => setDeleting(item)}><Trash2 size={13} /></Button></div>,
    },
  ], [workingId]);

  const showLesson = form.activityType === LearningActivityType.LessonStarted || form.activityType === LearningActivityType.LessonCompleted;
  const showVocabulary = form.activityType === LearningActivityType.VocabularyLearned || form.activityType === LearningActivityType.VocabularyReviewed;
  const showFlashcard = form.activityType === LearningActivityType.FlashcardStarted || form.activityType === LearningActivityType.FlashcardCompleted;
  const showQuiz = form.activityType === LearningActivityType.QuizStarted || form.activityType === LearningActivityType.QuizCompleted;

  return (
    <>
      <div className="grid gap-5 xl:grid-cols-[390px_minmax(0,1fr)]">
        <Card className="h-fit">
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle>{editing ? "Sửa hoạt động" : "Thêm hoạt động"}</CardTitle>
            {editing ? <Button size="icon" variant="ghost" aria-label="Hủy chỉnh sửa" onClick={resetForm}><X size={14} /></Button> : null}
          </CardHeader>
          <CardContent>
            <form className="space-y-3" onSubmit={submit}>
              <label className="block space-y-1"><span className="text-[11px] font-medium">Học viên *</span><UserSelector value={form.userId} disabled={Boolean(editing)} clearable={!editing} onValueChange={(value) => setForm((current) => ({ ...current, userId: value }))} /></label>
              <label className="block space-y-1"><span className="text-[11px] font-medium">Loại hoạt động</span><Select value={String(form.activityType)} options={activityTypeOptions} onValueChange={(value) => setForm((current) => ({ ...current, activityType: Number(value) as LearningActivityType, lessonId: null, vocabularyId: null, flashcardSessionId: null, quizAttemptId: null }))} /></label>

              {showLesson ? <label className="block space-y-1"><span className="text-[11px] font-medium">Bài học</span><LessonSelector value={form.lessonId ? String(form.lessonId) : ""} onValueChange={(value) => setForm((current) => ({ ...current, lessonId: value ? Number(value) : null }))} /></label> : null}
              {showVocabulary ? <label className="block space-y-1"><span className="text-[11px] font-medium">Từ vựng</span><VocabularySelector value={form.vocabularyId ? String(form.vocabularyId) : ""} onValueChange={(value) => setForm((current) => ({ ...current, vocabularyId: value ? Number(value) : null }))} /></label> : null}
              {showFlashcard ? <label className="block space-y-1"><span className="text-[11px] font-medium">Phiên flashcard</span><FlashcardSessionSelector userId={form.userId} value={form.flashcardSessionId ? String(form.flashcardSessionId) : ""} onValueChange={(value) => setForm((current) => ({ ...current, flashcardSessionId: value ? Number(value) : null }))} /></label> : null}
              {showQuiz ? <label className="block space-y-1"><span className="text-[11px] font-medium">Lượt làm bài</span><QuizAttemptSelector userId={form.userId} value={form.quizAttemptId ? String(form.quizAttemptId) : ""} onValueChange={(value) => setForm((current) => ({ ...current, quizAttemptId: value ? Number(value) : null }))} /></label> : null}

              <div className="grid grid-cols-2 gap-2"><label className="space-y-1"><span className="text-[10px] font-medium">Thời lượng (giây)</span><Input type="number" min={0} value={form.durationSeconds} onChange={(event) => setForm((v) => ({ ...v, durationSeconds: Number(event.target.value) }))} /></label><label className="space-y-1"><span className="text-[10px] font-medium">XP</span><Input type="number" min={0} value={form.xpEarned} onChange={(event) => setForm((v) => ({ ...v, xpEarned: Number(event.target.value) }))} /></label></div>
              <Switch checked={form.isCompleted} onCheckedChange={(checked) => setForm((v) => ({ ...v, isCompleted: checked }))} label="Đã hoàn thành" />
              <label className="block space-y-1"><span className="text-[11px] font-medium">Metadata JSON</span><Textarea value={form.metadataJson ?? ""} onChange={(event) => setForm((v) => ({ ...v, metadataJson: event.target.value || null }))} /></label>
              <Button className="w-full gap-2" type="submit" loading={saving}><Plus size={14} />{editing ? "Lưu thay đổi" : "Tạo hoạt động"}</Button>
            </form>
          </CardContent>
        </Card>

        <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
          <DataTableToolbar
            left={<div className="flex min-w-0 flex-1 flex-wrap gap-2"><UserSelector className="w-full sm:w-[280px]" value={selectedUserId} onValueChange={(value) => { setSelectedUserId(value); setPage(1); }} placeholder="Lọc theo học viên" /><Select className="w-full sm:w-[210px]" value={typeFilter} options={activityTypeOptions} clearable placeholder="Tất cả hoạt động" onValueChange={(value) => { setTypeFilter(value); setPage(1); }} /><Select className="w-full sm:w-[170px]" value={completedFilter} options={completedOptions} clearable placeholder="Mọi trạng thái" onValueChange={(value) => { setCompletedFilter(value); setPage(1); }} /></div>}
            right={<Button variant="outline" className="h-[38px] gap-2 text-[11px]" onClick={() => void load()}><RefreshCw size={14} />Làm mới</Button>}
          />
          {loadError && !loading ? <ErrorState description={loadError} onRetry={() => void load()} /> : <DataTable data={items} columns={columns} rowKey={(item) => item.id} loading={loading} selectable={false} page={page} pageSize={pageSize} totalItems={total} totalPages={totalPages} onPageChange={setPage} onPageSizeChange={(value) => { setPageSize(value); setPage(1); }} />}
        </div>
      </div>

      <ConfirmDialog open={Boolean(deleting)} title="Xóa hoạt động học tập?" description="Hoạt động đã ghi nhận sẽ bị xóa khỏi lịch sử học tập của học viên." confirmLabel="Xóa hoạt động" loading={Boolean(deleting && workingId === deleting.id)} onClose={() => setDeleting(null)} onConfirm={confirmRemove} />
    </>
  );
}
