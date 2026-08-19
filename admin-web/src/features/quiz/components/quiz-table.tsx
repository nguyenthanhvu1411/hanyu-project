"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { RefreshCw, Trash2 } from "lucide-react";

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
import { normalizeApiError } from "@/lib/api/api-error";
import { PermissionGuard } from "@/security/permission-guard";
import type { DataTableColumn } from "@/types/table.types";

import { quizApi } from "../quiz.api";
import {
  AdminQuiz,
  ContentStatus,
  QUIZ_STATUS_LABELS,
  QUIZ_TYPE_LABELS,
  QuizType,
} from "../quiz.types";

const STATUS_OPTIONS = Object.entries(QUIZ_STATUS_LABELS).map(([value, label]) => ({ label, value }));
const TYPE_OPTIONS = Object.entries(QUIZ_TYPE_LABELS).map(([value, label]) => ({ label, value }));

function statusVariant(status: ContentStatus): "default" | "success" | "warning" | "info" {
  if (status === ContentStatus.Published) return "success";
  if (status === ContentStatus.Review) return "warning";
  if (status === ContentStatus.Approved) return "info";
  return "default";
}

export function QuizTable() {
  const router = useRouter();
  const [items, setItems] = useState<AdminQuiz[]>([]);
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("");
  const [quizType, setQuizType] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [totalItems, setTotalItems] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);
  const [deletingId, setDeletingId] = useState<number | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await quizApi.list({
        q: search.trim() || undefined,
        status: status === "" ? undefined : (Number(status) as ContentStatus),
        quizType: quizType === "" ? undefined : (Number(quizType) as QuizType),
        page,
        pageSize,
        sort: "-updatedAt",
      });
      const total = result.total ?? result.totalCount ?? 0;
      setItems(result.items ?? []);
      setTotalItems(total);
      setTotalPages(result.totalPages ?? Math.max(1, Math.ceil(total / Math.max(1, result.pageSize ?? pageSize))));
    } catch (caught) {
      setItems([]);
      setTotalItems(0);
      setTotalPages(1);
      setError(caught instanceof Error ? caught : new Error("Không thể tải danh sách bài kiểm tra."));
    } finally {
      setLoading(false);
    }
  }, [page, pageSize, quizType, search, status]);

  useEffect(() => {
    const timeout = window.setTimeout(() => void load(), search ? 250 : 0);
    return () => window.clearTimeout(timeout);
  }, [load, search]);

  const remove = useCallback(async (quiz: AdminQuiz) => {
    if (deletingId || !window.confirm(`Xóa bài kiểm tra “${quiz.titleVi}”?`)) return;
    setDeletingId(quiz.id);
    try {
      await quizApi.delete(quiz.id);
      appToast.success("Đã xóa bài kiểm tra.");
      await load();
    } catch (caught) {
      appToast.error("Không thể xóa bài kiểm tra", normalizeApiError(caught).message);
    } finally {
      setDeletingId(null);
    }
  }, [deletingId, load]);

  const columns = useMemo<DataTableColumn<AdminQuiz>[]>(() => [
    {
      id: "title",
      header: "Bài kiểm tra",
      cell: (item) => (
        <div className="min-w-0">
          <div className="truncate text-[12px] font-semibold text-[#333]">{item.titleVi}</div>
          <div className="mt-0.5 text-[10px] text-[#8a8a8a]">{QUIZ_TYPE_LABELS[item.quizType]}</div>
        </div>
      ),
    },
    {
      id: "lesson",
      header: "Bài giảng",
      cell: (item) => <span className="text-[11px] text-[#555]">{item.lessonTitleVi ?? "Không gắn bài giảng"}</span>,
    },
    {
      id: "settings",
      header: "Thiết lập",
      align: "center",
      width: "150px",
      cell: (item) => (
        <div className="text-[10px] text-[#666]">
          <div>Điểm đạt: {item.passingScore}%</div>
          <div>{item.timeLimitSeconds ? `${Math.ceil(item.timeLimitSeconds / 60)} phút` : "Không giới hạn giờ"}</div>
        </div>
      ),
    },
    {
      id: "status",
      header: "Trạng thái",
      align: "center",
      width: "135px",
      cell: (item) => <Badge variant={statusVariant(item.status)}>{QUIZ_STATUS_LABELS[item.status]}</Badge>,
    },
    {
      id: "actions",
      header: "Thao tác",
      align: "center",
      width: "90px",
      cell: (item) => (
        <DataTableActions
          onView={() => router.push(`/bai-kiem-tra/${item.id}`)}
          onEdit={() => router.push(`/bai-kiem-tra/${item.id}/chinh-sua`)}
          customActions={
            <PermissionGuard permission={PERMISSIONS.QUIZZES.DELETE} fallback={null}>
              <ActionButton danger icon={<Trash2 size={14} />} onClick={() => void remove(item)}>
                {deletingId === item.id ? "Đang xóa..." : "Xóa"}
              </ActionButton>
            </PermissionGuard>
          }
        />
      ),
    },
  ], [deletingId, remove, router]);

  if (error && !loading) {
    return <ErrorState title="Không thể tải bài kiểm tra" description={error.message} onRetry={() => void load()} />;
  }

  return (
    <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
      <DataTableToolbar
        left={
          <>
            <DataTableSearch
              value={search}
              onChange={(value) => { setSearch(value); setPage(1); }}
              placeholder="Tìm bài kiểm tra..."
            />
            <DataTableFilter
              value={status}
              onChange={(value) => { setStatus(value); setPage(1); }}
              options={STATUS_OPTIONS}
              placeholder="Tất cả trạng thái"
            />
            <DataTableFilter
              value={quizType}
              onChange={(value) => { setQuizType(value); setPage(1); }}
              options={TYPE_OPTIONS}
              placeholder="Tất cả loại"
            />
          </>
        }
        right={
          <Button variant="outline" className="h-[38px] gap-2 text-[11px]" onClick={() => void load()}>
            <RefreshCw size={14} /> Làm mới
          </Button>
        }
      />
      <DataTable
        data={items}
        columns={columns}
        rowKey={(item) => item.id}
        loading={loading}
        selectable={false}
        page={page}
        pageSize={pageSize}
        totalItems={totalItems}
        totalPages={totalPages}
        onPageChange={setPage}
        onPageSizeChange={(value) => { setPageSize(value); setPage(1); }}
      />
    </div>
  );
}
