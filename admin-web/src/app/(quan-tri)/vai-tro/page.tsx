import {
  Plus,
} from "lucide-react";

import Link from "next/link";

import {
  PageContainer,
} from "@/components/layout/page-container";

import {
  PageHeader,
} from "@/components/layout/page-header";

import {
  Button,
} from "@/components/ui/button";

import {
  RoleTable,
} from "@/features/identity/components/role-table";
import { PermissionGuard } from "@/security/permission-guard";

import { PERMISSIONS } from "@/constants/permission.constants";

export default function RolesPage() {
  return (
    <PermissionGuard permission={PERMISSIONS.ROLES.READ}>
      <PageContainer>
      <PageHeader
        title="Quản lý vai trò"
        description="Quản lý vai trò và tập quyền được cấp."
        actions={
          <Link
            href="/vai-tro/them-moi"
          >
            <Button
              className="h-[38px] gap-2 text-[12px]"
            >
              <Plus
                size={15}
              />

              Thêm vai trò
            </Button>
          </Link>
        }
      />

      <RoleTable />
    </PageContainer>
    </PermissionGuard>
  );
}
