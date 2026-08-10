"use client";

import { ArrowLeft, Edit3 } from "lucide-react";
import Link from "next/link";
import { useParams } from "next/navigation";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PageLoading } from "@/components/common/page-loading";
import { ErrorState } from "@/components/common/error-state";
import { Button } from "@/components/ui/button";
import { PermissionGuard } from "@/security/permission-guard";
import { RoleDetail } from "@/features/identity/components/role-detail";
import { useAdminRole } from "@/features/identity/hooks/use-admin-roles";
import { PERMISSIONS } from "@/constants/permission.constants";

export default function RoleDetailPage() {
  const params = useParams<{ id: string; }>();
  const id = params.id;

  const query = useAdminRole(id);

  if (query.isLoading) {
    return <PageLoading text="Đang tải thông tin vai trò..." />;
  }

  if (query.isError || !query.data) {
    return (
      <PageContainer>
        <ErrorState
          title="Không thể tải vai trò"
          description="Vai trò không tồn tại hoặc dữ liệu không thể tải."
          onRetry={() => query.refetch()}
        />
      </PageContainer>
    );
  }

  const role = query.data;

  return (
    <PermissionGuard permission={PERMISSIONS.ROLES.READ}>
      <PageContainer>
        <PageHeader
          title={role.name}
          description={role.description || `Mã vai trò: ${role.code}`}
          actions={
            <div className="flex gap-2">
              <Link href="/vai-tro">
                <Button type="button" variant="outline" className="h-[38px] gap-2 text-[11px]">
                  <ArrowLeft size={14} />
                  Quay lại
                </Button>
              </Link>
              {!role.deletedAt && (
                <PermissionGuard permission={PERMISSIONS.ROLES.UPDATE} fallback={null}>
                  <Link href={`/vai-tro/${id}/chinh-sua`}>
                    <Button type="button" className="h-[38px] gap-2 text-[11px]">
                      <Edit3 size={14} />
                      Chỉnh sửa
                    </Button>
                  </Link>
                </PermissionGuard>
              )}
            </div>
          }
        />
        <RoleDetail role={role} />
      </PageContainer>
    </PermissionGuard>
  );
}
