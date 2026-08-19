"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { RefreshCw, ToggleLeft, ToggleRight } from "lucide-react";

import { DataTable } from "@/components/common/data-table/data-table";
import { ActionButton, DataTableActions } from "@/components/common/data-table/data-table-actions";
import { DataTableSearch } from "@/components/common/data-table/data-table-search";
import { DataTableToolbar } from "@/components/common/data-table/data-table-toolbar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";
import type { DataTableColumn } from "@/types/table.types";

import { quizApi } from "../quiz.api";
import type { AdminQuestionBank } from "../quiz.types";

export function QuestionBankTable() {
  const router = useRouter();
  const [items, setItems] = useState<AdminQuestionBank[]>([]);
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);
  const [workingId, setWorkingId] = useState<number | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      setItems(await quizApi.listQuestionBanks());
    } catch (caught) {
      appToast.error("Không thể tải ngân hàng câu hỏi", normalizeApiError(caught).message);
      setItems([]);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { void load(); }, [load]);

  const filtered = useMemo(() => {
    const keyword = search.trim().toLowerCase();
    if (!keyword) return items;
    return items.filter((item) =>
      item.code.toLowerCase().includes(keyword) ||
      item.nameVi.toLowerCase().includes(keyword) ||
      item.descriptionVi?.toLowerCase().includes(keyword),
    );
  }, [items, search]);

  async function toggle(item: AdminQuestionBank) {
    setWorkingId(item.id);
    try {
      if (item.isActive) {
        await quizApi.deactivateQuestionBank(item.id);
        appToast.success("Đã ngừng kích hoạt ngân hàng câu hỏi.");
      } else {
        await quizApi.activateQuestionBank(item.id);
        appToast.success("Đã kích hoạt ngân hàng câu hỏi.");
      }
      await load();
    } catch (caught) {
      appToast.error("Không thể cập nhật trạng thái", normalizeApiError(caught).message);
    } finally {
      setWorkingId(null);
    }
  }

  const columns = useMemo<DataTableColumn<AdminQuestionBank>[]>(() => [
    {
      id: "bank",
      header: "Ngân hàng câu hỏi",
      cell: (item) => (
        <div className="min-w-0">
          <div className="truncate text-[12px] font-semibold text-[#333]">{item.nameVi}</div>
          <div className="mt-0.5 text-[10px] text-muted-foreground">{item.code} · {item.publicId.slice(0, 8)}…</div>
        </div>
      ),
    },
    {
      id: "hsk",
      header: "HSK",
      align: "center",
      width: "100px",
      cell: (item) => <span className="text-[11px]">{item.hskLevelId ? `#${item.hskLevelId}` : "Tất cả"}</span>,
    },
    {
      id: "count",
      header: "Số câu hỏi",
      align: "center",
      width: "110px",
      cell: (item) => <span className="text-[11px] font-medium">{item.questionCount}</span>,
    },
    {
      id: "status",
      header: "Trạng thái",
      align: "center",
      width: "120px",
      cell: (item) => <Badge variant={item.isActive ? "success" : "default"}>{item.isActive ? "Đang dùng" : "Tạm dừng"}</Badge>,
    },
    {
      id: "actions",
      header: "Thao tác",
      align: "center",
      width: "90px",
      cell: (item) => (
        <DataTableActions
          onView={() => router.push(`/cau-hoi/${item.id}`)}
          onEdit={() => router.push(`/cau-hoi/${item.id}/chinh-sua`)}
          customActions={
            <ActionButton
              icon={item.isActive ? <ToggleLeft size={14} /> : <ToggleRight size={14} />}
              onClick={() => void toggle(item)}
            >
              {workingId === item.id ? "Đang cập nhật..." : item.isActive ? "Tạm dừng" : "Kích hoạt"}
            </ActionButton>
          }
        />
      ),
    },
  ], [router, workingId]);

  return (
    <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
      <DataTableToolbar
        left={<DataTableSearch value={search} onChange={setSearch} placeholder="Tìm theo tên hoặc mã ngân hàng..." />}
        right={<Button variant="outline" className="h-[38px] gap-2 text-[11px]" onClick={() => void load()}><RefreshCw size={14} /> Làm mới</Button>}
      />
      <DataTable
        data={filtered}
        columns={columns}
        rowKey={(item) => item.id}
        loading={loading}
        selectable={false}
        page={1}
        pageSize={Math.max(20, filtered.length)}
        totalItems={filtered.length}
        totalPages={1}
      />
    </div>
  );
}
