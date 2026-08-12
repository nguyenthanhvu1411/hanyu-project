"use client";

import { useParams } from "next/navigation";

import { ErrorState } from "@/components/common/error-state";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { VocabularyDetail } from "@/features/vocabulary/components/vocabulary-detail";

export default function VocabularyDetailPage() {
  const params = useParams<{ id: string }>();
  const vocabularyId = Number(params.id);

  if (!Number.isSafeInteger(vocabularyId) || vocabularyId <= 0) {
    return (
      <PageContainer>
        <ErrorState
          title="Từ vựng không hợp lệ"
          description="ID từ vựng không đúng định dạng."
        />
      </PageContainer>
    );
  }

  return (
    <PageContainer>
      <PageHeader
        title="Chi tiết từ vựng"
        description="Xem thông tin, phân loại, workflow và các nhóm nội dung mở rộng của từ vựng."
      />
      <VocabularyDetail vocabularyId={vocabularyId} />
    </PageContainer>
  );
}
