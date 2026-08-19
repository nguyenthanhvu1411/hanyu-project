"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { KeyRound, ShieldCheck } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { appToast } from "@/components/ui/toast";
import type { CurrentUserDto } from "@/dto/identity/auth.dto";
import { normalizeApiError } from "@/lib/api/api-error";

import { authApi } from "../auth/auth.api";

function Row({ label, value }: { label: string; value: React.ReactNode }) {
  return <div className="grid gap-1 border-b py-3 md:grid-cols-[180px_1fr]"><div className="text-[10px] font-medium uppercase tracking-wide text-muted-foreground">{label}</div><div className="text-[12px] text-[#333]">{value}</div></div>;
}

export function ProfileOverview() {
  const [user, setUser] = useState<CurrentUserDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let active = true;
    void authApi.currentUser()
      .then((value) => { if (active) setUser(value); })
      .catch((caught) => appToast.error("Không thể tải hồ sơ", normalizeApiError(caught).message))
      .finally(() => { if (active) setLoading(false); });
    return () => { active = false; };
  }, []);

  if (loading) return <Card><CardContent className="py-10 text-center text-[12px] text-muted-foreground">Đang tải hồ sơ...</CardContent></Card>;
  if (!user) return <Card><CardContent className="py-10 text-center text-[12px] text-muted-foreground">Không thể đọc thông tin tài khoản hiện tại.</CardContent></Card>;

  return (
    <div className="grid gap-5 xl:grid-cols-[minmax(0,1fr)_320px]">
      <Card>
        <CardHeader>
          <CardTitle>{user.displayName}</CardTitle>
          <p className="mt-1 text-[11px] text-muted-foreground">Thông tin lấy trực tiếp từ `/auth/me`.</p>
        </CardHeader>
        <CardContent>
          <Row label="Email" value={<div className="flex flex-wrap items-center gap-2"><span>{user.email}</span>{user.emailVerified ? <Badge variant="success">Đã xác minh</Badge> : <Badge variant="warning">Chưa xác minh</Badge>}</div>} />
          <Row label="Public ID" value={<span className="font-mono">{user.publicId ?? "—"}</span>} />
          <Row label="Trạng thái" value={user.status ?? "—"} />
          <Row label="Ngôn ngữ" value={user.locale ?? "—"} />
          <Row label="Roles" value={<div className="flex flex-wrap gap-2">{user.roles.map((role) => <Badge key={role}>{role}</Badge>)}</div>} />
          <Row label="Permissions" value={<div className="flex max-h-44 flex-wrap gap-1.5 overflow-auto">{user.permissions.map((permission) => <Badge key={permission} variant="default">{permission}</Badge>)}</div>} />
        </CardContent>
      </Card>

      <div className="space-y-4">
        <Card>
          <CardHeader><CardTitle>Bảo mật tài khoản</CardTitle></CardHeader>
          <CardContent className="space-y-2">
            <Link href="/ho-so/bao-mat"><Button variant="outline" className="w-full justify-start gap-2"><ShieldCheck size={15} />Sessions & Security Events</Button></Link>
            <Link href="/ho-so/doi-mat-khau"><Button variant="outline" className="w-full justify-start gap-2"><KeyRound size={15} />Đổi mật khẩu</Button></Link>
          </CardContent>
        </Card>
        <Card>
          <CardHeader><CardTitle>Chỉnh sửa hồ sơ</CardTitle></CardHeader>
          <CardContent><p className="text-[11px] leading-5 text-muted-foreground">Backend hiện có endpoint đọc `/auth/me` nhưng chưa có endpoint update profile của chính người dùng, nên Admin Web không hiển thị form lưu giả.</p></CardContent>
        </Card>
      </div>
    </div>
  );
}
