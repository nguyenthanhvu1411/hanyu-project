"use client";

import { CircleCheckBig, CircleOff, GraduationCap } from "lucide-react";
import { useRouter } from "next/navigation";
import { useState } from "react";

import { DataTable } from "@/components/common/data-table/data-table";
import { DataTableActions } from "@/components/common/data-table/data-table-actions";
import { ErrorState } from "@/components/common/error-state";
import { PermissionGuard } from "@/security/permission-guard";
import { PERMISSIONS } from "@/constants/permission.constants";
import type { DataTableColumn } from "@/types/table.types";
import type { AdminHskLevelDto } from "@/dto/learning/hsk-level.dto";

import { HskLevelFilter } from "./hsk-level-filter";
import { HskLevelStatusBadge } from "./hsk-level-status-badge";
import { HskLevelDeleteDialog } from "./hsk-level-delete-dialog";
import { HskLevelStatusDialog } from "./hsk-level-status-dialog";
import { useHskLevels } from "../../hooks/use-hsk-levels";

export function HskLevelTable() {
  const router = useRouter();
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("");
  const [selected, setSelected] = useState<AdminHskLevelDto | null>(null);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [statusOpen, setStatusOpen] = useState(false);

  const query = useHskLevels({
    page,
    pageSize,
    q: search || undefined,
    isActive: status === "" ? undefined : status === "true",
    sortBy: "sortOrder",
    sortDirection: "asc",
  });

  const columns: DataTableColumn<AdminHskLevelDto>[] = [
    {
      id: "level",
      header: "Cấp độ",
      width: "170px",
      cell: (item) => (
        <div className="flex items-center gap-2">
          <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-[7px] bg-[#fff0ee] text-[#ef241c]">
            <GraduationCap size={15} />
          </div>
          <div>
            <div className="text-[11px] font-semibold text-[#333]">{item.code}</div>
            <div className="text-[10px] text-[#999]">ID: {item.id}</div>
          </div>
        </div>
      ),
    },
    {
      id: "nameVi",
      header: "Tên cấp độ",
      accessor: "nameVi",
    },
    {
      id: "sortOrder",
      header: "Thứ tự",
      align: "center",
      width: "100px",
      accessor: (item) => item.sortOrder,
    },
    {
      id: "status",
      header: "Trạng thái",
      align: "center",
      width: "150px",
      cell: (item) => <HskLevelStatusBadge isActive={item.isActive} />,
    },
    {
      id: "actions",
      header: "Thao tác",
      align: "center",
      width: "90px",
      cell: (item) => (
        <DataTableActions
          onView={() => router.push(`/cap-do-hsk/${item.id}`)}
          onEdit={() => router.push(`/cap-do-hsk/${item.id}/chinh-sua`)}
          onDelete={() => {
            setSelected(item);
            setDeleteOpen(true);
          }}
          customActions={
            <PermissionGuard
              permission={
                item.isActive
                  ? PERMISSIONS.HSK_LEVELS.DEACTIVATE
                  : PERMISSIONS.HSK_LEVELS.ACTIVATE
              }
              fallback={null}
            >
              <button
                type="button"
                onClick={() => {
                  setSelected(item);
                  setStatusOpen(true);
                }}
                className="flex w-full items-center gap-2 rounded-[5px] px-2 py-2 text-left text-[11px] text-[#555] hover:bg-[#f5f4f1]"
              >
                {item.isActive ? <CircleOff size={14} /> : <CircleCheckBig size={14} />}
                {item.isActive ? "Ngừng hoạt động" : "Kích hoạt"}
              </button>
            </PermissionGuard>
          }
        />
      ),
    },
  ];

  if (query.isError) {
    return (
      <ErrorState
        title="Không thể tải cấp độ HSK"
        description={query.error?.message ?? "Đã xảy ra lỗi khi tải dữ liệu."}
        onRetry={() => query.refetch()}
      />
    );
  }

  return (
    <>
      <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
        <div className="border-b border-[#eee9e2] p-3">
          <HskLevelFilter
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
        </div>

        <DataTable
          data={query.data?.items ?? []}
          columns={columns}
          rowKey={(item) => item.id}
          loading={query.isLoading}
          selectable={false}
          page={page}
          pageSize={pageSize}
          totalItems={query.data?.total ?? 0}
          totalPages={query.data?.totalPages ?? 1}
          onPageChange={setPage}
          onPageSizeChange={(value) => {
            setPageSize(value);
            setPage(1);
          }}
        />
      </div>

      <HskLevelDeleteDialog
        open={deleteOpen}
        onOpenChange={(open) => {
          setDeleteOpen(open);
          if (!open) setSelected(null);
        }}
        item={selected}
      />

      <HskLevelStatusDialog
        open={statusOpen}
        onOpenChange={(open) => {
          setStatusOpen(open);
          if (!open) setSelected(null);
        }}
        item={selected}
      />
    </>
  );
}
