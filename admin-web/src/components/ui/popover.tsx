"use client";

import {
  useEffect,
  useRef,
  useState,
} from "react";

import {
  cn,
} from "@/lib/utils/cn";

interface PopoverProps {
  trigger: React.ReactNode;

  children: React.ReactNode;

  align?:
    | "left"
    | "right";

  className?: string;
}

export function Popover({
  trigger,
  children,
  align = "left",
  className,
}: PopoverProps) {
  const [
    open,
    setOpen,
  ] = useState(false);

  const ref =
    useRef<HTMLDivElement>(
      null,
    );

  useEffect(() => {
    function outside(
      event: MouseEvent,
    ) {
      if (
        ref.current &&
        !ref.current.contains(
          event.target as Node,
        )
      ) {
        setOpen(false);
      }
    }

    document.addEventListener(
      "mousedown",
      outside,
    );

    return () =>
      document.removeEventListener(
        "mousedown",
        outside,
      );
  }, []);

  return (
    <div
      ref={ref}
      className="relative inline-block"
    >
      <div
        onClick={() =>
          setOpen(
            !open,
          )
        }
      >
        {trigger}
      </div>

      {open && (
        <div
          className={cn(
            "absolute",
            "top-[calc(100%+6px)]",
            "z-50",
            "rounded-[9px]",
            "border",
            "border-[#e4dfd7]",
            "bg-white",
            "p-2",
            "shadow-[0_14px_40px_rgba(0,0,0,0.12)]",

            align ===
            "right"
              ? "right-0"
              : "left-0",

            className,
          )}
        >
          {children}
        </div>
      )}
    </div>
  );
}
