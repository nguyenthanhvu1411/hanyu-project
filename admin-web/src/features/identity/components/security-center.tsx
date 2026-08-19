"use client";

import { useCallback, useEffect, useState } from "react";
import { Laptop, RefreshCw, ShieldAlert, Trash2 } from "lucide-react";

import { EmptyState } from "@/components/common/empty-state";
import { ErrorState } from "@/components/common/error-state";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";
import { Skeleton } from "@/components/ui/skeleton";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";

import { authApi } from "../auth/auth.api";
import type { AuthSession, SecurityEvent } from "../auth/auth.types";
import { TwoFactorSecurity } from "./two-factor-security";

export function SecurityCenter() {
  const [sessions, setSessions] = useState<AuthSession[]>([]);
  const [events, setEvents] = useState<SecurityEvent[]>([]);
  const [loading, setLoading] = useState(true);
  const [working, setWorking] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [revokeTarget, setRevokeTarget] = useState<AuthSession | null>(null);
  const [revokeOthersOpen, setRevokeOthersOpen] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const [sessionData, eventData] = await Promise.all([
        authApi.sessions(),
        authApi.securityEvents(50),
      ]);
      setSessions(sessionData);
      setEvents(eventData);
    } catch (caught) {
      setError(normalizeApiError(caught).message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { void load(); }, [load]);

  async function revoke() {
    if (!revokeTarget || revokeTarget.isCurrent) return;
    setWorking(revokeTarget.sessionKey);
    try {
      await authApi.revokeSession(revokeTarget.sessionKey);
      appToast.success("Đã thu hồi phiên đăng nhập.");
      setRevokeTarget(null);
      await load();
    } catch (caught) {
      appToast.error("Không thể thu hồi phiên", normalizeApiError(caught).message);
    } finally {
      setWorking(null);
    }
  }

  async function revokeOthers() {
    setWorking("others");
    try {
      await authApi.revokeOtherSessions();
      appToast.success("Đã thu hồi tất cả phiên đăng nhập khác.");
      setRevokeOthersOpen(false);
      await load();
    } catch (caught) {
      appToast.error("Không thể thu hồi phiên", normalizeApiError(caught).message);
    } finally {
      setWorking(null);
    }
  }

  return (
    <>
      <div className="space-y-5">
        <Card>
          <CardHeader className="flex flex-row items-start justify-between gap-3">
            <div>
              <CardTitle>Phiên đăng nhập</CardTitle>
              <p className="mt-1 text-[11px] text-muted-foreground">Quản lý các thiết bị đang đăng nhập vào tài khoản Admin hiện tại.</p>
            </div>
            <div className="flex flex-wrap gap-2">
              <Button size="sm" variant="outline" onClick={() => void load()}><RefreshCw size={14} />Làm mới</Button>
              <Button size="sm" variant="outline" disabled={sessions.filter((item) => !item.isCurrent).length === 0} onClick={() => setRevokeOthersOpen(true)}><Trash2 size={14} />Thu hồi phiên khác</Button>
            </div>
          </CardHeader>
          <CardContent>
            {error && !loading ? <ErrorState description={error} onRetry={() => void load()} /> : loading ? (
              <div className="space-y-3">{Array.from({ length: 3 }).map((_, index) => <Skeleton key={index} className="h-[86px] w-full" />)}</div>
            ) : sessions.length === 0 ? (
              <EmptyState title="Không có phiên đăng nhập" description="Không tìm thấy phiên đăng nhập nào của tài khoản hiện tại." />
            ) : (
              <div className="space-y-3">{sessions.map((session) => (
                <div key={session.sessionKey} className="flex flex-wrap items-center justify-between gap-3 rounded-[10px] border p-4">
                  <div className="flex min-w-0 items-start gap-3">
                    <div className="rounded-md bg-muted p-2"><Laptop size={16} /></div>
                    <div className="min-w-0">
                      <div className="flex flex-wrap items-center gap-2"><span className="text-[12px] font-semibold">{session.deviceName || session.deviceType || "Thiết bị không xác định"}</span>{session.isCurrent ? <Badge variant="success">Phiên hiện tại</Badge> : <Badge>{session.status}</Badge>}</div>
                      <div className="mt-1 text-[10px] text-muted-foreground">{[session.browser, session.operatingSystem, session.ipAddress].filter(Boolean).join(" · ") || "Không có thông tin thiết bị"}</div>
                      <div className="mt-1 text-[10px] text-muted-foreground">Hoạt động cuối: {new Date(session.lastActivityAt).toLocaleString("vi-VN")}</div>
                    </div>
                  </div>
                  {!session.isCurrent ? <Button size="sm" variant="dangerGhost" disabled={working === session.sessionKey} onClick={() => setRevokeTarget(session)}><Trash2 size={13} />Thu hồi</Button> : null}
                </div>
              ))}</div>
            )}
          </CardContent>
        </Card>

        <TwoFactorSecurity />

        <Card>
          <CardHeader><CardTitle>Sự kiện bảo mật</CardTitle><p className="mt-1 text-[11px] text-muted-foreground">50 sự kiện gần nhất của tài khoản Admin hiện tại.</p></CardHeader>
          <CardContent>
            {loading ? <div className="space-y-2">{Array.from({ length: 4 }).map((_, index) => <Skeleton key={index} className="h-[74px] w-full" />)}</div> : events.length === 0 ? <EmptyState title="Chưa có sự kiện bảo mật" description="Các hoạt động đăng nhập, đổi mật khẩu, session và 2FA sẽ xuất hiện tại đây." /> : (
              <div className="space-y-2">{events.map((event, index) => (
                <div key={`${event.occurredAt}-${index}`} className="grid gap-2 rounded-md border p-3 md:grid-cols-[220px_170px_1fr]">
                  <div className="flex items-center gap-2 text-[11px] font-medium"><ShieldAlert size={13} />{event.eventType}</div>
                  <div className="text-[10px] text-muted-foreground">{event.ipAddress || "Không xác định"}<br />{new Date(event.occurredAt).toLocaleString("vi-VN")}</div>
                  <div className="min-w-0 text-[10px] text-muted-foreground"><div className="truncate">{event.userAgent || "Không có user-agent"}</div>{event.metadataJson ? <pre className="mt-1 max-h-20 overflow-auto whitespace-pre-wrap rounded bg-muted p-2">{event.metadataJson}</pre> : null}</div>
                </div>
              ))}</div>
            )}
          </CardContent>
        </Card>
      </div>

      <ConfirmDialog open={Boolean(revokeTarget)} title="Thu hồi phiên đăng nhập?" description={revokeTarget ? `Thiết bị “${revokeTarget.deviceName || revokeTarget.deviceType || "không xác định"}” sẽ phải đăng nhập lại.` : ""} confirmLabel="Thu hồi phiên" loading={Boolean(revokeTarget && working === revokeTarget.sessionKey)} onClose={() => setRevokeTarget(null)} onConfirm={revoke} />
      <ConfirmDialog open={revokeOthersOpen} title="Thu hồi tất cả phiên khác?" description="Chỉ phiên hiện tại được giữ lại. Các thiết bị còn lại sẽ phải đăng nhập lại." confirmLabel="Thu hồi tất cả" loading={working === "others"} onClose={() => setRevokeOthersOpen(false)} onConfirm={revokeOthers} />
    </>
  );
}
