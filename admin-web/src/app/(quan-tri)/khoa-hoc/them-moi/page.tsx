import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { CourseForm } from "@/features/course/components/course-form";
import { PERMISSIONS } from "@/constants/permission.constants";
import { PermissionGuard } from "@/security/permission-guard";

export default function CreateCoursePage() {
  return (
    <PermissionGuard permission={PERMISSIONS.COURSES.CREATE}>
      <PageContainer>
        <PageHeader
          title="Thêm khóa học"
          description="Tạo khóa học mới theo đúng contract Course của backend."
        />
        <CourseForm />
      </PageContainer>
    </PermissionGuard>
  );
}
