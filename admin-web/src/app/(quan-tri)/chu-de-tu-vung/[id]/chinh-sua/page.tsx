"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";

import { ErrorState } from "@/components/common/error-state";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { Alert } from "@/components/ui/alert";
import { Card, CardContent } from "@/components/ui/card";
import { TopicForm, type TopicFormValues } from "@/features/vocabulary/components/topic-form";
import type { TopicDto } from "@/features/vocabulary/components/topic-table";
import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";

export default function EditTopicPage() {
  const params = useParams<{ id: string }>();
  const router = useRouter();
  const topicId = Number(params.id);
  const [topic, setTopic] = useState<TopicDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
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

  async function handleSubmit(values: TopicFormValues) {
    if (!topic) return;
    setSubmitting(true);
    setError(null);
    try {
      await apiClient(`${API_ENDPOINTS.VOCABULARY.TOPICS}/${topic.id}`, {
        method: "PUT",
        body: {
          slug: values.slug,
          nameVi: values.nameVi,
          descriptionVi: values.descriptionVi || null,
          sortOrder: values.sortOrder,
        },
      });
      router.push(`/chu-de-tu-vung/${topic.id}`);
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : "Không thể cập nhật chủ đề.");
    } finally {
      setSubmitting(false);
    }
  }

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
        title={`Chỉnh sửa: ${topic.nameVi}`}
        description="Cập nhật thông tin chủ đề dùng chung cho từ vựng và bài giảng."
      />

      {error && (
        <Alert variant="danger" title="Không thể cập nhật chủ đề" className="mb-4">
          {error}
        </Alert>
      )}

      <TopicForm
        topicId={topic.id}
        initialValues={{
          slug: topic.slug,
          nameVi: topic.nameVi,
          descriptionVi: topic.descriptionVi ?? "",
          sortOrder: topic.sortOrder,
        }}
        submitting={submitting}
        onSubmit={handleSubmit}
        onCancel={() => router.push(`/chu-de-tu-vung/${topic.id}`)}
      />
    </PageContainer>
  );
}
