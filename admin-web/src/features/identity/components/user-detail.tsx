"use client";

import {
  Lock,
  ShieldCheck,
  Smartphone,
  Unlock,
} from "lucide-react";

import Link from "next/link";

import {
  FormSection,
} from "@/components/forms/form-section";

import {
  Button,
} from "@/components/ui/button";

import {
  UserStatusBadge,
} from "./user-status-badge";

import {
  formatDateTime,
} from "@/utils/date.util";

import type {
  AdminUserDetailDto,
} from "@/dto/identity/admin-user.dto";

import {
  PermissionGuard,
} from "@/security/permission-guard";

import { PERMISSIONS } from "@/constants/permission.constants";

export function UserDetail({
  user,
}: {
  user:
    AdminUserDetailDto;
}) {
  return (
    <div className="space-y-5">
      <FormSection
        title="Thông tin tài khoản"
      >
        <DetailGrid>
          <DetailItem
            label="ID"
            value={
              user.id
            }
          />

          <DetailItem
            label="Tên hiển thị"
            value={
              user.displayName
            }
          />

          <DetailItem
            label="Email"
            value={
              user.email
            }
          />

          <DetailItem
            label="Trạng thái"
            value={
              <UserStatusBadge
                status={
                  user.status
                }
              />
            }
          />

          <DetailItem
            label="Xác minh Email"
            value={
              user.emailVerifiedAt
                ? formatDateTime(
                    user.emailVerifiedAt,
                  )
                : "Chưa xác minh"
            }
          />

          <DetailItem
            label="Đăng nhập cuối"
            value={
              formatDateTime(
                user.lastLoginAt,
              )
            }
          />

          <DetailItem
            label="Số lần đăng nhập sai"
            value={
              user.failedLoginCount ??
              0
            }
          />

          <DetailItem
            label="Khóa đến"
            value={
              formatDateTime(
                user.lockedUntil,
              )
            }
          />
        </DetailGrid>
      </FormSection>

      <FormSection
        title="Vai trò"
        icon={
          <ShieldCheck
            size={17}
          />
        }
      >
        <div
          className="
            flex flex-wrap
            gap-2
          "
        >
          {user.roles
            ?.length ? (
            user.roles.map(
              (role) => (
                <span
                  key={
                    role.id
                  }
                  className="
                    rounded-full
                    bg-[#edf8f2]
                    px-3 py-1
                    text-[10px]
                    text-[#168152]
                  "
                >
                  {
                    role.name
                  }{" "}
                  (
                  {
                    role.code
                  }
                  )
                </span>
              ),
            )
          ) : (
            <span className="text-[11px] text-[#999]">
              Chưa có vai trò.
            </span>
          )}
        </div>
      </FormSection>

      <FormSection
        title="Phiên đăng nhập"
        icon={
          <Smartphone
            size={17}
          />
        }
      >
        <div
          className="
            flex
            items-center
            justify-between
            gap-4
          "
        >
          <div>
            <div
              className="
                text-[20px]
                font-semibold
              "
            >
              {
                user.activeSessionCount ??
                0
              }
            </div>

            <div
              className="
                text-[10px]
                text-[#888]
              "
            >
              phiên đang hoạt động
            </div>
          </div>

          <PermissionGuard permission={PERMISSIONS.SESSIONS.READ} fallback={null}>
            <Link
              href={`/nguoi-dung/${user.id}/phien-dang-nhap`}
            >
              <Button
                variant="outline"
                className="h-[37px] text-[11px]"
              >
                Xem phiên
              </Button>
            </Link>
          </PermissionGuard>
        </div>
      </FormSection>
    </div>
  );
}

function DetailGrid({
  children,
}: {
  children:
    React.ReactNode;
}) {
  return (
    <div
      className="
        grid
        gap-x-6
        gap-y-4
        md:grid-cols-2
      "
    >
      {children}
    </div>
  );
}

function DetailItem({
  label,
  value,
}: {
  label: string;

  value:
    React.ReactNode;
}) {
  return (
    <div>
      <div
        className="
          text-[10px]
          text-[#929292]
        "
      >
        {label}
      </div>

      <div
        className="
          mt-1
          text-[12px]
          font-medium
          text-[#444]
        "
      >
        {value ?? "-"}
      </div>
    </div>
  );
}
