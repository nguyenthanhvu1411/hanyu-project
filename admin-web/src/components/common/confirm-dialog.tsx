"use client";

import {
  AlertCircle,
  CheckCircle2,
  HelpCircle,
  Info,
  X,
} from "lucide-react";

import { useEffect } from "react";

import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils/cn";

type ConfirmDialogVariant =
  | "default"
  | "success"
  | "warning"
  | "danger";

interface ConfirmDialogProps {
  open: boolean;

  onOpenChange: (
    open: boolean,
  ) => void;

  title: string;

  description?: string;

  confirmText?: string;

  cancelText?: string;

  variant?: ConfirmDialogVariant;

  loading?: boolean;

  children?: React.ReactNode;

  onConfirm: () =>
    | void
    | Promise<void>;
}

const variants = {
  default: {
    icon: HelpCircle,

    iconClass:
      "bg-[#f3f5f7] text-[#60656d]",

    buttonVariant:
      "primary" as const,
  },

  success: {
    icon: CheckCircle2,

    iconClass:
      "bg-[#edf8f2] text-[#16975b]",

    buttonVariant:
      "secondary" as const,
  },

  warning: {
    icon: AlertCircle,

    iconClass:
      "bg-[#fff7e4] text-[#c18a21]",

    buttonVariant:
      "primary" as const,
  },

  danger: {
    icon: AlertCircle,

    iconClass:
      "bg-[#fff0ee] text-[#ef241c]",

    buttonVariant:
      "danger" as const,
  },
};

export function ConfirmDialog({
  open,
  onOpenChange,
  title,
  description,
  confirmText = "Xác nhận",
  cancelText = "Hủy",
  variant = "default",
  loading = false,
  children,
  onConfirm,
}: ConfirmDialogProps) {
  const config =
    variants[variant];

  const Icon =
    config.icon;

  useEffect(() => {
    if (!open) {
      return;
    }

    function handleKeyDown(
      event: KeyboardEvent,
    ) {
      if (
        event.key ===
          "Escape" &&
        !loading
      ) {
        onOpenChange(false);
      }
    }

    document.addEventListener(
      "keydown",
      handleKeyDown,
    );

    return () =>
      document.removeEventListener(
        "keydown",
        handleKeyDown,
      );
  }, [
    open,
    loading,
    onOpenChange,
  ]);

  if (!open) {
    return null;
  }

  async function handleConfirm() {
    await onConfirm();
  }

  return (
    <div
      className="
        fixed inset-0
        z-[120]
        flex items-center
        justify-center
        p-4
      "
    >
      <button
        type="button"
        aria-label="Đóng"
        disabled={loading}
        onClick={() =>
          onOpenChange(false)
        }
        className="
          absolute inset-0
          bg-black/40
          backdrop-blur-[1px]
        "
      />

      <div
        role="alertdialog"
        aria-modal="true"
        className="
          relative z-10
          w-full max-w-[440px]
          overflow-hidden
          rounded-[14px]
          border border-[#e7e2da]
          bg-white
          shadow-[0_24px_70px_rgba(0,0,0,0.18)]
        "
      >
        <div className="p-5">
          <div
            className="
              flex
              items-start
              gap-3
            "
          >
            <div
              className={cn(
                "flex h-10 w-10",
                "shrink-0",
                "items-center",
                "justify-center",
                "rounded-[9px]",
                config.iconClass,
              )}
            >
              <Icon
                size={20}
              />
            </div>

            <div className="min-w-0 flex-1">
              <h2
                className="
                  pr-7
                  text-[15px]
                  font-semibold
                  text-[#292929]
                "
              >
                {title}
              </h2>

              {description && (
                <p
                  className="
                    mt-[6px]
                    text-[12px]
                    leading-[19px]
                    text-[#757575]
                  "
                >
                  {description}
                </p>
              )}
            </div>

            <button
              type="button"
              disabled={loading}
              onClick={() =>
                onOpenChange(false)
              }
              className="
                absolute
                right-4 top-4
                flex h-8 w-8
                items-center
                justify-center
                rounded-[6px]
                text-[#999]
                transition
                hover:bg-[#f4f4f4]
                hover:text-[#444]
              "
            >
              <X size={17} />
            </button>
          </div>

          {children && (
            <div
              className="
                mt-4
                rounded-[8px]
                border
                border-[#ebe6df]
                bg-[#faf9f7]
                p-3
              "
            >
              {children}
            </div>
          )}
        </div>

        <div
          className="
            flex
            items-center
            justify-end
            gap-2
            border-t
            border-[#ebe6df]
            bg-[#faf9f7]
            px-5
            py-3
          "
        >
          <Button
            type="button"
            variant="outline"
            disabled={loading}
            onClick={() =>
              onOpenChange(false)
            }
            className="
              h-[38px]
              min-w-[80px]
              text-[12px]
            "
          >
            {cancelText}
          </Button>

          <Button
            type="button"
            variant={
              config.buttonVariant
            }
            loading={loading}
            onClick={
              handleConfirm
            }
            className="
              h-[38px]
              min-w-[90px]
              text-[12px]
            "
          >
            {confirmText}
          </Button>
        </div>
      </div>
    </div>
  );
}
