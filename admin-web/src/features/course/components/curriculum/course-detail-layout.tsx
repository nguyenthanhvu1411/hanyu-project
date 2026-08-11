"use client";

import { ReactNode } from "react";
import { useParams, usePathname, useRouter } from "next/navigation";
import { useQuery } from "@tanstack/react-query";
import { courseApi } from "@/features/course/api/course.api";
import { ErrorState } from "@/components/common/error-state";
import { StorageImage } from "@/components/media/storage-image";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { getContentStatusLabel } from "@/lib/constants/content-status";

export function CourseDetailLayout({ children }: { children: ReactNode }) {
  const params = useParams<{ id: string }>();
  const router = useRouter();
  const pathname = usePathname();
  const courseId = Number(params.id);

  const { data: course, isLoading, error, refetch } = useQuery({
    queryKey: ["course", courseId],
    queryFn: () => courseApi.getById(courseId),
    enabled: Number.isSafeInteger(courseId) && courseId > 0,
  });

  if (isLoading) {
    return <div className="space-y-4"><Skeleton className="h-28 w-full rounded-[11px]" /><Skeleton className="h-12 w-full rounded-[11px]" /><Skeleton className="h-64 w-full rounded-[11px]" /></div>;
  }

  if (!course) {
    return <ErrorState title="Không tìm thấy khóa học" description={error instanceof Error ? error.message : "Dữ liệu khóa học không tồn tại."} onRetry={() => void refetch()} />;
  }

  const currentTab = pathname.includes("/noi-dung")
    ? "noi-dung"
    : pathname.includes("/tien-quyet")
      ? "tien-quyet"
      : pathname.includes("/lich-su")
        ? "lich-su"
        : pathname.includes("/thong-ke")
          ? "thong-ke"
          : pathname.includes("/hoc-vien")
            ? "hoc-vien"
            : "tong-quan";

  return (
    <div className="space-y-5">
      <Card>
        <CardHeader className="flex flex-row items-center justify-between gap-3">
          <div className="flex min-w-0 items-center gap-4">
            <div className="h-16 w-24 shrink-0 overflow-hidden rounded-[9px] border border-[#ebe5dd] bg-[#faf9f7]">
              <StorageImage
                value={course.coverImageUrl}
                alt={`Ảnh bìa ${course.titleVi}`}
                className="h-full w-full object-cover"
                emptyClassName="h-full min-h-0"
              />
            </div>
            <div className="min-w-0">
              <CardTitle className="truncate text-[18px]">{course.titleVi}</CardTitle>
              <div className="mt-2 flex flex-wrap gap-2">
                <Badge>{course.code}</Badge>
                <Badge variant="primary">{course.hskCode || "Chưa có HSK"}</Badge>
                <Badge variant="info">{getContentStatusLabel(course.status)}</Badge>
                {course.isFeatured ? <Badge variant="warning">Nổi bật</Badge> : null}
              </div>
            </div>
          </div>
        </CardHeader>
      </Card>

      <Tabs value={currentTab} onValueChange={(value) => router.push(`/khoa-hoc/${courseId}/${value}`)}>
        <TabsList className="rounded-[11px] border border-[#e8e3dc] bg-white px-2">
          <TabsTrigger value="tong-quan">Tổng quan</TabsTrigger>
          <TabsTrigger value="noi-dung">Nội dung</TabsTrigger>
          <TabsTrigger value="tien-quyet">Tiên quyết</TabsTrigger>
          <TabsTrigger value="lich-su">Lịch sử</TabsTrigger>
          <TabsTrigger value="thong-ke">Thống kê</TabsTrigger>
          <TabsTrigger value="hoc-vien">Học viên</TabsTrigger>
        </TabsList>
      </Tabs>

      <Card><CardContent className="p-5">{children}</CardContent></Card>
    </div>
  );
}
