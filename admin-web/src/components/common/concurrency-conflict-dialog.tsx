"use client";

import { RefreshCw, TriangleAlert } from "lucide-react";
import { Button } from "@/components/ui/button";

interface ConcurrencyConflictDialogProps {
  open: boolean;
  title?: string;
  description?: string;
  onReload: () => void | Promise<void>;
  onCancel: () => void;
  loading?: boolean;
}

export function ConcurrencyConflictDialog({
  open,
  title = "Dữ liệu đã được thay đổi",
  description = "Một quản trị viên hoặc tiến trình khác đã cập nhật dữ liệu này sau khi bạn mở trang. Hệ thống không ghi đè để tránh mất dữ liệu.",
  onReload,
  onCancel,
  loading = false,
}: ConcurrencyConflictDialogProps) {
  if (!open) {
    return null;
  }

  return (
    <div className="fixed inset-0 z-[140] flex items-center justify-center p-4">
      <div className="absolute inset-0 bg-black/40 backdrop-blur-[1px]" />

      <div
        role="alertdialog"
        aria-modal="true"
        className="relative w-full max-w-[470px] overflow-hidden rounded-[14px] border border-[#eadbd8] bg-white shadow-[0_24px_70px_rgba(0,0,0,0.18)]"
      >
        <div className="p-5">
          <div className="flex items-start gap-3">
            <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-[10px] bg-[#fff7e4] text-[#b77c14]">
              <TriangleAlert size={21} />
            </div>

            <div className="min-w-0">
              <h2 className="text-[15px] font-semibold text-[#333]">{title}</h2>
              <p className="mt-2 text-[11px] leading-[18px] text-[#777]">
                {description}
              </p>
            </div>
          </div>

          <div className="mt-4 rounded-[8px] border border-[#eee3c7] bg-[#fffaf0] px-3 py-3 text-[10px] leading-[16px] text-[#806522]">
            Hãy tải lại dữ liệu mới nhất, kiểm tra thay đổi rồi thực hiện lưu lại.
          </div>
        </div>

        <div className="flex justify-end gap-2 border-t border-[#ebe6df] bg-[#faf9f7] px-5 py-3">
          <Button
            type="button"
            variant="outline"
            disabled={loading}
            onClick={onCancel}
            className="h-[38px] text-[11px]"
          >
            Đóng
          </Button>

          <Button
            type="button"
            loading={loading}
            onClick={onReload}
            className="h-[38px] gap-2 text-[11px]"
          >
            <RefreshCw size={14} />
            Tải dữ liệu mới
          </Button>
        </div>
      </div>
    </div>
  );
}
