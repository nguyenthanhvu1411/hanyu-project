"use client";

import { Unlock } from "lucide-react";
import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { FormField } from "@/components/forms/form-field";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";
import { useUnlockAdminUser } from "../hooks/use-admin-users";

interface UserUnlockDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  userId: string;
  userName: string;
  concurrencyToken?: string;
  onSuccess?: () => void | Promise<void>;
}

export function UserUnlockDialog({
  open,
  onOpenChange,
  userId,
  userName,
  concurrencyToken,
  onSuccess,
}: UserUnlockDialogProps) {
  const [reason, setReason] = useState("");
  const [error, setError] = useState("");

  const mutation = useUnlockAdminUser(userId);

  useEffect(() => {
    if (!open) {
      setReason("");
      setError("");
    }
  }, [open]);

  if (!open) {
    return null;
  }

  async function submit() {
    if (reason.trim().length < 3) {
      setError("Vui lòng ghi lý do mở khóa.");
      return;
    }

    try {
      await mutation.mutateAsync({
        reason: reason.trim(),
        concurrencyToken,
      });

      appToast.success("Đã mở khóa tài khoản.");
      onOpenChange(false);
      await onSuccess?.();
    } catch (error) {
      const apiError = normalizeApiError(error);
      appToast.error("Không thể mở khóa", apiError.message);
    }
  }

  return (
    <div className="fixed inset-0 z-[130] flex items-center justify-center p-4">
      <button
        type="button"
        className="absolute inset-0 bg-black/40"
        onClick={() => onOpenChange(false)}
      />

      <section className="relative w-full max-w-[500px] overflow-hidden rounded-[14px] bg-white shadow-2xl">
        <div className="p-5">
          <div className="flex gap-3">
            <div className="flex h-10 w-10 items-center justify-center rounded-[9px] bg-[#edf8f2] text-[#16975b]">
              <Unlock size={18} />
            </div>
            <div>
              <h2 className="text-[15px] font-semibold">Mở khóa tài khoản</h2>
              <p className="mt-1 text-[11px] text-[#777]">
                Mở lại quyền đăng nhập cho <strong>{userName}</strong>.
              </p>
            </div>
          </div>

          <div className="mt-5">
            <FormField label="Lý do mở khóa" required error={error}>
              <Textarea
                value={reason}
                onChange={(event) => {
                  setReason(event.target.value);
                  setError("");
                }}
                placeholder="Ví dụ: Đã xác minh hoạt động tài khoản an toàn..."
              />
            </FormField>
          </div>
        </div>

        <div className="flex justify-end gap-2 border-t border-[#ebe6df] bg-[#faf9f7] px-5 py-3">
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Hủy
          </Button>
          <Button
            loading={mutation.isPending}
            onClick={submit}
            className="gap-2 bg-[#16975b] hover:bg-[#12814d]"
          >
            <Unlock size={14} />
            Mở khóa
          </Button>
        </div>
      </section>
    </div>
  );
}
