import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { ReviewAdminWorkspace } from "@/features/review/components/review-admin-workspace";

export default function ReviewAdminPage() {
  return (
    <PageContainer>
      <PageHeader
        title="Ôn tập & Flashcard"
        description="Theo dõi dashboard ôn tập, trạng thái từ vựng, phiên flashcard, sự kiện review và tổng hợp theo người dùng."
      />
      <ReviewAdminWorkspace />
    </PageContainer>
  );
}
