"use client";

import { useParams } from "next/navigation";
import { Clock3 } from "lucide-react";
import { useQuery } from "@tanstack/react-query";

import { EmptyState } from "@/components/common/empty-state";
import { ErrorState } from "@/components/common/error-state";
import { FormSection } from "@/components/forms/form-section";
import { Badge } from "@/components/ui/badge";
import { Skeleton } from "@/components/ui/skeleton";
import { courseApi } from "@/features/course/api/course.api";

export default function CourseHistoryPage() {
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
    return <Skeleton className="h-64 w-full rounded-[11px]" />;
  }

  if (!query.data) {
    return <ErrorState title="Không thể tải lịch sử khóa học" description={query.error instanceof Error ? query.error.message : "Không có dữ liệu."} onRetry={() => void query.refetch()} />;
  }

  const course = query.data;
  const events = [
    { label: "Tạo khóa học", at: course.createdAt, by: course.createdById },
    { label: "Cập nhật gần nhất", at: course.updatedAt, by: course.updatedById },
    course.publishedAt ? { label: "Xuất bản", at: course.publishedAt, by: course.publishedById } : null,
    course.archivedAt ? { label: "Lưu trữ", at: course.archivedAt, by: course.archivedById } : null,
    course.deletedAt ? { label: "Xóa", at: course.deletedAt, by: course.deletedById } : null,
  ].filter(Boolean) as Array<{ label: string; at: string; by?: string | null }>;

  return (
    <FormSection
      title="Lịch sử khóa học"
      description="Hiển thị các mốc audit hiện có từ backend. Khi Audit Log chuyên dụng được nối vào Course, trang này có thể hiển thị lịch sử chi tiết từng thay đổi trường."
      icon={<Clock3 size={18} />}
    >
      {events.length === 0 ? (
        <EmptyState title="Chưa có lịch sử" description="Chưa ghi nhận sự kiện nào cho khóa học này." />
      ) : (
        <div className="space-y-3">
          {events.map((event) => (
            <div key={`${event.label}-${event.at}`} className="flex flex-col gap-2 rounded-[9px] border border-[#e8e3dc] bg-white p-4 md:flex-row md:items-center md:justify-between">
              <div>
                <div className="text-[12px] font-semibold text-[#333]">{event.label}</div>
                <div className="mt-1 text-[10px] text-[#888]">{formatDate(event.at)}</div>
              </div>
              <Badge variant="default">{event.by ? `User ${event.by.slice(0, 8)}…` : "Hệ thống"}</Badge>
            </div>
          ))}
        </div>
      )}
    </FormSection>
  );
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat("vi-VN", { dateStyle: "medium", timeStyle: "short" }).format(new Date(value));
}
