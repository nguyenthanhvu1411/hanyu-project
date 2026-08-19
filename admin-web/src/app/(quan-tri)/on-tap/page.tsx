import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { ReviewAdminWorkspaceV2 } from "@/features/review/components/review-admin-workspace-v2";

export default function ReviewAdminPage() {
  return (
    <PageContainer>
      <PageHeader
        title="Ôn tập & Flashcard"
        description="Theo dõi dashboard ôn tập, trạng thái từ vựng, phiên flashcard, lịch sử review và tổng hợp theo học viên."
      />
      <ReviewAdminWorkspaceV2 />
    </PageContainer>
  );
}