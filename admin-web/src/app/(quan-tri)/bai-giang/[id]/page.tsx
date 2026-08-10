"use client";

import { useParams } from "next/navigation";

import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PermissionGuard } from "@/security/permission-guard";
import { PERMISSIONS } from "@/constants/permission.constants";
import { LessonEditor } from "@/features/bai-giang/components/lesson-editor";

export default function LessonDetailPage() {
  const params = useParams<{ id: string }>();
  const lessonId = Number(params.id);

  return (
    <PermissionGuard permission={PERMISSIONS.LESSONS.READ}>
      <PageContainer>
        <PageHeader
          title="Chi tiết bài giảng"
          description="Chỉnh sửa thông tin và quản lý quy trình duyệt, xuất bản bài giảng."
        />
        <LessonEditor lessonId={lessonId} />
      </PageContainer>
    </PermissionGuard>
  );
}
