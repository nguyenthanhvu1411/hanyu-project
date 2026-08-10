"use client";

import { useState } from "react";
import type { CourseEditorController } from "../../hooks/use-course-editor";
import { ChapterCard } from "./chapter-card";
import { ChapterForm } from "./chapter-form";
import { Button } from "@/components/ui/button";

interface Props {
  editor: CourseEditorController;
}

export function CourseCurriculumTab({ editor }: Props) {
  const [creatingChapter, setCreatingChapter] = useState(false);

  return (
    <section className="space-y-4">
      <header className="flex items-center justify-between">
        <div>
          <h2 className="text-lg font-semibold">Chương trình học</h2>
          <p className="mt-1 text-sm text-neutral-500">
            {editor.chapters.length} chương · {editor.lessonCount} bài học
          </p>
        </div>

        {editor.canEdit && (
          <Button
            variant="danger"
            onClick={() => setCreatingChapter(true)}
          >
            + Thêm chương
          </Button>
        )}
      </header>

      {creatingChapter && (
        <ChapterForm
          nextSortOrder={
            editor.chapters.length === 0
              ? 0
              : Math.max(...editor.chapters.map((x) => x.sortOrder)) + 1
          }
          saving={editor.saving}
          onCancel={() => setCreatingChapter(false)}
          onSubmit={async (request) => {
            const result = await editor.createChapter(request);
            if (result) {
              setCreatingChapter(false);
            }
          }}
        />
      )}

      {editor.chapters.length === 0 && !creatingChapter && (
        <div className="rounded-xl border border-dashed bg-white p-12 text-center">
          <h3 className="font-medium">Chưa có chương học</h3>
          <p className="mt-1 text-sm text-neutral-500">
            Tạo chương đầu tiên để bắt đầu xây dựng curriculum.
          </p>
        </div>
      )}

      <div className="space-y-3">
        {editor.chapters.map((chapter, chapterIndex) => (
          <ChapterCard
            key={chapter.id}
            chapter={chapter}
            number={chapterIndex + 1}
            lessons={editor.chapterLessons[chapter.id] ?? []}
            chapters={editor.chapters}
            canEdit={editor.canEdit}
            saving={editor.saving}
            onUpdate={(request) => editor.updateChapter(chapter.id, request)}
            onDelete={() => editor.deleteChapter(chapter)}
            onAssignLesson={(lessonId) => editor.assignLesson(chapter.id, lessonId)}
            onRemoveLesson={(lessonId) => editor.removeLesson(chapter.id, lessonId)}
            onMoveLesson={(lessonId, targetChapterId, sortOrder) =>
              editor.moveLesson(chapter.id, lessonId, {
                targetChapterId,
                sortOrder,
              })
            }
            onMoveUp={(lessonId) => editor.moveLessonUp(chapter.id, lessonId)}
            onMoveDown={(lessonId) => editor.moveLessonDown(chapter.id, lessonId)}
          />
        ))}
      </div>
    </section>
  );
}
