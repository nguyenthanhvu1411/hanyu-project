import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PermissionGuard } from "@/security/permission-guard";
import { PERMISSIONS } from "@/constants/permission.constants";
import { LessonEditor } from "@/features/bai-giang/components/lesson-editor";

export default function CreateLessonPage() {
  return (
    <PermissionGuard permission={PERMISSIONS.LESSONS.CREATE}>
      <PageContainer>
        <PageHeader
          title="Thêm bài giảng"
          description="Tạo bài giảng mới, thiết lập HSK, nội dung mô tả và thông tin học tập."
        />
        <LessonEditor />
      </PageContainer>
    </PermissionGuard>
  );
}
