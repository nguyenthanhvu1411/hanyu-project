import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { AiAnalyticsWorkspace } from "@/features/ai-analytics/components/ai-analytics-workspace";

export default function AiAnalyticsPage() {
  return (
    <PageContainer>
      <PageHeader
        title="AI & Analytics"
        description="Giám sát request AI, conversation, feedback, cache và analytics học tập bằng dữ liệu quản trị thực."
      />
      <AiAnalyticsWorkspace />
    </PageContainer>
  );
}
