"use client";

import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ArrowDown, ArrowUp, Plus, RefreshCw } from "lucide-react";

import { courseApi } from "@/features/course/api/course.api";
import { chapterApi } from "@/features/course/api/chapter.api";
import { curriculumApi } from "@/features/course/api/curriculum.api";
import type { AdminCourseChapter } from "@/features/course/types/course.types";
import { EmptyState } from "@/components/common/empty-state";
import { ErrorState } from "@/components/common/error-state";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";

import { ChapterItem } from "./chapter-item";
import { ChapterFormDialog } from "./chapter-form-dialog";

export function CourseCurriculumTab({ courseId }: { courseId: number }) {
  const queryClient = useQueryClient();
  const [isChapterFormOpen, setIsChapterFormOpen] = useState(false);

  const courseQuery = useQuery({
    queryKey: ["course", courseId],
    queryFn: () => courseApi.getById(courseId),
    enabled: Number.isSafeInteger(courseId) && courseId > 0,
  });

  const chapterQuery = useQuery({
    queryKey: ["chapters", courseId, "include-deleted"],
    queryFn: () => chapterApi.list(courseId, true),
    enabled: Number.isSafeInteger(courseId) && courseId > 0,
  });

  const reorderMutation = useMutation({
    mutationFn: (chapters: AdminCourseChapter[]) =>
      curriculumApi.reorderChapters(courseId, {
        items: chapters.map((chapter, index) => ({
          chapterId: chapter.id,
          sortOrder: index,
        })),
      }),
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: ["chapters", courseId] }),
        queryClient.invalidateQueries({ queryKey: ["course", courseId] }),
      ]);
      appToast.success("Đã cập nhật thứ tự chương học.");
    },
    onError: (error) => {
      appToast.error("Không thể sắp xếp chương học", normalizeApiError(error).message);
    },
  });

  if (courseQuery.isLoading || chapterQuery.isLoading) {
    return (
      <div className="space-y-3">
        <Skeleton className="h-20 w-full rounded-[11px]" />
        <Skeleton className="h-20 w-full rounded-[11px]" />
      </div>
    );
  }

  const course = courseQuery.data;
  if (!course) {
    return (
      <ErrorState
        title="Không tìm thấy khóa học"
        description={
          courseQuery.error instanceof Error
            ? courseQuery.error.message
            : "Không có dữ liệu khóa học."
        }
        onRetry={() => void courseQuery.refetch()}
      />
    );
  }

  if (chapterQuery.error) {
    return (
      <ErrorState
        title="Không thể tải chương học"
        description={normalizeApiError(chapterQuery.error).message}
        onRetry={() => void chapterQuery.refetch()}
      />
    );
  }

  const chapters = [...(chapterQuery.data ?? [])].sort(
    (a, b) => a.sortOrder - b.sortOrder || a.id - b.id,
  );
  const activeChapters = chapters.filter((chapter) => !chapter.deletedAt);

  function moveChapter(chapterId: number, direction: -1 | 1) {
    const currentIndex = activeChapters.findIndex((chapter) => chapter.id === chapterId);
    const targetIndex = currentIndex + direction;

    if (currentIndex < 0 || targetIndex < 0 || targetIndex >= activeChapters.length) {
      return;
    }

    const reordered = [...activeChapters];
    [reordered[currentIndex], reordered[targetIndex]] = [
      reordered[targetIndex],
      reordered[currentIndex],
    ];
    reorderMutation.mutate(reordered);
  }

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between gap-3">
        <div>
          <CardTitle>Nội dung khóa học</CardTitle>
          <p className="mt-1 text-[11px] text-muted-foreground">
            Quản lý Chapter và Lesson theo đúng quan hệ Course → Chapter → Lesson của backend.
          </p>
        </div>
        <div className="flex items-center gap-2">
          <Button
            variant="outline"
            size="sm"
            className="gap-2"
            onClick={() => void chapterQuery.refetch()}
          >
            <RefreshCw size={14} /> Làm mới
          </Button>
          <Button onClick={() => setIsChapterFormOpen(true)} className="gap-2">
            <Plus size={14} /> Thêm chương
          </Button>
        </div>
      </CardHeader>

      <CardContent className="space-y-4">
        {chapters.length === 0 ? (
          <EmptyState
            title="Chưa có chương học"
            description="Tạo chương đầu tiên để bắt đầu xây dựng nội dung khóa học."
            actionLabel="Thêm chương"
            onAction={() => setIsChapterFormOpen(true)}
          />
        ) : (
          chapters.map((chapter) => {
            const activeIndex = activeChapters.findIndex((item) => item.id === chapter.id);
            return (
              <div key={chapter.id} className="flex items-start gap-2">
                {!chapter.deletedAt ? (
                  <div className="flex shrink-0 flex-col gap-1 pt-3">
                    <Button
                      type="button"
                      variant="outline"
                      size="icon"
                      aria-label="Đưa chương lên"
                      disabled={reorderMutation.isPending || activeIndex <= 0}
                      onClick={() => moveChapter(chapter.id, -1)}
                    >
                      <ArrowUp size={14} />
                    </Button>
                    <Button
                      type="button"
                      variant="outline"
                      size="icon"
                      aria-label="Đưa chương xuống"
                      disabled={
                        reorderMutation.isPending ||
                        activeIndex < 0 ||
                        activeIndex >= activeChapters.length - 1
                      }
                      onClick={() => moveChapter(chapter.id, 1)}
                    >
                      <ArrowDown size={14} />
                    </Button>
                  </div>
                ) : null}

                <div className="min-w-0 flex-1">
                  <ChapterItem
                    chapter={chapter}
                    courseId={courseId}
                    courseHskLevelId={course.hskLevelId ?? 0}
                    chapters={activeChapters}
                  />
                </div>
              </div>
            );
          })
        )}
      </CardContent>

      {isChapterFormOpen ? (
        <ChapterFormDialog
          courseId={courseId}
          open={isChapterFormOpen}
          onOpenChange={setIsChapterFormOpen}
        />
      ) : null}
    </Card>
  );
}
