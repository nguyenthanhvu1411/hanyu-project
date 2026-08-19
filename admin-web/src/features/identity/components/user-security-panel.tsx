"use client";

import { KeyRound, LogOut, ShieldAlert, ShieldCheck } from "lucide-react";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { ErrorState } from "@/components/common/error-state";
import { PageLoading } from "@/components/common/page-loading";
import { UserUnlockDialog } from "./user-unlock-dialog";
import { UserLockDialog } from "./user-lock-dialog";
import { RevokeUserSessionsDialog } from "./revoke-user-sessions-dialog";
import { ResetUserPasswordDialog } from "./reset-user-password-dialog";
import { useAdminUser } from "../hooks/use-admin-users";
import { useCurrentUser } from "../auth/hooks/use-current-user";
import { PermissionGuard } from "@/security/permission-guard";
import { ConcurrencyConflictDialog } from "@/components/common/concurrency-conflict-dialog";
import { PERMISSIONS } from "@/constants/permission.constants";

interface UserSecurityPanelProps {
  userId: string;
}

export function UserSecurityPanel({ userId }: UserSecurityPanelProps) {
  const query = useAdminUser(userId);
  const currentUserQuery = useCurrentUser();

  const [lockOpen, setLockOpen] = useState(false);
  const [unlockOpen, setUnlockOpen] = useState(false);
  const [revokeOpen, setRevokeOpen] = useState(false);
  const [resetPasswordOpen, setResetPasswordOpen] = useState(false);
  const [conflictOpen, setConflictOpen] = useState(false);

  if (query.isLoading) {
    return <PageLoading text="Đang tải dữ liệu bảo mật..." />;
  }

  if (query.isError || !query.data) {
    return <ErrorState onRetry={() => void query.refetch()} />;
  }

  const user = query.data;
  const isSuperAdmin = currentUserQuery.data?.roles?.includes("SUPER_ADMIN") === true;

  return (
    <>
      <div className="space-y-4">
        <PermissionGuard permission="user.manage" fallback={null}>
          {user.status === "locked" ? (
            <div className="flex items-center justify-between rounded-[11px] border border-[#efc5c1] bg-[#fff7f5] p-5">
              <div className="flex items-center gap-3">
                <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-[10px] bg-[#fff0ee] text-[#ef241c]">
                  <ShieldAlert size={20} />
                </div>
                <div>
                  <h3 className="text-[13px] font-semibold text-[#d8322b]">Tài khoản đang bị khóa</h3>
                  <p className="mt-1 text-[11px] text-[#888]">
                    {user.lockedUntil
                      ? `Khóa đến ${new Date(user.lockedUntil).toLocaleString("vi-VN")}`
                      : "Khóa cho đến khi quản trị viên mở khóa thủ công."}
                  </p>
                </div>
              </div>
              <Button
                variant="outline"
                onClick={() => setUnlockOpen(true)}
                className="gap-2 bg-white text-[11px] text-[#16975b] hover:bg-[#edf8f2]"
                title="Mở khóa để người dùng có thể đăng nhập trở lại"
              >
                <ShieldCheck size={14} />
                Mở khóa tài khoản
              </Button>
            </div>
          ) : (
            <div className="flex items-center justify-between rounded-[11px] border border-[#e8e3dc] bg-white p-5">
              <div className="flex items-center gap-3">
                <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-[10px] bg-[#edf8f2] text-[#16975b]">
                  <ShieldCheck size={20} />
                </div>
                <div>
                  <h3 className="text-[13px] font-semibold text-[#333]">Tài khoản đang hoạt động</h3>
                  <p className="mt-1 text-[11px] text-[#888]">Người dùng có thể đăng nhập bình thường.</p>
                </div>
              </div>
              <Button
                variant="danger"
                onClick={() => setLockOpen(true)}
                className="gap-2 text-[11px]"
                title="Khóa tài khoản người dùng"
              >
                <ShieldAlert size={14} />
                Khóa tài khoản
              </Button>
            </div>
          )}
        </PermissionGuard>

        {isSuperAdmin ? (
          <div className="flex items-center justify-between rounded-[11px] border border-[#e8e3dc] bg-white p-5">
            <div className="flex items-center gap-3">
              <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-[10px] bg-[#fff0ee] text-[#ef241c]">
                <KeyRound size={20} />
              </div>
              <div>
                <h3 className="text-[13px] font-semibold text-[#333]">Đặt lại mật khẩu</h3>
                <p className="mt-1 text-[11px] text-[#888]">
                  Tạo mật khẩu mới cho tài khoản. Chỉ SuperAdmin được sử dụng chức năng này.
                </p>
              </div>
            </div>
            <Button
              variant="outline"
              onClick={() => setResetPasswordOpen(true)}
              className="gap-2 text-[11px]"
              title="Đặt lại mật khẩu cho người dùng"
            >
              <KeyRound size={14} />
              Đặt lại mật khẩu
            </Button>
          </div>
        ) : null}

        <PermissionGuard permission={PERMISSIONS.SESSIONS.REVOKE} fallback={null}>
          <div className="flex items-center justify-between rounded-[11px] border border-[#e8e3dc] bg-white p-5">
            <div className="flex items-center gap-3">
              <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-[10px] bg-[#fff0ee] text-[#ef241c]">
                <LogOut size={20} />
              </div>
              <div>
                <h3 className="text-[13px] font-semibold text-[#333]">Thu hồi phiên đăng nhập</h3>
                <p className="mt-1 text-[11px] text-[#888]">
                  Thu hồi tất cả {user.activeSessionCount ?? 0} phiên đang hoạt động.
                </p>
              </div>
            </div>
            <Button
              variant="outline"
              onClick={() => setRevokeOpen(true)}
              disabled={!user.activeSessionCount || user.activeSessionCount === 0}
              className="gap-2 text-[11px]"
              title="Đăng xuất tài khoản khỏi tất cả phiên đang hoạt động"
            >
              <LogOut size={14} />
              Thu hồi phiên
            </Button>
          </div>
        </PermissionGuard>
      </div>

      <UserLockDialog
        open={lockOpen}
        onOpenChange={setLockOpen}
        userId={user.id}
        userName={user.displayName}
        concurrencyToken={user.concurrencyToken}
        onSuccess={async () => { await query.refetch(); }}
      />

      <UserUnlockDialog
        open={unlockOpen}
        onOpenChange={setUnlockOpen}
        userId={user.id}
        userName={user.displayName}
        concurrencyToken={user.concurrencyToken}
        onSuccess={async () => { await query.refetch(); }}
      />

      <ResetUserPasswordDialog
        open={resetPasswordOpen}
        onOpenChange={setResetPasswordOpen}
        userId={user.id}
        userName={user.displayName}
        onSuccess={async () => { await query.refetch(); }}
      />

      <RevokeUserSessionsDialog
        open={revokeOpen}
        onOpenChange={setRevokeOpen}
        userId={user.id}
        userName={user.displayName}
        activeSessions={user.activeSessionCount}
        onSuccess={async () => { await query.refetch(); }}
      />

      <ConcurrencyConflictDialog
        open={conflictOpen}
        onCancel={() => setConflictOpen(false)}
        onReload={async () => {
          await query.refetch();
          setConflictOpen(false);
        }}
      />
    </>
  );
}
