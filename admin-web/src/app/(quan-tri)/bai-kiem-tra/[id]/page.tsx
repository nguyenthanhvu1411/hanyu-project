"use client";

import { useParams } from "next/navigation";

import { ErrorState } from "@/components/common/error-state";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PERMISSIONS } from "@/constants/permission.constants";
import { QuizDetail } from "@/features/quiz/components/quiz-detail";
import { PermissionGuard } from "@/security/permission-guard";

export default function QuizDetailPage() {
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
    <PermissionGuard permission={PERMISSIONS.QUIZZES.READ}>
      <PageContainer>
        <PageHeader title="Chi tiết bài kiểm tra" description="Theo dõi cấu hình, trạng thái và thực hiện workflow biên tập Quiz." />
        <QuizDetail quizId={quizId} />
      </PageContainer>
    </PermissionGuard>
  );
}
