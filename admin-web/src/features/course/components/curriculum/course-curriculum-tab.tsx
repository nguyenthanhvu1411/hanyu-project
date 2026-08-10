"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { courseApi } from "@/features/course/api/course.api";
import { Button } from "@/components/ui/button";
import { Plus } from "lucide-react";
import { Skeleton } from "@/components/ui/skeleton";
import { ChapterItem } from "./chapter-item";
import { ChapterFormDialog } from "./chapter-form-dialog";

export function CourseCurriculumTab({ courseId }: { courseId: number }) {
  const [isChapterFormOpen, setIsChapterFormOpen] = useState(false);

  const { data: course, isLoading } = useQuery({
    queryKey: ["course", courseId],
    queryFn: () => courseApi.chiTiet(courseId),
    enabled: Number.isSafeInteger(courseId) && courseId > 0,
  });

  if (isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-20 w-full" />
        <Skeleton className="h-20 w-full" />
      </div>
    );
  }

  if (!course) {
    return <div>Không tìm thấy khóa học.</div>;
  }

  const chapters = course.chapters ?? [];

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h2 className="text-xl font-semibold">Nội dung khóa học</h2>
      </div>

      <div className="space-y-4">
        {chapters.length === 0 ? (
          <div className="flex h-32 items-center justify-center rounded-lg border border-dashed text-muted-foreground">
            Chưa có chương học nào
          </div>
        ) : (
          chapters.map((chapter) => (
            <ChapterItem
              key={chapter.id}
              chapter={chapter}
              courseId={courseId}
              courseHskLevelId={course.hskLevelId ?? 1}
            />
          ))
        )}
      </div>

      <Button
        variant="outline"
        className="w-full justify-start text-muted-foreground hover:text-foreground"
        onClick={() => setIsChapterFormOpen(true)}
      >
        <Plus className="mr-2 h-4 w-4" />
        Thêm chương
      </Button>

      {isChapterFormOpen && (
        <ChapterFormDialog
          courseId={courseId}
          open={isChapterFormOpen}
          onOpenChange={setIsChapterFormOpen}
        />
      )}
    </div>
  );
}
