"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { Activity, Brain, CheckCircle2, Clock3, RefreshCw, RotateCcw, XCircle } from "lucide-react";

import { UserDisplay } from "@/components/admin/entity-display";
import { UserSelector, VocabularySelector } from "@/components/admin/entity-selectors";
import { DataTable } from "@/components/common/data-table/data-table";
import { DataTableToolbar } from "@/components/common/data-table/data-table-toolbar";
import { EmptyState } from "@/components/common/empty-state";
import { ErrorState } from "@/components/common/error-state";
import { MetricCard } from "@/components/common/metric-card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";
import { Dialog } from "@/components/ui/dialog";
import { Select } from "@/components/ui/select";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Textarea } from "@/components/ui/textarea";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";
import type { DataTableColumn } from "@/types/table.types";

import { reviewApi } from "../review.api";
import type {
  AdminFlashcardSession,
  AdminFlashcardSessionDetail,
  AdminReviewDashboard,
  AdminReviewEvent,
  AdminUserReviewSummary,
  AdminVocabularyState,
} from "../review.types";

type Tab = "dashboard" | "states" | "sessions" | "events" | "user";

const learningStateLabels = ["Chưa bắt đầu", "Đang học", "Đã biết", "Thành thạo"];
const sessionStatusLabels = ["Đang chạy", "Hoàn thành", "Đã bỏ"];
const ratingLabels = ["Again", "Hard", "Good", "Easy"];

const statusOptions = [
  { value: "0", label: "Đang chạy" },
  { value: "1", label: "Hoàn thành" },
  { value: "2", label: "Đã bỏ" },
];

const resultOptions = [
  { value: "true", label: "Đúng" },
  { value: "false", label: "Sai" },
];

function formatDate(value: string | null | undefined) {
  return value ? new Date(value).toLocaleString("vi-VN") : "—";
}

function pagination(total: number, pageSize: number) {
  return Math.max(1, Math.ceil(total / Math.max(1, pageSize)));
}

export function ReviewAdminWorkspaceV2() {
  const [tab, setTab] = useState<Tab>("dashboard");

  return (
    <Tabs value={tab} onValueChange={(value) => setTab(value as Tab)}>
      <TabsList>
        <TabsTrigger value="dashboard">Dashboard ôn tập</TabsTrigger>
        <TabsTrigger value="states">Trạng thái từ vựng</TabsTrigger>
        <TabsTrigger value="sessions">Phiên Flashcard</TabsTrigger>
        <TabsTrigger value="events">Lịch sử ôn tập</TabsTrigger>
        <TabsTrigger value="user">Theo học viên</TabsTrigger>
      </TabsList>
      <TabsContent value="dashboard"><ReviewDashboardPanel /></TabsContent>
      <TabsContent value="states"><ReviewStatesPanel /></TabsContent>
      <TabsContent value="sessions"><FlashcardSessionsPanel /></TabsContent>
      <TabsContent value="events"><ReviewEventsPanel /></TabsContent>
      <TabsContent value="user"><UserReviewPanel /></TabsContent>
    </Tabs>
  );
}

function ReviewDashboardPanel() {
  const [data, setData] = useState<AdminReviewDashboard | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setData(await reviewApi.dashboard());
    } catch (caught) {
      setError(normalizeApiError(caught).message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { void load(); }, [load]);

  if (error && !loading) return <ErrorState description={error} onRetry={() => void load()} />;
  if (loading) {
    return (
      <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
        {Array.from({ length: 8 }, (_, index) => <Card key={index} className="h-[118px] animate-pulse bg-[#faf9f7]" />)}
      </div>
    );
  }
  if (!data) return <EmptyState title="Chưa có dữ liệu ôn tập" description="Dashboard sẽ xuất hiện khi hệ thống ghi nhận hoạt động ôn tập đầu tiên." />;

  return (
    <div className="space-y-4">
      <div className="flex justify-end">
        <Button variant="outline" size="sm" onClick={() => void load()}><RefreshCw size={14} className="mr-2" />Làm mới</Button>
      </div>
      <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
        <MetricCard title="Từ vựng đang theo dõi" value={data.totalVocabularyStates} icon={<Brain size={18} />} />
        <MetricCard title="Đến hạn ôn" value={data.dueReviews} icon={<Clock3 size={18} />} />
        <MetricCard title="Quá hạn" value={data.overdueReviews} icon={<Clock3 size={18} />} />
        <MetricCard title="Đã thành thạo" value={data.masteredVocabulary} icon={<CheckCircle2 size={18} />} />
        <MetricCard title="Lượt ôn hôm nay" value={data.reviewsToday} icon={<Activity size={18} />} />
        <MetricCard title="Đúng hôm nay" value={data.correctReviewsToday} icon={<CheckCircle2 size={18} />} />
        <MetricCard title="Sai hôm nay" value={data.wrongReviewsToday} icon={<XCircle size={18} />} />
        <MetricCard title="Độ chính xác hôm nay" value={Number(data.accuracyToday).toFixed(1)} suffix="%" />
        <MetricCard title="Flashcard đang chạy" value={data.activeFlashcardSessions} />
        <MetricCard title="Phiên hoàn thành hôm nay" value={data.completedFlashcardSessionsToday} />
        <MetricCard title="Phiên đã bỏ hôm nay" value={data.abandonedFlashcardSessionsToday} />
        <MetricCard title="Từ yêu thích" value={data.favoriteVocabulary} />
      </div>
    </div>
  );
}

function ReviewStatesPanel() {
  const [items, setItems] = useState<AdminVocabularyState[]>([]);
  const [userId, setUserId] = useState("");
  const [vocabularyId, setVocabularyId] = useState("");
  const [dueOnly, setDueOnly] = useState(false);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [resetTarget, setResetTarget] = useState<AdminVocabularyState | null>(null);
  const [reason, setReason] = useState("");
  const [resetting, setResetting] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await reviewApi.states.list({
        userId: userId || undefined,
        vocabularyId: vocabularyId ? Number(vocabularyId) : undefined,
        isDue: dueOnly || undefined,
        page,
        pageSize,
      });
      setItems(result.items ?? []);
      setTotal(result.total ?? result.totalCount ?? 0);
    } catch (caught) {
      setError(normalizeApiError(caught).message);
      setItems([]);
    } finally {
      setLoading(false);
    }
  }, [dueOnly, page, pageSize, userId, vocabularyId]);

  useEffect(() => { void load(); }, [load]);

  async function resetState() {
    if (!resetTarget || !reason.trim()) {
      appToast.error("Thiếu lý do", "Vui lòng ghi rõ lý do reset trạng thái ôn tập.");
      return;
    }
    setResetting(true);
    try {
      await reviewApi.states.reset(resetTarget.userId, resetTarget.vocabularyId, reason.trim());
      appToast.success("Đã reset trạng thái ôn tập.");
      setResetTarget(null);
      setReason("");
      await load();
    } catch (caught) {
      appToast.error("Không thể reset trạng thái", normalizeApiError(caught).message);
    } finally {
      setResetting(false);
    }
  }

  const columns = useMemo<DataTableColumn<AdminVocabularyState>[]>(() => [
    {
      id: "vocabulary",
      header: "Từ vựng",
      cell: (item) => <div><div className="font-semibold text-[#444]">{item.simplified} · {item.pinyin}</div><div className="text-[10px] text-[#999]">{item.primaryMeaningVi}</div></div>,
    },
    { id: "user", header: "Học viên", width: "220px", cell: (item) => <UserDisplay id={item.userId} /> },
    { id: "state", header: "Trạng thái", width: "130px", cell: (item) => <Badge variant={item.learningState === 3 ? "success" : "default"}>{learningStateLabels[item.learningState] ?? "Khác"}</Badge> },
    { id: "mastery", header: "Mastery", width: "90px", align: "center", cell: (item) => <span className="font-semibold">{Number(item.masteryScore).toFixed(1)}</span> },
    { id: "result", header: "Đúng / Sai", width: "100px", align: "center", cell: (item) => <span>{item.correctCount} / {item.wrongCount}</span> },
    { id: "next", header: "Lần ôn tiếp", width: "150px", cell: (item) => <span className="text-[10px]">{formatDate(item.nextReviewAt)}</span> },
    { id: "actions", header: "Thao tác", width: "90px", align: "center", cell: (item) => <Button size="icon" variant="dangerGhost" aria-label="Reset trạng thái" onClick={() => { setResetTarget(item); setReason(""); }}><RotateCcw size={13} /></Button> },
  ], []);

  return (
    <>
      <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
        <DataTableToolbar
          left={<div className="flex min-w-0 flex-1 flex-wrap gap-2"><UserSelector className="w-full sm:w-[260px]" value={userId} onValueChange={(value) => { setUserId(value); setPage(1); }} placeholder="Lọc theo học viên" /><VocabularySelector className="w-full sm:w-[300px]" value={vocabularyId} onValueChange={(value) => { setVocabularyId(value); setPage(1); }} placeholder="Lọc theo từ vựng" /><Button variant={dueOnly ? "secondary" : "outline"} size="sm" onClick={() => { setDueOnly((current) => !current); setPage(1); }}>Chỉ đến hạn</Button></div>}
          right={<Button variant="outline" size="sm" onClick={() => void load()}><RefreshCw size={14} className="mr-2" />Làm mới</Button>}
        />
        {error && !loading ? <ErrorState description={error} onRetry={() => void load()} /> : <DataTable data={items} columns={columns} rowKey={(item) => `${item.userId}-${item.vocabularyId}`} loading={loading} selectable={false} page={page} pageSize={pageSize} totalItems={total} totalPages={pagination(total, pageSize)} onPageChange={setPage} onPageSizeChange={(value) => { setPageSize(value); setPage(1); }} />}
      </div>

      <Dialog
        open={Boolean(resetTarget)}
        onOpenChange={(open) => { if (!open) { setResetTarget(null); setReason(""); } }}
        title="Reset trạng thái ôn tập"
        description={resetTarget ? `${resetTarget.simplified} · ${resetTarget.pinyin}` : undefined}
        footer={<div className="flex justify-end gap-2"><Button variant="outline" disabled={resetting} onClick={() => setResetTarget(null)}>Hủy</Button><Button variant="danger" loading={resetting} onClick={() => void resetState()}>Reset trạng thái</Button></div>}
      >
        <label className="block space-y-1.5"><span className="text-[12px] font-medium">Lý do *</span><Textarea value={reason} onChange={(event) => setReason(event.target.value)} placeholder="Ví dụ: điều chỉnh dữ liệu sai sau khi hỗ trợ học viên..." /></label>
      </Dialog>
    </>
  );
}

function FlashcardSessionsPanel() {
  const [items, setItems] = useState<AdminFlashcardSession[]>([]);
  const [userId, setUserId] = useState("");
  const [status, setStatus] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [detail, setDetail] = useState<AdminFlashcardSessionDetail | null>(null);
  const [detailLoading, setDetailLoading] = useState(false);
  const [abandonTarget, setAbandonTarget] = useState<AdminFlashcardSession | null>(null);
  const [abandoning, setAbandoning] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await reviewApi.sessions.list({ userId: userId || undefined, status: status ? Number(status) : undefined, page, pageSize });
      setItems(result.items ?? []);
      setTotal(result.total ?? result.totalCount ?? 0);
    } catch (caught) {
      setError(normalizeApiError(caught).message);
      setItems([]);
    } finally { setLoading(false); }
  }, [page, pageSize, status, userId]);

  useEffect(() => { void load(); }, [load]);

  async function openDetail(item: AdminFlashcardSession) {
    setDetailLoading(true);
    try { setDetail(await reviewApi.sessions.get(item.id)); }
    catch (caught) { appToast.error("Không thể tải phiên flashcard", normalizeApiError(caught).message); }
    finally { setDetailLoading(false); }
  }

  async function abandon() {
    if (!abandonTarget) return;
    setAbandoning(true);
    try {
      await reviewApi.sessions.abandon(abandonTarget.id);
      appToast.success("Đã kết thúc phiên flashcard.");
      setAbandonTarget(null);
      setDetail(null);
      await load();
    } catch (caught) { appToast.error("Không thể kết thúc phiên", normalizeApiError(caught).message); }
    finally { setAbandoning(false); }
  }

  const columns = useMemo<DataTableColumn<AdminFlashcardSession>[]>(() => [
    { id: "user", header: "Học viên", cell: (item) => <UserDisplay id={item.userId} /> },
    { id: "status", header: "Trạng thái", width: "120px", cell: (item) => <Badge variant={item.status === 1 ? "success" : item.status === 2 ? "danger" : "info"}>{sessionStatusLabels[item.status] ?? "Khác"}</Badge> },
    { id: "progress", header: "Tiến độ", width: "110px", align: "center", cell: (item) => <span>{item.currentIndex}/{item.totalItems}</span> },
    { id: "result", header: "Đúng / Sai", width: "100px", align: "center", cell: (item) => <span>{item.correctItems} / {item.wrongItems}</span> },
    { id: "accuracy", header: "Chính xác", width: "100px", align: "center", cell: (item) => <span className="font-semibold">{Number(item.accuracyPercent).toFixed(1)}%</span> },
    { id: "started", header: "Bắt đầu", width: "160px", cell: (item) => <span className="text-[10px]">{formatDate(item.startedAt)}</span> },
    { id: "actions", header: "Thao tác", width: "150px", align: "center", cell: (item) => <div className="flex justify-center gap-2"><Button size="sm" variant="outline" onClick={() => void openDetail(item)}>Chi tiết</Button>{item.status === 0 ? <Button size="sm" variant="dangerGhost" onClick={() => setAbandonTarget(item)}>Kết thúc</Button> : null}</div> },
  ], []);

  return (
    <>
      <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
        <DataTableToolbar left={<div className="flex min-w-0 flex-1 flex-wrap gap-2"><UserSelector className="w-full sm:w-[280px]" value={userId} onValueChange={(value) => { setUserId(value); setPage(1); }} placeholder="Lọc theo học viên" /><Select className="w-full sm:w-[180px]" value={status} options={statusOptions} clearable placeholder="Mọi trạng thái" onValueChange={(value) => { setStatus(value); setPage(1); }} /></div>} right={<Button variant="outline" size="sm" onClick={() => void load()}><RefreshCw size={14} className="mr-2" />Làm mới</Button>} />
        {error && !loading ? <ErrorState description={error} onRetry={() => void load()} /> : <DataTable data={items} columns={columns} rowKey={(item) => item.id} loading={loading} selectable={false} page={page} pageSize={pageSize} totalItems={total} totalPages={pagination(total, pageSize)} onPageChange={setPage} onPageSizeChange={(value) => { setPageSize(value); setPage(1); }} />}
      </div>

      <Dialog open={Boolean(detail) || detailLoading} onOpenChange={(open) => { if (!open) setDetail(null); }} title="Chi tiết phiên flashcard" size="lg">
        {detailLoading && !detail ? <div className="py-10 text-center text-[12px] text-[#999]">Đang tải chi tiết...</div> : detail ? <div className="space-y-4"><div className="grid gap-3 sm:grid-cols-3"><MetricCard title="Tổng mục" value={detail.totalItems} /><MetricCard title="Đúng" value={detail.correctItems} /><MetricCard title="Chính xác" value={Number(detail.accuracyPercent).toFixed(1)} suffix="%" /></div>{detail.items.length === 0 ? <EmptyState title="Phiên chưa có mục từ vựng" /> : <div className="overflow-hidden rounded-[9px] border border-[#e8e3dc]"><table className="w-full text-left text-[11px]"><thead className="bg-[#faf9f7]"><tr><th className="p-3">Từ vựng</th><th className="p-3">Rating</th><th className="p-3">Kết quả</th><th className="p-3">Phản hồi</th></tr></thead><tbody>{detail.items.map((item) => <tr key={item.id} className="border-t border-[#eee9e2]"><td className="p-3"><b>{item.simplified}</b> · {item.pinyin}<div className="text-[#999]">{item.primaryMeaningVi}</div></td><td className="p-3">{item.rating == null ? "—" : ratingLabels[item.rating] ?? item.rating}</td><td className="p-3">{item.wasCorrect == null ? "—" : item.wasCorrect ? <Badge variant="success">Đúng</Badge> : <Badge variant="danger">Sai</Badge>}</td><td className="p-3">{item.responseTimeMs == null ? "—" : `${item.responseTimeMs} ms`}</td></tr>)}</tbody></table></div>}</div> : null}
      </Dialog>

      <ConfirmDialog open={Boolean(abandonTarget)} title="Kết thúc phiên flashcard?" description="Phiên đang hoạt động sẽ được chuyển sang trạng thái đã bỏ. Dữ liệu đã trả lời vẫn được giữ lại." confirmLabel="Kết thúc phiên" loading={abandoning} onClose={() => setAbandonTarget(null)} onConfirm={abandon} />
    </>
  );
}

function ReviewEventsPanel() {
  const [items, setItems] = useState<AdminReviewEvent[]>([]);
  const [userId, setUserId] = useState("");
  const [vocabularyId, setVocabularyId] = useState("");
  const [correct, setCorrect] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await reviewApi.events.list({ userId: userId || undefined, vocabularyId: vocabularyId ? Number(vocabularyId) : undefined, wasCorrect: correct ? correct === "true" : undefined, page, pageSize });
      setItems(result.items ?? []);
      setTotal(result.total ?? result.totalCount ?? 0);
    } catch (caught) {
      setError(normalizeApiError(caught).message);
      setItems([]);
    } finally { setLoading(false); }
  }, [correct, page, pageSize, userId, vocabularyId]);

  useEffect(() => { void load(); }, [load]);

  const columns = useMemo<DataTableColumn<AdminReviewEvent>[]>(() => [
    { id: "vocabulary", header: "Từ vựng", cell: (item) => <div><div className="font-semibold">{item.simplified} · {item.pinyin}</div><div className="text-[10px] text-[#999]">{item.primaryMeaningVi}</div></div> },
    { id: "user", header: "Học viên", width: "210px", cell: (item) => <UserDisplay id={item.userId} /> },
    { id: "rating", header: "Rating", width: "90px", cell: (item) => <span>{ratingLabels[item.rating] ?? item.rating}</span> },
    { id: "result", header: "Kết quả", width: "90px", cell: (item) => item.wasCorrect ? <Badge variant="success">Đúng</Badge> : <Badge variant="danger">Sai</Badge> },
    { id: "mastery", header: "Mastery", width: "120px", cell: (item) => <span>{item.masteryBefore} → {item.masteryAfter}</span> },
    { id: "response", header: "Phản hồi", width: "100px", cell: (item) => <span>{item.responseTimeMs == null ? "—" : `${item.responseTimeMs} ms`}</span> },
    { id: "time", header: "Thời gian", width: "160px", cell: (item) => <span className="text-[10px]">{formatDate(item.reviewedAt)}</span> },
  ], []);

  return <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white"><DataTableToolbar left={<div className="flex min-w-0 flex-1 flex-wrap gap-2"><UserSelector className="w-full sm:w-[260px]" value={userId} onValueChange={(value) => { setUserId(value); setPage(1); }} placeholder="Lọc theo học viên" /><VocabularySelector className="w-full sm:w-[300px]" value={vocabularyId} onValueChange={(value) => { setVocabularyId(value); setPage(1); }} placeholder="Lọc theo từ vựng" /><Select className="w-full sm:w-[150px]" value={correct} options={resultOptions} clearable placeholder="Mọi kết quả" onValueChange={(value) => { setCorrect(value); setPage(1); }} /></div>} right={<Button variant="outline" size="sm" onClick={() => void load()}><RefreshCw size={14} className="mr-2" />Làm mới</Button>} />{error && !loading ? <ErrorState description={error} onRetry={() => void load()} /> : <DataTable data={items} columns={columns} rowKey={(item) => item.id} loading={loading} selectable={false} page={page} pageSize={pageSize} totalItems={total} totalPages={pagination(total, pageSize)} onPageChange={setPage} onPageSizeChange={(value) => { setPageSize(value); setPage(1); }} />}</div>;
}

function UserReviewPanel() {
  const [userId, setUserId] = useState("");
  const [data, setData] = useState<AdminUserReviewSummary | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async (id: string) => {
    if (!id) {
      setData(null);
      setError(null);
      return;
    }
    setLoading(true);
    setError(null);
    try { setData(await reviewApi.userSummary(id)); }
    catch (caught) { setData(null); setError(normalizeApiError(caught).message); }
    finally { setLoading(false); }
  }, []);

  useEffect(() => { void load(userId); }, [load, userId]);

  return (
    <Card>
      <CardHeader><CardTitle>Tổng hợp ôn tập theo học viên</CardTitle></CardHeader>
      <CardContent className="space-y-5">
        <UserSelector value={userId} onValueChange={setUserId} placeholder="Chọn học viên để xem tổng hợp" />
        {!userId ? <EmptyState title="Chưa chọn học viên" description="Tìm theo tên hoặc email để xem tiến độ ôn tập, độ chính xác và các từ đến hạn." /> : error && !loading ? <ErrorState description={error} onRetry={() => void load(userId)} /> : loading ? <div className="py-10 text-center text-[12px] text-[#999]">Đang tải tổng hợp...</div> : data ? <div className="space-y-4"><UserDisplay id={data.userId} /><div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4"><MetricCard title="Tổng từ" value={data.totalVocabulary} /><MetricCard title="Đang học" value={data.learningVocabulary} /><MetricCard title="Thành thạo" value={data.masteredVocabulary} /><MetricCard title="Đến hạn" value={data.dueVocabulary} /><MetricCard title="Quá hạn" value={data.overdueVocabulary} /><MetricCard title="Tổng lượt ôn" value={data.totalReviews} /><MetricCard title="Độ chính xác" value={Number(data.overallAccuracy).toFixed(1)} suffix="%" /><MetricCard title="Phiên đang chạy" value={data.activeFlashcardSessions} /></div><div className="text-[11px] text-[#888]">Lần ôn gần nhất: {formatDate(data.lastReviewedAt)}</div></div> : <EmptyState title="Chưa có dữ liệu ôn tập" />}
      </CardContent>
    </Card>
  );
}
