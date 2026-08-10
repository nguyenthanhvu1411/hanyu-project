"use client";

import { DeleteDialog } from "@/components/common/delete-dialog";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";
import { useDeleteHskLevel } from "../../hooks/use-hsk-levels";
import type { AdminHskLevelDto } from "@/dto/learning/hsk-level.dto";

interface HskLevelDeleteDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  item: AdminHskLevelDto | null;
}

export function HskLevelDeleteDialog({
  open,
  onOpenChange,
  item,
}: HskLevelDeleteDialogProps) {
  const mutation = useDeleteHskLevel();

  async function handleDelete() {
    if (!item) {
      return;
    }

    try {
      await mutation.mutateAsync(item.id);
      appToast.success("Xóa cấp độ HSK thành công.");
      onOpenChange(false);
    } catch (error) {
      const apiError = normalizeApiError(error);
      appToast.error("Không thể xóa cấp độ HSK", apiError.message);
    }
  }

  return (
    <DeleteDialog
      open={open}
      onOpenChange={onOpenChange}
      title="Xóa cấp độ HSK"
      itemName={item ? `${item.code} - ${item.nameVi}` : undefined}
      description="Nếu cấp độ đang được khóa học, bài giảng hoặc dữ liệu khác sử dụng, backend có thể từ chối thao tác này."
      loading={mutation.isPending}
      onDelete={handleDelete}
    />
  );
}
