"use client";

import {
  Archive,
  FolderKanban,
  Pencil,
  RotateCcw,
  Search,
  Send,
  Trash2,
} from "lucide-react";
import { useCallback, useEffect, useMemo, useState } from "react";

import { DataTable } from "@/components/common/data-table/data-table";
import { DataTableActions } from "@/components/common/data-table/data-table-actions";
import { ErrorState } from "@/components/common/error-state";
import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";
import {
  ContentStatus,
  getContentStatusLabel,
} from "@/lib/constants/content-status";
import type { DataTableColumn } from "@/types/table.types";

import { TopicForm, type TopicFormValues } from "./topic-form";

interface TopicDto {
  id: number;
  slug: string;
  nameVi: string;
  descriptionVi: string | null;
  sortOrder: number;
  status: ContentStatus;
  createdAt: string;
  updatedAt: string;
}

function statusClassName(status: ContentStatus) {
  switch (status) {
    case ContentStatus.Published:
      return "bg-[#eaf7ef] text-[#217a46] border-[#cce8d7]";
    case ContentStatus.Approved:
      return "bg-[#eef4ff] text-[#3568b8] border-[#d6e3fb]";
    case ContentStatus.Review:
      return "bg-[#fff8e6] text-[#9b6a12] border-[#f2dfad]";
    case ContentStatus.Archived:
      return "bg-[#f1f1f1] text-[#777] border-[#dddddd]";
    default:
      return "bg-[#fff1ef] text-[#c93b33] border-[#f3d1cd]";
  }
}

export function TopicTable() {
  const [items, setItems] = useState<TopicDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [notice, setNotice] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState<string>("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<TopicDto | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const data = await apiClient<TopicDto[]>(API_ENDPOINTS.VOCABULARY.TOPICS);
      setItems(data);
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

        const matchesStatus = statusValue === null || item.status === statusValue;
        return matchesKeyword && matchesStatus;
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

  async function runAction(path: string, successMessage: string, method: "POST" | "DELETE" = "POST") {
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
        <div className="flex min-w-0 items-center gap-2.5">
          <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-[7px] bg-[#fff0ee] text-[#ef241c]">
            <FolderKanban size={15} />
          </div>
          <div className="min-w-0">
            <div className="truncate text-[11px] font-semibold text-[#333]">{item.nameVi}</div>
            <div className="truncate text-[10px] text-[#999]">/{item.slug}</div>
          </div>
        </div>
      ),
    },
    {
      id: "description",
      header: "Mô tả",
      cell: (item) => (
        <span className="line-clamp-2 text-[11px] leading-5 text-[#666]">
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
        <span
          className={`inline-flex rounded-full border px-2.5 py-1 text-[10px] font-medium ${statusClassName(item.status)}`}
        >
          {getContentStatusLabel(item.status)}
        </span>
      ),
    },
    {
      id: "actions",
      header: "Thao tác",
      width: "90px",
      align: "center",
      cell: (item) => (
        <DataTableActions
          onEdit={
            item.status === ContentStatus.Archived
              ? undefined
              : () => {
                  setEditing(item);
                  setFormOpen(true);
                  setNotice(null);
                }
          }
          onDelete={() => {
            if (!window.confirm(`Xóa chủ đề “${item.nameVi}”?`)) return;
            void runAction(
              `${API_ENDPOINTS.VOCABULARY.TOPICS}/${item.id}`,
              "Đã xóa chủ đề.",
              "DELETE",
            );
          }}
          customActions={
            <>
              {(item.status === ContentStatus.Draft || item.status === ContentStatus.Approved) && (
                <button
                  type="button"
                  disabled={busy}
                  onClick={() =>
                    void runAction(
                      `${API_ENDPOINTS.VOCABULARY.TOPICS}/${item.id}/publish`,
                      "Đã xuất bản chủ đề.",
                    )
                  }
                  className="flex h-8 w-full items-center gap-2 rounded-[6px] px-2 text-[11px] text-[#555] hover:bg-[#f7f7f7] disabled:opacity-50"
                >
                  <Send size={14} /> Xuất bản
                </button>
              )}

              {item.status === ContentStatus.Published && (
                <button
                  type="button"
                  disabled={busy}
                  onClick={() =>
                    void runAction(
                      `${API_ENDPOINTS.VOCABULARY.TOPICS}/${item.id}/archive`,
                      "Đã lưu trữ chủ đề.",
                    )
                  }
                  className="flex h-8 w-full items-center gap-2 rounded-[6px] px-2 text-[11px] text-[#555] hover:bg-[#f7f7f7] disabled:opacity-50"
                >
                  <Archive size={14} /> Lưu trữ
                </button>
              )}

              {item.status === ContentStatus.Archived && (
                <button
                  type="button"
                  disabled={busy}
                  onClick={() =>
                    void runAction(
                      `${API_ENDPOINTS.VOCABULARY.TOPICS}/${item.id}/restore`,
                      "Đã khôi phục chủ đề về bản nháp.",
                    )
                  }
                  className="flex h-8 w-full items-center gap-2 rounded-[6px] px-2 text-[11px] text-[#555] hover:bg-[#f7f7f7] disabled:opacity-50"
                >
                  <RotateCcw size={14} /> Khôi phục
                </button>
              )}
            </>
          }
        />
      ),
    },
  ];

  if (error && items.length === 0 && !formOpen) {
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
        <div className="rounded-[8px] border border-[#cfe7d8] bg-[#f0faf4] px-3 py-2 text-[11px] text-[#2d7048]">
          {notice}
        </div>
      )}
      {error && (
        <div className="rounded-[8px] border border-[#f0cfcb] bg-[#fff5f4] px-3 py-2 text-[11px] text-[#b9433d]">
          {error}
        </div>
      )}

      {formOpen && (
        <TopicForm
          submitting={busy}
          initialValues={
            editing
              ? {
                  slug: editing.slug,
                  nameVi: editing.nameVi,
                  descriptionVi: editing.descriptionVi ?? "",
                  sortOrder: editing.sortOrder,
                }
              : undefined
          }
          onCancel={() => {
            if (busy) return;
            setFormOpen(false);
            setEditing(null);
          }}
          onSubmit={async (values: TopicFormValues) => {
            setBusy(true);
            setError(null);
            setNotice(null);
            try {
              const body = {
                slug: values.slug,
                nameVi: values.nameVi,
                descriptionVi: values.descriptionVi || null,
                sortOrder: values.sortOrder,
              };

              if (editing) {
                await apiClient(`${API_ENDPOINTS.VOCABULARY.TOPICS}/${editing.id}`, {
                  method: "PUT",
                  body,
                });
                setNotice("Đã cập nhật chủ đề.");
              } else {
                await apiClient(API_ENDPOINTS.VOCABULARY.TOPICS, {
                  method: "POST",
                  body,
                });
                setNotice("Đã tạo chủ đề mới.");
              }

              setFormOpen(false);
              setEditing(null);
              await load();
            } catch (exception) {
              setError(exception instanceof Error ? exception.message : "Không thể lưu chủ đề.");
            } finally {
              setBusy(false);
            }
          }}
        />
      )}

      <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
        <div className="flex flex-col gap-3 border-b border-[#eee9e2] p-3 lg:flex-row lg:items-center lg:justify-between">
          <div className="flex flex-1 flex-col gap-2 sm:flex-row">
            <label className="relative block w-full max-w-[420px]">
              <Search
                size={14}
                className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-[#999]"
              />
              <input
                value={search}
                onChange={(event) => {
                  setSearch(event.target.value);
                  setPage(1);
                }}
                placeholder="Tìm theo tên chủ đề, slug, mô tả..."
                className="h-[36px] w-full rounded-[7px] border border-[#dfdbd4] bg-white pl-9 pr-3 text-[11px] outline-none focus:border-[#ef5b55]"
              />
            </label>

            <select
              value={status}
              onChange={(event) => {
                setStatus(event.target.value);
                setPage(1);
              }}
              className="h-[36px] rounded-[7px] border border-[#dfdbd4] bg-white px-3 text-[11px] text-[#555] outline-none focus:border-[#ef5b55]"
            >
              <option value="">Tất cả trạng thái</option>
              <option value={ContentStatus.Draft}>Bản nháp</option>
              <option value={ContentStatus.Review}>Chờ duyệt</option>
              <option value={ContentStatus.Approved}>Đã duyệt</option>
              <option value={ContentStatus.Published}>Đã xuất bản</option>
              <option value={ContentStatus.Archived}>Đã lưu trữ</option>
            </select>
          </div>

          <button
            type="button"
            disabled={busy}
            onClick={() => {
              setEditing(null);
              setFormOpen((value) => !value);
              setNotice(null);
            }}
            className="inline-flex h-[36px] items-center justify-center gap-2 rounded-[7px] bg-[#ef241c] px-4 text-[11px] font-semibold text-white hover:bg-[#d91f18] disabled:opacity-50"
          >
            <Pencil size={14} />
            {formOpen && !editing ? "Đóng form" : "Thêm chủ đề"}
          </button>
        </div>

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
    </div>
  );
}
