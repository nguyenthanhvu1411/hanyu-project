"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { BookOpen, RefreshCw } from "lucide-react";

import { DataTable } from "@/components/common/data-table/data-table";
import { DataTableActions } from "@/components/common/data-table/data-table-actions";
import { DataTableSearch } from "@/components/common/data-table/data-table-search";
import { DataTableToolbar } from "@/components/common/data-table/data-table-toolbar";
import { ErrorState } from "@/components/common/error-state";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { getContentStatusLabel } from "@/lib/constants/content-status";
import type { DataTableColumn } from "@/types/table.types";

import { courseApi } from "../api/course.api";
import type { AdminCourseListItem } from "../types/course.types";

interface CoursePageData {
  items: AdminCourseListItem[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

const EMPTY_DATA: CoursePageData = {
  items: [],
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 1,
};

export function CourseTable() {
  const router = useRouter();
  const [data, setData] = useState<CoursePageData>(EMPTY_DATA);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const result = await courseApi.danhSach({
        search: search.trim() || undefined,
        page,
        pageSize,
        sortBy: "sortorder",
        sortDescending: false,
      });

      setData({
        items: result.items ?? [],
        page: result.page ?? page,
        pageSize: result.pageSize ?? pageSize,
        totalCount: result.totalCount ?? 0,
        totalPages:
          result.totalPages ??
          Math.max(1, Math.ceil((result.totalCount ?? 0) / Math.max(1, result.pageSize ?? pageSize))),
      });
    } catch (caught) {
      setError(caught instanceof Error ? caught : new Error("Không thể tải khóa học."));
      setData((current) => ({ ...current, items: [] }));
    } finally {
      setLoading(false);
    }
  }, [page, pageSize, search]);

  useEffect(() => {
    const timeout = window.setTimeout(() => {
      void load();
    }, search ? 250 : 0);

    return () => window.clearTimeout(timeout);
  }, [load, search]);

  const columns = useMemo<DataTableColumn<AdminCourseListItem>[]>(
    () => [
      {
        id: "course",
        header: "Khóa học",
        cell: (item) => (
          <div className="flex min-w-0 items-center gap-3">
            <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-[8px] bg-[#fff0ee] text-[#ef241c]">
              <BookOpen size={16} />
            </div>
            <div className="min-w-0">
              <div className="truncate text-[12px] font-semibold text-[#333]">{item.titleVi}</div>
              <div className="mt-0.5 flex flex-wrap gap-x-2 text-[10px] text-[#8a8a8a]">
                <span>{item.code}</span>
                <span>{item.slug}</span>
                <span title={`PublicId: ${item.publicId}`}>PublicId: {item.publicId.slice(0, 8)}…</span>
              </div>
            </div>
          </div>
        ),
      },
      {
        id: "hsk",
        header: "HSK",
        width: "130px",
        cell: (item) => item.hskCode ?? "—",
      },
      {
        id: "chapters",
        header: "Chương",
        align: "center",
        width: "100px",
        accessor: (item) => item.chapterCount,
      },
      {
        id: "status",
        header: "Trạng thái",
        align: "center",
        width: "140px",
        cell: (item) => <Badge variant="info">{getContentStatusLabel(item.status)}</Badge>,
      },
      {
        id: "active",
        header: "Hoạt động",
        align: "center",
        width: "120px",
        cell: (item) => (
          <Badge variant={item.isActive ? "success" : "default"}>
            {item.isActive ? "Hoạt động" : "Ngừng"}
          </Badge>
        ),
      },
      {
        id: "actions",
        header: "Thao tác",
        align: "center",
        width: "90px",
        cell: (item) => (
          <DataTableActions
            onView={() => router.push(`/khoa-hoc/${item.id}`)}
            onEdit={() => router.push(`/khoa-hoc/${item.id}/chinh-sua`)}
          />
        ),
      },
    ],
    [router],
  );

  if (error && !loading) {
    return (
      <ErrorState
        title="Không thể tải khóa học"
        description={error.message}
        onRetry={() => void load()}
      />
    );
  }

  return (
    <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
      <DataTableToolbar
        left={
          <DataTableSearch
            value={search}
            onChange={(value) => {
              setSearch(value);
              setPage(1);
            }}
            placeholder="Tìm mã, slug, tên khóa học..."
          />
        }
        right={
          <Button variant="outline" className="h-[38px] gap-2 text-[11px]" onClick={() => void load()}>
            <RefreshCw size={14} />
            Làm mới
          </Button>
        }
      />

      <DataTable
        data={data.items}
        columns={columns}
        rowKey={(item) => item.id}
        loading={loading}
        selectable={false}
        page={page}
        pageSize={pageSize}
        totalItems={data.totalCount}
        totalPages={data.totalPages}
        onPageChange={setPage}
        onPageSizeChange={(value) => {
          setPageSize(value);
          setPage(1);
        }}
      />
    </div>
  );
}
