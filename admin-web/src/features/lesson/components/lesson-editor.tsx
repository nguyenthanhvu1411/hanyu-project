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
import { courseApi } from "@/features/course/api/course.api";
import type { AdminCourseChapter, AdminCourseListItem } from "@/features/course/types/course.types";
import { learningApi } from "@/features/learning/learning.api";
import { ContentStatus, getContentStatusLabel } from "@/lib/constants/content-status";
import { PermissionGuard } from "@/security/permission-guard";

import { lessonApi } from "../api/lesson.api";
import type {
  AdminLessonDetail,
  AdminLessonTopicOption,
  CreateLessonRequest,
} from "../types/lesson.types";

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

function slugifyVietnamese(value: string) {
  return value
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replace(/đ/g, "d")
    .replace(/Đ/g, "D")
    .toLowerCase()
    .trim()
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "")
    .replace(/-{2,}/g, "-");
}

function normalizeRequest(form: CreateLessonRequest): CreateLessonRequest {
  const titleVi = form.titleVi.trim();
  const slug = form.slug.trim() || slugifyVietnamese(titleVi);

  return {
    ...form,
    courseChapterId: form.courseChapterId || null,
    topicId: form.topicId || null,
    slug,
    titleVi,
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
  const [selectedCourseId, setSelectedCourseId] = useState<number | null>(null);

  const [hskLevels, setHskLevels] = useState<AdminHskLevelDto[]>([]);
  const [courses, setCourses] = useState<AdminCourseListItem[]>([]);
  const [chapters, setChapters] = useState<AdminCourseChapter[]>([]);
  const [topics, setTopics] = useState<AdminLessonTopicOption[]>([]);

  const [loading, setLoading] = useState(editing);
  const [catalogLoading, setCatalogLoading] = useState(true);
  const [chapterLoading, setChapterLoading] = useState(false);
  const [catalogError, setCatalogError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);
  const [workflowLoading, setWorkflowLoading] = useState<string | null>(null);
  const [validationMessages, setValidationMessages] = useState<string[]>([]);

  useEffect(() => {
    let active = true;
    setCatalogLoading(true);
    setCatalogError(null);

    void Promise.all([
      learningApi.hskLevels.list(),
      courseApi.list({ isActive: true, page: 1, pageSize: 100, sortBy: "sortOrder" }),
      lessonApi.listTopics(),
    ])
      .then(([hskItems, courseResult, topicItems]) => {
        if (!active) return;
        setHskLevels([...hskItems].sort((a, b) => a.sortOrder - b.sortOrder || a.id - b.id));
        setCourses([...courseResult.items].sort((a, b) => a.sortOrder - b.sortOrder || a.id - b.id));
        setTopics([...topicItems].sort((a, b) => a.sortOrder - b.sortOrder || a.id - b.id));
      })
      .catch((error) => {
        if (!active) return;
        setCatalogError(error instanceof Error ? error.message : "Không thể tải dữ liệu danh mục.");
      })
      .finally(() => {
        if (active) setCatalogLoading(false);
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
        setSelectedCourseId(lesson.courseId ?? null);
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
      .catch((error) => toast.error(error instanceof Error ? error.message : "Không thể tải bài giảng."))
      .finally(() => {
        if (active) setLoading(false);
      });

    return () => {
      active = false;
    };
  }, [editing, lessonId]);

  useEffect(() => {
    let active = true;

    if (!selectedCourseId) {
      setChapters([]);
      return () => {
        active = false;
      };
    }

    setChapterLoading(true);
    void courseApi
      .getById(selectedCourseId)
      .then((course) => {
        if (!active) return;
        setChapters(
          [...course.chapters]
            .filter((chapter) => !chapter.deletedAt)
            .sort((a, b) => a.sortOrder - b.sortOrder || a.id - b.id),
        );
      })
      .catch((error) => {
        if (active) {
          setChapters([]);
          toast.error(error instanceof Error ? error.message : "Không thể tải chương học.");
        }
      })
      .finally(() => {
        if (active) setChapterLoading(false);
      });

    return () => {
      active = false;
    };
  }, [selectedCourseId]);

  const selectedCourse = useMemo(
    () => courses.find((course) => course.id === selectedCourseId) ?? null,
    [courses, selectedCourseId],
  );

  const hskLockedByCourse = Boolean(selectedCourse?.hskLevelId);

  useEffect(() => {
    const courseHskLevelId = selectedCourse?.hskLevelId;
    if (!courseHskLevelId) return;

    setForm((current) =>
      current.hskLevelId === courseHskLevelId
        ? current
        : { ...current, hskLevelId: courseHskLevelId },
    );
  }, [selectedCourse?.hskLevelId]);

  const hskOptions = useMemo(
    () =>
      hskLevels.map((item) => ({
        value: String(item.id),
        label: `${item.code} — ${item.nameVi}`,
        description: item.isActive ? `Thứ tự hiển thị: ${item.sortOrder}` : "Đang tạm ngưng",
        disabled: !item.isActive && item.id !== form.hskLevelId,
      })),
    [form.hskLevelId, hskLevels],
  );

  const courseOptions = useMemo(
    () =>
      courses.map((course) => ({
        value: String(course.id),
        label: `${course.code} — ${course.titleVi}`,
        description: course.hskCode ? `${course.hskCode} · ${course.chapterCount} chương` : `${course.chapterCount} chương`,
      })),
    [courses],
  );

  const chapterOptions = useMemo(
    () =>
      chapters.map((chapter) => ({
        value: String(chapter.id),
        label: chapter.titleVi,
        description: `Thứ tự ${chapter.sortOrder} · ${chapter.lessonCount} bài giảng${chapter.isActive ? "" : " · Tạm ngưng"}`,
        disabled: !chapter.isActive && chapter.id !== form.courseChapterId,
      })),
    [chapters, form.courseChapterId],
  );

  const topicOptions = useMemo(
    () =>
      topics
        .filter((topic) => topic.status === ContentStatus.Published || topic.id === form.topicId)
        .map((topic) => ({
          value: String(topic.id),
          label: topic.nameVi,
          description: topic.slug,
          disabled: topic.status !== ContentStatus.Published,
        })),
    [form.topicId, topics],
  );

  const canSubmit = useMemo(
    () =>
      form.titleVi.trim().length > 0 &&
      slugifyVietnamese(form.titleVi).length > 0 &&
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

  function setField<K extends keyof CreateLessonRequest>(key: K, value: CreateLessonRequest[K]) {
    setForm((current) => ({ ...current, [key]: value }));
  }

  function changeCourse(value: string) {
    const courseId = value ? Number(value) : null;
    setSelectedCourseId(courseId);
    setField("courseChapterId", null);

    const selected = courses.find((course) => course.id === courseId);
    if (selected?.hskLevelId) {
      setField("hskLevelId", selected.hskLevelId);
    }
  }

  function changeTitle(value: string) {
    setForm((current) => ({
      ...current,
      titleVi: value,
      slug: current.slug.trim() ? current.slug : "",
    }));
  }

  async function save(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!canSubmit || saving) return;

    setSaving(true);
    try {
      const request = normalizeRequest(form);
      if (lessonId && detail) {
        const updated = await lessonApi.update(lessonId, { ...request, version: detail.version });
        setDetail(updated);
        setSelectedCourseId(updated.courseId ?? selectedCourseId);
        setForm({
          courseChapterId: updated.courseChapterId ?? null,
          hskLevelId: updated.hskLevelId,
          topicId: updated.topicId ?? null,
          slug: updated.slug,
          titleVi: updated.titleVi,
          shortDescriptionVi: updated.shortDescriptionVi ?? "",
          descriptionVi: updated.descriptionVi ?? "",
          objectiveVi: updated.objectiveVi ?? "",
          coverImageUrl: updated.coverImageUrl ?? "",
          sortOrder: updated.sortOrder,
          estimatedMinutes: updated.estimatedMinutes,
          difficulty: updated.difficulty,
          isFeatured: updated.isFeatured,
        });
        toast.success("Đã cập nhật bài giảng.");
      } else {
        const created = await lessonApi.create(request);
        toast.success("Đã tạo bài giảng.");
        router.replace(`/bai-giang/${created.id}`);
      }
      router.refresh();
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
      result.isValid ? toast.success("Bài giảng hợp lệ để tiếp tục quy trình.") : toast.error("Bài giảng còn lỗi cần xử lý.");
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
    <form onSubmit={save} className="space-y-5">
      {catalogError ? (
        <div className="rounded-[10px] border border-[#f3c9c5] bg-[#fff5f4] px-4 py-3 text-[12px] text-[#b42318]">
          Không thể tải đầy đủ danh mục: {catalogError}
        </div>
      ) : null}

      <FormSection
        title="Thông tin bài giảng"
        description="Thiết lập tên, slug và nội dung mô tả. Nếu để trống slug, hệ thống tự tạo từ tên bài giảng."
        icon={<BookOpenText size={18} />}
      >
        <FormField label="Tên bài giảng" required>
          <Input value={form.titleVi} onChange={(event) => changeTitle(event.target.value)} placeholder="Ví dụ: Chào hỏi và giới thiệu bản thân" />
        </FormField>

        <FormField
          label="Slug"
          description={form.slug.trim() ? "Bạn đang dùng slug tùy chỉnh." : `Tự động khi lưu: ${slugifyVietnamese(form.titleVi) || "nhap-ten-bai-giang"}`}
        >
          <Input
            value={form.slug}
            onChange={(event) => setField("slug", slugifyVietnamese(event.target.value))}
            placeholder="Để trống để tự tạo từ tên bài giảng"
          />
        </FormField>

        <FormRow columns={2}>
          <FormField label="Khóa học" description="Chọn khóa học trước để tải đúng danh sách chương.">
            <Select
              value={selectedCourseId ? String(selectedCourseId) : ""}
              onValueChange={changeCourse}
              options={courseOptions}
              placeholder={catalogLoading ? "Đang tải khóa học..." : "Chọn khóa học"}
              disabled={catalogLoading}
              clearable
            />
          </FormField>

          <FormField label="Chương học" description={selectedCourseId ? "Danh sách chương thuộc khóa học đã chọn." : "Hãy chọn khóa học trước."}>
            <Select
              value={form.courseChapterId ? String(form.courseChapterId) : ""}
              onValueChange={(value) => setField("courseChapterId", value ? Number(value) : null)}
              options={chapterOptions}
              placeholder={chapterLoading ? "Đang tải chương học..." : selectedCourseId ? "Chọn chương học" : "Chọn khóa học trước"}
              disabled={!selectedCourseId || chapterLoading}
              clearable
            />
          </FormField>
        </FormRow>

        <FormRow columns={2}>
          <FormField
            label="Cấp độ HSK"
            required
            description={
              hskLockedByCourse
                ? `Được khóa theo khóa học${selectedCourse?.hskCode ? ` ${selectedCourse.hskCode}` : ""}; muốn đổi HSK hãy đổi cấu hình khóa học trước.`
                : "Lấy từ danh mục HSK. Nếu khóa học có HSK, hệ thống sẽ tự đồng bộ và khóa trường này."
            }
          >
            <Select
              value={form.hskLevelId > 0 ? String(form.hskLevelId) : ""}
              onValueChange={(value) => setField("hskLevelId", Number(value))}
              options={hskOptions}
              placeholder={catalogLoading ? "Đang tải HSK..." : "Chọn cấp độ HSK"}
              disabled={catalogLoading || hskLockedByCourse}
            />
          </FormField>

          <FormField label="Chủ đề" description="Chỉ chủ đề Published mới được chọn cho bài giảng; chủ đề là danh mục nội dung dùng chung.">
            <Select
              value={form.topicId ? String(form.topicId) : ""}
              onValueChange={(value) => setField("topicId", value ? Number(value) : null)}
              options={topicOptions}
              placeholder={catalogLoading ? "Đang tải chủ đề..." : "Chọn chủ đề"}
              disabled={catalogLoading}
              clearable
            />
          </FormField>
        </FormRow>

        <FormField label="Mô tả ngắn">
          <Textarea value={form.shortDescriptionVi ?? ""} onChange={(event) => setField("shortDescriptionVi", event.target.value)} rows={3} />
        </FormField>

        <FormField label="Mục tiêu bài học">
          <Textarea value={form.objectiveVi ?? ""} onChange={(event) => setField("objectiveVi", event.target.value)} rows={4} />
        </FormField>

        <FormField label="Mô tả chi tiết">
          <Textarea value={form.descriptionVi ?? ""} onChange={(event) => setField("descriptionVi", event.target.value)} rows={6} />
        </FormField>
      </FormSection>

      <FormSection
        title="Thiết lập học tập"
        description="Thứ tự, thời lượng, độ khó và trạng thái nổi bật."
        icon={<Settings2 size={18} />}
      >
        <FormRow columns={3}>
          <FormField label="Thứ tự" required>
            <Input type="number" min={0} value={form.sortOrder} onChange={(event) => setField("sortOrder", Number(event.target.value))} />
          </FormField>
          <FormField label="Thời lượng (phút)" required>
            <Input type="number" min={1} value={form.estimatedMinutes} onChange={(event) => setField("estimatedMinutes", Number(event.target.value))} />
          </FormField>
          <FormField label="Độ khó" required>
            <Input type="number" min={1} max={5} value={form.difficulty} onChange={(event) => setField("difficulty", Number(event.target.value))} />
          </FormField>
        </FormRow>

        <FormField label="Nổi bật">
          <Switch checked={form.isFeatured} onCheckedChange={(value) => setField("isFeatured", value)} label={form.isFeatured ? "Bài giảng nổi bật" : "Bài giảng thường"} />
        </FormField>
      </FormSection>

      <FormSection
        title="Ảnh bìa"
        description="Tải file lên Storage hoặc dùng URL ngoài. DB chỉ lưu tham chiếu ổn định; URL đọc được tạo lại khi hiển thị."
        icon={<ImageIcon size={18} />}
      >
        <CoverImageField value={form.coverImageUrl} onChange={(value) => setField("coverImageUrl", value)} disabled={saving} />
      </FormSection>

      {detail ? (
        <FormSection title="Trạng thái và quy trình" description={`Revision kỹ thuật v${detail.version} · PublicId ${detail.publicId}`} icon={<ShieldCheck size={18} />}>
          <div className="grid gap-3 md:grid-cols-5">
            <Metric label="Trạng thái" value={getContentStatusLabel(detail.status)} />
            <Metric label="Nội dung" value={detail.sectionCount} />
            <Metric label="Từ vựng" value={detail.vocabularyCount} />
            <Metric label="Tài liệu" value={detail.assetCount} />
            <Metric label="Tiên quyết" value={detail.prerequisiteCount} />
          </div>

          <div className="mt-4 flex flex-wrap gap-2">
            <Button type="button" variant="outline" onClick={() => void validateLesson()} disabled={Boolean(workflowLoading)}>
              <ShieldCheck size={15} /> Kiểm tra hợp lệ
            </Button>
            {detail.status === ContentStatus.Draft ? (
              <Button type="button" variant="outline" onClick={() => void workflow("review")} disabled={Boolean(workflowLoading)}>
                <Send size={15} /> Gửi duyệt
              </Button>
            ) : null}
            {detail.status === ContentStatus.Review ? (
              <Button type="button" variant="outline" onClick={() => void workflow("approve")} disabled={Boolean(workflowLoading)}>
                <CheckCircle2 size={15} /> Duyệt bài giảng
              </Button>
            ) : null}
            {detail.status === ContentStatus.Approved ? (
              <PermissionGuard permission={PERMISSIONS.LESSONS.PUBLISH} fallback={null}>
                <Button type="button" onClick={() => void workflow("publish")} disabled={Boolean(workflowLoading)}>
                  <Rocket size={15} /> Xuất bản
                </Button>
              </PermissionGuard>
            ) : null}
            {detail.status === ContentStatus.Published ? (
              <Button type="button" variant="outline" onClick={() => void workflow("archive")} disabled={Boolean(workflowLoading)}>
                <Archive size={15} /> Lưu trữ
              </Button>
            ) : null}
            {detail.status === ContentStatus.Archived ? (
              <Button type="button" variant="outline" onClick={() => void workflow("restore")} disabled={Boolean(workflowLoading)}>
                <RotateCcw size={15} /> Khôi phục
              </Button>
            ) : null}
          </div>

          {validationMessages.length > 0 ? (
            <ul className="mt-4 list-disc space-y-1 pl-5 text-[11px] text-[#8a6413]">
              {validationMessages.map((message, index) => <li key={`${message}-${index}`}>{message}</li>)}
            </ul>
          ) : null}
        </FormSection>
      ) : null}

      <PermissionGuard permission={editing ? PERMISSIONS.LESSONS.UPDATE : PERMISSIONS.LESSONS.CREATE} fallback={null}>
        <FormActions
          loading={saving}
          disabled={!canSubmit}
          submitText={editing ? "Lưu thay đổi" : "Tạo bài giảng"}
          onCancel={() => router.push(editing && lessonId ? `/bai-giang/${lessonId}` : "/bai-giang")}
        />
      </PermissionGuard>
    </form>
  );
}

function Metric({ label, value }: { label: string; value: React.ReactNode }) {
  return (
    <div className="rounded-[8px] bg-[#f8f6f2] px-3 py-2.5">
      <div className="text-[13px] font-semibold text-[#333]">{value}</div>
      <div className="mt-0.5 text-[10px] text-[#888]">{label}</div>
    </div>
  );
}