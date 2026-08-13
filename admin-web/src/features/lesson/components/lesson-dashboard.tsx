"use client";

import Link from "next/link";
import { AlertCircle, BookOpenText, Boxes, Clock3, FileText, GraduationCap, Link2, Pencil, RefreshCw } from "lucide-react";
import { useCallback, useEffect, useState } from "react";

import { Alert } from "@/components/ui/alert";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { getContentStatusLabel } from "@/lib/constants/content-status";

import { lessonApi } from "../api/lesson.api";
import type { AdminLessonDetail } from "../types/lesson.types";
import { LessonValidationPanel } from "./lesson-validation-panel";

export function LessonDashboard({ lessonId }: { lessonId: number }) {
  const [lesson, setLesson] = useState<AdminLessonDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setLesson(await lessonApi.getById(lessonId));
    } catch (cause) {
      setLesson(null);
      setError(cause instanceof Error ? cause.message : "Không thể tải bài giảng.");
    } finally {
      setLoading(false);
    }
  }, [lessonId]);

  useEffect(() => {
    void load();
  }, [load]);

  if (loading && !lesson) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-[190px] rounded-[11px]" />
        <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
          {Array.from({ length: 4 }).map((_, index) => (
            <Skeleton key={index} className="h-[94px] rounded-[11px]" />
          ))}
        </div>
        <Skeleton className="h-[260px] rounded-[11px]" />
      </div>
    );
  }

  if (!lesson) {
    return (
      <Card>
        <CardContent className="space-y-4 p-6">
          <Alert variant="danger" title="Không thể tải bài giảng">
            {error || "Bài giảng không tồn tại hoặc bạn không có quyền truy cập."}
          </Alert>
          <div className="flex flex-wrap gap-2">
            <Button type="button" variant="outline" onClick={() => void load()} className="gap-2">
              <RefreshCw size={14} /> Thử lại
            </Button>
            <Link href="/bai-giang">
              <Button type="button" variant="ghost">Về danh sách bài giảng</Button>
            </Link>
          </div>
        </CardContent>
      </Card>
    );
  }

  const metrics = [
    { label: "Sections", value: lesson.sectionCount, icon: BookOpenText },
    { label: "Từ vựng", value: lesson.vocabularyCount, icon: GraduationCap },
    { label: "Media", value: lesson.assetCount, icon: FileText },
    { label: "Tiên quyết", value: lesson.prerequisiteCount, icon: Link2 },
  ] as const;

  const isStructurallyEmpty =
    lesson.sectionCount === 0 &&
    lesson.vocabularyCount === 0 &&
    lesson.assetCount === 0 &&
    lesson.prerequisiteCount === 0;

  return (
    <div className="space-y-5">
      {error ? (
        <Alert variant="warning" title="Dữ liệu có thể chưa mới nhất">
          {error}
        </Alert>
      ) : null}

      <Card>
        <CardHeader>
          <div className="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
            <div className="min-w-0">
              <div className="flex flex-wrap items-center gap-2">
                <CardTitle className="text-[18px]">{lesson.titleVi}</CardTitle>
                <Badge variant="info">{getContentStatusLabel(lesson.status)}</Badge>
                {lesson.hskCode ? <Badge>{lesson.hskCode}</Badge> : null}
                {lesson.isFeatured ? <Badge variant="warning">Nổi bật</Badge> : null}
              </div>
              <p className="mt-2 max-w-3xl text-[13px] leading-5 text-[#666]">
                {lesson.shortDescriptionVi || "Chưa có mô tả ngắn cho bài giảng này."}
              </p>
              <p className="mt-2 break-all text-[11px] text-[#999]">
                {lesson.slug} · revision v{lesson.version} · {lesson.publicId}
              </p>
            </div>
            <Button type="button" variant="outline" size="sm" onClick={() => void load()} loading={loading} className="gap-2">
              <RefreshCw size={14} /> Làm mới
            </Button>
          </div>
        </CardHeader>
        <CardContent className="flex flex-wrap gap-2 border-t border-[#eee9e2] pt-4">
          <Link href={`/bai-giang/${lessonId}/chinh-sua`}>
            <Button size="sm" className="gap-2"><Pencil size={14} /> Metadata & Workflow</Button>
          </Link>
          <Link href={`/bai-giang/${lessonId}/noi-dung`}>
            <Button variant="outline" size="sm" className="gap-2"><BookOpenText size={14} /> Content Editor</Button>
          </Link>
          <Link href={`/bai-giang/${lessonId}/noi-dung/quan-ly`}>
            <Button variant="outline" size="sm" className="gap-2"><Boxes size={14} /> Vocabulary & Tiên quyết</Button>
          </Link>
        </CardContent>
      </Card>

      {isStructurallyEmpty ? (
        <Alert variant="info" title="Lesson chưa có nội dung liên kết">
          Bắt đầu ở Content Editor để tạo Section và Media, sau đó gắn Vocabulary hoặc Lesson tiên quyết nếu cần.
        </Alert>
      ) : null}

      <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
        {metrics.map(({ label, value, icon: Icon }) => (
          <Card key={label}>
            <CardContent className="flex items-center justify-between gap-3 p-4">
              <div>
                <div className="text-[20px] font-semibold text-[#333]">{value}</div>
                <div className="mt-0.5 text-[12px] text-[#777]">{label}</div>
              </div>
              <div className="flex h-9 w-9 items-center justify-center rounded-[9px] bg-[#f6f3ef] text-[#666]">
                <Icon size={16} />
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      <div className="grid gap-4 xl:grid-cols-2">
        <Card>
          <CardHeader><CardTitle>Phân loại nội dung</CardTitle></CardHeader>
          <CardContent className="space-y-3">
            <DetailRow label="Khóa học" value={lesson.courseTitleVi || "Chưa gắn khóa học"} />
            <DetailRow label="Chương" value={lesson.courseChapterTitleVi || "Chưa gắn chương"} />
            <DetailRow label="HSK" value={lesson.hskNameVi || lesson.hskCode || "Chưa xác định"} />
            <DetailRow label="Chủ đề" value={lesson.topicNameVi || "Chưa chọn chủ đề"} />
          </CardContent>
        </Card>

        <Card>
          <CardHeader><CardTitle>Thiết lập bài giảng</CardTitle></CardHeader>
          <CardContent className="space-y-3">
            <DetailRow label="Thời lượng" value={`${lesson.estimatedMinutes} phút`} icon={<Clock3 size={14} />} />
            <DetailRow label="Độ khó" value={`${lesson.difficulty}/5`} />
            <DetailRow label="Thứ tự" value={String(lesson.sortOrder)} />
            <DetailRow
              label="Xuất bản"
              value={lesson.publishedAt ? new Date(lesson.publishedAt).toLocaleString("vi-VN") : "Chưa xuất bản"}
            />
          </CardContent>
        </Card>
      </div>

      <LessonValidationPanel lessonId={lessonId} />
    </div>
  );
}

function DetailRow({ label, value, icon }: { label: string; value: string; icon?: React.ReactNode }) {
  return (
    <div className="flex items-start justify-between gap-4 border-b border-[#f0ece6] pb-2.5 last:border-0 last:pb-0">
      <span className="flex items-center gap-1.5 text-[12px] text-[#888]">
        {icon || <AlertCircle size={13} className="opacity-60" />}
        {label}
      </span>
      <span className="text-right text-[13px] font-medium text-[#444]">{value}</span>
    </div>
  );
}
