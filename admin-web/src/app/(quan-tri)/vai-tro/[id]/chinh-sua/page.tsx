"use client";

import {
  useParams,
} from "next/navigation";

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
  ErrorState,
} from "@/components/common/error-state";

import {
  RoleForm,
} from "@/features/identity/components/role-form";

import {
  useAdminRole,
} from "@/features/identity/hooks/use-admin-roles";

export default function EditRolePage() {
  const params =
    useParams<{
      id: string;
    }>();

  const query =
    useAdminRole(
      params.id,
    );

  if (
    query.isLoading
  ) {
    return (
      <PageLoading />
    );
  }

  if (
    !query.data
  ) {
    return (
      <ErrorState />
    );
  }

  return (
    <PageContainer>
      <PageHeader
        title="Chỉnh sửa vai trò"
        description={
          query.data.code
        }
      />

      <RoleForm
        role={
          query.data
        }
      />
    </PageContainer>
  );
}
