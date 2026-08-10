import { Plus } from "lucide-react";
import Link from "next/link";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { Button } from "@/components/ui/button";
import { PermissionGuard } from "@/security/permission-guard";
import { HskLevelTable } from "@/features/learning/components/hsk-level/hsk-level-table";
import { PERMISSIONS } from "@/constants/permission.constants";

export default function HskLevelsPage() {
  return (
    <PermissionGuard permission={PERMISSIONS.HSK_LEVELS.READ}>
      <PageContainer>
        <PageHeader
          title="Cấp độ HSK"
          description="Quản lý các cấp độ HSK được sử dụng trong hệ thống."
          actions={
            <PermissionGuard
              permission={PERMISSIONS.HSK_LEVELS.CREATE}
              fallback={null}
            >
              <Link href="/cap-do-hsk/them-moi">
                <Button className="h-[38px] gap-2 px-4 text-[11px]">
                  <Plus size={14} />
                  Thêm cấp độ
                </Button>
              </Link>
            </PermissionGuard>
          }
        />

        <HskLevelTable />
      </PageContainer>
    </PermissionGuard>
  );
}
