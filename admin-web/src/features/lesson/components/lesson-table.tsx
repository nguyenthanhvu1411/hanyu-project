"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { Clock, RefreshCw, Star, Trash2 } from "lucide-react";
import { DataTable } from "@/components/common/data-table/data-table";
import { ActionButton, DataTableActions } from "@/components/common/data-table/data-table-actions";
import { DataTableFilter } from "@/components/common/data-table/data-table-filter";
import { DataTableSearch } from "@/components/common/data-table/data-table-search";
import { DataTableToolbar } from "@/components/common/data-table/data-table-toolbar";
import { ErrorState } from "@/components/common/error-state";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { appToast } from "@/components/ui/toast";
import { PERMISSIONS } from "@/constants/permission.constants";
import { ContentStatus, getContentStatusLabel } from "@/lib/constants/content-status";
import { normalizeApiError } from "@/lib/api/api-error";
import { PermissionGuard } from "@/security/permission-guard";
import type { DataTableColumn } from "@/types/table.types";
import { lessonApi } from "../api/lesson.api";
import type { AdminLessonListItem } from "../types/lesson.types";

interface LessonPageData {
  items: AdminLessonListItem[];
  totalCount: number;
  totalPages: number;
}

const STATUS_OPTIONS = [
  { label: "Bản nháp", value: String(ContentStatus.Draft) },
  { label: "Chờ duyệt", value: String(ContentStatus.Review) },
  { label: "Đã duyệt", value: String(ContentStatus.Approved) },
  { label: "Đã xuất bản", value: String(ContentStatus.Published) },
  { label: "Đã lưu trữ", value: String(ContentStatus.Archived) },
];

function statusVariant(status: ContentStatus): "default" | "success" | "warning" | "info" {
  switch (status) {
    case ContentStatus.Published:
      return "success";
    case ContentStatus.Review:
      return "warning";
    case ContentStatus.Approved:
      return "info";
    default:
      return "default";
  }
}

export function LessonTable() {
  const router = useRouter();
  const [data, setData] = useState<LessonPageData>({ items: [], totalCount: 0, totalPages: 1 });
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);
  const [deletingId, setDeletingId] = useState<number | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const result = await lessonApi.list({
        search: search.trim() || undefined,
        status: status === "" ? undefined : (Number(status) as ContentStatus),
        page,
        pageSize,
        sortBy: "updatedAt",
        sortDescending: true,
      });
      const total = result.total ?? result.totalCount ?? 0;
      const resolvedPageSize = result.pageSize ?? pageSize;

      setData({
        items: result.items ?? [],
        totalCount: total,
        totalPages: result.totalPages ?? Math.max(1, Math.ceil(total / Math.max(1, resolvedPageSize))),
      });
    } catch (caught) {
      setError(caught instanceof Error ? caught : new Error("Không thể tải danh sách bài giảng."));
      setData({ items: [], totalCount: 0, totalPages: 1 });
    } finally {
      setLoading(false);
    }
  }, [page, pageSize, search, status]);

  useEffect(() => {
    const timeout = window.setTimeout(() => void load(), search ? 250 : 0);
    return () => window.clearTimeout(timeout);
  }, [load, search]);

  const deleteLesson = useCallback(
    async (item: AdminLessonListItem) => {
      if (deletingId || !window.confirm(`Xóa bài giảng “${item.titleVi}”?`)) return;

      setDeletingId(item.id);
      try {
        const detail = await lessonApi.getById(item.id);
        await lessonApi.delete(item.id, { version: detail.version });
        appToast.success("Đã xóa bài giảng.");
        await load();
      } catch (caught) {
        appToast.error("Không thể xóa bài giảng", normalizeApiError(caught).message);
      } finally {
        setDeletingId(null);
      }
    },
    [deletingId, load],
  );

  const columns = useMemo<DataTableColumn<AdminLessonListItem>[]>(
    () => [
      {
        id: "lesson",
        header: "Bài giảng",
        cell: (item) => (
          <div className="min-w-0">
            <div className="flex items-center gap-2">
              {item.isFeatured ? <Star size={13} className="fill-current text-[#d99716]" /> : null}
              <span className="truncate text-[12px] font-semibold text-[#333]">{item.titleVi}</span>
            </div>
            <div className="mt-0.5 flex flex-wrap gap-x-2 text-[10px] text-[#8a8a8a]">
              <span>/{item.slug}</span>
              <span title={`PublicId: ${item.publicId}`}>PublicId: {item.publicId.slice(0, 8)}…</span>
            </div>
          </div>
        ),
      },
      {
        id: "classification",
        header: "Phân loại",
        cell: (item) => (
          <div className="text-[11px] text-[#555]">
            <div>{item.hskCode ?? `HSK #${item.hskLevelId}`}</div>
            <div className="mt-0.5 text-[10px] text-[#929292]">
              {item.courseTitleVi ?? "Chưa gán khóa học"}
              {item.chapterTitleVi ? ` · ${item.chapterTitleVi}` : ""}
            </div>
          </div>
        ),
      },
      {
        id: "duration",
        header: "Thời lượng",
        align: "center",
        width: "120px",
        cell: (item) => (
          <span className="inline-flex items-center gap-1.5 text-[11px] text-[#666]">
            <Clock size={13} />
            {item.estimatedMinutes} phút
          </span>
        ),
      },
      {
        id: "status",
        header: "Trạng thái",
        align: "center",
        width: "140px",
        cell: (item) => (
          <Badge variant={statusVariant(item.status)}>{getContentStatusLabel(item.status)}</Badge>
        ),
      },
      {
        id: "actions",
        header: "Thao tác",
        align: "center",
        width: "90px",
        cell: (item) => (
          <DataTableActions
            onView={() => router.push(`/bai-giang/${item.id}`)}
            onEdit={() => router.push(`/bai-giang/${item.id}/chinh-sua`)}
            customActions={
              <PermissionGuard permission={PERMISSIONS.LESSONS.DELETE} fallback={null}>
                <ActionButton
                  danger
                  icon={<Trash2 size={14} />}
                  onClick={() => void deleteLesson(item)}
                >
                  {deletingId === item.id ? "Đang xóa..." : "Xóa"}
                </ActionButton>
              </PermissionGuard>
            }
          />
        ),
      },
    ],
    [deleteLesson, deletingId, router],
  );

  if (error && !loading) {
    return (
      <ErrorState
        title="Không thể tải bài giảng"
        description={error.message}
        onRetry={() => void load()}
      />
    );
  }

  return (
    <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
      <DataTableToolbar
        left={
          <>
            <DataTableSearch
              value={search}
              onChange={(value) => {
                setSearch(value);
                setPage(1);
              }}
              placeholder="Tìm theo tên, slug, khóa học..."
            />
            <DataTableFilter
              value={status}
              onChange={(value) => {
                setStatus(value);
                setPage(1);
              }}
              options={STATUS_OPTIONS}
              placeholder="Tất cả trạng thái"
            />
          </>
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
