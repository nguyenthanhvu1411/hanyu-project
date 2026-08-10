"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Plus } from "lucide-react";
import { courseApi } from "@/features/course/api/course.api";
import { EmptyState } from "@/components/common/empty-state";
import { ErrorState } from "@/components/common/error-state";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { ChapterItem } from "./chapter-item";
import { ChapterFormDialog } from "./chapter-form-dialog";

export function CourseCurriculumTab({ courseId }: { courseId: number }) {
  const [isChapterFormOpen, setIsChapterFormOpen] = useState(false);
  const { data: course, isLoading, error, refetch } = useQuery({
    queryKey: ["course", courseId],
    queryFn: () => courseApi.getById(courseId),
    enabled: Number.isSafeInteger(courseId) && courseId > 0,
  });

  if (isLoading) return <div className="space-y-3"><Skeleton className="h-20 w-full rounded-[11px]" /><Skeleton className="h-20 w-full rounded-[11px]" /></div>;
  if (!course) return <ErrorState title="Không tìm thấy khóa học" description={error instanceof Error ? error.message : "Không có dữ liệu khóa học."} onRetry={() => void refetch()} />;

  const chapters = course.chapters ?? [];

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between gap-3">
        <CardTitle>Nội dung khóa học</CardTitle>
        <Button onClick={() => setIsChapterFormOpen(true)} className="gap-2"><Plus size={14} />Thêm chương</Button>
      </CardHeader>
      <CardContent className="space-y-4">
        {chapters.length === 0 ? (
          <EmptyState title="Chưa có chương học" description="Tạo chương đầu tiên để bắt đầu xây dựng nội dung khóa học." actionLabel="Thêm chương" onAction={() => setIsChapterFormOpen(true)} />
        ) : (
          chapters.map((chapter) => (
            <ChapterItem key={chapter.id} chapter={chapter} courseId={courseId} courseHskLevelId={course.hskLevelId ?? 1} />
          ))
        )}
      </CardContent>

      {isChapterFormOpen ? (
        <ChapterFormDialog courseId={courseId} open={isChapterFormOpen} onOpenChange={setIsChapterFormOpen} />
      ) : null}
    </Card>
  );
}
