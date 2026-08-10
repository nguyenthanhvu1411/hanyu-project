import Link from "next/link";
import { Plus } from "lucide-react";

import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { Button } from "@/components/ui/button";
import { PermissionGuard } from "@/security/permission-guard";
import { PERMISSIONS } from "@/constants/permission.constants";
import { CourseTable } from "@/features/course/components/course-table";

export default function CoursesPage() {
  return (
    <PermissionGuard permission={PERMISSIONS.COURSES.READ}>
      <PageContainer>
        <PageHeader
          title="Khóa học"
          description="Quản lý khóa học, HSK, chương học, trạng thái biên tập và nội dung liên quan."
          actions={
            <PermissionGuard permission={PERMISSIONS.COURSES.CREATE} fallback={null}>
              <Link href="/khoa-hoc/them-moi">
                <Button className="h-[38px] gap-2 px-4 text-[11px]">
                  <Plus size={14} />
                  Thêm khóa học
                </Button>
              </Link>
            </PermissionGuard>
          }
        />

        <CourseTable />
      </PageContainer>
    </PermissionGuard>
  );
}
