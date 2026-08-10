"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { chuongHocApi, CreateCourseChapterRequest, UpdateCourseChapterRequest } from "@/features/course/api/chuong-hoc.api";
import type { AdminCourseChapter } from "@/features/course/types/course.types";

import { Dialog } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";

const schema = z.object({
  titleVi: z.string().min(1, "Vui lòng nhập tên chương"),
  descriptionVi: z.string().optional().nullable(),
  sortOrder: z.number().min(0, "Thứ tự phải lớn hơn hoặc bằng 0"),
  isActive: z.boolean(),
});

type FormData = z.infer<typeof schema>;

interface ChapterFormDialogProps {
  courseId: number;
  chapter?: AdminCourseChapter | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function ChapterFormDialog({ courseId, chapter, open, onOpenChange }: ChapterFormDialogProps) {
  const queryClient = useQueryClient();

  const form = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: {
      titleVi: chapter?.titleVi || "",
      descriptionVi: chapter?.descriptionVi || "",
      sortOrder: chapter?.sortOrder || 0,
      isActive: chapter?.isActive ?? true,
    },
  });

  const isEditing = !!chapter;

  const mutation = useMutation({
    mutationFn: async (values: FormData) => {
      if (isEditing) {
        const payload: UpdateCourseChapterRequest = {
          ...values,
          concurrencyToken: chapter.concurrencyToken,
        };
        return await chuongHocApi.capNhat(courseId, chapter.id, payload);
      }

      const payload: CreateCourseChapterRequest = values;
      return await chuongHocApi.tao(courseId, payload);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["chapters", courseId] });
      queryClient.invalidateQueries({ queryKey: ["course", courseId] });
      onOpenChange(false);
      form.reset();
    },
  });

  const onSubmit = (values: FormData) => mutation.mutate(values);

  return (
    <Dialog
      open={open}
      onOpenChange={onOpenChange}
      title={isEditing ? "Sửa chương học" : "Thêm chương học mới"}
      footer={
        <div className="flex justify-end gap-2">
          <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>Hủy</Button>
          <Button type="submit" disabled={mutation.isPending} onClick={form.handleSubmit(onSubmit)}>
            {mutation.isPending ? "Đang lưu..." : "Lưu"}
          </Button>
        </div>
      }
    >
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
        <div className="space-y-2">
          <label className="text-sm font-medium leading-none">Tên chương</label>
          <Input placeholder="Nhập tên chương..." {...form.register("titleVi")} />
        </div>
        <div className="space-y-2">
          <label className="text-sm font-medium leading-none">Thứ tự</label>
          <Input type="number" {...form.register("sortOrder", { valueAsNumber: true })} />
        </div>
        <div className="space-y-2">
          <label className="text-sm font-medium leading-none">Mô tả (tùy chọn)</label>
          <Textarea placeholder="Nhập mô tả..." {...form.register("descriptionVi")} />
        </div>
      </form>
    </Dialog>
  );
}
