"use client";

import Link from "next/link";
import { useCallback, useEffect, useMemo, useState } from "react";
import { RefreshCw, Trash2 } from "lucide-react";

import { QuizSelector } from "@/components/admin/entity-selectors";
import { EmptyState } from "@/components/common/empty-state";
import { ErrorState } from "@/components/common/error-state";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";
import { Select } from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";

import { quizApi } from "../quiz.api";
import type { AdminQuestionBank, AdminQuizQuestion } from "../quiz.types";

export function QuestionBankDetail({ bankId }: { bankId: number }) {
  const [bank, setBank] = useState<AdminQuestionBank | null>(null);
  const [members, setMembers] = useState<AdminQuizQuestion[]>([]);
  const [quizId, setQuizId] = useState("");
  const [questions, setQuestions] = useState<AdminQuizQuestion[]>([]);
  const [questionId, setQuestionId] = useState("");
  const [loading, setLoading] = useState(true);
  const [questionsLoading, setQuestionsLoading] = useState(false);
  const [working, setWorking] = useState(false);
  const [removeTarget, setRemoveTarget] = useState<AdminQuizQuestion | null>(null);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true); setError(null);
    try {
      const [banks, membership] = await Promise.all([
        quizApi.listQuestionBanks(),
        quizApi.listQuestionBankQuestions(bankId),
      ]);
      const found = banks.find((item) => item.id === bankId);
      if (!found) throw new Error("Không tìm thấy ngân hàng câu hỏi.");
      setBank(found);
      setMembers(membership);
    } catch (caught) {
      setError(normalizeApiError(caught).message);
    } finally { setLoading(false); }
  }, [bankId]);

  useEffect(() => { void load(); }, [load]);

  useEffect(() => {
    if (!quizId) { setQuestions([]); setQuestionId(""); return; }
    let active = true;
    setQuestionsLoading(true);
    void quizApi.listQuestions(Number(quizId))
      .then((items) => { if (active) setQuestions(items); })
      .catch((caught) => appToast.error("Không thể tải câu hỏi", normalizeApiError(caught).message))
      .finally(() => { if (active) setQuestionsLoading(false); });
    return () => { active = false; };
  }, [quizId]);

  const questionOptions = useMemo(() => questions
    .filter((question) => !members.some((member) => member.id === question.id))
    .map((question) => ({
      value: String(question.id),
      label: question.prompt,
      description: `Loại ${question.questionType} · ${question.points} điểm`,
    })), [members, questions]);

  async function toggle() {
    if (!bank || working) return;
    setWorking(true);
    try {
      if (bank.isActive) await quizApi.deactivateQuestionBank(bank.id);
      else await quizApi.activateQuestionBank(bank.id);
      appToast.success(bank.isActive ? "Đã tạm dừng ngân hàng câu hỏi." : "Đã kích hoạt ngân hàng câu hỏi.");
      await load();
    } catch (caught) { appToast.error("Không thể cập nhật trạng thái", normalizeApiError(caught).message); }
    finally { setWorking(false); }
  }

  async function attach() {
    if (!questionId) { appToast.error("Chưa chọn câu hỏi", "Hãy chọn bài kiểm tra và câu hỏi cần thêm."); return; }
    setWorking(true);
    try {
      await quizApi.addQuestionToBank(bankId, Number(questionId));
      appToast.success("Đã gắn câu hỏi vào ngân hàng.");
      setQuestionId("");
      await load();
    } catch (caught) { appToast.error("Không thể gắn câu hỏi", normalizeApiError(caught).message); }
    finally { setWorking(false); }
  }

  async function remove() {
    if (!removeTarget) return;
    setWorking(true);
    try {
      await quizApi.removeQuestionFromBank(bankId, removeTarget.id);
      appToast.success("Đã gỡ câu hỏi khỏi ngân hàng.");
      setRemoveTarget(null);
      await load();
    } catch (caught) { appToast.error("Không thể gỡ câu hỏi", normalizeApiError(caught).message); }
    finally { setWorking(false); }
  }

  if (loading) return <div className="space-y-3">{Array.from({ length: 4 }).map((_, index) => <Skeleton key={index} className="h-24 w-full" />)}</div>;
  if (error || !bank) return <ErrorState title="Không thể tải ngân hàng câu hỏi" description={error ?? "Không tìm thấy dữ liệu."} onRetry={() => void load()} />;

  return (
    <>
      <div className="space-y-5">
        <Card>
          <CardHeader className="flex flex-row items-start justify-between gap-3">
            <div><CardTitle>{bank.nameVi}</CardTitle><p className="mt-1 text-[11px] text-muted-foreground">Mã ngân hàng: {bank.code}</p></div>
            <Badge variant={bank.isActive ? "success" : "default"}>{bank.isActive ? "Đang dùng" : "Tạm dừng"}</Badge>
          </CardHeader>
          <CardContent className="space-y-4">
            <p className="text-[12px] leading-6 text-[#555]">{bank.descriptionVi || "Chưa có mô tả."}</p>
            <div className="grid gap-3 sm:grid-cols-2"><div className="rounded-md border p-3"><div className="text-[10px] text-muted-foreground">Phạm vi HSK</div><div className="mt-1 text-[12px] font-medium">{bank.hskLevelId ? "Có giới hạn theo cấp HSK" : "Dùng chung mọi cấp"}</div></div><div className="rounded-md border p-3"><div className="text-[10px] text-muted-foreground">Số câu hỏi</div><div className="mt-1 text-[12px] font-medium">{members.length}</div></div></div>
            <div className="flex flex-wrap gap-2"><Link href={`/cau-hoi/${bank.id}/chinh-sua`}><Button variant="outline">Chỉnh sửa</Button></Link><Button disabled={working} onClick={() => void toggle()}>{bank.isActive ? "Tạm dừng" : "Kích hoạt"}</Button><Button variant="outline" onClick={() => void load()}><RefreshCw size={14} />Làm mới</Button></div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader><CardTitle>Thêm câu hỏi</CardTitle><p className="mt-1 text-[11px] text-muted-foreground">Chọn bài kiểm tra, sau đó chọn câu hỏi bằng nội dung prompt. Không cần nhập Question ID.</p></CardHeader>
          <CardContent className="grid gap-3 md:grid-cols-[1fr_1.4fr_auto] md:items-end">
            <label className="space-y-1"><span className="text-[11px] font-medium">Bài kiểm tra</span><QuizSelector value={quizId} onValueChange={(value) => { setQuizId(value); setQuestionId(""); }} /></label>
            <label className="space-y-1"><span className="text-[11px] font-medium">Câu hỏi</span><Select value={questionId} options={questionOptions} searchable disabled={!quizId || questionsLoading} placeholder={questionsLoading ? "Đang tải câu hỏi..." : "Chọn câu hỏi"} onValueChange={setQuestionId} /></label>
            <Button disabled={!questionId} loading={working} onClick={() => void attach()}>Thêm vào bank</Button>
          </CardContent>
        </Card>

        <Card>
          <CardHeader><CardTitle>Câu hỏi trong ngân hàng</CardTitle></CardHeader>
          <CardContent>
            {members.length === 0 ? <EmptyState title="Ngân hàng chưa có câu hỏi" description="Chọn bài kiểm tra và thêm các câu hỏi phù hợp ở phía trên." /> : <div className="space-y-2">{members.map((question) => <div key={question.id} className="flex items-center justify-between gap-3 rounded-[9px] border p-3"><div className="min-w-0"><div className="line-clamp-2 text-[12px] font-medium">{question.prompt}</div><div className="mt-1 text-[10px] text-muted-foreground">{question.points} điểm · {question.isRequired ? "Bắt buộc" : "Tùy chọn"}</div></div><Button size="icon" variant="dangerGhost" aria-label="Gỡ câu hỏi" onClick={() => setRemoveTarget(question)}><Trash2 size={14} /></Button></div>)}</div>}
          </CardContent>
        </Card>
      </div>

      <ConfirmDialog open={Boolean(removeTarget)} title="Gỡ câu hỏi khỏi ngân hàng?" description={removeTarget ? `Câu “${removeTarget.prompt}” sẽ không còn thuộc ngân hàng này.` : ""} confirmLabel="Gỡ câu hỏi" loading={working} onClose={() => setRemoveTarget(null)} onConfirm={remove} />
    </>
  );
}
