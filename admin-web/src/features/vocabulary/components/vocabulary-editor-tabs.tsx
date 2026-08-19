"use client";

import { useState } from "react";

import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { cn } from "@/lib/utils/cn";
import { VocabularyAudioTab } from "./vocabulary-audio-tab";
import { VocabularyExamplesTab } from "./vocabulary-examples-tab";
import { VocabularyForm } from "./vocabulary-form";
import { VocabularyMeaningsTab } from "./vocabulary-meanings-tab";
import { VocabularyRelationsTab } from "./vocabulary-relations-tab";

type TabKey = "general" | "meanings" | "examples" | "relations" | "audio";

const TABS: Array<{ key: TabKey; label: string }> = [
  { key: "general", label: "Thông tin chung" },
  { key: "meanings", label: "Nghĩa" },
  { key: "examples", label: "Ví dụ" },
  { key: "relations", label: "Quan hệ" },
  { key: "audio", label: "Audio" },
];

export function VocabularyEditorTabs({ vocabularyId }: { vocabularyId: number }) {
  const [activeTab, setActiveTab] = useState<TabKey>("general");

  return (
    <div className="space-y-4">
      <Card>
        <CardContent className="flex flex-wrap gap-1 p-1.5">
          {TABS.map((tab) => (
            <Button
              key={tab.key}
              type="button"
              size="sm"
              variant="ghost"
              onClick={() => setActiveTab(tab.key)}
              className={cn(
                "h-9 px-4 text-[13px]",
                activeTab === tab.key
                  ? "bg-[#fff0ee] text-[#d92720] hover:bg-[#fff0ee]"
                  : "text-[#666]",
              )}
            >
              {tab.label}
            </Button>
          ))}
        </CardContent>
      </Card>

      {activeTab === "general" && <VocabularyForm vocabularyId={vocabularyId} />}
      {activeTab === "meanings" && <VocabularyMeaningsTab vocabularyId={vocabularyId} />}
      {activeTab === "examples" && <VocabularyExamplesTab vocabularyId={vocabularyId} />}
      {activeTab === "relations" && <VocabularyRelationsTab vocabularyId={vocabularyId} />}
      {activeTab === "audio" && <VocabularyAudioTab vocabularyId={vocabularyId} />}
    </div>
  );
}
