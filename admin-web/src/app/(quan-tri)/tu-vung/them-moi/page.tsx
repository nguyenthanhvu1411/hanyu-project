import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { VocabularyForm } from "@/features/vocabulary/components/vocabulary-form";

export default function CreateVocabularyPage() {
  return (
    <PageContainer>
      <PageHeader
        title="Thêm từ vựng"
        description="Tạo từ vựng mới và gắn HSK, loại từ, chủ đề trước khi đưa vào workflow duyệt."
      />
      <VocabularyForm />
    </PageContainer>
  );
}
