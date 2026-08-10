"use client";

import { Lock } from "lucide-react";
import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { DatePicker } from "@/components/ui/date-picker";
import { Textarea } from "@/components/ui/textarea";
import { FormField } from "@/components/forms/form-field";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";
import { useLockAdminUser } from "../hooks/use-admin-users";

interface UserLockDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  userId: string;
  userName: string;
  concurrencyToken?: string;
  onSuccess?: () => void | Promise<void>;
}

export function UserLockDialog({
  open,
  onOpenChange,
  userId,
  userName,
  concurrencyToken,
  onSuccess,
}: UserLockDialogProps) {
  const [reason, setReason] = useState("");
  const [until, setUntil] = useState("");
  const [error, setError] = useState("");

  const mutation = useLockAdminUser(userId);

  useEffect(() => {
    if (!open) {
      setReason("");
      setUntil("");
      setError("");
    }
  }, [open]);

  if (!open) {
    return null;
  }

  async function submit() {
    const normalized = reason.trim();
    if (normalized.length < 3) {
      setError("Vui lòng nhập lý do khóa tài khoản.");
      return;
    }

    try {
      await mutation.mutateAsync({
        reason: normalized,
        until: until || null,
        concurrencyToken,
      });

      appToast.success(
        "Đã khóa tài khoản",
        `${userName} đã bị khóa và các phiên đang hoạt động sẽ được thu hồi.`
      );

      onOpenChange(false);
      await onSuccess?.();
    } catch (error) {
      const apiError = normalizeApiError(error);
      appToast.error("Không thể khóa tài khoản", apiError.message);
    }
  }

  return (
    <div className="fixed inset-0 z-[130] flex items-center justify-center p-4">
      <button
        type="button"
        onClick={() => onOpenChange(false)}
        className="absolute inset-0 bg-black/40"
      />

      <section className="relative z-10 w-full max-w-[520px] overflow-hidden rounded-[14px] border border-[#e7e2da] bg-white shadow-2xl">
        <div className="p-5">
          <div className="flex items-start gap-3">
            <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-[9px] bg-[#fff0ee] text-[#ef241c]">
              <Lock size={19} />
            </div>
            <div>
              <h2 className="text-[15px] font-semibold text-[#333]">
                Khóa tài khoản
              </h2>
              <p className="mt-1 text-[11px] leading-[17px] text-[#777]">
                Bạn đang khóa <strong>{userName}</strong>. Các phiên đăng nhập đang hoạt động sẽ bị thu hồi.
              </p>
            </div>
          </div>

          <div className="mt-5 space-y-4">
            <FormField label="Lý do khóa" required error={error}>
              <Textarea
                value={reason}
                onChange={(event) => {
                  setReason(event.target.value);
                  setError("");
                }}
                placeholder="Ví dụ: Phát hiện hoạt động đăng nhập bất thường..."
                className="min-h-[100px]"
              />
            </FormField>

            <FormField
              label="Khóa đến ngày"
              description="Để trống nếu khóa cho đến khi quản trị viên mở khóa thủ công."
            >
              <DatePicker
                value={until}
                onChange={setUntil}
                min={new Date().toISOString().slice(0, 10)}
              />
            </FormField>
          </div>
        </div>

        <div className="flex justify-end gap-2 border-t border-[#ebe6df] bg-[#faf9f7] px-5 py-3">
          <Button
            type="button"
            variant="outline"
            disabled={mutation.isPending}
            onClick={() => onOpenChange(false)}
            className="h-[38px]"
          >
            Hủy
          </Button>
          <Button
            type="button"
            variant="danger"
            loading={mutation.isPending}
            onClick={submit}
            className="h-[38px] gap-2 text-[11px]"
          >
            <Lock size={14} />
            Khóa tài khoản
          </Button>
        </div>
      </section>
    </div>
  );
}
