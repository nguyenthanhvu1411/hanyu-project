"use client";

import { useParams } from "next/navigation";

import { ErrorState } from "@/components/common/error-state";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PERMISSIONS } from "@/constants/permission.constants";
import { QuestionBankForm } from "@/features/quiz/components/question-bank-form";
import { PermissionGuard } from "@/security/permission-guard";

export default function EditQuestionBankPage() {
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
    <PermissionGuard permission={PERMISSIONS.QUESTION_BANK.UPDATE}>
      <PageContainer>
        <PageHeader title="Chỉnh sửa ngân hàng câu hỏi" description="Cập nhật mã, tên, mô tả và cấp độ HSK của ngân hàng." />
        <QuestionBankForm bankId={bankId} />
      </PageContainer>
    </PermissionGuard>
  );
}
