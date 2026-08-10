"use client";

import { useParams } from "next/navigation";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PageLoading } from "@/components/common/page-loading";
import { ErrorState } from "@/components/common/error-state";
import { PermissionGuard } from "@/security/permission-guard";
import { HskLevelForm } from "@/features/learning/components/hsk-level/hsk-level-form";
import { useHskLevelDetail } from "@/features/learning/hooks/use-hsk-levels";
import { PERMISSIONS } from "@/constants/permission.constants";

export default function EditHskLevelPage() {
  const params = useParams<{ id: string }>();
  const id = Number(params.id);
  const query = useHskLevelDetail(id);

  if (!Number.isFinite(id) || id <= 0) {
    return (
      <PageContainer>
        <ErrorState title="ID cấp độ HSK không hợp lệ" />
      </PageContainer>
    );
  }

  if (query.isLoading) {
    return <PageLoading text="Đang tải cấp độ HSK..." />;
  }

  if (query.isError) {
    return (
      <PageContainer>
        <ErrorState
          title="Không thể tải cấp độ HSK"
          onRetry={() => query.refetch()}
        />
      </PageContainer>
    );
  }

  if (!query.data) {
    return (
      <PageContainer>
        <ErrorState title="Không tìm thấy cấp độ HSK" />
      </PageContainer>
    );
  }

  return (
    <PermissionGuard permission={PERMISSIONS.HSK_LEVELS.UPDATE}>
      <PageContainer>
        <PageHeader
          title="Chỉnh sửa cấp độ HSK"
          description={query.data.code}
        />

        <HskLevelForm item={query.data} />
      </PageContainer>
    </PermissionGuard>
  );
}
