"use client";

import { useQuery } from "@tanstack/react-query";
import { khoaHocApi } from "@/features/khoa-hoc/api/khoa-hoc.api";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Skeleton } from "@/components/ui/skeleton";
import { SendHorizontal } from "lucide-react";
import { useParams, usePathname, useRouter } from "next/navigation";
import { ReactNode } from "react";
import { getContentStatusLabel } from "@/lib/constants/content-status";

export function CourseDetailLayout({ children }: { children: ReactNode }) {
  const params = useParams();
  const router = useRouter();
  const pathname = usePathname();
  
  const courseId = Number(params.id);

  const { data: course, isLoading } = useQuery({
    queryKey: ["course", courseId],
    queryFn: () => khoaHocApi.chiTiet(courseId),
    enabled: !!courseId,
  });

  if (isLoading) {
    return (
      <div className="space-y-4 p-6">
        <Skeleton className="h-10 w-1/3" />
        <Skeleton className="h-12 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    );
  }

  if (!course) {
    return <div className="p-6">Course not found</div>;
  }

  const currentTab = pathname.includes("/noi-dung") ? "noi-dung" :
                     pathname.includes("/tien-quyet") ? "tien-quyet" :
                     pathname.includes("/lich-su") ? "lich-su" : "tong-quan";

  const handleTabChange = (value: string) => {
    router.push(`/khoa-hoc/${courseId}/${value}`);
  };

  return (
    <div className="flex flex-col gap-6 p-6">
      <div className="flex items-center justify-between border-b pb-4">
        <div className="flex flex-col gap-2">
          <h1 className="text-2xl font-bold tracking-tight">{course.titleVi}</h1>
          <div className="flex items-center gap-2 text-sm text-muted-foreground">
            <Badge variant="primary">{course.hskCode || "Chưa có HSK"}</Badge>
            <Badge variant="info">
              {getContentStatusLabel(course.status)}
            </Badge>
          </div>
        </div>
        
        <div className="flex items-center gap-2">
          {course.status === 0 && ( // Draft
            <Button variant="primary" size="sm">
              <SendHorizontal className="mr-2 h-4 w-4" />
              Gửi duyệt
            </Button>
          )}
        </div>
      </div>

      <Tabs value={currentTab} onValueChange={handleTabChange}>
        <TabsList className="mb-4">
          <TabsTrigger value="tong-quan">Tổng quan</TabsTrigger>
          <TabsTrigger value="noi-dung">Nội dung</TabsTrigger>
          <TabsTrigger value="tien-quyet">Tiên quyết</TabsTrigger>
          <TabsTrigger value="lich-su">Lịch sử</TabsTrigger>
        </TabsList>
      </Tabs>

      <div className="flex-1 rounded-lg border bg-card p-6 shadow-sm">
        {children}
      </div>
    </div>
  );
}
