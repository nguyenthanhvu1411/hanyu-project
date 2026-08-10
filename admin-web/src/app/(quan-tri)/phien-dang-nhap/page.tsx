import {
  PageContainer,
} from "@/components/layout/page-container";

import {
  PageHeader,
} from "@/components/layout/page-header";

import {
  PermissionGuard,
} from "@/security/permission-guard";

import {
  SessionTable,
} from "@/features/identity/components/session-table";

import {
  PERMISSIONS,
} from "@/constants/permission.constants";

export default function SessionsPage() {
  return (
    <PermissionGuard
      permission={
        PERMISSIONS.SESSIONS.READ
      }
    >
      <PageContainer>
        <PageHeader
          title="Phiên đăng nhập"
          description="Theo dõi và quản lý toàn bộ phiên đăng nhập trong hệ thống."
        />

        <SessionTable />
      </PageContainer>
    </PermissionGuard>
  );
}
