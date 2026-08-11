"use client";

import { FormEvent, useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import {
  Archive,
  BookOpenText,
  CheckCircle2,
  ImageIcon,
  Loader2,
  RotateCcw,
  Rocket,
  Send,
  Settings2,
  ShieldCheck,
} from "lucide-react";
import { toast } from "sonner";

import { CoverImageField } from "@/components/forms/cover-image-field";
import { FormActions } from "@/components/forms/form-actions";
import { FormField } from "@/components/forms/form-field";
import { FormRow } from "@/components/forms/form-row";
import { FormSection } from "@/components/forms/form-section";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";
import { Textarea } from "@/components/ui/textarea";
import { PERMISSIONS } from "@/constants/permission.constants";
import type { AdminHskLevelDto } from "@/dto/learning/hsk-level.dto";
import { learningApi } from "@/features/learning/learning.api";
import { ContentStatus, getContentStatusLabel } from "@/lib/constants/content-status";
import { PermissionGuard } from "@/security/permission-guard";

import { lessonApi } from "../api/lesson.api";
import type { AdminLessonDetail, CreateLessonRequest } from "../types/lesson.types";

interface LessonEditorProps {
  lessonId?: number;
}

type WorkflowAction = "review" | "approve" | "publish" | "archive" | "restore";

const EMPTY_FORM: CreateLessonRequest = {
  courseChapterId: null,
  hskLevelId: 0,
  topicId: null,
  slug: "",
  titleVi: "",
  shortDescriptionVi: "",
  descriptionVi: "",
  objectiveVi: "",
  coverImageUrl: "",
  sortOrder: 0,
  estimatedMinutes: 15,
  difficulty: 1,
  isFeatured: false,
};

function normalizeRequest(form: CreateLessonRequest): CreateLessonRequest {
  return {
    ...form,
    courseChapterId: form.courseChapterId || null,
    topicId: form.topicId || null,
    slug: form.slug.trim(),
    titleVi: form.titleVi.trim(),
    shortDescriptionVi: form.shortDescriptionVi?.trim() || null,
    descriptionVi: form.descriptionVi?.trim() || null,
    objectiveVi: form.objectiveVi?.trim() || null,
    coverImageUrl: form.coverImageUrl?.trim() || null,
  };
}

export function LessonEditor({ lessonId }: LessonEditorProps) {
  const router = useRouter();
  const editing = Number.isSafeInteger(lessonId) && Number(lessonId) > 0;

  const [detail, setDetail] = useState<AdminLessonDetail | null>(null);
  const [form, setForm] = useState<CreateLessonRequest>(EMPTY_FORM);
  const [hskLevels, setHskLevels] = useState<AdminHskLevelDto[]>([]);
  const [loading, setLoading] = useState(editing);
  const [hskLoading, setHskLoading] = useState(true);
  const [hskError, setHskError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);
  const [workflowLoading, setWorkflowLoading] = useState<string | null>(null);
  const [validationMessages, setValidationMessages] = useState<string[]>([]);

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
      .catch((error) => {
        if (!active) return;
        setHskError(error instanceof Error ? error.message : "Không thể tải danh mục HSK.");
      })
      .finally(() => {
        if (active) setHskLoading(false);
      });

    return () => {
      active = false;
    };
  }, []);

  useEffect(() => {
    if (!editing || !lessonId) return;

    let active = true;

    void lessonApi
      .getById(lessonId)
      .then((lesson) => {
        if (!active) return;

        setDetail(lesson);
        setForm({
          courseChapterId: lesson.courseChapterId ?? null,
          hskLevelId: lesson.hskLevelId,
          topicId: lesson.topicId ?? null,
          slug: lesson.slug,
          titleVi: lesson.titleVi,
          shortDescriptionVi: lesson.shortDescriptionVi ?? "",
          descriptionVi: lesson.descriptionVi ?? "",
          objectiveVi: lesson.objectiveVi ?? "",
          coverImageUrl: lesson.coverImageUrl ?? "",
          sortOrder: lesson.sortOrder,
          estimatedMinutes: lesson.estimatedMinutes,
          difficulty: lesson.difficulty,
          isFeatured: lesson.isFeatured,
        });
      })
      .catch((error) =>
        toast.error(error instanceof Error ? error.message : "Không thể tải bài giảng."),
      )
      .finally(() => {
        if (active) setLoading(false);
      });

    return () => {
      active = false;
    };
  }, [editing, lessonId]);

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

  const canSubmit = useMemo(
    () =>
      form.titleVi.trim().length > 0 &&
      form.slug.trim().length > 0 &&
      Number.isSafeInteger(form.hskLevelId) &&
      form.hskLevelId > 0 &&
      Number.isInteger(form.sortOrder) &&
      form.sortOrder >= 0 &&
      Number.isInteger(form.estimatedMinutes) &&
      form.estimatedMinutes > 0 &&
      Number.isInteger(form.difficulty) &&
      form.difficulty >= 1 &&
      form.difficulty <= 5,
    [form],
  );

  function setField<K extends keyof CreateLessonRequest>(
    key: K,
    value: CreateLessonRequest[K],
  ) {
    setForm((current) => ({ ...current, [key]: value }));
  }

  async function save(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!canSubmit || saving) return;

    setSaving(true);

    try {
      if (lessonId && detail) {
        const updated = await lessonApi.update(lessonId, {
          ...normalizeRequest(form),
          version: detail.version,
        });
        setDetail(updated);
        setForm((current) => ({ ...current, coverImageUrl: updated.coverImageUrl ?? current.coverImageUrl }));
        toast.success("Đã cập nhật bài giảng.");
      } else {
        const created = await lessonApi.create(normalizeRequest(form));
        toast.success("Đã tạo bài giảng.");
        router.replace(`/bai-giang/${created.id}`);
      }
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể lưu bài giảng.");
    } finally {
      setSaving(false);
    }
  }

  async function validateLesson() {
    if (!lessonId) return;

    setWorkflowLoading("validate");
    setValidationMessages([]);

    try {
      const result = await lessonApi.validate(lessonId);
      setValidationMessages([...(result.errors ?? []), ...(result.warnings ?? [])]);
      result.isValid
        ? toast.success("Bài giảng hợp lệ để tiếp tục quy trình.")
        : toast.error("Bài giảng còn lỗi cần xử lý.");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể kiểm tra bài giảng.");
    } finally {
      setWorkflowLoading(null);
    }
  }

  async function workflow(action: WorkflowAction) {
    if (!lessonId || !detail || workflowLoading) return;

    setWorkflowLoading(action);

    try {
      const request = { version: detail.version };
      const updated =
        action === "review"
          ? await lessonApi.submitReview(lessonId, request)
          : action === "approve"
            ? await lessonApi.approve(lessonId, request)
            : action === "publish"
              ? await lessonApi.publish(lessonId, request)
              : action === "archive"
                ? await lessonApi.archive(lessonId, request)
                : await lessonApi.restore(lessonId, request);

      setDetail(updated);
      toast.success("Đã cập nhật trạng thái bài giảng.");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể cập nhật trạng thái.");
    } finally {
      setWorkflowLoading(null);
    }
  }

  if (loading) {
    return (
      <div className="flex min-h-[320px] items-center justify-center rounded-[11px] border border-[#e8e3dc] bg-white">
        <Loader2 className="animate-spin text-[#777]" size={22} />
      </div>
    );
  }

  return (
    <div className="grid gap-5 xl:grid-cols-[minmax(0,1fr)_320px]">
      <form onSubmit={save} className="space-y-5">
        <FormSection
          title="Thông tin bài giảng"
          description="Thông tin định danh, phân loại và nội dung mô tả của Lesson."
          icon={<BookOpenText size={18} />}
        >
          <FormField label="Tên bài giảng" required>
            <Input value={form.titleVi} onChange={(e) => setField("titleVi", e.target.value)} />
          </FormField>

          <FormRow columns={2}>
            <FormField label="Slug" required>
              <Input value={form.slug} onChange={(e) => setField("slug", e.target.value)} />
            </FormField>

            <FormField
              label="Cấp độ HSK"
              required
              description="Chọn từ danh mục HSK đã cấu hình trong hệ thống."
              error={hskError ?? undefined}
            >
              <Select
                value={form.hskLevelId > 0 ? String(form.hskLevelId) : ""}
                onValueChange={(value) => setField("hskLevelId", Number(value))}
                options={hskOptions}
                placeholder={hskLoading ? "Đang tải danh mục HSK..." : "Chọn cấp độ HSK"}
                disabled={hskLoading}
                error={Boolean(hskError)}
              />
            </FormField>
          </FormRow>

          <FormRow columns={2}>
            <FormField label="Course Chapter ID">
              <Input
                type="number"
                min={1}
                value={form.courseChapterId ?? ""}
                onChange={(e) =>
                  setField("courseChapterId", e.target.value ? Number(e.target.value) : null)
                }
              />
            </FormField>
            <FormField label="Topic ID">
              <Input
                type="number"
                min={1}
                value={form.topicId ?? ""}
                onChange={(e) => setField("topicId", e.target.value ? Number(e.target.value) : null)}
              />
            </FormField>
          </FormRow>

          <FormField label="Mô tả ngắn">
            <Textarea
              value={form.shortDescriptionVi ?? ""}
              onChange={(e) => setField("shortDescriptionVi", e.target.value)}
              rows={3}
            />
          </FormField>

          <FormField label="Mục tiêu bài học">
            <Textarea
              value={form.objectiveVi ?? ""}
              onChange={(e) => setField("objectiveVi", e.target.value)}
              rows={4}
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
          title="Thiết lập học tập"
          description="Thứ tự, thời lượng, độ khó và trạng thái nổi bật."
          icon={<Settings2 size={18} />}
        >
          <FormRow columns={3}>
            <FormField label="Thứ tự" required>
              <Input
                type="number"
                min={0}
                value={form.sortOrder}
                onChange={(e) => setField("sortOrder", Number(e.target.value))}
              />
            </FormField>
            <FormField label="Thời lượng (phút)" required>
              <Input
                type="number"
                min={1}
                value={form.estimatedMinutes}
                onChange={(e) => setField("estimatedMinutes", Number(e.target.value))}
              />
            </FormField>
            <FormField label="Độ khó" required>
              <Input
                type="number"
                min={1}
                max={5}
                value={form.difficulty}
                onChange={(e) => setField("difficulty", Number(e.target.value))}
              />
            </FormField>
          </FormRow>

          <FormField label="Nổi bật">
            <Switch
              checked={form.isFeatured}
              onCheckedChange={(value) => setField("isFeatured", value)}
              label={form.isFeatured ? "Bài giảng nổi bật" : "Bài giảng thường"}
            />
          </FormField>
        </FormSection>

        <FormSection
          title="Ảnh bìa"
          description="Nhập URL hoặc tải file ảnh; hệ thống lưu tham chiếu Storage ổn định vào CoverImageUrl."
          icon={<ImageIcon size={18} />}
        >
          <CoverImageField
            value={form.coverImageUrl}
            onChange={(value) => setField("coverImageUrl", value)}
            disabled={saving}
          />
        </FormSection>

        <PermissionGuard
          permission={editing ? PERMISSIONS.LESSONS.UPDATE : PERMISSIONS.LESSONS.CREATE}
          fallback={null}
        >
          <FormActions
            loading={saving}
            disabled={!canSubmit}
            submitText={editing ? "Lưu thay đổi" : "Tạo bài giảng"}
            onCancel={() =>
              router.push(editing && lessonId ? `/bai-giang/${lessonId}` : "/bai-giang")
            }
          />
        </PermissionGuard>
      </form>

      <aside className="space-y-4">
        {detail ? (
          <FormSection title="Trạng thái bài giảng">
            <div className="text-[15px] font-semibold text-[#292929]">
              {getContentStatusLabel(detail.status)}
            </div>
            <div className="text-[11px] text-[#888]">
              Phiên bản v{detail.version} · PublicId {detail.publicId}
            </div>
            <div className="grid grid-cols-2 gap-2">
              <Metric label="Nội dung" value={detail.sectionCount} />
              <Metric label="Từ vựng" value={detail.vocabularyCount} />
              <Metric label="Tài liệu" value={detail.assetCount} />
              <Metric label="Tiên quyết" value={detail.prerequisiteCount} />
            </div>
          </FormSection>
        ) : null}

        {detail ? (
          <FormSection title="Quy trình xuất bản">
            <div className="space-y-2">
              <Button
                type="button"
                variant="outline"
                className="w-full justify-start gap-2"
                onClick={() => void validateLesson()}
                disabled={Boolean(workflowLoading)}
              >
                <ShieldCheck size={15} />
                Kiểm tra hợp lệ
              </Button>

              {detail.status === ContentStatus.Draft ? (
                <Button
                  type="button"
                  variant="outline"
                  className="w-full justify-start gap-2"
                  onClick={() => void workflow("review")}
                  disabled={Boolean(workflowLoading)}
                >
                  <Send size={15} />
                  Gửi duyệt
                </Button>
              ) : null}

              {detail.status === ContentStatus.Review ? (
                <Button
                  type="button"
                  variant="outline"
                  className="w-full justify-start gap-2"
                  onClick={() => void workflow("approve")}
                  disabled={Boolean(workflowLoading)}
                >
                  <CheckCircle2 size={15} />
                  Duyệt bài giảng
                </Button>
              ) : null}

              {detail.status === ContentStatus.Approved ? (
                <PermissionGuard permission={PERMISSIONS.LESSONS.PUBLISH} fallback={null}>
                  <Button
                    type="button"
                    className="w-full justify-start gap-2"
                    onClick={() => void workflow("publish")}
                    disabled={Boolean(workflowLoading)}
                  >
                    <Rocket size={15} />
                    Xuất bản
                  </Button>
                </PermissionGuard>
              ) : null}

              {detail.status === ContentStatus.Published ? (
                <Button
                  type="button"
                  variant="outline"
                  className="w-full justify-start gap-2"
                  onClick={() => void workflow("archive")}
                  disabled={Boolean(workflowLoading)}
                >
                  <Archive size={15} />
                  Lưu trữ
                </Button>
              ) : null}

              {detail.status === ContentStatus.Archived ? (
                <Button
                  type="button"
                  variant="outline"
                  className="w-full justify-start gap-2"
                  onClick={() => void workflow("restore")}
                  disabled={Boolean(workflowLoading)}
                >
                  <RotateCcw size={15} />
                  Khôi phục bài giảng
                </Button>
              ) : null}
            </div>

            {validationMessages.length > 0 ? (
              <ul className="list-disc space-y-1 pl-4 text-[11px] text-[#8a6413]">
                {validationMessages.map((message, index) => (
                  <li key={`${message}-${index}`}>{message}</li>
                ))}
              </ul>
            ) : null}
          </FormSection>
        ) : null}
      </aside>
    </div>
  );
}

function Metric({ label, value }: { label: string; value: number }) {
  return (
    <div className="rounded-[8px] bg-[#f8f6f2] px-3 py-2.5">
      <div className="text-[13px] font-semibold text-[#333]">{value}</div>
      <div className="mt-0.5 text-[10px] text-[#888]">{label}</div>
    </div>
  );
}
