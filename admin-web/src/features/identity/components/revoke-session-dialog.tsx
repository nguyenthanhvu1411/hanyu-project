"use client";

import {
  LogOut,
} from "lucide-react";

import {
  ConfirmDialog,
} from "@/components/common/confirm-dialog";

import {
  appToast,
} from "@/components/ui/toast";

import {
  normalizeApiError,
} from "@/lib/api/api-error";

import {
  useDeleteAdminSession,
} from "../hooks/use-admin-sessions";

interface RevokeSessionDialogProps {
  open: boolean;

  onOpenChange: (
    open: boolean,
  ) => void;

  sessionId?: number;

  userName?: string;

  onSuccess?: () =>
    void | Promise<void>;
}

export function RevokeSessionDialog({
  open,
  onOpenChange,
  sessionId,
  userName,
  onSuccess,
}: RevokeSessionDialogProps) {
  const mutation =
    useDeleteAdminSession();

  async function revoke() {
    if (!sessionId) {
      return;
    }

    try {
      await mutation.mutateAsync(
        sessionId,
      );

      appToast.success(
        "Đã thu hồi phiên đăng nhập.",
      );

      onOpenChange(
        false,
      );

      await onSuccess?.();
    } catch (error) {
      const apiError =
        normalizeApiError(
          error,
        );

      appToast.error(
        "Không thể thu hồi phiên",
        apiError.message,
      );
    }
  }

  return (
    <ConfirmDialog
      open={open}
      onOpenChange={
        onOpenChange
      }
      variant="danger"
      title="Thu hồi phiên đăng nhập"
      description={
        userName
          ? `Phiên đăng nhập của ${userName} sẽ bị vô hiệu hóa.`
          : "Phiên đăng nhập này sẽ bị vô hiệu hóa."
      }
      confirmText="Thu hồi phiên"
      loading={
        mutation.isPending
      }
      onConfirm={
        revoke
      }
    >
      <div
        className="
          flex
          items-center
          gap-3
          rounded-[8px]
          bg-[#fff7f5]
          p-3
        "
      >
        <div
          className="
            flex h-9 w-9
            items-center
            justify-center
            rounded-[8px]
            bg-[#fff0ee]
            text-[#ef241c]
          "
        >
          <LogOut
            size={16}
          />
        </div>

        <div
          className="
            text-[10px]
            leading-[16px]
            text-[#777]
          "
        >
          Người dùng sẽ cần đăng nhập
          lại nếu đây là phiên đang
          hoạt động.
        </div>
      </div>
    </ConfirmDialog>
  );
}
