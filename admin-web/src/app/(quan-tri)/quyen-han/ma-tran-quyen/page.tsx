"use client";

import {
  useState,
} from "react";

import {
  PageContainer,
} from "@/components/layout/page-container";

import {
  PageHeader,
} from "@/components/layout/page-header";

import {
  PageLoading,
} from "@/components/common/page-loading";

import {
  PermissionMatrix,
} from "@/features/identity/components/permission-matrix";

import {
  useAdminPermissions,
} from "@/features/identity/hooks/use-admin-permissions";

export default function PermissionMatrixPage() {
  const [
    selected,
    setSelected,
  ] = useState<string[]>([]);

  const query =
    useAdminPermissions({
      page: 1,
      pageSize: 1000,
    });

  if (
    query.isLoading
  ) {
    return (
      <PageLoading />
    );
  }

  return (
    <PageContainer>
      <PageHeader
        title="Ma trận quyền"
        description="Xem và kiểm tra quyền theo nhóm tài nguyên."
      />

      <PermissionMatrix
        permissions={
          (query.data?.items ?? []) as any[]
        }
        value={
          selected
        }
        onChange={
          setSelected
        }
      />
    </PageContainer>
  );
}
