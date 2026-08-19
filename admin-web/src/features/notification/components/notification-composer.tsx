"use client";

import { FormEvent, useState } from "react";
import { Megaphone, Send } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";

import { notificationApi } from "../notification.api";
import {
  BroadcastNotificationRequest,
  NOTIFICATION_TYPE_LABELS,
  NotificationType,
  SendNotificationRequest,
} from "../notification.types";

interface FormState {
  mode: "single" | "broadcast";
  userId: string;
  userIds: string;
  type: NotificationType;
  title: string;
  message: string;
  actionUrl: string;
  metadataJson: string;
  expiresAt: string;
}

const EMPTY_FORM: FormState = {
  mode: "single",
  userId: "",
  userIds: "",
  type: NotificationType.General,
  title: "",
  message: "",
  actionUrl: "",
  metadataJson: "",
  expiresAt: "",
};

export function NotificationComposer() {
  const [form, setForm] = useState<FormState>(EMPTY_FORM);
  const [saving, setSaving] = useState(false);

  function update<K extends keyof FormState>(key: K, value: FormState[K]) {
    setForm((current) => ({ ...current, [key]: value }));
  }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!form.title.trim() || !form.message.trim()) {
      appToast.error("Thiếu nội dung", "Tiêu đề và nội dung thông báo là bắt buộc.");
      return;
    }
    if (form.mode === "single" && !form.userId.trim()) {
      appToast.error("Thiếu người nhận", "Vui lòng nhập UserId.");
      return;
    }

    setSaving(true);
    try {
      if (form.mode === "single") {
        const payload: SendNotificationRequest = {
          userId: form.userId.trim(),
          type: form.type,
          title: form.title.trim(),
          message: form.message.trim(),
          actionUrl: form.actionUrl.trim() || null,
          metadataJson: form.metadataJson.trim() || null,
          expiresAt: form.expiresAt ? new Date(form.expiresAt).toISOString() : null,
        };
        await notificationApi.send(payload);
        appToast.success("Đã gửi thông báo.");
      } else {
        const parsedUserIds = form.userIds
          .split(/[\n,]/)
          .map((value) => value.trim())
          .filter(Boolean);
        const payload: BroadcastNotificationRequest = {
          type: form.type,
          title: form.title.trim(),
          message: form.message.trim(),
          actionUrl: form.actionUrl.trim() || null,
          metadataJson: form.metadataJson.trim() || null,
          expiresAt: form.expiresAt ? new Date(form.expiresAt).toISOString() : null,
          userIds: parsedUserIds.length ? parsedUserIds : null,
        };
        const result = await notificationApi.broadcast(payload);
        appToast.success(`Đã broadcast tới ${result.sentCount ?? 0} người nhận.`);
      }
      setForm((current) => ({ ...EMPTY_FORM, mode: current.mode }));
    } catch (caught) {
      appToast.error("Không thể gửi thông báo", normalizeApiError(caught).message);
    } finally {
      setSaving(false);
    }
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>Gửi thông báo</CardTitle>
        <p className="mt-1 text-[11px] text-muted-foreground">Gửi trực tiếp cho một User hoặc broadcast bằng backend Notification Admin.</p>
      </CardHeader>
      <CardContent>
        <form onSubmit={submit} className="space-y-4">
          <div className="grid gap-3 md:grid-cols-2">
            <label className="space-y-1">
              <span className="text-[11px] font-medium">Chế độ gửi</span>
              <select className="h-10 w-full rounded-md border border-input bg-background px-3 text-[12px]" value={form.mode} onChange={(event) => update("mode", event.target.value as FormState["mode"])}>
                <option value="single">Một người dùng</option>
                <option value="broadcast">Broadcast</option>
              </select>
            </label>
            <label className="space-y-1">
              <span className="text-[11px] font-medium">Loại thông báo</span>
              <select className="h-10 w-full rounded-md border border-input bg-background px-3 text-[12px]" value={form.type} onChange={(event) => update("type", Number(event.target.value) as NotificationType)}>
                {Object.entries(NOTIFICATION_TYPE_LABELS).map(([value, label]) => <option key={value} value={value}>{label}</option>)}
              </select>
            </label>
          </div>

          {form.mode === "single" ? (
            <label className="block space-y-1"><span className="text-[11px] font-medium">UserId *</span><Input value={form.userId} onChange={(event) => update("userId", event.target.value)} placeholder="UUID người nhận" /></label>
          ) : (
            <label className="block space-y-1"><span className="text-[11px] font-medium">Danh sách UserId</span><Textarea value={form.userIds} onChange={(event) => update("userIds", event.target.value)} placeholder="Mỗi UUID một dòng hoặc phân cách bằng dấu phẩy. Để trống để broadcast toàn bộ theo backend." /></label>
          )}

          <label className="block space-y-1"><span className="text-[11px] font-medium">Tiêu đề *</span><Input value={form.title} onChange={(event) => update("title", event.target.value)} /></label>
          <label className="block space-y-1"><span className="text-[11px] font-medium">Nội dung *</span><Textarea value={form.message} onChange={(event) => update("message", event.target.value)} rows={5} /></label>

          <div className="grid gap-3 md:grid-cols-2">
            <label className="space-y-1"><span className="text-[11px] font-medium">Action URL</span><Input value={form.actionUrl} onChange={(event) => update("actionUrl", event.target.value)} placeholder="/bai-giang/..." /></label>
            <label className="space-y-1"><span className="text-[11px] font-medium">Hết hạn</span><Input type="datetime-local" value={form.expiresAt} onChange={(event) => update("expiresAt", event.target.value)} /></label>
          </div>
          <label className="block space-y-1"><span className="text-[11px] font-medium">Metadata JSON</span><Textarea value={form.metadataJson} onChange={(event) => update("metadataJson", event.target.value)} placeholder='{"source":"admin"}' /></label>

          <Button type="submit" loading={saving} className="gap-2">
            {form.mode === "single" ? <Send size={14} /> : <Megaphone size={14} />}
            {form.mode === "single" ? "Gửi thông báo" : "Broadcast"}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
