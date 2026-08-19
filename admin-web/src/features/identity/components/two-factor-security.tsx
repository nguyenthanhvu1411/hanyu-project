"use client";

import { FormEvent, useState } from "react";
import { Copy, KeyRound, RefreshCw, ShieldCheck, ShieldOff } from "lucide-react";

import { Alert } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Dialog } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";

import { authApi } from "../auth/auth.api";
import type { TwoFactorSetupResponse } from "../auth/auth.types";

type PasswordAction = "recovery" | "regenerate" | null;

export function TwoFactorSecurity() {
  const [setup, setSetup] = useState<TwoFactorSetupResponse | null>(null);
  const [setupOpen, setSetupOpen] = useState(false);
  const [code, setCode] = useState("");
  const [recoveryCodes, setRecoveryCodes] = useState<string[]>([]);
  const [recoveryOpen, setRecoveryOpen] = useState(false);
  const [passwordAction, setPasswordAction] = useState<PasswordAction>(null);
  const [password, setPassword] = useState("");
  const [disableOpen, setDisableOpen] = useState(false);
  const [disablePassword, setDisablePassword] = useState("");
  const [disableCode, setDisableCode] = useState("");
  const [working, setWorking] = useState(false);

  async function copy(value: string, label: string) {
    try {
      await navigator.clipboard.writeText(value);
      appToast.success(`Đã sao chép ${label}.`);
    } catch {
      appToast.error("Không thể sao chép", "Trình duyệt không cho phép truy cập clipboard.");
    }
  }

  async function beginSetup() {
    setWorking(true);
    try {
      const result = await authApi.setupTwoFactor();
      setSetup(result);
      setCode("");
      setSetupOpen(true);
    } catch (caught) {
      appToast.error("Không thể bắt đầu thiết lập 2FA", normalizeApiError(caught).message);
    } finally {
      setWorking(false);
    }
  }

  async function enable(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!code.trim()) {
      appToast.error("Thiếu mã xác thực", "Nhập mã 6 số từ ứng dụng Authenticator.");
      return;
    }

    setWorking(true);
    try {
      const result = await authApi.enableTwoFactor({ code: code.trim() });
      setSetupOpen(false);
      setSetup(null);
      setCode("");
      setRecoveryCodes(result.recoveryCodes ?? []);
      setRecoveryOpen(true);
      appToast.success("Đã bật xác thực hai lớp.");
    } catch (caught) {
      appToast.error("Không thể bật 2FA", normalizeApiError(caught).message);
    } finally {
      setWorking(false);
    }
  }

  async function passwordSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!passwordAction || !password) return;

    setWorking(true);
    try {
      if (passwordAction === "recovery") {
        const result = await authApi.generateRecoveryCodes({ password });
        setRecoveryCodes(result.recoveryCodes ?? []);
        setPasswordAction(null);
        setPassword("");
        setRecoveryOpen(true);
        appToast.success("Đã tạo bộ recovery codes mới.");
      } else {
        const result = await authApi.regenerateAuthenticatorKey({ password });
        setPasswordAction(null);
        setPassword("");
        setSetup(result);
        setCode("");
        setSetupOpen(true);
        appToast.success("Đã tạo khóa Authenticator mới. Hãy xác minh lại để bật 2FA.");
      }
    } catch (caught) {
      appToast.error("Không thể cập nhật 2FA", normalizeApiError(caught).message);
    } finally {
      setWorking(false);
    }
  }

  async function disable(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!disablePassword || !disableCode.trim()) {
      appToast.error("Thiếu xác nhận", "Nhập mật khẩu và mã Authenticator hiện tại.");
      return;
    }

    setWorking(true);
    try {
      await authApi.disableTwoFactor({ password: disablePassword, code: disableCode.trim() });
      setDisableOpen(false);
      setDisablePassword("");
      setDisableCode("");
      setRecoveryCodes([]);
      appToast.success("Đã tắt xác thực hai lớp.");
    } catch (caught) {
      appToast.error("Không thể tắt 2FA", normalizeApiError(caught).message);
    } finally {
      setWorking(false);
    }
  }

  return (
    <>
      <Card>
        <CardHeader>
          <CardTitle>Xác thực hai lớp (2FA)</CardTitle>
          <p className="mt-1 text-[11px] leading-5 text-muted-foreground">
            Bảo vệ tài khoản bằng ứng dụng Authenticator. Mã bí mật và recovery codes chỉ được hiển thị trong phiên thao tác hiện tại.
          </p>
        </CardHeader>
        <CardContent className="space-y-4">
          <Alert variant="info" title="Quản lý bằng xác minh thực tế">
            Backend không có endpoint đọc trạng thái 2FA riêng. Các thao tác bên dưới được backend xác minh trực tiếp: setup sẽ từ chối nếu đã bật, còn recovery/disable sẽ từ chối nếu chưa bật hoặc xác nhận không hợp lệ.
          </Alert>

          <div className="grid gap-3 md:grid-cols-2 xl:grid-cols-4">
            <SecurityAction icon={<ShieldCheck size={18} />} title="Thiết lập / bật" description="Lấy khóa Authenticator và xác minh mã TOTP." action="Thiết lập" loading={working} onClick={() => void beginSetup()} />
            <SecurityAction icon={<KeyRound size={18} />} title="Recovery codes" description="Tạo mới 10 mã khôi phục sau khi xác nhận mật khẩu." action="Tạo mã mới" onClick={() => { setPassword(""); setPasswordAction("recovery"); }} />
            <SecurityAction icon={<RefreshCw size={18} />} title="Đổi khóa" description="Hủy khóa hiện tại và thiết lập Authenticator lại từ đầu." action="Đổi khóa" onClick={() => { setPassword(""); setPasswordAction("regenerate"); }} />
            <SecurityAction icon={<ShieldOff size={18} />} title="Tắt 2FA" description="Yêu cầu mật khẩu và mã Authenticator hiện tại." action="Tắt 2FA" danger onClick={() => { setDisablePassword(""); setDisableCode(""); setDisableOpen(true); }} />
          </div>
        </CardContent>
      </Card>

      <Dialog
        open={setupOpen}
        onOpenChange={(open) => { if (!working) setSetupOpen(open); }}
        title="Thiết lập Authenticator"
        description="Thêm tài khoản bằng URI hoặc shared key, sau đó nhập mã 6 số để xác minh."
        size="lg"
        footer={<div className="flex justify-end gap-2"><Button variant="outline" disabled={working} onClick={() => setSetupOpen(false)}>Đóng</Button><Button loading={working} onClick={() => document.getElementById("two-factor-enable-form")?.dispatchEvent(new Event("submit", { bubbles: true, cancelable: true }))}>Xác minh & bật 2FA</Button></div>}
      >
        {setup ? (
          <form id="two-factor-enable-form" className="space-y-4" onSubmit={enable}>
            <Alert variant="warning" title="Thông tin nhạy cảm">
              Không gửi shared key hoặc Authenticator URI cho người khác. Đóng dialog sau khi hoàn tất thiết lập.
            </Alert>
            <SecretField label="Shared key" value={setup.sharedKey} onCopy={() => void copy(setup.sharedKey, "shared key")} />
            <SecretField label="Authenticator URI" value={setup.authenticatorUri} onCopy={() => void copy(setup.authenticatorUri, "Authenticator URI")} />
            <label className="block space-y-1"><span className="text-[11px] font-medium">Mã xác thực *</span><Input inputMode="numeric" autoComplete="one-time-code" value={code} onChange={(event) => setCode(event.target.value)} placeholder="123456" /></label>
          </form>
        ) : null}
      </Dialog>

      <Dialog
        open={recoveryOpen}
        onOpenChange={setRecoveryOpen}
        title="Recovery codes"
        description="Mỗi mã chỉ sử dụng được một lần. Hãy lưu ở nơi an toàn trước khi đóng."
        size="md"
        footer={<div className="flex justify-between gap-2"><Button variant="outline" onClick={() => void copy(recoveryCodes.join("\n"), "recovery codes")} disabled={recoveryCodes.length === 0}><Copy size={14} />Sao chép tất cả</Button><Button onClick={() => setRecoveryOpen(false)}>Đã lưu an toàn</Button></div>}
      >
        <Alert variant="warning" title="Chỉ hiển thị lần này">Tạo bộ mã mới sẽ làm bộ mã trước đó không còn giá trị.</Alert>
        <div className="mt-4 grid grid-cols-2 gap-2">
          {recoveryCodes.map((item) => <code key={item} className="rounded-[7px] border bg-[#faf9f7] px-3 py-2 text-center text-[12px]">{item}</code>)}
        </div>
      </Dialog>

      <Dialog
        open={Boolean(passwordAction)}
        onOpenChange={(open) => { if (!open && !working) setPasswordAction(null); }}
        title={passwordAction === "regenerate" ? "Đổi khóa Authenticator" : "Tạo recovery codes mới"}
        description="Xác nhận mật khẩu tài khoản trước khi thực hiện thao tác nhạy cảm."
        footer={<div className="flex justify-end gap-2"><Button variant="outline" disabled={working} onClick={() => setPasswordAction(null)}>Hủy</Button><Button loading={working} onClick={() => document.getElementById("two-factor-password-form")?.dispatchEvent(new Event("submit", { bubbles: true, cancelable: true }))}>Xác nhận</Button></div>}
      >
        <form id="two-factor-password-form" onSubmit={passwordSubmit}><label className="block space-y-1"><span className="text-[11px] font-medium">Mật khẩu *</span><Input type="password" autoComplete="current-password" value={password} onChange={(event) => setPassword(event.target.value)} /></label></form>
      </Dialog>

      <Dialog
        open={disableOpen}
        onOpenChange={(open) => { if (!working) setDisableOpen(open); }}
        title="Tắt xác thực hai lớp"
        description="Sau khi tắt, đăng nhập chỉ còn được bảo vệ bằng mật khẩu."
        footer={<div className="flex justify-end gap-2"><Button variant="outline" disabled={working} onClick={() => setDisableOpen(false)}>Hủy</Button><Button variant="danger" loading={working} onClick={() => document.getElementById("two-factor-disable-form")?.dispatchEvent(new Event("submit", { bubbles: true, cancelable: true }))}>Tắt 2FA</Button></div>}
      >
        <form id="two-factor-disable-form" onSubmit={disable} className="space-y-3">
          <Alert variant="danger" title="Thao tác bảo mật">Bạn sẽ cần thiết lập Authenticator lại nếu muốn bật 2FA sau này.</Alert>
          <label className="block space-y-1"><span className="text-[11px] font-medium">Mật khẩu *</span><Input type="password" autoComplete="current-password" value={disablePassword} onChange={(event) => setDisablePassword(event.target.value)} /></label>
          <label className="block space-y-1"><span className="text-[11px] font-medium">Mã Authenticator *</span><Input inputMode="numeric" autoComplete="one-time-code" value={disableCode} onChange={(event) => setDisableCode(event.target.value)} placeholder="123456" /></label>
        </form>
      </Dialog>
    </>
  );
}

function SecurityAction({ icon, title, description, action, onClick, danger = false, loading = false }: { icon: React.ReactNode; title: string; description: string; action: string; onClick: () => void; danger?: boolean; loading?: boolean }) {
  return <div className="flex flex-col rounded-[10px] border border-[#e8e3dc] p-4"><div className="flex h-9 w-9 items-center justify-center rounded-[9px] bg-[#faf5ec] text-[#9a7440]">{icon}</div><div className="mt-3 text-[12px] font-semibold">{title}</div><p className="mt-1 flex-1 text-[10px] leading-4 text-muted-foreground">{description}</p><Button className="mt-3" size="sm" variant={danger ? "dangerGhost" : "outline"} loading={loading} onClick={onClick}>{action}</Button></div>;
}

function SecretField({ label, value, onCopy }: { label: string; value: string; onCopy: () => void }) {
  return <div className="rounded-[9px] border p-3"><div className="mb-2 flex items-center justify-between gap-2"><span className="text-[10px] font-medium uppercase tracking-wide text-muted-foreground">{label}</span><Button type="button" size="sm" variant="ghost" onClick={onCopy}><Copy size={13} />Sao chép</Button></div><code className="block break-all rounded-md bg-[#faf9f7] p-2 text-[11px] text-[#555]">{value}</code></div>;
}
