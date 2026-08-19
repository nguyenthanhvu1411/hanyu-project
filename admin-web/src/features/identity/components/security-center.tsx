"use client";

import { useCallback, useEffect, useState } from "react";
import { Laptop, RefreshCw, ShieldAlert, Trash2 } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";

import { authApi } from "../auth/auth.api";
import type { AuthSession, SecurityEvent } from "../auth/auth.types";

export function SecurityCenter() {
  const [sessions, setSessions] = useState<AuthSession[]>([]);
  const [events, setEvents] = useState<SecurityEvent[]>([]);
  const [loading, setLoading] = useState(true);
  const [working, setWorking] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [sessionData, eventData] = await Promise.all([
        authApi.sessions(),
        authApi.securityEvents(50),
      ]);
      setSessions(sessionData);
      setEvents(eventData);
    } catch (caught) {
      appToast.error("Không thể tải dữ liệu bảo mật", normalizeApiError(caught).message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { void load(); }, [load]);

  async function revoke(session: AuthSession) {
    if (session.isCurrent || !window.confirm("Thu hồi phiên đăng nhập này?")) return;
    setWorking(session.sessionKey);
    try {
      await authApi.revokeSession(session.sessionKey);
      appToast.success("Đã thu hồi phiên đăng nhập.");
      await load();
    } catch (caught) {
      appToast.error("Không thể thu hồi phiên", normalizeApiError(caught).message);
    } finally { setWorking(null); }
  }

  async function revokeOthers() {
    if (!window.confirm("Thu hồi tất cả phiên đăng nhập khác và chỉ giữ phiên hiện tại?")) return;
    setWorking("others");
    try {
      await authApi.revokeOtherSessions();
      appToast.success("Đã thu hồi các phiên khác.");
      await load();
    } catch (caught) {
      appToast.error("Không thể thu hồi phiên", normalizeApiError(caught).message);
    } finally { setWorking(null); }
  }

  return (
    <div className="space-y-5">
      <Card>
        <CardHeader className="flex flex-row items-start justify-between gap-3">
          <div><CardTitle>Phiên đăng nhập</CardTitle><p className="mt-1 text-[11px] text-muted-foreground">Quản lý các thiết bị đang đăng nhập vào tài khoản Admin hiện tại.</p></div>
          <div className="flex gap-2"><Button size="sm" variant="outline" onClick={() => void load()}><RefreshCw size={14} />Làm mới</Button><Button size="sm" variant="outline" disabled={working === "others"} onClick={() => void revokeOthers()}><Trash2 size={14} />Thu hồi phiên khác</Button></div>
        </CardHeader>
        <CardContent className="space-y-3">
          {loading ? <div className="py-6 text-center text-[12px] text-muted-foreground">Đang tải sessions...</div> : null}
          {!loading && sessions.length === 0 ? <div className="rounded-md border border-dashed p-6 text-center text-[12px] text-muted-foreground">Không có session.</div> : null}
          {sessions.map((session) => (
            <div key={session.sessionKey} className="flex flex-wrap items-center justify-between gap-3 rounded-[10px] border p-4">
              <div className="flex min-w-0 items-start gap-3">
                <div className="rounded-md bg-muted p-2"><Laptop size={16} /></div>
                <div className="min-w-0">
                  <div className="flex flex-wrap items-center gap-2"><span className="text-[12px] font-semibold">{session.deviceName || session.deviceType || "Thiết bị không xác định"}</span>{session.isCurrent ? <Badge variant="success">Phiên hiện tại</Badge> : <Badge>{session.status}</Badge>}</div>
                  <div className="mt-1 text-[10px] text-muted-foreground">{[session.browser, session.operatingSystem, session.ipAddress].filter(Boolean).join(" · ") || "Không có thông tin thiết bị"}</div>
                  <div className="mt-1 text-[10px] text-muted-foreground">Hoạt động cuối: {new Date(session.lastActivityAt).toLocaleString("vi-VN")}</div>
                </div>
              </div>
              {!session.isCurrent ? <Button size="sm" variant="outline" disabled={working === session.sessionKey} onClick={() => void revoke(session)}><Trash2 size={13} />Thu hồi</Button> : null}
            </div>
          ))}
        </CardContent>
      </Card>

      <Card>
        <CardHeader><CardTitle>Sự kiện bảo mật</CardTitle><p className="mt-1 text-[11px] text-muted-foreground">50 sự kiện gần nhất từ `/auth/security-events`.</p></CardHeader>
        <CardContent className="space-y-2">
          {events.map((event, index) => (
            <div key={`${event.occurredAt}-${index}`} className="grid gap-2 rounded-md border p-3 md:grid-cols-[220px_160px_1fr]">
              <div className="flex items-center gap-2 text-[11px] font-medium"><ShieldAlert size={13} />{event.eventType}</div>
              <div className="text-[10px] text-muted-foreground">{event.ipAddress || "—"}<br />{new Date(event.occurredAt).toLocaleString("vi-VN")}</div>
              <div className="min-w-0 text-[10px] text-muted-foreground"><div className="truncate">{event.userAgent || "—"}</div>{event.metadataJson ? <pre className="mt-1 max-h-20 overflow-auto whitespace-pre-wrap rounded bg-muted p-2">{event.metadataJson}</pre> : null}</div>
            </div>
          ))}
          {!loading && events.length === 0 ? <div className="rounded-md border border-dashed p-6 text-center text-[12px] text-muted-foreground">Chưa có sự kiện bảo mật.</div> : null}
        </CardContent>
      </Card>

      <Card>
        <CardHeader><CardTitle>Xác thực hai lớp (2FA)</CardTitle></CardHeader>
        <CardContent><p className="text-[11px] leading-5 text-muted-foreground">Backend đã có setup/enable/disable/recovery-code/regenerate-key. UI 2FA cần màn nhập mã xác thực và hiển thị secret/recovery codes an toàn; phần này được giữ ở vòng tiếp theo thay vì hiển thị secret bằng UI tạm.</p></CardContent>
      </Card>
    </div>
  );
}
