"use client";

import { useParams } from "next/navigation";

import { ErrorState } from "@/components/common/error-state";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { VocabularyNestedContentManager } from "@/features/vocabulary/components/vocabulary-nested-content-manager";

export default function VocabularyRelationsPage() {
  const params = useParams<{ id: string }>();
  const vocabularyId = Number(params.id);

  if (!Number.isSafeInteger(vocabularyId) || vocabularyId <= 0) {
    return <PageContainer><ErrorState title="Từ vựng không hợp lệ" description="ID từ vựng không đúng định dạng." /></PageContainer>;
  }

  return (
    <PageContainer>
      <PageHeader title="Quan hệ từ vựng" description={`Quản lý từ liên quan, dễ nhầm, đồng nghĩa và trái nghĩa của Vocabulary #${vocabularyId}.`} />
      <VocabularyNestedContentManager vocabularyId={vocabularyId} mode="relations" />
    </PageContainer>
  );
}
