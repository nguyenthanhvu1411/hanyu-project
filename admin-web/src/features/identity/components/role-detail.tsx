"use client";

import { CalendarDays, Edit3, KeyRound, ShieldCheck, Trash2, UsersRound } from "lucide-react";
import Link from "next/link";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { FormSection } from "@/components/common/form-section";
import { MetricCard } from "@/components/common/metric-card";
import { CopyButton } from "@/components/common/copy-button";
import { formatDateTime } from "@/utils/date.util";
import type { AdminRoleDto } from "@/dto/identity/admin-role.dto";
import { RolePermissionSummary } from "./role-permission-summary";
import { RoleUsersPreview } from "./role-users-preview";

interface RoleDetailProps {
  role: AdminRoleDto;
}

export function RoleDetail({ role }: RoleDetailProps) {
  return (
    <div className="space-y-5">
      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        <MetricCard
          title="Tổng quyền"
          value={role.permissions?.length ?? 0}
          icon={<KeyRound size={18} />}
          description="Quyền được gán"
        />
        <MetricCard
          title="Người dùng"
          value={role.userCount ?? 0}
          icon={<UsersRound size={18} />}
          description="Tài khoản đang sử dụng"
        />
        <MetricCard
          title="Loại vai trò"
          value={role.isSystem ? "Hệ thống" : "Tùy chỉnh"}
          icon={<ShieldCheck size={18} />}
        />
        <MetricCard
          title="Cập nhật gần nhất"
          value={role.updatedAt ? formatDateTime(role.updatedAt) : "-"}
          icon={<CalendarDays size={18} />}
        />
      </div>

      <FormSection
        title="Thông tin vai trò"
        description="Thông tin định danh và trạng thái của vai trò."
        icon={<ShieldCheck size={18} />}
      >
        <div className="grid gap-x-8 gap-y-5 md:grid-cols-2">
          <DetailItem
            label="ID"
            value={
              <div className="flex items-center gap-1">
                <span>{role.id}</span>
                <CopyButton value={String(role.id)} />
              </div>
            }
          />
          <DetailItem
            label="Mã vai trò"
            value={
              <div className="flex items-center gap-2">
                <code className="rounded-[5px] bg-[#f4f3f0] px-2 py-[4px] text-[10px] font-semibold text-[#555]">
                  {role.code}
                </code>
                <CopyButton value={role.code} />
              </div>
            }
          />
          <DetailItem label="Tên vai trò" value={role.name} />
          <DetailItem
            label="Loại"
            value={role.isSystem ? <Badge variant="primary">Vai trò hệ thống</Badge> : <Badge>Vai trò tùy chỉnh</Badge>}
          />
          <DetailItem label="Ngày tạo" value={formatDateTime(role.createdAt)} />
          <DetailItem label="Cập nhật lần cuối" value={formatDateTime(role.updatedAt)} />
          <div className="md:col-span-2">
            <DetailItem label="Mô tả" value={role.description || "Chưa có mô tả cho vai trò này."} />
          </div>
        </div>
      </FormSection>

      <FormSection
        title="Quyền hạn"
        description={`Vai trò hiện có ${role.permissions?.length ?? 0} quyền được cấp.`}
        icon={<KeyRound size={18} />}
        actions={
          <Link href={`/vai-tro/${role.id}/chinh-sua`}>
            <Button type="button" variant="outline" className="h-[36px] gap-2 text-[10px]">
              <Edit3 size={13} />
              Chỉnh sửa quyền
            </Button>
          </Link>
        }
      >
        <RolePermissionSummary permissions={role.permissions ?? []} />
      </FormSection>

      <FormSection
        title="Người dùng đang sử dụng"
        description="Các tài khoản đang được gán vai trò này."
        icon={<UsersRound size={18} />}
      >
        <RoleUsersPreview roleId={role.id} total={role.userCount ?? 0} />
      </FormSection>

      <FormSection
        title="Thông tin kỹ thuật"
        description="Thông tin phục vụ quản trị và xử lý concurrency."
      >
        <div className="grid gap-4 md:grid-cols-2">
          <DetailItem
            label="Concurrency Token"
            value={
              role.concurrencyToken ? (
                <div className="flex items-center gap-2">
                  <code className="max-w-[360px] truncate rounded bg-[#f7f6f3] px-2 py-1 text-[9px] text-[#777]">
                    {role.concurrencyToken}
                  </code>
                  <CopyButton value={role.concurrencyToken} />
                </div>
              ) : (
                "-"
              )
            }
          />
          <DetailItem
            label="Trạng thái xóa"
            value={role.deletedAt ? <Badge variant="danger">Đã xóa</Badge> : <Badge variant="success">Đang hoạt động</Badge>}
          />
        </div>
      </FormSection>

      {!role.isSystem && !role.deletedAt && (
        <div className="rounded-[11px] border border-[#f0d5d1] bg-white">
          <div className="border-b border-[#f0d5d1] px-5 py-4">
            <h3 className="text-[13px] font-semibold text-[#d9362f]">Khu vực nguy hiểm</h3>
            <p className="mt-1 text-[10px] text-[#888]">
              Các thao tác tại đây có thể ảnh hưởng tới quyền truy cập của người dùng.
            </p>
          </div>
          <div className="flex flex-col gap-4 p-5 sm:flex-row sm:items-center sm:justify-between">
            <div>
              <div className="text-[11px] font-semibold text-[#444]">Xóa vai trò</div>
              <div className="mt-1 text-[10px] text-[#888]">
                Vai trò sẽ được xóa mềm và có thể khôi phục lại.
              </div>
            </div>
            <Button type="button" variant="danger" className="h-[38px] gap-2 text-[11px]">
              <Trash2 size={14} />
              Xóa vai trò
            </Button>
          </div>
        </div>
      )}
    </div>
  );
}

function DetailItem({ label, value }: { label: string; value: React.ReactNode; }) {
  return (
    <div className="min-w-0">
      <div className="text-[10px] font-medium text-[#929292]">{label}</div>
      <div className="mt-[5px] min-h-[20px] text-[12px] font-medium leading-[18px] text-[#414141]">
        {value ?? "-"}
      </div>
    </div>
  );
}
