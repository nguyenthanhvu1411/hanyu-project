"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";

import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { Alert } from "@/components/ui/alert";
import { TopicForm, type TopicFormValues } from "@/features/vocabulary/components/topic-form";
import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";

export default function CreateTopicPage() {
  const router = useRouter();
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(values: TopicFormValues) {
    setSubmitting(true);
    setError(null);
    try {
      const created = await apiClient<{ id: number }>(API_ENDPOINTS.VOCABULARY.TOPICS, {
        method: "POST",
        body: {
          slug: values.slug,
          nameVi: values.nameVi,
          descriptionVi: values.descriptionVi || null,
          sortOrder: values.sortOrder,
        },
      });
      router.push(`/chu-de-tu-vung/${created.id}`);
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : "Không thể tạo chủ đề.");
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <PageContainer>
      <PageHeader
        title="Thêm chủ đề"
        description="Tạo chủ đề nội dung dùng chung cho từ vựng và bài giảng."
      />

      {error && (
        <Alert variant="danger" title="Không thể tạo chủ đề" className="mb-4">
          {error}
        </Alert>
      )}

      <TopicForm
        submitting={submitting}
        onSubmit={handleSubmit}
        onCancel={() => router.push("/chu-de-tu-vung")}
      />
    </PageContainer>
  );
}
