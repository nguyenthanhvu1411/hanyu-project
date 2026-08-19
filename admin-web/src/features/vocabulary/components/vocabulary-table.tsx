"use client";

import {
  Archive,
  CheckCircle2,
  Languages,
  RotateCcw,
  Search,
  Send,
  Upload,
} from "lucide-react";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";

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

interface VocabularyDto {
  id: number;
  hskLevelId: number;
  hskCode: string;
  hskNameVi: string;
  partOfSpeechId: number | null;
  partOfSpeechCode: string | null;
  partOfSpeechNameVi: string | null;
  topicId: number | null;
  topicSlug: string | null;
  topicNameVi: string | null;
  audioAssetId: number | null;
  simplified: string;
  traditional: string | null;
  pinyin: string;
  pinyinNormalized: string;
  primaryMeaningVi: string;
  notesVi: string | null;
  difficulty: number;
  status: ContentStatus;
  version: number;
  publishedAt: string | null;
  createdAt: string;
  updatedAt: string;
}

interface PagedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
  totalPages: number;
}

function statusClassName(status: ContentStatus) {
  switch (status) {
    case ContentStatus.Published:
      return "border-[#cce8d7] bg-[#eaf7ef] text-[#217a46]";
    case ContentStatus.Approved:
      return "border-[#d6e3fb] bg-[#eef4ff] text-[#3568b8]";
    case ContentStatus.Review:
      return "border-[#f2dfad] bg-[#fff8e6] text-[#9b6a12]";
    case ContentStatus.Archived:
      return "border-[#dddddd] bg-[#f1f1f1] text-[#777]";
    default:
      return "border-[#f3d1cd] bg-[#fff1ef] text-[#c93b33]";
  }
}

export function VocabularyTable() {
  const router = useRouter();
  const [data, setData] = useState<PagedResponse<VocabularyDto>>({
    items: [],
    page: 1,
    pageSize: 20,
    total: 0,
    totalPages: 1,
  });
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [notice, setNotice] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("");
  const [difficulty, setDifficulty] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);

  const queryString = useMemo(() => {
    const params = new URLSearchParams();
    params.set("page", String(page));
    params.set("pageSize", String(pageSize));
    params.set("sort", "-updatedAt");
    if (search.trim()) params.set("q", search.trim());
    if (status) params.set("status", status);
    if (difficulty) params.set("difficulty", difficulty);
    return params.toString();
  }, [difficulty, page, pageSize, search, status]);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await apiClient<PagedResponse<VocabularyDto>>(
        `${API_ENDPOINTS.VOCABULARY.ROOT}?${queryString}`,
      );
      setData(result);
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : "Không thể tải danh sách từ vựng.");
    } finally {
      setLoading(false);
    }
  }, [queryString]);

  useEffect(() => {
    void load();
  }, [load]);

  async function workflow(path: string, message: string) {
    setBusy(true);
    setError(null);
    setNotice(null);
    try {
      await apiClient(path, { method: "POST" });
      setNotice(message);
      await load();
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : "Không thể chuyển trạng thái từ vựng.");
    } finally {
      setBusy(false);
    }
  }

  const columns: DataTableColumn<VocabularyDto>[] = [
    {
      id: "word",
      header: "Từ vựng",
      cell: (item) => (
        <div className="flex items-center gap-2.5">
          <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-[8px] bg-[#fff0ee] text-[#ef241c]">
            <Languages size={16} />
          </div>
          <div className="min-w-0">
            <div className="flex items-baseline gap-2">
              <span className="text-[15px] font-semibold text-[#222]">{item.simplified}</span>
              {item.traditional && item.traditional !== item.simplified && (
                <span className="text-[10px] text-[#999]">{item.traditional}</span>
              )}
            </div>
            <div className="text-[10px] text-[#777]">{item.pinyin}</div>
          </div>
        </div>
      ),
    },
    {
      id: "meaning",
      header: "Nghĩa chính",
      cell: (item) => <span className="text-[11px] text-[#555]">{item.primaryMeaningVi}</span>,
    },
    {
      id: "classification",
      header: "Phân loại",
      width: "190px",
      cell: (item) => (
        <div className="space-y-0.5 text-[10px] text-[#666]">
          <div>{item.hskCode} · Độ khó {item.difficulty}</div>
          <div>{item.partOfSpeechNameVi || "Chưa gắn từ loại"}</div>
          <div className="text-[#999]">{item.topicNameVi || "Chưa gắn chủ đề"}</div>
        </div>
      ),
    },
    {
      id: "status",
      header: "Trạng thái",
      width: "130px",
      align: "center",
      cell: (item) => (
        <span className={`inline-flex rounded-full border px-2.5 py-1 text-[10px] font-medium ${statusClassName(item.status)}`}>
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
          onView={() => router.push(`/tu-vung/${item.id}`)}
          onEdit={item.status === ContentStatus.Archived ? undefined : () => router.push(`/tu-vung/${item.id}/chinh-sua`)}
          onDelete={() => {
            if (!window.confirm(`Xóa từ vựng “${item.simplified}”?`)) return;
            void (async () => {
              setBusy(true);
              setError(null);
              try {
                await apiClient(API_ENDPOINTS.VOCABULARY.DETAIL(item.id), { method: "DELETE" });
                setNotice("Đã xóa từ vựng.");
                await load();
              } catch (exception) {
                setError(exception instanceof Error ? exception.message : "Không thể xóa từ vựng.");
              } finally {
                setBusy(false);
              }
            })();
          }}
          customActions={
            <>
              {item.status === ContentStatus.Draft && (
                <button
                  type="button"
                  disabled={busy}
                  onClick={() => void workflow(API_ENDPOINTS.VOCABULARY.SUBMIT_REVIEW(item.id), "Đã gửi từ vựng chờ duyệt.")}
                  className="flex h-8 w-full items-center gap-2 rounded-[6px] px-2 text-[11px] text-[#555] hover:bg-[#f7f7f7] disabled:opacity-50"
                >
                  <Send size={14} /> Gửi duyệt
                </button>
              )}
              {item.status === ContentStatus.Review && (
                <button
                  type="button"
                  disabled={busy}
                  onClick={() => void workflow(API_ENDPOINTS.VOCABULARY.APPROVE(item.id), "Đã duyệt từ vựng.")}
                  className="flex h-8 w-full items-center gap-2 rounded-[6px] px-2 text-[11px] text-[#555] hover:bg-[#f7f7f7] disabled:opacity-50"
                >
                  <CheckCircle2 size={14} /> Duyệt
                </button>
              )}
              {item.status === ContentStatus.Approved && (
                <button
                  type="button"
                  disabled={busy}
                  onClick={() => void workflow(API_ENDPOINTS.VOCABULARY.PUBLISH(item.id), "Đã xuất bản từ vựng.")}
                  className="flex h-8 w-full items-center gap-2 rounded-[6px] px-2 text-[11px] text-[#555] hover:bg-[#f7f7f7] disabled:opacity-50"
                >
                  <Upload size={14} /> Xuất bản
                </button>
              )}
              {item.status === ContentStatus.Published && (
                <button
                  type="button"
                  disabled={busy}
                  onClick={() => void workflow(API_ENDPOINTS.VOCABULARY.ARCHIVE(item.id), "Đã lưu trữ từ vựng.")}
                  className="flex h-8 w-full items-center gap-2 rounded-[6px] px-2 text-[11px] text-[#555] hover:bg-[#f7f7f7] disabled:opacity-50"
                >
                  <Archive size={14} /> Lưu trữ
                </button>
              )}
              {item.status === ContentStatus.Archived && (
                <button
                  type="button"
                  disabled={busy}
                  onClick={() => void workflow(API_ENDPOINTS.VOCABULARY.RESTORE(item.id), "Đã khôi phục từ vựng về bản nháp.")}
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

  if (error && data.items.length === 0) {
    return <ErrorState title="Không thể tải từ vựng" description={error} onRetry={() => void load()} />;
  }

  return (
    <div className="space-y-4">
      {notice && <div className="rounded-[8px] border border-[#cfe7d8] bg-[#f0faf4] px-3 py-2 text-[11px] text-[#2d7048]">{notice}</div>}
      {error && <div className="rounded-[8px] border border-[#f0cfcb] bg-[#fff5f4] px-3 py-2 text-[11px] text-[#b9433d]">{error}</div>}

      <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
        <div className="flex flex-col gap-2 border-b border-[#eee9e2] p-3 lg:flex-row lg:items-center">
          <label className="relative block min-w-0 flex-1 lg:max-w-[460px]">
            <Search size={14} className="absolute left-3 top-1/2 -translate-y-1/2 text-[#999]" />
            <input
              value={search}
              onChange={(event) => {
                setSearch(event.target.value);
                setPage(1);
              }}
              placeholder="Tìm Hán tự, Pinyin hoặc nghĩa..."
              className="h-[36px] w-full rounded-[7px] border border-[#dfdbd4] pl-9 pr-3 text-[11px] outline-none focus:border-[#ef5b55]"
            />
          </label>
          <select
            value={status}
            onChange={(event) => {
              setStatus(event.target.value);
              setPage(1);
            }}
            className="h-[36px] rounded-[7px] border border-[#dfdbd4] bg-white px-3 text-[11px] text-[#555] outline-none"
          >
            <option value="">Tất cả trạng thái</option>
            <option value={ContentStatus.Draft}>Bản nháp</option>
            <option value={ContentStatus.Review}>Chờ duyệt</option>
            <option value={ContentStatus.Approved}>Đã duyệt</option>
            <option value={ContentStatus.Published}>Đã xuất bản</option>
            <option value={ContentStatus.Archived}>Đã lưu trữ</option>
          </select>
          <select
            value={difficulty}
            onChange={(event) => {
              setDifficulty(event.target.value);
              setPage(1);
            }}
            className="h-[36px] rounded-[7px] border border-[#dfdbd4] bg-white px-3 text-[11px] text-[#555] outline-none"
          >
            <option value="">Tất cả độ khó</option>
            <option value="1">Dễ</option>
            <option value="2">Trung bình</option>
            <option value="3">Khó</option>
          </select>
        </div>

        <DataTable
          data={data.items}
          columns={columns}
          rowKey={(item) => item.id}
          loading={loading}
          selectable={false}
          page={data.page || page}
          pageSize={data.pageSize || pageSize}
          totalItems={data.total}
          totalPages={Math.max(1, data.totalPages)}
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
