"use client";

import {
  useState,
} from "react";

import {
  useRouter,
} from "next/navigation";

import {
  DataTable,
} from "@/components/common/data-table/data-table";

import {
  DataTableActions,
} from "@/components/common/data-table/data-table-actions";

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
  AdminRoleDto,
} from "@/dto/identity/admin-role.dto";

import {
  useAdminRoles,
  useDeleteAdminRole,
} from "../hooks/use-admin-roles";

export function RoleTable() {
  const router =
    useRouter();

  const [
    page,
    setPage,
  ] = useState(1);

  const [
    pageSize,
    setPageSize,
  ] = useState(20);

  const [
    selected,
    setSelected,
  ] =
    useState<AdminRoleDto | null>(
      null,
    );

  const query =
    useAdminRoles({
      page,
      pageSize,
      sortBy:
        "name",

      sortDirection:
        "asc",
    });

  const deleteMutation =
    useDeleteAdminRole();

  const columns: DataTableColumn<AdminRoleDto>[] =
    [
      {
        id: "code",

        header:
          "Mã",

        accessor:
          "code",
      },

      {
        id: "name",

        header:
          "Tên vai trò",

        accessor:
          "name",
      },

      {
        id: "system",

        header:
          "Loại",

        align:
          "center",

        cell: (
          role,
        ) =>
          role.isSystem
            ? (
                <Badge variant="primary">
                  Hệ thống
                </Badge>
              )
            : (
                <Badge>
                  Tùy chỉnh
                </Badge>
              ),
      },

      {
        id: "permissions",

        header:
          "Số quyền",

        align:
          "center",

        cell: (
          role,
        ) =>
          role.permissionCount ??
          0,
      },

      {
        id: "users",

        header:
          "Người dùng",

        align:
          "center",

        accessor:
          (
            role,
          ) =>
            role.userCount ??
            0,
      },

      {
        id: "action",

        header:
          "Thao tác",

        align:
          "center",

        cell: (
          role,
        ) => (
          <DataTableActions
            onView={() =>
              router.push(
                `/vai-tro/${role.id}`,
              )
            }
            onEdit={() =>
              router.push(
                `/vai-tro/${role.id}/chinh-sua`,
              )
            }
            onDelete={
              role.isSystem
                ? undefined
                : () =>
                    setSelected(
                      role,
                    )
            }
          />
        ),
      },
    ];

  return (
    <>
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
          role,
        ) =>
          role.id
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
          selected?.name
        }
        loading={
          deleteMutation.isPending
        }
        onDelete={async () => {
          if (!selected) {
            return;
          }

          await deleteMutation.mutateAsync(
            selected.id,
          );

          setSelected(
            null,
          );

          appToast.success(
            "Đã xóa vai trò.",
          );
        }}
      />
    </>
  );
}
