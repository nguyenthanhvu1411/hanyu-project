"use client";

import { useCallback, useEffect, useState } from "react";

import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";

import { notificationApi } from "../notification.api";
import type { AdminNotification } from "../notification.types";
import { NOTIFICATION_TYPE_LABELS } from "../notification.types";

interface NotificationDetailProps {
  id: number;
}

function Row({ label, value }: { label: string; value: React.ReactNode }) {
  return <div className="grid gap-1 border-b py-3 md:grid-cols-[180px_1fr]"><div className="text-[10px] font-medium uppercase tracking-wide text-muted-foreground">{label}</div><div className="text-[12px] text-[#333]">{value}</div></div>;
}

export function NotificationDetail({ id }: NotificationDetailProps) {
  const [item, setItem] = useState<AdminNotification | null>(null);
  const [loading, setLoading] = useState(true);

  const load = useCallback(async () => {
    setLoading(true);
    try { setItem(await notificationApi.getById(id)); }
    catch (caught) { appToast.error("Không thể tải thông báo", normalizeApiError(caught).message); setItem(null); }
    finally { setLoading(false); }
  }, [id]);

  useEffect(() => { void load(); }, [load]);

  if (loading) return <Card><CardContent className="py-10 text-center text-[12px] text-muted-foreground">Đang tải...</CardContent></Card>;
  if (!item) return <Card><CardContent className="py-10 text-center text-[12px] text-muted-foreground">Không tìm thấy thông báo.</CardContent></Card>;

  return (
    <Card>
      <CardHeader className="flex flex-row items-start justify-between gap-3">
        <div><CardTitle>{item.title}</CardTitle><p className="mt-1 text-[11px] text-muted-foreground">PublicId: {item.publicId}</p></div>
        <div className="flex gap-2"><Badge>{NOTIFICATION_TYPE_LABELS[item.type] ?? `#${item.type}`}</Badge>{item.isExpired ? <Badge>Hết hạn</Badge> : item.isRead ? <Badge variant="success">Đã đọc</Badge> : <Badge variant="warning">Chưa đọc</Badge>}</div>
      </CardHeader>
      <CardContent>
        <Row label="UserId" value={<span className="font-mono">{item.userId}</span>} />
        <Row label="Nội dung" value={<p className="whitespace-pre-wrap leading-5">{item.message}</p>} />
        <Row label="Action URL" value={item.actionUrl || "—"} />
        <Row label="Ngày gửi" value={new Date(item.createdAt).toLocaleString("vi-VN")} />
        <Row label="Đã đọc lúc" value={item.readAt ? new Date(item.readAt).toLocaleString("vi-VN") : "—"} />
        <Row label="Hết hạn lúc" value={item.expiresAt ? new Date(item.expiresAt).toLocaleString("vi-VN") : "—"} />
        <Row label="Metadata" value={item.metadataJson ? <pre className="overflow-auto rounded-md bg-muted p-3 text-[10px]">{item.metadataJson}</pre> : "—"} />
      </CardContent>
    </Card>
  );
}
