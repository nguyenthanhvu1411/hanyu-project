"use client";

import { useCallback, useEffect, useState } from "react";
import { RefreshCw } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";

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

function formatDate(value: string | null | undefined) {
  return value ? new Date(value).toLocaleString("vi-VN") : "—";
}

function getTotal(result: { total?: number; totalCount?: number }) {
  return result.total ?? result.totalCount ?? 0;
}

export function ReviewAdminWorkspace() {
  const [tab, setTab] = useState<Tab>("dashboard");

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap gap-2 rounded-[11px] border border-[#e8e3dc] bg-white p-2">
        {([
          ["dashboard", "Dashboard ôn tập"],
          ["states", "Review States"],
          ["sessions", "Flashcard Sessions"],
          ["events", "Review Events"],
          ["user", "User Review"],
        ] as const).map(([value, label]) => (
          <Button key={value} size="sm" variant={tab === value ? "default" : "ghost"} onClick={() => setTab(value)}>
            {label}
          </Button>
        ))}
      </div>

      {tab === "dashboard" ? <ReviewDashboardPanel /> : null}
      {tab === "states" ? <ReviewStatesPanel /> : null}
      {tab === "sessions" ? <FlashcardSessionsPanel /> : null}
      {tab === "events" ? <ReviewEventsPanel /> : null}
      {tab === "user" ? <UserReviewPanel /> : null}
    </div>
  );
}

function ReviewDashboardPanel() {
  const [data, setData] = useState<AdminReviewDashboard | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try { setData(await reviewApi.dashboard()); }
    catch (caught) { setError(caught instanceof Error ? caught.message : "Không thể tải dashboard ôn tập."); }
    finally { setLoading(false); }
  }, []);

  useEffect(() => { void load(); }, [load]);

  if (loading) return <Card><CardContent className="py-8 text-center text-sm text-muted-foreground">Đang tải dashboard...</CardContent></Card>;
  if (error || !data) return <Card><CardContent className="space-y-3 py-6"><p className="text-sm text-destructive">{error ?? "Không có dữ liệu."}</p><Button onClick={() => void load()}>Thử lại</Button></CardContent></Card>;

  const stats = [
    ["Vocabulary states", data.totalVocabularyStates],
    ["Đến hạn", data.dueReviews],
    ["Quá hạn", data.overdueReviews],
    ["Đang học", data.learningVocabulary],
    ["Đã biết", data.knownVocabulary],
    ["Thành thạo", data.masteredVocabulary],
    ["Yêu thích", data.favoriteVocabulary],
    ["Review hôm nay", data.reviewsToday],
    ["Đúng hôm nay", data.correctReviewsToday],
    ["Sai hôm nay", data.wrongReviewsToday],
    ["Accuracy hôm nay", `${data.accuracyToday}%`],
    ["Session đang chạy", data.activeFlashcardSessions],
    ["Session hoàn thành hôm nay", data.completedFlashcardSessionsToday],
    ["Session bỏ hôm nay", data.abandonedFlashcardSessionsToday],
  ];

  return (
    <div className="space-y-4">
      <div className="flex justify-end"><Button variant="outline" size="sm" onClick={() => void load()}><RefreshCw className="mr-2 h-4 w-4" />Làm mới</Button></div>
      <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
        {stats.map(([label, value]) => (
          <Card key={String(label)}><CardContent className="p-4"><div className="text-xs text-muted-foreground">{label}</div><div className="mt-2 text-2xl font-semibold">{value}</div></CardContent></Card>
        ))}
      </div>
    </div>
  );
}

function ReviewStatesPanel() {
  const [items, setItems] = useState<AdminVocabularyState[]>([]);
  const [q, setQ] = useState("");
  const [userId, setUserId] = useState("");
  const [dueOnly, setDueOnly] = useState(false);
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await reviewApi.states.list({ q: q || undefined, userId: userId || undefined, isDue: dueOnly || undefined, page, pageSize: 20 });
      setItems(result.items ?? []);
      setTotal(getTotal(result));
    } catch (caught) { setError(caught instanceof Error ? caught.message : "Không thể tải review states."); }
    finally { setLoading(false); }
  }, [dueOnly, page, q, userId]);

  useEffect(() => { const timer = window.setTimeout(() => void load(), 250); return () => window.clearTimeout(timer); }, [load]);

  async function reset(item: AdminVocabularyState) {
    const reason = window.prompt(`Lý do reset trạng thái của ${item.simplified}:`);
    if (!reason?.trim()) return;
    await reviewApi.states.reset(item.userId, item.vocabularyId, reason.trim());
    await load();
  }

  return (
    <Card>
      <CardHeader><CardTitle>Vocabulary Review States</CardTitle></CardHeader>
      <CardContent className="space-y-4">
        <div className="flex flex-wrap gap-2">
          <Input className="max-w-xs" value={q} onChange={(event) => { setQ(event.target.value); setPage(1); }} placeholder="Tìm Hanzi / Pinyin / nghĩa..." />
          <Input className="max-w-sm" value={userId} onChange={(event) => { setUserId(event.target.value); setPage(1); }} placeholder="UserId..." />
          <Button variant={dueOnly ? "default" : "outline"} onClick={() => { setDueOnly((value) => !value); setPage(1); }}>Chỉ đến hạn</Button>
          <Button variant="outline" onClick={() => void load()}><RefreshCw className="mr-2 h-4 w-4" />Làm mới</Button>
        </div>
        {error ? <p className="text-sm text-destructive">{error}</p> : null}
        <div className="overflow-x-auto rounded-md border">
          <table className="w-full text-left text-xs">
            <thead className="bg-muted/50"><tr><th className="p-3">Từ vựng</th><th className="p-3">State</th><th className="p-3">Mastery</th><th className="p-3">Đúng / Sai</th><th className="p-3">Next review</th><th className="p-3">User</th><th className="p-3 text-right">Thao tác</th></tr></thead>
            <tbody>{items.map((item) => <tr key={`${item.userId}-${item.vocabularyId}`} className="border-t"><td className="p-3"><div className="font-semibold">{item.simplified} · {item.pinyin}</div><div className="text-muted-foreground">{item.primaryMeaningVi}</div></td><td className="p-3"><Badge variant="secondary">{learningStateLabels[item.learningState] ?? item.learningState}</Badge>{item.isFavorite ? <span className="ml-2">★</span> : null}</td><td className="p-3">{item.masteryScore}</td><td className="p-3">{item.correctCount} / {item.wrongCount}</td><td className="p-3">{formatDate(item.nextReviewAt)}</td><td className="p-3 font-mono text-[10px]">{item.userId.slice(0, 8)}…</td><td className="p-3 text-right"><Button size="sm" variant="destructive" onClick={() => void reset(item)}>Reset</Button></td></tr>)}</tbody>
          </table>
        </div>
        <div className="flex items-center justify-between text-xs text-muted-foreground"><span>{loading ? "Đang tải..." : `${total} trạng thái`}</span><div className="flex gap-2"><Button size="sm" variant="outline" disabled={page <= 1} onClick={() => setPage((value) => value - 1)}>Trước</Button><span className="px-2 py-2">Trang {page}</span><Button size="sm" variant="outline" disabled={items.length < 20} onClick={() => setPage((value) => value + 1)}>Sau</Button></div></div>
      </CardContent>
    </Card>
  );
}

function FlashcardSessionsPanel() {
  const [items, setItems] = useState<AdminFlashcardSession[]>([]);
  const [userId, setUserId] = useState("");
  const [status, setStatus] = useState("");
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const [selected, setSelected] = useState<AdminFlashcardSessionDetail | null>(null);
  const [loading, setLoading] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const result = await reviewApi.sessions.list({ userId: userId || undefined, status: status === "" ? undefined : Number(status), page, pageSize: 20 });
      setItems(result.items ?? []);
      setTotal(getTotal(result));
    } finally { setLoading(false); }
  }, [page, status, userId]);

  useEffect(() => { void load(); }, [load]);

  async function open(id: number) { setSelected(await reviewApi.sessions.get(id)); }
  async function abandon(id: number) {
    if (!window.confirm("Bỏ phiên flashcard đang hoạt động này?")) return;
    await reviewApi.sessions.abandon(id);
    setSelected(null);
    await load();
  }

  return (
    <div className="space-y-4">
      <Card><CardHeader><CardTitle>Flashcard Sessions</CardTitle></CardHeader><CardContent className="space-y-4">
        <div className="flex flex-wrap gap-2"><Input className="max-w-sm" value={userId} onChange={(event) => { setUserId(event.target.value); setPage(1); }} placeholder="UserId..." /><select className="h-10 rounded-md border bg-background px-3 text-sm" value={status} onChange={(event) => { setStatus(event.target.value); setPage(1); }}><option value="">Mọi trạng thái</option><option value="0">Đang chạy</option><option value="1">Hoàn thành</option><option value="2">Đã bỏ</option></select><Button variant="outline" onClick={() => void load()}><RefreshCw className="mr-2 h-4 w-4" />Làm mới</Button></div>
        <div className="overflow-x-auto rounded-md border"><table className="w-full text-left text-xs"><thead className="bg-muted/50"><tr><th className="p-3">Session</th><th className="p-3">User</th><th className="p-3">Status</th><th className="p-3">Tiến độ</th><th className="p-3">Đúng/Sai</th><th className="p-3">Accuracy</th><th className="p-3">Bắt đầu</th><th className="p-3 text-right">Thao tác</th></tr></thead><tbody>{items.map((item) => <tr key={item.id} className="border-t"><td className="p-3">#{item.id}</td><td className="p-3 font-mono text-[10px]">{item.userId.slice(0, 8)}…</td><td className="p-3"><Badge variant="secondary">{sessionStatusLabels[item.status] ?? item.status}</Badge></td><td className="p-3">{item.currentIndex}/{item.totalItems}</td><td className="p-3">{item.correctItems}/{item.wrongItems}</td><td className="p-3">{item.accuracyPercent}%</td><td className="p-3">{formatDate(item.startedAt)}</td><td className="p-3 text-right"><Button size="sm" variant="outline" onClick={() => void open(item.id)}>Chi tiết</Button>{item.status === 0 ? <Button className="ml-2" size="sm" variant="destructive" onClick={() => void abandon(item.id)}>Abandon</Button> : null}</td></tr>)}</tbody></table></div>
        <div className="flex items-center justify-between text-xs text-muted-foreground"><span>{loading ? "Đang tải..." : `${total} sessions`}</span><div className="flex gap-2"><Button size="sm" variant="outline" disabled={page <= 1} onClick={() => setPage((value) => value - 1)}>Trước</Button><span className="px-2 py-2">Trang {page}</span><Button size="sm" variant="outline" disabled={items.length < 20} onClick={() => setPage((value) => value + 1)}>Sau</Button></div></div>
      </CardContent></Card>
      {selected ? <Card><CardHeader><CardTitle>Session #{selected.id}</CardTitle></CardHeader><CardContent className="space-y-3"><div className="grid gap-2 sm:grid-cols-3 text-xs"><div>User: <span className="font-mono">{selected.userId}</span></div><div>Mode: {selected.mode}</div><div>Source: {selected.sourceType}{selected.sourceId ? ` #${selected.sourceId}` : ""}</div></div><div className="overflow-x-auto rounded-md border"><table className="w-full text-left text-xs"><thead className="bg-muted/50"><tr><th className="p-3">#</th><th className="p-3">Từ vựng</th><th className="p-3">Answered</th><th className="p-3">Rating</th><th className="p-3">Đúng</th><th className="p-3">Response</th></tr></thead><tbody>{selected.items.map((item) => <tr key={item.id} className="border-t"><td className="p-3">{item.sortOrder}</td><td className="p-3"><b>{item.simplified}</b> · {item.pinyin}<div className="text-muted-foreground">{item.primaryMeaningVi}</div></td><td className="p-3">{item.isAnswered ? "Có" : "Chưa"}</td><td className="p-3">{item.rating == null ? "—" : ratingLabels[item.rating] ?? item.rating}</td><td className="p-3">{item.wasCorrect == null ? "—" : item.wasCorrect ? "✓" : "✕"}</td><td className="p-3">{item.responseTimeMs == null ? "—" : `${item.responseTimeMs} ms`}</td></tr>)}</tbody></table></div></CardContent></Card> : null}
    </div>
  );
}

function ReviewEventsPanel() {
  const [items, setItems] = useState<AdminReviewEvent[]>([]);
  const [userId, setUserId] = useState("");
  const [correct, setCorrect] = useState("");
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);

  const load = useCallback(async () => {
    const result = await reviewApi.events.list({ userId: userId || undefined, wasCorrect: correct === "" ? undefined : correct === "true", page, pageSize: 20 });
    setItems(result.items ?? []);
    setTotal(getTotal(result));
  }, [correct, page, userId]);

  useEffect(() => { void load(); }, [load]);

  return <Card><CardHeader><CardTitle>Review Events</CardTitle></CardHeader><CardContent className="space-y-4"><div className="flex flex-wrap gap-2"><Input className="max-w-sm" value={userId} onChange={(event) => { setUserId(event.target.value); setPage(1); }} placeholder="UserId..." /><select className="h-10 rounded-md border bg-background px-3 text-sm" value={correct} onChange={(event) => { setCorrect(event.target.value); setPage(1); }}><option value="">Đúng + Sai</option><option value="true">Chỉ đúng</option><option value="false">Chỉ sai</option></select><Button variant="outline" onClick={() => void load()}><RefreshCw className="mr-2 h-4 w-4" />Làm mới</Button></div><div className="overflow-x-auto rounded-md border"><table className="w-full text-left text-xs"><thead className="bg-muted/50"><tr><th className="p-3">Từ vựng</th><th className="p-3">Rating</th><th className="p-3">Kết quả</th><th className="p-3">Mastery</th><th className="p-3">Interval</th><th className="p-3">Response</th><th className="p-3">Thời gian</th></tr></thead><tbody>{items.map((item) => <tr key={item.id} className="border-t"><td className="p-3"><b>{item.simplified}</b> · {item.pinyin}<div className="text-muted-foreground">{item.primaryMeaningVi}</div></td><td className="p-3">{ratingLabels[item.rating] ?? item.rating}</td><td className="p-3">{item.wasCorrect ? <Badge>Đúng</Badge> : <Badge variant="destructive">Sai</Badge>}</td><td className="p-3">{item.masteryBefore} → {item.masteryAfter}</td><td className="p-3">{item.intervalBeforeMinutes ?? 0} → {item.intervalAfterMinutes} phút</td><td className="p-3">{item.responseTimeMs == null ? "—" : `${item.responseTimeMs} ms`}</td><td className="p-3">{formatDate(item.reviewedAt)}</td></tr>)}</tbody></table></div><div className="flex items-center justify-between text-xs text-muted-foreground"><span>{total} events</span><div className="flex gap-2"><Button size="sm" variant="outline" disabled={page <= 1} onClick={() => setPage((value) => value - 1)}>Trước</Button><span className="px-2 py-2">Trang {page}</span><Button size="sm" variant="outline" disabled={items.length < 20} onClick={() => setPage((value) => value + 1)}>Sau</Button></div></div></CardContent></Card>;
}

function UserReviewPanel() {
  const [userId, setUserId] = useState("");
  const [data, setData] = useState<AdminUserReviewSummary | null>(null);
  const [error, setError] = useState<string | null>(null);

  async function load() {
    if (!userId.trim()) return;
    setError(null);
    try { setData(await reviewApi.userSummary(userId.trim())); }
    catch (caught) { setData(null); setError(caught instanceof Error ? caught.message : "Không thể tải review summary."); }
  }

  return <Card><CardHeader><CardTitle>User Review Summary</CardTitle></CardHeader><CardContent className="space-y-4"><div className="flex gap-2"><Input value={userId} onChange={(event) => setUserId(event.target.value)} placeholder="Nhập UserId GUID..." /><Button onClick={() => void load()}>Tra cứu</Button></div>{error ? <p className="text-sm text-destructive">{error}</p> : null}{data ? <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">{[["Tổng từ", data.totalVocabulary],["Đang học", data.learningVocabulary],["Đã biết", data.knownVocabulary],["Thành thạo", data.masteredVocabulary],["Đến hạn", data.dueVocabulary],["Quá hạn", data.overdueVocabulary],["Yêu thích", data.favoriteVocabulary],["Tổng review", data.totalReviews],["Đúng", data.correctReviews],["Sai", data.wrongReviews],["Accuracy", `${data.overallAccuracy}%`],["Session active", data.activeFlashcardSessions]].map(([label, value]) => <div key={String(label)} className="rounded-md border p-4"><div className="text-xs text-muted-foreground">{label}</div><div className="mt-1 text-xl font-semibold">{value}</div></div>)}</div> : null}{data ? <div className="text-xs text-muted-foreground">Review gần nhất: {formatDate(data.lastReviewedAt)}</div> : null}</CardContent></Card>;
}
