import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { QuizAttemptsAdmin } from "@/features/quiz/components/quiz-attempts-admin";

export default function QuizAttemptsPage() {
  return (
    <PageContainer>
      <PageHeader
        title="Lượt làm bài"
        description="Theo dõi lượt làm quiz theo học viên và bài kiểm tra, kết quả, tỷ lệ đạt và chi tiết từng câu trả lời."
      />
      <QuizAttemptsAdmin />
    </PageContainer>
  );
}