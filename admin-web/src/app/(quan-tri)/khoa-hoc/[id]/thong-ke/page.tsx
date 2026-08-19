"use client";

import { useParams } from "next/navigation";
import { BarChart3, BookOpen, CheckCircle2, Clock3, Layers3, Users } from "lucide-react";
import { useQuery } from "@tanstack/react-query";

import { ErrorState } from "@/components/common/error-state";
import { FormSection } from "@/components/forms/form-section";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { courseApi } from "@/features/course/api/course.api";

export default function CourseStatisticsPage() {
  const params = useParams<{ id: string }>();
  const courseId = Number(params.id);
  const query = useQuery({
    queryKey: ["course-statistics", courseId],
    queryFn: () => courseApi.statistics(courseId),
    enabled: Number.isSafeInteger(courseId) && courseId > 0,
  });

  if (!Number.isSafeInteger(courseId) || courseId <= 0) {
    return <ErrorState title="Khóa học không hợp lệ" description="CourseId phải là số nguyên dương." />;
  }

  if (query.isLoading) {
    return (
      <div className="grid gap-3 md:grid-cols-2 xl:grid-cols-4">
        {Array.from({ length: 4 }).map((_, index) => (
          <Skeleton key={index} className="h-28 rounded-[11px]" />
        ))}
      </div>
    );
  }

  if (!query.data) {
    return (
      <ErrorState
        title="Không thể tải thống kê khóa học"
        description={query.error instanceof Error ? query.error.message : "Không có dữ liệu."}
        onRetry={() => void query.refetch()}
      />
    );
  }

  const statistics = query.data;

  return (
    <div className="space-y-4">
      <div className="grid gap-3 md:grid-cols-2 xl:grid-cols-4">
        <MetricCard
          icon={<Layers3 size={18} />}
          label="Tổng chương"
          value={statistics.totalChapters}
          detail={`${statistics.activeChapters} chương đang hoạt động`}
        />
        <MetricCard
          icon={<BookOpen size={18} />}
          label="Tổng bài giảng"
          value={statistics.totalLessons}
          detail="Lesson thật đang thuộc Course"
        />
        <MetricCard
          icon={<Users size={18} />}
          label="Học viên"
          value={statistics.totalStudents}
          detail={`${statistics.studentsInProgress} đang học`}
        />
        <MetricCard
          icon={<CheckCircle2 size={18} />}
          label="Hoàn thành"
          value={statistics.studentsCompleted}
          detail="Đã hoàn thành toàn bộ Lesson"
        />
      </div>

      <FormSection
        title="Tiến độ học tập"
        description="Tính trực tiếp từ UserLessonProgress của các Lesson thuộc khóa học."
        icon={<BarChart3 size={18} />}
      >
        <div className="grid gap-4 md:grid-cols-3">
          <Info
            label="Tiến độ trung bình"
            value={<span className="text-[18px] font-semibold">{formatPercent(statistics.averageCompletionPercent)}</span>}
          />
          <Info
            label="Học viên đang học"
            value={<Badge variant={statistics.studentsInProgress > 0 ? "info" : "default"}>{statistics.studentsInProgress}</Badge>}
          />
          <Info
            label="Học viên hoàn thành"
            value={<Badge variant={statistics.studentsCompleted > 0 ? "success" : "default"}>{statistics.studentsCompleted}</Badge>}
          />
        </div>
      </FormSection>

      <FormSection
        title="Quy mô nội dung"
        description="Các chỉ số backend tính từ CourseChapter và Lesson, không suy diễn từ dữ liệu giao diện."
        icon={<Clock3 size={18} />}
      >
        <div className="grid gap-4 md:grid-cols-3">
          <Info label="Chương hoạt động" value={`${statistics.activeChapters}/${statistics.totalChapters}`} />
          <Info label="Tổng bài giảng" value={String(statistics.totalLessons)} />
          <Info
            label="Thời lượng dự kiến"
            value={statistics.estimatedMinutes ? `${statistics.estimatedMinutes} phút` : "Chưa thiết lập"}
          />
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
  return (
    <div>
      <div className="text-[10px] text-[#888]">{label}</div>
      <div className="mt-1 text-[12px] font-medium text-[#333]">{value}</div>
    </div>
  );
}

function formatPercent(value: number) {
  return new Intl.NumberFormat("vi-VN", { maximumFractionDigits: 2 }).format(value) + "%";
}
