"use client";

import { ArrowDown, ArrowRightLeft, ArrowUp, Pencil, Unlink } from "lucide-react";

import type { CourseChapterLesson } from "@/features/course/types/curriculum.types";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { getContentStatusLabel } from "@/lib/constants/content-status";

interface LessonItemProps {
  lesson: CourseChapterLesson;
  index: number;
  total: number;
  busy: boolean;
  onEdit: () => void;
  onMoveUp: () => void;
  onMoveDown: () => void;
  onMoveChapter: () => void;
  onRemove: () => void;
}

function getDifficultyLabel(difficulty: number) {
  if (difficulty <= 1) return "Cơ bản";
  if (difficulty === 2) return "Trung bình";
  if (difficulty === 3) return "Khá";
  return "Nâng cao";
}

export function LessonItem({
  lesson,
  index,
  total,
  busy,
  onEdit,
  onMoveUp,
  onMoveDown,
  onMoveChapter,
  onRemove,
}: LessonItemProps) {
  return (
    <div className="flex flex-col gap-3 rounded-md border bg-background px-4 py-3 md:flex-row md:items-center md:justify-between">
      <div className="min-w-0">
        <div className="flex flex-wrap items-center gap-2">
          <span className="text-sm font-medium">
            {index + 1}. {lesson.titleVi}
          </span>
          <Badge variant="info">{getContentStatusLabel(lesson.status)}</Badge>
        </div>
        <p className="mt-1 text-[11px] text-muted-foreground">
          Thời lượng: {lesson.estimatedMinutes} phút · Thứ tự: {lesson.sortOrder} · Độ khó: {getDifficultyLabel(lesson.difficulty)}
        </p>
      </div>

      <div className="flex flex-wrap items-center gap-1">
        <Button
          type="button"
          variant="outline"
          size="icon"
          disabled={busy || index === 0}
          onClick={onMoveUp}
          aria-label="Đưa bài giảng lên"
          title="Đưa bài giảng lên một vị trí"
        >
          <ArrowUp size={14} />
        </Button>
        <Button
          type="button"
          variant="outline"
          size="icon"
          disabled={busy || index >= total - 1}
          onClick={onMoveDown}
          aria-label="Đưa bài giảng xuống"
          title="Đưa bài giảng xuống một vị trí"
        >
          <ArrowDown size={14} />
        </Button>
        <Button
          type="button"
          variant="outline"
          size="icon"
          disabled={busy}
          onClick={onMoveChapter}
          aria-label="Chuyển sang chương khác"
          title="Chuyển bài giảng sang chương khác"
        >
          <ArrowRightLeft size={14} />
        </Button>
        <Button
          type="button"
          variant="outline"
          size="icon"
          disabled={busy}
          onClick={onEdit}
          aria-label="Sửa bài giảng"
          title="Chỉnh sửa thông tin bài giảng"
        >
          <Pencil size={14} />
        </Button>
        <Button
          type="button"
          variant="ghost"
          size="icon"
          disabled={busy}
          onClick={onRemove}
          aria-label="Gỡ bài giảng khỏi chương"
          title="Gỡ bài giảng khỏi chương hiện tại"
        >
          <Unlink size={14} />
        </Button>
      </div>
    </div>
  );
}
