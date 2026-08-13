"use client";

import Link from "next/link";
import { useParams } from "next/navigation";
import { ArrowLeft, BookOpenText } from "lucide-react";

import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { Button } from "@/components/ui/button";
import { PERMISSIONS } from "@/constants/permission.constants";
import { LessonRelationsManager } from "@/features/lesson/components/lesson-relations-manager";
import { PermissionGuard } from "@/security/permission-guard";

export default function LessonResourceManagementPage() {
  const params = useParams<{ id: string }>();
  const lessonId = Number(params.id);

  if (!Number.isFinite(lessonId) || lessonId <= 0) {
    return (
      <PageContainer>
        <div className="rounded-lg border border-destructive/30 bg-destructive/5 p-6 text-[13px] text-destructive">
          ID bài giảng không hợp lệ.
        </div>
      </PageContainer>
    );
  }

  return (
    <PermissionGuard permission={PERMISSIONS.LESSONS.UPDATE}>
      <PageContainer>
        <PageHeader
          title="Liên kết Lesson"
          description="Quản lý từ vựng và bài học tiên quyết. Section và media được quản lý tại Content Editor."
          actions={
            <>
              <Link href={`/bai-giang/${lessonId}/noi-dung`}>
                <Button variant="outline" size="md" className="gap-2">
                  <BookOpenText size={14} /> Content Editor
                </Button>
              </Link>
              <Link href={`/bai-giang/${lessonId}`}>
                <Button variant="outline" size="md" className="gap-2">
                  <ArrowLeft size={14} /> Chi tiết
                </Button>
              </Link>
            </>
          }
        />

        <LessonRelationsManager lessonId={lessonId} />
      </PageContainer>
    </PermissionGuard>
  );
}
