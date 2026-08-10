"use client";

import {
  Eye,
  LogOut,
  MonitorSmartphone,
} from "lucide-react";

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
  ErrorState,
} from "@/components/common/error-state";

import {
  formatDateTime,
} from "@/utils/date.util";

import type {
  DataTableColumn,
} from "@/types/table.types";

import type {
  AdminSessionDto,
} from "@/dto/identity/admin-session.dto";

import {
  useAdminSessions,
} from "../hooks/use-admin-sessions";

import {
  SessionFilter,
} from "./session-filter";

import {
  SessionStatusBadge,
} from "./session-status-badge";

import {
  SessionDetailDrawer,
} from "./session-detail-drawer";

import {
  RevokeSessionDialog,
} from "./revoke-session-dialog";

export function SessionTable() {
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
    active,
    setActive,
  ] = useState("");

  const [
    selected,
    setSelected,
  ] =
    useState<AdminSessionDto | null>(
      null,
    );

  const [
    detailOpen,
    setDetailOpen,
  ] = useState(false);

  const [
    revokeOpen,
    setRevokeOpen,
  ] = useState(false);

  const query =
    useAdminSessions({
      page,
      pageSize,

      search:
        search ||
        undefined,

      active:
        active === ""
          ? undefined
          : active ===
            "true",

      sortBy:
        "createdAt",

      sortDirection:
        "desc",
    });

  const columns: DataTableColumn<AdminSessionDto>[] =
    [
      {
        id:
          "user",

        header:
          "Người dùng",

        cell: (
          session,
        ) => (
          <div>
            <div
              className="
                text-[11px]
                font-semibold
                text-[#3f3f3f]
              "
            >
              {
                session.userDisplayName ??
                "-"
              }
            </div>

            <div
              className="
                mt-[2px]
                text-[9px]
                text-[#999]
              "
            >
              {
                session.userEmail ??
                `User #${session.userId}`
              }
            </div>
          </div>
        ),
      },

      {
        id:
          "device",

        header:
          "Thiết bị",

        cell: (
          session,
        ) => (
          <div
            className="
              flex
              items-center
              gap-2
            "
          >
            <MonitorSmartphone
              size={14}
              className="text-[#777]"
            />

            <span>
              {
                session.deviceInfo ??
                "Không xác định"
              }
            </span>
          </div>
        ),
      },

      {
        id:
          "ip",

        header:
          "Địa chỉ IP",

        accessor:
          (
            session,
          ) =>
            session.ipAddress ??
            "-",
      },

      {
        id:
          "created",

        header:
          "Đăng nhập lúc",

        cell: (
          session,
        ) =>
          formatDateTime(
            session.createdAt,
          ),
      },

      {
        id:
          "lastUsed",

        header:
          "Hoạt động cuối",

        cell: (
          session,
        ) =>
          formatDateTime(
            session.lastUsedAt,
          ),
      },

      {
        id:
          "expires",

        header:
          "Hết hạn",

        cell: (
          session,
        ) =>
          formatDateTime(
            session.expiresAt,
          ),
      },

      {
        id:
          "status",

        header:
          "Trạng thái",

        align:
          "center",

        cell: (
          session,
        ) => (
          <SessionStatusBadge
            active={
              session.isActive
            }
          />
        ),
      },

      {
        id:
          "actions",

        header:
          "Thao tác",

        align:
          "center",

        width:
          "85px",

        cell: (
          session,
        ) => (
          <DataTableActions
            onView={() => {
              setSelected(
                session,
              );

              setDetailOpen(
                true,
              );
            }}
            onDelete={
              session.isActive
                ? () => {
                    setSelected(
                      session,
                    );

                    setRevokeOpen(
                      true,
                    );
                  }
                : undefined
            }
          />
        ),
      },
    ];

  if (
    query.isError
  ) {
    return (
      <ErrorState
        onRetry={() =>
          query.refetch()
        }
      />
    );
  }

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
          <SessionFilter
            search={search}
            active={active}
            onSearchChange={(
              value,
            ) => {
              setSearch(
                value,
              );

              setPage(1);
            }}
            onActiveChange={(
              value,
            ) => {
              setActive(
                value,
              );

              setPage(1);
            }}
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
            session,
          ) =>
            String(session.id)
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
          onPageChange={(page) => {
            setPage(page);
          }}
          onPageSizeChange={(pageSize) => {
            setPageSize(pageSize);
            setPage(1);
          }}
        />
      </div>

      <SessionDetailDrawer
        open={
          detailOpen
        }
        onOpenChange={(
          open,
        ) => {
          setDetailOpen(
            open,
          );

          if (!open) {
            setSelected(
              null,
            );
          }
        }}
        session={
          selected
        }
      />

      <RevokeSessionDialog
        open={
          revokeOpen
        }
        onOpenChange={(
          open,
        ) => {
          setRevokeOpen(
            open,
          );

          if (!open) {
            setSelected(
              null,
            );
          }
        }}
        sessionId={
          selected?.id
        }
        userName={
          selected?.userDisplayName
        }
        onSuccess={() => {
          query.refetch();
        }}
      />
    </>
  );
}
