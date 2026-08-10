"use client";

import { ContentStatus } from "@/lib/constants/content-status";
import { courseApi } from "../../api/course.api";
import type { CourseEditorController } from "../../hooks/use-course-editor";
import { Button } from "@/components/ui/button";

interface Props { editor: CourseEditorController; }

export function CourseWorkflowActions({ editor }: Props) {
  const course = editor.course!;
  const request = { concurrencyToken: course.concurrencyToken };

  async function execute(action: () => Promise<unknown>) {
    try {
      await action();
      await editor.refreshCourse();
    } catch {
      // API error is surfaced by the surrounding editor state/toast flow.
    }
  }

  switch (course.status) {
    case ContentStatus.Draft:
      return (
        <Button
          disabled={editor.saving}
          onClick={async () => {
            const validation = await editor.validateCourse();
            if (!validation?.isValid) return;
            await execute(() => courseApi.submitReview(course.id, request));
          }}
        >
          Gửi duyệt
        </Button>
      );

    case ContentStatus.Review:
      return (
        <div className="flex gap-2">
          <Button disabled={editor.saving} onClick={() => void execute(() => courseApi.approve(course.id, request))}>
            Duyệt
          </Button>
          <Button
            variant="outline"
            disabled={editor.saving}
            onClick={async () => {
              const reason = window.prompt("Lý do từ chối:");
              if (!reason?.trim()) return;
              await execute(() => courseApi.reject(course.id, { ...request, reason: reason.trim() }));
            }}
          >
            Từ chối
          </Button>
        </div>
      );

    case ContentStatus.Approved:
      return <Button disabled={editor.saving} onClick={() => void execute(() => courseApi.publish(course.id, request))}>Xuất bản</Button>;
    case ContentStatus.Published:
      return <Button variant="outline" disabled={editor.saving} onClick={() => void execute(() => courseApi.archive(course.id, request))}>Lưu trữ</Button>;
    case ContentStatus.Archived:
      return <Button variant="outline" disabled={editor.saving} onClick={() => void execute(() => courseApi.restore(course.id, request))}>Khôi phục Draft</Button>;
    default:
      return null;
  }
}
