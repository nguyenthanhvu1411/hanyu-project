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

interface DialogProps {
  open: boolean;

  onOpenChange: (
    open: boolean,
  ) => void;

  title?: string;

  description?: string;

  children: React.ReactNode;

  footer?: React.ReactNode;

  size?:
    | "sm"
    | "md"
    | "lg"
    | "xl";
}

const sizes = {
  sm:
    "max-w-[420px]",

  md:
    "max-w-[600px]",

  lg:
    "max-w-[820px]",

  xl:
    "max-w-[1080px]",
};

export function Dialog({
  open,
  onOpenChange,
  title,
  description,
  children,
  footer,
  size = "md",
}: DialogProps) {
  useEffect(() => {
    if (!open) {
      return;
    }

    function keydown(
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
      keydown,
    );

    return () =>
      document.removeEventListener(
        "keydown",
        keydown,
      );
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
        fixed inset-0
        z-[110]
        flex
        items-center
        justify-center
        p-4
      "
    >
      <button
        type="button"
        onClick={() =>
          onOpenChange(false)
        }
        className="
          absolute inset-0
          bg-black/40
          backdrop-blur-[1px]
        "
      />

      <section
        className={cn(
          "relative z-10",
          "flex",
          "max-h-[90vh]",
          "w-full",
          "flex-col",
          "overflow-hidden",
          "rounded-[14px]",
          "border",
          "border-[#e6e0d8]",
          "bg-white",
          "shadow-[0_25px_80px_rgba(0,0,0,0.18)]",
          sizes[size],
        )}
      >
        {(title ||
          description) && (
          <header
            className="
              flex
              items-start
              justify-between
              border-b
              border-[#ebe6df]
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
                    text-[#888]
                  "
                >
                  {description}
                </p>
              )}
            </div>

            <button
              type="button"
              onClick={() =>
                onOpenChange(
                  false,
                )
              }
              className="
                flex h-8 w-8
                items-center
                justify-center
                rounded-[6px]
                text-[#888]
                hover:bg-[#f2f2f2]
              "
            >
              <X size={17} />
            </button>
          </header>
        )}

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
              border-t
              border-[#ebe6df]
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
