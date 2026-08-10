"use client";

import { FormEvent, useState } from "react";
import type { CreateChapterRequest } from "../../types/curriculum.types";
import { Button } from "@/components/ui/button";

interface Props {
  initial?: CreateChapterRequest;
  nextSortOrder?: number;
  saving: boolean;
  onSubmit: (request: CreateChapterRequest) => Promise<unknown>;
  onCancel: () => void;
}

export function ChapterForm({
  initial,
  nextSortOrder = 0,
  saving,
  onSubmit,
  onCancel,
}: Props) {
  const [titleVi, setTitleVi] = useState(initial?.titleVi ?? "");
  const [descriptionVi, setDescriptionVi] = useState(initial?.descriptionVi ?? "");
  const [sortOrder, setSortOrder] = useState(initial?.sortOrder ?? nextSortOrder);
  const [isActive, setActive] = useState(initial?.isActive ?? true);

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!titleVi.trim()) {
      return;
    }

    await onSubmit({
      titleVi: titleVi.trim(),
      descriptionVi: descriptionVi.trim() || null,
      sortOrder,
      isActive,
    });
  }

  return (
    <form onSubmit={submit} className="rounded-xl border bg-white p-4">
      <div className="grid gap-4 lg:grid-cols-[1fr_160px]">
        <label className="space-y-1">
          <span className="text-sm font-medium">Tên chương</span>
          <input
            value={titleVi}
            onChange={(e) => setTitleVi(e.target.value)}
            maxLength={200}
            required
            className="h-10 w-full rounded-lg border px-3 text-sm"
          />
        </label>

        <label className="space-y-1">
          <span className="text-sm font-medium">Thứ tự</span>
          <input
            type="number"
            min={0}
            value={sortOrder}
            onChange={(e) => setSortOrder(Number(e.target.value))}
            className="h-10 w-full rounded-lg border px-3 text-sm"
          />
        </label>
      </div>

      <label className="mt-4 block space-y-1">
        <span className="text-sm font-medium">Mô tả</span>
        <textarea
          value={descriptionVi}
          onChange={(e) => setDescriptionVi(e.target.value)}
          rows={3}
          className="w-full rounded-lg border px-3 py-2 text-sm"
        />
      </label>

      <label className="mt-4 flex items-center gap-2 text-sm">
        <input
          type="checkbox"
          checked={isActive}
          onChange={(e) => setActive(e.target.checked)}
          className="h-4 w-4 rounded border-gray-300 text-red-600 focus:ring-red-500"
        />
        Hoạt động
      </label>

      <div className="mt-4 flex justify-end gap-2">
        <Button type="button" variant="outline" onClick={onCancel} disabled={saving}>
          Hủy
        </Button>
        <Button type="submit" variant="danger" disabled={saving}>
          Lưu chương
        </Button>
      </div>
    </form>
  );
}
