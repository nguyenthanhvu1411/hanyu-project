"use client";

import { CircleCheckBig, CircleOff } from "lucide-react";
import { ConfirmDialog } from "@/components/common/confirm-dialog";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";
import {
  useActivateHskLevel,
  useDeactivateHskLevel,
} from "../../hooks/use-hsk-levels";
import type { AdminHskLevelDto } from "@/dto/learning/hsk-level.dto";

interface HskLevelStatusDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  item: AdminHskLevelDto | null;
}

export function HskLevelStatusDialog({
  open,
  onOpenChange,
  item,
}: HskLevelStatusDialogProps) {
  const activate = useActivateHskLevel();
  const deactivate = useDeactivateHskLevel();

  if (!item) {
    return null;
  }

  const activating = !item.isActive;
  const loading = activate.isPending || deactivate.isPending;

  async function handleConfirm() {
    try {
      if (activating) {
        await activate.mutateAsync(item!.id);
        appToast.success(`${item!.code} đã được kích hoạt.`);
      } else {
        await deactivate.mutateAsync(item!.id);
        appToast.success(`${item!.code} đã ngừng hoạt động.`);
      }
      onOpenChange(false);
    } catch (error) {
      const apiError = normalizeApiError(error);
      appToast.error(
        activating
          ? "Không thể kích hoạt cấp độ"
          : "Không thể ngừng hoạt động",
        apiError.message
      );
    }
  }

  return (
    <ConfirmDialog
      open={open}
      onOpenChange={onOpenChange}
      variant={activating ? "success" : "warning"}
      title={
        activating ? "Kích hoạt cấp độ HSK" : "Ngừng hoạt động cấp độ HSK"
      }
      description={
        activating
          ? `Bạn có chắc muốn kích hoạt ${item.code} - ${item.nameVi}?`
          : `Bạn có chắc muốn ngừng hoạt động ${item.code} - ${item.nameVi}?`
      }
      confirmText={activating ? "Kích hoạt" : "Ngừng hoạt động"}
      loading={loading}
      onConfirm={handleConfirm}
    >
      <div className="flex items-start gap-3 rounded-[8px] bg-[#faf9f7] p-3">
        <div
          className={`flex h-9 w-9 shrink-0 items-center justify-center rounded-[8px] ${
            activating
              ? "bg-[#edf8f2] text-[#16975b]"
              : "bg-[#fff7e4] text-[#b77c14]"
          }`}
        >
          {activating ? <CircleCheckBig size={17} /> : <CircleOff size={17} />}
        </div>
        <div className="text-[10px] leading-[16px] text-[#777]">
          {activating
            ? "Cấp độ sẽ có thể được sử dụng cho dữ liệu mới."
            : "Dữ liệu cũ vẫn được giữ nguyên, nhưng cấp độ này không nên được chọn cho nội dung mới."}
        </div>
      </div>
    </ConfirmDialog>
  );
}
