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
import {
  AdminQuiz,
  CreateQuizRequest,
  QUIZ_TYPE_LABELS,
  QuizFeedbackMode,
  QuizShuffleMode,
  QuizType,
} from "../quiz.types";

interface QuizFormProps {
  quizId?: number;
}

type QuizFormState = Omit<CreateQuizRequest, "descriptionVi"> & {
  descriptionVi: string;
};

const DEFAULT_VALUES: QuizFormState = {
  titleVi: "",
  descriptionVi: "",
  quizType: QuizType.Lesson,
  passingScore: 70,
  timeLimitSeconds: null,
  maxAttempts: 3,
  lessonId: null,
  shuffleMode: QuizShuffleMode.QuestionsAndOptions,
  feedbackMode: QuizFeedbackMode.AfterSubmit,
  allowRetry: true,
  showCorrectAnswer: true,
  showExplanation: true,
};

function toFormState(quiz: AdminQuiz): QuizFormState {
  return {
    titleVi: quiz.titleVi,
    descriptionVi: quiz.descriptionVi ?? "",
    quizType: quiz.quizType,
    passingScore: quiz.passingScore,
    timeLimitSeconds: quiz.timeLimitSeconds,
    maxAttempts: quiz.maxAttempts,
    lessonId: quiz.lessonId,
    shuffleMode: quiz.shuffleMode,
    feedbackMode: quiz.feedbackMode,
    allowRetry: quiz.allowRetry,
    showCorrectAnswer: quiz.showCorrectAnswer,
    showExplanation: quiz.showExplanation,
  };
}

export function QuizForm({ quizId }: QuizFormProps) {
  const router = useRouter();
  const isEditing = Boolean(quizId);
  const [values, setValues] = useState<QuizFormState>(DEFAULT_VALUES);
  const [version, setVersion] = useState(1);
  const [loading, setLoading] = useState(isEditing);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    if (!quizId) return;
    let active = true;
    setLoading(true);
    quizApi.getById(quizId)
      .then((quiz) => {
        if (!active) return;
        setValues(toFormState(quiz));
        setVersion(quiz.version);
        setError(null);
      })
      .catch((caught) => {
        if (!active) return;
        setError(caught instanceof Error ? caught : new Error("Không thể tải bài kiểm tra."));
      })
      .finally(() => {
        if (active) setLoading(false);
      });
    return () => { active = false; };
  }, [quizId]);

  function update<K extends keyof QuizFormState>(key: K, value: QuizFormState[K]) {
    setValues((current) => ({ ...current, [key]: value }));
  }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!values.titleVi.trim()) {
      appToast.error("Thiếu tiêu đề", "Vui lòng nhập tên bài kiểm tra.");
      return;
    }
    if (values.passingScore < 0 || values.passingScore > 100) {
      appToast.error("Điểm đạt không hợp lệ", "Điểm đạt phải nằm trong khoảng 0–100.");
      return;
    }
    if (values.maxAttempts < 1) {
      appToast.error("Số lượt làm không hợp lệ", "MaxAttempts phải từ 1 trở lên.");
      return;
    }

    setSaving(true);
    try {
      const payload: CreateQuizRequest = {
        ...values,
        titleVi: values.titleVi.trim(),
        descriptionVi: values.descriptionVi.trim() || null,
        lessonId: values.lessonId || null,
        timeLimitSeconds: values.timeLimitSeconds || null,
      };

      const saved = isEditing && quizId
        ? await quizApi.update(quizId, { ...payload, version })
        : await quizApi.create(payload);

      appToast.success(isEditing ? "Đã cập nhật bài kiểm tra." : "Đã tạo bài kiểm tra.");
      router.push(`/bai-kiem-tra/${saved.id}`);
      router.refresh();
    } catch (caught) {
      appToast.error(
        isEditing ? "Không thể cập nhật bài kiểm tra" : "Không thể tạo bài kiểm tra",
        normalizeApiError(caught).message,
      );
    } finally {
      setSaving(false);
    }
  }

  if (loading) {
    return <div className="rounded-[11px] border border-[#e8e3dc] bg-white p-6 text-[12px] text-muted-foreground">Đang tải dữ liệu...</div>;
  }

  if (error) {
    return <ErrorState title="Không thể tải bài kiểm tra" description={error.message} onRetry={() => router.refresh()} />;
  }

  return (
    <form onSubmit={submit} className="space-y-5">
      <Card>
        <CardHeader><CardTitle>Thông tin cơ bản</CardTitle></CardHeader>
        <CardContent className="space-y-4">
          <label className="block space-y-1.5">
            <span className="text-[11px] font-medium">Tên bài kiểm tra *</span>
            <Input value={values.titleVi} onChange={(event) => update("titleVi", event.target.value)} placeholder="Ví dụ: Kiểm tra bài 1" />
          </label>

          <label className="block space-y-1.5">
            <span className="text-[11px] font-medium">Mô tả</span>
            <Textarea value={values.descriptionVi} onChange={(event) => update("descriptionVi", event.target.value)} placeholder="Mô tả mục tiêu và nội dung bài kiểm tra..." />
          </label>

          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
            <label className="space-y-1.5">
              <span className="text-[11px] font-medium">Loại Quiz</span>
              <select className="h-10 w-full rounded-md border border-input bg-background px-3 text-[12px]" value={values.quizType} onChange={(event) => update("quizType", Number(event.target.value) as QuizType)}>
                {Object.entries(QUIZ_TYPE_LABELS).map(([value, label]) => <option key={value} value={value}>{label}</option>)}
              </select>
            </label>
            <label className="space-y-1.5">
              <span className="text-[11px] font-medium">Lesson ID</span>
              <Input type="number" min={1} value={values.lessonId ?? ""} onChange={(event) => update("lessonId", event.target.value ? Number(event.target.value) : null)} placeholder="Không bắt buộc" />
            </label>
            <label className="space-y-1.5">
              <span className="text-[11px] font-medium">Điểm đạt (%)</span>
              <Input type="number" min={0} max={100} step="0.01" value={values.passingScore} onChange={(event) => update("passingScore", Number(event.target.value))} />
            </label>
            <label className="space-y-1.5">
              <span className="text-[11px] font-medium">Số lượt tối đa</span>
              <Input type="number" min={1} value={values.maxAttempts} onChange={(event) => update("maxAttempts", Number(event.target.value))} />
            </label>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader><CardTitle>Thiết lập làm bài</CardTitle></CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-4 md:grid-cols-3">
            <label className="space-y-1.5">
              <span className="text-[11px] font-medium">Thời gian (giây)</span>
              <Input type="number" min={1} value={values.timeLimitSeconds ?? ""} onChange={(event) => update("timeLimitSeconds", event.target.value ? Number(event.target.value) : null)} placeholder="Không giới hạn" />
            </label>
            <label className="space-y-1.5">
              <span className="text-[11px] font-medium">Trộn nội dung</span>
              <select className="h-10 w-full rounded-md border border-input bg-background px-3 text-[12px]" value={values.shuffleMode} onChange={(event) => update("shuffleMode", Number(event.target.value) as QuizShuffleMode)}>
                <option value={QuizShuffleMode.None}>Không trộn</option>
                <option value={QuizShuffleMode.QuestionsOnly}>Chỉ câu hỏi</option>
                <option value={QuizShuffleMode.OptionsOnly}>Chỉ đáp án</option>
                <option value={QuizShuffleMode.QuestionsAndOptions}>Câu hỏi và đáp án</option>
              </select>
            </label>
            <label className="space-y-1.5">
              <span className="text-[11px] font-medium">Phản hồi</span>
              <select className="h-10 w-full rounded-md border border-input bg-background px-3 text-[12px]" value={values.feedbackMode} onChange={(event) => update("feedbackMode", Number(event.target.value) as QuizFeedbackMode)}>
                <option value={QuizFeedbackMode.AfterEachAnswer}>Sau từng câu</option>
                <option value={QuizFeedbackMode.AfterSubmit}>Sau khi nộp</option>
                <option value={QuizFeedbackMode.None}>Không hiển thị</option>
              </select>
            </label>
          </div>

          <div className="grid gap-3 md:grid-cols-3">
            {([
              ["allowRetry", "Cho phép làm lại"],
              ["showCorrectAnswer", "Hiển thị đáp án đúng"],
              ["showExplanation", "Hiển thị giải thích"],
            ] as const).map(([key, label]) => (
              <label key={key} className="flex items-center gap-2 rounded-md border border-[#ece7df] p-3 text-[11px]">
                <input type="checkbox" checked={values[key]} onChange={(event) => update(key, event.target.checked)} />
                {label}
              </label>
            ))}
          </div>
        </CardContent>
      </Card>

      <div className="flex justify-end gap-2">
        <Button type="button" variant="outline" onClick={() => router.back()}>Hủy</Button>
        <Button type="submit" loading={saving}>{isEditing ? "Lưu thay đổi" : "Tạo bài kiểm tra"}</Button>
      </div>
    </form>
  );
}
