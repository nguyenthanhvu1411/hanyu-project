"use client";

import {
  useState,
} from "react";

import {
  cn,
} from "@/lib/utils/cn";

interface TooltipProps {
  content: React.ReactNode;

  children: React.ReactNode;

  side?:
    | "top"
    | "bottom";

  className?: string;
}

export function Tooltip({
  content,
  children,
  side = "top",
  className,
}: TooltipProps) {
  const [
    visible,
    setVisible,
  ] = useState(false);

  return (
    <span
      className="relative inline-flex"
      onMouseEnter={() =>
        setVisible(
          true,
        )
      }
      onMouseLeave={() =>
        setVisible(
          false,
        )
      }
      onFocus={() =>
        setVisible(
          true,
        )
      }
      onBlur={() =>
        setVisible(
          false,
        )
      }
    >
      {children}

      {visible && (
        <span
          role="tooltip"
          className={cn(
            "pointer-events-none",
            "absolute",
            "left-1/2",
            "z-[150]",
            "w-max",
            "max-w-[250px]",
            "-translate-x-1/2",
            "rounded-[6px]",
            "bg-[#292929]",
            "px-2",
            "py-[5px]",
            "text-[10px]",
            "leading-[14px]",
            "text-white",
            "shadow-lg",

            side ===
            "top"
              ? "bottom-[calc(100%+7px)]"
              : "top-[calc(100%+7px)]",

            className,
          )}
        >
          {content}
        </span>
      )}
    </span>
  );
}
