"use client";

import { useState } from "react";
import Link from "next/link";
import { ChevronDown, ChevronRight, Plus } from "lucide-react";
import { DataTableActions } from "@/components/common/data-table/data-table-actions";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import type {
  CourseChapter,
  CourseChapterLesson,
  CreateChapterRequest,
  UpdateChapterRequest,
} from "../../types/curriculum.types";
import { ChapterForm } from "./chapter-form";
import { LessonRow } from "./lesson-row";
import { AssignLessonForm } from "./assign-lesson-form";

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
  onMoveLesson: (lessonId: number, targetChapterId: number, sortOrder: number) => Promise<unknown>;
  onMoveUp: (lessonId: number) => Promise<unknown>;
  onMoveDown: (lessonId: number) => Promise<unknown>;
}

export function ChapterCard({
  chapter, number, lessons, chapters, canEdit, saving, onUpdate, onDelete,
  onAssignLesson, onRemoveLesson, onMoveLesson, onMoveUp, onMoveDown,
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
          const result = await onUpdate({ ...request, concurrencyToken: chapter.concurrencyToken });
          if (result) setEditing(false);
        }}
      />
    );
  }

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between gap-3 py-3">
        <div className="flex min-w-0 flex-1 items-center gap-3">
          <Button
            type="button"
            variant="ghost"
            size="icon"
            onClick={() => setCollapsed((value) => !value)}
            aria-label={collapsed ? "Mở chương" : "Thu gọn chương"}
          >
            {collapsed ? <ChevronRight size={16} /> : <ChevronDown size={16} />}
          </Button>
          <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-[8px] bg-[#f3f1ed] text-[12px] font-semibold text-[#555]">{number}</div>
          <div className="min-w-0">
            <div className="flex flex-wrap items-center gap-2">
              <CardTitle className="truncate">{chapter.titleVi}</CardTitle>
              <Badge variant={chapter.isActive ? "success" : "default"}>{chapter.isActive ? "Hoạt động" : "Tắt"}</Badge>
            </div>
            <p className="mt-1 text-[10px] text-[#888]">ID {chapter.id} · {lessons.length} bài học</p>
          </div>
        </div>
        {canEdit ? (
          <DataTableActions
            onEdit={() => setEditing(true)}
            onDelete={() => {
              if (lessons.length > 0) {
                window.alert("Hãy chuyển hoặc gỡ các bài học khỏi chương trước khi xóa.");
                return;
              }
              if (window.confirm(`Xóa chương \"${chapter.titleVi}\"?`)) void onDelete();
            }}
          />
        ) : null}
      </CardHeader>

      {!collapsed ? (
        <>
          <CardContent className="p-0">
            <div className="divide-y divide-[#eee9e2]">
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
                    const targetChapter = chapters.find((item) => item.id === targetChapterId);
                    return targetChapter
                      ? onMoveLesson(lesson.id, targetChapterId, targetChapter.lessonCount)
                      : Promise.resolve();
                  }}
                />
              ))}
              {lessons.length === 0 ? (
                <div className="px-4 py-8 text-center text-[11px] text-[#888]">Chương này chưa có bài học.</div>
              ) : null}
            </div>
          </CardContent>

          {canEdit ? (
            <CardFooter className="bg-[#fbfaf8]">
              {!assigningLesson ? (
                <div className="flex flex-wrap gap-2">
                  <Button variant="outline" onClick={() => setAssigningLesson(true)} className="gap-2">
                    <Plus size={14} /> Thêm bài học có sẵn
                  </Button>
                  <Link href={`/bai-giang/them-moi?courseId=${chapter.courseId}&chapterId=${chapter.id}`}>
                    <Button variant="outline" className="gap-2"><Plus size={14} /> Tạo bài học mới</Button>
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
            </CardFooter>
          ) : null}
        </>
      ) : null}
    </Card>
  );
}
