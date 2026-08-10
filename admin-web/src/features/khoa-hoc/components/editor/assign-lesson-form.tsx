"use client";

import { useEffect, useState } from "react";
import { baiGiangApi } from "@/features/bai-giang/api/bai-giang.api";
import type { AdminLessonListItem } from "@/features/bai-giang/types/bai-giang.types";
import { Button } from "@/components/ui/button";

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
        const result = await baiGiangApi.danhSach({
          search,
          page: 1,
          pageSize: 50,
        });

        setItems(result.items.filter((item) => !item.chapterId));
      } finally {
        setLoading(false);
      }
    }, 250);

    return () => window.clearTimeout(timeout);
  }, [search]);

  return (
    <div className="rounded-lg border bg-white p-3">
      <div className="flex gap-2">
        <input
          autoFocus
          placeholder="Tìm bài học chưa thuộc chương..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="h-9 flex-1 rounded-lg border px-3 text-sm"
        />

        <Button
          type="button"
          variant="outline"
          size="sm"
          onClick={onCancel}
        >
          Đóng
        </Button>
      </div>

      <div className="mt-2 max-h-64 overflow-y-auto">
        {loading && <p className="p-3 text-sm text-neutral-500">Đang tìm...</p>}

        {!loading &&
          items.map((lesson) => (
            <button
              type="button"
              key={lesson.id}
              onClick={async () => {
                await onAssign(lesson.id);
                onCancel();
              }}
              className="flex w-full items-center justify-between border-b px-3 py-2 text-left last:border-b-0 hover:bg-neutral-50"
            >
              <span>
                <span className="block text-sm font-medium">
                  {lesson.titleVi}
                </span>
                <span className="text-xs text-neutral-500">{lesson.slug}</span>
              </span>

              <span className="text-xs text-red-600">Thêm</span>
            </button>
          ))}

        {!loading && items.length === 0 && (
          <p className="p-3 text-sm text-neutral-500">
            Không tìm thấy bài học phù hợp.
          </p>
        )}
      </div>
    </div>
  );
}
