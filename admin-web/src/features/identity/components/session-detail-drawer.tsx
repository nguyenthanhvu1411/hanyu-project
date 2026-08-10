"use client";

import {
  Activity,
  Clock3,
  Globe2,
  Laptop,
  Network,
  ShieldCheck,
  UserRound,
} from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { StatusBadge } from "@/components/common/status-badge";
import { ViewDetailDrawer } from "@/components/common/view-detail-drawer";
import { CopyButton } from "@/components/common/copy-button";
import { formatDateTime } from "@/utils/date.util";
import type { AdminSessionDto } from "@/dto/identity/admin-session.dto";

interface SessionDetailDrawerProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  session: AdminSessionDto | null;
}

export function SessionDetailDrawer({
  open,
  onOpenChange,
  session,
}: SessionDetailDrawerProps) {
  if (!session) {
    return null;
  }

  return (
    <ViewDetailDrawer
      open={open}
      onOpenChange={onOpenChange}
      title={`Phiên #${session.id}`}
      description="Thông tin chi tiết phiên đăng nhập."
      metadata={{
        id: session.id,
        createdAt: formatDateTime(session.createdAt),
      }}
    >
      <div className="grid gap-3 md:grid-cols-2">
        <SessionField icon={<UserRound size={15} />} label="Người dùng">
          <div>{session.userDisplayName ?? "-"}</div>
          <div className="text-[9px] text-[#999]">
            {session.userEmail ?? `User #${session.userId}`}
          </div>
        </SessionField>

        <SessionField icon={<ShieldCheck size={15} />} label="Trạng thái">
          {session.isActive ? (
            <StatusBadge variant="success">Hoạt động</StatusBadge>
          ) : (
            <StatusBadge variant="neutral">Đã thu hồi</StatusBadge>
          )}
        </SessionField>

        <SessionField icon={<Laptop size={15} />} label="Thiết bị">
          {session.deviceInfo ?? "-"}
        </SessionField>

        <SessionField icon={<Network size={15} />} label="Địa chỉ IP">
          <div className="flex items-center gap-1">
            <span>{session.ipAddress ?? "-"}</span>
            {session.ipAddress && <CopyButton value={session.ipAddress} />}
          </div>
        </SessionField>

        <SessionField icon={<Clock3 size={15} />} label="Hoạt động cuối">
          {formatDateTime(session.lastUsedAt)}
        </SessionField>

        <SessionField icon={<Clock3 size={15} />} label="Hết hạn">
          {formatDateTime(session.expiresAt)}
        </SessionField>

        <SessionField icon={<Activity size={15} />} label="Thu hồi lúc">
          {formatDateTime(session.revokedAt)}
        </SessionField>

        <SessionField icon={<Activity size={15} />} label="Lý do thu hồi">
          {session.revokedReason ?? "-"}
        </SessionField>
      </div>

      <div className="rounded-[10px] border border-[#e8e3dc] bg-[#faf9f7] p-4">
        <div className="flex items-center gap-2 text-[11px] font-semibold text-[#444]">
          <Globe2 size={14} />
          User Agent
        </div>
        <div className="mt-2 break-all rounded-[6px] bg-white p-3 font-mono text-[9px] leading-[15px] text-[#777]">
          {session.userAgent ?? "-"}
        </div>
      </div>
    </ViewDetailDrawer>
  );
}

function SessionField({
  icon,
  label,
  children,
}: {
  icon: React.ReactNode;
  label: string;
  children: React.ReactNode;
}) {
  return (
    <div className="rounded-[9px] border border-[#e9e4dc] bg-white p-3">
      <div className="flex items-center gap-2 text-[9px] uppercase tracking-[0.04em] text-[#999]">
        {icon}
        {label}
      </div>
      <div className="mt-2 text-[11px] font-medium text-[#444]">{children}</div>
    </div>
  );
}
