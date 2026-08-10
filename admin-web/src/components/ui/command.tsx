"use client";

import {
  Search,
} from "lucide-react";

import {
  cn,
} from "@/lib/utils/cn";

export function Command({
  children,
  className,
}: {
  children: React.ReactNode;
  className?: string;
}) {
  return (
    <div
      className={cn(
        "overflow-hidden",
        "rounded-[9px]",
        "border",
        "border-[#e4dfd7]",
        "bg-white",
        className,
      )}
    >
      {children}
    </div>
  );
}

export function CommandInput({
  value,
  onChange,
  placeholder = "Tìm kiếm...",
}: {
  value: string;
  onChange: (
    value: string,
  ) => void;
  placeholder?: string;
}) {
  return (
    <div
      className="
        relative
        border-b
        border-[#eee9e2]
        p-2
      "
    >
      <Search
        size={14}
        className="
          absolute
          left-5
          top-1/2
          -translate-y-1/2
          text-[#999]
        "
      />

      <input
        value={value}
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
          h-9
          w-full
          rounded-[6px]
          bg-[#faf9f7]
          pl-9
          pr-3
          text-[11px]
          outline-none
        "
      />
    </div>
  );
}

export function CommandList({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <div
      className="
        scrollbar-thin
        max-h-[250px]
        overflow-y-auto
        p-1
      "
    >
      {children}
    </div>
  );
}

export function CommandEmpty({
  children = "Không có dữ liệu.",
}: {
  children?: React.ReactNode;
}) {
  return (
    <div
      className="
        px-3
        py-7
        text-center
        text-[10px]
        text-[#999]
      "
    >
      {children}
    </div>
  );
}
