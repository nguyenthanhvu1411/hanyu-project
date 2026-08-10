import {
  ArrowLeft,
  Save,
} from "lucide-react";

import {
  Button,
} from "@/components/ui/button";

interface FormActionsProps {
  submitText?: string;
  cancelText?: string;

  loading?: boolean;
  disabled?: boolean;

  onCancel?: () => void;
}

export function FormActions({
  submitText = "Lưu dữ liệu",
  cancelText = "Quay lại",
  loading = false,
  disabled = false,
  onCancel,
}: FormActionsProps) {
  return (
    <div
      className="
        flex
        min-h-[76px]
        items-center
        justify-end
        gap-2
        rounded-[11px]
        border
        border-[#e8e3dc]
        bg-white
        px-5
        py-3
      "
    >
      {onCancel && (
        <Button
          type="button"
          variant="outline"
          disabled={loading}
          onClick={onCancel}
          className="
            h-[40px]
            gap-2
            px-4
            text-[12px]
          "
        >
          <ArrowLeft
            size={14}
          />

          {cancelText}
        </Button>
      )}

      <Button
        type="submit"
        loading={loading}
        disabled={disabled}
        className="
          h-[40px]
          gap-2
          px-4
          text-[12px]
        "
      >
        <Save
          size={14}
        />

        {submitText}
      </Button>
    </div>
  );
}
