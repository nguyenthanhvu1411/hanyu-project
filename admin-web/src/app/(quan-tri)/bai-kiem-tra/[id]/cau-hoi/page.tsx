"use client";

import { useParams } from "next/navigation";

import { ErrorState } from "@/components/common/error-state";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PERMISSIONS } from "@/constants/permission.constants";
import { QuizQuestionAdvancedManager } from "@/features/quiz/components/quiz-question-advanced-manager";
import { QuizQuestionManager } from "@/features/quiz/components/quiz-question-manager";
import { PermissionGuard } from "@/security/permission-guard";

export default function QuizQuestionsPage() {
  const params = useParams<{ id: string }>();
  const quizId = Number(params.id);

  if (!Number.isSafeInteger(quizId) || quizId <= 0) {
    return (
      <PageContainer>
        <ErrorState title="Bài kiểm tra không hợp lệ" description="ID bài kiểm tra không đúng định dạng." />
      </PageContainer>
    );
  }

  return (
    <PermissionGuard permission={PERMISSIONS.QUIZZES.UPDATE}>
      <PageContainer>
        <PageHeader
          title="Câu hỏi bài kiểm tra"
          description="Tạo, sửa, sắp thứ tự, quản lý workflow và cấu hình Options / Matching Pairs / Tags cho từng Question."
        />
        <div className="space-y-5">
          <QuizQuestionManager quizId={quizId} />
          <QuizQuestionAdvancedManager quizId={quizId} />
        </div>
      </PageContainer>
    </PermissionGuard>
  );
}
