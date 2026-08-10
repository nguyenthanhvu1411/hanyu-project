import Link from "next/link";
import { Plus } from "lucide-react";

import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { Button } from "@/components/ui/button";
import { PermissionGuard } from "@/security/permission-guard";
import { PERMISSIONS } from "@/constants/permission.constants";
import { LessonTable } from "@/features/bai-giang/components/lesson-table";

export default function LessonsPage() {
  return (
    <PermissionGuard permission={PERMISSIONS.LESSONS.READ}>
      <PageContainer>
        <PageHeader
          title="Quản lý bài giảng"
          description="Quản lý nội dung bài học, trạng thái biên tập, HSK, khóa học và thứ tự hiển thị."
          actions={
            <PermissionGuard permission={PERMISSIONS.LESSONS.CREATE} fallback={null}>
              <Link href="/bai-giang/them-moi">
                <Button className="h-[38px] gap-2 text-[12px]">
                  <Plus size={15} />
                  Thêm bài giảng
                </Button>
              </Link>
            </PermissionGuard>
          }
        />

        <LessonTable />
      </PageContainer>
    </PermissionGuard>
  );
}
