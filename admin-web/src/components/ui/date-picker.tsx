"use client";

import {
  CalendarDays,
  X,
} from "lucide-react";

import {
  cn,
} from "@/lib/utils/cn";

interface DatePickerProps {
  value?: string;

  onChange?: (
    value: string,
  ) => void;

  min?: string;

  max?: string;

  disabled?: boolean;

  error?: boolean;

  clearable?: boolean;

  className?: string;
}

export function DatePicker({
  value = "",
  onChange,
  min,
  max,
  disabled = false,
  error = false,
  clearable = true,
  className,
}: DatePickerProps) {
  return (
    <div
      className={cn(
        "relative",
        className,
      )}
    >
      <CalendarDays
        size={16}
        className="
          pointer-events-none
          absolute
          left-3 top-1/2
          z-10
          -translate-y-1/2
          text-[#777]
        "
      />

      <input
        type="date"
        value={value}
        min={min}
        max={max}
        disabled={disabled}
        onChange={(
          event,
        ) =>
          onChange?.(
            event.target
              .value,
          )
        }
        className={cn(
          "h-[42px]",
          "w-full",
          "rounded-[7px]",
          "border",
          "bg-white",
          "pl-9",
          "pr-9",
          "text-[12px]",
          "text-[#444]",
          "outline-none",
          "transition",

          error
            ? "border-[#ef453f]"
            : "border-[#dedbd6] focus:border-[#bbb5ad] focus:ring-2 focus:ring-black/[0.03]",

          disabled &&
            "cursor-not-allowed bg-[#f7f7f7] opacity-60",
        )}
      />

      {value &&
        clearable &&
        !disabled && (
          <button
            type="button"
            onClick={() =>
              onChange?.("")
            }
            className="
              absolute
              right-[34px]
              top-1/2
              z-20
              flex h-6 w-6
              -translate-y-1/2
              items-center
              justify-center
              rounded
              bg-white
              text-[#aaa]
              hover:bg-[#f2f2f2]
            "
          >
            <X size={13} />
          </button>
        )}
    </div>
  );
}
