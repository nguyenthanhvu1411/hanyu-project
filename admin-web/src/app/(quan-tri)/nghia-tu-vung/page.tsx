import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { VocabularyScopeSelector } from "@/features/vocabulary/components/vocabulary-scope-selector";

export default function VocabularyMeaningsIndexPage() {
  return (
    <PageContainer>
      <PageHeader
        title="Nghĩa từ vựng"
        description="Chọn Vocabulary để quản lý danh sách nghĩa theo đúng scope API của backend."
      />
      <VocabularyScopeSelector
        target="nghia"
        title="Chọn từ vựng cần quản lý nghĩa"
        description="Meaning được lưu theo từng Vocabulary; sau khi nhập ID, bạn sẽ mở workspace CRUD nghĩa của từ đó."
      />
    </PageContainer>
  );
}
