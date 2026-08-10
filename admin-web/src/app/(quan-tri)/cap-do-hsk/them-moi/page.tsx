import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PermissionGuard } from "@/security/permission-guard";
import { HskLevelForm } from "@/features/learning/components/hsk-level/hsk-level-form";
import { PERMISSIONS } from "@/constants/permission.constants";

export default function CreateHskLevelPage() {
  return (
    <PermissionGuard permission={PERMISSIONS.HSK_LEVELS.CREATE}>
      <PageContainer>
        <PageHeader
          title="Thêm cấp độ HSK"
          description="Tạo mới cấp độ HSK."
        />

        <HskLevelForm />
      </PageContainer>
    </PermissionGuard>
  );
}
