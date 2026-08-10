"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { lessonApi } from "@/features/lesson/api/lesson.api";
import type { AdminCourseChapter } from "@/features/course/types/course.types";
import { Button } from "@/components/ui/button";
import { Plus, GripVertical } from "lucide-react";
import { LessonItem } from "./lesson-item";
import { Skeleton } from "@/components/ui/skeleton";
import { ChapterFormDialog } from "./chapter-form-dialog";
import { LessonFormDialog } from "./lesson-form-dialog";

interface ChapterItemProps {
  chapter: AdminCourseChapter;
  courseId: number;
  courseHskLevelId: number;
}

export function ChapterItem({ chapter, courseId, courseHskLevelId }: ChapterItemProps) {
  const [isChapterFormOpen, setIsChapterFormOpen] = useState(false);
  const [isLessonFormOpen, setIsLessonFormOpen] = useState(false);

  const { data: lessonsResult, isLoading } = useQuery({
    queryKey: ["lessons", "by-chapter", chapter.id],
    queryFn: () => lessonApi.danhSach({ chapterId: chapter.id, pageSize: 100 }),
    enabled: chapter.id > 0,
  });

  const lessons = lessonsResult?.items ?? [];

  return (
    <div className="rounded-md border bg-card">
      <div className="flex items-center justify-between border-b bg-muted/30 px-4 py-3">
        <div className="flex items-center gap-3">
          <GripVertical className="h-4 w-4 cursor-grab text-muted-foreground" />
          <span className="font-medium">Chương {chapter.sortOrder}: {chapter.titleVi}</span>
        </div>
        <Button variant="ghost" size="sm" className="h-8 text-muted-foreground" onClick={() => setIsChapterFormOpen(true)}>
          Sửa
        </Button>
      </div>

      <div className="p-4">
        {isLoading ? (
          <div className="space-y-2">
            <Skeleton className="h-10 w-full" />
            <Skeleton className="h-10 w-full" />
          </div>
        ) : (
          <div className="relative pl-6">
            <div className="absolute bottom-0 left-[15px] top-0 w-px bg-border" />
            <div className="space-y-3">
              {lessons.map((lesson, index) => (
                <LessonItem
                  key={lesson.id}
                  lesson={lesson}
                  isLast={index === lessons.length - 1}
                  courseId={courseId}
                  chapterId={chapter.id}
                  hskLevelId={courseHskLevelId}
                />
              ))}

              <div className="relative flex items-center gap-3">
                <div className="absolute -left-[10px] h-px w-[14px] bg-border" />
                <Button variant="ghost" size="sm" className="h-8 text-muted-foreground" onClick={() => setIsLessonFormOpen(true)}>
                  <Plus className="mr-2 h-4 w-4" />
                  Thêm bài giảng
                </Button>
              </div>
            </div>
          </div>
        )}
      </div>

      {isChapterFormOpen && (
        <ChapterFormDialog courseId={courseId} chapter={chapter} open={isChapterFormOpen} onOpenChange={setIsChapterFormOpen} />
      )}

      {isLessonFormOpen && (
        <LessonFormDialog courseId={courseId} chapterId={chapter.id} hskLevelId={courseHskLevelId} open={isLessonFormOpen} onOpenChange={setIsLessonFormOpen} />
      )}
    </div>
  );
}
