"use client";

import { Eye, Trash2 } from "lucide-react";
import { useState } from "react";
import { DataTable } from "@/components/common/data-table/data-table";
import { DeleteDialog } from "@/components/common/delete-dialog";
import { StatusBadge } from "@/components/common/status-badge";
import { appToast } from "@/components/ui/toast";
import { formatDateTime } from "@/utils/date.util";
import type { DataTableColumn } from "@/types/table.types";
import type { AdminSessionDto } from "@/dto/identity/admin-session.dto";
import { useAdminSessions, useDeleteAdminSession } from "../hooks/use-admin-sessions";
import { SessionDetailDrawer } from "./session-detail-drawer";
import { DataTableActions } from "@/components/common/data-table/data-table-actions";

export function UserSessionTable({ userId }: { userId?: string }) {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [selected, setSelected] = useState<AdminSessionDto | null>(null);
  const [detailSession, setDetailSession] = useState<AdminSessionDto | null>(null);

  const query = useAdminSessions({
    page,
    pageSize,
    userId,
    sortBy: "createdAt",
    sortDirection: "desc",
  });

  const deleteMutation = useDeleteAdminSession();

  const columns: DataTableColumn<AdminSessionDto>[] = [
    {
      id: "user",
      header: "Người dùng",
      cell: (session) => (
        <div>
          <div className="font-medium">{session.userDisplayName ?? "-"}</div>
          <div className="text-[10px] text-[#999]">
            {session.userEmail ?? `User #${session.userId}`}
          </div>
        </div>
      ),
    },
    {
      id: "device",
      header: "Thiết bị",
      accessor: (session) => session.deviceInfo ?? "-",
    },
    {
      id: "ip",
      header: "IP",
      accessor: (session) => session.ipAddress ?? "-",
    },
    {
      id: "lastUsed",
      header: "Hoạt động cuối",
      cell: (session) => formatDateTime(session.lastUsedAt),
    },
    {
      id: "status",
      header: "Trạng thái",
      align: "center",
      cell: (session) =>
        session.isActive ? (
          <StatusBadge variant="success">Hoạt động</StatusBadge>
        ) : (
          <StatusBadge variant="neutral">Đã thu hồi</StatusBadge>
        ),
    },
    {
      id: "expires",
      header: "Hết hạn",
      cell: (session) => formatDateTime(session.expiresAt),
    },
    {
      id: "action",
      header: "Thao tác",
      align: "center",
      width: "80px",
      cell: (session) => (
        <DataTableActions
          onView={() => setDetailSession(session)}
          onDelete={
            session.isActive ? () => setSelected(session) : undefined
          }
        />
      ),
    },
  ];

  return (
    <>
      <DataTable
        data={query.data?.items ?? []}
        columns={columns}
        rowKey={(item) => item.id}
        loading={query.isLoading}
        page={page}
        pageSize={pageSize}
        totalItems={query.data?.total ?? 0}
        totalPages={query.data?.totalPages ?? 1}
        onPageChange={setPage}
        onPageSizeChange={setPageSize}
      />

      <DeleteDialog
        open={Boolean(selected)}
        onOpenChange={(open) => {
          if (!open) setSelected(null);
        }}
        title="Thu hồi phiên đăng nhập"
        itemName={selected ? `Session #${selected.id}` : undefined}
        loading={deleteMutation.isPending}
        onDelete={async () => {
          if (!selected) return;
          await deleteMutation.mutateAsync(selected.id);
          setSelected(null);
          appToast.success("Đã thu hồi phiên đăng nhập.");
        }}
      />

      <SessionDetailDrawer
        open={Boolean(detailSession)}
        onOpenChange={(open) => {
          if (!open) setDetailSession(null);
        }}
        session={detailSession}
      />
    </>
  );
}
