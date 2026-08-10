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
  UserSessionTable,
} from "@/features/identity/components/user-session-table";

export default function UserSessionsPage() {
  const params =
    useParams<{
      id: string;
    }>();

  const userId =
    params.id;

  return (
    <PageContainer>
      <PageHeader
        title="Phiên đăng nhập"
        description="Theo dõi và thu hồi các phiên đăng nhập của người dùng."
      />

      <UserSessionTable
        userId={
          userId
        }
      />
    </PageContainer>
  );
}
