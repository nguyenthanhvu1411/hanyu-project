"use client";

import {
  Check,
  ChevronRight,
} from "lucide-react";

import {
  Popover,
} from "./popover";

import {
  cn,
} from "@/lib/utils/cn";

export function DropdownMenu({
  trigger,
  children,
  align = "right",
}: {
  trigger: React.ReactNode;
  children: React.ReactNode;
  align?:
    | "left"
    | "right";
}) {
  return (
    <Popover
      trigger={
        trigger
      }
      align={
        align
      }
      className="
        min-w-[180px]
        p-1
      "
    >
      {children}
    </Popover>
  );
}

export function DropdownMenuItem({
  children,
  icon,
  danger = false,
  disabled = false,
  onClick,
}: {
  children: React.ReactNode;
  icon?: React.ReactNode;
  danger?: boolean;
  disabled?: boolean;
  onClick?: () => void;
}) {
  return (
    <button
      type="button"
      disabled={
        disabled
      }
      onClick={
        onClick
      }
      className={cn(
        "flex h-9",
        "w-full",
        "items-center",
        "gap-2",
        "rounded-[6px]",
        "px-2",
        "text-left",
        "text-[11px]",
        "transition",

        danger
          ? "text-[#ef241c] hover:bg-[#fff0ee]"
          : "text-[#555] hover:bg-[#f7f6f4]",

        disabled &&
          "cursor-not-allowed opacity-40",
      )}
    >
      {icon}

      <span className="flex-1">
        {children}
      </span>
    </button>
  );
}

export function DropdownMenuSeparator() {
  return (
    <div
      className="
        my-1
        h-px
        bg-[#ebe7e0]
      "
    />
  );
}
