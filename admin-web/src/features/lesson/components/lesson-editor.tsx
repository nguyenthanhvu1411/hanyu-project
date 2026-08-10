"use client";

import { FormEvent, useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import {
  Archive,
  BookOpenText,
  CheckCircle2,
  ImageIcon,
  Loader2,
  Rocket,
  Send,
  Settings2,
  ShieldCheck,
} from "lucide-react";
import { toast } from "sonner";

import { FormActions } from "@/components/forms/form-actions";
import { FormField } from "@/components/forms/form-field";
import { FormRow } from "@/components/forms/form-row";
import { FormSection } from "@/components/forms/form-section";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Switch } from "@/components/ui/switch";
import { Textarea } from "@/components/ui/textarea";
import { ContentStatus, getContentStatusLabel } from "@/lib/constants/content-status";
import { PermissionGuard } from "@/security/permission-guard";
import { PERMISSIONS } from "@/constants/permission.constants";

import { baiGiangApi } from "../api/bai-giang.api";
import type {
  AdminLessonDetail,
  CreateLessonRequest,
} from "../types/bai-giang.types";

interface LessonEditorProps {
  lessonId?: number;
}

const EMPTY_FORM: CreateLessonRequest = {
  courseChapterId: null,
  hskLevelId: 1,
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

function toRequest(form: CreateLessonRequest): CreateLessonRequest {
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
  const [loading, setLoading] = useState(editing);
  const [saving, setSaving] = useState(false);
  const [workflowLoading, setWorkflowLoading] = useState<string | null>(null);
  const [validationMessages, setValidationMessages] = useState<string[]>([]);

  useEffect(() => {
    if (!editing || !lessonId) return;

    let active = true;

    void baiGiangApi
      .chiTiet(lessonId)
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
      .catch((error) => {
        toast.error(error instanceof Error ? error.message : "Không thể tải bài giảng.");
      })
      .finally(() => {
        if (active) setLoading(false);
      });

    return () => {
      active = false;
    };
  }, [editing, lessonId]);

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
        const updated = await baiGiangApi.capNhat(lessonId, {
          ...toRequest(form),
          version: detail.version,
        });
        setDetail(updated);
        toast.success("Đã cập nhật bài giảng.");
      } else {
        const created = await baiGiangApi.tao(toRequest(form));
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
      const result = await baiGiangApi.kiemTra(lessonId);
      const messages = [...(result.errors ?? []), ...(result.warnings ?? [])];
      setValidationMessages(messages);
      if (result.isValid) toast.success("Bài giảng hợp lệ để tiếp tục quy trình.");
      else toast.error("Bài giảng còn lỗi cần xử lý.");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể kiểm tra bài giảng.");
    } finally {
      setWorkflowLoading(null);
    }
  }

  async function workflow(action: "review" | "approve" | "publish" | "archive") {
    if (!lessonId || !detail || workflowLoading) return;
    setWorkflowLoading(action);

    try {
      const request = { version: detail.version };
      const updated =
        action === "review"
          ? await baiGiangApi.guiDuyet(lessonId, request)
          : action === "approve"
            ? await baiGiangApi.duyet(lessonId, request)
            : action === "publish"
              ? await baiGiangApi.xuatBan(lessonId, request)
              : await baiGiangApi.luuTru(lessonId, request);

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
          <FormRow columns={1}>
            <FormField label="Tên bài giảng" required>
              <Input
                value={form.titleVi}
                onChange={(event) => setField("titleVi", event.target.value)}
                placeholder="Ví dụ: Chào hỏi cơ bản"
              />
            </FormField>
          </FormRow>

          <FormRow columns={2}>
            <FormField label="Slug" required>
              <Input
                value={form.slug}
                onChange={(event) => setField("slug", event.target.value)}
                placeholder="chao-hoi-co-ban"
              />
            </FormField>

            <FormField
              label="HSK Level ID"
              required
              description="Backend sử dụng long HskLevelId."
            >
              <Input
                type="number"
                min={1}
                step={1}
                value={form.hskLevelId}
                onChange={(event) => setField("hskLevelId", Number(event.target.value))}
              />
            </FormField>
          </FormRow>

          <FormRow columns={2}>
            <FormField
              label="Course Chapter ID"
              description="long CourseChapterId, để trống nếu bài giảng chưa thuộc chương."
            >
              <Input
                type="number"
                min={1}
                step={1}
                value={form.courseChapterId ?? ""}
                onChange={(event) =>
                  setField(
                    "courseChapterId",
                    event.target.value ? Number(event.target.value) : null,
                  )
                }
                placeholder="Không bắt buộc"
              />
            </FormField>

            <FormField
              label="Topic ID"
              description="long TopicId, để trống nếu chưa phân loại topic."
            >
              <Input
                type="number"
                min={1}
                step={1}
                value={form.topicId ?? ""}
                onChange={(event) =>
                  setField("topicId", event.target.value ? Number(event.target.value) : null)
                }
                placeholder="Không bắt buộc"
              />
            </FormField>
          </FormRow>

          <FormField label="Mô tả ngắn">
            <Textarea
              value={form.shortDescriptionVi ?? ""}
              onChange={(event) => setField("shortDescriptionVi", event.target.value)}
              rows={3}
            />
          </FormField>

          <FormField label="Mục tiêu bài học">
            <Textarea
              value={form.objectiveVi ?? ""}
              onChange={(event) => setField("objectiveVi", event.target.value)}
              rows={4}
            />
          </FormField>

          <FormField label="Mô tả chi tiết">
            <Textarea
              value={form.descriptionVi ?? ""}
              onChange={(event) => setField("descriptionVi", event.target.value)}
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
                step={1}
                value={form.sortOrder}
                onChange={(event) => setField("sortOrder", Number(event.target.value))}
              />
            </FormField>

            <FormField label="Thời lượng (phút)" required>
              <Input
                type="number"
                min={1}
                step={1}
                value={form.estimatedMinutes}
                onChange={(event) => setField("estimatedMinutes", Number(event.target.value))}
              />
            </FormField>

            <FormField label="Độ khó" required description="Giá trị từ 1 đến 5.">
              <Input
                type="number"
                min={1}
                max={5}
                step={1}
                value={form.difficulty}
                onChange={(event) => setField("difficulty", Number(event.target.value))}
              />
            </FormField>
          </FormRow>

          <FormField label="Nổi bật">
            <Switch
              checked={form.isFeatured}
              onCheckedChange={(value) => setField("isFeatured", value)}
              label={form.isFeatured ? "Bài giảng nổi bật" : "Bài giảng thường"}
              description="Đánh dấu để ưu tiên bài giảng trong các khu vực nổi bật."
            />
          </FormField>
        </FormSection>

        <FormSection
          title="Ảnh bìa"
          description="URL ảnh được lưu trong CoverImageUrl của Lesson."
          icon={<ImageIcon size={18} />}
        >
          <FormField label="URL ảnh bìa">
            <Input
              type="url"
              value={form.coverImageUrl ?? ""}
              onChange={(event) => setField("coverImageUrl", event.target.value)}
              placeholder="https://..."
            />
          </FormField>
        </FormSection>

        <PermissionGuard
          permission={editing ? PERMISSIONS.LESSONS.UPDATE : PERMISSIONS.LESSONS.CREATE}
          fallback={null}
        >
          <FormActions
            loading={saving}
            disabled={!canSubmit}
            submitText={editing ? "Lưu thay đổi" : "Tạo bài giảng"}
            onCancel={() => router.push(editing && lessonId ? `/bai-giang/${lessonId}` : "/bai-giang")}
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

            <div className="grid grid-cols-2 gap-2 text-[12px]">
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
                variant="outline"
                className="w-full justify-start gap-2 text-[12px]"
                onClick={() => void validateLesson()}
                disabled={Boolean(workflowLoading)}
              >
                <ShieldCheck size={15} /> Kiểm tra hợp lệ
              </Button>

              {detail.status === ContentStatus.Draft ? (
                <Button
                  variant="outline"
                  className="w-full justify-start gap-2 text-[12px]"
                  onClick={() => void workflow("review")}
                  disabled={Boolean(workflowLoading)}
                >
                  <Send size={15} /> Gửi duyệt
                </Button>
              ) : null}

              {detail.status === ContentStatus.Review ? (
                <Button
                  variant="outline"
                  className="w-full justify-start gap-2 text-[12px]"
                  onClick={() => void workflow("approve")}
                  disabled={Boolean(workflowLoading)}
                >
                  <CheckCircle2 size={15} /> Duyệt bài giảng
                </Button>
              ) : null}

              {detail.status === ContentStatus.Approved ? (
                <PermissionGuard permission={PERMISSIONS.LESSONS.PUBLISH} fallback={null}>
                  <Button
                    className="w-full justify-start gap-2 text-[12px]"
                    onClick={() => void workflow("publish")}
                    disabled={Boolean(workflowLoading)}
                  >
                    <Rocket size={15} /> Xuất bản
                  </Button>
                </PermissionGuard>
              ) : null}

              {detail.status === ContentStatus.Published ? (
                <Button
                  variant="outline"
                  className="w-full justify-start gap-2 text-[12px]"
                  onClick={() => void workflow("archive")}
                  disabled={Boolean(workflowLoading)}
                >
                  <Archive size={15} /> Lưu trữ
                </Button>
              ) : null}
            </div>

            {validationMessages.length > 0 ? (
              <div className="rounded-[7px] bg-[#fff7e4] p-3 text-[11px] text-[#9b6811]">
                <ul className="list-disc space-y-1 pl-4">
                  {validationMessages.map((message, index) => (
                    <li key={`${message}-${index}`}>{message}</li>
                  ))}
                </ul>
              </div>
            ) : null}
          </FormSection>
        ) : null}
      </aside>
    </div>
  );
}

function Metric({ label, value }: { label: string; value: number }) {
  return (
    <div className="rounded-[7px] bg-[#faf9f7] px-3 py-2.5">
      <div className="font-semibold text-[#292929]">{value}</div>
      <div className="mt-0.5 text-[10px] text-[#888]">{label}</div>
    </div>
  );
}
