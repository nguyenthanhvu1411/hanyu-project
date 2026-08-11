"use client";

import { KeyRound, X } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { FormField } from "@/components/forms/form-field";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";
import { useResetAdminUserPassword } from "../hooks/use-reset-admin-user-password";

interface ResetUserPasswordDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  userId: string;
  userName: string;
  onSuccess?: () => void | Promise<void>;
}

export function ResetUserPasswordDialog({
  open,
  onOpenChange,
  userId,
  userName,
  onSuccess,
}: ResetUserPasswordDialogProps) {
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");
  const mutation = useResetAdminUserPassword(userId);

  useEffect(() => {
    if (!open) {
      setNewPassword("");
      setConfirmPassword("");
      setError("");
    }
  }, [open]);

  const passwordValid = useMemo(
    () =>
      newPassword.length >= 12 &&
      /[a-z]/.test(newPassword) &&
      /[A-Z]/.test(newPassword) &&
      /\d/.test(newPassword),
    [newPassword],
  );

  if (!open) return null;

  async function submit() {
    if (!passwordValid) {
      setError("Mật khẩu phải có ít nhất 12 ký tự, gồm chữ hoa, chữ thường và chữ số.");
      return;
    }

    if (newPassword !== confirmPassword) {
      setError("Mật khẩu xác nhận không khớp.");
      return;
    }

    try {
      await mutation.mutateAsync({ newPassword });
      appToast.success(
        "Đặt lại mật khẩu thành công",
        `Mật khẩu của ${userName} đã được thay đổi.`,
      );
      onOpenChange(false);
      await onSuccess?.();
    } catch (exception) {
      const apiError = normalizeApiError(exception);
      appToast.error("Không thể đặt lại mật khẩu", apiError.message);
    }
  }

  return (
    <div className="fixed inset-0 z-[140] flex items-center justify-center p-4">
      <button
        type="button"
        aria-label="Đóng hộp thoại đặt lại mật khẩu"
        title="Đóng"
        className="absolute inset-0 bg-black/40"
        onClick={() => onOpenChange(false)}
      />

      <section className="relative z-10 w-full max-w-[520px] overflow-hidden rounded-[14px] border border-[#e7e2da] bg-white shadow-2xl">
        <div className="flex items-start justify-between border-b border-[#ebe6df] p-5">
          <div className="flex items-start gap-3">
            <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-[9px] bg-[#fff0ee] text-[#ef241c]">
              <KeyRound size={19} />
            </div>
            <div>
              <h2 className="text-[15px] font-semibold text-[#333]">Đặt lại mật khẩu</h2>
              <p className="mt-1 text-[11px] leading-[17px] text-[#777]">
                Đặt mật khẩu mới cho <strong>{userName}</strong>. Chức năng này chỉ dành cho SuperAdmin.
              </p>
            </div>
          </div>
          <button
            type="button"
            title="Đóng"
            aria-label="Đóng"
            onClick={() => onOpenChange(false)}
            className="rounded-md p-1.5 text-[#777] hover:bg-[#f5f3f0]"
          >
            <X size={17} />
          </button>
        </div>

        <div className="space-y-4 p-5">
          <FormField
            label="Mật khẩu mới"
            required
            description="Ít nhất 12 ký tự, có chữ hoa, chữ thường và chữ số."
          >
            <Input
              type="password"
              autoComplete="new-password"
              value={newPassword}
              onChange={(event) => {
                setNewPassword(event.target.value);
                setError("");
              }}
              placeholder="Nhập mật khẩu mới"
            />
          </FormField>

          <FormField label="Xác nhận mật khẩu" required error={error}>
            <Input
              type="password"
              autoComplete="new-password"
              value={confirmPassword}
              onChange={(event) => {
                setConfirmPassword(event.target.value);
                setError("");
              }}
              placeholder="Nhập lại mật khẩu mới"
              onKeyDown={(event) => {
                if (event.key === "Enter") void submit();
              }}
            />
          </FormField>
        </div>

        <div className="flex justify-end gap-2 border-t border-[#ebe6df] bg-[#faf9f7] px-5 py-3">
          <Button
            type="button"
            variant="outline"
            disabled={mutation.isPending}
            onClick={() => onOpenChange(false)}
          >
            Hủy
          </Button>
          <Button
            type="button"
            variant="danger"
            loading={mutation.isPending}
            onClick={submit}
            title="Đặt lại mật khẩu cho người dùng"
            className="gap-2"
          >
            <KeyRound size={14} />
            Đặt lại mật khẩu
          </Button>
        </div>
      </section>
    </div>
  );
}
