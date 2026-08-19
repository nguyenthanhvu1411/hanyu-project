"use client";

import Link from "next/link";
import { useCallback, useEffect, useState } from "react";
import { useRouter } from "next/navigation";

import { ErrorState } from "@/components/common/error-state";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { appToast } from "@/components/ui/toast";
import { PERMISSIONS } from "@/constants/permission.constants";
import { normalizeApiError } from "@/lib/api/api-error";
import { PermissionGuard } from "@/security/permission-guard";

import { quizApi } from "../quiz.api";
import {
  AdminQuiz,
  ContentStatus,
  QUIZ_STATUS_LABELS,
  QUIZ_TYPE_LABELS,
} from "../quiz.types";

interface QuizDetailProps {
  quizId: number;
}

function statusVariant(status: ContentStatus): "default" | "success" | "warning" | "info" {
  if (status === ContentStatus.Published) return "success";
  if (status === ContentStatus.Review) return "warning";
  if (status === ContentStatus.Approved) return "info";
  return "default";
}

export function QuizDetail({ quizId }: QuizDetailProps) {
  const router = useRouter();
  const [quiz, setQuiz] = useState<AdminQuiz | null>(null);
  const [loading, setLoading] = useState(true);
  const [working, setWorking] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setQuiz(await quizApi.getById(quizId));
    } catch (caught) {
      setError(caught instanceof Error ? caught : new Error("Không thể tải bài kiểm tra."));
    } finally {
      setLoading(false);
    }
  }, [quizId]);

  useEffect(() => { void load(); }, [load]);

  async function run(label: string, action: () => Promise<void>) {
    if (working) return;
    setWorking(true);
    try {
      await action();
      appToast.success(label);
      await load();
    } catch (caught) {
      appToast.error("Không thể cập nhật workflow", normalizeApiError(caught).message);
    } finally {
      setWorking(false);
    }
  }

  if (loading) {
    return <div className="rounded-[11px] border border-[#e8e3dc] bg-white p-6 text-[12px] text-muted-foreground">Đang tải bài kiểm tra...</div>;
  }

  if (error || !quiz) {
    return <ErrorState title="Không thể tải bài kiểm tra" description={error?.message ?? "Không tìm thấy dữ liệu."} onRetry={() => void load()} />;
  }

  return (
    <div className="space-y-5">
      <Card>
        <CardHeader className="flex flex-row items-start justify-between gap-4">
          <div>
            <CardTitle>{quiz.titleVi}</CardTitle>
            <p className="mt-1 text-[11px] text-muted-foreground">{QUIZ_TYPE_LABELS[quiz.quizType]} · Version {quiz.version}</p>
          </div>
          <Badge variant={statusVariant(quiz.status)}>{QUIZ_STATUS_LABELS[quiz.status]}</Badge>
        </CardHeader>
        <CardContent className="space-y-4">
          <p className="text-[12px] leading-6 text-[#555]">{quiz.descriptionVi || "Chưa có mô tả."}</p>
          <div className="grid gap-3 md:grid-cols-2 lg:grid-cols-4">
            <div className="rounded-md border p-3"><div className="text-[10px] text-muted-foreground">Lesson</div><div className="mt-1 text-[12px] font-medium">{quiz.lessonTitleVi ?? "Không gắn bài giảng"}</div></div>
            <div className="rounded-md border p-3"><div className="text-[10px] text-muted-foreground">Điểm đạt</div><div className="mt-1 text-[12px] font-medium">{quiz.passingScore}%</div></div>
            <div className="rounded-md border p-3"><div className="text-[10px] text-muted-foreground">Thời gian</div><div className="mt-1 text-[12px] font-medium">{quiz.timeLimitSeconds ? `${Math.ceil(quiz.timeLimitSeconds / 60)} phút` : "Không giới hạn"}</div></div>
            <div className="rounded-md border p-3"><div className="text-[10px] text-muted-foreground">Số lượt tối đa</div><div className="mt-1 text-[12px] font-medium">{quiz.maxAttempts}</div></div>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader><CardTitle>Quản lý bài kiểm tra</CardTitle></CardHeader>
        <CardContent className="flex flex-wrap gap-2">
          <PermissionGuard permission={PERMISSIONS.QUIZZES.UPDATE} fallback={null}>
            <Link href={`/bai-kiem-tra/${quiz.id}/chinh-sua`}><Button variant="outline">Chỉnh sửa</Button></Link>
          </PermissionGuard>
          <Link href={`/bai-kiem-tra/${quiz.id}/cau-hoi`}><Button variant="outline">Quản lý câu hỏi</Button></Link>

          {quiz.status === ContentStatus.Draft ? (
            <Button disabled={working} onClick={() => void run("Đã gửi bài kiểm tra chờ duyệt.", () => quizApi.submitReview(quiz.id))}>Gửi duyệt</Button>
          ) : null}
          {quiz.status === ContentStatus.Review ? (
            <Button disabled={working} onClick={() => void run("Đã duyệt bài kiểm tra.", () => quizApi.approve(quiz.id))}>Duyệt</Button>
          ) : null}
          {quiz.status === ContentStatus.Approved ? (
            <PermissionGuard permission={PERMISSIONS.QUIZZES.PUBLISH} fallback={null}>
              <Button disabled={working} onClick={() => void run("Đã xuất bản bài kiểm tra.", () => quizApi.publish(quiz.id))}>Xuất bản</Button>
            </PermissionGuard>
          ) : null}
          {quiz.status === ContentStatus.Published ? (
            <Button variant="outline" disabled={working} onClick={() => void run("Đã lưu trữ bài kiểm tra.", () => quizApi.archive(quiz.id))}>Lưu trữ</Button>
          ) : null}
          {quiz.status === ContentStatus.Archived ? (
            <PermissionGuard permission={PERMISSIONS.QUIZZES.RESTORE} fallback={null}>
              <Button disabled={working} onClick={() => void run("Đã khôi phục bài kiểm tra về Draft.", () => quizApi.restore(quiz.id))}>Khôi phục</Button>
            </PermissionGuard>
          ) : null}
        </CardContent>
      </Card>

      <div className="flex justify-end">
        <Button variant="outline" onClick={() => router.push("/bai-kiem-tra")}>Quay lại danh sách</Button>
      </div>
    </div>
  );
}
