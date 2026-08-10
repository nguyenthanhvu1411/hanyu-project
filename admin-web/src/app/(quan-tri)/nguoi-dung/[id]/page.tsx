"use client";

import {
  Pencil,
  ShieldCheck,
  LockKeyhole,
} from "lucide-react";

import Link from "next/link";

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
  Button,
} from "@/components/ui/button";

import {
  UserDetail,
} from "@/features/identity/components/user-detail";

import {
  useAdminUser,
} from "@/features/identity/hooks/use-admin-users";

import {
  PermissionGuard,
} from "@/security/permission-guard";

import { PERMISSIONS } from "@/constants/permission.constants";

export default function UserDetailPage() {
  const params =
    useParams<{
      id: string;
    }>();

  const id =
    params.id;

  const query =
    useAdminUser(id);

  if (
    query.isLoading
  ) {
    return (
      <PageLoading />
    );
  }

  if (
    query.isError ||
    !query.data
  ) {
    return (
      <ErrorState
        onRetry={() =>
          query.refetch()
        }
      />
    );
  }

  return (
    <PageContainer>
      <PageHeader
        title={
          query.data
            .displayName
        }
        description={
          query.data.email
        }
        actions={
          !query.data.deletedAt ? (
            <>
              <PermissionGuard permission={PERMISSIONS.USERS.MANAGE_ROLES} fallback={null}>
                <Link
                  href={`/nguoi-dung/${id}/vai-tro`}
                >
                  <Button
                    variant="outline"
                    className="
                      h-[38px]
                      gap-2
                      text-[11px]
                    "
                  >
                    <ShieldCheck
                      size={14}
                    />
                    Vai trò
                  </Button>
                </Link>
              </PermissionGuard>

              <PermissionGuard permission={PERMISSIONS.USERS.UPDATE} fallback={null}>
                <Link
                  href={`/nguoi-dung/${id}/bao-mat`}
                >
                  <Button
                    variant="outline"
                    className="
                      h-[38px]
                      gap-2
                      text-[11px]
                    "
                  >
                    <LockKeyhole
                      size={14}
                    />
                    Bảo mật
                  </Button>
                </Link>
              </PermissionGuard>

              <Link
                href={`/nguoi-dung/${id}/chinh-sua`}
              >
                <Button
                  className="
                    h-[38px]
                    gap-2
                    text-[11px]
                  "
                >
                  <Pencil
                    size={14}
                  />
                  Chỉnh sửa
                </Button>
              </Link>
            </>
          ) : undefined
        }
      />

      <UserDetail
        user={
          query.data
        }
      />
    </PageContainer>
  );
}
