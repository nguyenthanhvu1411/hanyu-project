"use client";

import { useMemo, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ArchiveRestore, Link2, Plus, Trash2 } from "lucide-react";

import { lessonApi } from "@/features/lesson/api/lesson.api";
import { chapterApi } from "@/features/course/api/chapter.api";
import { curriculumApi } from "@/features/course/api/curriculum.api";
import type { AdminCourseChapter } from "@/features/course/types/course.types";
import type { CourseChapterLesson } from "@/features/course/types/curriculum.types";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Dialog } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";

import { LessonItem } from "./lesson-item";
import { ChapterFormDialog } from "./chapter-form-dialog";
import { LessonFormDialog } from "./lesson-form-dialog";

interface ChapterItemProps {
  chapter: AdminCourseChapter;
  courseId: number;
  courseHskLevelId: number;
  chapters: AdminCourseChapter[];
}

export function ChapterItem({
  chapter,
  courseId,
  courseHskLevelId,
  chapters,
}: ChapterItemProps) {
  const queryClient = useQueryClient();
  const [isChapterFormOpen, setIsChapterFormOpen] = useState(false);
  const [isLessonFormOpen, setIsLessonFormOpen] = useState(false);
  const [editingLesson, setEditingLesson] = useState<CourseChapterLesson | null>(null);
  const [attachOpen, setAttachOpen] = useState(false);
  const [attachSearch, setAttachSearch] = useState("");
  const [selectedLessonId, setSelectedLessonId] = useState("");
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [restoreOpen, setRestoreOpen] = useState(false);
  const [removeLesson, setRemoveLesson] = useState<CourseChapterLesson | null>(null);
  const [moveLesson, setMoveLesson] = useState<CourseChapterLesson | null>(null);
  const [targetChapterId, setTargetChapterId] = useState("");

  const deleted = Boolean(chapter.deletedAt);

  const lessonsQuery = useQuery({
    queryKey: ["chapter-lessons", courseId, chapter.id],
    queryFn: () => curriculumApi.lessons(courseId, chapter.id),
    enabled: !deleted && chapter.id > 0,
  });

  const candidateQuery = useQuery({
    queryKey: ["lesson-picker", attachSearch],
    queryFn: () =>
      lessonApi.list({
        search: attachSearch.trim() || undefined,
        page: 1,
        pageSize: 50,
      }),
    enabled: attachOpen,
  });

  const lessons = useMemo(
    () =>
      [...(lessonsQuery.data ?? [])].sort(
        (a, b) => a.sortOrder - b.sortOrder || a.id - b.id,
      ),
    [lessonsQuery.data],
  );

  const candidateOptions = useMemo(() => {
    const currentIds = new Set(lessons.map((item) => item.id));
    return (candidateQuery.data?.items ?? [])
      .filter((item) => !currentIds.has(item.id) && !item.chapterId)
      .map((item) => ({
        value: String(item.id),
        label: `${item.titleVi} · ${item.slug}`,
      }));
  }, [candidateQuery.data?.items, lessons]);

  const targetOptions = chapters
    .filter((item) => item.id !== chapter.id && !item.deletedAt)
    .map((item) => ({
      value: String(item.id),
      label: `${item.sortOrder}. ${item.titleVi}`,
    }));

  async function invalidateAll() {
    await Promise.all([
      queryClient.invalidateQueries({ queryKey: ["chapter-lessons", courseId] }),
      queryClient.invalidateQueries({ queryKey: ["chapters", courseId] }),
      queryClient.invalidateQueries({ queryKey: ["course", courseId] }),
      queryClient.invalidateQueries({ queryKey: ["lessons"] }),
    ]);
  }

  const relationMutation = useMutation({
    mutationFn: async (action: () => Promise<unknown>) => action(),
    onSuccess: async () => {
      await invalidateAll();
    },
    onError: (error) => {
      appToast.error("Thao tác bài giảng thất bại", normalizeApiError(error).message);
    },
  });

  const chapterMutation = useMutation({
    mutationFn: async (action: () => Promise<unknown>) => action(),
    onSuccess: async () => {
      await invalidateAll();
    },
    onError: (error) => {
      appToast.error("Thao tác chương học thất bại", normalizeApiError(error).message);
    },
  });

  const busy = relationMutation.isPending || chapterMutation.isPending;

  async function attachExistingLesson() {
    const lessonId = Number(selectedLessonId);
    if (!Number.isSafeInteger(lessonId) || lessonId <= 0) {
      appToast.error("Vui lòng chọn bài giảng hợp lệ.");
      return;
    }

    try {
      await relationMutation.mutateAsync(() =>
        curriculumApi.assignLesson(courseId, chapter.id, {
          lessonId,
          sortOrder: lessons.length,
        }),
      );
      appToast.success("Đã gắn bài giảng vào chương.");
      setSelectedLessonId("");
      setAttachSearch("");
      setAttachOpen(false);
    } catch {
      // Mutation handles and displays the API error.
    }
  }

  async function reorderLesson(index: number, direction: -1 | 1) {
    const targetIndex = index + direction;
    if (targetIndex < 0 || targetIndex >= lessons.length) return;

    const reordered = [...lessons];
    [reordered[index], reordered[targetIndex]] = [
      reordered[targetIndex],
      reordered[index],
    ];

    try {
      await relationMutation.mutateAsync(() =>
        curriculumApi.reorderLessons(courseId, chapter.id, {
          items: reordered.map((item, order) => ({
            lessonId: item.id,
            sortOrder: order,
          })),
        }),
      );
      appToast.success("Đã cập nhật thứ tự bài giảng.");
    } catch {
      // Mutation handles and displays the API error.
    }
  }

  async function confirmMoveLesson() {
    if (!moveLesson) return;
    const targetId = Number(targetChapterId);
    if (!Number.isSafeInteger(targetId) || targetId <= 0 || targetId === chapter.id) {
      appToast.error("Vui lòng chọn chương đích hợp lệ.");
      return;
    }

    try {
      await relationMutation.mutateAsync(() =>
        curriculumApi.moveLesson(courseId, chapter.id, moveLesson.id, {
          targetChapterId: targetId,
          sortOrder: 0,
        }),
      );
      appToast.success("Đã chuyển bài giảng sang chương mới.");
      setMoveLesson(null);
      setTargetChapterId("");
    } catch {
      // Mutation handles and displays the API error.
    }
  }

  async function confirmRemoveLesson() {
    if (!removeLesson) return;

    try {
      await relationMutation.mutateAsync(() =>
        curriculumApi.removeLesson(courseId, chapter.id, removeLesson.id),
      );
      appToast.success("Đã gỡ bài giảng khỏi chương.");
      setRemoveLesson(null);
    } catch {
      // Mutation handles and displays the API error.
    }
  }

  async function confirmDeleteChapter() {
    try {
      await chapterMutation.mutateAsync(() =>
        chapterApi.delete(courseId, chapter.id, {
          concurrencyToken: chapter.concurrencyToken,
        }),
      );
      appToast.success("Đã xóa chương học.");
      setDeleteOpen(false);
    } catch {
      // Mutation handles and displays the API error.
    }
  }

  async function confirmRestoreChapter() {
    try {
      await chapterMutation.mutateAsync(() =>
        chapterApi.restore(courseId, chapter.id, {
          concurrencyToken: chapter.concurrencyToken,
        }),
      );
      appToast.success("Đã khôi phục chương học.");
      setRestoreOpen(false);
    } catch {
      // Mutation handles and displays the API error.
    }
  }

  return (
    <>
      <Card className={deleted ? "opacity-70" : undefined}>
        <CardHeader className="flex flex-row items-center justify-between gap-3 py-3">
          <div className="min-w-0">
            <div className="flex flex-wrap items-center gap-2">
              <CardTitle className="truncate">
                Chương {chapter.sortOrder}: {chapter.titleVi}
              </CardTitle>
              {deleted ? <Badge variant="danger">Đã xóa</Badge> : null}
              {!deleted && !chapter.isActive ? (
                <Badge variant="warning">Ngừng hoạt động</Badge>
              ) : null}
            </div>
            <p className="mt-1 text-[10px] text-[#888]">
              ID {chapter.id} · PublicId {chapter.publicId.slice(0, 8)}… · {lessons.length} bài giảng
            </p>
          </div>

          <div className="flex items-center gap-1">
            {deleted ? (
              <Button
                variant="outline"
                size="sm"
                className="gap-2"
                disabled={busy}
                onClick={() => setRestoreOpen(true)}
              >
                <ArchiveRestore size={14} /> Khôi phục
              </Button>
            ) : (
              <>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setIsChapterFormOpen(true)}
                >
                  Sửa
                </Button>
                <Button
                  variant="ghost"
                  size="icon"
                  disabled={busy}
                  onClick={() => setDeleteOpen(true)}
                  aria-label="Xóa chương"
                >
                  <Trash2 size={15} />
                </Button>
              </>
            )}
          </div>
        </CardHeader>

        {!deleted ? (
          <>
            <CardContent className="space-y-2">
              {lessonsQuery.isLoading ? (
                <div className="space-y-2">
                  <Skeleton className="h-12 w-full" />
                  <Skeleton className="h-12 w-full" />
                </div>
              ) : lessonsQuery.error ? (
                <div className="rounded-md border p-3 text-[11px] text-destructive">
                  {normalizeApiError(lessonsQuery.error).message}
                </div>
              ) : lessons.length > 0 ? (
                lessons.map((lesson, index) => (
                  <LessonItem
                    key={lesson.id}
                    lesson={lesson}
                    index={index}
                    total={lessons.length}
                    busy={busy}
                    onEdit={() => setEditingLesson(lesson)}
                    onMoveUp={() => void reorderLesson(index, -1)}
                    onMoveDown={() => void reorderLesson(index, 1)}
                    onMoveChapter={() => {
                      setMoveLesson(lesson);
                      setTargetChapterId("");
                    }}
                    onRemove={() => setRemoveLesson(lesson)}
                  />
                ))
              ) : (
                <p className="py-5 text-center text-[11px] text-[#888]">
                  Chưa có bài giảng trong chương này.
                </p>
              )}
            </CardContent>

            <CardFooter className="flex flex-wrap gap-2">
              <Button
                variant="outline"
                size="sm"
                className="gap-2"
                onClick={() => setIsLessonFormOpen(true)}
              >
                <Plus size={14} /> Tạo bài giảng mới
              </Button>
              <Button
                variant="outline"
                size="sm"
                className="gap-2"
                onClick={() => setAttachOpen(true)}
              >
                <Link2 size={14} /> Gắn bài giảng có sẵn
              </Button>
            </CardFooter>
          </>
        ) : null}
      </Card>

      {isChapterFormOpen ? (
        <ChapterFormDialog
          courseId={courseId}
          chapter={chapter}
          open={isChapterFormOpen}
          onOpenChange={setIsChapterFormOpen}
        />
      ) : null}

      {isLessonFormOpen ? (
        <LessonFormDialog
          courseId={courseId}
          chapterId={chapter.id}
          hskLevelId={courseHskLevelId}
          open={isLessonFormOpen}
          onOpenChange={setIsLessonFormOpen}
        />
      ) : null}

      {editingLesson ? (
        <LessonFormDialog
          courseId={courseId}
          chapterId={chapter.id}
          hskLevelId={courseHskLevelId}
          lesson={editingLesson}
          open={Boolean(editingLesson)}
          onOpenChange={(open) => {
            if (!open) setEditingLesson(null);
          }}
        />
      ) : null}

      <Dialog
        open={attachOpen}
        onOpenChange={setAttachOpen}
        title="Gắn bài giảng có sẵn"
        description="Chọn Lesson chưa thuộc Chapter nào. Không nhập ID thủ công."
        footer={
          <div className="flex justify-end gap-2">
            <Button variant="outline" onClick={() => setAttachOpen(false)}>
              Hủy
            </Button>
            <Button
              loading={relationMutation.isPending}
              disabled={!selectedLessonId}
              onClick={() => void attachExistingLesson()}
            >
              Gắn bài giảng
            </Button>
          </div>
        }
      >
        <div className="space-y-3">
          <Input
            value={attachSearch}
            onChange={(event) => setAttachSearch(event.target.value)}
            placeholder="Tìm theo tên hoặc slug bài giảng..."
          />
          <Select
            value={selectedLessonId}
            onValueChange={setSelectedLessonId}
            options={candidateOptions}
            placeholder={
              candidateQuery.isLoading ? "Đang tải..." : "Chọn bài giảng"
            }
          />
          {!candidateQuery.isLoading && candidateOptions.length === 0 ? (
            <p className="text-[11px] text-muted-foreground">
              Không có bài giảng chưa gắn Chapter phù hợp với từ khóa hiện tại.
            </p>
          ) : null}
        </div>
      </Dialog>

      <Dialog
        open={Boolean(moveLesson)}
        onOpenChange={(open) => {
          if (!open) {
            setMoveLesson(null);
            setTargetChapterId("");
          }
        }}
        title="Chuyển bài giảng sang chương khác"
        description={moveLesson ? `Bài giảng: ${moveLesson.titleVi}` : undefined}
        footer={
          <div className="flex justify-end gap-2">
            <Button variant="outline" onClick={() => setMoveLesson(null)}>
              Hủy
            </Button>
            <Button
              loading={relationMutation.isPending}
              disabled={!targetChapterId}
              onClick={() => void confirmMoveLesson()}
            >
              Chuyển chương
            </Button>
          </div>
        }
      >
        <Select
          value={targetChapterId}
          onValueChange={setTargetChapterId}
          options={targetOptions}
          placeholder="Chọn chương đích"
        />
      </Dialog>

      <Dialog
        open={Boolean(removeLesson)}
        onOpenChange={(open) => {
          if (!open) setRemoveLesson(null);
        }}
        title="Gỡ bài giảng khỏi chương?"
        description={
          removeLesson
            ? `Bài giảng “${removeLesson.titleVi}” sẽ trở thành Lesson chưa thuộc Chapter.`
            : undefined
        }
        footer={
          <div className="flex justify-end gap-2">
            <Button variant="outline" onClick={() => setRemoveLesson(null)}>
              Hủy
            </Button>
            <Button
              loading={relationMutation.isPending}
              onClick={() => void confirmRemoveLesson()}
            >
              Gỡ bài giảng
            </Button>
          </div>
        }
      >
        <p className="text-[12px] text-muted-foreground">
          Thao tác này không xóa Lesson; chỉ xóa quan hệ với Chapter hiện tại.
        </p>
      </Dialog>

      <Dialog
        open={deleteOpen}
        onOpenChange={setDeleteOpen}
        title="Xóa chương học?"
        description={`Chương “${chapter.titleVi}” sẽ được soft-delete và có thể khôi phục.`}
        footer={
          <div className="flex justify-end gap-2">
            <Button variant="outline" onClick={() => setDeleteOpen(false)}>
              Hủy
            </Button>
            <Button
              loading={chapterMutation.isPending}
              onClick={() => void confirmDeleteChapter()}
            >
              Xóa chương
            </Button>
          </div>
        }
      >
        <p className="text-[12px] text-muted-foreground">
          Backend sẽ kiểm tra concurrencyToken trước khi xóa để tránh ghi đè dữ liệu mới hơn.
        </p>
      </Dialog>

      <Dialog
        open={restoreOpen}
        onOpenChange={setRestoreOpen}
        title="Khôi phục chương học?"
        description={`Khôi phục “${chapter.titleVi}” về trạng thái hoạt động.`}
        footer={
          <div className="flex justify-end gap-2">
            <Button variant="outline" onClick={() => setRestoreOpen(false)}>
              Hủy
            </Button>
            <Button
              loading={chapterMutation.isPending}
              onClick={() => void confirmRestoreChapter()}
            >
              Khôi phục
            </Button>
          </div>
        }
      >
        <p className="text-[12px] text-muted-foreground">
          Khôi phục dùng đúng concurrencyToken mới nhất của Chapter đã xóa.
        </p>
      </Dialog>
    </>
  );
}
