"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { RefreshCw } from "lucide-react";
import { useRouter } from "next/navigation";

import { DataTable } from "@/components/common/data-table/data-table";
import { DataTableActions } from "@/components/common/data-table/data-table-actions";
import { DataTableFilter } from "@/components/common/data-table/data-table-filter";
import { DataTableSearch } from "@/components/common/data-table/data-table-search";
import { DataTableToolbar } from "@/components/common/data-table/data-table-toolbar";
import { ErrorState } from "@/components/common/error-state";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import type { DataTableColumn } from "@/types/table.types";

import { notificationApi } from "../notification.api";
import {
  AdminNotification,
  NOTIFICATION_TYPE_LABELS,
  NotificationType,
} from "../notification.types";

const TYPE_OPTIONS = Object.entries(NOTIFICATION_TYPE_LABELS).map(([value, label]) => ({ value, label }));
const READ_OPTIONS = [
  { value: "true", label: "Đã đọc" },
  { value: "false", label: "Chưa đọc" },
];

interface NotificationPageData {
  items: AdminNotification[];
  total: number;
  totalPages: number;
}

export function NotificationTable() {
  const router = useRouter();
  const [data, setData] = useState<NotificationPageData>({ items: [], total: 0, totalPages: 1 });
  const [userId, setUserId] = useState("");
  const [type, setType] = useState("");
  const [isRead, setIsRead] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await notificationApi.list({
        userId: userId.trim() || undefined,
        type: type === "" ? undefined : Number(type) as NotificationType,
        isRead: isRead === "" ? undefined : isRead === "true",
        page,
        pageSize,
        sort: "-createdAt",
      });
      const total = result.total ?? result.totalCount ?? 0;
      setData({
        items: result.items ?? [],
        total,
        totalPages: result.totalPages ?? Math.max(1, Math.ceil(total / Math.max(1, result.pageSize ?? pageSize))),
      });
    } catch (caught) {
      setError(caught instanceof Error ? caught : new Error("Không thể tải danh sách thông báo."));
      setData({ items: [], total: 0, totalPages: 1 });
    } finally {
      setLoading(false);
    }
  }, [isRead, page, pageSize, type, userId]);

  useEffect(() => {
    const timer = window.setTimeout(() => void load(), userId ? 250 : 0);
    return () => window.clearTimeout(timer);
  }, [load, userId]);

  const columns = useMemo<DataTableColumn<AdminNotification>[]>(() => [
    {
      id: "notification",
      header: "Thông báo",
      cell: (item) => (
        <div className="min-w-0">
          <div className="truncate text-[12px] font-semibold text-[#333]">{item.title}</div>
          <div className="mt-0.5 line-clamp-2 text-[10px] text-muted-foreground">{item.message}</div>
        </div>
      ),
    },
    {
      id: "user",
      header: "Người nhận",
      width: "180px",
      cell: (item) => <span className="font-mono text-[10px] text-[#666]">{item.userId.slice(0, 8)}…{item.userId.slice(-4)}</span>,
    },
    {
      id: "type",
      header: "Loại",
      align: "center",
      width: "130px",
      cell: (item) => <Badge>{NOTIFICATION_TYPE_LABELS[item.type] ?? `#${item.type}`}</Badge>,
    },
    {
      id: "state",
      header: "Trạng thái",
      align: "center",
      width: "130px",
      cell: (item) => item.isExpired
        ? <Badge variant="default">Hết hạn</Badge>
        : item.isRead
          ? <Badge variant="success">Đã đọc</Badge>
          : <Badge variant="warning">Chưa đọc</Badge>,
    },
    {
      id: "createdAt",
      header: "Ngày gửi",
      width: "150px",
      cell: (item) => <span className="text-[10px] text-[#666]">{new Date(item.createdAt).toLocaleString("vi-VN")}</span>,
    },
    {
      id: "actions",
      header: "Thao tác",
      align: "center",
      width: "80px",
      cell: (item) => <DataTableActions onView={() => router.push(`/thong-bao/${item.id}`)} />,
    },
  ], [router]);

  if (error && !loading) {
    return <ErrorState title="Không thể tải thông báo" description={error.message} onRetry={() => void load()} />;
  }

  return (
    <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
      <DataTableToolbar
        left={
          <>
            <DataTableSearch
              value={userId}
              onChange={(value) => { setUserId(value); setPage(1); }}
              placeholder="Lọc theo UserId..."
            />
            <DataTableFilter
              value={type}
              onChange={(value) => { setType(value); setPage(1); }}
              options={TYPE_OPTIONS}
              placeholder="Tất cả loại"
            />
            <DataTableFilter
              value={isRead}
              onChange={(value) => { setIsRead(value); setPage(1); }}
              options={READ_OPTIONS}
              placeholder="Đọc / chưa đọc"
            />
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
