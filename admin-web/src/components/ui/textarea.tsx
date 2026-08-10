"use client";

import * as React from "react";

import { cn } from "@/lib/utils/cn";

export interface TextareaProps
  extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  error?: boolean;
}

export const Textarea =
  React.forwardRef<
    HTMLTextAreaElement,
    TextareaProps
  >(function Textarea(
    {
      className,
      error = false,
      ...props
    },
    ref,
  ) {
    return (
      <textarea
        ref={ref}
        className={cn(
          "min-h-[110px]",
          "w-full",
          "resize-y",
          "rounded-[7px]",
          "border",
          "bg-white",
          "px-3",
          "py-[10px]",
          "text-[12px]",
          "leading-[19px]",
          "text-[#3f3f3f]",
          "outline-none",
          "transition",
          "placeholder:text-[#a0a0a0]",

          error
            ? "border-[#ef453f] focus:ring-2 focus:ring-[#ef241c]/10"
            : "border-[#dedbd6] focus:border-[#bab5ad] focus:ring-2 focus:ring-black/[0.03]",

          props.disabled &&
            "cursor-not-allowed bg-[#f7f7f7] opacity-60",

          className,
        )}
        {...props}
      />
    );
  });
