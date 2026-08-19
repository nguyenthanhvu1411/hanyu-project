"use client";

import { ArrowLeft } from "lucide-react";
import Link from "next/link";
import { useParams } from "next/navigation";

import { ErrorState } from "@/components/common/error-state";
import { PageLoading } from "@/components/common/page-loading";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { Button } from "@/components/ui/button";
import { PERMISSIONS } from "@/constants/permission.constants";
import { HskLevelForm } from "@/features/learning/components/hsk-level/hsk-level-form";
import { useHskLevelDetail } from "@/features/learning/hooks/use-hsk-levels";
import { PermissionGuard } from "@/security/permission-guard";

export default function EditHskLevelPage() {
  const params = useParams<{ id: string }>();
  const id = Number(params.id);
  const query = useHskLevelDetail(id);

  if (!Number.isSafeInteger(id) || id <= 0) {
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
          description="Không thể lấy dữ liệu cần chỉnh sửa từ backend."
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
          description="Cấp độ này không tồn tại hoặc đã bị xóa."
        />
      </PageContainer>
    );
  }

  const item = query.data;

  return (
    <PermissionGuard permission={PERMISSIONS.HSK_LEVELS.UPDATE}>
      <PageContainer>
        <PageHeader
          title={`Chỉnh sửa ${item.code}`}
          description={item.nameVi}
          actions={
            <Link href={`/cap-do-hsk/${item.id}`}>
              <Button variant="outline" className="h-[38px] gap-2 text-[11px]">
                <ArrowLeft size={14} />
                Về chi tiết
              </Button>
            </Link>
          }
        />

        <HskLevelForm item={item} />
      </PageContainer>
    </PermissionGuard>
  );
}
