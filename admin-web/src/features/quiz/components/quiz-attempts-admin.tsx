"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { CheckCircle2, Clock3, RefreshCw, Target, XCircle } from "lucide-react";

import { QuizSelector, UserSelector } from "@/components/admin/entity-selectors";
import { DataTable } from "@/components/common/data-table/data-table";
import { DataTableToolbar } from "@/components/common/data-table/data-table-toolbar";
import { EmptyState } from "@/components/common/empty-state";
import { ErrorState } from "@/components/common/error-state";
import { MetricCard } from "@/components/common/metric-card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Dialog } from "@/components/ui/dialog";
import { Select } from "@/components/ui/select";
import { normalizeApiError } from "@/lib/api/api-error";
import type { DataTableColumn } from "@/types/table.types";

import { quizAttemptsApi } from "../quiz-attempts.api";
import {
  QUIZ_ATTEMPT_STATUS_LABELS,
  QuizAttemptStatus,
  type AdminQuizAttempt,
  type AdminQuizAttemptDetail,
  type AdminQuizAttemptStatistics,
} from "../quiz-attempts.types";

const statusOptions = Object.entries(QUIZ_ATTEMPT_STATUS_LABELS).map(([value, label]) => ({ value, label }));
const resultOptions = [
  { value: "true", label: "Đạt" },
  { value: "false", label: "Không đạt" },
];

function statusVariant(status: QuizAttemptStatus): "info" | "success" | "warning" | "default" {
  if (status === QuizAttemptStatus.InProgress) return "info";
  if (status === QuizAttemptStatus.Submitted) return "success";
  if (status === QuizAttemptStatus.Expired) return "warning";
  return "default";
}

export function QuizAttemptsAdmin() {
  const [items, setItems] = useState<AdminQuizAttempt[]>([]);
  const [statistics, setStatistics] = useState<AdminQuizAttemptStatistics | null>(null);
  const [userId, setUserId] = useState("");
  const [quizId, setQuizId] = useState("");
  const [status, setStatus] = useState("");
  const [passed, setPassed] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [detail, setDetail] = useState<AdminQuizAttemptDetail | null>(null);
  const [detailLoading, setDetailLoading] = useState(false);

  const query = useMemo(() => ({
    userId: userId || undefined,
    quizId: quizId ? Number(quizId) : undefined,
    status: status ? Number(status) as QuizAttemptStatus : undefined,
    isPassed: passed ? passed === "true" : undefined,
  }), [passed, quizId, status, userId]);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const [listResult, statsResult] = await Promise.all([
        quizAttemptsApi.list({ ...query, page, pageSize }),
        quizAttemptsApi.statistics(query),
      ]);
      setItems(listResult.items ?? []);
      setTotal(listResult.total ?? listResult.totalCount ?? 0);
      setStatistics(statsResult);
    } catch (caught) {
      setError(normalizeApiError(caught).message);
      setItems([]);
      setStatistics(null);
    } finally {
      setLoading(false);
    }
  }, [page, pageSize, query]);

  useEffect(() => { void load(); }, [load]);

  async function openDetail(item: AdminQuizAttempt) {
    setDetailLoading(true);
    try {
      setDetail(await quizAttemptsApi.get(item.id));
    } catch (caught) {
      setError(normalizeApiError(caught).message);
    } finally {
      setDetailLoading(false);
    }
  }

  const columns = useMemo<DataTableColumn<AdminQuizAttempt>[]>(() => [
    {
      id: "user",
      header: "Học viên",
      cell: (item) => <div><div className="font-semibold text-[#444]">{item.userDisplayName}</div><div className="text-[10px] text-[#999]">{item.userEmail}</div></div>,
    },
    {
      id: "quiz",
      header: "Bài kiểm tra",
      cell: (item) => <div><div className="font-medium text-[#444]">{item.quizTitleVi}</div><div className="text-[10px] text-[#999]">Lần {item.attemptNumber}</div></div>,
    },
    {
      id: "status",
      header: "Trạng thái",
      width: "120px",
      cell: (item) => <Badge variant={statusVariant(item.status)}>{QUIZ_ATTEMPT_STATUS_LABELS[item.status]}</Badge>,
    },
    {
      id: "score",
      header: "Kết quả",
      width: "130px",
      cell: (item) => item.percentage == null ? <span className="text-[#aaa]">Chưa chấm</span> : <div><div className="font-semibold">{Number(item.percentage).toFixed(1)}%</div><div className="text-[10px] text-[#999]">{item.score ?? 0}/{item.maxScore ?? 0} điểm</div></div>,
    },
    {
      id: "passed",
      header: "Đánh giá",
      width: "100px",
      cell: (item) => item.isPassed == null ? <span className="text-[#aaa]">—</span> : item.isPassed ? <Badge variant="success">Đạt</Badge> : <Badge variant="danger">Không đạt</Badge>,
    },
    {
      id: "answers",
      header: "Đúng / Sai / Bỏ",
      width: "120px",
      align: "center",
      cell: (item) => <span>{item.correctAnswers} / {item.wrongAnswers} / {item.unansweredQuestions}</span>,
    },
    {
      id: "started",
      header: "Bắt đầu",
      width: "160px",
      cell: (item) => <span className="text-[10px]">{new Date(item.startedAt).toLocaleString("vi-VN")}</span>,
    },
    {
      id: "actions",
      header: "Thao tác",
      width: "90px",
      align: "center",
      cell: (item) => <Button size="sm" variant="outline" onClick={() => void openDetail(item)}>Chi tiết</Button>,
    },
  ], []);

  return (
    <div className="space-y-5">
      {statistics ? (
        <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4 xl:grid-cols-7">
          <MetricCard title="Tổng lượt" value={statistics.totalAttempts} icon={<Target size={18} />} />
          <MetricCard title="Đang làm" value={statistics.inProgressAttempts} icon={<Clock3 size={18} />} />
          <MetricCard title="Đã nộp" value={statistics.submittedAttempts} />
          <MetricCard title="Đạt" value={statistics.passedAttempts} icon={<CheckCircle2 size={18} />} />
          <MetricCard title="Không đạt" value={statistics.failedAttempts} icon={<XCircle size={18} />} />
          <MetricCard title="Điểm TB" value={Number(statistics.averagePercentage).toFixed(1)} suffix="%" />
          <MetricCard title="Tỷ lệ đạt" value={Number(statistics.passRatePercent).toFixed(1)} suffix="%" />
        </div>
      ) : null}

      <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
        <DataTableToolbar
          left={<div className="flex min-w-0 flex-1 flex-wrap gap-2"><UserSelector className="w-full sm:w-[260px]" value={userId} onValueChange={(value) => { setUserId(value); setPage(1); }} placeholder="Lọc theo học viên" /><QuizSelector className="w-full sm:w-[300px]" value={quizId} onValueChange={(value) => { setQuizId(value); setPage(1); }} placeholder="Lọc theo bài kiểm tra" /><Select className="w-full sm:w-[170px]" value={status} options={statusOptions} clearable placeholder="Mọi trạng thái" onValueChange={(value) => { setStatus(value); setPage(1); }} /><Select className="w-full sm:w-[150px]" value={passed} options={resultOptions} clearable placeholder="Mọi kết quả" onValueChange={(value) => { setPassed(value); setPage(1); }} /></div>}
          right={<Button variant="outline" size="sm" onClick={() => void load()}><RefreshCw size={14} className="mr-2" />Làm mới</Button>}
        />

        {error && !loading ? (
          <ErrorState description={error} onRetry={() => void load()} />
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
            totalPages={Math.max(1, Math.ceil(total / Math.max(1, pageSize)))}
            onPageChange={setPage}
            onPageSizeChange={(value) => { setPageSize(value); setPage(1); }}
          />
        )}
      </div>

      <Dialog open={Boolean(detail) || detailLoading} onOpenChange={(open) => { if (!open) setDetail(null); }} title="Chi tiết lượt làm bài" size="lg">
        {detailLoading && !detail ? <div className="py-10 text-center text-[12px] text-[#999]">Đang tải chi tiết...</div> : detail ? <div className="space-y-5"><div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4"><MetricCard title="Học viên" value={detail.attempt.userDisplayName} description={detail.attempt.userEmail} /><MetricCard title="Bài kiểm tra" value={detail.attempt.quizTitleVi} description={`Lần ${detail.attempt.attemptNumber}`} /><MetricCard title="Điểm" value={detail.attempt.percentage == null ? "—" : Number(detail.attempt.percentage).toFixed(1)} suffix={detail.attempt.percentage == null ? undefined : "%"} /><MetricCard title="Thời lượng" value={detail.attempt.durationSeconds == null ? "—" : Math.round(detail.attempt.durationSeconds / 60)} suffix={detail.attempt.durationSeconds == null ? undefined : "phút"} /></div>{detail.answers.length === 0 ? <EmptyState title="Chưa có câu trả lời" description="Lượt làm bài này chưa ghi nhận câu trả lời nào." /> : <div className="space-y-2">{detail.answers.map((answer, index) => <div key={answer.id} className="rounded-[9px] border border-[#e8e3dc] p-3"><div className="flex items-start justify-between gap-3"><div><div className="text-[11px] font-semibold">Câu {index + 1}: {answer.questionPrompt}</div>{answer.questionPinyin ? <div className="mt-1 text-[10px] text-[#999]">{answer.questionPinyin}</div> : null}</div>{answer.isCorrect == null ? <Badge>Chưa trả lời</Badge> : answer.isCorrect ? <Badge variant="success">Đúng</Badge> : <Badge variant="danger">Sai</Badge>}</div><div className="mt-2 grid gap-2 text-[10px] text-[#777] sm:grid-cols-3"><span>Trả lời: {answer.answerText || "—"}</span><span>Điểm: {answer.earnedPoints ?? "—"}</span><span>Phản hồi: {answer.responseTimeMs == null ? "—" : `${answer.responseTimeMs} ms`}</span></div></div>)}</div>}</div> : null}
      </Dialog>
    </div>
  );
}