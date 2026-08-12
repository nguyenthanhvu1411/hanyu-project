"use client";

import { useParams } from "next/navigation";

import { ErrorState } from "@/components/common/error-state";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { VocabularyEditorTabs } from "@/features/vocabulary/components/vocabulary-editor-tabs";
import { VocabularyValidationPanel } from "@/features/vocabulary/components/vocabulary-validation-panel";

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
        title="Vocabulary Editor"
        description="Biên tập thông tin chung, nghĩa, ví dụ, quan hệ và audio trong cùng một workspace."
      />
      <div className="space-y-5">
        <VocabularyEditorTabs vocabularyId={vocabularyId} />
        <VocabularyValidationPanel vocabularyId={vocabularyId} />
      </div>
    </PageContainer>
  );
}
