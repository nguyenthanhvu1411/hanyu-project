"use client";

import Link from "next/link";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import type { CourseEditorController } from "../../hooks/use-course-editor";
import { CourseWorkflowActions } from "./course-workflow-actions";
import { getContentStatusLabel } from "@/lib/constants/content-status";

interface Props {
  editor: CourseEditorController;
}

export function CourseHeader({ editor }: Props) {
  const course = editor.course!;

  return (
    <section className="rounded-xl border bg-white p-5">
      <div className="flex flex-col justify-between gap-4 xl:flex-row xl:items-start">
        <div className="min-w-0">
          <div className="mb-2 flex flex-wrap items-center gap-2">
            <Badge variant="default">{course.code}</Badge>

            {course.hskCode && (
              <Badge variant="danger">{course.hskCode}</Badge>
            )}

            <Badge variant="default">{getContentStatusLabel(course.status)}</Badge>

            {course.isFeatured && (
              <Badge variant="warning" className="bg-amber-100 text-amber-700 hover:bg-amber-100 border-none">
                Nổi bật
              </Badge>
            )}
          </div>

          <h1 className="truncate text-2xl font-semibold">{course.titleVi}</h1>

          <p className="mt-1 text-sm text-neutral-500">/{course.slug}</p>

          <div className="mt-4 flex flex-wrap gap-5 text-sm">
            <Metric label="Chương" value={editor.chapters.length} />
            <Metric label="Bài học" value={editor.lessonCount} />
            <Metric
              label="Thời lượng"
              value={course.estimatedMinutes ? `${course.estimatedMinutes} phút` : "—"}
            />
          </div>
        </div>

        <div className="flex flex-wrap justify-end gap-2">
          {editor.canEdit && (
            <Link href={`/khoa-hoc/${course.id}/chinh-sua`}>
              <Button variant="outline">Chỉnh sửa</Button>
            </Link>
          )}

          <Button
            variant="outline"
            onClick={() => void editor.validateCourse()}
            disabled={editor.saving}
          >
            Kiểm tra
          </Button>

          <CourseWorkflowActions editor={editor} />
        </div>
      </div>
    </section>
  );
}

function Metric({ label, value }: { label: string; value: React.ReactNode }) {
  return (
    <div>
      <div className="text-xs text-neutral-500">{label}</div>
      <div className="mt-1 font-semibold">{value}</div>
    </div>
  );
}
