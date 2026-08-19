import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { TopicTable } from "@/features/vocabulary/components/topic-table";

export default function TopicsPage() {
  return (
    <PageContainer>
      <PageHeader
        title="Chủ đề"
        description="Quản lý chủ đề nội dung dùng chung cho từ vựng và bài giảng."
      />

      <TopicTable />
    </PageContainer>
  );
}
