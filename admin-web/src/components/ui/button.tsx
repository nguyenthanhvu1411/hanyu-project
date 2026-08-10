"use client";

import * as React from "react";

import { cn } from "@/lib/utils/cn";

type ButtonVariant =
  | "primary"
  | "secondary"
  | "outline"
  | "ghost"
  | "danger";

type ButtonSize =
  | "sm"
  | "md"
  | "lg"
  | "icon";

interface ButtonProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: ButtonVariant;
  size?: ButtonSize;
  loading?: boolean;
}

const variants: Record<ButtonVariant, string> = {
  primary:
    "bg-[#ef241c] text-white hover:bg-[#dc1f18]",

  secondary:
    "bg-[#19975b] text-white hover:bg-[#12804e]",

  outline:
    "border border-[#dedede] bg-white text-[#202124] hover:bg-[#fafafa]",

  ghost:
    "bg-transparent text-[#202124] hover:bg-[#f4f4f4]",

  danger:
    "bg-[#ef241c] text-white hover:bg-[#d51b16]",
};

const sizes: Record<ButtonSize, string> = {
  sm: "h-9 px-3 text-[13px]",
  md: "h-10 px-4 text-[14px]",
  lg: "h-12 px-5 text-[15px]",
  icon: "h-10 w-10",
};

export const Button = React.forwardRef<
  HTMLButtonElement,
  ButtonProps
>(function Button(
  {
    className,
    variant = "primary",
    size = "md",
    loading = false,
    disabled,
    children,
    ...props
  },
  ref,
) {
  return (
    <button
      ref={ref}
      disabled={disabled || loading}
      className={cn(
        "inline-flex items-center justify-center",
        "rounded-[7px]",
        "font-medium",
        "transition-colors duration-150",
        "outline-none",
        "focus-visible:ring-2",
        "focus-visible:ring-[#ef241c]/30",
        "disabled:pointer-events-none",
        "disabled:opacity-60",
        variants[variant],
        sizes[size],
        className,
      )}
      {...props}
    >
      {loading ? (
        <span className="flex items-center gap-2">
          <span
            className="
              h-4 w-4 animate-spin
              rounded-full border-2 border-white/40
              border-t-white
            "
          />

          Đang xử lý...
        </span>
      ) : (
        children
      )}
    </button>
  );
});
