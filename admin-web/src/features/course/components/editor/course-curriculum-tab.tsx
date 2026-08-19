"use client";

import { useState } from "react";
import { BookOpen, Plus } from "lucide-react";
import { EmptyState } from "@/components/common/empty-state";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import type { CourseEditorController } from "../../hooks/use-course-editor";
import { ChapterCard } from "./chapter-card";
import { ChapterForm } from "./chapter-form";

interface Props { editor: CourseEditorController; }

export function CourseCurriculumTab({ editor }: Props) {
  const [creatingChapter, setCreatingChapter] = useState(false);

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between gap-3">
        <div>
          <CardTitle>Chương trình học</CardTitle>
          <p className="mt-1 text-[10px] text-[#888]">{editor.chapters.length} chương · {editor.lessonCount} bài học</p>
        </div>
        {editor.canEdit ? (
          <Button onClick={() => setCreatingChapter(true)} className="gap-2">
            <Plus size={14} /> Thêm chương
          </Button>
        ) : null}
      </CardHeader>
      <CardContent className="space-y-4">
        {creatingChapter ? (
          <ChapterForm
            nextSortOrder={editor.chapters.length === 0 ? 0 : Math.max(...editor.chapters.map((x) => x.sortOrder)) + 1}
            saving={editor.saving}
            onCancel={() => setCreatingChapter(false)}
            onSubmit={async (request) => {
              const result = await editor.createChapter(request);
              if (result) setCreatingChapter(false);
            }}
          />
        ) : null}

        {editor.chapters.length === 0 && !creatingChapter ? (
          <EmptyState
            icon={<BookOpen size={24} />}
            title="Chưa có chương học"
            description="Tạo chương đầu tiên để bắt đầu xây dựng nội dung khóa học."
            actionLabel={editor.canEdit ? "Thêm chương" : undefined}
            onAction={editor.canEdit ? () => setCreatingChapter(true) : undefined}
          />
        ) : null}

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
              onMoveLesson={(lessonId, targetChapterId, sortOrder) => editor.moveLesson(chapter.id, lessonId, { targetChapterId, sortOrder })}
              onMoveUp={(lessonId) => editor.moveLessonUp(chapter.id, lessonId)}
              onMoveDown={(lessonId) => editor.moveLessonDown(chapter.id, lessonId)}
            />
          ))}
        </div>
      </CardContent>
    </Card>
  );
}
