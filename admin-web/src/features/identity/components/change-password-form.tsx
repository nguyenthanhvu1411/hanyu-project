"use client";

import { FormEvent, useState } from "react";
import { KeyRound } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";

import { authApi } from "../auth/auth.api";

export function ChangePasswordForm() {
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [saving, setSaving] = useState(false);

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!currentPassword || !newPassword || !confirmPassword) {
      appToast.error("Thiếu thông tin", "Vui lòng nhập đầy đủ mật khẩu hiện tại và mật khẩu mới.");
      return;
    }
    if (newPassword !== confirmPassword) {
      appToast.error("Mật khẩu không khớp", "Xác nhận mật khẩu mới không trùng khớp.");
      return;
    }

    setSaving(true);
    try {
      await authApi.changePassword({ currentPassword, newPassword, confirmPassword });
      appToast.success("Đã đổi mật khẩu.");
      setCurrentPassword("");
      setNewPassword("");
      setConfirmPassword("");
    } catch (caught) {
      appToast.error("Không thể đổi mật khẩu", normalizeApiError(caught).message);
    } finally {
      setSaving(false);
    }
  }

  return (
    <Card className="max-w-2xl">
      <CardHeader>
        <CardTitle>Đổi mật khẩu</CardTitle>
        <p className="mt-1 text-[11px] text-muted-foreground">Thao tác trực tiếp qua `/auth/change-password`.</p>
      </CardHeader>
      <CardContent>
        <form onSubmit={submit} className="space-y-4">
          <label className="block space-y-1"><span className="text-[11px] font-medium">Mật khẩu hiện tại</span><Input type="password" autoComplete="current-password" value={currentPassword} onChange={(event) => setCurrentPassword(event.target.value)} /></label>
          <label className="block space-y-1"><span className="text-[11px] font-medium">Mật khẩu mới</span><Input type="password" autoComplete="new-password" value={newPassword} onChange={(event) => setNewPassword(event.target.value)} /></label>
          <label className="block space-y-1"><span className="text-[11px] font-medium">Xác nhận mật khẩu mới</span><Input type="password" autoComplete="new-password" value={confirmPassword} onChange={(event) => setConfirmPassword(event.target.value)} /></label>
          <Button type="submit" loading={saving} className="gap-2"><KeyRound size={14} />Đổi mật khẩu</Button>
        </form>
      </CardContent>
    </Card>
  );
}
