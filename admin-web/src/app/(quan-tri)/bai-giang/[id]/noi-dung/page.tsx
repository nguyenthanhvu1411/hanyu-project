"use client";

import Link from "next/link";
import { useParams } from "next/navigation";
import { ArrowLeft, Boxes, Pencil } from "lucide-react";

import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { Button } from "@/components/ui/button";
import { PERMISSIONS } from "@/constants/permission.constants";
import { LessonSectionStudio } from "@/features/lesson/components/lesson-section-studio";
import { LessonValidationPanel } from "@/features/lesson/components/lesson-validation-panel";
import { PermissionGuard } from "@/security/permission-guard";

export default function LessonContentPage() {
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
          title="Lesson Content Editor"
          description="Workspace duy nhất để quản lý LessonSection, kho media Lesson và media theo từng section bằng modal workflow."
          actions={
            <>
              <Link href={`/bai-giang/${lessonId}`}>
                <Button variant="outline" size="md" className="gap-2">
                  <ArrowLeft size={14} /> Chi tiết
                </Button>
              </Link>
              <Link href={`/bai-giang/${lessonId}/noi-dung/quan-ly`}>
                <Button variant="outline" size="md" className="gap-2">
                  <Boxes size={14} /> Từ vựng & tiên quyết
                </Button>
              </Link>
              <Link href={`/bai-giang/${lessonId}/chinh-sua`}>
                <Button variant="outline" size="md" className="gap-2">
                  <Pencil size={14} /> Metadata
                </Button>
              </Link>
            </>
          }
        />

        <div className="space-y-5">
          <LessonSectionStudio lessonId={lessonId} />
          <LessonValidationPanel lessonId={lessonId} />
        </div>
      </PageContainer>
    </PermissionGuard>
  );
}
