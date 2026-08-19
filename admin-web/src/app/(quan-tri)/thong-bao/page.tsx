import Link from "next/link";
import { Plus } from "lucide-react";

import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { Button } from "@/components/ui/button";
import { PERMISSIONS } from "@/constants/permission.constants";
import { NotificationTable } from "@/features/notification/components/notification-table";
import { PermissionGuard } from "@/security/permission-guard";

export default function NotificationsPage() {
  return (
    <PermissionGuard permission={PERMISSIONS.NOTIFICATIONS.READ}>
      <PageContainer>
        <PageHeader
          title="Quản lý thông báo"
          description="Theo dõi lịch sử thông báo và trạng thái đọc/hết hạn của người dùng."
          actions={
            <PermissionGuard permission={PERMISSIONS.NOTIFICATIONS.SEND} fallback={null}>
              <Link href="/thong-bao/them-moi">
                <Button className="h-[38px] gap-2 text-[12px]"><Plus size={15} />Gửi thông báo</Button>
              </Link>
            </PermissionGuard>
          }
        />
        <NotificationTable />
      </PageContainer>
    </PermissionGuard>
  );
}
