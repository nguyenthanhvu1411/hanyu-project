"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { baiGiangApi } from "@/features/bai-giang/api/bai-giang.api";
import { AdminLessonListItem, CreateLessonRequest, UpdateLessonRequest } from "@/features/bai-giang/types/bai-giang.types";

import { Dialog } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";

const schema = z.object({
  titleVi: z.string().min(1, "Vui lòng nhập tên bài giảng"),
  slug: z.string().min(1, "Vui lòng nhập slug"),
  shortDescriptionVi: z.string().optional().nullable(),
  sortOrder: z.number().min(0, "Thứ tự phải lớn hơn hoặc bằng 0"),
  estimatedMinutes: z.number().min(0, "Thời lượng phải lớn hơn hoặc bằng 0"),
  difficulty: z.number().min(0).max(10),
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

export function LessonFormDialog({ courseId, chapterId, hskLevelId, lesson, open, onOpenChange }: LessonFormDialogProps) {
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
          hskLevelId: hskLevelId,
          isFeatured: lesson.isFeatured,
          version: lesson.version,
        };
        return await baiGiangApi.capNhat(lesson.id, payload);
      } else {
        const payload: CreateLessonRequest = {
          ...values,
          courseChapterId: chapterId,
          hskLevelId: hskLevelId,
          isFeatured: false,
        };
        return await baiGiangApi.tao(payload);
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["lessons", "by-chapter", chapterId] });
      onOpenChange(false);
      form.reset();
    },
  });

  const onSubmit = (values: FormData) => {
    mutation.mutate(values);
  };

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
          <Button type="submit" disabled={mutation.isPending} onClick={form.handleSubmit(onSubmit)}>
            {mutation.isPending ? "Đang lưu..." : "Lưu"}
          </Button>
        </div>
      }
    >
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
        <div className="space-y-2">
          <label className="text-sm font-medium leading-none">Tên bài giảng</label>
          <Input placeholder="Nhập tên bài giảng..." {...form.register("titleVi")} />
          {form.formState.errors.titleVi && (
            <p className="text-[0.8rem] font-medium text-destructive">
              {form.formState.errors.titleVi.message}
            </p>
          )}
        </div>

        <div className="space-y-2">
          <label className="text-sm font-medium leading-none">Slug</label>
          <Input placeholder="vd: xin-chao" {...form.register("slug")} />
          {form.formState.errors.slug && (
            <p className="text-[0.8rem] font-medium text-destructive">
              {form.formState.errors.slug.message}
            </p>
          )}
        </div>
        
        <div className="grid grid-cols-2 gap-4">
          <div className="space-y-2">
            <label className="text-sm font-medium leading-none">Thứ tự</label>
            <Input type="number" {...form.register("sortOrder", { valueAsNumber: true })} />
            {form.formState.errors.sortOrder && (
              <p className="text-[0.8rem] font-medium text-destructive">
                {form.formState.errors.sortOrder.message}
              </p>
            )}
          </div>
          <div className="space-y-2">
            <label className="text-sm font-medium leading-none">Thời lượng (phút)</label>
            <Input type="number" {...form.register("estimatedMinutes", { valueAsNumber: true })} />
            {form.formState.errors.estimatedMinutes && (
              <p className="text-[0.8rem] font-medium text-destructive">
                {form.formState.errors.estimatedMinutes.message}
              </p>
            )}
          </div>
        </div>

        <div className="space-y-2">
          <label className="text-sm font-medium leading-none">Mô tả ngắn (tùy chọn)</label>
          <Textarea 
            placeholder="Nhập mô tả..." 
            className="resize-none" 
            {...form.register("shortDescriptionVi")} 
          />
          {form.formState.errors.shortDescriptionVi && (
            <p className="text-[0.8rem] font-medium text-destructive">
              {form.formState.errors.shortDescriptionVi.message}
            </p>
          )}
        </div>
      </form>
    </Dialog>
  );
}
