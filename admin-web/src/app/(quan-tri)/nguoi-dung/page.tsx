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
  ExcelToolbar,
} from "@/components/common/excel/excel-toolbar";

import {
  UserTable,
} from "@/features/identity/components/user-table";
import { PermissionGuard } from "@/security/permission-guard";

import { PERMISSIONS } from "@/constants/permission.constants";

export default function UsersPage() {
  return (
    <PermissionGuard permission={PERMISSIONS.USERS.READ}>
      <PageContainer>
        <PageHeader
          title="Quản lý người dùng"
          description="Quản lý tài khoản, trạng thái, vai trò và bảo mật người dùng."
          actions={
            <>
              <ExcelToolbar
                moduleName="người dùng"
                sampleFileUrl="/file-mau/mau-import-nguoi-dung.xlsx"
              />

              <Link
                href="/nguoi-dung/them-moi"
              >
                <Button
                  className="
                  h-[38px]
                  gap-2
                  text-[12px]
                "
                >
                  <Plus
                    size={15}
                  />

                  Thêm người dùng
                </Button>
              </Link>
            </>
          }
        />

        <UserTable />
      </PageContainer>
    </PermissionGuard>
  );
}
