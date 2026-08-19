"use client";

import { useState, type MouseEvent, type ReactNode } from "react";
import { toast } from "sonner";

import { ConfirmDialog } from "@/components/ui/confirm-dialog";

import { lessonApi } from "../api/lesson.api";

type GuardAction = "publish" | "archive";

interface LessonEditorWorkflowGuardProps {
  lessonId: number;
  children: ReactNode;
  onCompleted?: () => void;
}

export function LessonEditorWorkflowGuard({
  lessonId,
  children,
  onCompleted,
}: LessonEditorWorkflowGuardProps) {
  const [pending, setPending] = useState<GuardAction | null>(null);
  const [loading, setLoading] = useState(false);

  function intercept(event: MouseEvent<HTMLDivElement>) {
    const target = event.target as HTMLElement;
    const button = target.closest("button");
    if (!button || button.disabled) return;

    const label = button.textContent?.trim().toLowerCase() ?? "";
    const action = label.includes("xuất bản")
      ? "publish"
      : label.includes("lưu trữ")
        ? "archive"
        : null;

    if (!action) return;

    event.preventDefault();
    event.stopPropagation();
    setPending(action);
  }

  async function confirm() {
    if (!pending || loading) return;

    setLoading(true);
    try {
      const detail = await lessonApi.getById(lessonId);
      const request = { version: detail.version };

      if (pending === "publish") {
        await lessonApi.publish(lessonId, request);
        toast.success("Đã xuất bản bài giảng.");
      } else {
        await lessonApi.archive(lessonId, request);
        toast.success("Đã lưu trữ bài giảng.");
      }

      setPending(null);
      onCompleted?.();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể cập nhật trạng thái bài giảng.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <>
      <div onClickCapture={intercept}>{children}</div>

      <ConfirmDialog
        open={pending === "publish"}
        title="Xuất bản bài giảng?"
        description="Bài giảng sẽ chuyển sang Published. Backend sẽ kiểm tra lại toàn bộ điều kiện workflow và từ chối nếu nội dung chưa hợp lệ."
        confirmLabel="Xuất bản"
        destructive={false}
        loading={loading}
        onClose={() => setPending(null)}
        onConfirm={confirm}
      />

      <ConfirmDialog
        open={pending === "archive"}
        title="Lưu trữ bài giảng?"
        description="Bài giảng Published sẽ chuyển sang Archived và không còn nằm trong luồng nội dung đang hoạt động."
        confirmLabel="Lưu trữ"
        loading={loading}
        onClose={() => setPending(null)}
        onConfirm={confirm}
      />
    </>
  );
}
