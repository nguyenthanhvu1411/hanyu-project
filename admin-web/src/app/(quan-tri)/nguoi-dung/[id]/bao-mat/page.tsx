"use client";

import { useParams } from "next/navigation";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PermissionGuard } from "@/security/permission-guard";
import { UserSecurityPanel } from "@/features/identity/components/user-security-panel";
import { PERMISSIONS } from "@/constants/permission.constants";

export default function UserSecurityPage() {
  const params = useParams<{ id: string }>();

  return (
    <PermissionGuard permission={PERMISSIONS.USERS.UPDATE}>
      <PageContainer>
        <PageHeader
          title="Bảo mật tài khoản"
          description="Khóa, mở khóa và quản lý các phiên đăng nhập của tài khoản."
        />

        <UserSecurityPanel userId={params.id} />
      </PageContainer>
    </PermissionGuard>
  );
}
