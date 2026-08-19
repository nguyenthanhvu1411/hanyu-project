import Link from "next/link";
import { Plus } from "lucide-react";

import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { Button } from "@/components/ui/button";
import { PERMISSIONS } from "@/constants/permission.constants";
import { QuestionBankTable } from "@/features/quiz/components/question-bank-table";
import { PermissionGuard } from "@/security/permission-guard";

export default function QuestionBanksPage() {
  return (
    <PermissionGuard permission={PERMISSIONS.QUESTION_BANK.READ}>
      <PageContainer>
        <PageHeader
          title="Ngân hàng câu hỏi"
          description="Quản lý bộ câu hỏi dùng lại cho Quiz theo mã, cấp độ HSK và trạng thái kích hoạt."
          actions={
            <PermissionGuard permission={PERMISSIONS.QUESTION_BANK.CREATE} fallback={null}>
              <Link href="/cau-hoi/them-moi">
                <Button className="h-[38px] gap-2 text-[12px]"><Plus size={15} /> Thêm ngân hàng</Button>
              </Link>
            </PermissionGuard>
          }
        />
        <QuestionBankTable />
      </PageContainer>
    </PermissionGuard>
  );
}
