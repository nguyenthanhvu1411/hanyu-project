"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import Link from "next/link";
import { Clock, Pencil, RefreshCw, Search, Star } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  ContentStatus,
  getContentStatusLabel,
} from "@/lib/constants/content-status";

import { baiGiangApi } from "../api/bai-giang.api";
import type {
  AdminLessonListItem,
  AdminLessonQuery,
} from "../types/bai-giang.types";

const PAGE_SIZE = 20;

function statusClass(status: ContentStatus) {
  switch (status) {
    case ContentStatus.Published:
      return "bg-emerald-50 text-emerald-700 ring-emerald-200";
    case ContentStatus.Approved:
      return "bg-sky-50 text-sky-700 ring-sky-200";
    case ContentStatus.Review:
      return "bg-amber-50 text-amber-700 ring-amber-200";
    case ContentStatus.Archived:
      return "bg-slate-100 text-slate-600 ring-slate-200";
    default:
      return "bg-zinc-50 text-zinc-700 ring-zinc-200";
  }
}

export function LessonTable() {
  const [items, setItems] = useState<AdminLessonListItem[]>([]);
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState<string>("");
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const query = useMemo<AdminLessonQuery>(
    () => ({
      search: search.trim() || undefined,
      status: status === "" ? undefined : Number(status) as ContentStatus,
      page,
      pageSize: PAGE_SIZE,
      sortBy: "updatedAt",
      sortDescending: true,
    }),
    [page, search, status],
  );

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const result = await baiGiangApi.danhSach(query);
      setItems(result.items ?? []);
      setTotalCount(result.totalCount ?? 0);
    } catch (cause) {
      setItems([]);
      setTotalCount(0);
      setError(
        cause instanceof Error
          ? cause.message
          : "Không thể tải danh sách bài giảng.",
      );
    } finally {
      setLoading(false);
    }
  }, [query]);

  useEffect(() => {
    void load();
  }, [load]);

  const totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE));

  return (
    <section className="overflow-hidden rounded-xl border border-zinc-200 bg-white shadow-sm">
      <div className="flex flex-col gap-3 border-b border-zinc-200 p-4 lg:flex-row lg:items-center lg:justify-between">
        <div className="flex flex-1 flex-col gap-2 sm:flex-row">
          <label className="relative block min-w-0 flex-1 lg:max-w-[420px]">
            <Search
              className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-zinc-400"
              size={16}
            />
            <input
              value={search}
              onChange={(event) => {
                setSearch(event.target.value);
                setPage(1);
              }}
              placeholder="Tìm theo tên, slug, khóa học..."
              className="h-[38px] w-full rounded-md border border-zinc-200 bg-white pl-9 pr-3 text-[13px] outline-none transition focus:border-zinc-400 focus:ring-2 focus:ring-zinc-100"
            />
          </label>

          <select
            value={status}
            onChange={(event) => {
              setStatus(event.target.value);
              setPage(1);
            }}
            className="h-[38px] rounded-md border border-zinc-200 bg-white px-3 text-[13px] outline-none focus:border-zinc-400"
          >
            <option value="">Tất cả trạng thái</option>
            <option value={ContentStatus.Draft}>Bản nháp</option>
            <option value={ContentStatus.Review}>Chờ duyệt</option>
            <option value={ContentStatus.Approved}>Đã duyệt</option>
            <option value={ContentStatus.Published}>Đã xuất bản</option>
            <option value={ContentStatus.Archived}>Đã lưu trữ</option>
          </select>
        </div>

        <Button
          variant="outline"
          className="h-[38px] gap-2 text-[12px]"
          onClick={() => void load()}
          disabled={loading}
        >
          <RefreshCw size={14} className={loading ? "animate-spin" : ""} />
          Làm mới
        </Button>
      </div>

      {error ? (
        <div className="m-4 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-[13px] text-red-700">
          {error}
        </div>
      ) : null}

      <div className="overflow-x-auto">
        <table className="w-full min-w-[980px] text-left text-[13px]">
          <thead className="bg-zinc-50 text-[12px] font-semibold uppercase tracking-wide text-zinc-500">
            <tr>
              <th className="w-[64px] px-4 py-3 text-center">STT</th>
              <th className="px-4 py-3">Bài giảng</th>
              <th className="px-4 py-3">Phân loại</th>
              <th className="px-4 py-3 text-center">Thời lượng</th>
              <th className="px-4 py-3 text-center">Trạng thái</th>
              <th className="px-4 py-3 text-center">Phiên bản</th>
              <th className="w-[92px] px-4 py-3 text-center">Thao tác</th>
            </tr>
          </thead>

          <tbody className="divide-y divide-zinc-100">
            {loading ? (
              Array.from({ length: 6 }).map((_, index) => (
                <tr key={index} className="animate-pulse">
                  <td className="px-4 py-4"><div className="mx-auto h-4 w-5 rounded bg-zinc-100" /></td>
                  <td className="px-4 py-4"><div className="h-4 w-56 rounded bg-zinc-100" /></td>
                  <td className="px-4 py-4"><div className="h-4 w-32 rounded bg-zinc-100" /></td>
                  <td className="px-4 py-4"><div className="mx-auto h-4 w-16 rounded bg-zinc-100" /></td>
                  <td className="px-4 py-4"><div className="mx-auto h-6 w-20 rounded-full bg-zinc-100" /></td>
                  <td className="px-4 py-4"><div className="mx-auto h-4 w-8 rounded bg-zinc-100" /></td>
                  <td className="px-4 py-4"><div className="mx-auto h-8 w-8 rounded bg-zinc-100" /></td>
                </tr>
              ))
            ) : items.length === 0 ? (
              <tr>
                <td colSpan={7} className="px-4 py-14 text-center text-zinc-500">
                  Chưa có bài giảng phù hợp với bộ lọc hiện tại.
                </td>
              </tr>
            ) : (
              items.map((lesson, index) => (
                <tr key={lesson.id} className="transition hover:bg-zinc-50/70">
                  <td className="px-4 py-3 text-center text-zinc-500">
                    {(page - 1) * PAGE_SIZE + index + 1}
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex items-start gap-2">
                      {lesson.isFeatured ? (
                        <Star size={15} className="mt-0.5 shrink-0 fill-amber-400 text-amber-400" />
                      ) : null}
                      <div className="min-w-0">
                        <Link
                          href={`/bai-giang/${lesson.id}`}
                          className="font-semibold text-zinc-900 hover:underline"
                        >
                          {lesson.titleVi}
                        </Link>
                        <p className="mt-0.5 truncate text-[12px] text-zinc-500">
                          /{lesson.slug}
                        </p>
                      </div>
                    </div>
                  </td>
                  <td className="px-4 py-3 text-zinc-600">
                    <div>{lesson.hskCode ?? `HSK #${lesson.hskLevelId}`}</div>
                    <div className="mt-0.5 text-[12px] text-zinc-400">
                      {lesson.courseTitleVi ?? "Chưa gán khóa học"}
                      {lesson.chapterTitleVi ? ` · ${lesson.chapterTitleVi}` : ""}
                    </div>
                  </td>
                  <td className="px-4 py-3 text-center text-zinc-600">
                    <span className="inline-flex items-center gap-1.5">
                      <Clock size={14} />
                      {lesson.estimatedMinutes} phút
                    </span>
                  </td>
                  <td className="px-4 py-3 text-center">
                    <span
                      className={`inline-flex rounded-full px-2.5 py-1 text-[11px] font-medium ring-1 ring-inset ${statusClass(lesson.status)}`}
                    >
                      {getContentStatusLabel(lesson.status)}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-center font-medium text-zinc-600">
                    v{lesson.version}
                  </td>
                  <td className="px-4 py-3 text-center">
                    <Link href={`/bai-giang/${lesson.id}`}>
                      <Button variant="ghost" size="icon" className="h-8 w-8" aria-label={`Sửa ${lesson.titleVi}`}>
                        <Pencil size={15} />
                      </Button>
                    </Link>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      <div className="flex flex-col gap-3 border-t border-zinc-200 px-4 py-3 sm:flex-row sm:items-center sm:justify-between">
        <p className="text-[12px] text-zinc-500">
          Tổng {totalCount} bài giảng · Trang {page}/{totalPages}
        </p>
        <div className="flex gap-2">
          <Button
            variant="outline"
            className="h-8 text-[12px]"
            disabled={page <= 1 || loading}
            onClick={() => setPage((value) => Math.max(1, value - 1))}
          >
            Trang trước
          </Button>
          <Button
            variant="outline"
            className="h-8 text-[12px]"
            disabled={page >= totalPages || loading}
            onClick={() => setPage((value) => Math.min(totalPages, value + 1))}
          >
            Trang sau
          </Button>
        </div>
      </div>
    </section>
  );
}
