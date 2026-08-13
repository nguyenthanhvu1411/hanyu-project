"use client";

import { useParams } from "next/navigation";
import { ErrorState } from "@/components/common/error-state";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PERMISSIONS } from "@/constants/permission.constants";
import { LessonEditor } from "@/features/lesson/components/lesson-editor";
import { PermissionGuard } from "@/security/permission-guard";

export default function EditLessonPage() {
  const params = useParams<{ id: string }>();
  const lessonId = Number(params.id);

  if (!Number.isSafeInteger(lessonId) || lessonId <= 0) {
    return <PageContainer><ErrorState title="Bài giảng không hợp lệ" description="ID bài giảng không đúng định dạng." /></PageContainer>;
  }

  return (
    <PermissionGuard permission={PERMISSIONS.LESSONS.UPDATE}>
      <PageContainer>
        <PageHeader
          title="Metadata & Workflow"
          description="Chỉnh thông tin bài giảng và quản lý Review / Approve / Publish. Nội dung, media và liên kết được quản lý ở workspace riêng."
        />
        <LessonEditor lessonId={lessonId} />
      </PageContainer>
    </PermissionGuard>
  );
}
