"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { Save } from "lucide-react";

import { ErrorState } from "@/components/common/error-state";
import { Alert } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";

import { profileApi, type UpdateUserProfileRequest, type UserProfile } from "../profile.api";

const hskOptions = Array.from({ length: 9 }, (_, index) => ({ value: String(index + 1), label: `HSK ${index + 1}` }));
const languageOptions = [
  { value: "vi", label: "Tiếng Việt" },
  { value: "en", label: "English" },
];

export function ProfileEditForm() {
  const [profile, setProfile] = useState<UserProfile | null>(null);
  const [form, setForm] = useState<UpdateUserProfileRequest | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await profileApi.get();
      setProfile(data);
      setForm({
        displayName: data.displayName,
        avatarUrl: data.avatarUrl,
        currentHskLevel: data.currentHskLevel,
        dailyGoalMinutes: data.dailyGoalMinutes,
        timezone: data.timezone,
        uiLanguage: data.uiLanguage,
      });
    } catch (caught) {
      setError(normalizeApiError(caught).message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { void load(); }, [load]);

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!form) return;
    if (!form.displayName.trim()) {
      appToast.error("Thiếu tên hiển thị", "Vui lòng nhập tên hiển thị.");
      return;
    }
    setSaving(true);
    try {
      const updated = await profileApi.update({
        ...form,
        displayName: form.displayName.trim(),
        avatarUrl: form.avatarUrl?.trim() || null,
        timezone: form.timezone.trim(),
        uiLanguage: form.uiLanguage.trim(),
      });
      setProfile(updated);
      setForm({
        displayName: updated.displayName,
        avatarUrl: updated.avatarUrl,
        currentHskLevel: updated.currentHskLevel,
        dailyGoalMinutes: updated.dailyGoalMinutes,
        timezone: updated.timezone,
        uiLanguage: updated.uiLanguage,
      });
      appToast.success("Đã cập nhật hồ sơ.");
    } catch (caught) {
      appToast.error("Không thể cập nhật hồ sơ", normalizeApiError(caught).message);
    } finally {
      setSaving(false);
    }
  }

  if (loading) {
    return <Card><CardContent className="space-y-4 p-5"><Skeleton className="h-10 w-full" /><Skeleton className="h-10 w-full" /><Skeleton className="h-10 w-full" /><Skeleton className="h-10 w-full" /></CardContent></Card>;
  }
  if (error || !form || !profile) return <ErrorState description={error ?? "Không thể tải hồ sơ."} onRetry={() => void load()} />;

  return (
    <div className="grid gap-5 xl:grid-cols-[minmax(0,720px)_minmax(280px,1fr)]">
      <Card>
        <CardHeader><CardTitle>Thông tin cá nhân</CardTitle></CardHeader>
        <CardContent>
          <form className="space-y-4" onSubmit={submit}>
            <div className="grid gap-4 sm:grid-cols-2">
              <label className="space-y-1.5"><span className="text-[12px] font-medium">Tên hiển thị *</span><Input value={form.displayName} onChange={(event) => setForm((current) => current ? { ...current, displayName: event.target.value } : current)} /></label>
              <label className="space-y-1.5"><span className="text-[12px] font-medium">Email</span><Input value={profile.email} disabled /></label>
            </div>
            <label className="block space-y-1.5"><span className="text-[12px] font-medium">Avatar URL</span><Input value={form.avatarUrl ?? ""} onChange={(event) => setForm((current) => current ? { ...current, avatarUrl: event.target.value || null } : current)} placeholder="https://..." /></label>
            <div className="grid gap-4 sm:grid-cols-2">
              <label className="space-y-1.5"><span className="text-[12px] font-medium">Trình độ hiện tại</span><Select value={String(form.currentHskLevel)} options={hskOptions} onValueChange={(value) => setForm((current) => current ? { ...current, currentHskLevel: Number(value) } : current)} /></label>
              <label className="space-y-1.5"><span className="text-[12px] font-medium">Mục tiêu phút/ngày</span><Input type="number" min={1} max={1440} value={form.dailyGoalMinutes} onChange={(event) => setForm((current) => current ? { ...current, dailyGoalMinutes: Number(event.target.value) } : current)} /></label>
            </div>
            <div className="grid gap-4 sm:grid-cols-2">
              <label className="space-y-1.5"><span className="text-[12px] font-medium">Múi giờ</span><Input value={form.timezone} onChange={(event) => setForm((current) => current ? { ...current, timezone: event.target.value } : current)} placeholder="Asia/Ho_Chi_Minh" /></label>
              <label className="space-y-1.5"><span className="text-[12px] font-medium">Ngôn ngữ giao diện</span><Select value={form.uiLanguage} options={languageOptions} onValueChange={(value) => setForm((current) => current ? { ...current, uiLanguage: value } : current)} /></label>
            </div>
            <div className="flex justify-end"><Button type="submit" loading={saving}><Save size={14} className="mr-2" />Lưu hồ sơ</Button></div>
          </form>
        </CardContent>
      </Card>

      <div className="space-y-4">
        <Alert variant="info" title="Tài khoản">
          Username và email được quản lý bởi luồng Account/Security riêng. Trang này chỉ cập nhật hồ sơ học tập và thông tin hiển thị.
        </Alert>
        <Card><CardHeader><CardTitle>Trạng thái hồ sơ</CardTitle></CardHeader><CardContent className="space-y-2 text-[12px]"><div className="flex justify-between"><span className="text-[#888]">Email</span><span>{profile.emailConfirmed ? "Đã xác minh" : "Chưa xác minh"}</span></div><div className="flex justify-between"><span className="text-[#888]">Onboarding</span><span>{profile.onboardingCompleted ? "Hoàn thành" : "Chưa hoàn thành"}</span></div><div className="flex justify-between"><span className="text-[#888]">Cập nhật gần nhất</span><span>{new Date(profile.updatedAt).toLocaleString("vi-VN")}</span></div></CardContent></Card>
      </div>
    </div>
  );
}