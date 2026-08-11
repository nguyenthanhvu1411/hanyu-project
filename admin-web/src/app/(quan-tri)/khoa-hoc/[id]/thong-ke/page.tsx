"use client";

import { useParams } from "next/navigation";
import { BarChart3, BookOpen, Clock3, Layers3, Link2 } from "lucide-react";
import { useQuery } from "@tanstack/react-query";

import { ErrorState } from "@/components/common/error-state";
import { FormSection } from "@/components/forms/form-section";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { courseApi } from "@/features/course/api/course.api";
import { getContentStatusLabel } from "@/lib/constants/content-status";

export default function CourseStatisticsPage() {
  const params = useParams<{ id: string }>();
  const courseId = Number(params.id);
  const query = useQuery({
    queryKey: ["course", courseId],
    queryFn: () => courseApi.getById(courseId),
    enabled: Number.isSafeInteger(courseId) && courseId > 0,
  });

  if (!Number.isSafeInteger(courseId) || courseId <= 0) {
    return <ErrorState title="Khóa học không hợp lệ" description="CourseId phải là số nguyên dương." />;
  }

  if (query.isLoading) {
    return <div className="grid gap-3 md:grid-cols-2 xl:grid-cols-4"><Skeleton className="h-28 rounded-[11px]" /><Skeleton className="h-28 rounded-[11px]" /><Skeleton className="h-28 rounded-[11px]" /><Skeleton className="h-28 rounded-[11px]" /></div>;
  }

  if (!query.data) {
    return <ErrorState title="Không thể tải thống kê khóa học" description={query.error instanceof Error ? query.error.message : "Không có dữ liệu."} onRetry={() => void query.refetch()} />;
  }

  const course = query.data;
  const chapterCount = course.chapters.filter((chapter) => !chapter.deletedAt).length;
  const activeChapterCount = course.chapters.filter((chapter) => !chapter.deletedAt && chapter.isActive).length;
  const lessonCount = course.chapters.reduce((total, chapter) => total + (chapter.deletedAt ? 0 : chapter.lessonCount), 0);

  return (
    <div className="space-y-4">
      <div className="grid gap-3 md:grid-cols-2 xl:grid-cols-4">
        <MetricCard icon={<Layers3 size={18} />} label="Tổng chương" value={chapterCount} detail={`${activeChapterCount} chương đang hoạt động`} />
        <MetricCard icon={<BookOpen size={18} />} label="Tổng bài giảng" value={lessonCount} detail="Tổng Lesson đang gắn vào Chapter" />
        <MetricCard icon={<Clock3 size={18} />} label="Thời lượng dự kiến" value={course.estimatedMinutes ? `${course.estimatedMinutes} phút` : "—"} detail="Thiết lập trên Course" />
        <MetricCard icon={<Link2 size={18} />} label="Tiên quyết" value={course.prerequisites.length} detail="Khóa học bắt buộc/khuyến nghị" />
      </div>

      <FormSection
        title="Trạng thái nội dung"
        description="Các chỉ số hiện có được tính trực tiếp từ Course detail của backend, không dùng dữ liệu giả."
        icon={<BarChart3 size={18} />}
      >
        <div className="grid gap-4 md:grid-cols-3">
          <Info label="Trạng thái biên tập" value={<Badge variant="info">{getContentStatusLabel(course.status)}</Badge>} />
          <Info label="Hoạt động" value={<Badge variant={course.isActive ? "success" : "default"}>{course.isActive ? "Hoạt động" : "Ngừng hoạt động"}</Badge>} />
          <Info label="Nổi bật" value={<Badge variant={course.isFeatured ? "warning" : "default"}>{course.isFeatured ? "Có" : "Không"}</Badge>} />
        </div>
      </FormSection>
    </div>
  );
}

function MetricCard({ icon, label, value, detail }: { icon: React.ReactNode; label: string; value: React.ReactNode; detail: string }) {
  return (
    <Card>
      <CardContent className="p-4">
        <div className="flex items-start justify-between gap-3">
          <div>
            <div className="text-[10px] text-[#888]">{label}</div>
            <div className="mt-1 text-[20px] font-semibold text-[#292929]">{value}</div>
            <div className="mt-1 text-[10px] text-[#999]">{detail}</div>
          </div>
          <div className="flex h-10 w-10 items-center justify-center rounded-[9px] bg-[#fff0ee] text-[#ef241c]">{icon}</div>
        </div>
      </CardContent>
    </Card>
  );
}

function Info({ label, value }: { label: string; value: React.ReactNode }) {
  return <div><div className="text-[10px] text-[#888]">{label}</div><div className="mt-1 text-[12px] font-medium text-[#333]">{value}</div></div>;
}
