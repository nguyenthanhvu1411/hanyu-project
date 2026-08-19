"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { RefreshCw, RotateCcw } from "lucide-react";

import { UserDisplay } from "@/components/admin/entity-display";
import { UserSelector } from "@/components/admin/entity-selectors";
import { DataTable } from "@/components/common/data-table/data-table";
import { DataTableToolbar } from "@/components/common/data-table/data-table-toolbar";
import { ErrorState } from "@/components/common/error-state";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Select } from "@/components/ui/select";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";
import type { DataTableColumn } from "@/types/table.types";

import { learningApi } from "../learning.api";
import type { AdminLearningSummary } from "../learning.types";

const hskOptions = Array.from({ length: 9 }, (_, index) => ({
  value: String(index + 1),
  label: `HSK ${index + 1}`,
}));

export function LearningSummariesAdmin() {
  const [items, setItems] = useState<AdminLearningSummary[]>([]);
  const [total, setTotal] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [selectedUserId, setSelectedUserId] = useState("");
  const [hsk, setHsk] = useState("");
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [workingUserId, setWorkingUserId] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setLoadError(null);
    try {
      const result = await learningApi.summaries.list({
        userId: selectedUserId || undefined,
        currentHskLevel: hsk ? Number(hsk) : undefined,
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
  }, [hsk, page, pageSize, selectedUserId]);

  useEffect(() => {
    void load();
  }, [load]);

  async function recompute(item: AdminLearningSummary) {
    setWorkingUserId(item.userId);
    try {
      await learningApi.summaries.recompute(item.userId);
      appToast.success("Đã tính lại tổng hợp học tập.");
      await load();
    } catch (caught) {
      appToast.error("Không thể tính lại dữ liệu", normalizeApiError(caught).message);
    } finally {
      setWorkingUserId(null);
    }
  }

  const columns = useMemo<DataTableColumn<AdminLearningSummary>[]>(() => [
    {
      id: "user",
      header: "Học viên",
      cell: (item) => <UserDisplay id={item.userId} label={item.userDisplayName} description={item.userEmail} />,
    },
    { id: "hsk", header: "HSK", width: "80px", align: "center", cell: (item) => <Badge>HSK {item.currentHskLevel}</Badge> },
    { id: "mastery", header: "Mastery", width: "100px", align: "center", cell: (item) => <span className="text-[11px] font-semibold">{Number(item.overallMasteryPercent).toFixed(1)}%</span> },
    { id: "learning", header: "Thời gian học", width: "120px", cell: (item) => <span className="text-[11px]">{Math.round(item.totalLearningSeconds / 60)} phút</span> },
    { id: "lesson", header: "Bài học", width: "90px", align: "center", cell: (item) => <span className="text-[11px]">{item.totalLessonsCompleted}</span> },
    { id: "vocab", header: "Từ vựng", width: "130px", cell: (item) => <div className="text-[11px]"><div>{item.totalVocabularyLearned} đã học</div><div className="text-muted-foreground">{item.totalVocabularyMastered} thành thạo</div></div> },
    { id: "quiz", header: "Quiz", width: "110px", cell: (item) => <div className="text-[11px]"><div>{item.totalQuizAttempts} lượt</div><div className="text-muted-foreground">{item.totalQuizPassed} đạt</div></div> },
    { id: "xp", header: "XP", width: "90px", align: "center", cell: (item) => <span className="text-[11px] font-semibold">{item.totalXp}</span> },
    { id: "updated", header: "Cập nhật", width: "150px", cell: (item) => <span className="text-[10px]">{new Date(item.updatedAt).toLocaleString("vi-VN")}</span> },
    {
      id: "actions",
      header: "Thao tác",
      width: "90px",
      align: "center",
      cell: (item) => (
        <Button
          size="icon"
          variant="outline"
          aria-label="Tính lại tổng hợp"
          title="Tính lại từ dữ liệu nguồn"
          disabled={workingUserId === item.userId}
          onClick={() => void recompute(item)}
        >
          <RotateCcw size={13} />
        </Button>
      ),
    },
  ], [workingUserId]);

  return (
    <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
      <DataTableToolbar
        left={(
          <div className="flex min-w-0 flex-1 flex-wrap gap-2">
            <UserSelector
              className="w-full sm:w-[280px]"
              value={selectedUserId}
              onValueChange={(value) => { setSelectedUserId(value); setPage(1); }}
              placeholder="Lọc theo học viên"
            />
            <Select
              className="w-full sm:w-[150px]"
              value={hsk}
              clearable
              options={hskOptions}
              placeholder="Mọi HSK"
              onValueChange={(value) => { setHsk(value); setPage(1); }}
            />
          </div>
        )}
        right={<Button variant="outline" className="h-[38px] gap-2 text-[11px]" onClick={() => void load()}><RefreshCw size={14} />Làm mới</Button>}
      />

      {loadError && !loading ? (
        <ErrorState description={loadError} onRetry={() => void load()} />
      ) : (
        <DataTable
          data={items}
          columns={columns}
          rowKey={(item) => item.userId}
          loading={loading}
          selectable={false}
          page={page}
          pageSize={pageSize}
          totalItems={total}
          totalPages={totalPages}
          onPageChange={setPage}
          onPageSizeChange={(value) => { setPageSize(value); setPage(1); }}
        />
      )}
    </div>
  );
}
