import {
  Grid2X2,
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
  PermissionTable,
} from "@/features/identity/components/permission-table";
import { PermissionGuard } from "@/security/permission-guard";

import { PERMISSIONS } from "@/constants/permission.constants";

export default function PermissionsPage() {
  return (
    <PermissionGuard permission={PERMISSIONS.PERMISSIONS.READ}>
      <PageContainer>
      <PageHeader
        title="Quản lý quyền hạn"
        description="Danh mục quyền truy cập chi tiết của hệ thống."
        actions={
          <Link
            href="/quyen-han/ma-tran-quyen"
          >
            <Button
              variant="outline"
              className="h-[38px] gap-2 text-[11px]"
            >
              <Grid2X2
                size={14}
              />

              Ma trận quyền
            </Button>
          </Link>
        }
      />

      <PermissionTable />
    </PageContainer>
    </PermissionGuard>
  );
}
