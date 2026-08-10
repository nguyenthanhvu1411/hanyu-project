"use client";

import {
  Filter,
} from "lucide-react";

interface FilterOption {
  label: string;
  value: string;
}

interface DataTableFilterProps {
  value: string;

  onChange: (
    value: string,
  ) => void;

  options: FilterOption[];

  placeholder?: string;
}

export function DataTableFilter({
  value,
  onChange,
  options,
  placeholder = "Tất cả trạng thái",
}: DataTableFilterProps) {
  return (
    <div className="relative">
      <Filter
        size={15}
        className="
          pointer-events-none
          absolute
          left-3
          top-1/2
          -translate-y-1/2
          text-[#888]
        "
      />

      <select
        value={
          value
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
          min-w-[165px]
          appearance-none
          rounded-[7px]
          border
          border-[#dedbd6]
          bg-white
          pl-9
          pr-8
          text-[12px]
          text-[#555]
          outline-none
        "
      >
        <option value="">
          {
            placeholder
          }
        </option>

        {options.map(
          (
            option,
          ) => (
            <option
              key={
                option.value
              }
              value={
                option.value
              }
            >
              {
                option.label
              }
            </option>
          ),
        )}
      </select>
    </div>
  );
}
