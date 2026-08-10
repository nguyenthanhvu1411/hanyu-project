import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PermissionGuard } from "@/security/permission-guard";
import { PERMISSIONS } from "@/constants/permission.constants";
import { ChapterDirectory } from "@/features/course/components/chapter-directory";

export default function ChaptersPage() {
  return (
    <PermissionGuard permission={PERMISSIONS.CHAPTERS.READ}>
      <PageContainer>
        <PageHeader
          title="Chương học"
          description="Chương học thuộc từng khóa học. Chọn khóa học để quản lý đúng endpoint backend lồng theo CourseId."
        />
        <ChapterDirectory />
      </PageContainer>
    </PermissionGuard>
  );
}
