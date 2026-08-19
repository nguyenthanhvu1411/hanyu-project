import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { ContentOperationsWorkspace } from "@/features/content/components/content-operations-workspace";

export default function ContentOperationsPage() {
  return (
    <PageContainer>
      <PageHeader
        title="Vận hành nội dung"
        description="Xử lý báo cáo nội dung và theo dõi các content import job bằng API quản trị thực."
      />
      <ContentOperationsWorkspace />
    </PageContainer>
  );
}
