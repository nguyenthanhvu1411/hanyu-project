"use client";

import { Lock, RotateCcw, ShieldCheck, Trash2, Unlock } from "lucide-react";
import { useState } from "react";
import { useRouter } from "next/navigation";
import { DataTable } from "@/components/common/data-table/data-table";
import { DataTableActions, ActionButton } from "@/components/common/data-table/data-table-actions";
import { DeleteDialog } from "@/components/common/delete-dialog";
import { ErrorState } from "@/components/common/error-state";
import { Switch } from "@/components/ui/switch";
import { appToast } from "@/components/ui/toast";
import { formatDateTime } from "@/utils/date.util";
import type { DataTableColumn } from "@/types/table.types";
import type { AdminUserListItemDto } from "@/dto/identity/admin-user.dto";
import type { UserStatus } from "../identity.constants";
import { UserFilter } from "./user-filter";
import { UserStatusBadge } from "./user-status-badge";
import { useAdminUsers, useDeleteAdminUser } from "../hooks/use-admin-users";
import { UserLockDialog } from "./user-lock-dialog";
import { UserUnlockDialog } from "./user-unlock-dialog";
import { RestoreUserDialog } from "./restore-user-dialog";

export function UserTable() {
  const router = useRouter();
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("");
  const [includeDeleted, setIncludeDeleted] = useState(false);
  const [selected, setSelected] = useState<AdminUserListItemDto | null>(null);
  
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [lockOpen, setLockOpen] = useState(false);
  const [unlockOpen, setUnlockOpen] = useState(false);
  const [restoreOpen, setRestoreOpen] = useState(false);

  const query = useAdminUsers({
    page,
    pageSize,
    search: search || undefined,
    status: status ? (status as UserStatus) : undefined,
    includeDeleted,
    sortBy: "createdAt",
    sortDirection: "desc",
  });

  const deleteMutation = useDeleteAdminUser();

  const columns: DataTableColumn<AdminUserListItemDto>[] = [
    {
      id: "user",
      header: "Người dùng",
      cell: (user) => (
        <div>
          <div className="font-medium text-[#333]">{user.displayName}</div>
          <div className="mt-[2px] text-[10px] text-[#929292]">{user.email}</div>
        </div>
      ),
    },
    {
      id: "roles",
      header: "Vai trò",
      cell: (user) => user.roles?.length ? user.roles.map((role) => role.name).join(", ") : "-",
    },
    {
      id: "emailVerified",
      header: "Email",
      align: "center",
      cell: (user) =>
        user.emailVerifiedAt ? (
          <span className="text-[#16975b]">Đã xác minh</span>
        ) : (
          <span className="text-[#b67d18]">Chưa xác minh</span>
        ),
    },
    {
      id: "status",
      header: "Trạng thái",
      align: "center",
      cell: (user) => <UserStatusBadge status={user.status} />,
    },
    {
      id: "lastLogin",
      header: "Đăng nhập cuối",
      cell: (user) => formatDateTime(user.lastLoginAt),
    },
    {
      id: "createdAt",
      header: "Ngày tạo",
      cell: (user) => formatDateTime(user.createdAt),
    },
    {
      id: "action",
      header: "Thao tác",
      align: "center",
      width: "80px",
      cell: (user) => (
        <DataTableActions
          onView={() => router.push(`/nguoi-dung/${user.id}`)}
          onEdit={
            user.deletedAt
              ? undefined
              : () => router.push(`/nguoi-dung/${user.id}/chinh-sua`)
          }
          onRestore={
            user.deletedAt
              ? () => {
                  setSelected(user);
                  setRestoreOpen(true);
                }
              : undefined
          }
          onDelete={
            !user.deletedAt
              ? () => {
                  setSelected(user);
                  setDeleteOpen(true);
                }
              : undefined
          }
          customActions={
            !user.deletedAt && (
              <>
                {user.status === "active" && (
                  <ActionButton
                    icon={<Lock size={14} />}
                    onClick={() => {
                      setSelected(user);
                      setLockOpen(true);
                    }}
                    danger
                  >
                    Khóa tài khoản
                  </ActionButton>
                )}
                {user.status === "locked" && (
                  <ActionButton
                    icon={<Unlock size={14} />}
                    onClick={() => {
                      setSelected(user);
                      setUnlockOpen(true);
                    }}
                  >
                    Mở khóa tài khoản
                  </ActionButton>
                )}
              </>
            )
          }
        />
      ),
    },
  ];

  if (query.isError) {
    return <ErrorState onRetry={() => query.refetch()} />;
  }

  return (
    <>
      <div className="overflow-visible rounded-[11px] border border-[#e8e3dc] bg-white">
        <div className="flex flex-wrap items-center justify-between gap-3 border-b border-[#eee9e2] p-3">
          <UserFilter
            search={search}
            status={status}
            onSearchChange={(value) => {
              setSearch(value);
              setPage(1);
            }}
            onStatusChange={(value) => {
              setStatus(value);
              setPage(1);
            }}
          />
          <Switch
            checked={includeDeleted}
            onCheckedChange={(checked: boolean) => {
              setIncludeDeleted(checked);
              setPage(1);
            }}
            label="Hiện đã xóa"
          />
        </div>

        <DataTable
          data={query.data?.items ?? []}
          columns={columns}
          rowKey={(user) => user.id}
          loading={query.isLoading}
          page={page}
          pageSize={pageSize}
          totalItems={query.data?.total ?? 0}
          totalPages={query.data?.totalPages ?? 1}
          onPageChange={setPage}
          onPageSizeChange={(size) => {
            setPageSize(size);
            setPage(1);
          }}
        />
      </div>

      <DeleteDialog
        open={deleteOpen}
        onOpenChange={setDeleteOpen}
        itemName={selected?.displayName}
        loading={deleteMutation.isPending}
        onDelete={async () => {
          if (!selected) return;
          await deleteMutation.mutateAsync({
            id: selected.id,
            request: { reason: "Xóa tài khoản từ bảng điều khiển" }
          });
          setDeleteOpen(false);
          appToast.success("Đã xóa người dùng.");
        }}
      />

      {selected && (
        <>
          <UserLockDialog
            open={lockOpen}
            onOpenChange={setLockOpen}
            userId={selected.id}
            userName={selected.displayName}
            concurrencyToken={selected.concurrencyToken}
            onSuccess={async () => { await query.refetch(); }}
          />

          <UserUnlockDialog
            open={unlockOpen}
            onOpenChange={setUnlockOpen}
            userId={selected.id}
            userName={selected.displayName}
            concurrencyToken={selected.concurrencyToken}
            onSuccess={async () => { await query.refetch(); }}
          />

          <RestoreUserDialog
            open={restoreOpen}
            onOpenChange={setRestoreOpen}
            userId={selected.id}
            userName={selected.displayName}
            onSuccess={async () => { await query.refetch(); }}
          />
        </>
      )}
    </>
  );
}

