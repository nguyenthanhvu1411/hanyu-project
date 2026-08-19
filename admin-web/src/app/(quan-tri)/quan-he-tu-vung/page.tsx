import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { VocabularyScopeSelector } from "@/features/vocabulary/components/vocabulary-scope-selector";

export default function VocabularyRelationsIndexPage() {
  return (
    <PageContainer>
      <PageHeader
        title="Quan hệ từ vựng"
        description="Chọn Vocabulary để quản lý các từ liên quan, dễ nhầm, đồng nghĩa và trái nghĩa."
      />
      <VocabularyScopeSelector
        target="quan-he"
        title="Chọn từ vựng cần quản lý quan hệ"
        description="Relation được lưu theo từng Vocabulary; workspace cho phép CRUD trên API nested thật của backend."
      />
    </PageContainer>
  );
}
