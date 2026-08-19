"use client";

import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";
import { Pencil, Plus, RefreshCw, Trash2, X } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";

import { quizApi } from "../quiz.api";
import {
  AdminQuizQuestion,
  ContentStatus,
  QUIZ_QUESTION_TYPE_LABELS,
  QUIZ_STATUS_LABELS,
  QuizQuestionRequest,
  QuizQuestionType,
} from "../quiz.types";

interface QuizQuestionManagerProps {
  quizId: number;
}

const EMPTY_FORM: QuizQuestionRequest = {
  questionType: QuizQuestionType.MultipleChoice,
  prompt: "",
  promptPinyin: null,
  correctAnswerText: null,
  explanationVi: null,
  hintVi: null,
  points: 1,
  sortOrder: 0,
  timeLimitSeconds: null,
  isRequired: true,
  vocabularyId: null,
};

function fromQuestion(question: AdminQuizQuestion): QuizQuestionRequest {
  return {
    questionType: question.questionType,
    prompt: question.prompt,
    promptPinyin: question.promptPinyin,
    correctAnswerText: question.correctAnswerText,
    explanationVi: question.explanationVi,
    hintVi: question.hintVi,
    points: question.points,
    sortOrder: question.sortOrder,
    timeLimitSeconds: question.timeLimitSeconds,
    isRequired: question.isRequired,
    vocabularyId: question.vocabularyId,
  };
}

function statusVariant(status: ContentStatus): "default" | "success" | "warning" | "info" {
  if (status === ContentStatus.Published) return "success";
  if (status === ContentStatus.Review) return "warning";
  if (status === ContentStatus.Approved) return "info";
  return "default";
}

export function QuizQuestionManager({ quizId }: QuizQuestionManagerProps) {
  const [items, setItems] = useState<AdminQuizQuestion[]>([]);
  const [form, setForm] = useState<QuizQuestionRequest>(EMPTY_FORM);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [workingId, setWorkingId] = useState<number | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const result = await quizApi.listQuestions(quizId);
      setItems([...result].sort((a, b) => a.sortOrder - b.sortOrder || a.id - b.id));
    } catch (caught) {
      appToast.error("Không thể tải câu hỏi", normalizeApiError(caught).message);
      setItems([]);
    } finally {
      setLoading(false);
    }
  }, [quizId]);

  useEffect(() => { void load(); }, [load]);

  const nextSortOrder = useMemo(
    () => items.length === 0 ? 0 : Math.max(...items.map((item) => item.sortOrder)) + 1,
    [items],
  );

  function resetForm() {
    setEditingId(null);
    setForm({ ...EMPTY_FORM, sortOrder: nextSortOrder });
  }

  function update<K extends keyof QuizQuestionRequest>(key: K, value: QuizQuestionRequest[K]) {
    setForm((current) => ({ ...current, [key]: value }));
  }

  function edit(question: AdminQuizQuestion) {
    setEditingId(question.id);
    setForm(fromQuestion(question));
    window.scrollTo({ top: 0, behavior: "smooth" });
  }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!form.prompt.trim()) {
      appToast.error("Thiếu câu hỏi", "Vui lòng nhập nội dung câu hỏi.");
      return;
    }
    if (form.points <= 0) {
      appToast.error("Điểm không hợp lệ", "Điểm phải lớn hơn 0.");
      return;
    }

    setSaving(true);
    try {
      const payload: QuizQuestionRequest = {
        ...form,
        prompt: form.prompt.trim(),
        promptPinyin: form.promptPinyin?.trim() || null,
        correctAnswerText: form.correctAnswerText?.trim() || null,
        explanationVi: form.explanationVi?.trim() || null,
        hintVi: form.hintVi?.trim() || null,
        vocabularyId: form.vocabularyId || null,
        timeLimitSeconds: form.timeLimitSeconds || null,
      };

      if (editingId) {
        await quizApi.updateQuestion(quizId, editingId, payload);
        appToast.success("Đã cập nhật câu hỏi.");
      } else {
        await quizApi.createQuestion(quizId, payload);
        appToast.success("Đã thêm câu hỏi.");
      }
      resetForm();
      await load();
    } catch (caught) {
      appToast.error(editingId ? "Không thể cập nhật câu hỏi" : "Không thể thêm câu hỏi", normalizeApiError(caught).message);
    } finally {
      setSaving(false);
    }
  }

  async function remove(question: AdminQuizQuestion) {
    if (!window.confirm(`Xóa câu hỏi #${question.sortOrder + 1}?`)) return;
    setWorkingId(question.id);
    try {
      await quizApi.deleteQuestion(quizId, question.id);
      appToast.success("Đã xóa câu hỏi.");
      if (editingId === question.id) resetForm();
      await load();
    } catch (caught) {
      appToast.error("Không thể xóa câu hỏi", normalizeApiError(caught).message);
    } finally {
      setWorkingId(null);
    }
  }

  async function workflow(question: AdminQuizQuestion, action: () => Promise<void>, message: string) {
    setWorkingId(question.id);
    try {
      await action();
      appToast.success(message);
      await load();
    } catch (caught) {
      appToast.error("Không thể cập nhật trạng thái", normalizeApiError(caught).message);
    } finally {
      setWorkingId(null);
    }
  }

  return (
    <div className="grid gap-5 xl:grid-cols-[420px_minmax(0,1fr)]">
      <Card className="h-fit xl:sticky xl:top-4">
        <CardHeader className="flex flex-row items-center justify-between gap-3">
          <CardTitle>{editingId ? "Sửa câu hỏi" : "Thêm câu hỏi"}</CardTitle>
          {editingId ? <Button type="button" size="sm" variant="outline" onClick={resetForm}><X size={14} /> Hủy sửa</Button> : null}
        </CardHeader>
        <CardContent>
          <form onSubmit={submit} className="space-y-3">
            <label className="block space-y-1">
              <span className="text-[11px] font-medium">Loại câu hỏi</span>
              <select className="h-10 w-full rounded-md border border-input bg-background px-3 text-[12px]" value={form.questionType} onChange={(event) => update("questionType", Number(event.target.value) as QuizQuestionType)}>
                {Object.entries(QUIZ_QUESTION_TYPE_LABELS).map(([value, label]) => <option key={value} value={value}>{label}</option>)}
              </select>
            </label>
            <label className="block space-y-1">
              <span className="text-[11px] font-medium">Nội dung câu hỏi *</span>
              <Textarea value={form.prompt} onChange={(event) => update("prompt", event.target.value)} placeholder="Nhập nội dung câu hỏi..." />
            </label>
            <label className="block space-y-1">
              <span className="text-[11px] font-medium">Pinyin</span>
              <Input value={form.promptPinyin ?? ""} onChange={(event) => update("promptPinyin", event.target.value || null)} placeholder="Không bắt buộc" />
            </label>
            <label className="block space-y-1">
              <span className="text-[11px] font-medium">Đáp án đúng dạng text</span>
              <Input value={form.correctAnswerText ?? ""} onChange={(event) => update("correctAnswerText", event.target.value || null)} placeholder="Dùng cho FillBlank/TrueFalse..." />
            </label>
            <div className="grid grid-cols-3 gap-2">
              <label className="space-y-1"><span className="text-[10px] font-medium">Điểm</span><Input type="number" min={0.01} step="0.01" value={form.points} onChange={(event) => update("points", Number(event.target.value))} /></label>
              <label className="space-y-1"><span className="text-[10px] font-medium">Thứ tự</span><Input type="number" min={0} value={form.sortOrder} onChange={(event) => update("sortOrder", Number(event.target.value))} /></label>
              <label className="space-y-1"><span className="text-[10px] font-medium">Giây</span><Input type="number" min={1} value={form.timeLimitSeconds ?? ""} onChange={(event) => update("timeLimitSeconds", event.target.value ? Number(event.target.value) : null)} /></label>
            </div>
            <label className="block space-y-1"><span className="text-[11px] font-medium">Vocabulary ID</span><Input type="number" min={1} value={form.vocabularyId ?? ""} onChange={(event) => update("vocabularyId", event.target.value ? Number(event.target.value) : null)} placeholder="Không bắt buộc" /></label>
            <label className="block space-y-1"><span className="text-[11px] font-medium">Gợi ý</span><Textarea value={form.hintVi ?? ""} onChange={(event) => update("hintVi", event.target.value || null)} /></label>
            <label className="block space-y-1"><span className="text-[11px] font-medium">Giải thích</span><Textarea value={form.explanationVi ?? ""} onChange={(event) => update("explanationVi", event.target.value || null)} /></label>
            <label className="flex items-center gap-2 text-[11px]"><input type="checkbox" checked={form.isRequired} onChange={(event) => update("isRequired", event.target.checked)} /> Bắt buộc</label>
            <Button type="submit" className="w-full gap-2" loading={saving}><Plus size={14} />{editingId ? "Lưu câu hỏi" : "Thêm câu hỏi"}</Button>
          </form>
        </CardContent>
      </Card>

      <Card>
        <CardHeader className="flex flex-row items-center justify-between gap-3">
          <div><CardTitle>Danh sách câu hỏi</CardTitle><p className="mt-1 text-[11px] text-muted-foreground">{items.length} câu hỏi trong bài kiểm tra.</p></div>
          <Button variant="outline" size="sm" className="gap-2" onClick={() => void load()}><RefreshCw size={14} /> Làm mới</Button>
        </CardHeader>
        <CardContent className="space-y-3">
          {loading ? <div className="py-10 text-center text-[12px] text-muted-foreground">Đang tải câu hỏi...</div> : null}
          {!loading && items.length === 0 ? <div className="rounded-md border border-dashed p-8 text-center text-[12px] text-muted-foreground">Chưa có câu hỏi. Tạo câu hỏi đầu tiên ở biểu mẫu bên trái.</div> : null}
          {items.map((question, index) => (
            <div key={question.id} className="rounded-[10px] border border-[#e9e4dc] bg-white p-4">
              <div className="flex flex-wrap items-start justify-between gap-3">
                <div className="min-w-0 flex-1">
                  <div className="flex flex-wrap items-center gap-2">
                    <span className="text-[10px] font-semibold text-muted-foreground">#{index + 1}</span>
                    <Badge>{QUIZ_QUESTION_TYPE_LABELS[question.questionType]}</Badge>
                    <Badge variant={statusVariant(question.status)}>{QUIZ_STATUS_LABELS[question.status]}</Badge>
                    <span className="text-[10px] text-muted-foreground">{question.points} điểm</span>
                  </div>
                  <p className="mt-2 whitespace-pre-wrap text-[12px] font-medium leading-5 text-[#333]">{question.prompt}</p>
                  {question.correctAnswerText ? <p className="mt-1 text-[10px] text-[#6b6b6b]">Đáp án: {question.correctAnswerText}</p> : null}
                </div>
                <div className="flex gap-1">
                  <Button size="icon" variant="outline" aria-label="Sửa câu hỏi" onClick={() => edit(question)}><Pencil size={14} /></Button>
                  <Button size="icon" variant="outline" aria-label="Xóa câu hỏi" disabled={workingId === question.id} onClick={() => void remove(question)}><Trash2 size={14} /></Button>
                </div>
              </div>
              <div className="mt-3 flex flex-wrap gap-2 border-t pt-3">
                {question.status === ContentStatus.Draft ? <Button size="sm" variant="outline" disabled={workingId === question.id} onClick={() => void workflow(question, () => quizApi.submitQuestionReview(quizId, question.id), "Đã gửi câu hỏi chờ duyệt.")}>Gửi duyệt</Button> : null}
                {question.status === ContentStatus.Review ? <Button size="sm" variant="outline" disabled={workingId === question.id} onClick={() => void workflow(question, () => quizApi.approveQuestion(quizId, question.id), "Đã duyệt câu hỏi.")}>Duyệt</Button> : null}
                {question.status === ContentStatus.Approved ? <Button size="sm" disabled={workingId === question.id} onClick={() => void workflow(question, () => quizApi.publishQuestion(quizId, question.id), "Đã xuất bản câu hỏi.")}>Xuất bản</Button> : null}
                {question.status === ContentStatus.Published ? <Button size="sm" variant="outline" disabled={workingId === question.id} onClick={() => void workflow(question, () => quizApi.archiveQuestion(quizId, question.id), "Đã lưu trữ câu hỏi.")}>Lưu trữ</Button> : null}
                {question.status === ContentStatus.Archived ? <Button size="sm" variant="outline" disabled={workingId === question.id} onClick={() => void workflow(question, () => quizApi.restoreQuestion(quizId, question.id), "Đã khôi phục câu hỏi.")}>Khôi phục</Button> : null}
              </div>
            </div>
          ))}
        </CardContent>
      </Card>
    </div>
  );
}
