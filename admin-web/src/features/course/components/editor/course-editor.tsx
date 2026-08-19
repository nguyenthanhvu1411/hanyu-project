"use client";

import { ErrorState } from "@/components/common/error-state";
import { Skeleton } from "@/components/ui/skeleton";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { useCourseEditor } from "../../hooks/use-course-editor";
import { CourseHeader } from "./course-header";
import { CourseOverviewTab } from "./course-overview-tab";
import { CourseCurriculumTab } from "./course-curriculum-tab";
import { CoursePrerequisitesTab } from "./course-prerequisites-tab";
import { CourseValidationPanel } from "./course-validation-panel";

interface Props { courseId: number; }

export function CourseEditor({ courseId }: Props) {
  const editor = useCourseEditor(courseId);

  if (editor.loading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-28 w-full rounded-[11px]" />
        <Skeleton className="h-12 w-full rounded-[11px]" />
        <Skeleton className="h-80 w-full rounded-[11px]" />
      </div>
    );
  }

  if (!editor.course) {
    return (
      <ErrorState
        title="Không thể tải khóa học"
        description={editor.error ?? "Không tìm thấy khóa học."}
        onRetry={() => void editor.reload()}
      />
    );
  }

  return (
    <div className="space-y-5">
      <CourseHeader editor={editor} />
      {editor.error ? (
        <ErrorState title="Thao tác khóa học thất bại" description={editor.error} />
      ) : null}
      {editor.validation ? <CourseValidationPanel result={editor.validation} /> : null}

      <Tabs value={editor.tab} onValueChange={(value) => editor.setTab(value as typeof editor.tab)}>
        <TabsList className="rounded-[11px] border border-[#e8e3dc] bg-white px-2">
          <TabsTrigger value="overview">Tổng quan</TabsTrigger>
          <TabsTrigger value="curriculum">Nội dung</TabsTrigger>
          <TabsTrigger value="prerequisites">Tiên quyết</TabsTrigger>
        </TabsList>
      </Tabs>

      {editor.tab === "overview" ? <CourseOverviewTab editor={editor} /> : null}
      {editor.tab === "curriculum" ? <CourseCurriculumTab editor={editor} /> : null}
      {editor.tab === "prerequisites" ? <CoursePrerequisitesTab editor={editor} /> : null}
    </div>
  );
}
