import Link from "next/link";
import type { CourseEditorController } from "../../hooks/use-course-editor";
import { getContentStatusLabel } from "@/lib/constants/content-status";

export function CourseOverviewTab({
  editor,
}: {
  editor: CourseEditorController;
}) {
  const course = editor.course!;

  return (
    <div className="grid gap-4 xl:grid-cols-3">
      <section className="rounded-xl border bg-white p-5 xl:col-span-2">
        <div className="flex items-center justify-between">
          <h2 className="font-semibold">Thông tin khóa học</h2>

          {editor.canEdit && (
            <Link
              href={`/khoa-hoc/${course.id}/chinh-sua`}
              className="text-sm font-medium text-red-600 hover:underline"
            >
              Chỉnh sửa
            </Link>
          )}
        </div>

        <dl className="mt-5 grid gap-5 md:grid-cols-2">
          <Info label="Mã" value={course.code} />
          <Info label="Slug" value={course.slug} />
          <Info
            label="HSK"
            value={course.hskNameVi ?? course.hskCode ?? "—"}
          />
          <Info label="Trạng thái" value={getContentStatusLabel(course.status)} />
          <Info
            label="Thời lượng"
            value={
              course.estimatedMinutes ? `${course.estimatedMinutes} phút` : "—"
            }
          />
          <Info label="Nổi bật" value={course.isFeatured ? "Có" : "Không"} />
        </dl>

        <div className="mt-6">
          <h3 className="text-sm font-medium">Mô tả ngắn</h3>
          <p className="mt-2 whitespace-pre-wrap text-sm leading-6 text-neutral-600">
            {course.shortDescriptionVi ?? "Chưa có mô tả."}
          </p>
        </div>

        <div className="mt-6">
          <h3 className="text-sm font-medium">Mô tả chi tiết</h3>
          <p className="mt-2 whitespace-pre-wrap text-sm leading-6 text-neutral-600">
            {course.descriptionVi ?? "Chưa có nội dung."}
          </p>
        </div>
      </section>

      <aside className="rounded-xl border bg-white p-5">
        <h2 className="font-semibold">Thông tin hệ thống</h2>

        <dl className="mt-5 space-y-4">
          <Info label="Public ID" value={course.publicId} />
          <Info label="Ngày tạo" value={formatDate(course.createdAt)} />
          <Info label="Cập nhật" value={formatDate(course.updatedAt)} />
          <Info
            label="Xuất bản"
            value={course.publishedAt ? formatDate(course.publishedAt) : "—"}
          />
        </dl>
      </aside>
    </div>
  );
}

function Info({ label, value }: { label: string; value: React.ReactNode }) {
  return (
    <div>
      <dt className="text-xs text-neutral-500">{label}</dt>
      <dd className="mt-1 break-words text-sm font-medium">{value}</dd>
    </div>
  );
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat("vi-VN", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value));
}
