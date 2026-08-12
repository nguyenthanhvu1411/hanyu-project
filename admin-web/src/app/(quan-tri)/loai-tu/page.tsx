import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PartOfSpeechTable } from "@/features/vocabulary/components/part-of-speech-table";

export default function PartOfSpeechPage() {
  return (
    <PageContainer>
      <PageHeader
        title="Từ loại"
        description="Quản lý danh từ, động từ, tính từ và các từ loại dùng trong hệ thống từ vựng."
      />
      <PartOfSpeechTable />
    </PageContainer>
  );
}
