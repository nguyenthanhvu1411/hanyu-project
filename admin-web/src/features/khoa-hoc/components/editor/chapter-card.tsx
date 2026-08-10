"use client";

import { useState } from "react";
import Link from "next/link";
import type {
  CourseChapter,
  CourseChapterLesson,
  CreateChapterRequest,
  UpdateChapterRequest,
} from "../../types/curriculum.types";
import { ChapterForm } from "./chapter-form";
import { LessonRow } from "./lesson-row";
import { AssignLessonForm } from "./assign-lesson-form";
import { Button } from "@/components/ui/button";

interface Props {
  chapter: CourseChapter;
  number: number;
  lessons: CourseChapterLesson[];
  chapters: CourseChapter[];
  canEdit: boolean;
  saving: boolean;
  onUpdate: (request: UpdateChapterRequest) => Promise<unknown>;
  onDelete: () => Promise<unknown>;
  onAssignLesson: (lessonId: number) => Promise<unknown>;
  onRemoveLesson: (lessonId: number) => Promise<unknown>;
  onMoveLesson: (
    lessonId: number,
    targetChapterId: number,
    sortOrder: number
  ) => Promise<unknown>;
  onMoveUp: (lessonId: number) => Promise<unknown>;
  onMoveDown: (lessonId: number) => Promise<unknown>;
}

export function ChapterCard({
  chapter,
  number,
  lessons,
  chapters,
  canEdit,
  saving,
  onUpdate,
  onDelete,
  onAssignLesson,
  onRemoveLesson,
  onMoveLesson,
  onMoveUp,
  onMoveDown,
}: Props) {
  const [editing, setEditing] = useState(false);
  const [assigningLesson, setAssigningLesson] = useState(false);
  const [collapsed, setCollapsed] = useState(false);

  if (editing) {
    const initial: CreateChapterRequest = {
      titleVi: chapter.titleVi,
      descriptionVi: chapter.descriptionVi,
      sortOrder: chapter.sortOrder,
      isActive: chapter.isActive,
    };

    return (
      <ChapterForm
        initial={initial}
        saving={saving}
        onCancel={() => setEditing(false)}
        onSubmit={async (request) => {
          const result = await onUpdate({
            ...request,
            concurrencyToken: chapter.concurrencyToken,
          });

          if (result) {
            setEditing(false);
          }
        }}
      />
    );
  }

  return (
    <article className="overflow-hidden rounded-xl border bg-white">
      <header className="flex items-center justify-between gap-4 border-b px-4 py-3">
        <button
          type="button"
          className="flex min-w-0 flex-1 items-center gap-3 text-left hover:bg-neutral-50 rounded-lg p-1 -ml-1 transition-colors"
          onClick={() => setCollapsed(!collapsed)}
        >
          <span className="flex size-8 shrink-0 items-center justify-center rounded-lg bg-neutral-100 text-sm font-semibold">
            {number}
          </span>

          <div className="min-w-0">
            <div className="flex items-center gap-2">
              <h3 className="truncate font-semibold">{chapter.titleVi}</h3>
              {!chapter.isActive && (
                <span className="rounded bg-neutral-100 px-2 py-0.5 text-xs text-neutral-600">
                  Tắt
                </span>
              )}
            </div>
            <p className="mt-0.5 text-xs text-neutral-500">
              {lessons.length} bài học
            </p>
          </div>
        </button>

        {canEdit && (
          <div className="flex gap-2">
            <Button
              variant="outline"
              size="sm"
              onClick={() => setEditing(true)}
            >
              Sửa
            </Button>
            <Button
              variant="outline"
              size="sm"
              className="border-red-200 text-red-700 hover:bg-red-50"
              onClick={() => {
                if (lessons.length > 0) {
                  window.alert(
                    "Hãy chuyển hoặc gỡ các bài học khỏi chương trước khi xóa."
                  );
                  return;
                }
                if (window.confirm(`Xóa chương "${chapter.titleVi}"?`)) {
                  void onDelete();
                }
              }}
            >
              Xóa
            </Button>
          </div>
        )}
      </header>

      {!collapsed && (
        <>
          <div className="divide-y">
            {lessons.map((lesson, index) => (
              <LessonRow
                key={lesson.id}
                lesson={lesson}
                chapter={chapter}
                chapters={chapters}
                index={index}
                total={lessons.length}
                canEdit={canEdit}
                onMoveUp={() => onMoveUp(lesson.id)}
                onMoveDown={() => onMoveDown(lesson.id)}
                onRemove={() => onRemoveLesson(lesson.id)}
                onMove={(targetChapterId) => {
                  const targetChapter = chapters.find(
                    (x) => x.id === targetChapterId
                  );
                  if (!targetChapter) return Promise.resolve();
                  return onMoveLesson(
                    lesson.id,
                    targetChapterId,
                    targetChapter.lessonCount
                  );
                }}
              />
            ))}

            {lessons.length === 0 && (
              <div className="px-4 py-8 text-center text-sm text-neutral-500">
                Chương này chưa có bài học.
              </div>
            )}
          </div>

          {canEdit && (
            <footer className="border-t bg-neutral-50 px-4 py-3">
              {!assigningLesson ? (
                <div className="flex gap-2">
                  <Button
                    variant="outline"
                    onClick={() => setAssigningLesson(true)}
                  >
                    + Thêm bài học có sẵn
                  </Button>
                  <Link href={`/bai-giang/them-moi?courseId=${chapter.courseId}&chapterId=${chapter.id}`}>
                    <Button variant="outline">
                      + Tạo bài học mới
                    </Button>
                  </Link>
                </div>
              ) : (
                <AssignLessonForm
                  courseId={chapter.courseId}
                  chapterId={chapter.id}
                  onAssign={onAssignLesson}
                  onCancel={() => setAssigningLesson(false)}
                />
              )}
            </footer>
          )}
        </>
      )}
    </article>
  );
}
