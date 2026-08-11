"use client";

import { useParams } from "next/navigation";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { ErrorState } from "@/components/common/error-state";
import { PermissionGuard } from "@/security/permission-guard";
import { PERMISSIONS } from "@/constants/permission.constants";
import { LessonEditor } from "@/features/lesson/components/lesson-editor";

export default function EditLessonPage() {
  const params = useParams<{ id: string }>();
  const lessonId = Number(params.id);

  if (!Number.isSafeInteger(lessonId) || lessonId <= 0) {
    return (
      <PageContainer>
        <ErrorState
          title="Bài giảng không hợp lệ"
          description="ID bài giảng không đúng định dạng."
        />
      </PageContainer>
    );
  }

  return (
    <PermissionGuard permission={PERMISSIONS.LESSONS.UPDATE}>
      <PageContainer>
        <PageHeader
          title="Chỉnh sửa bài giảng"
          description="Cập nhật thông tin bài giảng, ảnh bìa và metadata học tập."
        />
        <LessonEditor lessonId={lessonId} />
      </PageContainer>
    </PermissionGuard>
  );
}
