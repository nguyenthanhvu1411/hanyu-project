import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PERMISSIONS } from "@/constants/permission.constants";
import { NotificationComposer } from "@/features/notification/components/notification-composer";
import { PermissionGuard } from "@/security/permission-guard";

export default function CreateNotificationPage() {
  return (
    <PermissionGuard permission={PERMISSIONS.NOTIFICATIONS.SEND}>
      <PageContainer>
        <PageHeader
          title="Gửi thông báo"
          description="Gửi trực tiếp cho một người dùng hoặc broadcast theo API Notification Admin."
        />
        <NotificationComposer />
      </PageContainer>
    </PermissionGuard>
  );
}
