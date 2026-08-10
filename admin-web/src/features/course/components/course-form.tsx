"use client";

import { FormEvent, useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { BookOpen, ImageIcon, Settings2 } from "lucide-react";

import { ErrorState } from "@/components/common/error-state";
import { FormActions } from "@/components/forms/form-actions";
import { FormField } from "@/components/forms/form-field";
import { FormRow } from "@/components/forms/form-row";
import { FormSection } from "@/components/forms/form-section";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";
import { Textarea } from "@/components/ui/textarea";
import { appToast } from "@/components/ui/toast";
import type { AdminHskLevelDto } from "@/dto/learning/hsk-level.dto";
import { learningApi } from "@/features/learning/learning.api";
import { normalizeApiError } from "@/lib/api/api-error";

import { courseApi } from "../api/course.api";
import type { AdminCourseDetail, CreateCourseRequest } from "../types/course.types";

interface CourseFormProps {
  courseId?: number;
}

const EMPTY_FORM: CreateCourseRequest = {
  code: "",
  slug: "",
  titleVi: "",
  shortDescriptionVi: "",
  descriptionVi: "",
  hskLevelId: null,
  coverImageUrl: "",
  sortOrder: 0,
  estimatedMinutes: null,
  isFeatured: false,
};

function normalizeRequest(form: CreateCourseRequest): CreateCourseRequest {
  return {
    code: form.code.trim().toUpperCase(),
    slug: form.slug.trim(),
    titleVi: form.titleVi.trim(),
    shortDescriptionVi: form.shortDescriptionVi?.trim() || null,
    descriptionVi: form.descriptionVi?.trim() || null,
    hskLevelId: form.hskLevelId || null,
    coverImageUrl: form.coverImageUrl?.trim() || null,
    sortOrder: form.sortOrder,
    estimatedMinutes: form.estimatedMinutes || null,
    isFeatured: form.isFeatured,
  };
}

export function CourseForm({ courseId }: CourseFormProps) {
  const router = useRouter();
  const editing = Number.isSafeInteger(courseId) && Number(courseId) > 0;

  const [detail, setDetail] = useState<AdminCourseDetail | null>(null);
  const [form, setForm] = useState<CreateCourseRequest>(EMPTY_FORM);
  const [hskLevels, setHskLevels] = useState<AdminHskLevelDto[]>([]);
  const [loading, setLoading] = useState(editing);
  const [hskLoading, setHskLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<Error | null>(null);
  const [hskError, setHskError] = useState<string | null>(null);

  useEffect(() => {
    let active = true;

    setHskLoading(true);
    setHskError(null);

    void learningApi.hskLevels
      .list()
      .then((items) => {
        if (!active) return;
        setHskLevels([...items].sort((a, b) => a.sortOrder - b.sortOrder || a.id - b.id));
      })
      .catch((caught) => {
        if (!active) return;
        const apiError = normalizeApiError(caught);
        setHskError(apiError.message || "Không thể tải danh mục HSK.");
      })
      .finally(() => {
        if (active) setHskLoading(false);
      });

    return () => {
      active = false;
    };
  }, []);

  useEffect(() => {
    if (!editing || !courseId) return;

    let active = true;
    setLoading(true);
    setError(null);

    void courseApi
      .getById(courseId)
      .then((course) => {
        if (!active) return;

        setDetail(course);
        setForm({
          code: course.code,
          slug: course.slug,
          titleVi: course.titleVi,
          shortDescriptionVi: course.shortDescriptionVi ?? "",
          descriptionVi: course.descriptionVi ?? "",
          hskLevelId: course.hskLevelId ?? null,
          coverImageUrl: course.coverImageUrl ?? "",
          sortOrder: course.sortOrder,
          estimatedMinutes: course.estimatedMinutes ?? null,
          isFeatured: course.isFeatured,
        });
      })
      .catch((caught) =>
        setError(
          caught instanceof Error
            ? caught
            : new Error("Không thể tải khóa học."),
        ),
      )
      .finally(() => {
        if (active) setLoading(false);
      });

    return () => {
      active = false;
    };
  }, [courseId, editing]);

  const hskOptions = useMemo(
    () =>
      hskLevels.map((item) => ({
        value: String(item.id),
        label: `${item.code} — ${item.nameVi}`,
        description: item.isActive
          ? `Thứ tự hiển thị: ${item.sortOrder}`
          : "Đang tạm ngưng",
        disabled: !item.isActive && item.id !== form.hskLevelId,
      })),
    [form.hskLevelId, hskLevels],
  );

  const valid = useMemo(
    () =>
      form.code.trim().length > 0 &&
      form.slug.trim().length > 0 &&
      form.titleVi.trim().length > 0 &&
      Number.isInteger(form.sortOrder) &&
      form.sortOrder >= 0,
    [form],
  );

  function setField<K extends keyof CreateCourseRequest>(
    key: K,
    value: CreateCourseRequest[K],
  ) {
    setForm((current) => ({ ...current, [key]: value }));
  }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!valid || saving) return;

    setSaving(true);
    setError(null);

    try {
      if (editing && courseId && detail) {
        await courseApi.update(courseId, {
          ...normalizeRequest(form),
          concurrencyToken: detail.concurrencyToken,
        });
        appToast.success("Cập nhật khóa học thành công.");
        router.push(`/khoa-hoc/${courseId}`);
      } else {
        const created = await courseApi.create(normalizeRequest(form));
        appToast.success("Tạo khóa học thành công.");
        router.replace(`/khoa-hoc/${created.id}`);
      }

      router.refresh();
    } catch (caught) {
      const apiError = normalizeApiError(caught);
      setError(new Error(apiError.message));
      appToast.error(
        editing ? "Không thể cập nhật khóa học" : "Không thể tạo khóa học",
        apiError.message,
      );
    } finally {
      setSaving(false);
    }
  }

  if (loading) {
    return (
      <div className="space-y-4">
        <div className="h-[220px] animate-pulse rounded-[11px] bg-[#f3f1ed]" />
        <div className="h-[180px] animate-pulse rounded-[11px] bg-[#f3f1ed]" />
      </div>
    );
  }

  if (error && editing && !detail) {
    return (
      <ErrorState
        title="Không thể tải khóa học"
        description={error.message}
        onRetry={() => window.location.reload()}
      />
    );
  }

  return (
    <form onSubmit={submit} className="space-y-5">
      {error ? (
        <ErrorState title="Không thể lưu khóa học" description={error.message} />
      ) : null}

      <FormSection
        title="Thông tin khóa học"
        description="Thông tin định danh và nội dung chính của khóa học."
        icon={<BookOpen size={18} />}
      >
        <FormRow columns={2}>
          <FormField label="Mã khóa học" required>
            <Input
              value={form.code}
              onChange={(e) => setField("code", e.target.value)}
              maxLength={50}
              placeholder="Ví dụ: HSK1-A"
            />
          </FormField>
          <FormField label="Slug" required>
            <Input
              value={form.slug}
              onChange={(e) => setField("slug", e.target.value)}
              maxLength={200}
              placeholder="hsk-1-can-ban"
            />
          </FormField>
        </FormRow>

        <FormField label="Tên khóa học" required>
          <Input
            value={form.titleVi}
            onChange={(e) => setField("titleVi", e.target.value)}
            maxLength={250}
          />
        </FormField>

        <FormField label="Mô tả ngắn">
          <Textarea
            value={form.shortDescriptionVi ?? ""}
            onChange={(e) => setField("shortDescriptionVi", e.target.value)}
            rows={3}
          />
        </FormField>

        <FormField label="Mô tả chi tiết">
          <Textarea
            value={form.descriptionVi ?? ""}
            onChange={(e) => setField("descriptionVi", e.target.value)}
            rows={6}
          />
        </FormField>
      </FormSection>

      <FormSection
        title="Phân loại và hiển thị"
        description="Thiết lập HSK, thứ tự, thời lượng và trạng thái nổi bật."
        icon={<Settings2 size={18} />}
      >
        <FormRow columns={3}>
          <FormField
            label="Cấp độ HSK"
            description="Chọn từ danh mục HSK đã cấu hình trong hệ thống."
            error={hskError ?? undefined}
          >
            <Select
              value={form.hskLevelId ? String(form.hskLevelId) : ""}
              onValueChange={(value) =>
                setField("hskLevelId", value ? Number(value) : null)
              }
              options={hskOptions}
              placeholder={hskLoading ? "Đang tải danh mục HSK..." : "Chọn cấp độ HSK"}
              disabled={hskLoading}
              error={Boolean(hskError)}
              clearable
            />
          </FormField>

          <FormField label="Thứ tự" required>
            <Input
              type="number"
              min={0}
              step={1}
              value={form.sortOrder}
              onChange={(e) => setField("sortOrder", Number(e.target.value))}
            />
          </FormField>

          <FormField label="Thời lượng (phút)">
            <Input
              type="number"
              min={1}
              step={1}
              value={form.estimatedMinutes ?? ""}
              onChange={(e) =>
                setField(
                  "estimatedMinutes",
                  e.target.value ? Number(e.target.value) : null,
                )
              }
            />
          </FormField>
        </FormRow>

        <FormField label="Nổi bật">
          <Switch
            checked={form.isFeatured}
            onCheckedChange={(value) => setField("isFeatured", value)}
            label={form.isFeatured ? "Khóa học nổi bật" : "Khóa học thường"}
          />
        </FormField>
      </FormSection>

      <FormSection
        title="Ảnh bìa"
        description="URL ảnh được backend lưu trong CoverImageUrl."
        icon={<ImageIcon size={18} />}
      >
        <FormField label="URL ảnh bìa">
          <Input
            type="url"
            value={form.coverImageUrl ?? ""}
            onChange={(e) => setField("coverImageUrl", e.target.value)}
            placeholder="https://..."
          />
        </FormField>
      </FormSection>

      <FormActions
        loading={saving}
        disabled={!valid}
        submitText={editing ? "Lưu thay đổi" : "Tạo khóa học"}
        onCancel={() =>
          router.push(editing && courseId ? `/khoa-hoc/${courseId}` : "/khoa-hoc")
        }
      />
    </form>
  );
}
