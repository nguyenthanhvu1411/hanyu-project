"use client";

import Link from "next/link";
import type {
  CourseChapter,
  CourseChapterLesson,
} from "../../types/curriculum.types";
import { getContentStatusLabel } from "@/lib/constants/content-status";
import { Badge } from "@/components/ui/badge";

interface Props {
  lesson: CourseChapterLesson;
  chapter: CourseChapter;
  chapters: CourseChapter[];
  index: number;
  total: number;
  canEdit: boolean;
  onMoveUp: () => Promise<unknown>;
  onMoveDown: () => Promise<unknown>;
  onRemove: () => Promise<unknown>;
  onMove: (targetChapterId: number) => Promise<unknown>;
}

export function LessonRow({
  lesson,
  chapter,
  chapters,
  index,
  total,
  canEdit,
  onMoveUp,
  onMoveDown,
  onRemove,
  onMove,
}: Props) {
  return (
    <div className="flex items-center gap-3 px-4 py-3">
      <span className="w-6 text-center text-xs text-neutral-400">
        {index + 1}
      </span>

      <div className="min-w-0 flex-1">
        <Link
          href={`/bai-giang/${lesson.id}`}
          className="font-medium hover:text-red-600"
        >
          {lesson.titleVi}
        </Link>

        <div className="mt-1 flex flex-wrap gap-3 text-xs text-neutral-500">
          <span>/{lesson.slug}</span>
          <span>{lesson.estimatedMinutes} phút</span>
          <Badge variant="default">{getContentStatusLabel(lesson.status)}</Badge>
        </div>
      </div>

      {canEdit && (
        <div className="flex items-center gap-1">
          <button
            type="button"
            disabled={index === 0}
            onClick={() => void onMoveUp()}
            aria-label="Đưa bài học lên"
            className="rounded border px-2 py-1 text-xs disabled:opacity-30"
          >
            ↑
          </button>

          <button
            type="button"
            disabled={index === total - 1}
            onClick={() => void onMoveDown()}
            aria-label="Đưa bài học xuống"
            className="rounded border px-2 py-1 text-xs disabled:opacity-30"
          >
            ↓
          </button>

          {chapters.length > 1 && (
            <select
              defaultValue=""
              aria-label="Chuyển chương"
              onChange={(event) => {
                const target = Number(event.target.value);
                if (target > 0 && target !== chapter.id) {
                  void onMove(target);
                }
                event.target.value = "";
              }}
              className="h-8 rounded border bg-white px-2 text-xs"
            >
              <option value="">Chuyển...</option>
              {chapters
                .filter((item) => item.id !== chapter.id)
                .map((item) => (
                  <option key={item.id} value={item.id}>
                    {item.titleVi}
                  </option>
                ))}
            </select>
          )}

          <button
            type="button"
            onClick={() => {
              if (
                window.confirm(
                  `Gỡ "${lesson.titleVi}" khỏi chương? Bài học sẽ không bị xóa.`
                )
              ) {
                void onRemove();
              }
            }}
            className="rounded border border-red-200 px-2 py-1 text-xs text-red-700"
          >
            Gỡ
          </button>
        </div>
      )}
    </div>
  );
}
