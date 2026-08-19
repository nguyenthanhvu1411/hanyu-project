import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PERMISSIONS } from "@/constants/permission.constants";
import { QuestionBankForm } from "@/features/quiz/components/question-bank-form";
import { PermissionGuard } from "@/security/permission-guard";

export default function CreateQuestionBankPage() {
  return (
    <PermissionGuard permission={PERMISSIONS.QUESTION_BANK.CREATE}>
      <PageContainer>
        <PageHeader title="Thêm ngân hàng câu hỏi" description="Tạo bộ câu hỏi dùng lại cho Quiz và phân loại theo HSK." />
        <QuestionBankForm />
      </PageContainer>
    </PermissionGuard>
  );
}
