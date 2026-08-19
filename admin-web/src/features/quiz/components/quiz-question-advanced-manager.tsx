"use client";

import { useCallback, useEffect, useState } from "react";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";

import { quizApi } from "../quiz.api";
import type { AdminQuizQuestion } from "../quiz.types";
import { QUIZ_QUESTION_TYPE_LABELS } from "../quiz.types";
import { QuizQuestionContentManager } from "./quiz-question-content-manager";

interface QuizQuestionAdvancedManagerProps {
  quizId: number;
}

export function QuizQuestionAdvancedManager({ quizId }: QuizQuestionAdvancedManagerProps) {
  const [questions, setQuestions] = useState<AdminQuizQuestion[]>([]);
  const [questionId, setQuestionId] = useState<number | null>(null);
  const [loading, setLoading] = useState(true);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const items = await quizApi.listQuestions(quizId);
      const ordered = [...items].sort((a, b) => a.sortOrder - b.sortOrder || a.id - b.id);
      setQuestions(ordered);
      setQuestionId((current) => current && ordered.some((item) => item.id === current) ? current : ordered[0]?.id ?? null);
    } catch (caught) {
      appToast.error("Không thể tải câu hỏi", normalizeApiError(caught).message);
      setQuestions([]);
      setQuestionId(null);
    } finally {
      setLoading(false);
    }
  }, [quizId]);

  useEffect(() => { void load(); }, [load]);

  const selected = questions.find((item) => item.id === questionId) ?? null;

  return (
    <Card>
      <CardHeader>
        <CardTitle>Cấu hình nâng cao Question</CardTitle>
        <p className="mt-1 text-[11px] text-muted-foreground">
          Quản lý Options, Matching Pairs và Tags bằng API backend thật của từng câu hỏi.
        </p>
      </CardHeader>
      <CardContent>
        {loading ? <div className="py-6 text-center text-[12px] text-muted-foreground">Đang tải...</div> : null}
        {!loading && questions.length === 0 ? (
          <div className="rounded-md border border-dashed p-6 text-center text-[12px] text-muted-foreground">
            Hãy tạo ít nhất một câu hỏi trước khi cấu hình đáp án hoặc Tags.
          </div>
        ) : null}
        {questions.length > 0 ? (
          <>
            <label className="block max-w-xl space-y-1">
              <span className="text-[11px] font-medium">Chọn Question</span>
              <select
                className="h-10 w-full rounded-md border border-input bg-background px-3 text-[12px]"
                value={questionId ?? ""}
                onChange={(event) => setQuestionId(Number(event.target.value))}
              >
                {questions.map((question, index) => (
                  <option key={question.id} value={question.id}>
                    #{index + 1} · {QUIZ_QUESTION_TYPE_LABELS[question.questionType]} · {question.prompt.slice(0, 80)}
                  </option>
                ))}
              </select>
            </label>
            {selected ? (
              <QuizQuestionContentManager
                key={selected.id}
                quizId={quizId}
                questionId={selected.id}
                questionType={selected.questionType}
              />
            ) : null}
          </>
        ) : null}
      </CardContent>
    </Card>
  );
}
