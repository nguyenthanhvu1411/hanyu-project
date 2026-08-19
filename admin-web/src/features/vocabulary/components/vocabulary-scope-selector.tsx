"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { ArrowRight } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";

interface VocabularyScopeSelectorProps {
  target: "nghia" | "vi-du" | "quan-he";
  title: string;
  description: string;
}

export function VocabularyScopeSelector({ target, title, description }: VocabularyScopeSelectorProps) {
  const router = useRouter();
  const [vocabularyId, setVocabularyId] = useState("");
  const id = Number(vocabularyId);
  const valid = Number.isSafeInteger(id) && id > 0;

  return (
    <Card className="max-w-[620px]">
      <CardHeader>
        <CardTitle>{title}</CardTitle>
        <p className="mt-1 text-[11px] leading-5 text-muted-foreground">{description}</p>
      </CardHeader>
      <CardContent className="space-y-3">
        <label className="block space-y-1">
          <span className="text-[11px] font-medium">Vocabulary ID</span>
          <Input
            type="number"
            min={1}
            value={vocabularyId}
            onChange={(event) => setVocabularyId(event.target.value)}
            placeholder="Nhập ID từ vựng cần quản lý..."
          />
        </label>
        <Button className="gap-2" disabled={!valid} onClick={() => router.push(`/tu-vung/${id}/${target}`)}>
          Mở workspace <ArrowRight size={14} />
        </Button>
        <p className="text-[10px] leading-4 text-muted-foreground">
          Backend hiện tổ chức dữ liệu theo từng Vocabulary, vì vậy trang này yêu cầu chọn Vocabulary trước thay vì tải toàn bộ dữ liệu bằng nhiều request N+1.
        </p>
      </CardContent>
    </Card>
  );
}
