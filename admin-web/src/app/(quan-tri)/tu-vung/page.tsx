import { Plus } from "lucide-react";
import Link from "next/link";

import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { Button } from "@/components/ui/button";
import { VocabularyTable } from "@/features/vocabulary/components/vocabulary-table";

export default function VocabularyPage() {
  return (
    <PageContainer>
      <PageHeader
        title="Từ vựng"
        description="Quản lý Hán tự, Pinyin, HSK, chủ đề, loại từ và workflow xuất bản."
        actions={
          <Link href="/tu-vung/them-moi">
            <Button className="h-[38px] gap-2 px-4 text-[11px]">
              <Plus size={14} />
              Thêm từ vựng
            </Button>
          </Link>
        }
      />
      <VocabularyTable />
    </PageContainer>
  );
}
