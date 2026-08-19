import Link from "next/link";
import { Plus } from "lucide-react";

import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { Button } from "@/components/ui/button";
import { PERMISSIONS } from "@/constants/permission.constants";
import { QuizTable } from "@/features/quiz/components/quiz-table";
import { PermissionGuard } from "@/security/permission-guard";

export default function QuizzesPage() {
  return (
    <PermissionGuard permission={PERMISSIONS.QUIZZES.READ}>
      <PageContainer>
        <PageHeader
          title="Quản lý bài kiểm tra"
          description="Quản lý Quiz, cấu hình làm bài, trạng thái biên tập và liên kết với bài giảng."
          actions={
            <PermissionGuard permission={PERMISSIONS.QUIZZES.CREATE} fallback={null}>
              <Link href="/bai-kiem-tra/them-moi">
                <Button className="h-[38px] gap-2 text-[12px]">
                  <Plus size={15} />
                  Thêm bài kiểm tra
                </Button>
              </Link>
            </PermissionGuard>
          }
        />
        <QuizTable />
      </PageContainer>
    </PermissionGuard>
  );
}
