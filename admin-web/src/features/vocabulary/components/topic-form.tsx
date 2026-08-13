"use client";

import { useEffect, useState } from "react";

import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";

export interface TopicFormValues {
  slug: string;
  nameVi: string;
  descriptionVi: string;
  sortOrder: number;
}

interface TopicFormProps {
  initialValues?: TopicFormValues;
  submitting?: boolean;
  onSubmit: (values: TopicFormValues) => Promise<void> | void;
  onCancel: () => void;
}

const EMPTY_VALUES: TopicFormValues = {
  slug: "",
  nameVi: "",
  descriptionVi: "",
  sortOrder: 0,
};

function normalizeSlug(value: string) {
  return value
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replace(/đ/g, "d")
    .replace(/Đ/g, "d")
    .toLowerCase()
    .trim()
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "");
}

export function TopicForm({
  initialValues,
  submitting = false,
  onSubmit,
  onCancel,
}: TopicFormProps) {
  const [values, setValues] = useState<TopicFormValues>(
    initialValues ?? EMPTY_VALUES,
  );
  const [slugTouched, setSlugTouched] = useState(Boolean(initialValues?.slug));

  useEffect(() => {
    setValues(initialValues ?? EMPTY_VALUES);
    setSlugTouched(Boolean(initialValues?.slug));
  }, [initialValues]);

  return (
    <form
      onSubmit={async (event) => {
        event.preventDefault();
        if (!values.nameVi.trim() || !values.slug.trim()) return;

        await onSubmit({
          ...values,
          slug: normalizeSlug(values.slug),
          nameVi: values.nameVi.trim(),
          descriptionVi: values.descriptionVi.trim(),
          sortOrder: Number.isFinite(values.sortOrder) ? values.sortOrder : 0,
        });
      }}
    >
      <Card>
        <CardHeader>
          <CardTitle className="text-[16px]">
            {initialValues ? "Chỉnh sửa chủ đề" : "Thêm chủ đề mới"}
          </CardTitle>
          <CardDescription className="text-[13px] leading-5">
            Chủ đề được dùng chung cho từ vựng và bài giảng. Slug phải duy nhất.
          </CardDescription>
        </CardHeader>

        <CardContent className="grid gap-5 p-5 lg:grid-cols-2">
          <label className="space-y-2">
            <span className="text-[13px] font-medium text-[#555]">Tên chủ đề *</span>
            <Input
              value={values.nameVi}
              onChange={(event) => {
                const nameVi = event.target.value;
                setValues((current) => ({
                  ...current,
                  nameVi,
                  slug: slugTouched ? current.slug : normalizeSlug(nameVi),
                }));
              }}
              placeholder="Ví dụ: Chào hỏi, Gia đình, Du lịch..."
              className="h-10 px-3 text-[14px]"
            />
          </label>

          <label className="space-y-2">
            <span className="text-[13px] font-medium text-[#555]">Slug *</span>
            <Input
              value={values.slug}
              onChange={(event) => {
                setSlugTouched(true);
                setValues((current) => ({
                  ...current,
                  slug: normalizeSlug(event.target.value),
                }));
              }}
              placeholder="chao-hoi"
              className="h-10 px-3 text-[14px]"
            />
          </label>

          <label className="space-y-2">
            <span className="text-[13px] font-medium text-[#555]">Thứ tự hiển thị</span>
            <Input
              type="number"
              min={0}
              value={values.sortOrder}
              onChange={(event) =>
                setValues((current) => ({
                  ...current,
                  sortOrder: Number(event.target.value),
                }))
              }
              className="h-10 px-3 text-[14px]"
            />
          </label>

          <label className="space-y-2 lg:col-span-2">
            <span className="text-[13px] font-medium text-[#555]">Mô tả</span>
            <Textarea
              value={values.descriptionVi}
              onChange={(event) =>
                setValues((current) => ({
                  ...current,
                  descriptionVi: event.target.value,
                }))
              }
              rows={5}
              placeholder="Mô tả ngắn nội dung thuộc chủ đề này..."
              className="min-h-[130px] text-[14px] leading-6"
            />
          </label>
        </CardContent>

        <CardFooter className="flex justify-end gap-2 px-5 py-4">
          <Button type="button" variant="outline" size="md" onClick={onCancel} disabled={submitting}>
            Hủy
          </Button>
          <Button
            type="submit"
            variant="primary"
            size="md"
            loading={submitting}
            disabled={!values.nameVi.trim() || !values.slug.trim()}
          >
            {initialValues ? "Lưu thay đổi" : "Tạo chủ đề"}
          </Button>
        </CardFooter>
      </Card>
    </form>
  );
}
