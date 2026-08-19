"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { ArrowRight, BookOpenText } from "lucide-react";

import { VocabularySelector } from "@/components/admin/entity-selectors";
import { EmptyState } from "@/components/common/empty-state";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

interface VocabularyScopeSelectorProps {
  target: "nghia" | "vi-du" | "quan-he";
  title: string;
  description: string;
}

export function VocabularyScopeSelector({ target, title, description }: VocabularyScopeSelectorProps) {
  const router = useRouter();
  const [vocabularyId, setVocabularyId] = useState("");

  return (
    <Card className="max-w-[720px]">
      <CardHeader>
        <CardTitle>{title}</CardTitle>
        <p className="mt-1 text-[12px] leading-5 text-muted-foreground">{description}</p>
      </CardHeader>
      <CardContent className="space-y-4">
        <label className="block space-y-1.5">
          <span className="text-[12px] font-medium text-[#555]">Từ vựng</span>
          <VocabularySelector
            value={vocabularyId}
            onValueChange={setVocabularyId}
            placeholder="Tìm và chọn từ vựng cần quản lý"
          />
        </label>

        {vocabularyId ? (
          <Button className="gap-2" onClick={() => router.push(`/tu-vung/${vocabularyId}/${target}`)}>
            Mở workspace
            <ArrowRight size={14} />
          </Button>
        ) : (
          <EmptyState
            icon={<BookOpenText size={24} />}
            title="Chọn một từ vựng để bắt đầu"
            description="Tìm theo chữ Hán, Pinyin hoặc nghĩa tiếng Việt. Hệ thống sẽ mở đúng workspace của từ đã chọn."
          />
        )}
      </CardContent>
    </Card>
  );
}
