"use client";

import { ContentStatus } from "@/lib/constants/content-status";
import { PERMISSIONS } from "@/constants/permission.constants";
import { PermissionGuard } from "@/security/permission-guard";
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
      // API errors are surfaced by the shared API/editor error flow.
    }
  }

  switch (course.status) {
    case ContentStatus.Draft:
      return (
        <PermissionGuard permission={PERMISSIONS.COURSES.SUBMIT_REVIEW} fallback={null}>
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
        </PermissionGuard>
      );

    case ContentStatus.Review:
      return (
        <div className="flex gap-2">
          <PermissionGuard permission={PERMISSIONS.COURSES.APPROVE} fallback={null}>
            <Button disabled={editor.saving} onClick={() => void execute(() => courseApi.approve(course.id, request))}>
              Duyệt
            </Button>
          </PermissionGuard>
          <PermissionGuard permission={PERMISSIONS.COURSES.REJECT} fallback={null}>
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
          </PermissionGuard>
        </div>
      );

    case ContentStatus.Approved:
      return (
        <PermissionGuard permission={PERMISSIONS.COURSES.PUBLISH} fallback={null}>
          <Button disabled={editor.saving} onClick={() => void execute(() => courseApi.publish(course.id, request))}>
            Xuất bản
          </Button>
        </PermissionGuard>
      );

    case ContentStatus.Published:
      return (
        <PermissionGuard permission={PERMISSIONS.COURSES.ARCHIVE} fallback={null}>
          <Button variant="outline" disabled={editor.saving} onClick={() => void execute(() => courseApi.archive(course.id, request))}>
            Lưu trữ
          </Button>
        </PermissionGuard>
      );

    case ContentStatus.Archived:
      return (
        <PermissionGuard permission={PERMISSIONS.COURSES.RESTORE} fallback={null}>
          <Button variant="outline" disabled={editor.saving} onClick={() => void execute(() => courseApi.restore(course.id, request))}>
            Khôi phục Draft
          </Button>
        </PermissionGuard>
      );

    default:
      return null;
  }
}
