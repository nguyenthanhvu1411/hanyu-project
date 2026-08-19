"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import { BarChart3, BookOpenText, ImageIcon, Link2 } from "lucide-react";

import { ErrorState } from "@/components/common/error-state";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { getContentStatusLabel } from "@/lib/constants/content-status";
import { lessonApi } from "@/features/lesson/api/lesson.api";
import type { AdminLessonDetail } from "@/features/lesson/types/lesson.types";

export default function LessonStatisticsPage() {
  const params = useParams<{ id: string }>();
  const lessonId = Number(params.id);
  const [detail, setDetail] = useState<AdminLessonDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!Number.isSafeInteger(lessonId) || lessonId <= 0) {
      setError("ID bài giảng không hợp lệ.");
      setLoading(false);
      return;
    }

    let active = true;
    setLoading(true);
    setError(null);

    void lessonApi
      .getById(lessonId)
      .then((result) => {
        if (active) setDetail(result);
      })
      .catch((caught) => {
        if (active) setError(caught instanceof Error ? caught.message : "Không thể tải thống kê bài giảng.");
      })
      .finally(() => {
        if (active) setLoading(false);
      });

    return () => {
      active = false;
    };
  }, [lessonId]);

  if (loading) {
    return (
      <PageContainer>
        <Skeleton className="h-24 w-full rounded-[11px]" />
        <Skeleton className="mt-4 h-48 w-full rounded-[11px]" />
      </PageContainer>
    );
  }

  if (!detail || error) {
    return (
      <PageContainer>
        <ErrorState title="Không thể tải thống kê bài giảng" description={error ?? "Không có dữ liệu."} />
      </PageContainer>
    );
  }

  const metrics = [
    { label: "Section", value: detail.sectionCount, icon: BookOpenText },
    { label: "Từ vựng", value: detail.vocabularyCount, icon: BarChart3 },
    { label: "Tài nguyên", value: detail.assetCount, icon: ImageIcon },
    { label: "Tiên quyết", value: detail.prerequisiteCount, icon: Link2 },
  ];

  return (
    <PageContainer>
      <PageHeader
        title={`Thống kê · ${detail.titleVi}`}
        description={`Trạng thái: ${getContentStatusLabel(detail.status)} · Phiên bản v${detail.version}`}
      />

      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        {metrics.map((metric) => {
          const Icon = metric.icon;
          return (
            <Card key={metric.label}>
              <CardContent className="flex items-center gap-3 p-4">
                <div className="flex h-10 w-10 items-center justify-center rounded-[9px] bg-[#fff0ee] text-[#ef241c]">
                  <Icon size={18} />
                </div>
                <div>
                  <div className="text-[20px] font-semibold text-[#2f2f2f]">{metric.value}</div>
                  <div className="text-[11px] text-[#888]">{metric.label}</div>
                </div>
              </CardContent>
            </Card>
          );
        })}
      </div>

      <Card className="mt-4">
        <CardHeader>
          <CardTitle>Thông tin học tập</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-3 text-[12px] md:grid-cols-2 xl:grid-cols-4">
          <Info label="HSK" value={detail.hskCode ?? `#${detail.hskLevelId}`} />
          <Info label="Thời lượng" value={`${detail.estimatedMinutes} phút`} />
          <Info label="Độ khó" value={`${detail.difficulty}/5`} />
          <Info label="Nổi bật" value={detail.isFeatured ? "Có" : "Không"} />
        </CardContent>
      </Card>
    </PageContainer>
  );
}

function Info({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-[8px] bg-[#faf9f7] p-3">
      <div className="text-[10px] text-[#888]">{label}</div>
      <div className="mt-1 font-medium text-[#333]">{value}</div>
    </div>
  );
}
