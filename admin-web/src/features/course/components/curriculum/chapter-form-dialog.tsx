"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { chuongHocApi, CreateCourseChapterRequest, UpdateCourseChapterRequest } from "@/features/course/api/chuong-hoc.api";
import type { AdminCourseChapter } from "@/features/course/types/course.types";

import { FormField } from "@/components/forms/form-field";
import { FormRow } from "@/components/forms/form-row";
import { Dialog } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Switch } from "@/components/ui/switch";
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
  const isActive = form.watch("isActive");

  const mutation = useMutation({
    mutationFn: async (values: FormData) => {
      if (isEditing && chapter) {
        const current = await chuongHocApi.chiTiet(courseId, chapter.id);
        const payload: UpdateCourseChapterRequest = {
          ...values,
          concurrencyToken: current.concurrencyToken,
        };
        return chuongHocApi.capNhat(courseId, chapter.id, payload);
      }

      const payload: CreateCourseChapterRequest = values;
      return chuongHocApi.tao(courseId, payload);
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
        <FormField label="Tên chương" required error={form.formState.errors.titleVi?.message}>
          <Input placeholder="Nhập tên chương..." {...form.register("titleVi")} />
        </FormField>

        <FormRow columns={2}>
          <FormField label="Thứ tự" error={form.formState.errors.sortOrder?.message}>
            <Input type="number" min={0} {...form.register("sortOrder", { valueAsNumber: true })} />
          </FormField>

          <FormField label="Trạng thái">
            <Switch
              checked={isActive}
              onCheckedChange={(checked) => form.setValue("isActive", checked, { shouldDirty: true })}
              label={isActive ? "Đang hoạt động" : "Ngừng hoạt động"}
            />
          </FormField>
        </FormRow>

        <FormField label="Mô tả" error={form.formState.errors.descriptionVi?.message}>
          <Textarea placeholder="Nhập mô tả..." {...form.register("descriptionVi")} />
        </FormField>
      </form>
    </Dialog>
  );
}
