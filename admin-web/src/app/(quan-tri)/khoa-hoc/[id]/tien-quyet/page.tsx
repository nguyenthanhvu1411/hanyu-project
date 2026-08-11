"use client";

import { useParams } from "next/navigation";

import { ErrorState } from "@/components/common/error-state";
import { Skeleton } from "@/components/ui/skeleton";
import { CoursePrerequisitesTab } from "@/features/course/components/editor/course-prerequisites-tab";
import { useCourseEditor } from "@/features/course/hooks/use-course-editor";

export default function CoursePrerequisitesPage() {
  const params = useParams<{ id: string }>();
  const courseId = Number(params.id);
  const editor = useCourseEditor(courseId);

  if (!Number.isSafeInteger(courseId) || courseId <= 0) {
    return <ErrorState title="Khóa học không hợp lệ" description="CourseId phải là số nguyên dương." />;
  }

  if (editor.loading) {
    return <Skeleton className="h-72 w-full rounded-[11px]" />;
  }

  if (!editor.course) {
    return <ErrorState title="Không thể tải khóa học" description={editor.error ?? "Không tìm thấy khóa học."} onRetry={() => void editor.reload()} />;
  }

  return <CoursePrerequisitesTab editor={editor} />;
}
