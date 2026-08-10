"use client";

import {
  X,
} from "lucide-react";

import {
  useEffect,
} from "react";

import {
  cn,
} from "@/lib/utils/cn";

interface DrawerProps {
  open: boolean;

  onOpenChange: (
    open: boolean,
  ) => void;

  title?: string;

  description?: string;

  children: React.ReactNode;

  footer?: React.ReactNode;

  width?:
    | "sm"
    | "md"
    | "lg"
    | "xl";

  closeOnOverlay?: boolean;
}

const widths = {
  sm: "max-w-[420px]",
  md: "max-w-[560px]",
  lg: "max-w-[720px]",
  xl: "max-w-[920px]",
};

export function Drawer({
  open,
  onOpenChange,
  title,
  description,
  children,
  footer,
  width = "md",
  closeOnOverlay = true,
}: DrawerProps) {
  useEffect(() => {
    if (!open) {
      return;
    }

    const original =
      document.body
        .style.overflow;

    document.body.style.overflow =
      "hidden";

    function onKeyDown(
      event: KeyboardEvent,
    ) {
      if (
        event.key ===
        "Escape"
      ) {
        onOpenChange(false);
      }
    }

    document.addEventListener(
      "keydown",
      onKeyDown,
    );

    return () => {
      document.body.style.overflow =
        original;

      document.removeEventListener(
        "keydown",
        onKeyDown,
      );
    };
  }, [
    open,
    onOpenChange,
  ]);

  if (!open) {
    return null;
  }

  return (
    <div
      className="
        fixed
        inset-0
        z-[110]
      "
    >
      <button
        type="button"
        aria-label="Đóng"
        onClick={() => {
          if (
            closeOnOverlay
          ) {
            onOpenChange(
              false,
            );
          }
        }}
        className="
          absolute
          inset-0
          bg-black/35
          backdrop-blur-[1px]
        "
      />

      <section
        className={cn(
          "absolute",
          "bottom-0",
          "right-0",
          "top-0",
          "flex",
          "w-full",
          "flex-col",
          "bg-white",
          "shadow-[-20px_0_60px_rgba(0,0,0,0.12)]",
          widths[
            width
          ],
        )}
      >
        <header
          className="
            flex
            min-h-[68px]
            shrink-0
            items-start
            justify-between
            gap-4
            border-b
            border-[#eae5de]
            px-5
            py-4
          "
        >
          <div>
            {title && (
              <h2
                className="
                  text-[15px]
                  font-semibold
                  text-[#292929]
                "
              >
                {title}
              </h2>
            )}

            {description && (
              <p
                className="
                  mt-[3px]
                  text-[10px]
                  leading-[16px]
                  text-[#898989]
                "
              >
                {description}
              </p>
            )}
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
              rounded-[7px]
              text-[#888]
              hover:bg-[#f3f3f3]
            "
          >
            <X size={18} />
          </button>
        </header>

        <div
          className="
            scrollbar-thin
            flex-1
            overflow-y-auto
            p-5
          "
        >
          {children}
        </div>

        {footer && (
          <footer
            className="
              shrink-0
              border-t
              border-[#eae5de]
              bg-[#faf9f7]
              px-5
              py-3
            "
          >
            {footer}
          </footer>
        )}
      </section>
    </div>
  );
}
