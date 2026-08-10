"use client";

import Link from "next/link";
import { useParams } from "next/navigation";
import { ArrowLeft } from "lucide-react";

import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { Button } from "@/components/ui/button";
import { PermissionGuard } from "@/security/permission-guard";
import { PERMISSIONS } from "@/constants/permission.constants";
import { CourseChapterTable } from "@/features/course/components/course-chapter-table";

export default function CourseChaptersPage() {
  const params = useParams<{ id: string }>();
  const courseId = Number(params.id);

  if (!Number.isSafeInteger(courseId) || courseId <= 0) {
    return (
      <PageContainer>
        <div className="rounded-[9px] border border-[#f1cbc8] bg-[#fff5f4] p-4 text-[12px] text-[#d9342d]">
          CourseId không hợp lệ.
        </div>
      </PageContainer>
    );
  }

  return (
    <PermissionGuard permission={PERMISSIONS.CHAPTERS.READ}>
      <PageContainer>
        <PageHeader
          title="Chương học"
          description="Dữ liệu được lấy từ endpoint chương học theo CourseId. ID nội bộ là long, PublicId là Guid."
          actions={
            <Link href={`/khoa-hoc/${courseId}`}>
              <Button variant="outline" className="h-[38px] gap-2 text-[11px]">
                <ArrowLeft size={14} />
                Quay lại khóa học
              </Button>
            </Link>
          }
        />
        <CourseChapterTable courseId={courseId} />
      </PageContainer>
    </PermissionGuard>
  );
}
