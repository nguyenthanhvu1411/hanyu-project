"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { RefreshCw, RotateCcw } from "lucide-react";

import { DataTable } from "@/components/common/data-table/data-table";
import { DataTableSearch } from "@/components/common/data-table/data-table-search";
import { DataTableToolbar } from "@/components/common/data-table/data-table-toolbar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";
import type { DataTableColumn } from "@/types/table.types";

import { learningApi } from "../learning.api";
import type { AdminLearningSummary } from "../learning.types";

export function LearningSummariesAdmin() {
  const [items, setItems] = useState<AdminLearningSummary[]>([]);
  const [total, setTotal] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [userId, setUserId] = useState("");
  const [hsk, setHsk] = useState("");
  const [loading, setLoading] = useState(true);
  const [workingUserId, setWorkingUserId] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const result = await learningApi.summaries.list({
        userId: userId.trim() || undefined,
        currentHskLevel: hsk ? Number(hsk) : undefined,
        page,
        pageSize,
      });
      const count = result.total ?? result.totalCount ?? 0;
      setItems(result.items ?? []);
      setTotal(count);
      setTotalPages(result.totalPages ?? Math.max(1, Math.ceil(count / Math.max(1, result.pageSize ?? pageSize))));
    } catch (caught) {
      appToast.error("Không thể tải tổng hợp học tập", normalizeApiError(caught).message);
      setItems([]);
      setTotal(0);
      setTotalPages(1);
    } finally {
      setLoading(false);
    }
  }, [hsk, page, pageSize, userId]);

  useEffect(() => {
    const timer = window.setTimeout(() => void load(), userId ? 250 : 0);
    return () => window.clearTimeout(timer);
  }, [load, userId]);

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
    { id: "user", header: "Học viên", cell: (item) => <span className="font-mono text-[10px]">{item.userId}</span> },
    { id: "hsk", header: "HSK", width: "80px", align: "center", cell: (item) => <Badge>HSK {item.currentHskLevel}</Badge> },
    { id: "mastery", header: "Mastery", width: "100px", align: "center", cell: (item) => <span className="text-[11px] font-semibold">{Number(item.overallMasteryPercent).toFixed(1)}%</span> },
    { id: "learning", header: "Thời gian học", width: "120px", cell: (item) => <span className="text-[10px]">{Math.round(item.totalLearningSeconds / 60)} phút</span> },
    { id: "lesson", header: "Bài học", width: "90px", align: "center", cell: (item) => <span className="text-[11px]">{item.totalLessonsCompleted}</span> },
    { id: "vocab", header: "Từ vựng", width: "130px", cell: (item) => <div className="text-[10px]"><div>{item.totalVocabularyLearned} đã học</div><div className="text-muted-foreground">{item.totalVocabularyMastered} mastered</div></div> },
    { id: "quiz", header: "Quiz", width: "110px", cell: (item) => <div className="text-[10px]"><div>{item.totalQuizAttempts} lượt</div><div className="text-muted-foreground">{item.totalQuizPassed} đạt</div></div> },
    { id: "xp", header: "XP", width: "90px", align: "center", cell: (item) => <span className="text-[11px] font-semibold">{item.totalXp}</span> },
    { id: "updated", header: "Cập nhật", width: "150px", cell: (item) => <span className="text-[10px]">{new Date(item.updatedAt).toLocaleString("vi-VN")}</span> },
    { id: "actions", header: "Thao tác", width: "90px", align: "center", cell: (item) => <Button size="icon" variant="outline" title="Tính lại" disabled={workingUserId === item.userId} onClick={() => void recompute(item)}><RotateCcw size={13} /></Button> },
  ], [workingUserId]);

  return (
    <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
      <DataTableToolbar
        left={<><DataTableSearch value={userId} onChange={(value) => { setUserId(value); setPage(1); }} placeholder="Lọc UserId..." /><Input className="h-[38px] w-[130px] text-[11px]" type="number" min={1} max={9} value={hsk} onChange={(event) => { setHsk(event.target.value); setPage(1); }} placeholder="HSK..." /></>}
        right={<Button variant="outline" className="h-[38px] gap-2 text-[11px]" onClick={() => void load()}><RefreshCw size={14} />Làm mới</Button>}
      />
      <DataTable data={items} columns={columns} rowKey={(item) => item.userId} loading={loading} selectable={false} page={page} pageSize={pageSize} totalItems={total} totalPages={totalPages} onPageChange={setPage} onPageSizeChange={(value) => { setPageSize(value); setPage(1); }} />
    </div>
  );
}
