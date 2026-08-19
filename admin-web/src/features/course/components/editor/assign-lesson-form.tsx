"use client";

import { useEffect, useState } from "react";
import { lessonApi } from "@/features/lesson/api/lesson.api";
import type { AdminLessonListItem } from "@/features/lesson/types/lesson.types";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";

interface Props {
  courseId: number;
  chapterId: number;
  onAssign: (lessonId: number) => Promise<unknown>;
  onCancel: () => void;
}

export function AssignLessonForm({ onAssign, onCancel }: Props) {
  const [search, setSearch] = useState("");
  const [items, setItems] = useState<AdminLessonListItem[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const timeout = window.setTimeout(async () => {
      setLoading(true);
      try {
        const result = await lessonApi.list({ search, page: 1, pageSize: 50 });
        setItems(result.items.filter((item) => !item.chapterId));
      } finally {
        setLoading(false);
      }
    }, 250);
    return () => window.clearTimeout(timeout);
  }, [search]);

  return (
    <Card className="w-full shadow-none">
      <CardContent className="space-y-3 p-3">
        <div className="flex gap-2">
          <Input autoFocus placeholder="Tìm bài học chưa thuộc chương..." value={search} onChange={(event) => setSearch(event.target.value)} className="flex-1" />
          <Button type="button" variant="outline" size="sm" onClick={onCancel}>Đóng</Button>
        </div>

        <div className="max-h-64 space-y-1 overflow-y-auto">
          {loading ? <p className="p-3 text-[11px] text-[#888]">Đang tìm...</p> : null}
          {!loading ? items.map((lesson) => (
            <Button
              type="button"
              variant="ghost"
              key={lesson.id}
              onClick={async () => { await onAssign(lesson.id); onCancel(); }}
              className="h-auto w-full justify-between px-3 py-2 text-left"
            >
              <span className="min-w-0">
                <span className="block truncate text-[12px] font-medium">{lesson.titleVi}</span>
                <span className="block truncate text-[10px] text-[#888]">{lesson.slug}</span>
              </span>
              <span className="text-[10px] text-[#ef241c]">Thêm</span>
            </Button>
          )) : null}
          {!loading && items.length === 0 ? <p className="p-3 text-[11px] text-[#888]">Không tìm thấy bài học phù hợp.</p> : null}
        </div>
      </CardContent>
    </Card>
  );
}
