"use client";

import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";
import { Pencil, Plus, RefreshCw, Trash2, X } from "lucide-react";

import { UserDisplay } from "@/components/admin/entity-display";
import { UserSelector } from "@/components/admin/entity-selectors";
import { DataTable } from "@/components/common/data-table/data-table";
import { DataTableToolbar } from "@/components/common/data-table/data-table-toolbar";
import { ErrorState } from "@/components/common/error-state";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";
import { DatePicker } from "@/components/ui/date-picker";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
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

const statusOptions = Object.entries(LEARNING_GOAL_STATUS_LABELS).map(([value, label]) => ({
  value,
  label,
}));

export function LearningGoalsAdmin() {
  const [items, setItems] = useState<AdminLearningGoal[]>([]);
  const [total, setTotal] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [selectedUserId, setSelectedUserId] = useState("");
  const [status, setStatus] = useState<string>("");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [workingId, setWorkingId] = useState<number | null>(null);
  const [editing, setEditing] = useState<AdminLearningGoal | null>(null);
  const [deleting, setDeleting] = useState<AdminLearningGoal | null>(null);
  const [form, setForm] = useState<CreateLearningGoalRequest>(EMPTY_FORM);
  const [loadError, setLoadError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setLoadError(null);
    try {
      const result = await learningApi.goals.list({
        userId: selectedUserId || undefined,
        status: status === "" ? undefined : Number(status) as LearningGoalStatus,
        page,
        pageSize,
      });
      const count = result.total ?? result.totalCount ?? 0;
      setItems(result.items ?? []);
      setTotal(count);
      setTotalPages(result.totalPages ?? Math.max(1, Math.ceil(count / Math.max(1, result.pageSize ?? pageSize))));
    } catch (caught) {
      const message = normalizeApiError(caught).message;
      setLoadError(message);
      setItems([]);
      setTotal(0);
      setTotalPages(1);
    } finally {
      setLoading(false);
    }
  }, [page, pageSize, selectedUserId, status]);

  useEffect(() => {
    void load();
  }, [load]);

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
    if (!form.userId) {
      appToast.error("Chưa chọn học viên", "Vui lòng chọn học viên cho mục tiêu học tập.");
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
        await learningApi.goals.create(form);
        appToast.success("Đã tạo mục tiêu học tập.");
      }
      resetForm();
      await load();
    } catch (caught) {
      appToast.error(
        editing ? "Không thể cập nhật mục tiêu" : "Không thể tạo mục tiêu",
        normalizeApiError(caught).message,
      );
    } finally {
      setSaving(false);
    }
  }

  async function confirmRemove() {
    if (!deleting) return;
    setWorkingId(deleting.id);
    try {
      await learningApi.goals.remove(deleting.id);
      appToast.success("Đã xóa mục tiêu học tập.");
      setDeleting(null);
      await load();
    } catch (caught) {
      appToast.error("Không thể xóa mục tiêu", normalizeApiError(caught).message);
    } finally {
      setWorkingId(null);
    }
  }

  const columns = useMemo<DataTableColumn<AdminLearningGoal>[]>(() => [
    {
      id: "user",
      header: "Học viên",
      cell: (item) => (
        <UserDisplay
          id={item.userId}
          label={item.userDisplayName}
          description={item.userEmail}
        />
      ),
    },
    {
      id: "hsk",
      header: "Mục tiêu HSK",
      width: "110px",
      align: "center",
      cell: (item) => <Badge>HSK {item.targetHskLevel}</Badge>,
    },
    {
      id: "daily",
      header: "Hàng ngày",
      width: "160px",
      cell: (item) => (
        <div className="text-[11px]">
          <div>{item.dailyGoalMinutes} phút</div>
          <div className="text-muted-foreground">{item.dailyVocabularyGoal ?? 0} từ/ngày</div>
        </div>
      ),
    },
    {
      id: "weekly",
      header: "Tuần",
      width: "100px",
      align: "center",
      cell: (item) => <span className="text-[11px]">{item.weeklyLessonGoal ?? 0} bài</span>,
    },
    {
      id: "targetDate",
      header: "Hạn",
      width: "120px",
      cell: (item) => <span className="text-[11px]">{item.targetDate ?? "—"}</span>,
    },
    {
      id: "status",
      header: "Trạng thái",
      width: "130px",
      cell: (item) => (
        <Badge variant={item.status === LearningGoalStatus.Active ? "success" : "default"}>
          {LEARNING_GOAL_STATUS_LABELS[item.status]}
        </Badge>
      ),
    },
    {
      id: "actions",
      header: "Thao tác",
      width: "100px",
      align: "center",
      cell: (item) => (
        <div className="flex justify-center gap-1">
          <Button size="icon" variant="outline" aria-label="Sửa mục tiêu" onClick={() => edit(item)}>
            <Pencil size={13} />
          </Button>
          <Button
            size="icon"
            variant="dangerGhost"
            aria-label="Xóa mục tiêu"
            disabled={workingId === item.id}
            onClick={() => setDeleting(item)}
          >
            <Trash2 size={13} />
          </Button>
        </div>
      ),
    },
  ], [workingId]);

  return (
    <>
      <div className="grid gap-5 xl:grid-cols-[360px_minmax(0,1fr)]">
        <Card className="h-fit">
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle>{editing ? "Sửa mục tiêu" : "Thêm mục tiêu"}</CardTitle>
            {editing ? (
              <Button size="icon" variant="ghost" aria-label="Hủy chỉnh sửa" onClick={resetForm}>
                <X size={14} />
              </Button>
            ) : null}
          </CardHeader>
          <CardContent>
            <form className="space-y-3" onSubmit={submit}>
              <label className="block space-y-1">
                <span className="text-[11px] font-medium">Học viên *</span>
                <UserSelector
                  value={form.userId}
                  disabled={Boolean(editing)}
                  clearable={!editing}
                  onValueChange={(value) => setForm((current) => ({ ...current, userId: value }))}
                />
              </label>

              <div className="grid grid-cols-2 gap-2">
                <label className="space-y-1">
                  <span className="text-[10px] font-medium">HSK mục tiêu</span>
                  <Input type="number" min={1} max={9} value={form.targetHskLevel} onChange={(event) => setForm((v) => ({ ...v, targetHskLevel: Number(event.target.value) }))} />
                </label>
                <label className="space-y-1">
                  <span className="text-[10px] font-medium">Phút/ngày</span>
                  <Input type="number" min={1} value={form.dailyGoalMinutes} onChange={(event) => setForm((v) => ({ ...v, dailyGoalMinutes: Number(event.target.value) }))} />
                </label>
              </div>

              <div className="grid grid-cols-2 gap-2">
                <label className="space-y-1">
                  <span className="text-[10px] font-medium">Từ/ngày</span>
                  <Input type="number" min={0} value={form.dailyVocabularyGoal ?? ""} onChange={(event) => setForm((v) => ({ ...v, dailyVocabularyGoal: event.target.value ? Number(event.target.value) : null }))} />
                </label>
                <label className="space-y-1">
                  <span className="text-[10px] font-medium">Bài/tuần</span>
                  <Input type="number" min={0} value={form.weeklyLessonGoal ?? ""} onChange={(event) => setForm((v) => ({ ...v, weeklyLessonGoal: event.target.value ? Number(event.target.value) : null }))} />
                </label>
              </div>

              <label className="block space-y-1">
                <span className="text-[11px] font-medium">Ngày mục tiêu</span>
                <DatePicker value={form.targetDate ?? ""} onChange={(value) => setForm((v) => ({ ...v, targetDate: value || null }))} />
              </label>

              <Button className="w-full gap-2" type="submit" loading={saving}>
                <Plus size={14} />
                {editing ? "Lưu thay đổi" : "Tạo mục tiêu"}
              </Button>
            </form>
          </CardContent>
        </Card>

        <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
          <DataTableToolbar
            left={(
              <div className="flex min-w-0 flex-1 flex-wrap gap-2">
                <UserSelector
                  className="w-full sm:w-[280px]"
                  value={selectedUserId}
                  onValueChange={(value) => {
                    setSelectedUserId(value);
                    setPage(1);
                  }}
                  placeholder="Lọc theo học viên"
                />
                <Select
                  className="w-full sm:w-[190px]"
                  value={status}
                  clearable
                  options={statusOptions}
                  placeholder="Tất cả trạng thái"
                  onValueChange={(value) => {
                    setStatus(value);
                    setPage(1);
                  }}
                />
              </div>
            )}
            right={(
              <Button variant="outline" className="h-[38px] gap-2 text-[11px]" onClick={() => void load()}>
                <RefreshCw size={14} />
                Làm mới
              </Button>
            )}
          />

          {loadError && !loading ? (
            <ErrorState description={loadError} onRetry={() => void load()} />
          ) : (
            <DataTable
              data={items}
              columns={columns}
              rowKey={(item) => item.id}
              loading={loading}
              selectable={false}
              page={page}
              pageSize={pageSize}
              totalItems={total}
              totalPages={totalPages}
              onPageChange={setPage}
              onPageSizeChange={(value) => {
                setPageSize(value);
                setPage(1);
              }}
            />
          )}
        </div>
      </div>

      <ConfirmDialog
        open={Boolean(deleting)}
        title="Xóa mục tiêu học tập?"
        description="Mục tiêu này sẽ bị xóa khỏi hồ sơ học tập của học viên."
        confirmLabel="Xóa mục tiêu"
        loading={Boolean(deleting && workingId === deleting.id)}
        onClose={() => setDeleting(null)}
        onConfirm={confirmRemove}
      />
    </>
  );
}
