import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PERMISSIONS } from "@/constants/permission.constants";
import { AuditLogTable } from "@/features/system/components/audit-log-table";
import { ProductEventTable } from "@/features/system/components/product-event-table";
import { PermissionGuard } from "@/security/permission-guard";

export default function AuditLogsPage() {
  return (
    <PermissionGuard permission={PERMISSIONS.AUDIT.READ}>
      <PageContainer>
        <PageHeader
          title="Nhật ký hệ thống"
          description="Theo dõi hành động quản trị, thay đổi dữ liệu và các Product Event phát sinh trong hệ thống."
        />

        <section className="space-y-3">
          <div>
            <h2 className="text-[14px] font-semibold text-[#333]">Audit Logs</h2>
            <p className="mt-1 text-[11px] text-muted-foreground">Hành động quản trị, dữ liệu trước/sau, IP và correlation.</p>
          </div>
          <AuditLogTable />
        </section>

        <section className="mt-8 space-y-3">
          <div>
            <h2 className="text-[14px] font-semibold text-[#333]">Product Events</h2>
            <p className="mt-1 text-[11px] text-muted-foreground">Sự kiện sản phẩm theo user, session, entity, trang và thiết bị.</p>
          </div>
          <ProductEventTable />
        </section>
      </PageContainer>
    </PermissionGuard>
  );
}
