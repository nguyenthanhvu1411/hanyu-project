"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { RefreshCw } from "lucide-react";

import { DataTable } from "@/components/common/data-table/data-table";
import { DataTableSearch } from "@/components/common/data-table/data-table-search";
import { DataTableToolbar } from "@/components/common/data-table/data-table-toolbar";
import { ErrorState } from "@/components/common/error-state";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import type { DataTableColumn } from "@/types/table.types";

import { systemApi } from "../system.api";
import type { AdminProductEvent } from "../system.types";

interface ProductEventPageData {
  items: AdminProductEvent[];
  total: number;
  totalPages: number;
}

export function ProductEventTable() {
  const [data, setData] = useState<ProductEventPageData>({ items: [], total: 0, totalPages: 1 });
  const [eventName, setEventName] = useState("");
  const [entityType, setEntityType] = useState("");
  const [userId, setUserId] = useState("");
  const [deviceType, setDeviceType] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await systemApi.listProductEvents({
        eventName: eventName.trim() || undefined,
        entityType: entityType.trim() || undefined,
        userId: userId.trim() || undefined,
        deviceType: deviceType.trim() || undefined,
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
      setError(caught instanceof Error ? caught : new Error("Không thể tải product events."));
      setData({ items: [], total: 0, totalPages: 1 });
    } finally {
      setLoading(false);
    }
  }, [deviceType, entityType, eventName, page, pageSize, userId]);

  useEffect(() => {
    const timer = window.setTimeout(() => void load(), eventName || entityType || userId || deviceType ? 250 : 0);
    return () => window.clearTimeout(timer);
  }, [deviceType, entityType, eventName, load, userId]);

  const columns = useMemo<DataTableColumn<AdminProductEvent>[]>(() => [
    {
      id: "event",
      header: "Sự kiện",
      cell: (item) => (
        <div className="min-w-0">
          <div className="truncate text-[12px] font-semibold text-[#333]">{item.eventName}</div>
          <div className="mt-0.5 text-[10px] text-muted-foreground">#{item.id} · {item.publicId.slice(0, 8)}…</div>
        </div>
      ),
    },
    {
      id: "entity",
      header: "Entity",
      width: "180px",
      cell: (item) => item.entityType ? <Badge>{item.entityType}</Badge> : <span className="text-[10px] text-muted-foreground">—</span>,
    },
    {
      id: "user",
      header: "User / Session",
      width: "190px",
      cell: (item) => (
        <div className="font-mono text-[10px] text-[#666]">
          <div>{item.userId ? `${item.userId.slice(0, 8)}…` : "Anonymous"}</div>
          <div className="mt-0.5 text-muted-foreground">{item.sessionId ? `${item.sessionId.slice(0, 8)}…` : "—"}</div>
        </div>
      ),
    },
    {
      id: "page",
      header: "Trang / Thiết bị",
      width: "210px",
      cell: (item) => (
        <div className="text-[10px] text-[#666]">
          <div className="truncate">{item.pagePath ?? "—"}</div>
          <div className="mt-0.5 text-muted-foreground">{item.deviceType ?? "—"}</div>
        </div>
      ),
    },
    {
      id: "properties",
      header: "Properties",
      width: "240px",
      cell: (item) => <span className="block max-w-[230px] truncate font-mono text-[10px] text-muted-foreground">{item.propertiesJson ?? "—"}</span>,
    },
    {
      id: "time",
      header: "Thời gian",
      width: "155px",
      cell: (item) => <span className="text-[10px] text-[#666]">{new Date(item.occurredAt).toLocaleString("vi-VN")}</span>,
    },
  ], []);

  if (error && !loading) {
    return <ErrorState title="Không thể tải Product Events" description={error.message} onRetry={() => void load()} />;
  }

  return (
    <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
      <DataTableToolbar
        left={
          <>
            <DataTableSearch value={eventName} onChange={(value) => { setEventName(value); setPage(1); }} placeholder="Event name..." />
            <Input className="h-[38px] w-[180px] text-[11px]" value={entityType} onChange={(event) => { setEntityType(event.target.value); setPage(1); }} placeholder="Entity type..." />
            <Input className="h-[38px] w-[220px] text-[11px]" value={userId} onChange={(event) => { setUserId(event.target.value); setPage(1); }} placeholder="UserId..." />
            <Input className="h-[38px] w-[150px] text-[11px]" value={deviceType} onChange={(event) => { setDeviceType(event.target.value); setPage(1); }} placeholder="Device..." />
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
  );
}
