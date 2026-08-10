"use client";

import {
  Search,
  X,
} from "lucide-react";

interface DataTableSearchProps {
  value: string;

  onChange: (
    value: string,
  ) => void;

  placeholder?: string;
}

export function DataTableSearch({
  value,
  onChange,
  placeholder = "Tìm kiếm...",
}: DataTableSearchProps) {
  return (
    <div
      className="
        relative
        w-full
        sm:max-w-[300px]
      "
    >
      <Search
        size={16}
        className="
          absolute
          left-3
          top-1/2
          -translate-y-1/2
          text-[#909090]
        "
      />

      <input
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
        placeholder={
          placeholder
        }
        className="
          h-[38px]
          w-full
          rounded-[7px]
          border
          border-[#dedbd6]
          bg-white
          pl-9
          pr-9
          text-[13px]
          outline-none
          transition
          placeholder:text-[#a0a0a0]
          focus:border-[#bdb8b0]
        "
      />

      {value && (
        <button
          type="button"
          onClick={() =>
            onChange("")
          }
          className="
            absolute
            right-2
            top-1/2
            flex h-6 w-6
            -translate-y-1/2
            items-center
            justify-center
            rounded
            text-[#999]
            hover:bg-[#f2f2f2]
          "
        >
          <X
            size={14}
          />
        </button>
      )}
    </div>
  );
}
