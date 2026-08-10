"use client";

import { useParams } from "next/navigation";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PermissionGuard } from "@/security/permission-guard";
import { UserRoleManager } from "@/features/identity/components/user-role-manager";
import { PERMISSIONS } from "@/constants/permission.constants";

export default function UserRolesPage() {
  const params = useParams<{ id: string }>();

  return (
    <PermissionGuard permission={PERMISSIONS.USERS.MANAGE_ROLES}>
      <PageContainer>
        <PageHeader
          title="Vai trò người dùng"
          description="Quản lý tập vai trò và quyền truy cập của tài khoản."
        />

        <UserRoleManager userId={params.id} />
      </PageContainer>
    </PermissionGuard>
  );
}
