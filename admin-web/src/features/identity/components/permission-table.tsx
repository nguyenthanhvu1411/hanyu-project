"use client";

import {
  useState,
} from "react";

import {
  DataTable,
} from "@/components/common/data-table/data-table";

import {
  DataTableActions,
} from "@/components/common/data-table/data-table-actions";

import {
  SearchInput,
} from "@/components/common/search-input";

import {
  DeleteDialog,
} from "@/components/common/delete-dialog";

import {
  Badge,
} from "@/components/ui/badge";

import {
  appToast,
} from "@/components/ui/toast";

import type {
  DataTableColumn,
} from "@/types/table.types";

import type {
  AdminPermissionDto,
} from "@/dto/identity/admin-permission.dto";

import {
  useAdminPermissions,
  useDeleteAdminPermission,
} from "../hooks/use-admin-permissions";

export function PermissionTable() {
  const [
    page,
    setPage,
  ] = useState(1);

  const [
    pageSize,
    setPageSize,
  ] = useState(20);

  const [
    search,
    setSearch,
  ] = useState("");

  const [
    selected,
    setSelected,
  ] =
    useState<AdminPermissionDto | null>(
      null,
    );

  const query =
    useAdminPermissions({
      page,
      pageSize,
      search:
        search ||
        undefined,
      sortBy:
        "resource",
      sortDirection:
        "asc",
    });

  const remove =
    useDeleteAdminPermission();

  const columns: DataTableColumn<AdminPermissionDto>[] =
    [
      {
        id: "code",

        header:
          "Mã quyền",

        cell: (
          permission,
        ) => (
          <code
            className="
              rounded
              bg-[#f4f3f0]
              px-2
              py-1
              text-[10px]
            "
          >
            {
              permission.code
            }
          </code>
        ),
      },

      {
        id: "resource",

        header:
          "Resource",

        cell: (
          permission,
        ) => (
          <Badge>
            {
              permission.resource
            }
          </Badge>
        ),
      },

      {
        id: "action",

        header:
          "Action",

        cell: (
          permission,
        ) => (
          <Badge variant="info">
            {
              permission.action
            }
          </Badge>
        ),
      },

      {
        id: "description",

        header:
          "Mô tả",

        accessor:
          (
            permission,
          ) =>
            permission.description ??
            "-",
      },

      {
        id: "roles",

        header:
          "Vai trò",

        align:
          "center",

        accessor:
          (
            permission,
          ) =>
            permission.roleCount ??
            0,
      },

      {
        id: "actions",

        header:
          "Thao tác",

        align:
          "center",

        cell: (
          permission,
        ) => (
          <DataTableActions
            onDelete={() =>
              setSelected(
                permission,
              )
            }
          />
        ),
      },
    ];

  return (
    <>
      <div
        className="
          overflow-hidden
          rounded-[11px]
          border
          border-[#e8e3dc]
          bg-white
        "
      >
        <div
          className="
            border-b
            border-[#eee9e2]
            p-3
          "
        >
          <SearchInput
            value={search}
            onChange={(
              value,
            ) => {
              setSearch(
                value,
              );

              setPage(1);
            }}
            className="max-w-[320px]"
            placeholder="Tìm mã quyền, resource..."
          />
        </div>

        <DataTable
          data={
            query.data
              ?.items ??
            []
          }
          columns={
            columns
          }
          rowKey={(
            permission,
          ) =>
            permission.id
          }
          loading={
            query.isLoading
          }
          page={page}
          pageSize={
            pageSize
          }
          totalItems={
            query.data
              ?.total ??
            0
          }
          totalPages={
            query.data
              ?.totalPages ??
            1
          }
          onPageChange={
            setPage
          }
          onPageSizeChange={
            setPageSize
          }
        />
      </div>

      <DeleteDialog
        open={
          Boolean(
            selected,
          )
        }
        onOpenChange={(
          open,
        ) => {
          if (!open) {
            setSelected(
              null,
            );
          }
        }}
        itemName={
          selected?.code
        }
        onDelete={async () => {
          if (!selected) {
            return;
          }

          await remove.mutateAsync(
            selected.id,
          );

          setSelected(
            null,
          );

          appToast.success(
            "Đã xóa quyền.",
          );
        }}
      />
    </>
  );
}
