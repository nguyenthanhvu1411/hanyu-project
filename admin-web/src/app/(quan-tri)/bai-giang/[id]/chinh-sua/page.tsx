"use client";

import { useParams } from "next/navigation";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { ErrorState } from "@/components/common/error-state";
import { PermissionGuard } from "@/security/permission-guard";
import { PERMISSIONS } from "@/constants/permission.constants";
import { LessonEditor } from "@/features/lesson/components/lesson-editor";
import { LessonContentManager } from "@/features/lesson/components/lesson-content-manager";
import { LessonValidationPanel } from "@/features/lesson/components/lesson-validation-panel";

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
          title="Biên tập bài giảng"
          description="Quản lý thông tin chung, nội dung bài học, từ vựng, tài nguyên, điều kiện tiên quyết và kiểm tra workflow trong cùng một workspace."
        />

        <div className="space-y-6">
          <LessonEditor lessonId={lessonId} />

          <section className="space-y-3">
            <div>
              <h2 className="text-[14px] font-semibold text-[#333]">Nội dung bài giảng</h2>
              <p className="mt-1 text-[11px] leading-5 text-[#777]">
                Quản lý LessonSection, Vocabulary, Asset và LessonPrerequisite. Các quy tắc trùng thứ tự,
                self-reference và prerequisite cycle tiếp tục được backend kiểm tra.
              </p>
            </div>

            <LessonContentManager lessonId={lessonId} />
          </section>

          <section className="space-y-3">
            <div>
              <h2 className="text-[14px] font-semibold text-[#333]">Workflow Validation</h2>
              <p className="mt-1 text-[11px] leading-5 text-[#777]">
                Kiểm tra các điều kiện bắt buộc trước khi chuyển Lesson sang Review hoặc Published.
              </p>
            </div>

            <LessonValidationPanel lessonId={lessonId} />
          </section>
        </div>
      </PageContainer>
    </PermissionGuard>
  );
}
