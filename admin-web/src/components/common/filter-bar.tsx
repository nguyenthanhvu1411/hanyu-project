import {
  RotateCcw,
} from "lucide-react";

import {
  Button,
} from "@/components/ui/button";

interface FilterBarProps {
  children: React.ReactNode;

  onReset?: () => void;

  extra?: React.ReactNode;
}

export function FilterBar({
  children,
  onReset,
  extra,
}: FilterBarProps) {
  return (
    <div
      className="
        flex
        flex-col
        gap-3
        rounded-[10px]
        border
        border-[#e8e3dc]
        bg-white
        p-3
        sm:flex-row
        sm:items-center
        sm:justify-between
      "
    >
      <div
        className="
          flex
          flex-1
          flex-wrap
          items-center
          gap-2
        "
      >
        {children}

        {onReset && (
          <Button
            type="button"
            variant="ghost"
            onClick={
              onReset
            }
            className="
              h-[36px]
              gap-2
              text-[11px]
              text-[#777]
            "
          >
            <RotateCcw
              size={13}
            />

            Đặt lại
          </Button>
        )}
      </div>

      {extra && (
        <div
          className="
            flex
            items-center
            gap-2
          "
        >
          {extra}
        </div>
      )}
    </div>
  );
}
