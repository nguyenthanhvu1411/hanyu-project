"use client";

import { FormEvent, useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { Archive, CheckCircle2, Loader2, Rocket, Save, Send, ShieldCheck } from "lucide-react";
import { toast } from "sonner";

import { Button } from "@/components/ui/button";
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
  const editing = Boolean(lessonId);

  const [detail, setDetail] = useState<AdminLessonDetail | null>(null);
  const [form, setForm] = useState<CreateLessonRequest>(EMPTY_FORM);
  const [loading, setLoading] = useState(editing);
  const [saving, setSaving] = useState(false);
  const [workflowLoading, setWorkflowLoading] = useState<string | null>(null);
  const [validationMessages, setValidationMessages] = useState<string[]>([]);

  useEffect(() => {
    if (!lessonId) return;

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
  }, [lessonId]);

  const canSubmit = useMemo(
    () => form.titleVi.trim().length > 0 && form.slug.trim().length > 0 && form.hskLevelId > 0,
    [form.hskLevelId, form.slug, form.titleVi],
  );

  function setField<K extends keyof CreateLessonRequest>(key: K, value: CreateLessonRequest[K]) {
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

  async function workflow(
    action: "review" | "approve" | "publish" | "archive",
  ) {
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
      <div className="flex min-h-[320px] items-center justify-center rounded-xl border border-zinc-200 bg-white">
        <Loader2 className="animate-spin text-zinc-500" size={22} />
      </div>
    );
  }

  return (
    <div className="grid gap-5 xl:grid-cols-[minmax(0,1fr)_320px]">
      <form onSubmit={save} className="space-y-5">
        <section className="rounded-xl border border-zinc-200 bg-white p-5 shadow-sm">
          <div className="mb-5">
            <h2 className="text-[15px] font-semibold text-zinc-900">Thông tin bài giảng</h2>
            <p className="mt-1 text-[12px] text-zinc-500">Thông tin cơ bản dùng cho danh sách, tìm kiếm và hiển thị bài học.</p>
          </div>

          <div className="grid gap-4 md:grid-cols-2">
            <Field label="Tên bài giảng" required className="md:col-span-2">
              <input
                value={form.titleVi}
                onChange={(event) => setField("titleVi", event.target.value)}
                className={inputClass}
                placeholder="Ví dụ: Chào hỏi cơ bản"
              />
            </Field>

            <Field label="Slug" required>
              <input
                value={form.slug}
                onChange={(event) => setField("slug", event.target.value)}
                className={inputClass}
                placeholder="chao-hoi-co-ban"
              />
            </Field>

            <Field label="HSK Level ID" required>
              <input
                type="number"
                min={1}
                value={form.hskLevelId}
                onChange={(event) => setField("hskLevelId", Number(event.target.value))}
                className={inputClass}
              />
            </Field>

            <Field label="Course Chapter ID">
              <input
                type="number"
                min={1}
                value={form.courseChapterId ?? ""}
                onChange={(event) => setField("courseChapterId", event.target.value ? Number(event.target.value) : null)}
                className={inputClass}
                placeholder="Không bắt buộc"
              />
            </Field>

            <Field label="Topic ID">
              <input
                type="number"
                min={1}
                value={form.topicId ?? ""}
                onChange={(event) => setField("topicId", event.target.value ? Number(event.target.value) : null)}
                className={inputClass}
                placeholder="Không bắt buộc"
              />
            </Field>

            <Field label="Mô tả ngắn" className="md:col-span-2">
              <textarea
                value={form.shortDescriptionVi ?? ""}
                onChange={(event) => setField("shortDescriptionVi", event.target.value)}
                className={`${inputClass} min-h-[80px] py-2.5`}
              />
            </Field>

            <Field label="Mục tiêu bài học" className="md:col-span-2">
              <textarea
                value={form.objectiveVi ?? ""}
                onChange={(event) => setField("objectiveVi", event.target.value)}
                className={`${inputClass} min-h-[100px] py-2.5`}
              />
            </Field>

            <Field label="Mô tả chi tiết" className="md:col-span-2">
              <textarea
                value={form.descriptionVi ?? ""}
                onChange={(event) => setField("descriptionVi", event.target.value)}
                className={`${inputClass} min-h-[130px] py-2.5`}
              />
            </Field>

            <Field label="Ảnh bìa URL" className="md:col-span-2">
              <input
                value={form.coverImageUrl ?? ""}
                onChange={(event) => setField("coverImageUrl", event.target.value)}
                className={inputClass}
                placeholder="https://..."
              />
            </Field>
          </div>
        </section>

        <section className="rounded-xl border border-zinc-200 bg-white p-5 shadow-sm">
          <h2 className="mb-4 text-[15px] font-semibold text-zinc-900">Thiết lập học tập</h2>
          <div className="grid gap-4 sm:grid-cols-3">
            <Field label="Thứ tự">
              <input
                type="number"
                min={0}
                value={form.sortOrder}
                onChange={(event) => setField("sortOrder", Number(event.target.value))}
                className={inputClass}
              />
            </Field>
            <Field label="Thời lượng (phút)">
              <input
                type="number"
                min={1}
                value={form.estimatedMinutes}
                onChange={(event) => setField("estimatedMinutes", Number(event.target.value))}
                className={inputClass}
              />
            </Field>
            <Field label="Độ khó">
              <input
                type="number"
                min={1}
                max={5}
                value={form.difficulty}
                onChange={(event) => setField("difficulty", Number(event.target.value))}
                className={inputClass}
              />
            </Field>
          </div>

          <label className="mt-4 inline-flex cursor-pointer items-center gap-2 text-[13px] text-zinc-700">
            <input
              type="checkbox"
              checked={form.isFeatured}
              onChange={(event) => setField("isFeatured", event.target.checked)}
              className="h-4 w-4 rounded border-zinc-300"
            />
            Đánh dấu bài giảng nổi bật
          </label>
        </section>

        <div className="flex justify-end gap-2">
          <Button type="button" variant="outline" onClick={() => router.push("/bai-giang")}>
            Hủy
          </Button>
          <PermissionGuard
            permission={editing ? PERMISSIONS.LESSONS.UPDATE : PERMISSIONS.LESSONS.CREATE}
            fallback={null}
          >
            <Button type="submit" disabled={!canSubmit || saving} className="gap-2">
              {saving ? <Loader2 size={15} className="animate-spin" /> : <Save size={15} />}
              {editing ? "Lưu thay đổi" : "Tạo bài giảng"}
            </Button>
          </PermissionGuard>
        </div>
      </form>

      <aside className="space-y-4">
        {detail ? (
          <section className="rounded-xl border border-zinc-200 bg-white p-4 shadow-sm">
            <p className="text-[11px] font-semibold uppercase tracking-wide text-zinc-400">Trạng thái</p>
            <p className="mt-2 text-[15px] font-semibold text-zinc-900">{getContentStatusLabel(detail.status)}</p>
            <p className="mt-1 text-[12px] text-zinc-500">Phiên bản v{detail.version}</p>

            <div className="mt-4 grid grid-cols-2 gap-2 text-[12px]">
              <Metric label="Nội dung" value={detail.sectionCount} />
              <Metric label="Từ vựng" value={detail.vocabularyCount} />
              <Metric label="Tài liệu" value={detail.assetCount} />
              <Metric label="Tiên quyết" value={detail.prerequisiteCount} />
            </div>
          </section>
        ) : null}

        {detail ? (
          <section className="rounded-xl border border-zinc-200 bg-white p-4 shadow-sm">
            <h3 className="text-[13px] font-semibold text-zinc-900">Quy trình xuất bản</h3>
            <div className="mt-3 space-y-2">
              <Button variant="outline" className="w-full justify-start gap-2 text-[12px]" onClick={() => void validateLesson()} disabled={Boolean(workflowLoading)}>
                <ShieldCheck size={15} /> Kiểm tra hợp lệ
              </Button>

              {detail.status === ContentStatus.Draft ? (
                <Button variant="outline" className="w-full justify-start gap-2 text-[12px]" onClick={() => void workflow("review")} disabled={Boolean(workflowLoading)}>
                  <Send size={15} /> Gửi duyệt
                </Button>
              ) : null}

              {detail.status === ContentStatus.Review ? (
                <Button variant="outline" className="w-full justify-start gap-2 text-[12px]" onClick={() => void workflow("approve")} disabled={Boolean(workflowLoading)}>
                  <CheckCircle2 size={15} /> Duyệt bài giảng
                </Button>
              ) : null}

              {detail.status === ContentStatus.Approved ? (
                <PermissionGuard permission={PERMISSIONS.LESSONS.PUBLISH} fallback={null}>
                  <Button className="w-full justify-start gap-2 text-[12px]" onClick={() => void workflow("publish")} disabled={Boolean(workflowLoading)}>
                    <Rocket size={15} /> Xuất bản
                  </Button>
                </PermissionGuard>
              ) : null}

              {detail.status === ContentStatus.Published ? (
                <Button variant="outline" className="w-full justify-start gap-2 text-[12px]" onClick={() => void workflow("archive")} disabled={Boolean(workflowLoading)}>
                  <Archive size={15} /> Lưu trữ
                </Button>
              ) : null}
            </div>

            {validationMessages.length > 0 ? (
              <div className="mt-3 rounded-lg bg-amber-50 p-3 text-[12px] text-amber-800">
                <ul className="list-disc space-y-1 pl-4">
                  {validationMessages.map((message, index) => <li key={`${message}-${index}`}>{message}</li>)}
                </ul>
              </div>
            ) : null}
          </section>
        ) : null}
      </aside>
    </div>
  );
}

const inputClass = "h-[38px] w-full rounded-md border border-zinc-200 bg-white px-3 text-[13px] text-zinc-900 outline-none transition placeholder:text-zinc-400 focus:border-zinc-400 focus:ring-2 focus:ring-zinc-100";

function Field({
  label,
  required,
  className = "",
  children,
}: {
  label: string;
  required?: boolean;
  className?: string;
  children: React.ReactNode;
}) {
  return (
    <label className={`block ${className}`}>
      <span className="mb-1.5 block text-[12px] font-medium text-zinc-700">
        {label}{required ? <span className="ml-1 text-red-500">*</span> : null}
      </span>
      {children}
    </label>
  );
}

function Metric({ label, value }: { label: string; value: number }) {
  return (
    <div className="rounded-lg bg-zinc-50 px-3 py-2.5">
      <div className="font-semibold text-zinc-900">{value}</div>
      <div className="mt-0.5 text-[11px] text-zinc-500">{label}</div>
    </div>
  );
}
