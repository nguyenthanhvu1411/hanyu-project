"use client";

import { useState } from "react";
import { AdminLessonListItem } from "@/features/bai-giang/types/bai-giang.types";
import { FileText, Pencil } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { LessonFormDialog } from "./lesson-form-dialog";
import { getContentStatusLabel } from "@/lib/constants/content-status";

interface LessonItemProps {
  lesson: AdminLessonListItem;
  isLast: boolean;
  courseId: number;
  chapterId: number;
  hskLevelId: number;
}

export function LessonItem({ lesson, isLast, courseId, chapterId, hskLevelId }: LessonItemProps) {
  const [isEditOpen, setIsEditOpen] = useState(false);

  return (
    <>
      <div className="relative flex items-center justify-between rounded-md border bg-background px-4 py-2 hover:bg-accent/50">
        {/* Horizontal connector to the main vertical branch */}
        <div className="absolute -left-[10px] h-px w-[14px] bg-border" />

        <div className="flex items-center gap-3">
          <FileText className="h-4 w-4 text-muted-foreground" />
          <span className="text-sm font-medium">
            {lesson.sortOrder}. {lesson.titleVi}
          </span>
          <Badge variant="info" className="ml-2 text-xs">
            {getContentStatusLabel(lesson.status)}
          </Badge>
        </div>

        <div className="flex items-center gap-1 opacity-0 transition-opacity hover:opacity-100 group-hover:opacity-100 lg:opacity-100">
          <Button 
            variant="ghost" 
            size="icon" 
            className="h-8 w-8"
            onClick={() => setIsEditOpen(true)}
          >
            <Pencil className="h-4 w-4" />
          </Button>
        </div>
      </div>

      {isEditOpen && (
        <LessonFormDialog
          courseId={courseId}
          chapterId={chapterId}
          hskLevelId={hskLevelId}
          lesson={lesson}
          open={isEditOpen}
          onOpenChange={setIsEditOpen}
        />
      )}
    </>
  );
}
