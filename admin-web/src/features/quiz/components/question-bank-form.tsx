"use client";

import { FormEvent, useEffect, useState } from "react";
import { useRouter } from "next/navigation";

import { ErrorState } from "@/components/common/error-state";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";

import { quizApi } from "../quiz.api";
import type { QuestionBankRequest } from "../quiz.types";

interface QuestionBankFormProps {
  bankId?: number;
}

const DEFAULT_VALUES: QuestionBankRequest = {
  code: "",
  nameVi: "",
  descriptionVi: null,
  hskLevelId: null,
};

export function QuestionBankForm({ bankId }: QuestionBankFormProps) {
  const router = useRouter();
  const isEditing = Boolean(bankId);
  const [values, setValues] = useState<QuestionBankRequest>(DEFAULT_VALUES);
  const [loading, setLoading] = useState(isEditing);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    if (!bankId) return;
    let active = true;
    setLoading(true);
    quizApi.listQuestionBanks()
      .then((banks) => {
        if (!active) return;
        const bank = banks.find((item) => item.id === bankId);
        if (!bank) throw new Error("Không tìm thấy ngân hàng câu hỏi.");
        setValues({
          code: bank.code,
          nameVi: bank.nameVi,
          descriptionVi: bank.descriptionVi,
          hskLevelId: bank.hskLevelId,
        });
        setError(null);
      })
      .catch((caught) => {
        if (active) setError(caught instanceof Error ? caught : new Error("Không thể tải ngân hàng câu hỏi."));
      })
      .finally(() => {
        if (active) setLoading(false);
      });
    return () => { active = false; };
  }, [bankId]);

  function update<K extends keyof QuestionBankRequest>(key: K, value: QuestionBankRequest[K]) {
    setValues((current) => ({ ...current, [key]: value }));
  }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!values.code.trim() || !values.nameVi.trim()) {
      appToast.error("Thiếu thông tin", "Mã và tên ngân hàng câu hỏi là bắt buộc.");
      return;
    }

    setSaving(true);
    try {
      const payload: QuestionBankRequest = {
        code: values.code.trim().toUpperCase(),
        nameVi: values.nameVi.trim(),
        descriptionVi: values.descriptionVi?.trim() || null,
        hskLevelId: values.hskLevelId || null,
      };
      const saved = isEditing && bankId
        ? await quizApi.updateQuestionBank(bankId, payload)
        : await quizApi.createQuestionBank(payload);
      appToast.success(isEditing ? "Đã cập nhật ngân hàng câu hỏi." : "Đã tạo ngân hàng câu hỏi.");
      router.push(`/cau-hoi/${saved.id}`);
      router.refresh();
    } catch (caught) {
      appToast.error(isEditing ? "Không thể cập nhật" : "Không thể tạo", normalizeApiError(caught).message);
    } finally {
      setSaving(false);
    }
  }

  if (loading) {
    return <div className="rounded-[11px] border border-[#e8e3dc] bg-white p-6 text-[12px] text-muted-foreground">Đang tải dữ liệu...</div>;
  }

  if (error) {
    return <ErrorState title="Không thể tải ngân hàng câu hỏi" description={error.message} onRetry={() => router.refresh()} />;
  }

  return (
    <form onSubmit={submit} className="space-y-5">
      <Card>
        <CardHeader><CardTitle>Thông tin ngân hàng</CardTitle></CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-4 md:grid-cols-2">
            <label className="space-y-1.5">
              <span className="text-[11px] font-medium">Mã ngân hàng *</span>
              <Input value={values.code} onChange={(event) => update("code", event.target.value)} placeholder="VD: HSK1_CORE" />
            </label>
            <label className="space-y-1.5">
              <span className="text-[11px] font-medium">Tên ngân hàng *</span>
              <Input value={values.nameVi} onChange={(event) => update("nameVi", event.target.value)} placeholder="Ngân hàng câu hỏi HSK 1" />
            </label>
          </div>
          <label className="block space-y-1.5">
            <span className="text-[11px] font-medium">HSK Level ID</span>
            <Input type="number" min={1} value={values.hskLevelId ?? ""} onChange={(event) => update("hskLevelId", event.target.value ? Number(event.target.value) : null)} placeholder="Để trống nếu dùng chung nhiều cấp độ" />
          </label>
          <label className="block space-y-1.5">
            <span className="text-[11px] font-medium">Mô tả</span>
            <Textarea value={values.descriptionVi ?? ""} onChange={(event) => update("descriptionVi", event.target.value || null)} placeholder="Mô tả phạm vi và mục đích sử dụng..." />
          </label>
        </CardContent>
      </Card>
      <div className="flex justify-end gap-2">
        <Button type="button" variant="outline" onClick={() => router.back()}>Hủy</Button>
        <Button type="submit" loading={saving}>{isEditing ? "Lưu thay đổi" : "Tạo ngân hàng"}</Button>
      </div>
    </form>
  );
}
