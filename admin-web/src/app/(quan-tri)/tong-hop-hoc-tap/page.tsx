import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { LearningSummariesAdmin } from "@/features/learning/components/learning-summaries-admin";

export default function LearningSummariesPage() {
  return (
    <PageContainer>
      <PageHeader
        title="Tổng hợp học tập"
        description="Theo dõi HSK hiện tại, mastery, thời lượng học, tiến độ từ vựng, quiz và XP; hỗ trợ tính lại summary từ dữ liệu nguồn."
      />
      <LearningSummariesAdmin />
    </PageContainer>
  );
}
