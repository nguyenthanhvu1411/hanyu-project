"use client";

import {
  CircleCheckBig,
  CircleOff,
  GraduationCap,
  Hash,
  ListOrdered,
} from "lucide-react";
import { useState } from "react";
import { FormSection } from "@/components/forms/form-section";
import { MetricCard } from "@/components/common/metric-card";
import { Button } from "@/components/ui/button";
import { PermissionGuard } from "@/security/permission-guard";
import { PERMISSIONS } from "@/constants/permission.constants";
import { HskLevelStatusBadge } from "./hsk-level-status-badge";
import { HskLevelStatusDialog } from "./hsk-level-status-dialog";
import type { AdminHskLevelDto } from "@/dto/learning/hsk-level.dto";

export function HskLevelDetail({ item }: { item: AdminHskLevelDto }) {
  const [statusOpen, setStatusOpen] = useState(false);

  return (
    <>
      <div className="space-y-5">
        <div className="grid gap-4 md:grid-cols-3">
          <MetricCard
            title="Cấp độ"
            value={`HSK ${item.id}`}
            icon={<GraduationCap size={18} />}
            description="Cấp độ HSK"
          />

          <MetricCard
            title="Thứ tự"
            value={item.sortOrder}
            icon={<ListOrdered size={18} />}
            description="Thứ tự hiển thị"
          />

          <MetricCard
            title="Trạng thái"
            value={item.isActive ? "Hoạt động" : "Ngừng hoạt động"}
            icon={<Hash size={18} />}
          />
        </div>

        <FormSection
          title="Thông tin cấp độ HSK"
          description="Thông tin cấu hình hiện tại của cấp độ."
          icon={<GraduationCap size={18} />}
        >
          <div className="grid gap-x-8 gap-y-5 md:grid-cols-2">
            <DetailItem label="ID" value={item.id} />
            <DetailItem
              label="Mã cấp độ"
              value={
                <code className="rounded-[5px] bg-[#f5f4f1] px-2 py-1 text-[10px]">
                  {item.code}
                </code>
              }
            />
            <DetailItem label="Tên cấp độ" value={item.nameVi} />
            <DetailItem label="Thứ tự" value={item.sortOrder} />
            <DetailItem
              label="Trạng thái"
              value={<HskLevelStatusBadge isActive={item.isActive} />}
            />
          </div>
        </FormSection>

        <FormSection
          title="Trạng thái sử dụng"
          description="Kích hoạt hoặc tạm ngừng cấp độ HSK."
        >
          <div className="flex flex-col gap-4 rounded-[9px] border border-[#ebe6df] bg-[#faf9f7] p-4 sm:flex-row sm:items-center sm:justify-between">
            <div>
              <div className="text-[11px] font-semibold text-[#444]">
                {item.isActive
                  ? "Cấp độ đang hoạt động"
                  : "Cấp độ đang ngừng hoạt động"}
              </div>
              <div className="mt-1 text-[10px] text-[#888]">
                {item.isActive
                  ? "Bạn có thể ngừng sử dụng cấp độ này cho dữ liệu mới."
                  : "Bạn có thể kích hoạt lại cấp độ này."}
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
                className="h-[38px] gap-2 text-[11px]"
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
}: {
  label: string;
  value: React.ReactNode;
}) {
  return (
    <div className="min-w-0">
      <div className="text-[10px] font-medium text-[#929292]">{label}</div>
      <div className="mt-[5px] min-h-[20px] text-[12px] font-medium text-[#414141]">
        {value}
      </div>
    </div>
  );
}
