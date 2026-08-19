import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PERMISSIONS } from "@/constants/permission.constants";
import { AuditLogTable } from "@/features/system/components/audit-log-table";
import { PermissionGuard } from "@/security/permission-guard";

export default function AuditLogsPage() {
  return (
    <PermissionGuard permission={PERMISSIONS.AUDIT.READ}>
      <PageContainer>
        <PageHeader
          title="Nhật ký hệ thống"
          description="Theo dõi hành động quản trị, thay đổi dữ liệu, IP, correlation và dữ liệu trước/sau."
        />
        <AuditLogTable />
      </PageContainer>
    </PermissionGuard>
  );
}
