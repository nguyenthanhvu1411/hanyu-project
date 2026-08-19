"use client";

import {
  Activity,
  CircleCheckBig,
  CircleOff,
  GraduationCap,
  KeyRound,
  ListOrdered,
} from "lucide-react";
import { useState } from "react";

import { MetricCard } from "@/components/common/metric-card";
import { FormSection } from "@/components/forms/form-section";
import { Button } from "@/components/ui/button";
import { PERMISSIONS } from "@/constants/permission.constants";
import type { AdminHskLevelDto } from "@/dto/learning/hsk-level.dto";
import { PermissionGuard } from "@/security/permission-guard";

import { HskLevelStatusBadge } from "./hsk-level-status-badge";
import { HskLevelStatusDialog } from "./hsk-level-status-dialog";

export function HskLevelDetail({ item }: { item: AdminHskLevelDto }) {
  const [statusOpen, setStatusOpen] = useState(false);

  return (
    <>
      <div className="space-y-5">
        <div className="grid gap-4 md:grid-cols-3">
          <MetricCard
            title="Cấp độ"
            value={item.code}
            icon={<GraduationCap size={18} />}
            description={item.nameVi}
          />

          <MetricCard
            title="Thứ tự"
            value={item.sortOrder}
            icon={<ListOrdered size={18} />}
            description="Thứ tự hiển thị trong danh mục HSK"
          />

          <MetricCard
            title="Trạng thái"
            value={item.isActive ? "Hoạt động" : "Ngừng hoạt động"}
            icon={<Activity size={18} />}
            description={
              item.isActive
                ? "Có thể sử dụng cho Course, Lesson và Vocabulary"
                : "Không dùng cho dữ liệu mới"
            }
          />
        </div>

        <FormSection
          title="Thông tin cấp độ HSK"
          description="Thông tin định danh và cấu hình hiện tại của cấp độ. ID nội bộ dùng cho quan hệ database; PublicId dùng làm định danh công khai."
          icon={<GraduationCap size={18} />}
        >
          <div className="grid gap-x-8 gap-y-5 md:grid-cols-2 xl:grid-cols-3">
            <DetailItem label="Mã cấp độ" value={item.code} emphasis />
            <DetailItem label="Tên cấp độ" value={item.nameVi} />
            <DetailItem label="Thứ tự hiển thị" value={item.sortOrder} />
            <DetailItem label="ID nội bộ" value={item.id} />
            <DetailItem
              label="PublicId"
              value={
                <span className="break-all font-mono text-[10px] text-[#666]">
                  {item.publicId}
                </span>
              }
            />
            <DetailItem
              label="Trạng thái"
              value={<HskLevelStatusBadge isActive={item.isActive} />}
            />
          </div>
        </FormSection>

        <FormSection
          title="Quản lý trạng thái"
          description="Kích hoạt hoặc tạm ngừng cấp độ HSK mà không thay đổi dữ liệu lịch sử đang tham chiếu tới cấp độ này."
          icon={<KeyRound size={18} />}
        >
          <div className="flex flex-col gap-4 rounded-[9px] border border-[#ebe6df] bg-[#faf9f7] p-4 sm:flex-row sm:items-center sm:justify-between">
            <div className="min-w-0">
              <div className="text-[11px] font-semibold text-[#444]">
                {item.isActive
                  ? `${item.code} đang hoạt động`
                  : `${item.code} đang ngừng hoạt động`}
              </div>
              <div className="mt-1 max-w-[720px] text-[10px] leading-4 text-[#888]">
                {item.isActive
                  ? "Ngừng hoạt động sẽ giữ nguyên dữ liệu Course/Lesson/Vocabulary hiện có nhưng không nên dùng cấp độ này cho dữ liệu mới."
                  : "Kích hoạt lại để cho phép cấp độ này tiếp tục xuất hiện trong các danh mục lựa chọn của hệ thống."}
              </div>
            </div>

            <PermissionGuard
              permission={
                item.isActive
                  ? PERMISSIONS.HSK_LEVELS.DEACTIVATE
                  : PERMISSIONS.HSK_LEVELS.ACTIVATE
              }
              fallback={null}
            >
              <Button
                type="button"
                variant={item.isActive ? "outline" : "primary"}
                onClick={() => setStatusOpen(true)}
                className="h-[38px] shrink-0 gap-2 text-[11px]"
              >
                {item.isActive ? (
                  <CircleOff size={14} />
                ) : (
                  <CircleCheckBig size={14} />
                )}
                {item.isActive ? "Ngừng hoạt động" : "Kích hoạt"}
              </Button>
            </PermissionGuard>
          </div>
        </FormSection>
      </div>

      <HskLevelStatusDialog
        open={statusOpen}
        onOpenChange={setStatusOpen}
        item={item}
      />
    </>
  );
}

function DetailItem({
  label,
  value,
  emphasis = false,
}: {
  label: string;
  value: React.ReactNode;
  emphasis?: boolean;
}) {
  return (
    <div className="min-w-0 rounded-[8px] border border-[#eee9e2] bg-[#fcfbf9] px-3.5 py-3">
      <div className="text-[10px] font-medium text-[#929292]">{label}</div>
      <div
        className={`mt-[6px] min-h-[20px] text-[12px] ${
          emphasis ? "font-semibold text-[#202020]" : "font-medium text-[#414141]"
        }`}
      >
        {value}
      </div>
    </div>
  );
}
