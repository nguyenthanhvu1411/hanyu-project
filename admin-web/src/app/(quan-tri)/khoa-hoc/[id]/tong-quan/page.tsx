"use client";

import { useParams } from "next/navigation";
import { ShieldCheck } from "lucide-react";

import { ErrorState } from "@/components/common/error-state";
import { FormSection } from "@/components/forms/form-section";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { CourseOverviewTab } from "@/features/course/components/editor/course-overview-tab";
import { CourseValidationPanel } from "@/features/course/components/editor/course-validation-panel";
import { CourseWorkflowActions } from "@/features/course/components/editor/course-workflow-actions";
import { useCourseEditor } from "@/features/course/hooks/use-course-editor";

export default function CourseOverviewPage() {
  const params = useParams<{ id: string }>();
  const courseId = Number(params.id);
  const editor = useCourseEditor(courseId);

  if (!Number.isSafeInteger(courseId) || courseId <= 0) {
    return <ErrorState title="Khóa học không hợp lệ" description="CourseId phải là số nguyên dương." />;
  }

  if (editor.loading) {
    return <div className="space-y-4"><Skeleton className="h-72 w-full rounded-[11px]" /><Skeleton className="h-40 w-full rounded-[11px]" /></div>;
  }

  if (!editor.course) {
    return <ErrorState title="Không thể tải khóa học" description={editor.error ?? "Không tìm thấy khóa học."} onRetry={() => void editor.reload()} />;
  }

  return (
    <div className="space-y-4">
      {editor.error ? <ErrorState title="Thao tác khóa học thất bại" description={editor.error} /> : null}
      {editor.validation ? <CourseValidationPanel result={editor.validation} /> : null}

      <CourseOverviewTab editor={editor} />

      <FormSection
        title="Quy trình biên tập"
        description="Kiểm tra dữ liệu trước khi gửi duyệt. Người tạo khóa học không được tự duyệt chính khóa học mình tạo."
        icon={<ShieldCheck size={18} />}
      >
        <div className="flex flex-wrap items-center gap-2">
          <Button
            type="button"
            variant="outline"
            onClick={() => void editor.validateCourse()}
            disabled={editor.saving}
          >
            Kiểm tra hợp lệ
          </Button>
          <CourseWorkflowActions editor={editor} />
        </div>
      </FormSection>
    </div>
  );
}
