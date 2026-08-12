"use client";

import { useParams } from "next/navigation";

import { ErrorState } from "@/components/common/error-state";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { VocabularyForm } from "@/features/vocabulary/components/vocabulary-form";

export default function EditVocabularyPage() {
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
        title="Chỉnh sửa từ vựng"
        description="Cập nhật thông tin chung và phân loại. Version kỹ thuật được giữ để bảo vệ optimistic concurrency."
      />
      <VocabularyForm vocabularyId={vocabularyId} />
    </PageContainer>
  );
}
