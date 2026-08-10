"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { RefreshCw } from "lucide-react";

import { DataTable } from "@/components/common/data-table/data-table";
import { DataTableSearch } from "@/components/common/data-table/data-table-search";
import { DataTableToolbar } from "@/components/common/data-table/data-table-toolbar";
import { ErrorState } from "@/components/common/error-state";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import type { DataTableColumn } from "@/types/table.types";

import { curriculumApi } from "../api/curriculum.api";
import type { CourseChapter } from "../types/curriculum.types";

interface CourseChapterTableProps {
  courseId: number;
}

export function CourseChapterTable({ courseId }: CourseChapterTableProps) {
  const [allItems, setAllItems] = useState<CourseChapter[]>([]);
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  const load = useCallback(async () => {
    if (!Number.isSafeInteger(courseId) || courseId <= 0) {
      setError(new Error("CourseId không hợp lệ."));
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);

    try {
      const result = await curriculumApi.chapters(courseId, false);
      setAllItems(result ?? []);
    } catch (caught) {
      setAllItems([]);
      setError(caught instanceof Error ? caught : new Error("Không thể tải chương học."));
    } finally {
      setLoading(false);
    }
  }, [courseId]);

  useEffect(() => {
    void load();
  }, [load]);

  const filtered = useMemo(() => {
    const keyword = search.trim().toLowerCase();
    if (!keyword) return allItems;

    return allItems.filter(
      (item) =>
        item.titleVi.toLowerCase().includes(keyword) ||
        item.publicId.toLowerCase().includes(keyword),
    );
  }, [allItems, search]);

  const totalPages = Math.max(1, Math.ceil(filtered.length / Math.max(1, pageSize)));
  const visibleItems = filtered.slice((page - 1) * pageSize, page * pageSize);

  const columns = useMemo<DataTableColumn<CourseChapter>[]>(
    () => [
      {
        id: "chapter",
        header: "Chương học",
        cell: (item) => (
          <div className="min-w-0">
            <div className="truncate text-[12px] font-semibold text-[#333]">{item.titleVi}</div>
            <div className="mt-0.5 text-[10px] text-[#8a8a8a]" title={item.publicId}>
              PublicId: {item.publicId}
            </div>
          </div>
        ),
      },
      {
        id: "sortOrder",
        header: "Thứ tự",
        align: "center",
        width: "100px",
        accessor: (item) => item.sortOrder,
      },
      {
        id: "lessonCount",
        header: "Bài giảng",
        align: "center",
        width: "110px",
        cell: (item) => <Badge variant="info">{item.lessonCount}</Badge>,
      },
      {
        id: "active",
        header: "Trạng thái",
        align: "center",
        width: "130px",
        cell: (item) => (
          <Badge variant={item.isActive ? "success" : "default"}>
            {item.isActive ? "Hoạt động" : "Ngừng"}
          </Badge>
        ),
      },
    ],
    [],
  );

  if (error && !loading) {
    return (
      <ErrorState
        title="Không thể tải chương học"
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
            placeholder="Tìm tên hoặc PublicId chương..."
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
        data={visibleItems}
        columns={columns}
        rowKey={(item) => item.id}
        loading={loading}
        selectable={false}
        page={page}
        pageSize={pageSize}
        totalItems={filtered.length}
        totalPages={totalPages}
        onPageChange={setPage}
        onPageSizeChange={(value) => {
          setPageSize(value);
          setPage(1);
        }}
      />
    </div>
  );
}
