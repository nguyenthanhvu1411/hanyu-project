"use client";

import { LogOut, ShieldAlert } from "lucide-react";
import { ConfirmDialog } from "@/components/common/confirm-dialog";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";
import { useRevokeUserSessions } from "../hooks/use-admin-users";

interface RevokeUserSessionsDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  userId: string;
  userName: string;
  activeSessions?: number;
  onSuccess?: () => void | Promise<void>;
}

export function RevokeUserSessionsDialog({
  open,
  onOpenChange,
  userId,
  userName,
  activeSessions,
  onSuccess,
}: RevokeUserSessionsDialogProps) {
  const mutation = useRevokeUserSessions(userId);

  async function revoke() {
    try {
      await mutation.mutateAsync();
      appToast.success(
        "Đã thu hồi phiên đăng nhập",
        "Người dùng cần đăng nhập lại trên các thiết bị."
      );
      onOpenChange(false);
      await onSuccess?.();
    } catch (error) {
      const apiError = normalizeApiError(error);
      appToast.error("Không thể thu hồi phiên", apiError.message);
    }
  }

  return (
    <ConfirmDialog
      open={open}
      onOpenChange={onOpenChange}
      variant="danger"
      title="Thu hồi toàn bộ phiên đăng nhập"
      description={`Tất cả phiên đăng nhập của ${userName} sẽ bị thu hồi.`}
      confirmText="Thu hồi tất cả"
      loading={mutation.isPending}
      onConfirm={revoke}
    >
      <div className="flex items-start gap-3">
        <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-[8px] bg-[#fff0ee] text-[#ef241c]">
          <ShieldAlert size={17} />
        </div>
        <div>
          <div className="text-[11px] font-semibold text-[#444]">
            {activeSessions ?? 0} phiên đang hoạt động
          </div>
          <div className="mt-1 text-[10px] leading-[16px] text-[#888]">
            Access/refresh session tương ứng sẽ không còn được sử dụng.
          </div>
        </div>
      </div>
    </ConfirmDialog>
  );
}
