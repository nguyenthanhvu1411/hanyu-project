import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { LearningActivitiesAdmin } from "@/features/learning/components/learning-activities-admin";

export default function LearningActivitiesPage() {
  return (
    <PageContainer>
      <PageHeader
        title="Hoạt động học tập"
        description="Theo dõi và quản trị hoạt động bài giảng, từ vựng, flashcard, quiz và AI Tutor của học viên."
      />
      <LearningActivitiesAdmin />
    </PageContainer>
  );
}
