"use client";

import { useEffect, useState } from "react";

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
      className="rounded-[11px] border border-[#e8e3dc] bg-white p-5"
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
      <div className="mb-5 flex items-start justify-between gap-4">
        <div>
          <h2 className="text-[14px] font-semibold text-[#2f2f2f]">
            {initialValues ? "Chỉnh sửa chủ đề" : "Thêm chủ đề mới"}
          </h2>
          <p className="mt-1 text-[11px] leading-5 text-[#8a8a8a]">
            Chủ đề được dùng chung cho từ vựng và bài giảng. Slug phải duy nhất.
          </p>
        </div>
        <button
          type="button"
          onClick={onCancel}
          className="rounded-[7px] border border-[#e5e0d9] px-3 py-2 text-[11px] text-[#666] hover:bg-[#f8f7f5]"
        >
          Đóng
        </button>
      </div>

      <div className="grid gap-4 lg:grid-cols-2">
        <label className="space-y-1.5">
          <span className="text-[11px] font-medium text-[#555]">Tên chủ đề *</span>
          <input
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
            className="h-[38px] w-full rounded-[7px] border border-[#dfdbd4] bg-white px-3 text-[12px] outline-none transition focus:border-[#ef5b55] focus:ring-2 focus:ring-[#ef241c]/10"
          />
        </label>

        <label className="space-y-1.5">
          <span className="text-[11px] font-medium text-[#555]">Slug *</span>
          <input
            value={values.slug}
            onChange={(event) => {
              setSlugTouched(true);
              setValues((current) => ({
                ...current,
                slug: normalizeSlug(event.target.value),
              }));
            }}
            placeholder="chao-hoi"
            className="h-[38px] w-full rounded-[7px] border border-[#dfdbd4] bg-white px-3 text-[12px] outline-none transition focus:border-[#ef5b55] focus:ring-2 focus:ring-[#ef241c]/10"
          />
        </label>

        <label className="space-y-1.5">
          <span className="text-[11px] font-medium text-[#555]">Thứ tự hiển thị</span>
          <input
            type="number"
            min={0}
            value={values.sortOrder}
            onChange={(event) =>
              setValues((current) => ({
                ...current,
                sortOrder: Number(event.target.value),
              }))
            }
            className="h-[38px] w-full rounded-[7px] border border-[#dfdbd4] bg-white px-3 text-[12px] outline-none transition focus:border-[#ef5b55] focus:ring-2 focus:ring-[#ef241c]/10"
          />
        </label>

        <div className="lg:col-span-2">
          <label className="space-y-1.5">
            <span className="text-[11px] font-medium text-[#555]">Mô tả</span>
            <textarea
              value={values.descriptionVi}
              onChange={(event) =>
                setValues((current) => ({
                  ...current,
                  descriptionVi: event.target.value,
                }))
              }
              rows={4}
              placeholder="Mô tả ngắn nội dung thuộc chủ đề này..."
              className="w-full resize-y rounded-[7px] border border-[#dfdbd4] bg-white px-3 py-2.5 text-[12px] leading-5 outline-none transition focus:border-[#ef5b55] focus:ring-2 focus:ring-[#ef241c]/10"
            />
          </label>
        </div>
      </div>

      <div className="mt-5 flex justify-end gap-2 border-t border-[#eee9e2] pt-4">
        <button
          type="button"
          onClick={onCancel}
          className="h-[36px] rounded-[7px] border border-[#ded9d2] px-4 text-[11px] font-medium text-[#666] hover:bg-[#f8f7f5]"
        >
          Hủy
        </button>
        <button
          type="submit"
          disabled={submitting || !values.nameVi.trim() || !values.slug.trim()}
          className="h-[36px] rounded-[7px] bg-[#ef241c] px-4 text-[11px] font-semibold text-white transition hover:bg-[#d91f18] disabled:cursor-not-allowed disabled:opacity-50"
        >
          {submitting ? "Đang lưu..." : initialValues ? "Lưu thay đổi" : "Tạo chủ đề"}
        </button>
      </div>
    </form>
  );
}
