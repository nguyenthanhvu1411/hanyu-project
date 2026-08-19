"use client";

import { Pencil, Search, Tags } from "lucide-react";
import { useCallback, useEffect, useMemo, useState } from "react";

import { DataTable } from "@/components/common/data-table/data-table";
import { DataTableActions } from "@/components/common/data-table/data-table-actions";
import { ErrorState } from "@/components/common/error-state";
import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";
import type { DataTableColumn } from "@/types/table.types";

import {
  PartOfSpeechForm,
  type PartOfSpeechFormValues,
} from "./part-of-speech-form";

interface PartOfSpeechDto {
  id: number;
  code: string;
  nameVi: string;
  nameEn: string | null;
  createdAt: string;
  updatedAt: string;
}

export function PartOfSpeechTable() {
  const [items, setItems] = useState<PartOfSpeechDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [notice, setNotice] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<PartOfSpeechDto | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const data = await apiClient<PartOfSpeechDto[]>(API_ENDPOINTS.VOCABULARY.PARTS_OF_SPEECH);
      setItems(data);
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : "Không thể tải danh sách từ loại.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  const filtered = useMemo(() => {
    const keyword = search.trim().toLowerCase();
    return items
      .filter((item) =>
        !keyword ||
        item.code.toLowerCase().includes(keyword) ||
        item.nameVi.toLowerCase().includes(keyword) ||
        item.nameEn?.toLowerCase().includes(keyword),
      )
      .sort((a, b) => a.nameVi.localeCompare(b.nameVi, "vi"));
  }, [items, search]);

  useEffect(() => {
    const totalPages = Math.max(1, Math.ceil(filtered.length / pageSize));
    if (page > totalPages) setPage(totalPages);
  }, [filtered.length, page, pageSize]);

  const pagedItems = useMemo(
    () => filtered.slice((page - 1) * pageSize, page * pageSize),
    [filtered, page, pageSize],
  );

  const columns: DataTableColumn<PartOfSpeechDto>[] = [
    {
      id: "partOfSpeech",
      header: "Từ loại",
      cell: (item) => (
        <div className="flex items-center gap-2.5">
          <div className="flex h-8 w-8 items-center justify-center rounded-[7px] bg-[#fff0ee] text-[#ef241c]">
            <Tags size={15} />
          </div>
          <div>
            <div className="text-[11px] font-semibold text-[#333]">{item.nameVi}</div>
            <div className="text-[10px] text-[#999]">{item.code}</div>
          </div>
        </div>
      ),
    },
    {
      id: "nameEn",
      header: "Tên tiếng Anh",
      accessor: (item) => item.nameEn || "—",
    },
    {
      id: "updatedAt",
      header: "Cập nhật",
      width: "160px",
      cell: (item) => (
        <span className="text-[11px] text-[#666]">
          {new Date(item.updatedAt).toLocaleString("vi-VN")}
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
          onEdit={() => {
            setEditing(item);
            setFormOpen(true);
            setNotice(null);
          }}
          onDelete={() => {
            if (!window.confirm(`Xóa từ loại “${item.nameVi}”?`)) return;
            void (async () => {
              setBusy(true);
              setError(null);
              setNotice(null);
              try {
                await apiClient(API_ENDPOINTS.VOCABULARY.PART_OF_SPEECH(item.id), {
                  method: "DELETE",
                });
                setNotice("Đã xóa từ loại.");
                await load();
              } catch (exception) {
                setError(exception instanceof Error ? exception.message : "Không thể xóa từ loại.");
              } finally {
                setBusy(false);
              }
            })();
          }}
        />
      ),
    },
  ];

  if (error && items.length === 0 && !formOpen) {
    return (
      <ErrorState
        title="Không thể tải từ loại"
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
        <PartOfSpeechForm
          submitting={busy}
          initialValues={
            editing
              ? {
                  code: editing.code,
                  nameVi: editing.nameVi,
                  nameEn: editing.nameEn ?? "",
                }
              : undefined
          }
          onCancel={() => {
            if (busy) return;
            setFormOpen(false);
            setEditing(null);
          }}
          onSubmit={async (values: PartOfSpeechFormValues) => {
            setBusy(true);
            setError(null);
            setNotice(null);
            try {
              const body = {
                code: values.code,
                nameVi: values.nameVi,
                nameEn: values.nameEn || null,
              };

              if (editing) {
                await apiClient(API_ENDPOINTS.VOCABULARY.PART_OF_SPEECH(editing.id), {
                  method: "PUT",
                  body,
                });
                setNotice("Đã cập nhật từ loại.");
              } else {
                await apiClient(API_ENDPOINTS.VOCABULARY.PARTS_OF_SPEECH, {
                  method: "POST",
                  body,
                });
                setNotice("Đã tạo từ loại mới.");
              }

              setFormOpen(false);
              setEditing(null);
              await load();
            } catch (exception) {
              setError(exception instanceof Error ? exception.message : "Không thể lưu từ loại.");
            } finally {
              setBusy(false);
            }
          }}
        />
      )}

      <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
        <div className="flex flex-col gap-3 border-b border-[#eee9e2] p-3 sm:flex-row sm:items-center sm:justify-between">
          <label className="relative block w-full max-w-[420px]">
            <Search size={14} className="absolute left-3 top-1/2 -translate-y-1/2 text-[#999]" />
            <input
              value={search}
              onChange={(event) => {
                setSearch(event.target.value);
                setPage(1);
              }}
              placeholder="Tìm mã, tên tiếng Việt hoặc tiếng Anh..."
              className="h-[36px] w-full rounded-[7px] border border-[#dfdbd4] pl-9 pr-3 text-[11px] outline-none focus:border-[#ef5b55]"
            />
          </label>

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
            {formOpen && !editing ? "Đóng form" : "Thêm từ loại"}
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
