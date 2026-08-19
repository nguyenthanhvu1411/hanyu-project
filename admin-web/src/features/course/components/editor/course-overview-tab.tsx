import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { StorageImage } from "@/components/media/storage-image";
import type { CourseEditorController } from "../../hooks/use-course-editor";
import { getContentStatusLabel } from "@/lib/constants/content-status";

export function CourseOverviewTab({ editor }: { editor: CourseEditorController }) {
  const course = editor.course!;

  return (
    <div className="grid gap-4 xl:grid-cols-3">
      <Card className="xl:col-span-2">
        <CardHeader className="flex flex-row items-center justify-between gap-3">
          <CardTitle>Thông tin khóa học</CardTitle>
          {editor.canEdit ? (
            <Link href={`/khoa-hoc/${course.id}/chinh-sua`}>
              <Button variant="outline" size="sm">Chỉnh sửa</Button>
            </Link>
          ) : null}
        </CardHeader>
        <CardContent className="space-y-5">
          <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-[#faf9f7]">
            <StorageImage
              value={course.coverImageUrl}
              alt={`Ảnh bìa ${course.titleVi}`}
              className="max-h-[360px] w-full object-cover"
              emptyClassName="min-h-[220px]"
            />
          </div>

          <dl className="grid gap-4 md:grid-cols-2">
            <Info label="Mã" value={course.code} />
            <Info label="Slug" value={course.slug} />
            <Info label="HSK" value={course.hskNameVi ?? course.hskCode ?? "—"} />
            <Info label="Trạng thái" value={<Badge variant="info">{getContentStatusLabel(course.status)}</Badge>} />
            <Info label="Thời lượng" value={course.estimatedMinutes ? `${course.estimatedMinutes} phút` : "—"} />
            <Info label="Nổi bật" value={<Badge variant={course.isFeatured ? "warning" : "default"}>{course.isFeatured ? "Có" : "Không"}</Badge>} />
          </dl>
          <InfoBlock label="Mô tả ngắn" value={course.shortDescriptionVi ?? "Chưa có mô tả."} />
          <InfoBlock label="Mô tả chi tiết" value={course.descriptionVi ?? "Chưa có nội dung."} />
        </CardContent>
      </Card>

      <Card>
        <CardHeader><CardTitle>Thông tin hệ thống</CardTitle></CardHeader>
        <CardContent>
          <dl className="space-y-4">
            <Info label="ID" value={course.id} />
            <Info label="Public ID" value={course.publicId} />
            <Info label="Ngày tạo" value={formatDate(course.createdAt)} />
            <Info label="Cập nhật" value={formatDate(course.updatedAt)} />
            <Info label="Xuất bản" value={course.publishedAt ? formatDate(course.publishedAt) : "—"} />
          </dl>
        </CardContent>
      </Card>
    </div>
  );
}

function Info({ label, value }: { label: string; value: React.ReactNode }) {
  return <div><dt className="text-[10px] text-[#888]">{label}</dt><dd className="mt-1 break-words text-[12px] font-medium text-[#333]">{value}</dd></div>;
}

function InfoBlock({ label, value }: { label: string; value: string }) {
  return <div><h3 className="text-[11px] font-medium text-[#555]">{label}</h3><p className="mt-2 whitespace-pre-wrap text-[12px] leading-5 text-[#666]">{value}</p></div>;
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat("vi-VN", { dateStyle: "medium", timeStyle: "short" }).format(new Date(value));
}
