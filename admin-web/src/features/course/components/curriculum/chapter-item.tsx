"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { GripVertical, Plus } from "lucide-react";
import { lessonApi } from "@/features/lesson/api/lesson.api";
import type { AdminCourseChapter } from "@/features/course/types/course.types";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { LessonItem } from "./lesson-item";
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
    queryFn: () => lessonApi.list({ chapterId: chapter.id, pageSize: 100 }),
    enabled: chapter.id > 0,
  });
  const lessons = lessonsResult?.items ?? [];

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between gap-3 py-3">
        <div className="flex min-w-0 items-center gap-3">
          <GripVertical size={15} className="shrink-0 text-[#999]" />
          <div className="min-w-0">
            <CardTitle className="truncate">Chương {chapter.sortOrder}: {chapter.titleVi}</CardTitle>
            <p className="mt-1 text-[10px] text-[#888]">ID {chapter.id} · PublicId {chapter.publicId.slice(0, 8)}… · {lessons.length} bài giảng</p>
          </div>
        </div>
        <Button variant="outline" size="sm" onClick={() => setIsChapterFormOpen(true)}>Sửa</Button>
      </CardHeader>

      <CardContent className="space-y-2">
        {isLoading ? (
          <div className="space-y-2"><Skeleton className="h-10 w-full" /><Skeleton className="h-10 w-full" /></div>
        ) : lessons.length > 0 ? (
          lessons.map((lesson, index) => (
            <LessonItem
              key={lesson.id}
              lesson={lesson}
              isLast={index === lessons.length - 1}
              courseId={courseId}
              chapterId={chapter.id}
              hskLevelId={courseHskLevelId}
            />
          ))
        ) : (
          <p className="py-5 text-center text-[11px] text-[#888]">Chưa có bài giảng trong chương này.</p>
        )}
      </CardContent>

      <CardFooter>
        <Button variant="outline" size="sm" className="gap-2" onClick={() => setIsLessonFormOpen(true)}>
          <Plus size={14} /> Thêm bài giảng
        </Button>
      </CardFooter>

      {isChapterFormOpen ? <ChapterFormDialog courseId={courseId} chapter={chapter} open={isChapterFormOpen} onOpenChange={setIsChapterFormOpen} /> : null}
      {isLessonFormOpen ? <LessonFormDialog courseId={courseId} chapterId={chapter.id} hskLevelId={courseHskLevelId} open={isLessonFormOpen} onOpenChange={setIsLessonFormOpen} /> : null}
    </Card>
  );
}
