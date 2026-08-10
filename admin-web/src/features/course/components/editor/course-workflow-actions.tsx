"use client";

import { ContentStatus } from "@/lib/constants/content-status";
import { khoaHocApi } from "../../api/khoa-hoc.api";
import type { CourseEditorController } from "../../hooks/use-course-editor";
import { Button } from "@/components/ui/button";

interface Props {
  editor: CourseEditorController;
}

export function CourseWorkflowActions({ editor }: Props) {
  const course = editor.course!;
  const request = {
    concurrencyToken: course.concurrencyToken,
  };

  async function execute(action: () => Promise<unknown>) {
    try {
      await action();
      await editor.refreshCourse();
    } catch {
      /* useCourseEditor error handler will catch this */
    }
  }

  switch (course.status) {
    case ContentStatus.Draft:
      return (
        <Button
          variant="danger"
          disabled={editor.saving}
          onClick={async () => {
            const validation = await editor.validateCourse();
            if (!validation || !validation.isValid) {
              return;
            }
            await execute(() => khoaHocApi.guiDuyet(course.id, request));
          }}
        >
          Gửi duyệt
        </Button>
      );

    case ContentStatus.Review:
      return (
        <div className="flex gap-2">
          <Button
            className="bg-emerald-600 hover:bg-emerald-700 text-white"
            disabled={editor.saving}
            onClick={() => void execute(() => khoaHocApi.duyet(course.id, request))}
          >
            Duyệt
          </Button>

          <Button
            variant="outline"
            className="border-red-200 text-red-700 hover:bg-red-50"
            disabled={editor.saving}
            onClick={async () => {
              const reason = window.prompt("Lý do từ chối:");
              if (!reason?.trim()) return;
              await execute(() =>
                khoaHocApi.tuChoi(course.id, {
                  ...request,
                  reason: reason.trim(),
                })
              );
            }}
          >
            Từ chối
          </Button>
        </div>
      );

    case ContentStatus.Approved:
      return (
        <Button
          variant="danger"
          disabled={editor.saving}
          onClick={() => void execute(() => khoaHocApi.xuatBan(course.id, request))}
        >
          Xuất bản
        </Button>
      );

    case ContentStatus.Published:
      return (
        <Button
          variant="outline"
          disabled={editor.saving}
          onClick={() => void execute(() => khoaHocApi.luuTru(course.id, request))}
        >
          Lưu trữ
        </Button>
      );

    case ContentStatus.Archived:
      return (
        <Button
          variant="outline"
          disabled={editor.saving}
          onClick={() => void execute(() => khoaHocApi.khoiPhuc(course.id, request))}
        >
          Khôi phục Draft
        </Button>
      );

    default:
      return null;
  }
}
