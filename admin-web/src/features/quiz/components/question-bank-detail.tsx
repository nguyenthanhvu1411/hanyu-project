"use client";

import Link from "next/link";
import { FormEvent, useCallback, useEffect, useState } from "react";

import { ErrorState } from "@/components/common/error-state";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";

import { quizApi } from "../quiz.api";
import type { AdminQuestionBank } from "../quiz.types";

export function QuestionBankDetail({ bankId }: { bankId: number }) {
  const [bank, setBank] = useState<AdminQuestionBank | null>(null);
  const [questionId, setQuestionId] = useState("");
  const [loading, setLoading] = useState(true);
  const [working, setWorking] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const banks = await quizApi.listQuestionBanks();
      const found = banks.find((item) => item.id === bankId);
      if (!found) throw new Error("Không tìm thấy ngân hàng câu hỏi.");
      setBank(found);
    } catch (caught) {
      setError(caught instanceof Error ? caught : new Error("Không thể tải ngân hàng câu hỏi."));
    } finally {
      setLoading(false);
    }
  }, [bankId]);

  useEffect(() => { void load(); }, [load]);

  async function toggle() {
    if (!bank || working) return;
    setWorking(true);
    try {
      if (bank.isActive) {
        await quizApi.deactivateQuestionBank(bank.id);
        appToast.success("Đã tạm dừng ngân hàng câu hỏi.");
      } else {
        await quizApi.activateQuestionBank(bank.id);
        appToast.success("Đã kích hoạt ngân hàng câu hỏi.");
      }
      await load();
    } catch (caught) {
      appToast.error("Không thể cập nhật trạng thái", normalizeApiError(caught).message);
    } finally {
      setWorking(false);
    }
  }

  async function attach(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const parsed = Number(questionId);
    if (!Number.isSafeInteger(parsed) || parsed <= 0) {
      appToast.error("Question ID không hợp lệ", "Vui lòng nhập ID câu hỏi hợp lệ.");
      return;
    }
    setWorking(true);
    try {
      await quizApi.addQuestionToBank(bankId, parsed);
      appToast.success("Đã gắn câu hỏi vào ngân hàng.");
      setQuestionId("");
      await load();
    } catch (caught) {
      appToast.error("Không thể gắn câu hỏi", normalizeApiError(caught).message);
    } finally {
      setWorking(false);
    }
  }

  if (loading) {
    return <div className="rounded-[11px] border border-[#e8e3dc] bg-white p-6 text-[12px] text-muted-foreground">Đang tải dữ liệu...</div>;
  }
  if (error || !bank) {
    return <ErrorState title="Không thể tải ngân hàng câu hỏi" description={error?.message ?? "Không tìm thấy dữ liệu."} onRetry={() => void load()} />;
  }

  return (
    <div className="space-y-5">
      <Card>
        <CardHeader className="flex flex-row items-start justify-between gap-3">
          <div>
            <CardTitle>{bank.nameVi}</CardTitle>
            <p className="mt-1 text-[11px] text-muted-foreground">{bank.code} · PublicId {bank.publicId}</p>
          </div>
          <Badge variant={bank.isActive ? "success" : "default"}>{bank.isActive ? "Đang dùng" : "Tạm dừng"}</Badge>
        </CardHeader>
        <CardContent className="space-y-4">
          <p className="text-[12px] leading-6 text-[#555]">{bank.descriptionVi || "Chưa có mô tả."}</p>
          <div className="grid gap-3 sm:grid-cols-2">
            <div className="rounded-md border p-3"><div className="text-[10px] text-muted-foreground">HSK Level</div><div className="mt-1 text-[12px] font-medium">{bank.hskLevelId ? `#${bank.hskLevelId}` : "Dùng chung"}</div></div>
            <div className="rounded-md border p-3"><div className="text-[10px] text-muted-foreground">Số câu hỏi</div><div className="mt-1 text-[12px] font-medium">{bank.questionCount}</div></div>
          </div>
          <div className="flex flex-wrap gap-2">
            <Link href={`/cau-hoi/${bank.id}/chinh-sua`}><Button variant="outline">Chỉnh sửa</Button></Link>
            <Button disabled={working} onClick={() => void toggle()}>{bank.isActive ? "Tạm dừng" : "Kích hoạt"}</Button>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader><CardTitle>Gắn câu hỏi vào ngân hàng</CardTitle></CardHeader>
        <CardContent>
          <form onSubmit={attach} className="flex max-w-xl gap-2">
            <Input type="number" min={1} value={questionId} onChange={(event) => setQuestionId(event.target.value)} placeholder="Question ID" />
            <Button type="submit" loading={working}>Gắn câu hỏi</Button>
          </form>
          <p className="mt-2 text-[10px] text-muted-foreground">Backend hiện chưa cung cấp endpoint đọc danh sách membership của bank, nên màn này chỉ hiển thị QuestionCount và thao tác attach an toàn theo API hiện có.</p>
        </CardContent>
      </Card>
    </div>
  );
}
