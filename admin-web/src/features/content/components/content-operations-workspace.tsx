"use client";

import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";
import { FileUp, RefreshCw, SearchCheck, Trash2 } from "lucide-react";

import { UserDisplay } from "@/components/admin/entity-display";
import { UserSelector } from "@/components/admin/entity-selectors";
import { DataTable } from "@/components/common/data-table/data-table";
import { DataTableToolbar } from "@/components/common/data-table/data-table-toolbar";
import { EmptyState } from "@/components/common/empty-state";
import { ErrorState } from "@/components/common/error-state";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";
import { Dialog } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Textarea } from "@/components/ui/textarea";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";
import type { DataTableColumn } from "@/types/table.types";

import { contentApi } from "../content.api";
import {
  type AdminContentImportJob,
  type AdminContentImportRow,
  type AdminContentReport,
  CONTENT_ENTITY_TYPE_LABELS,
  CONTENT_IMPORT_STATUS_LABELS,
  CONTENT_IMPORT_TYPE_LABELS,
  CONTENT_REPORT_REASON_LABELS,
  CONTENT_REPORT_STATUS_LABELS,
  ContentImportStatus,
  ContentImportType,
  ContentReportStatus,
} from "../content.types";

const reportStatusOptions = Object.entries(CONTENT_REPORT_STATUS_LABELS).map(([value, label]) => ({ value, label }));
const importStatusOptions = Object.entries(CONTENT_IMPORT_STATUS_LABELS).map(([value, label]) => ({ value, label }));
const importTypeOptions = Object.entries(CONTENT_IMPORT_TYPE_LABELS).map(([value, label]) => ({ value, label }));

function formatDate(value?: string | null) {
  return value ? new Date(value).toLocaleString("vi-VN") : "—";
}

function totalOf(result: { total?: number; totalCount?: number }) {
  return result.total ?? result.totalCount ?? 0;
}

export function ContentOperationsWorkspace() {
  const [tab, setTab] = useState("reports");

  return (
    <Tabs value={tab} onValueChange={setTab}>
      <TabsList>
        <TabsTrigger value="reports">Báo cáo nội dung</TabsTrigger>
        <TabsTrigger value="imports">Nhập dữ liệu</TabsTrigger>
      </TabsList>
      <TabsContent value="reports"><ReportsPanel /></TabsContent>
      <TabsContent value="imports"><ImportsPanel /></TabsContent>
    </Tabs>
  );
}

function ReportsPanel() {
  const [items, setItems] = useState<AdminContentReport[]>([]);
  const [status, setStatus] = useState("");
  const [userId, setUserId] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [total, setTotal] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [selected, setSelected] = useState<AdminContentReport | null>(null);
  const [actionTarget, setActionTarget] = useState<{ item: AdminContentReport; action: "resolve" | "reject" } | null>(null);
  const [resolutionNote, setResolutionNote] = useState("");
  const [loading, setLoading] = useState(true);
  const [workingId, setWorkingId] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true); setError(null);
    try {
      const result = await contentApi.reports.list({
        userId: userId || undefined,
        status: status === "" ? undefined : Number(status) as ContentReportStatus,
        page,
        pageSize,
      });
      const count = totalOf(result);
      setItems(result.items ?? []);
      setTotal(count);
      setTotalPages(result.totalPages ?? Math.max(1, Math.ceil(count / Math.max(1, result.pageSize ?? pageSize))));
    } catch (caught) {
      setItems([]); setTotal(0); setTotalPages(1); setError(normalizeApiError(caught).message);
    } finally { setLoading(false); }
  }, [page, pageSize, status, userId]);

  useEffect(() => { void load(); }, [load]);

  async function simpleAction(item: AdminContentReport, action: "review" | "reopen") {
    setWorkingId(item.id);
    try {
      if (action === "review") await contentApi.reports.startReview(item.id);
      else await contentApi.reports.reopen(item.id);
      appToast.success(action === "review" ? "Đã bắt đầu xử lý report." : "Đã mở lại report.");
      if (selected?.id === item.id) setSelected(await contentApi.reports.get(item.id));
      await load();
    } catch (caught) { appToast.error("Không thể cập nhật report", normalizeApiError(caught).message); }
    finally { setWorkingId(null); }
  }

  async function submitResolution(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!actionTarget) return;
    setWorkingId(actionTarget.item.id);
    try {
      if (actionTarget.action === "resolve") await contentApi.reports.resolve(actionTarget.item.id, resolutionNote);
      else await contentApi.reports.reject(actionTarget.item.id, resolutionNote);
      appToast.success(actionTarget.action === "resolve" ? "Đã giải quyết report." : "Đã từ chối report.");
      if (selected?.id === actionTarget.item.id) setSelected(await contentApi.reports.get(actionTarget.item.id));
      setActionTarget(null); setResolutionNote(""); await load();
    } catch (caught) { appToast.error("Không thể cập nhật report", normalizeApiError(caught).message); }
    finally { setWorkingId(null); }
  }

  const columns = useMemo<DataTableColumn<AdminContentReport>[]>(() => [
    {
      id: "entity",
      header: "Nội dung",
      cell: (item) => (
        <div className="min-w-0">
          <div className="max-w-[320px] truncate text-[12px] font-semibold">{item.entityDisplayName || CONTENT_ENTITY_TYPE_LABELS[item.entityType]}</div>
          <div className="mt-0.5 text-[10px] text-muted-foreground">{CONTENT_ENTITY_TYPE_LABELS[item.entityType]}</div>
        </div>
      ),
    },
    {
      id: "reason",
      header: "Lý do",
      width: "190px",
      cell: (item) => <span className="text-[11px]">{CONTENT_REPORT_REASON_LABELS[item.reason]}</span>,
    },
    {
      id: "user",
      header: "Người báo cáo",
      width: "220px",
      cell: (item) => <UserDisplay id={item.userId} label={item.userDisplayName} description={item.userEmail} />,
    },
    {
      id: "status",
      header: "Trạng thái",
      width: "130px",
      cell: (item) => <Badge variant={item.status === ContentReportStatus.Resolved ? "success" : item.status === ContentReportStatus.Rejected ? "danger" : "default"}>{CONTENT_REPORT_STATUS_LABELS[item.status]}</Badge>,
    },
    {
      id: "created",
      header: "Thời gian",
      width: "155px",
      cell: (item) => <span className="text-[10px]">{formatDate(item.createdAt)}</span>,
    },
    {
      id: "actions",
      header: "Thao tác",
      width: "250px",
      align: "right",
      cell: (item) => (
        <div className="flex justify-end gap-1">
          <Button size="sm" variant="outline" onClick={() => setSelected(item)}>Chi tiết</Button>
          {item.status === ContentReportStatus.Open ? <Button size="sm" disabled={workingId === item.id} onClick={() => void simpleAction(item, "review")}>Xử lý</Button> : null}
          {item.status === ContentReportStatus.InReview ? <><Button size="sm" disabled={workingId === item.id} onClick={() => { setResolutionNote(""); setActionTarget({ item, action: "resolve" }); }}>Giải quyết</Button><Button size="sm" variant="dangerGhost" disabled={workingId === item.id} onClick={() => { setResolutionNote(""); setActionTarget({ item, action: "reject" }); }}>Từ chối</Button></> : null}
          {item.status === ContentReportStatus.Resolved || item.status === ContentReportStatus.Rejected ? <Button size="sm" variant="outline" disabled={workingId === item.id} onClick={() => void simpleAction(item, "reopen")}>Mở lại</Button> : null}
        </div>
      ),
    },
  ], [workingId, selected?.id]);

  return (
    <>
      <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
        <DataTableToolbar
          left={<div className="flex min-w-0 flex-1 flex-wrap gap-2"><UserSelector className="w-full sm:w-[280px]" value={userId} onValueChange={(value) => { setUserId(value); setPage(1); }} placeholder="Lọc theo người báo cáo" /><Select className="w-full sm:w-[190px]" value={status} options={reportStatusOptions} clearable placeholder="Mọi trạng thái" onValueChange={(value) => { setStatus(value); setPage(1); }} /></div>}
          right={<Button variant="outline" className="gap-2" onClick={() => void load()}><RefreshCw size={14} />Làm mới</Button>}
        />
        {error && !loading ? <ErrorState description={error} onRetry={() => void load()} /> : <DataTable data={items} columns={columns} rowKey={(item) => item.id} loading={loading} selectable={false} page={page} pageSize={pageSize} totalItems={total} totalPages={totalPages} onPageChange={setPage} onPageSizeChange={(value) => { setPageSize(value); setPage(1); }} />}
      </div>

      <Dialog open={Boolean(selected)} onOpenChange={(open) => { if (!open) setSelected(null); }} title="Chi tiết báo cáo nội dung" description={selected?.entityDisplayName || undefined} size="lg">
        {selected ? <div className="space-y-4 text-[12px]">
          <div className="grid gap-3 md:grid-cols-2">
            <Info label="Nội dung" value={selected.entityDisplayName || CONTENT_ENTITY_TYPE_LABELS[selected.entityType]} />
            <Info label="Loại" value={CONTENT_ENTITY_TYPE_LABELS[selected.entityType]} />
            <Info label="Lý do" value={CONTENT_REPORT_REASON_LABELS[selected.reason]} />
            <Info label="Trạng thái" value={CONTENT_REPORT_STATUS_LABELS[selected.status]} />
            <Info label="Người báo cáo" value={selected.userDisplayName || selected.userEmail || "Không xác định"} />
            <Info label="Người xử lý" value={selected.resolvedByDisplayName || "—"} />
          </div>
          <Info label="Mô tả" value={selected.description || "—"} block />
          <Info label="Ghi chú xử lý" value={selected.resolutionNote || "—"} block />
        </div> : null}
      </Dialog>

      <Dialog open={Boolean(actionTarget)} onOpenChange={(open) => { if (!open && !workingId) setActionTarget(null); }} title={actionTarget?.action === "resolve" ? "Giải quyết báo cáo" : "Từ chối báo cáo"} description={actionTarget?.item.entityDisplayName || undefined} footer={<div className="flex justify-end gap-2"><Button variant="outline" disabled={Boolean(workingId)} onClick={() => setActionTarget(null)}>Hủy</Button><Button variant={actionTarget?.action === "reject" ? "danger" : "primary"} loading={Boolean(workingId)} onClick={() => document.getElementById("content-report-resolution-form")?.dispatchEvent(new Event("submit", { bubbles: true, cancelable: true }))}>{actionTarget?.action === "resolve" ? "Xác nhận giải quyết" : "Xác nhận từ chối"}</Button></div>}>
        <form id="content-report-resolution-form" onSubmit={submitResolution} className="space-y-2"><label className="block space-y-1"><span className="text-[11px] font-medium">Ghi chú xử lý</span><Textarea value={resolutionNote} onChange={(event) => setResolutionNote(event.target.value)} placeholder="Mô tả cách xử lý hoặc lý do từ chối..." /></label></form>
      </Dialog>
    </>
  );
}

function ImportsPanel() {
  const emptyForm = { importType: ContentImportType.Vocabulary, originalFileName: "", storagePath: "" };
  const [items, setItems] = useState<AdminContentImportJob[]>([]);
  const [status, setStatus] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [total, setTotal] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [rows, setRows] = useState<AdminContentImportRow[]>([]);
  const [selected, setSelected] = useState<AdminContentImportJob | null>(null);
  const [editing, setEditing] = useState<AdminContentImportJob | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<AdminContentImportJob | null>(null);
  const [formOpen, setFormOpen] = useState(false);
  const [form, setForm] = useState(emptyForm);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [rowsLoading, setRowsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true); setError(null);
    try {
      const result = await contentApi.imports.list({ status: status === "" ? undefined : Number(status) as ContentImportStatus, page, pageSize });
      const count = totalOf(result); setItems(result.items ?? []); setTotal(count); setTotalPages(result.totalPages ?? Math.max(1, Math.ceil(count / Math.max(1, result.pageSize ?? pageSize))));
    } catch (caught) { setItems([]); setTotal(0); setTotalPages(1); setError(normalizeApiError(caught).message); }
    finally { setLoading(false); }
  }, [page, pageSize, status]);
  useEffect(() => { void load(); }, [load]);

  function openCreate() { setEditing(null); setForm(emptyForm); setFormOpen(true); }
  function openEdit(item: AdminContentImportJob) { setEditing(item); setForm({ importType: item.importType, originalFileName: item.originalFileName, storagePath: item.storagePath }); setFormOpen(true); }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!form.originalFileName.trim() || !form.storagePath.trim()) { appToast.error("Thiếu nguồn import", "Vui lòng nhập tên file và storage path."); return; }
    setSaving(true);
    try {
      if (editing) await contentApi.imports.updateSource(editing.id, { originalFileName: form.originalFileName.trim(), storagePath: form.storagePath.trim() });
      else await contentApi.imports.create({ importType: form.importType, originalFileName: form.originalFileName.trim(), storagePath: form.storagePath.trim() });
      appToast.success(editing ? "Đã cập nhật nguồn import." : "Đã tạo import job."); setFormOpen(false); setEditing(null); await load();
    } catch (caught) { appToast.error("Không thể lưu import job", normalizeApiError(caught).message); }
    finally { setSaving(false); }
  }

  async function openRows(item: AdminContentImportJob) {
    setRowsLoading(true); setSelected(item); setRows([]);
    try { const [detail, data] = await Promise.all([contentApi.imports.get(item.id), contentApi.imports.rows(item.id)]); setSelected(detail); setRows(data); }
    catch (caught) { appToast.error("Không thể tải import rows", normalizeApiError(caught).message); }
    finally { setRowsLoading(false); }
  }

  async function confirmDelete() {
    if (!deleteTarget) return;
    setDeleting(true);
    try { await contentApi.imports.remove(deleteTarget.id); appToast.success("Đã xóa import job."); setDeleteTarget(null); if (selected?.id === deleteTarget.id) { setSelected(null); setRows([]); } await load(); }
    catch (caught) { appToast.error("Không thể xóa import job", normalizeApiError(caught).message); }
    finally { setDeleting(false); }
  }

  const columns = useMemo<DataTableColumn<AdminContentImportJob>[]>(() => [
    { id: "file", header: "Nguồn", cell: (item) => <div className="min-w-0"><div className="max-w-[300px] truncate text-[12px] font-semibold">{item.originalFileName}</div><div className="max-w-[320px] truncate text-[10px] text-muted-foreground">{item.storagePath}</div></div> },
    { id: "type", header: "Loại", width: "145px", cell: (item) => <span className="text-[11px]">{CONTENT_IMPORT_TYPE_LABELS[item.importType]}</span> },
    { id: "status", header: "Trạng thái", width: "150px", cell: (item) => <Badge variant={item.status === ContentImportStatus.Completed ? "success" : item.status === ContentImportStatus.Failed ? "danger" : "default"}>{CONTENT_IMPORT_STATUS_LABELS[item.status]}</Badge> },
    { id: "progress", header: "Tiến độ", width: "150px", cell: (item) => <div className="text-[11px]"><div>{item.processedRows}/{item.totalRows} dòng</div><div className="text-[10px] text-muted-foreground">{item.successRows} thành công · {item.failedRows} lỗi</div></div> },
    { id: "created", header: "Tạo lúc", width: "155px", cell: (item) => <span className="text-[10px]">{formatDate(item.createdAt)}</span> },
    { id: "actions", header: "Thao tác", width: "190px", align: "right", cell: (item) => <div className="flex justify-end gap-1"><Button size="sm" variant="outline" onClick={() => void openRows(item)}>Chi tiết</Button>{item.status === ContentImportStatus.Pending ? <><Button size="sm" variant="outline" onClick={() => openEdit(item)}>Nguồn</Button><Button size="icon" variant="dangerGhost" aria-label="Xóa import job" onClick={() => setDeleteTarget(item)}><Trash2 size={13} /></Button></> : null}</div> },
  ], [selected?.id]);

  return (
    <>
      <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
        <DataTableToolbar left={<Select className="w-full sm:w-[220px]" value={status} options={importStatusOptions} clearable placeholder="Mọi trạng thái" onValueChange={(value) => { setStatus(value); setPage(1); }} />} right={<div className="flex gap-2"><Button className="gap-2" onClick={openCreate}><FileUp size={14} />Tạo import job</Button><Button variant="outline" className="gap-2" onClick={() => void load()}><RefreshCw size={14} />Làm mới</Button></div>} />
        {error && !loading ? <ErrorState description={error} onRetry={() => void load()} /> : <DataTable data={items} columns={columns} rowKey={(item) => item.id} loading={loading} selectable={false} page={page} pageSize={pageSize} totalItems={total} totalPages={totalPages} onPageChange={setPage} onPageSizeChange={(value) => { setPageSize(value); setPage(1); }} />}
      </div>

      <Dialog open={formOpen} onOpenChange={(open) => { if (!saving) setFormOpen(open); }} title={editing ? "Cập nhật nguồn import" : "Tạo import job"} description="Chỉ tạo metadata job từ source đã có trong storage; không mock upload file." footer={<div className="flex justify-end gap-2"><Button variant="outline" disabled={saving} onClick={() => setFormOpen(false)}>Hủy</Button><Button loading={saving} onClick={() => document.getElementById("content-import-form")?.dispatchEvent(new Event("submit", { bubbles: true, cancelable: true }))}>{editing ? "Lưu nguồn" : "Tạo job"}</Button></div>}>
        <form id="content-import-form" className="space-y-3" onSubmit={submit}>
          {!editing ? <label className="block space-y-1"><span className="text-[11px] font-medium">Loại import</span><Select value={String(form.importType)} options={importTypeOptions} onValueChange={(value) => setForm((current) => ({ ...current, importType: Number(value) as ContentImportType }))} /></label> : null}
          <label className="block space-y-1"><span className="text-[11px] font-medium">Tên file gốc *</span><Input value={form.originalFileName} onChange={(event) => setForm((current) => ({ ...current, originalFileName: event.target.value }))} placeholder="vocabulary-2026-08.csv" /></label>
          <label className="block space-y-1"><span className="text-[11px] font-medium">Storage path *</span><Input value={form.storagePath} onChange={(event) => setForm((current) => ({ ...current, storagePath: event.target.value }))} placeholder="imports/vocabulary/..." /></label>
        </form>
      </Dialog>

      <Dialog open={Boolean(selected)} onOpenChange={(open) => { if (!open) { setSelected(null); setRows([]); } }} title={selected ? `Import: ${selected.originalFileName}` : "Chi tiết import"} description={selected ? CONTENT_IMPORT_STATUS_LABELS[selected.status] : undefined} size="xl">
        {rowsLoading ? <div className="py-12 text-center text-[12px] text-muted-foreground">Đang tải dữ liệu...</div> : rows.length === 0 ? <EmptyState title="Chưa có import row" description="Job chưa xử lý hoặc chưa phát sinh dữ liệu chi tiết." /> : <div className="space-y-2">{rows.map((row) => <div key={row.id} className="rounded-[9px] border p-3"><div className="flex flex-wrap items-center justify-between gap-2"><div className="flex items-center gap-2"><Badge variant={row.isSuccessful ? "success" : "danger"}>{row.isSuccessful ? "Thành công" : "Thất bại"}</Badge><span className="text-[11px] font-semibold">Dòng {row.rowNumber}</span></div><span className="text-[10px] text-muted-foreground">{formatDate(row.processedAt)}</span></div>{row.errorMessage ? <p className="mt-2 text-[11px] text-[#c43d38]">{row.errorCode ? `${row.errorCode}: ` : ""}{row.errorMessage}</p> : null}<pre className="mt-2 max-h-36 overflow-auto whitespace-pre-wrap rounded-md bg-[#faf9f7] p-2 text-[10px] text-[#666]">{row.sourceJson}</pre></div>)}</div>}
      </Dialog>

      <ConfirmDialog open={Boolean(deleteTarget)} title="Xóa import job?" description={deleteTarget ? `Job “${deleteTarget.originalFileName}” đang Pending và sẽ bị xóa.` : ""} confirmLabel="Xóa job" loading={deleting} onClose={() => setDeleteTarget(null)} onConfirm={confirmDelete} />
    </>
  );
}

function Info({ label, value, block = false }: { label: string; value: string; block?: boolean }) {
  return <div className={block ? "rounded-[9px] border p-3" : "rounded-[9px] border p-3"}><div className="text-[10px] font-medium uppercase tracking-wide text-muted-foreground">{label}</div><div className="mt-1 whitespace-pre-wrap text-[12px] text-[#444]">{value}</div></div>;
}
