"use client";

import { useCallback, useEffect, useState } from "react";
import { RefreshCw } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";

import { aiAnalyticsApi } from "../ai-analytics.api";
import type {
  AdminAiCacheEntry,
  AdminAiConversation,
  AdminAiDashboard,
  AdminAiFeedback,
  AdminAiRequest,
  AdminAnalyticsDashboard,
  AdminDailyLearningStat,
  UserAnalyticsSummary,
} from "../ai-analytics.types";

type Tab = "ai-dashboard" | "requests" | "conversations" | "feedback" | "cache" | "analytics";
const requestStatusLabels = ["Pending", "Completed", "Failed", "Cancelled"];
const conversationStatusLabels = ["Active", "Archived"];
const feedbackLabels: Record<number, string> = { [-1]: "Negative", 0: "Neutral", 1: "Positive" };

function formatDate(value: string | null | undefined) { return value ? new Date(value).toLocaleString("vi-VN") : "—"; }
function totalOf(result: { total?: number; totalCount?: number }) { return result.total ?? result.totalCount ?? 0; }

export function AiAnalyticsWorkspace() {
  const [tab, setTab] = useState<Tab>("ai-dashboard");
  const tabs: Array<[Tab, string]> = [
    ["ai-dashboard", "AI Dashboard"],
    ["requests", "AI Requests"],
    ["conversations", "Conversations"],
    ["feedback", "Feedback"],
    ["cache", "AI Cache"],
    ["analytics", "Analytics"],
  ];
  return <div className="space-y-4"><div className="flex flex-wrap gap-2 rounded-[11px] border bg-white p-2">{tabs.map(([value, label]) => <Button key={value} size="sm" variant={tab === value ? "default" : "ghost"} onClick={() => setTab(value)}>{label}</Button>)}</div>{tab === "ai-dashboard" ? <AiDashboardPanel /> : null}{tab === "requests" ? <RequestsPanel /> : null}{tab === "conversations" ? <ConversationsPanel /> : null}{tab === "feedback" ? <FeedbackPanel /> : null}{tab === "cache" ? <CachePanel /> : null}{tab === "analytics" ? <AnalyticsPanel /> : null}</div>;
}

function AiDashboardPanel() {
  const [data, setData] = useState<AdminAiDashboard | null>(null);
  const load = useCallback(async () => setData(await aiAnalyticsApi.ai.dashboard()), []);
  useEffect(() => { void load(); }, [load]);
  const stats = data ? [["Requests hôm nay", data.requestsToday],["Completed", data.completedToday],["Failed", data.failedToday],["Cancelled", data.cancelledToday],["Input tokens", data.inputTokensToday],["Output tokens", data.outputTokensToday],["Total tokens", data.totalTokensToday],["Estimated cost", `$${data.estimatedCostUsdToday}`],["Avg latency", `${data.averageLatencyMs} ms`]] : [];
  return <div className="space-y-4"><div className="flex justify-end"><Button variant="outline" size="sm" onClick={() => void load()}><RefreshCw className="mr-2 h-4 w-4" />Làm mới</Button></div>{data ? <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">{stats.map(([label, value]) => <Card key={String(label)}><CardContent className="p-4"><div className="text-xs text-muted-foreground">{label}</div><div className="mt-2 text-2xl font-semibold">{value}</div></CardContent></Card>)}</div> : <Card><CardContent className="py-8 text-center text-sm text-muted-foreground">Đang tải...</CardContent></Card>}</div>;
}

function RequestsPanel() {
  const [items, setItems] = useState<AdminAiRequest[]>([]);
  const [userId, setUserId] = useState("");
  const [status, setStatus] = useState("");
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const [selected, setSelected] = useState<AdminAiRequest | null>(null);
  const load = useCallback(async () => { const result = await aiAnalyticsApi.ai.requests({ userId: userId || undefined, status: status === "" ? undefined : Number(status), page, pageSize: 20 }); setItems(result.items ?? []); setTotal(totalOf(result)); }, [page, status, userId]);
  useEffect(() => { void load(); }, [load]);
  async function cancel(item: AdminAiRequest) { if (!window.confirm(`Cancel AI request #${item.id}?`)) return; setSelected(await aiAnalyticsApi.ai.cancelRequest(item.id)); await load(); }
  return <div className="space-y-4"><Card><CardHeader><CardTitle>AI Requests</CardTitle></CardHeader><CardContent className="space-y-4"><div className="flex flex-wrap gap-2"><Input className="max-w-sm" value={userId} onChange={(e) => { setUserId(e.target.value); setPage(1); }} placeholder="UserId..." /><select className="h-10 rounded-md border bg-background px-3 text-sm" value={status} onChange={(e) => { setStatus(e.target.value); setPage(1); }}><option value="">Mọi trạng thái</option>{requestStatusLabels.map((label, index) => <option key={label} value={index}>{label}</option>)}</select><Button variant="outline" onClick={() => void load()}><RefreshCw className="mr-2 h-4 w-4" />Làm mới</Button></div><div className="overflow-x-auto rounded-md border"><table className="w-full text-left text-xs"><thead className="bg-muted/50"><tr><th className="p-3">Request</th><th className="p-3">Feature</th><th className="p-3">Provider / Model</th><th className="p-3">Status</th><th className="p-3">Tokens</th><th className="p-3">Cost / Latency</th><th className="p-3">Requested</th><th className="p-3 text-right">Thao tác</th></tr></thead><tbody>{items.map((item) => <tr key={item.id} className="border-t"><td className="p-3">#{item.id}</td><td className="p-3">{item.featureType}</td><td className="p-3">{item.provider}<div className="text-muted-foreground">{item.model}</div></td><td className="p-3"><Badge variant="secondary">{requestStatusLabels[item.status] ?? item.status}</Badge></td><td className="p-3">{item.totalTokens}</td><td className="p-3">{item.estimatedCostUsd == null ? "—" : `$${item.estimatedCostUsd}`} / {item.latencyMs == null ? "—" : `${item.latencyMs} ms`}</td><td className="p-3">{formatDate(item.requestedAt)}</td><td className="p-3 text-right"><Button size="sm" variant="outline" onClick={() => setSelected(item)}>Chi tiết</Button>{item.status === 0 ? <Button className="ml-2" size="sm" variant="destructive" onClick={() => void cancel(item)}>Cancel</Button> : null}</td></tr>)}</tbody></table></div><Pager page={page} setPage={setPage} canNext={items.length >= 20} label={`${total} requests`} /></CardContent></Card>{selected ? <Card><CardHeader><CardTitle>AI Request #{selected.id}</CardTitle></CardHeader><CardContent className="space-y-2 text-sm"><p><b>User:</b> {selected.userId || "System/anonymous"}</p><p><b>Prompt version:</b> {selected.promptVersion || "—"}</p><p><b>Error:</b> {selected.errorCode || "—"} {selected.errorMessage || ""}</p><p><b>Completed:</b> {formatDate(selected.completedAt)}</p></CardContent></Card> : null}</div>;
}

function ConversationsPanel() {
  const [items, setItems] = useState<AdminAiConversation[]>([]);
  const [userId, setUserId] = useState("");
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const load = useCallback(async () => { const result = await aiAnalyticsApi.ai.conversations({ userId: userId || undefined, page, pageSize: 20 }); setItems(result.items ?? []); setTotal(totalOf(result)); }, [page, userId]);
  useEffect(() => { void load(); }, [load]);
  return <Card><CardHeader><CardTitle>AI Conversations</CardTitle></CardHeader><CardContent className="space-y-4"><div className="flex gap-2"><Input className="max-w-sm" value={userId} onChange={(e) => { setUserId(e.target.value); setPage(1); }} placeholder="UserId..." /><Button variant="outline" onClick={() => void load()}><RefreshCw className="mr-2 h-4 w-4" />Làm mới</Button></div><div className="overflow-x-auto rounded-md border"><table className="w-full text-left text-xs"><thead className="bg-muted/50"><tr><th className="p-3">Conversation</th><th className="p-3">Title</th><th className="p-3">User</th><th className="p-3">Status</th><th className="p-3">Messages</th><th className="p-3">Last message</th></tr></thead><tbody>{items.map((item) => <tr key={item.id} className="border-t"><td className="p-3">#{item.id}</td><td className="p-3">{item.title || "Không có tiêu đề"}</td><td className="p-3 font-mono text-[10px]">{item.userId.slice(0, 8)}…</td><td className="p-3"><Badge variant="secondary">{conversationStatusLabels[item.status] ?? item.status}</Badge></td><td className="p-3">{item.messageCount}</td><td className="p-3">{formatDate(item.lastMessageAt)}</td></tr>)}</tbody></table></div><Pager page={page} setPage={setPage} canNext={items.length >= 20} label={`${total} conversations`} /></CardContent></Card>;
}

function FeedbackPanel() {
  const [items, setItems] = useState<AdminAiFeedback[]>([]);
  const [issueType, setIssueType] = useState("");
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const load = useCallback(async () => { const result = await aiAnalyticsApi.ai.feedback({ issueType: issueType || undefined, page, pageSize: 20 }); setItems(result.items ?? []); setTotal(totalOf(result)); }, [issueType, page]);
  useEffect(() => { void load(); }, [load]);
  return <Card><CardHeader><CardTitle>AI Feedback</CardTitle></CardHeader><CardContent className="space-y-4"><div className="flex gap-2"><Input className="max-w-sm" value={issueType} onChange={(e) => { setIssueType(e.target.value); setPage(1); }} placeholder="Issue type..." /><Button variant="outline" onClick={() => void load()}><RefreshCw className="mr-2 h-4 w-4" />Làm mới</Button></div><div className="overflow-x-auto rounded-md border"><table className="w-full text-left text-xs"><thead className="bg-muted/50"><tr><th className="p-3">ID</th><th className="p-3">Rating</th><th className="p-3">Issue</th><th className="p-3">Comment</th><th className="p-3">AI Request</th><th className="p-3">User</th><th className="p-3">Created</th></tr></thead><tbody>{items.map((item) => <tr key={item.id} className="border-t"><td className="p-3">#{item.id}</td><td className="p-3">{feedbackLabels[item.rating] ?? item.rating}</td><td className="p-3">{item.issueType || "—"}</td><td className="max-w-md p-3">{item.comment || "—"}</td><td className="p-3">#{item.aiRequestId}</td><td className="p-3 font-mono text-[10px]">{item.userId.slice(0, 8)}…</td><td className="p-3">{formatDate(item.createdAt)}</td></tr>)}</tbody></table></div><Pager page={page} setPage={setPage} canNext={items.length >= 20} label={`${total} feedback`} /></CardContent></Card>;
}

function CachePanel() {
  const [items, setItems] = useState<AdminAiCacheEntry[]>([]);
  const [model, setModel] = useState("");
  const [expiredOnly, setExpiredOnly] = useState(false);
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const load = useCallback(async () => { const result = await aiAnalyticsApi.ai.cache({ model: model || undefined, expired: expiredOnly || undefined, page, pageSize: 20 }); setItems(result.items ?? []); setTotal(totalOf(result)); }, [expiredOnly, model, page]);
  useEffect(() => { void load(); }, [load]);
  async function remove(id: number) { if (!window.confirm(`Xóa cache entry #${id}? Endpoint backend chỉ cho xóa cache expired.`)) return; await aiAnalyticsApi.ai.deleteCache(id); await load(); }
  async function clean() { if (!window.confirm("Xóa toàn bộ AI cache đã hết hạn?")) return; await aiAnalyticsApi.ai.deleteExpiredCache(); await load(); }
  return <Card><CardHeader><CardTitle>AI Cache</CardTitle></CardHeader><CardContent className="space-y-4"><div className="flex flex-wrap gap-2"><Input className="max-w-sm" value={model} onChange={(e) => { setModel(e.target.value); setPage(1); }} placeholder="Model..." /><Button variant={expiredOnly ? "default" : "outline"} onClick={() => { setExpiredOnly((v) => !v); setPage(1); }}>Chỉ expired</Button><Button variant="destructive" onClick={() => void clean()}>Dọn cache expired</Button><Button variant="outline" onClick={() => void load()}><RefreshCw className="mr-2 h-4 w-4" />Làm mới</Button></div><div className="overflow-x-auto rounded-md border"><table className="w-full text-left text-xs"><thead className="bg-muted/50"><tr><th className="p-3">ID</th><th className="p-3">Feature</th><th className="p-3">Model</th><th className="p-3">Prompt</th><th className="p-3">Hits</th><th className="p-3">Expires</th><th className="p-3">State</th><th className="p-3 text-right">Thao tác</th></tr></thead><tbody>{items.map((item) => <tr key={item.id} className="border-t"><td className="p-3">#{item.id}</td><td className="p-3">{item.featureType}</td><td className="p-3">{item.model}</td><td className="p-3">{item.promptVersion}</td><td className="p-3">{item.hitCount}</td><td className="p-3">{formatDate(item.expiresAt)}</td><td className="p-3">{item.isExpired ? <Badge variant="destructive">Expired</Badge> : <Badge>Active</Badge>}</td><td className="p-3 text-right">{item.isExpired ? <Button size="sm" variant="destructive" onClick={() => void remove(item.id)}>Xóa</Button> : <span className="text-muted-foreground">—</span>}</td></tr>)}</tbody></table></div><Pager page={page} setPage={setPage} canNext={items.length >= 20} label={`${total} cache entries`} /></CardContent></Card>;
}

function AnalyticsPanel() {
  const [dashboard, setDashboard] = useState<AdminAnalyticsDashboard | null>(null);
  const [daily, setDaily] = useState<AdminDailyLearningStat[]>([]);
  const [userId, setUserId] = useState("");
  const [summary, setSummary] = useState<UserAnalyticsSummary | null>(null);
  const load = useCallback(async () => { const [d, list] = await Promise.all([aiAnalyticsApi.analytics.dashboard(), aiAnalyticsApi.analytics.daily({ page: 1, pageSize: 20 })]); setDashboard(d); setDaily(list.items ?? []); }, []);
  useEffect(() => { void load(); }, [load]);
  async function loadUser() { if (!userId.trim()) return; setSummary(await aiAnalyticsApi.analytics.user(userId.trim())); }
  const stats = dashboard ? [["Active users", dashboard.activeUsersToday],["Learning seconds", dashboard.learningSecondsToday],["Lessons completed", dashboard.lessonsCompletedToday],["Vocabulary reviewed", dashboard.vocabularyReviewedToday],["Quiz attempts", dashboard.quizAttemptsToday],["Quiz passed", dashboard.quizPassedToday],["AI interactions", dashboard.aiInteractionsToday],["XP earned", dashboard.xpEarnedToday]] : [];
  return <div className="space-y-4">{dashboard ? <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">{stats.map(([label, value]) => <Card key={String(label)}><CardContent className="p-4"><div className="text-xs text-muted-foreground">{label}</div><div className="mt-2 text-2xl font-semibold">{value}</div></CardContent></Card>)}</div> : null}<Card><CardHeader><CardTitle>Daily Learning Stats</CardTitle></CardHeader><CardContent><div className="overflow-x-auto rounded-md border"><table className="w-full text-left text-xs"><thead className="bg-muted/50"><tr><th className="p-3">Date</th><th className="p-3">User</th><th className="p-3">Learning</th><th className="p-3">Lessons</th><th className="p-3">Vocabulary</th><th className="p-3">Review Đ/S</th><th className="p-3">Quiz</th><th className="p-3">AI</th><th className="p-3">XP</th></tr></thead><tbody>{daily.map((item) => <tr key={`${item.userId}-${item.date}`} className="border-t"><td className="p-3">{item.date}</td><td className="p-3 font-mono text-[10px]">{item.userId.slice(0, 8)}…</td><td className="p-3">{item.learningSeconds}s</td><td className="p-3">{item.lessonsCompleted}/{item.lessonsStarted}</td><td className="p-3">{item.vocabularyReviewed}/{item.vocabularyLearned}</td><td className="p-3">{item.correctReviews}/{item.wrongReviews}</td><td className="p-3">{item.quizPassed}/{item.quizAttempts}</td><td className="p-3">{item.aiInteractions}</td><td className="p-3">{item.xpEarned}</td></tr>)}</tbody></table></div></CardContent></Card><Card><CardHeader><CardTitle>User Analytics</CardTitle></CardHeader><CardContent className="space-y-4"><div className="flex gap-2"><Input value={userId} onChange={(e) => setUserId(e.target.value)} placeholder="UserId GUID..." /><Button onClick={() => void loadUser()}>Tra cứu</Button></div>{summary ? <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">{[["Learning seconds", summary.totalLearningSeconds],["Lessons completed", summary.lessonsCompleted],["Vocabulary reviewed", summary.vocabularyReviewed],["Vocabulary learned", summary.vocabularyLearned],["Quiz passed/attempts", `${summary.quizPassed}/${summary.quizAttempts}`],["AI interactions", summary.aiInteractions],["XP", summary.xpEarned],["Review accuracy", `${summary.reviewAccuracy}%`],["Current streak", summary.currentStreak],["Longest streak", summary.longestStreak],["Active days", summary.totalActiveDays],["Last learning", summary.lastLearningDate || "—"]].map(([label, value]) => <div key={String(label)} className="rounded-md border p-4"><div className="text-xs text-muted-foreground">{label}</div><div className="mt-1 text-lg font-semibold">{value}</div></div>)}</div> : null}</CardContent></Card></div>;
}

function Pager({ page, setPage, canNext, label }: { page: number; setPage: (value: number | ((value: number) => number)) => void; canNext: boolean; label: string }) {
  return <div className="flex items-center justify-between text-xs text-muted-foreground"><span>{label}</span><div className="flex gap-2"><Button size="sm" variant="outline" disabled={page <= 1} onClick={() => setPage((value) => value - 1)}>Trước</Button><span className="px-2 py-2">Trang {page}</span><Button size="sm" variant="outline" disabled={!canNext} onClick={() => setPage((value) => value + 1)}>Sau</Button></div></div>;
}
