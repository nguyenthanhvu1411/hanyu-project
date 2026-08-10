"use client";

import { useParams } from "next/navigation";

import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { CourseForm } from "@/features/course/components/course-form";
import { PERMISSIONS } from "@/constants/permission.constants";
import { PermissionGuard } from "@/security/permission-guard";

export default function EditCoursePage() {
  const params = useParams<{ id: string }>();
  const courseId = Number(params.id);

  if (!Number.isSafeInteger(courseId) || courseId <= 0) {
    return (
      <PageContainer>
        <div className="rounded-[11px] border border-[#f2c3bf] bg-[#fff3f1] p-5 text-[12px] text-[#c93831]">
          ID khóa học không hợp lệ.
        </div>
      </PageContainer>
    );
  }

  return (
    <PermissionGuard permission={PERMISSIONS.COURSES.UPDATE}>
      <PageContainer>
        <PageHeader
          title="Chỉnh sửa khóa học"
          description="Cập nhật metadata khóa học và giữ concurrency token theo backend."
        />
        <CourseForm courseId={courseId} />
      </PageContainer>
    </PermissionGuard>
  );
}
