"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";

import { ErrorState } from "@/components/common/error-state";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import type { TopicDto } from "@/features/vocabulary/components/topic-table";
import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";
import { ContentStatus, getContentStatusLabel } from "@/lib/constants/content-status";

function statusVariant(status: ContentStatus): "default" | "primary" | "success" | "warning" | "info" {
  switch (status) {
    case ContentStatus.Published:
      return "success";
    case ContentStatus.Approved:
      return "info";
    case ContentStatus.Review:
      return "warning";
    case ContentStatus.Archived:
      return "default";
    default:
      return "primary";
  }
}

export default function TopicDetailPage() {
  const params = useParams<{ id: string }>();
  const router = useRouter();
  const topicId = Number(params.id);
  const [topic, setTopic] = useState<TopicDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!Number.isSafeInteger(topicId) || topicId <= 0) {
      setError("ID chủ đề không hợp lệ.");
      setLoading(false);
      return;
    }

    void (async () => {
      try {
        const items = await apiClient<TopicDto[]>(API_ENDPOINTS.VOCABULARY.TOPICS);
        const found = items.find((item) => item.id === topicId) ?? null;
        if (!found) setError("Không tìm thấy chủ đề.");
        setTopic(found);
      } catch (exception) {
        setError(exception instanceof Error ? exception.message : "Không thể tải chủ đề.");
      } finally {
        setLoading(false);
      }
    })();
  }, [topicId]);

  if (loading) {
    return (
      <PageContainer>
        <Card>
          <CardContent className="p-8 text-center text-[14px] text-[#777]">
            Đang tải chủ đề...
          </CardContent>
        </Card>
      </PageContainer>
    );
  }

  if (error && !topic) {
    return (
      <PageContainer>
        <ErrorState title="Không thể mở chủ đề" description={error} />
      </PageContainer>
    );
  }

  if (!topic) return null;

  return (
    <PageContainer>
      <PageHeader
        title={topic.nameVi}
        description="Chi tiết chủ đề nội dung dùng chung cho từ vựng và bài giảng."
      />

      <Card>
        <CardHeader className="flex flex-col gap-4 px-5 py-5 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <div className="text-[13px] font-medium uppercase tracking-[0.06em] text-[#888]">
              Chủ đề
            </div>
            <div className="mt-1 text-[18px] font-semibold text-[#2f2f2f]">{topic.nameVi}</div>
            <div className="mt-1 text-[13px] text-[#777]">/{topic.slug}</div>
          </div>

          <div className="flex gap-2">
            <Button
              type="button"
              variant="outline"
              size="md"
              onClick={() => router.push("/chu-de-tu-vung")}
            >
              Quay lại
            </Button>
            <Button
              type="button"
              variant="primary"
              size="md"
              onClick={() => router.push(`/chu-de-tu-vung/${topic.id}/chinh-sua`)}
            >
              Chỉnh sửa
            </Button>
          </div>
        </CardHeader>

        <CardContent className="grid gap-4 p-5 md:grid-cols-2 xl:grid-cols-4">
          <Info label="Tên chủ đề" value={topic.nameVi} />
          <Info label="Slug" value={topic.slug} />
          <Info label="Thứ tự hiển thị" value={String(topic.sortOrder)} />
          <Info
            label="Trạng thái"
            value={
              <Badge variant={statusVariant(topic.status)} className="px-2.5 py-1 text-[12px]">
                {getContentStatusLabel(topic.status)}
              </Badge>
            }
          />

          <div className="md:col-span-2 xl:col-span-4">
            <Info label="Mô tả" value={topic.descriptionVi || "Chưa có mô tả."} />
          </div>

          <Info label="Ngày tạo" value={new Date(topic.createdAt).toLocaleString("vi-VN")} />
          <Info label="Cập nhật cuối" value={new Date(topic.updatedAt).toLocaleString("vi-VN")} />
        </CardContent>
      </Card>
    </PageContainer>
  );
}

function Info({ label, value }: { label: string; value: React.ReactNode }) {
  return (
    <div className="rounded-[8px] border border-[#eee9e2] bg-[#faf9f7] p-4">
      <div className="text-[13px] font-medium text-[#777]">{label}</div>
      <div className="mt-2 text-[14px] leading-6 text-[#333]">{value}</div>
    </div>
  );
}
