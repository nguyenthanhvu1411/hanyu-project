"use client";

import { useParams } from "next/navigation";

import { ErrorState } from "@/components/common/error-state";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PERMISSIONS } from "@/constants/permission.constants";
import { NotificationDetail } from "@/features/notification/components/notification-detail";
import { PermissionGuard } from "@/security/permission-guard";

export default function NotificationDetailPage() {
  const params = useParams<{ id: string }>();
  const id = Number(params.id);

  if (!Number.isSafeInteger(id) || id <= 0) {
    return <PageContainer><ErrorState title="Thông báo không hợp lệ" description="ID thông báo không đúng định dạng." /></PageContainer>;
  }

  return (
    <PermissionGuard permission={PERMISSIONS.NOTIFICATIONS.READ}>
      <PageContainer>
        <PageHeader title="Chi tiết thông báo" description="Xem người nhận, nội dung, trạng thái đọc và metadata của thông báo." />
        <NotificationDetail id={id} />
      </PageContainer>
    </PermissionGuard>
  );
}
