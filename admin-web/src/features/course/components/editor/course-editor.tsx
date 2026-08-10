"use client";

import { useCourseEditor } from "../../hooks/use-course-editor";
import { CourseHeader } from "./course-header";
import { CourseOverviewTab } from "./course-overview-tab";
import { CourseCurriculumTab } from "./course-curriculum-tab";
import { CoursePrerequisitesTab } from "./course-prerequisites-tab";
import { CourseValidationPanel } from "./course-validation-panel";

interface Props {
  courseId: number;
}

export function CourseEditor({ courseId }: Props) {
  const editor = useCourseEditor(courseId);

  if (editor.loading) {
    return (
      <div className="space-y-4 p-6">
        <div className="h-24 animate-pulse rounded-xl bg-neutral-100" />
        <div className="h-96 animate-pulse rounded-xl bg-neutral-100" />
      </div>
    );
  }

  if (!editor.course) {
    return (
      <div className="rounded-xl border bg-white p-8 text-center">
        {editor.error ?? "Không tìm thấy khóa học."}
      </div>
    );
  }

  return (
    <div className="space-y-5">
      <CourseHeader editor={editor} />

      {editor.error && (
        <div
          role="alert"
          className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700"
        >
          {editor.error}
        </div>
      )}

      {editor.validation && <CourseValidationPanel result={editor.validation} />}

      <nav
        className="flex gap-1 rounded-xl border bg-white p-1"
        aria-label="Nội dung khóa học"
      >
        <TabButton
          active={editor.tab === "overview"}
          onClick={() => editor.setTab("overview")}
        >
          Tổng quan
        </TabButton>

        <TabButton
          active={editor.tab === "curriculum"}
          onClick={() => editor.setTab("curriculum")}
        >
          Nội dung
        </TabButton>

        <TabButton
          active={editor.tab === "prerequisites"}
          onClick={() => editor.setTab("prerequisites")}
        >
          Tiên quyết
        </TabButton>
      </nav>

      {editor.tab === "overview" && <CourseOverviewTab editor={editor} />}
      {editor.tab === "curriculum" && <CourseCurriculumTab editor={editor} />}
      {editor.tab === "prerequisites" && <CoursePrerequisitesTab editor={editor} />}
    </div>
  );
}

function TabButton({
  active,
  children,
  onClick,
}: {
  active: boolean;
  children: React.ReactNode;
  onClick: () => void;
}) {
  return (
    <button
      type="button"
      onClick={onClick}
      className={[
        "rounded-lg px-4 py-2 text-sm font-medium transition",
        active
          ? "bg-neutral-900 text-white"
          : "text-neutral-600 hover:bg-neutral-100",
      ].join(" ")}
    >
      {children}
    </button>
  );
}
