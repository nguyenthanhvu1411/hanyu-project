"use client";

import {
  Search,
  X,
} from "lucide-react";

import {
  cn,
} from "@/lib/utils/cn";

interface SearchInputProps {
  value: string;

  onChange: (
    value: string,
  ) => void;

  placeholder?: string;

  className?: string;

  disabled?: boolean;
}

export function SearchInput({
  value,
  onChange,
  placeholder = "Tìm kiếm...",
  className,
  disabled = false,
}: SearchInputProps) {
  return (
    <div
      className={cn(
        "relative",
        "w-full",
        className,
      )}
    >
      <Search
        size={15}
        className="
          pointer-events-none
          absolute
          left-3
          top-1/2
          -translate-y-1/2
          text-[#999]
        "
      />

      <input
        value={value}
        disabled={
          disabled
        }
        placeholder={
          placeholder
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
          h-[39px]
          w-full
          rounded-[7px]
          border
          border-[#dedbd6]
          bg-white
          pl-9
          pr-9
          text-[12px]
          outline-none
          transition
          placeholder:text-[#aaa]
          focus:border-[#bbb5ad]
        "
      />

      {value &&
        !disabled && (
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
              rounded-[5px]
              text-[#999]
              hover:bg-[#f2f2f2]
            "
          >
            <X size={13} />
          </button>
        )}
    </div>
  );
}
