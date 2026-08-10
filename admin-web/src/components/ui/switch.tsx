"use client";

import { cn } from "@/lib/utils/cn";

interface SwitchProps {
  checked: boolean;
  onCheckedChange: (checked: boolean) => void;

  disabled?: boolean;

  label?: string;
  description?: string;

  className?: string;
}

export function Switch({
  checked,
  onCheckedChange,
  disabled = false,
  label,
  description,
  className,
}: SwitchProps) {
  return (
    <div
      className={cn(
        "flex items-center gap-2",
        disabled && "opacity-60",
        className,
      )}
    >
      <button
        type="button"
        role="switch"
        aria-checked={checked}
        disabled={disabled}
        onClick={() => onCheckedChange(!checked)}
        className={cn(
          "relative inline-flex h-[22px] w-[40px] shrink-0 items-center rounded-full transition-colors",
          "focus:outline-none focus:ring-2 focus:ring-[#ef241c]/15",
          checked
            ? "bg-[#16975b]"
            : "bg-[#d3d3d3]",
          disabled &&
            "cursor-not-allowed",
        )}
      >
        <span
          className={cn(
            "pointer-events-none block h-4 w-4 rounded-full bg-white shadow-sm transition-transform",
            checked
              ? "translate-x-[21px]"
              : "translate-x-[3px]",
          )}
        />
      </button>

      {(label || description) && (
        <div className="min-w-0">
          {label && (
            <div
              className="
                whitespace-nowrap
                text-[12px]
                font-medium
                leading-[18px]
                text-[#444]
              "
            >
              {label}
            </div>
          )}

          {description && (
            <div
              className="
                mt-[1px]
                text-[10px]
                leading-[15px]
                text-[#8d8d8d]
              "
            >
              {description}
            </div>
          )}
        </div>
      )}
    </div>
  );
}
