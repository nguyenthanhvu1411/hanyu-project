"use client";

import { useCallback, useEffect, useState } from "react";
import { RefreshCw } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";

import { contentApi } from "../content.api";
import type { AdminContentImportJob, AdminContentImportRow, AdminContentReport } from "../content.types";

type Tab = "reports" | "imports";
const reportStatusLabels = ["Open", "In Review", "Resolved", "Rejected"];
const importTypeLabels = ["Vocabulary", "Vocabulary Example", "Lesson", "Quiz"];
const importStatusLabels = ["Pending", "Processing", "Completed", "Completed w/ errors", "Failed"];

function formatDate(value: string | null | undefined) { return value ? new Date(value).toLocaleString("vi-VN") : "—"; }
function totalOf(result: { total?: number; totalCount?: number }) { return result.total ?? result.totalCount ?? 0; }

export function ContentOperationsWorkspace() {
  const [tab, setTab] = useState<Tab>("reports");
  return <div className="space-y-4"><div className="flex gap-2 rounded-[11px] border bg-white p-2"><Button size="sm" variant={tab === "reports" ? "default" : "ghost"} onClick={() => setTab("reports")}>Content Reports</Button><Button size="sm" variant={tab === "imports" ? "default" : "ghost"} onClick={() => setTab("imports")}>Content Imports</Button></div>{tab === "reports" ? <ReportsPanel /> : <ImportsPanel />}</div>;
}

function ReportsPanel() {
  const [items, setItems] = useState<AdminContentReport[]>([]);
  const [status, setStatus] = useState("");
  const [userId, setUserId] = useState("");
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const [selected, setSelected] = useState<AdminContentReport | null>(null);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setError(null);
    try {
      const result = await contentApi.reports.list({ userId: userId || undefined, status: status === "" ? undefined : Number(status), page, pageSize: 20 });
      setItems(result.items ?? []); setTotal(totalOf(result));
    } catch (caught) { setError(caught instanceof Error ? caught.message : "Không thể tải content reports."); }
  }, [page, status, userId]);
  useEffect(() => { void load(); }, [load]);

  async function act(item: AdminContentReport, action: "review" | "resolve" | "reject" | "reopen") {
    let updated: AdminContentReport;
    if (action === "review") updated = await contentApi.reports.startReview(item.id);
    else if (action === "reopen") updated = await contentApi.reports.reopen(item.id);
    else {
      const note = window.prompt(action === "resolve" ? "Ghi chú xử lý:" : "Lý do từ chối:") ?? "";
      updated = action === "resolve" ? await contentApi.reports.resolve(item.id, note) : await contentApi.reports.reject(item.id, note);
    }
    setSelected(updated); await load();
  }

  return <div className="space-y-4"><Card><CardHeader><CardTitle>Content Reports</CardTitle></CardHeader><CardContent className="space-y-4"><div className="flex flex-wrap gap-2"><Input className="max-w-sm" value={userId} onChange={(e) => { setUserId(e.target.value); setPage(1); }} placeholder="UserId..." /><select className="h-10 rounded-md border bg-background px-3 text-sm" value={status} onChange={(e) => { setStatus(e.target.value); setPage(1); }}><option value="">Mọi trạng thái</option>{reportStatusLabels.map((label, index) => <option key={label} value={index}>{label}</option>)}</select><Button variant="outline" onClick={() => void load()}><RefreshCw className="mr-2 h-4 w-4" />Làm mới</Button></div>{error ? <p className="text-sm text-destructive">{error}</p> : null}<div className="overflow-x-auto rounded-md border"><table className="w-full text-left text-xs"><thead className="bg-muted/50"><tr><th className="p-3">ID</th><th className="p-3">Entity</th><th className="p-3">Reason</th><th className="p-3">Status</th><th className="p-3">User</th><th className="p-3">Created</th><th className="p-3 text-right">Thao tác</th></tr></thead><tbody>{items.map((item) => <tr key={item.id} className="border-t"><td className="p-3">#{item.id}</td><td className="p-3">Type {item.entityType} / #{item.entityId}</td><td className="p-3">{item.reason}</td><td className="p-3"><Badge variant="secondary">{reportStatusLabels[item.status] ?? item.status}</Badge></td><td className="p-3 font-mono text-[10px]">{item.userId.slice(0, 8)}…</td><td className="p-3">{formatDate(item.createdAt)}</td><td className="p-3 text-right"><Button size="sm" variant="outline" onClick={() => setSelected(item)}>Chi tiết</Button>{item.status === 0 ? <Button className="ml-2" size="sm" onClick={() => void act(item, "review")}>Start review</Button> : null}{item.status === 1 ? <><Button className="ml-2" size="sm" onClick={() => void act(item, "resolve")}>Resolve</Button><Button className="ml-2" size="sm" variant="destructive" onClick={() => void act(item, "reject")}>Reject</Button></> : null}{item.status >= 2 ? <Button className="ml-2" size="sm" variant="outline" onClick={() => void act(item, "reopen")}>Reopen</Button> : null}</td></tr>)}</tbody></table></div><div className="flex justify-between text-xs text-muted-foreground"><span>{total} reports</span><div className="flex gap-2"><Button size="sm" variant="outline" disabled={page <= 1} onClick={() => setPage((v) => v - 1)}>Trước</Button><span className="px-2 py-2">Trang {page}</span><Button size="sm" variant="outline" disabled={items.length < 20} onClick={() => setPage((v) => v + 1)}>Sau</Button></div></div></CardContent></Card>{selected ? <Card><CardHeader><CardTitle>Report #{selected.id}</CardTitle></CardHeader><CardContent className="space-y-2 text-sm"><p><b>Mô tả:</b> {selected.description || "—"}</p><p><b>Resolution:</b> {selected.resolutionNote || "—"}</p><p><b>Resolved by:</b> {selected.resolvedByUserId || "—"}</p><p><b>Resolved at:</b> {formatDate(selected.resolvedAt)}</p></CardContent></Card> : null}</div>;
}

function ImportsPanel() {
  const [items, setItems] = useState<AdminContentImportJob[]>([]);
  const [status, setStatus] = useState("");
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const [rows, setRows] = useState<AdminContentImportRow[]>([]);
  const [selected, setSelected] = useState<AdminContentImportJob | null>(null);

  const load = useCallback(async () => { const result = await contentApi.imports.list({ status: status === "" ? undefined : Number(status), page, pageSize: 20 }); setItems(result.items ?? []); setTotal(totalOf(result)); }, [page, status]);
  useEffect(() => { void load(); }, [load]);

  async function createJob() {
    const originalFileName = window.prompt("Tên file gốc:"); if (!originalFileName?.trim()) return;
    const storagePath = window.prompt("Storage path:"); if (!storagePath?.trim()) return;
    const rawType = window.prompt("Import type: 0 Vocabulary, 1 VocabularyExample, 2 Lesson, 3 Quiz", "0");
    if (rawType == null) return;
    await contentApi.imports.create({ importType: Number(rawType), originalFileName: originalFileName.trim(), storagePath: storagePath.trim() }); await load();
  }
  async function open(item: AdminContentImportJob) { setSelected(await contentApi.imports.get(item.id)); setRows(await contentApi.imports.rows(item.id)); }
  async function updateSource(item: AdminContentImportJob) { const name = window.prompt("Tên file:", item.originalFileName); if (!name?.trim()) return; const path = window.prompt("Storage path:", item.storagePath); if (!path?.trim()) return; await contentApi.imports.updateSource(item.id, { originalFileName: name.trim(), storagePath: path.trim() }); await load(); }
  async function remove(item: AdminContentImportJob) { if (!window.confirm(`Xóa import job #${item.id}?`)) return; await contentApi.imports.remove(item.id); setSelected(null); setRows([]); await load(); }

  return <div className="space-y-4"><Card><CardHeader><CardTitle>Content Import Jobs</CardTitle></CardHeader><CardContent className="space-y-4"><div className="flex flex-wrap gap-2"><select className="h-10 rounded-md border bg-background px-3 text-sm" value={status} onChange={(e) => { setStatus(e.target.value); setPage(1); }}><option value="">Mọi trạng thái</option>{importStatusLabels.map((label, index) => <option key={label} value={index}>{label}</option>)}</select><Button onClick={() => void createJob()}>Tạo import job</Button><Button variant="outline" onClick={() => void load()}><RefreshCw className="mr-2 h-4 w-4" />Làm mới</Button></div><div className="overflow-x-auto rounded-md border"><table className="w-full text-left text-xs"><thead className="bg-muted/50"><tr><th className="p-3">Job</th><th className="p-3">Type</th><th className="p-3">File</th><th className="p-3">Status</th><th className="p-3">Processed</th><th className="p-3">Success/Fail</th><th className="p-3 text-right">Thao tác</th></tr></thead><tbody>{items.map((item) => <tr key={item.id} className="border-t"><td className="p-3">#{item.id}</td><td className="p-3">{importTypeLabels[item.importType] ?? item.importType}</td><td className="p-3"><div>{item.originalFileName}</div><div className="max-w-xs truncate text-muted-foreground">{item.storagePath}</div></td><td className="p-3"><Badge variant="secondary">{importStatusLabels[item.status] ?? item.status}</Badge></td><td className="p-3">{item.processedRows}/{item.totalRows}</td><td className="p-3">{item.successRows}/{item.failedRows}</td><td className="p-3 text-right"><Button size="sm" variant="outline" onClick={() => void open(item)}>Rows</Button><Button className="ml-2" size="sm" variant="outline" onClick={() => void updateSource(item)}>Source</Button><Button className="ml-2" size="sm" variant="destructive" onClick={() => void remove(item)}>Xóa</Button></td></tr>)}</tbody></table></div><div className="flex justify-between text-xs text-muted-foreground"><span>{total} jobs</span><div className="flex gap-2"><Button size="sm" variant="outline" disabled={page <= 1} onClick={() => setPage((v) => v - 1)}>Trước</Button><span className="px-2 py-2">Trang {page}</span><Button size="sm" variant="outline" disabled={items.length < 20} onClick={() => setPage((v) => v + 1)}>Sau</Button></div></div></CardContent></Card>{selected ? <Card><CardHeader><CardTitle>Rows của job #{selected.id}</CardTitle></CardHeader><CardContent><div className="overflow-x-auto rounded-md border"><table className="w-full text-left text-xs"><thead className="bg-muted/50"><tr><th className="p-3">Row</th><th className="p-3">Kết quả</th><th className="p-3">Entity</th><th className="p-3">Error</th><th className="p-3">Source JSON</th></tr></thead><tbody>{rows.map((row) => <tr key={row.id} className="border-t"><td className="p-3">{row.rowNumber}</td><td className="p-3">{row.isSuccessful ? <Badge>Success</Badge> : <Badge variant="destructive">Failed</Badge>}</td><td className="p-3">{row.createdEntityId ?? "—"}</td><td className="p-3">{row.errorCode || "—"}<div className="text-muted-foreground">{row.errorMessage || ""}</div></td><td className="max-w-md p-3"><pre className="max-h-28 overflow-auto whitespace-pre-wrap text-[10px]">{row.sourceJson}</pre></td></tr>)}</tbody></table></div></CardContent></Card> : null}</div>;
}
