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
  UserForm,
} from "@/features/identity/components/user-form";

import {
  useAdminUser,
} from "@/features/identity/hooks/use-admin-users";

export default function EditUserPage() {
  const params =
    useParams<{
      id: string;
    }>();

  const query =
    useAdminUser(
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
        title="Chỉnh sửa người dùng"
        description={
          query.data.email
        }
      />

      <UserForm
        user={
          query.data
        }
      />
    </PageContainer>
  );
}
