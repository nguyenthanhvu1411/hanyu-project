"use client";

import Link from "next/link";
import { useParams } from "next/navigation";
import { ArrowLeft, Pencil } from "lucide-react";

import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { Button } from "@/components/ui/button";
import { PermissionGuard } from "@/security/permission-guard";
import { PERMISSIONS } from "@/constants/permission.constants";
import { LessonContentManager } from "@/features/bai-giang/components/lesson-content-manager";

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
          title="Nội dung bài giảng"
          description="Quản lý section, từ vựng, tài nguyên và bài học tiên quyết của Lesson."
          actions={
            <>
              <Link href={`/bai-giang/${lessonId}`}>
                <Button variant="outline" className="h-[38px] gap-2 text-[11px]">
                  <ArrowLeft size={14} /> Chi tiết
                </Button>
              </Link>
              <Link href={`/bai-giang/${lessonId}/chinh-sua`}>
                <Button variant="outline" className="h-[38px] gap-2 text-[11px]">
                  <Pencil size={14} /> Metadata
                </Button>
              </Link>
            </>
          }
        />

        <LessonContentManager lessonId={lessonId} />
      </PageContainer>
    </PermissionGuard>
  );
}
