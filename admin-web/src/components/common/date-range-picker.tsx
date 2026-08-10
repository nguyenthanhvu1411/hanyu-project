"use client";

import {
  CalendarDays,
  X,
} from "lucide-react";

interface DateRangeValue {
  from?: string;
  to?: string;
}

interface DateRangePickerProps {
  value: DateRangeValue;

  onChange: (
    value: DateRangeValue,
  ) => void;

  disabled?: boolean;
}

export function DateRangePicker({
  value,
  onChange,
  disabled = false,
}: DateRangePickerProps) {
  return (
    <div
      className="
        flex
        flex-col
        gap-2
        sm:flex-row
        sm:items-center
      "
    >
      <DateInput
        value={
          value.from
        }
        onChange={(
          from,
        ) =>
          onChange({
            ...value,
            from,
          })
        }
        disabled={
          disabled
        }
      />

      <span
        className="
          hidden
          text-[10px]
          text-[#999]
          sm:block
        "
      >
        đến
      </span>

      <DateInput
        value={
          value.to
        }
        min={
          value.from
        }
        onChange={(
          to,
        ) =>
          onChange({
            ...value,
            to,
          })
        }
        disabled={
          disabled
        }
      />

      {(value.from ||
        value.to) &&
        !disabled && (
          <button
            type="button"
            onClick={() =>
              onChange({})
            }
            className="
              flex h-8 w-8
              shrink-0
              items-center
              justify-center
              rounded-[6px]
              text-[#999]
              hover:bg-[#f3f3f3]
            "
          >
            <X size={14} />
          </button>
        )}
    </div>
  );
}

function DateInput({
  value = "",
  onChange,
  min,
  disabled,
}: {
  value?: string;
  onChange: (
    value: string,
  ) => void;
  min?: string;
  disabled?: boolean;
}) {
  return (
    <div className="relative">
      <CalendarDays
        size={14}
        className="
          pointer-events-none
          absolute
          left-3
          top-1/2
          -translate-y-1/2
          text-[#888]
        "
      />

      <input
        type="date"
        value={value}
        min={min}
        disabled={
          disabled
        }
        onChange={(
          event,
        ) =>
          onChange(
            event.target
              .value,
          )
        }
        className="
          h-[38px]
          rounded-[7px]
          border
          border-[#dedbd6]
          bg-white
          pl-9
          pr-3
          text-[11px]
          text-[#555]
          outline-none
        "
      />
    </div>
  );
}
