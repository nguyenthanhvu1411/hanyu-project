"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { useParams } from "next/navigation";

import { ErrorState } from "@/components/common/error-state";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import type { TopicDto } from "@/features/vocabulary/components/topic-table";
import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";
import { getContentStatusLabel } from "@/lib/constants/content-status";

export default function TopicDetailPage() {
  const params = useParams<{ id: string }>();
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

  if (loading) return <PageContainer><div className="rounded-[11px] border border-[#e8e3dc] bg-white p-8 text-center text-[11px] text-[#888]">Đang tải chủ đề...</div></PageContainer>;
  if (error && !topic) return <PageContainer><ErrorState title="Không thể mở chủ đề" description={error} /></PageContainer>;
  if (!topic) return null;

  return (
    <PageContainer>
      <PageHeader title={topic.nameVi} description="Chi tiết chủ đề nội dung dùng chung cho từ vựng và bài giảng." />

      <div className="rounded-[11px] border border-[#e8e3dc] bg-white p-5">
        <div className="mb-5 flex flex-col gap-3 border-b border-[#eee9e2] pb-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <div className="text-[10px] uppercase tracking-[0.08em] text-[#999]">Chủ đề</div>
            <div className="mt-1 text-[18px] font-semibold text-[#2f2f2f]">{topic.nameVi}</div>
            <div className="mt-1 text-[11px] text-[#888]">/{topic.slug}</div>
          </div>
          <div className="flex gap-2">
            <Link href="/chu-de-tu-vung" className="inline-flex h-[36px] items-center rounded-[7px] border border-[#ded9d2] px-4 text-[11px] font-medium text-[#666] hover:bg-[#f8f7f5]">Quay lại</Link>
            <Link href={`/chu-de-tu-vung/${topic.id}/chinh-sua`} className="inline-flex h-[36px] items-center rounded-[7px] bg-[#ef241c] px-4 text-[11px] font-semibold text-white hover:bg-[#d91f18]">Chỉnh sửa</Link>
          </div>
        </div>

        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
          <Info label="Tên chủ đề" value={topic.nameVi} />
          <Info label="Slug" value={topic.slug} />
          <Info label="Thứ tự hiển thị" value={String(topic.sortOrder)} />
          <Info label="Trạng thái" value={getContentStatusLabel(topic.status)} />
          <div className="md:col-span-2 xl:col-span-4"><Info label="Mô tả" value={topic.descriptionVi || "Chưa có mô tả."} /></div>
          <Info label="Ngày tạo" value={new Date(topic.createdAt).toLocaleString("vi-VN")} />
          <Info label="Cập nhật cuối" value={new Date(topic.updatedAt).toLocaleString("vi-VN")} />
        </div>
      </div>
    </PageContainer>
  );
}

function Info({ label, value }: { label: string; value: string }) {
  return <div className="rounded-[8px] bg-[#faf9f7] p-3"><div className="text-[10px] font-medium text-[#999]">{label}</div><div className="mt-1.5 text-[11px] leading-5 text-[#444]">{value}</div></div>;
}
