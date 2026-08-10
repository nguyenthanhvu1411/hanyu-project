"use client";

import { ArrowLeft, Pencil } from "lucide-react";
import Link from "next/link";
import { useParams } from "next/navigation";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PageLoading } from "@/components/common/page-loading";
import { ErrorState } from "@/components/common/error-state";
import { Button } from "@/components/ui/button";
import { PermissionGuard } from "@/security/permission-guard";
import { HskLevelDetail } from "@/features/learning/components/hsk-level/hsk-level-detail";
import { useHskLevelDetail } from "@/features/learning/hooks/use-hsk-levels";
import { PERMISSIONS } from "@/constants/permission.constants";

export default function HskLevelDetailPage() {
  const params = useParams<{ id: string }>();
  const id = Number(params.id);
  const query = useHskLevelDetail(id);

  if (!Number.isFinite(id) || id <= 0) {
    return (
      <PageContainer>
        <ErrorState
          title="Cấp độ HSK không hợp lệ"
          description="ID cấp độ HSK không đúng định dạng."
        />
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
        <ErrorState
          title="Không tìm thấy cấp độ HSK"
          description="Cấp độ HSK này không tồn tại."
        />
      </PageContainer>
    );
  }

  const item = query.data;

  return (
    <PermissionGuard permission={PERMISSIONS.HSK_LEVELS.READ}>
      <PageContainer>
        <PageHeader
          title={item.nameVi}
          description={item.code}
          actions={
            <>
              <Link href="/cap-do-hsk">
                <Button
                  variant="outline"
                  className="h-[38px] gap-2 text-[11px]"
                >
                  <ArrowLeft size={14} />
                  Quay lại
                </Button>
              </Link>

              <PermissionGuard
                permission={PERMISSIONS.HSK_LEVELS.UPDATE}
                fallback={null}
              >
                <Link href={`/cap-do-hsk/${item.id}/chinh-sua`}>
                  <Button className="h-[38px] gap-2 text-[11px]">
                    <Pencil size={14} />
                    Chỉnh sửa
                  </Button>
                </Link>
              </PermissionGuard>
            </>
          }
        />

        <HskLevelDetail item={item} />
      </PageContainer>
    </PermissionGuard>
  );
}
