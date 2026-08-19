"use client";

import { X } from "lucide-react";
import { useEffect, type ReactNode } from "react";

import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils/cn";

interface ModalProps {
  open: boolean;
  title: string;
  description?: string;
  children: ReactNode;
  footer?: ReactNode;
  onClose: () => void;
  size?: "sm" | "md" | "lg" | "xl";
  closeOnBackdrop?: boolean;
  className?: string;
}

const sizeClasses = {
  sm: "max-w-[520px]",
  md: "max-w-[720px]",
  lg: "max-w-[920px]",
  xl: "max-w-[1120px]",
};

export function Modal({
  open,
  title,
  description,
  children,
  footer,
  onClose,
  size = "md",
  closeOnBackdrop = true,
  className,
}: ModalProps) {
  useEffect(() => {
    if (!open) return;

    const previousOverflow = document.body.style.overflow;
    document.body.style.overflow = "hidden";

    const onKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Escape") onClose();
    };

    window.addEventListener("keydown", onKeyDown);
    return () => {
      window.removeEventListener("keydown", onKeyDown);
      document.body.style.overflow = previousOverflow;
    };
  }, [open, onClose]);

  if (!open) return null;

  return (
    <div
      className="fixed inset-0 z-[110] flex items-center justify-center overflow-y-auto bg-black/35 p-4"
      role="presentation"
      onMouseDown={() => {
        if (closeOnBackdrop) onClose();
      }}
    >
      <div
        role="dialog"
        aria-modal="true"
        aria-labelledby="foundation-modal-title"
        className={cn(
          "my-auto flex max-h-[calc(100vh-2rem)] w-full flex-col overflow-hidden rounded-[12px] border border-[#e7e2db] bg-white shadow-[0_24px_70px_rgba(0,0,0,0.20)]",
          sizeClasses[size],
          className,
        )}
        onMouseDown={(event) => event.stopPropagation()}
      >
        <div className="flex shrink-0 items-start justify-between gap-4 border-b border-[#eee9e2] px-5 py-4">
          <div className="min-w-0">
            <h2 id="foundation-modal-title" className="text-[16px] font-semibold text-[#2f2f2f]">
              {title}
            </h2>
            {description && <p className="mt-1 text-[13px] leading-5 text-[#777]">{description}</p>}
          </div>
          <Button type="button" variant="ghost" size="icon" aria-label="Đóng" title="Đóng" onClick={onClose}>
            <X size={16} />
          </Button>
        </div>

        <div className="min-h-0 flex-1 overflow-y-auto px-5 py-4">{children}</div>

        {footer && (
          <div className="shrink-0 border-t border-[#eee9e2] bg-[#fcfbf9] px-5 py-3.5">
            {footer}
          </div>
        )}
      </div>
    </div>
  );
}
