"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { RefreshCw, X } from "lucide-react";

import { DataTable } from "@/components/common/data-table/data-table";
import { DataTableActions } from "@/components/common/data-table/data-table-actions";
import { DataTableSearch } from "@/components/common/data-table/data-table-search";
import { DataTableToolbar } from "@/components/common/data-table/data-table-toolbar";
import { ErrorState } from "@/components/common/error-state";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import type { DataTableColumn } from "@/types/table.types";

import { systemApi } from "../system.api";
import type { AdminAuditLog } from "../system.types";

interface AuditPageData {
  items: AdminAuditLog[];
  total: number;
  totalPages: number;
}

function JsonBlock({ value }: { value: string | null }) {
  if (!value) return <span className="text-muted-foreground">—</span>;
  return <pre className="max-h-56 overflow-auto whitespace-pre-wrap rounded-md bg-muted p-3 text-[10px] leading-4">{value}</pre>;
}

export function AuditLogTable() {
  const [data, setData] = useState<AuditPageData>({ items: [], total: 0, totalPages: 1 });
  const [userId, setUserId] = useState("");
  const [action, setAction] = useState("");
  const [entityType, setEntityType] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);
  const [selected, setSelected] = useState<AdminAuditLog | null>(null);
  const [detailLoading, setDetailLoading] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await systemApi.listAuditLogs({
        userId: userId.trim() || undefined,
        action: action.trim() || undefined,
        entityType: entityType.trim() || undefined,
        sort: "-occurredAt",
        page,
        pageSize,
      });
      const total = result.total ?? result.totalCount ?? 0;
      setData({
        items: result.items ?? [],
        total,
        totalPages: result.totalPages ?? Math.max(1, Math.ceil(total / Math.max(1, result.pageSize ?? pageSize))),
      });
    } catch (caught) {
      setError(caught instanceof Error ? caught : new Error("Không thể tải audit log."));
      setData({ items: [], total: 0, totalPages: 1 });
    } finally {
      setLoading(false);
    }
  }, [action, entityType, page, pageSize, userId]);

  useEffect(() => {
    const timer = window.setTimeout(() => void load(), userId || action || entityType ? 250 : 0);
    return () => window.clearTimeout(timer);
  }, [action, entityType, load, userId]);

  async function openDetail(id: number) {
    setDetailLoading(true);
    try { setSelected(await systemApi.getAuditLog(id)); }
    catch { setSelected(data.items.find((item) => item.id === id) ?? null); }
    finally { setDetailLoading(false); }
  }

  const columns = useMemo<DataTableColumn<AdminAuditLog>[]>(() => [
    {
      id: "action",
      header: "Action",
      cell: (item) => <div><div className="text-[12px] font-semibold text-[#333]">{item.action}</div><div className="mt-0.5 text-[10px] text-muted-foreground">#{item.id} · {item.publicId.slice(0, 8)}…</div></div>,
    },
    {
      id: "entity",
      header: "Entity",
      width: "190px",
      cell: (item) => <div className="text-[11px]"><Badge>{item.entityType}</Badge><div className="mt-1 font-mono text-[10px] text-muted-foreground">{item.entityId ?? "—"}</div></div>,
    },
    {
      id: "user",
      header: "User",
      width: "160px",
      cell: (item) => <span className="font-mono text-[10px] text-[#666]">{item.userId ? `${item.userId.slice(0, 8)}…` : "System"}</span>,
    },
    {
      id: "network",
      header: "IP / Correlation",
      width: "180px",
      cell: (item) => <div className="text-[10px] text-[#666]"><div>{item.ipAddress || "—"}</div><div className="mt-0.5 truncate text-muted-foreground">{item.correlationId || "—"}</div></div>,
    },
    {
      id: "time",
      header: "Thời gian",
      width: "155px",
      cell: (item) => <span className="text-[10px] text-[#666]">{new Date(item.occurredAt).toLocaleString("vi-VN")}</span>,
    },
    {
      id: "actions",
      header: "Thao tác",
      align: "center",
      width: "80px",
      cell: (item) => <DataTableActions onView={() => void openDetail(item.id)} />,
    },
  ], [data.items]);

  if (error && !loading) return <ErrorState title="Không thể tải nhật ký hệ thống" description={error.message} onRetry={() => void load()} />;

  return (
    <div className="space-y-4">
      <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
        <DataTableToolbar
          left={
            <>
              <DataTableSearch value={action} onChange={(value) => { setAction(value); setPage(1); }} placeholder="Action..." />
              <Input className="h-[38px] w-[210px] text-[11px]" value={entityType} onChange={(event) => { setEntityType(event.target.value); setPage(1); }} placeholder="Entity type..." />
              <Input className="h-[38px] w-[240px] text-[11px]" value={userId} onChange={(event) => { setUserId(event.target.value); setPage(1); }} placeholder="UserId..." />
            </>
          }
          right={<Button variant="outline" className="h-[38px] gap-2 text-[11px]" onClick={() => void load()}><RefreshCw size={14} />Làm mới</Button>}
        />
        <DataTable
          data={data.items}
          columns={columns}
          rowKey={(item) => item.id}
          loading={loading}
          selectable={false}
          page={page}
          pageSize={pageSize}
          totalItems={data.total}
          totalPages={data.totalPages}
          onPageChange={setPage}
          onPageSizeChange={(value) => { setPageSize(value); setPage(1); }}
        />
      </div>

      {detailLoading ? <Card><CardContent className="py-6 text-center text-[11px] text-muted-foreground">Đang tải chi tiết...</CardContent></Card> : null}
      {selected && !detailLoading ? (
        <Card>
          <CardHeader className="flex flex-row items-start justify-between gap-3">
            <div><CardTitle>Audit #{selected.id} · {selected.action}</CardTitle><p className="mt-1 text-[10px] text-muted-foreground">{selected.entityType} / {selected.entityId ?? "—"}</p></div>
            <Button size="icon" variant="ghost" onClick={() => setSelected(null)} aria-label="Đóng"><X size={15} /></Button>
          </CardHeader>
          <CardContent className="grid gap-4 xl:grid-cols-3">
            <div><div className="mb-1 text-[10px] font-semibold uppercase text-muted-foreground">Old values</div><JsonBlock value={selected.oldValuesJson} /></div>
            <div><div className="mb-1 text-[10px] font-semibold uppercase text-muted-foreground">New values</div><JsonBlock value={selected.newValuesJson} /></div>
            <div><div className="mb-1 text-[10px] font-semibold uppercase text-muted-foreground">Changed properties</div><JsonBlock value={selected.changedPropertiesJson} /></div>
            <div className="xl:col-span-3 rounded-md border p-3 text-[10px] text-[#666]">
              <div><strong>User:</strong> {selected.userId ?? "System"}</div>
              <div className="mt-1"><strong>IP:</strong> {selected.ipAddress ?? "—"}</div>
              <div className="mt-1"><strong>Correlation:</strong> {selected.correlationId ?? "—"}</div>
              <div className="mt-1 break-all"><strong>User-Agent:</strong> {selected.userAgent ?? "—"}</div>
            </div>
          </CardContent>
        </Card>
      ) : null}
    </div>
  );
}
