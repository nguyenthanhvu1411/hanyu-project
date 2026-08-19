"use client";

import { useParams } from "next/navigation";

import { ErrorState } from "@/components/common/error-state";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PERMISSIONS } from "@/constants/permission.constants";
import { QuestionBankDetail } from "@/features/quiz/components/question-bank-detail";
import { PermissionGuard } from "@/security/permission-guard";

export default function QuestionBankDetailPage() {
  const params = useParams<{ id: string }>();
  const bankId = Number(params.id);

  if (!Number.isSafeInteger(bankId) || bankId <= 0) {
    return (
      <PageContainer>
        <ErrorState title="Ngân hàng câu hỏi không hợp lệ" description="ID không đúng định dạng." />
      </PageContainer>
    );
  }

  return (
    <PermissionGuard permission={PERMISSIONS.QUESTION_BANK.READ}>
      <PageContainer>
        <PageHeader title="Chi tiết ngân hàng câu hỏi" description="Theo dõi cấu hình, trạng thái và số lượng câu hỏi trong bank." />
        <QuestionBankDetail bankId={bankId} />
      </PageContainer>
    </PermissionGuard>
  );
}
