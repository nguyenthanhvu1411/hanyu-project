"use client";

import {
  X,
} from "lucide-react";

import {
  cn,
} from "@/lib/utils/cn";

interface SheetProps {
  open: boolean;

  onOpenChange: (
    open: boolean,
  ) => void;

  side?:
    | "left"
    | "right";

  title?: string;

  children: React.ReactNode;
}

export function Sheet({
  open,
  onOpenChange,
  side = "right",
  title,
  children,
}: SheetProps) {
  if (!open) {
    return null;
  }

  return (
    <div
      className="
        fixed
        inset-0
        z-[105]
      "
    >
      <button
        type="button"
        onClick={() =>
          onOpenChange(false)
        }
        className="
          absolute inset-0
          bg-black/30
        "
      />

      <aside
        className={cn(
          "absolute",
          "bottom-0",
          "top-0",
          "w-[300px]",
          "max-w-[86vw]",
          "bg-white",
          "shadow-xl",

          side ===
          "right"
            ? "right-0"
            : "left-0",
        )}
      >
        <div
          className="
            flex h-[60px]
            items-center
            justify-between
            border-b
            border-[#ebe6df]
            px-4
          "
        >
          <div
            className="
              text-[13px]
              font-semibold
            "
          >
            {title}
          </div>

          <button
            type="button"
            onClick={() =>
              onOpenChange(false)
            }
            className="
              flex h-8 w-8
              items-center
              justify-center
              rounded
              hover:bg-[#f2f2f2]
            "
          >
            <X size={17} />
          </button>
        </div>

        <div
          className="
            scrollbar-thin
            h-[calc(100vh-60px)]
            overflow-y-auto
            p-4
          "
        >
          {children}
        </div>
      </aside>
    </div>
  );
}
