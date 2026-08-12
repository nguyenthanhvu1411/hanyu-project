"use client";

import { useEffect, useState } from "react";

export interface PartOfSpeechFormValues {
  code: string;
  nameVi: string;
  nameEn: string;
}

interface PartOfSpeechFormProps {
  initialValues?: PartOfSpeechFormValues;
  submitting?: boolean;
  onSubmit: (values: PartOfSpeechFormValues) => Promise<void> | void;
  onCancel: () => void;
}

const EMPTY_VALUES: PartOfSpeechFormValues = {
  code: "",
  nameVi: "",
  nameEn: "",
};

export function PartOfSpeechForm({
  initialValues,
  submitting = false,
  onSubmit,
  onCancel,
}: PartOfSpeechFormProps) {
  const [values, setValues] = useState<PartOfSpeechFormValues>(initialValues ?? EMPTY_VALUES);
  const [validationError, setValidationError] = useState<string | null>(null);

  useEffect(() => {
    setValues(initialValues ?? EMPTY_VALUES);
    setValidationError(null);
  }, [initialValues]);

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const code = values.code.trim().toLowerCase();
    const nameVi = values.nameVi.trim();
    const nameEn = values.nameEn.trim();

    if (!code || !nameVi) {
      setValidationError("Mã từ loại và tên tiếng Việt là bắt buộc.");
      return;
    }

    setValidationError(null);
    await onSubmit({ code, nameVi, nameEn });
  }

  return (
    <form
      onSubmit={handleSubmit}
      className="rounded-[11px] border border-[#e8e3dc] bg-white p-4"
    >
      <div className="mb-4">
        <h2 className="text-[13px] font-semibold text-[#333]">
          {initialValues ? "Chỉnh sửa từ loại" : "Thêm từ loại"}
        </h2>
        <p className="mt-1 text-[11px] text-[#888]">
          Từ loại được dùng làm dữ liệu nền khi tạo và chỉnh sửa từ vựng.
        </p>
      </div>

      {validationError && (
        <div className="mb-3 rounded-[7px] border border-[#f0cfcb] bg-[#fff5f4] px-3 py-2 text-[11px] text-[#b9433d]">
          {validationError}
        </div>
      )}

      <div className="grid gap-4 md:grid-cols-3">
        <label className="space-y-1.5">
          <span className="text-[11px] font-medium text-[#555]">Mã từ loại *</span>
          <input
            value={values.code}
            onChange={(event) => setValues((current) => ({ ...current, code: event.target.value }))}
            placeholder="noun, verb, adjective..."
            className="h-[38px] w-full rounded-[7px] border border-[#dfdbd4] px-3 text-[11px] outline-none focus:border-[#ef5b55]"
          />
        </label>

        <label className="space-y-1.5">
          <span className="text-[11px] font-medium text-[#555]">Tên tiếng Việt *</span>
          <input
            value={values.nameVi}
            onChange={(event) => setValues((current) => ({ ...current, nameVi: event.target.value }))}
            placeholder="Danh từ"
            className="h-[38px] w-full rounded-[7px] border border-[#dfdbd4] px-3 text-[11px] outline-none focus:border-[#ef5b55]"
          />
        </label>

        <label className="space-y-1.5">
          <span className="text-[11px] font-medium text-[#555]">Tên tiếng Anh</span>
          <input
            value={values.nameEn}
            onChange={(event) => setValues((current) => ({ ...current, nameEn: event.target.value }))}
            placeholder="Noun"
            className="h-[38px] w-full rounded-[7px] border border-[#dfdbd4] px-3 text-[11px] outline-none focus:border-[#ef5b55]"
          />
        </label>
      </div>

      <div className="mt-4 flex justify-end gap-2">
        <button
          type="button"
          disabled={submitting}
          onClick={onCancel}
          className="h-[36px] rounded-[7px] border border-[#ddd8d1] px-4 text-[11px] font-medium text-[#555] hover:bg-[#f7f6f3] disabled:opacity-50"
        >
          Hủy
        </button>
        <button
          type="submit"
          disabled={submitting}
          className="h-[36px] rounded-[7px] bg-[#ef241c] px-4 text-[11px] font-semibold text-white hover:bg-[#d91f18] disabled:opacity-50"
        >
          {submitting ? "Đang lưu..." : initialValues ? "Lưu thay đổi" : "Tạo từ loại"}
        </button>
      </div>
    </form>
  );
}
