"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { useMutation, useQueryClient } from "@tanstack/react-query";

import { lessonApi } from "@/features/lesson/api/lesson.api";
import type {
  AdminLessonListItem,
  CreateLessonRequest,
  UpdateLessonRequest,
} from "@/features/lesson/types/lesson.types";
import type { CourseChapterLesson } from "@/features/course/types/curriculum.types";
import { FormField } from "@/components/forms/form-field";
import { FormRow } from "@/components/forms/form-row";
import { Dialog } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";

const schema = z.object({
  titleVi: z.string().min(1, "Vui lòng nhập tên bài giảng"),
  slug: z.string().max(200, "Slug không được vượt quá 200 ký tự"),
  shortDescriptionVi: z.string().optional().nullable(),
  sortOrder: z.number().min(0, "Thứ tự phải lớn hơn hoặc bằng 0"),
  estimatedMinutes: z.number().min(1, "Thời lượng phải từ 1 phút"),
  difficulty: z.number().min(1).max(5),
});

type FormData = z.infer<typeof schema>;
type CurriculumLesson = AdminLessonListItem | CourseChapterLesson;

interface LessonFormDialogProps {
  courseId: number;
  chapterId: number;
  hskLevelId: number;
  lesson?: CurriculumLesson | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

function slugify(value: string) {
  return value
    .trim()
    .toLowerCase()
    .replace(/đ/g, "d")
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "");
}

export function LessonFormDialog({
  courseId,
  chapterId,
  hskLevelId,
  lesson,
  open,
  onOpenChange,
}: LessonFormDialogProps) {
  const queryClient = useQueryClient();
  const [slugManuallyEdited, setSlugManuallyEdited] = useState(Boolean(lesson?.slug));
  const shortDescription =
    lesson && "shortDescriptionVi" in lesson ? lesson.shortDescriptionVi : "";

  const form = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: {
      titleVi: lesson?.titleVi ?? "",
      slug: lesson?.slug ?? "",
      shortDescriptionVi: shortDescription ?? "",
      sortOrder: lesson?.sortOrder ?? 0,
      estimatedMinutes: lesson?.estimatedMinutes ?? 10,
      difficulty: lesson?.difficulty ?? 1,
    },
  });
  const isEditing = Boolean(lesson);
  const canCreateLesson = Number.isSafeInteger(hskLevelId) && hskLevelId > 0;

  const mutation = useMutation({
    mutationFn: async (values: FormData) => {
      if (isEditing && lesson) {
        const current = await lessonApi.getById(lesson.id);
        const payload: UpdateLessonRequest = {
          courseChapterId: chapterId,
          hskLevelId: current.hskLevelId,
          topicId: current.topicId,
          slug: values.slug.trim(),
          titleVi: values.titleVi,
          shortDescriptionVi: values.shortDescriptionVi,
          descriptionVi: current.descriptionVi,
          objectiveVi: current.objectiveVi,
          coverImageUrl: current.coverImageUrl,
          sortOrder: values.sortOrder,
          estimatedMinutes: values.estimatedMinutes,
          difficulty: values.difficulty,
          isFeatured: current.isFeatured,
          version: current.version,
        };
        return lessonApi.update(lesson.id, payload);
      }

      if (!canCreateLesson) {
        throw new Error(
          "Khóa học chưa được gán cấp độ HSK. Hãy cập nhật HSK của khóa học trước khi tạo bài giảng mới.",
        );
      }

      const payload: CreateLessonRequest = {
        ...values,
        slug: values.slug.trim(),
        courseChapterId: chapterId,
        hskLevelId,
        isFeatured: false,
      };
      return lessonApi.create(payload);
    },
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: ["chapter-lessons", courseId, chapterId] }),
        queryClient.invalidateQueries({ queryKey: ["lessons"] }),
        queryClient.invalidateQueries({ queryKey: ["course", courseId] }),
        queryClient.invalidateQueries({ queryKey: ["chapters", courseId] }),
      ]);
      appToast.success(isEditing ? "Đã cập nhật bài giảng." : "Đã tạo bài giảng.");
      onOpenChange(false);
      form.reset();
    },
    onError: (error) => {
      appToast.error(
        isEditing ? "Không thể cập nhật bài giảng" : "Không thể tạo bài giảng",
        error instanceof Error ? error.message : normalizeApiError(error).message,
      );
    },
  });

  return (
    <Dialog
      open={open}
      onOpenChange={onOpenChange}
      title={isEditing ? "Sửa bài giảng" : "Thêm bài giảng mới"}
      footer={
        <div className="flex justify-end gap-2">
          <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
            Hủy
          </Button>
          <Button
            type="button"
            loading={mutation.isPending}
            disabled={!isEditing && !canCreateLesson}
            onClick={form.handleSubmit((values) => mutation.mutate(values))}
          >
            Lưu
          </Button>
        </div>
      }
    >
      <form
        onSubmit={form.handleSubmit((values) => mutation.mutate(values))}
        className="space-y-4"
      >
        {!isEditing && !canCreateLesson ? (
          <div className="rounded-[11px] border border-amber-500/30 bg-amber-500/10 px-3 py-2 text-sm text-amber-700 dark:text-amber-300">
            Khóa học chưa được gán cấp độ HSK. Hãy cập nhật HSK của khóa học trước khi tạo bài giảng mới.
          </div>
        ) : null}

        <FormField
          label="Tên bài giảng"
          required
          error={form.formState.errors.titleVi?.message}
        >
          <Input
            placeholder="Nhập tên bài giảng..."
            {...form.register("titleVi", {
              onChange: (event) => {
                if (!slugManuallyEdited) {
                  form.setValue("slug", slugify(event.target.value), { shouldDirty: true });
                }
              },
            })}
          />
        </FormField>
        <FormField
          label="Đường dẫn (slug)"
          description="Không bắt buộc. Để trống, backend tự sinh từ tên bài giảng."
          error={form.formState.errors.slug?.message}
        >
          <Input
            placeholder="Tự sinh, ví dụ: xin-chao"
            {...form.register("slug", {
              onChange: (event) => setSlugManuallyEdited(event.target.value.trim().length > 0),
              onBlur: (event) => {
                const normalized = slugify(event.target.value);
                if (normalized) form.setValue("slug", normalized, { shouldDirty: true });
              },
            })}
          />
        </FormField>
        <FormRow columns={3}>
          <FormField label="Thứ tự" error={form.formState.errors.sortOrder?.message}>
            <Input
              type="number"
              min={0}
              {...form.register("sortOrder", { valueAsNumber: true })}
            />
          </FormField>
          <FormField
            label="Thời lượng (phút)"
            error={form.formState.errors.estimatedMinutes?.message}
          >
            <Input
              type="number"
              min={1}
              {...form.register("estimatedMinutes", { valueAsNumber: true })}
            />
          </FormField>
          <FormField label="Độ khó" error={form.formState.errors.difficulty?.message}>
            <Input
              type="number"
              min={1}
              max={5}
              {...form.register("difficulty", { valueAsNumber: true })}
            />
          </FormField>
        </FormRow>
        <FormField
          label="Mô tả ngắn"
          error={form.formState.errors.shortDescriptionVi?.message}
        >
          <Textarea placeholder="Nhập mô tả..." {...form.register("shortDescriptionVi")} />
        </FormField>
      </form>
    </Dialog>
  );
}
