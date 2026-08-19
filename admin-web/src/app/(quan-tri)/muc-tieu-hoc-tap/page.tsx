import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { LearningGoalsAdmin } from "@/features/learning/components/learning-goals-admin";

export default function LearningGoalsPage() {
  return (
    <PageContainer>
      <PageHeader
        title="Mục tiêu học tập"
        description="Quản lý mục tiêu HSK, thời lượng học, từ vựng hàng ngày và số bài học hàng tuần của học viên."
      />
      <LearningGoalsAdmin />
    </PageContainer>
  );
}
