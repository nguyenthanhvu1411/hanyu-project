"use client";

import { useState } from "react";

import { ContentStatus } from "@/lib/constants/content-status";
import { PERMISSIONS } from "@/constants/permission.constants";
import { PermissionGuard } from "@/security/permission-guard";
import { courseApi } from "../../api/course.api";
import type { CourseEditorController } from "../../hooks/use-course-editor";
import { Button } from "@/components/ui/button";
import { Dialog } from "@/components/ui/dialog";
import { Textarea } from "@/components/ui/textarea";
import { FormField } from "@/components/forms/form-field";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";

interface Props { editor: CourseEditorController; }

export function CourseWorkflowActions({ editor }: Props) {
  const course = editor.course!;
  const request = { concurrencyToken: course.concurrencyToken };
  const [rejectOpen, setRejectOpen] = useState(false);
  const [rejectReason, setRejectReason] = useState("");
  const [working, setWorking] = useState(false);

  async function execute(action: () => Promise<unknown>, successMessage: string) {
    try {
      setWorking(true);
      await action();
      await editor.refreshCourse();
      appToast.success(successMessage);
      return true;
    } catch (error) {
      appToast.error("Không thể cập nhật khóa học", normalizeApiError(error).message);
      return false;
    } finally {
      setWorking(false);
    }
  }

  async function rejectCourse() {
    const reason = rejectReason.trim();
    if (!reason) return;

    const success = await execute(
      () => courseApi.reject(course.id, { ...request, reason }),
      "Đã từ chối khóa học.",
    );

    if (success) {
      setRejectReason("");
      setRejectOpen(false);
    }
  }

  let action: React.ReactNode = null;

  switch (course.status) {
    case ContentStatus.Draft:
      action = (
        <PermissionGuard permission={PERMISSIONS.COURSES.SUBMIT_REVIEW} fallback={null}>
          <Button
            disabled={editor.saving || working}
            onClick={async () => {
              const validation = await editor.validateCourse();
              if (!validation?.isValid) {
                appToast.error("Khóa học chưa hợp lệ", "Hãy xử lý các lỗi kiểm tra trước khi gửi duyệt.");
                return;
              }
              await execute(() => courseApi.submitReview(course.id, request), "Đã gửi khóa học để duyệt.");
            }}
          >
            Gửi duyệt
          </Button>
        </PermissionGuard>
      );
      break;

    case ContentStatus.Review:
      action = (
        <div className="flex gap-2">
          <PermissionGuard permission={PERMISSIONS.COURSES.APPROVE} fallback={null}>
            <Button
              disabled={editor.saving || working}
              onClick={() => void execute(() => courseApi.approve(course.id, request), "Đã duyệt khóa học.")}
            >
              Duyệt
            </Button>
          </PermissionGuard>
          <PermissionGuard permission={PERMISSIONS.COURSES.REJECT} fallback={null}>
            <Button variant="outline" disabled={editor.saving || working} onClick={() => setRejectOpen(true)}>
              Từ chối
            </Button>
          </PermissionGuard>
        </div>
      );
      break;

    case ContentStatus.Approved:
      action = (
        <PermissionGuard permission={PERMISSIONS.COURSES.PUBLISH} fallback={null}>
          <Button
            disabled={editor.saving || working}
            onClick={() => void execute(() => courseApi.publish(course.id, request), "Đã xuất bản khóa học.")}
          >
            Xuất bản
          </Button>
        </PermissionGuard>
      );
      break;

    case ContentStatus.Published:
      action = (
        <PermissionGuard permission={PERMISSIONS.COURSES.ARCHIVE} fallback={null}>
          <Button
            variant="outline"
            disabled={editor.saving || working}
            onClick={() => void execute(() => courseApi.archive(course.id, request), "Đã lưu trữ khóa học.")}
          >
            Lưu trữ
          </Button>
        </PermissionGuard>
      );
      break;

    case ContentStatus.Archived:
      action = (
        <PermissionGuard permission={PERMISSIONS.COURSES.RESTORE} fallback={null}>
          <Button
            variant="outline"
            disabled={editor.saving || working}
            onClick={() => void execute(() => courseApi.restore(course.id, request), "Đã khôi phục khóa học về bản nháp.")}
          >
            Khôi phục Draft
          </Button>
        </PermissionGuard>
      );
      break;
  }

  return (
    <>
      {action}

      <Dialog
        open={rejectOpen}
        onOpenChange={(open) => {
          setRejectOpen(open);
          if (!open) setRejectReason("");
        }}
        title="Từ chối khóa học"
        description="Nhập lý do để biên tập viên biết nội dung cần chỉnh sửa trước khi gửi duyệt lại."
        size="sm"
        footer={
          <div className="flex justify-end gap-2">
            <Button variant="outline" onClick={() => setRejectOpen(false)} disabled={working}>
              Hủy
            </Button>
            <Button onClick={() => void rejectCourse()} disabled={working || !rejectReason.trim()}>
              Xác nhận từ chối
            </Button>
          </div>
        }
      >
        <FormField label="Lý do từ chối" required>
          <Textarea
            value={rejectReason}
            onChange={(event) => setRejectReason(event.target.value)}
            rows={5}
            maxLength={1000}
            placeholder="Ví dụ: Cần bổ sung chương học và kiểm tra lại mô tả khóa học..."
          />
        </FormField>
      </Dialog>
    </>
  );
}
