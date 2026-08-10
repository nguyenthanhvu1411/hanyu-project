"use client";

import { RotateCcw } from "lucide-react";
import { ConfirmDialog } from "@/components/common/confirm-dialog";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";
import { useRestoreAdminUser } from "../hooks/use-admin-users";

interface RestoreUserDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  userId: string;
  userName: string;
  onSuccess?: () => void | Promise<void>;
}

export function RestoreUserDialog({
  open,
  onOpenChange,
  userId,
  userName,
  onSuccess,
}: RestoreUserDialogProps) {
  const mutation = useRestoreAdminUser();

  async function restore() {
    try {
      await mutation.mutateAsync(userId);
      appToast.success("Khôi phục người dùng thành công.");
      onOpenChange(false);
      await onSuccess?.();
    } catch (error) {
      const apiError = normalizeApiError(error);
      appToast.error("Không thể khôi phục", apiError.message);
    }
  }

  return (
    <ConfirmDialog
      open={open}
      onOpenChange={onOpenChange}
      variant="success"
      title="Khôi phục người dùng"
      description={`Khôi phục tài khoản ${userName} và đưa tài khoản trở lại hệ thống.`}
      confirmText="Khôi phục"
      loading={mutation.isPending}
      onConfirm={restore}
    >
      <div className="flex items-center gap-3">
        <div className="flex h-9 w-9 items-center justify-center rounded-[8px] bg-[#edf8f2] text-[#16975b]">
          <RotateCcw size={16} />
        </div>
        <div className="text-[10px] leading-[16px] text-[#777]">
          Backend sẽ kiểm tra lại email, unique constraint và dependency trước khi khôi phục.
        </div>
      </div>
    </ConfirmDialog>
  );
}
