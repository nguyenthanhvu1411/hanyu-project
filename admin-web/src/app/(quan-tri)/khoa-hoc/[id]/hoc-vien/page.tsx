"use client";

import { useMemo, useState } from "react";
import { useParams } from "next/navigation";
import { RefreshCw, Search, Users } from "lucide-react";
import { useQuery } from "@tanstack/react-query";

import { EmptyState } from "@/components/common/empty-state";
import { ErrorState } from "@/components/common/error-state";
import { FormSection } from "@/components/forms/form-section";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { courseApi } from "@/features/course/api/course.api";
import type { CourseStudentStatus } from "@/features/course/types/course-insights.types";

const PAGE_SIZE = 20;

export default function CourseStudentsPage() {
  const params = useParams<{ id: string }>();
  const courseId = Number(params.id);
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState<CourseStudentStatus | "">("");
  const [page, setPage] = useState(1);

  const queryArgs = useMemo(
    () => ({
      search: search.trim() || undefined,
      status: status || undefined,
      page,
      pageSize: PAGE_SIZE,
    }),
    [page, search, status],
  );

  const query = useQuery({
    queryKey: ["course-students", courseId, queryArgs],
    queryFn: () => courseApi.students(courseId, queryArgs),
    enabled: Number.isSafeInteger(courseId) && courseId > 0,
  });

  if (!Number.isSafeInteger(courseId) || courseId <= 0) {
    return <ErrorState title="Khóa học không hợp lệ" description="CourseId phải là số nguyên dương." />;
  }

  const result = query.data;
  const items = result?.items ?? [];
  const total = result?.total ?? 0;
  const totalPages = Math.max(1, Math.ceil(total / PAGE_SIZE));

  return (
    <FormSection
      title="Học viên khóa học"
      description="Danh sách được tổng hợp từ UserLessonProgress của các Lesson thuộc Course."
      icon={<Users size={18} />}
    >
      <div className="space-y-4">
        <div className="flex flex-col gap-2 lg:flex-row lg:items-center lg:justify-between">
          <div className="flex flex-1 flex-col gap-2 sm:flex-row">
            <div className="relative flex-1">
              <Search className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-[#999]" size={14} />
              <Input
                value={search}
                onChange={(event) => {
                  setSearch(event.target.value);
                  setPage(1);
                }}
                className="pl-9"
                placeholder="Tìm theo tên hoặc email học viên..."
              />
            </div>
            <div className="w-full sm:w-[190px]">
              <Select
                value={status}
                onValueChange={(value) => {
                  setStatus(value as CourseStudentStatus | "");
                  setPage(1);
                }}
                options={[
                  { value: "in_progress", label: "Đang học" },
                  { value: "completed", label: "Hoàn thành" },
                ]}
                placeholder="Tất cả trạng thái"
                clearable
              />
            </div>
          </div>

          <Button variant="outline" size="sm" className="gap-2" onClick={() => void query.refetch()}>
            <RefreshCw size={14} /> Làm mới
          </Button>
        </div>

        <div className="text-[10px] text-[#888]">
          {query.isFetching ? "Đang cập nhật dữ liệu..." : `Tổng ${total} học viên có tiến độ trong khóa học.`}
        </div>

        {query.isLoading ? (
          <div className="space-y-2">
            {Array.from({ length: 5 }).map((_, index) => (
              <Skeleton key={index} className="h-12 w-full rounded-[8px]" />
            ))}
          </div>
        ) : query.error ? (
          <ErrorState
            title="Không thể tải học viên"
            description={query.error instanceof Error ? query.error.message : "Không thể tải dữ liệu."}
            onRetry={() => void query.refetch()}
          />
        ) : items.length === 0 ? (
          <EmptyState
            title="Chưa có học viên phù hợp"
            description={search || status ? "Không có học viên khớp bộ lọc hiện tại." : "Chưa có người dùng bắt đầu Lesson nào trong khóa học."}
          />
        ) : (
          <div className="overflow-x-auto rounded-[9px] border border-[#e8e3dc]">
            <table className="w-full min-w-[980px] border-collapse text-left">
              <thead className="bg-[#faf9f7] text-[10px] font-semibold text-[#666]">
                <tr>
                  <th className="px-3 py-2.5">STT</th>
                  <th className="px-3 py-2.5">Học viên</th>
                  <th className="px-3 py-2.5">Bài đã bắt đầu</th>
                  <th className="px-3 py-2.5">Hoàn thành</th>
                  <th className="px-3 py-2.5">Tiến độ</th>
                  <th className="px-3 py-2.5">Trạng thái</th>
                  <th className="px-3 py-2.5">Bắt đầu</th>
                  <th className="px-3 py-2.5">Truy cập cuối</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-[#eee9e3] bg-white text-[11px] text-[#444]">
                {items.map((student, index) => (
                  <tr key={student.userId} className="hover:bg-[#fcfbf9]">
                    <td className="px-3 py-3 text-[#888]">{(page - 1) * PAGE_SIZE + index + 1}</td>
                    <td className="px-3 py-3">
                      <div className="font-semibold text-[#333]">{student.displayName}</div>
                      <div className="mt-0.5 text-[10px] text-[#888]">{student.email || student.userId}</div>
                    </td>
                    <td className="px-3 py-3">{student.startedLessons}/{student.totalLessons}</td>
                    <td className="px-3 py-3">{student.completedLessons}/{student.totalLessons}</td>
                    <td className="px-3 py-3">
                      <div className="flex items-center gap-2">
                        <div className="h-1.5 w-24 overflow-hidden rounded-full bg-[#eee]">
                          <div
                            className="h-full rounded-full bg-current text-[#ef241c]"
                            style={{ width: `${Math.min(100, Math.max(0, student.completionPercent))}%` }}
                          />
                        </div>
                        <span>{formatPercent(student.completionPercent)}</span>
                      </div>
                    </td>
                    <td className="px-3 py-3">
                      <Badge variant={student.status === "completed" ? "success" : "info"}>
                        {student.status === "completed" ? "Hoàn thành" : "Đang học"}
                      </Badge>
                    </td>
                    <td className="px-3 py-3 text-[#777]">{formatDate(student.startedAt)}</td>
                    <td className="px-3 py-3 text-[#777]">{formatDate(student.lastAccessedAt)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {total > 0 ? (
          <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
            <div className="text-[10px] text-[#888]">Trang {page}/{totalPages}</div>
            <div className="flex gap-2">
              <Button variant="outline" size="sm" disabled={page <= 1 || query.isFetching} onClick={() => setPage((value) => Math.max(1, value - 1))}>
                Trước
              </Button>
              <Button variant="outline" size="sm" disabled={page >= totalPages || query.isFetching} onClick={() => setPage((value) => Math.min(totalPages, value + 1))}>
                Sau
              </Button>
            </div>
          </div>
        ) : null}
      </div>
    </FormSection>
  );
}

function formatDate(value?: string | null) {
  if (!value) return "—";
  return new Intl.DateTimeFormat("vi-VN", { dateStyle: "short", timeStyle: "short" }).format(new Date(value));
}

function formatPercent(value: number) {
  return new Intl.NumberFormat("vi-VN", { maximumFractionDigits: 2 }).format(value) + "%";
}
