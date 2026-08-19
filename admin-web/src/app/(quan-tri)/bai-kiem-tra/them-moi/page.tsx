import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PERMISSIONS } from "@/constants/permission.constants";
import { QuizForm } from "@/features/quiz/components/quiz-form";
import { PermissionGuard } from "@/security/permission-guard";

export default function CreateQuizPage() {
  return (
    <PermissionGuard permission={PERMISSIONS.QUIZZES.CREATE}>
      <PageContainer>
        <PageHeader
          title="Thêm bài kiểm tra"
          description="Tạo Quiz mới và cấu hình điểm đạt, thời gian, số lượt làm, trộn câu hỏi và chế độ phản hồi."
        />
        <QuizForm />
      </PageContainer>
    </PermissionGuard>
  );
}
