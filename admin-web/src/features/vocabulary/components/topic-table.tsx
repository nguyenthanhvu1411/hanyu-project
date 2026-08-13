"use client";

import { Archive, FolderKanban, Plus, RotateCcw, Search, Send } from "lucide-react";
import Link from "next/link";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";

import { DataTable } from "@/components/common/data-table/data-table";
import {
  ActionButton,
  DataTableActions,
} from "@/components/common/data-table/data-table-actions";
import { ErrorState } from "@/components/common/error-state";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";
import { ContentStatus, getContentStatusLabel } from "@/lib/constants/content-status";
import type { DataTableColumn } from "@/types/table.types";

export interface TopicDto {
  id: number;
  slug: string;
  nameVi: string;
  descriptionVi: string | null;
  sortOrder: number;
  status: ContentStatus;
  createdAt: string;
  updatedAt: string;
}

function statusVariant(status: ContentStatus): "default" | "primary" | "success" | "warning" | "info" {
  switch (status) {
    case ContentStatus.Published:
      return "success";
    case ContentStatus.Approved:
      return "info";
    case ContentStatus.Review:
      return "warning";
    case ContentStatus.Archived:
      return "default";
    default:
      return "primary";
  }
}

const STATUS_OPTIONS = [
  { value: "", label: "Tất cả trạng thái" },
  { value: String(ContentStatus.Draft), label: "Bản nháp" },
  { value: String(ContentStatus.Review), label: "Chờ duyệt" },
  { value: String(ContentStatus.Approved), label: "Đã duyệt" },
  { value: String(ContentStatus.Published), label: "Đã xuất bản" },
  { value: String(ContentStatus.Archived), label: "Đã lưu trữ" },
];

export function TopicTable() {
  const router = useRouter();
  const [items, setItems] = useState<TopicDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [notice, setNotice] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setItems(await apiClient<TopicDto[]>(API_ENDPOINTS.VOCABULARY.TOPICS));
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : "Không thể tải danh sách chủ đề.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  const filtered = useMemo(() => {
    const keyword = search.trim().toLowerCase();
    const statusValue = status === "" ? null : Number(status);

    return items
      .filter((item) => {
        const matchesKeyword =
          !keyword ||
          item.nameVi.toLowerCase().includes(keyword) ||
          item.slug.toLowerCase().includes(keyword) ||
          item.descriptionVi?.toLowerCase().includes(keyword);

        return matchesKeyword && (statusValue === null || item.status === statusValue);
      })
      .sort((a, b) => a.sortOrder - b.sortOrder || a.nameVi.localeCompare(b.nameVi, "vi"));
  }, [items, search, status]);

  useEffect(() => {
    const totalPages = Math.max(1, Math.ceil(filtered.length / pageSize));
    if (page > totalPages) setPage(totalPages);
  }, [filtered.length, page, pageSize]);

  const pagedItems = useMemo(
    () => filtered.slice((page - 1) * pageSize, page * pageSize),
    [filtered, page, pageSize],
  );

  async function runAction(
    path: string,
    successMessage: string,
    method: "POST" | "DELETE" = "POST",
  ) {
    setBusy(true);
    setError(null);
    setNotice(null);
    try {
      await apiClient(path, { method });
      setNotice(successMessage);
      await load();
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : "Thao tác không thành công.");
    } finally {
      setBusy(false);
    }
  }

  const columns: DataTableColumn<TopicDto>[] = [
    {
      id: "topic",
      header: "Chủ đề",
      cell: (item) => (
        <Link
          href={`/chu-de-tu-vung/${item.id}`}
          className="flex min-w-0 items-center gap-3 hover:opacity-80"
        >
          <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-[7px] bg-[#fff0ee] text-[#ef241c]">
            <FolderKanban size={16} />
          </div>
          <div className="min-w-0">
            <div className="truncate text-[13px] font-semibold text-[#333]">{item.nameVi}</div>
            <div className="mt-0.5 truncate text-[12px] text-[#888]">/{item.slug}</div>
          </div>
        </Link>
      ),
    },
    {
      id: "description",
      header: "Mô tả",
      cell: (item) => (
        <span className="line-clamp-2 text-[13px] leading-5 text-[#555]">
          {item.descriptionVi || "—"}
        </span>
      ),
    },
    {
      id: "sortOrder",
      header: "Thứ tự",
      width: "90px",
      align: "center",
      accessor: (item) => item.sortOrder,
    },
    {
      id: "status",
      header: "Trạng thái",
      width: "140px",
      align: "center",
      cell: (item) => (
        <Badge variant={statusVariant(item.status)} className="px-2.5 py-1 text-[12px]">
          {getContentStatusLabel(item.status)}
        </Badge>
      ),
    },
    {
      id: "actions",
      header: "Thao tác",
      width: "90px",
      align: "center",
      cell: (item) => (
        <DataTableActions
          onView={() => router.push(`/chu-de-tu-vung/${item.id}`)}
          onEdit={
            item.status === ContentStatus.Archived
              ? undefined
              : () => router.push(`/chu-de-tu-vung/${item.id}/chinh-sua`)
          }
          onDelete={() => {
            if (window.confirm(`Xóa chủ đề “${item.nameVi}”?`)) {
              void runAction(
                `${API_ENDPOINTS.VOCABULARY.TOPICS}/${item.id}`,
                "Đã xóa chủ đề.",
                "DELETE",
              );
            }
          }}
          customActions={
            <>
              {(item.status === ContentStatus.Draft || item.status === ContentStatus.Approved) && (
                <ActionButton
                  icon={<Send size={14} />}
                  onClick={() =>
                    void runAction(
                      `${API_ENDPOINTS.VOCABULARY.TOPICS}/${item.id}/publish`,
                      "Đã xuất bản chủ đề.",
                    )
                  }
                >
                  Xuất bản
                </ActionButton>
              )}

              {item.status === ContentStatus.Published && (
                <ActionButton
                  icon={<Archive size={14} />}
                  onClick={() =>
                    void runAction(
                      `${API_ENDPOINTS.VOCABULARY.TOPICS}/${item.id}/archive`,
                      "Đã lưu trữ chủ đề.",
                    )
                  }
                >
                  Lưu trữ
                </ActionButton>
              )}

              {item.status === ContentStatus.Archived && (
                <ActionButton
                  icon={<RotateCcw size={14} />}
                  onClick={() =>
                    void runAction(
                      `${API_ENDPOINTS.VOCABULARY.TOPICS}/${item.id}/restore`,
                      "Đã khôi phục chủ đề về bản nháp.",
                    )
                  }
                >
                  Khôi phục
                </ActionButton>
              )}
            </>
          }
        />
      ),
    },
  ];

  if (error && items.length === 0) {
    return (
      <ErrorState
        title="Không thể tải danh sách chủ đề"
        description={error}
        onRetry={() => void load()}
      />
    );
  }

  return (
    <div className="space-y-4">
      {notice && (
        <div className="rounded-[8px] border border-[#cfe7d8] bg-[#f0faf4] px-4 py-3 text-[13px] text-[#2d7048]">
          {notice}
        </div>
      )}
      {error && (
        <div className="rounded-[8px] border border-[#f0cfcb] bg-[#fff5f4] px-4 py-3 text-[13px] text-[#b9433d]">
          {error}
        </div>
      )}

      <Card>
        <CardContent className="flex flex-col gap-3 p-4 lg:flex-row lg:items-center lg:justify-between">
          <div className="flex flex-1 flex-col gap-3 sm:flex-row">
            <label className="relative block w-full max-w-[460px]">
              <Search
                size={16}
                className="pointer-events-none absolute left-3 top-1/2 z-10 -translate-y-1/2 text-[#888]"
              />
              <Input
                value={search}
                onChange={(event) => {
                  setSearch(event.target.value);
                  setPage(1);
                }}
                placeholder="Tìm theo tên chủ đề, slug, mô tả..."
                className="h-10 pl-10 pr-3 text-[14px]"
              />
            </label>

            <div className="w-full sm:w-[210px]">
              <Select
                value={status}
                onValueChange={(value) => {
                  setStatus(value);
                  setPage(1);
                }}
                options={STATUS_OPTIONS}
                placeholder="Tất cả trạng thái"
              />
            </div>
          </div>

          <Button
            type="button"
            variant="primary"
            size="md"
            disabled={busy}
            onClick={() => router.push("/chu-de-tu-vung/them-moi")}
            className="gap-2"
          >
            <Plus size={16} />
            Thêm chủ đề
          </Button>
        </CardContent>
      </Card>

      <DataTable
        data={pagedItems}
        columns={columns}
        rowKey={(item) => item.id}
        loading={loading}
        selectable={false}
        page={page}
        pageSize={pageSize}
        totalItems={filtered.length}
        totalPages={Math.max(1, Math.ceil(filtered.length / pageSize))}
        onPageChange={setPage}
        onPageSizeChange={(value) => {
          setPageSize(value);
          setPage(1);
        }}
      />
    </div>
  );
}
