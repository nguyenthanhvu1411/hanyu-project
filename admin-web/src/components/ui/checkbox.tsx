"use client";

import {
  Check,
} from "lucide-react";

import { cn } from "@/lib/utils/cn";

interface CheckboxProps {
  checked: boolean;
  onCheckedChange: (
    checked: boolean,
  ) => void;
  className?: string;
}

export function Checkbox({
  checked,
  onCheckedChange,
  className,
}: CheckboxProps) {
  return (
    <button
      type="button"
      role="checkbox"
      aria-checked={checked}
      onClick={() =>
        onCheckedChange(!checked)
      }
      className={cn(
        "flex h-[19px] w-[19px]",
        "items-center justify-center",
        "rounded-[4px] border",
        "transition",

        checked
          ? "border-[#1e9c61] bg-[#1e9c61] text-white"
          : "border-[#cfcfcf] bg-white",

        className,
      )}
    >
      {checked && (
        <Check
          size={14}
          strokeWidth={3}
        />
      )}
    </button>
  );
}
