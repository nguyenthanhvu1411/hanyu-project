import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { VocabularyScopeSelector } from "@/features/vocabulary/components/vocabulary-scope-selector";

export default function VocabularyExamplesIndexPage() {
  return (
    <PageContainer>
      <PageHeader
        title="Ví dụ từ vựng"
        description="Chọn Vocabulary để quản lý câu ví dụ và workflow theo đúng scope API của backend."
      />
      <VocabularyScopeSelector
        target="vi-du"
        title="Chọn từ vựng cần quản lý ví dụ"
        description="Example được quản lý theo từng Vocabulary; workspace hỗ trợ CRUD, audio asset và content workflow."
      />
    </PageContainer>
  );
}
