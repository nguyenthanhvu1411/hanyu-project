"use client";

import { FormEvent, useState } from "react";
import { BookOpenText } from "lucide-react";

import { FormActions } from "@/components/forms/form-actions";
import { FormField } from "@/components/forms/form-field";
import { FormRow } from "@/components/forms/form-row";
import { FormSection } from "@/components/forms/form-section";
import { Input } from "@/components/ui/input";
import { Switch } from "@/components/ui/switch";
import { Textarea } from "@/components/ui/textarea";

import type { CreateChapterRequest } from "../../types/curriculum.types";

interface Props {
  initial?: CreateChapterRequest;
  nextSortOrder?: number;
  saving: boolean;
  onSubmit: (request: CreateChapterRequest) => Promise<unknown>;
  onCancel: () => void;
}

export function ChapterForm({
  initial,
  nextSortOrder = 0,
  saving,
  onSubmit,
  onCancel,
}: Props) {
  const [titleVi, setTitleVi] = useState(initial?.titleVi ?? "");
  const [descriptionVi, setDescriptionVi] = useState(initial?.descriptionVi ?? "");
  const [sortOrder, setSortOrder] = useState(initial?.sortOrder ?? nextSortOrder);
  const [isActive, setActive] = useState(initial?.isActive ?? true);

  const valid = titleVi.trim().length > 0 && Number.isInteger(sortOrder) && sortOrder >= 0;

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!valid || saving) return;

    await onSubmit({
      titleVi: titleVi.trim(),
      descriptionVi: descriptionVi.trim() || null,
      sortOrder,
      isActive,
    });
  }

  return (
    <form onSubmit={submit} className="space-y-4">
      <FormSection
        title={initial ? "Chỉnh sửa chương học" : "Thêm chương học"}
        description="Thông tin chapter được lưu trực tiếp theo CourseChapter contract của backend."
        icon={<BookOpenText size={18} />}
      >
        <FormRow columns={2}>
          <FormField label="Tên chương" required>
            <Input
              value={titleVi}
              onChange={(event) => setTitleVi(event.target.value)}
              maxLength={200}
              placeholder="Ví dụ: Chương 1 - Làm quen"
            />
          </FormField>

          <FormField label="Thứ tự" required>
            <Input
              type="number"
              min={0}
              step={1}
              value={sortOrder}
              onChange={(event) => setSortOrder(Number(event.target.value))}
            />
          </FormField>
        </FormRow>

        <FormField label="Mô tả">
          <Textarea
            value={descriptionVi}
            onChange={(event) => setDescriptionVi(event.target.value)}
            rows={4}
            placeholder="Mô tả ngắn nội dung của chương học."
          />
        </FormField>

        <FormField label="Trạng thái">
          <Switch
            checked={isActive}
            onCheckedChange={setActive}
            label={isActive ? "Đang hoạt động" : "Ngừng hoạt động"}
            description="Chương hoạt động có thể được sử dụng trong curriculum của khóa học."
          />
        </FormField>
      </FormSection>

      <FormActions
        loading={saving}
        disabled={!valid}
        submitText={initial ? "Lưu chương" : "Thêm chương"}
        cancelText="Hủy"
        onCancel={onCancel}
      />
    </form>
  );
}
