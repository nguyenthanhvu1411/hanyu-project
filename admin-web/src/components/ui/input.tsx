"use client";

import * as React from "react";

import { cn } from "@/lib/utils/cn";

export interface InputProps
  extends React.InputHTMLAttributes<HTMLInputElement> {
  error?: boolean;
}

export const Input = React.forwardRef<
  HTMLInputElement,
  InputProps
>(function Input(
  {
    className,
    error,
    ...props
  },
  ref,
) {
  return (
    <input
      ref={ref}
      className={cn(
        "h-[54px] w-full",
        "rounded-[7px]",
        "border bg-white",
        "px-4",
        "text-[14px] text-[#202124]",
        "outline-none",
        "transition-all duration-150",

        error
          ? "border-[#ef453f] focus:border-[#ef241c] focus:ring-2 focus:ring-[#ef241c]/10"
          : "border-[#dedede] hover:border-[#c9c9c9] focus:border-[#999] focus:ring-2 focus:ring-black/5",

        className,
      )}
      {...props}
    />
  );
});
