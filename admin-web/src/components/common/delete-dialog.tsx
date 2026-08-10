"use client";

import {
  Trash2,
} from "lucide-react";

import {
  ConfirmDialog,
} from "./confirm-dialog";

interface DeleteDialogProps {
  open: boolean;

  onOpenChange: (
    open: boolean,
  ) => void;

  itemName?: string;

  title?: string;

  description?: string;

  loading?: boolean;

  onDelete: () =>
    | void
    | Promise<void>;
}

export function DeleteDialog({
  open,
  onOpenChange,
  itemName,
  title = "Xác nhận xóa dữ liệu",
  description,
  loading = false,
  onDelete,
}: DeleteDialogProps) {
  return (
    <ConfirmDialog
      open={open}
      onOpenChange={
        onOpenChange
      }
      variant="danger"
      title={title}
      description={
        description ??
        "Dữ liệu sau khi xóa có thể không thể khôi phục. Hãy kiểm tra kỹ trước khi tiếp tục."
      }
      confirmText="Xóa dữ liệu"
      cancelText="Hủy"
      loading={loading}
      onConfirm={
        onDelete
      }
    >
      {itemName && (
        <div
          className="
            flex
            items-start
            gap-3
          "
        >
          <div
            className="
              flex h-8 w-8
              shrink-0
              items-center
              justify-center
              rounded-[7px]
              bg-[#fff0ee]
              text-[#ef241c]
            "
          >
            <Trash2
              size={15}
            />
          </div>

          <div>
            <div
              className="
                text-[10px]
                text-[#929292]
              "
            >
              Dữ liệu sẽ xóa
            </div>

            <div
              className="
                mt-[2px]
                text-[12px]
                font-semibold
                text-[#444]
              "
            >
              {itemName}
            </div>
          </div>
        </div>
      )}
    </ConfirmDialog>
  );
}
