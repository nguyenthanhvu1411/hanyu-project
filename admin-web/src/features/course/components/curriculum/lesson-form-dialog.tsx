"use client";

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
import { FormField } from "@/components/forms/form-field";
import { FormRow } from "@/components/forms/form-row";
import { Dialog } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";

const schema = z.object({
  titleVi: z.string().min(1, "Vui lòng nhập tên bài giảng"),
  slug: z.string().min(1, "Vui lòng nhập slug"),
  shortDescriptionVi: z.string().optional().nullable(),
  sortOrder: z.number().min(0, "Thứ tự phải lớn hơn hoặc bằng 0"),
  estimatedMinutes: z.number().min(1, "Thời lượng phải từ 1 phút"),
  difficulty: z.number().min(1).max(5),
});

type FormData = z.infer<typeof schema>;

interface LessonFormDialogProps {
  courseId: number;
  chapterId: number;
  hskLevelId: number;
  lesson?: AdminLessonListItem | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function LessonFormDialog({
  chapterId,
  hskLevelId,
  lesson,
  open,
  onOpenChange,
}: LessonFormDialogProps) {
  const queryClient = useQueryClient();

  const form = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: {
      titleVi: lesson?.titleVi || "",
      slug: lesson?.slug || "",
      shortDescriptionVi: lesson?.shortDescriptionVi || "",
      sortOrder: lesson?.sortOrder || 0,
      estimatedMinutes: lesson?.estimatedMinutes || 10,
      difficulty: lesson?.difficulty || 1,
    },
  });

  const isEditing = !!lesson;

  const mutation = useMutation({
    mutationFn: async (values: FormData) => {
      if (isEditing) {
        const payload: UpdateLessonRequest = {
          ...values,
          courseChapterId: chapterId,
          hskLevelId,
          isFeatured: lesson.isFeatured,
          version: lesson.version,
        };
        return lessonApi.capNhat(lesson.id, payload);
      }

      const payload: CreateLessonRequest = {
        ...values,
        courseChapterId: chapterId,
        hskLevelId,
        isFeatured: false,
      };
      return lessonApi.tao(payload);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["lessons", "by-chapter", chapterId] });
      onOpenChange(false);
      form.reset();
    },
  });

  const onSubmit = (values: FormData) => mutation.mutate(values);

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
          <Button type="button" loading={mutation.isPending} onClick={form.handleSubmit(onSubmit)}>
            Lưu
          </Button>
        </div>
      }
    >
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
        <FormField
          label="Tên bài giảng"
          required
          error={form.formState.errors.titleVi?.message}
        >
          <Input placeholder="Nhập tên bài giảng..." {...form.register("titleVi")} />
        </FormField>

        <FormField
          label="Slug"
          required
          error={form.formState.errors.slug?.message}
        >
          <Input placeholder="vd: xin-chao" {...form.register("slug")} />
        </FormField>

        <FormRow columns={3}>
          <FormField label="Thứ tự" error={form.formState.errors.sortOrder?.message}>
            <Input type="number" min={0} {...form.register("sortOrder", { valueAsNumber: true })} />
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
