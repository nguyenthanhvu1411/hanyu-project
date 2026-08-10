"use client";

import { ChevronDown, KeyRound } from "lucide-react";
import { useMemo, useState } from "react";
import { Badge } from "@/components/ui/badge";
import type { AdminRolePermissionDto } from "@/dto/identity/admin-role.dto";

interface RolePermissionSummaryProps {
  permissions: AdminRolePermissionDto[];
}

export function RolePermissionSummary({ permissions }: RolePermissionSummaryProps) {
  const groups = useMemo(() => {
    const map = new Map<string, AdminRolePermissionDto[]>();
    permissions.forEach((permission) => {
      const resource = permission.resource ?? getResourceFromCode(permission.code);
      const current = map.get(resource) ?? [];
      current.push(permission);
      map.set(resource, current);
    });
    return Array.from(map.entries()).sort(([a], [b]) => a.localeCompare(b));
  }, [permissions]);

  if (permissions.length === 0) {
    return (
      <div className="flex min-h-[160px] items-center justify-center rounded-[9px] border border-dashed border-[#ddd8d0] bg-[#faf9f7] p-5 text-center">
        <div>
          <div className="mx-auto flex h-10 w-10 items-center justify-center rounded-[9px] bg-[#fff0ee] text-[#ef241c]">
            <KeyRound size={18} />
          </div>
          <div className="mt-3 text-[11px] font-semibold text-[#555]">Chưa được gán quyền</div>
          <div className="mt-1 text-[9px] text-[#999]">Vai trò này hiện chưa có quyền truy cập quản trị.</div>
        </div>
      </div>
    );
  }

  return (
    <div className="overflow-hidden rounded-[9px] border border-[#e8e3dc]">
      {groups.map(([resource, items]) => (
        <PermissionGroup key={resource} resource={resource} items={items} />
      ))}
    </div>
  );
}

function PermissionGroup({ resource, items }: { resource: string; items: AdminRolePermissionDto[]; }) {
  const [open, setOpen] = useState(true);

  return (
    <div className="border-b border-[#eee9e2] last:border-0">
      <button
        type="button"
        onClick={() => setOpen((value) => !value)}
        className="flex w-full items-center gap-3 bg-[#faf9f7] px-4 py-3 text-left transition hover:bg-[#f7f6f3]"
      >
        <ChevronDown size={14} className={`text-[#777] transition-transform ${open ? "" : "-rotate-90"}`} />
        <span className="flex-1 text-[11px] font-semibold uppercase tracking-[0.04em] text-[#444]">
          {formatResourceName(resource)}
        </span>
        <Badge>{items.length} quyền</Badge>
      </button>

      {open && (
        <div className="grid gap-2 bg-white p-4 md:grid-cols-2 xl:grid-cols-3">
          {items.map((permission) => (
            <div key={permission.id} className="rounded-[8px] border border-[#e9e5de] bg-white p-3">
              <div className="flex items-center justify-between gap-2">
                <span className="text-[11px] font-semibold text-[#444]">
                  {formatActionName(permission.action ?? getActionFromCode(permission.code))}
                </span>
                <KeyRound size={13} className="text-[#16975b]" />
              </div>
              <code className="mt-2 block break-all text-[9px] text-[#16975b]">{permission.code}</code>
              {permission.description && (
                <p className="mt-2 text-[9px] leading-[14px] text-[#919191]">{permission.description}</p>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

function getResourceFromCode(code: string) {
  return code.split(".")[0] ?? "other";
}

function getActionFromCode(code: string) {
  const parts = code.split(".");
  return parts.slice(1).join(".") || "unknown";
}

function formatResourceName(resource: string) {
  const names: Record<string, string> = {
    users: "Người dùng",
    roles: "Vai trò",
    permissions: "Quyền hạn",
    sessions: "Phiên đăng nhập",
    courses: "Khóa học",
    chapters: "Chương học",
    lessons: "Bài giảng",
    vocabulary: "Từ vựng",
    "question-bank": "Ngân hàng câu hỏi",
    quizzes: "Bài kiểm tra",
    media: "Media",
    notifications: "Thông báo",
    "audit-logs": "Nhật ký hệ thống",
    "system-settings": "Cấu hình hệ thống",
  };
  return names[resource] ?? resource;
}

function formatActionName(action: string) {
  const names: Record<string, string> = {
    read: "Xem",
    create: "Tạo",
    update: "Cập nhật",
    delete: "Xóa",
    restore: "Khôi phục",
    lock: "Khóa",
    unlock: "Mở khóa",
    import: "Nhập dữ liệu",
    export: "Xuất dữ liệu",
    publish: "Xuất bản",
    unpublish: "Hủy xuất bản",
    approve: "Phê duyệt",
    reject: "Từ chối",
    review: "Kiểm duyệt",
    "roles.manage": "Quản lý vai trò",
    "permissions.manage": "Quản lý quyền",
    revoke: "Thu hồi phiên",
    "revoke-all": "Thu hồi toàn bộ",
  };
  return names[action] ?? action;
}
