"use client";

import { useCallback, useEffect, useState } from "react";

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
import { SlugInput, type SlugValidationState } from "@/components/ui/slug-input";
import { Textarea } from "@/components/ui/textarea";
import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";
import { slugify } from "@/lib/utils/slug";

export interface TopicFormValues {
  slug: string;
  nameVi: string;
  descriptionVi: string;
  sortOrder: number;
}

interface TopicSlugOption {
  id: number;
  slug: string;
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

export function TopicForm({
  initialValues,
  submitting = false,
  onSubmit,
  onCancel,
}: TopicFormProps) {
  const [values, setValues] = useState<TopicFormValues>(
    initialValues ?? EMPTY_VALUES,
  );
  const [slugValidation, setSlugValidation] = useState<SlugValidationState>({
    checking: false,
    error: null,
  });

  useEffect(() => {
    setValues(initialValues ?? EMPTY_VALUES);
    setSlugValidation({ checking: false, error: null });
  }, [initialValues]);

  const validateTopicSlug = useCallback(
    async (slug: string) => {
      const normalized = slug.toLowerCase();
      if (initialValues?.slug.toLowerCase() === normalized) return null;

      const topics = await apiClient<TopicSlugOption[]>(API_ENDPOINTS.VOCABULARY.TOPICS);
      const duplicated = topics.some((topic) => topic.slug.toLowerCase() === normalized);
      return duplicated ? "Slug đã tồn tại ở một chủ đề khác." : null;
    },
    [initialValues?.slug],
  );

  return (
    <form
      onSubmit={async (event) => {
        event.preventDefault();

        const finalSlug = slugify(values.slug || values.nameVi);
        if (
          !values.nameVi.trim() ||
          !finalSlug ||
          slugValidation.checking ||
          slugValidation.error
        ) {
          return;
        }

        await onSubmit({
          ...values,
          slug: finalSlug,
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
              onChange={(event) =>
                setValues((current) => ({
                  ...current,
                  nameVi: event.target.value,
                }))
              }
              placeholder="Ví dụ: Chào hỏi, Gia đình, Du lịch..."
              className="h-10 px-3 text-[14px]"
            />
          </label>

          <SlugInput
            value={values.slug}
            sourceValue={values.nameVi}
            mode={initialValues ? "edit" : "create"}
            required
            placeholder="chao-hoi"
            previewPrefix="/chu-de/"
            validateSlug={validateTopicSlug}
            onValidationChange={setSlugValidation}
            onChange={(slug) =>
              setValues((current) => ({
                ...current,
                slug,
              }))
            }
          />

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
            disabled={
              !values.nameVi.trim() ||
              slugValidation.checking ||
              Boolean(slugValidation.error)
            }
          >
            {initialValues ? "Lưu thay đổi" : "Tạo chủ đề"}
          </Button>
        </CardFooter>
      </Card>
    </form>
  );
}
