"use client";

import { useParams } from "next/navigation";

import { ErrorState } from "@/components/common/error-state";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PERMISSIONS } from "@/constants/permission.constants";
import { QuizForm } from "@/features/quiz/components/quiz-form";
import { PermissionGuard } from "@/security/permission-guard";

export default function EditQuizPage() {
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
        <PageHeader title="Chỉnh sửa bài kiểm tra" description="Cập nhật metadata và cấu hình làm bài. Workflow được quản lý tại trang chi tiết." />
        <QuizForm quizId={quizId} />
      </PageContainer>
    </PermissionGuard>
  );
}
