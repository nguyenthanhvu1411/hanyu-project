"use client";

import Link from "next/link";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import type { CourseEditorController } from "../../hooks/use-course-editor";
import { CourseWorkflowActions } from "./course-workflow-actions";
import { getContentStatusLabel } from "@/lib/constants/content-status";

interface Props { editor: CourseEditorController; }

export function CourseHeader({ editor }: Props) {
  const course = editor.course!;

  return (
    <Card>
      <CardContent className="p-5">
        <div className="flex flex-col justify-between gap-4 xl:flex-row xl:items-start">
          <div className="min-w-0">
            <div className="mb-2 flex flex-wrap items-center gap-2">
              <Badge>{course.code}</Badge>
              {course.hskCode ? <Badge variant="primary">{course.hskCode}</Badge> : null}
              <Badge variant="info">{getContentStatusLabel(course.status)}</Badge>
              {course.isFeatured ? <Badge variant="warning">Nổi bật</Badge> : null}
            </div>
            <h1 className="truncate text-[20px] font-semibold text-[#292929]">{course.titleVi}</h1>
            <p className="mt-1 text-[11px] text-[#888]">/{course.slug}</p>
            <div className="mt-4 flex flex-wrap gap-5">
              <Metric label="Chương" value={editor.chapters.length} />
              <Metric label="Bài học" value={editor.lessonCount} />
              <Metric label="Thời lượng" value={course.estimatedMinutes ? `${course.estimatedMinutes} phút` : "—"} />
              <Metric label="Public ID" value={`${course.publicId.slice(0, 8)}…`} />
            </div>
          </div>

          <div className="flex flex-wrap justify-end gap-2">
            {editor.canEdit ? (
              <Link href={`/khoa-hoc/${course.id}/chinh-sua`}>
                <Button variant="outline">Chỉnh sửa</Button>
              </Link>
            ) : null}
            <Button variant="outline" onClick={() => void editor.validateCourse()} disabled={editor.saving}>
              Kiểm tra
            </Button>
            <CourseWorkflowActions editor={editor} />
          </div>
        </div>
      </CardContent>
    </Card>
  );
}

function Metric({ label, value }: { label: string; value: React.ReactNode }) {
  return (
    <div>
      <div className="text-[10px] text-[#8a8a8a]">{label}</div>
      <div className="mt-1 text-[12px] font-semibold text-[#333]">{value}</div>
    </div>
  );
}
