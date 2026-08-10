"use client";

import {
  X,
} from "lucide-react";

import {
  useEffect,
} from "react";

import {
  Button,
} from "@/components/ui/button";

interface FormDialogProps {
  open: boolean;

  onOpenChange: (
    open: boolean,
  ) => void;

  title: string;

  description?: string;

  children: React.ReactNode;

  submitText?: string;

  cancelText?: string;

  loading?: boolean;

  disabled?: boolean;

  onSubmit?: () =>
    | void
    | Promise<void>;

  size?:
    | "sm"
    | "md"
    | "lg";
}

const sizeClass = {
  sm: "max-w-[460px]",
  md: "max-w-[620px]",
  lg: "max-w-[820px]",
};

export function FormDialog({
  open,
  onOpenChange,
  title,
  description,
  children,
  submitText = "Lưu dữ liệu",
  cancelText = "Hủy",
  loading = false,
  disabled = false,
  onSubmit,
  size = "md",
}: FormDialogProps) {
  useEffect(() => {
    if (!open) {
      return;
    }

    const oldOverflow =
      document.body
        .style.overflow;

    document.body.style.overflow =
      "hidden";

    return () => {
      document.body.style.overflow =
        oldOverflow;
    };
  }, [open]);

  if (!open) {
    return null;
  }

  return (
    <div
      className="
        fixed
        inset-0
        z-[115]
        flex
        items-center
        justify-center
        p-4
      "
    >
      <button
        type="button"
        onClick={() => {
          if (!loading) {
            onOpenChange(
              false,
            );
          }
        }}
        className="
          absolute
          inset-0
          bg-black/40
          backdrop-blur-[1px]
        "
      />

      <section
        className={`
          relative
          z-10
          flex
          max-h-[90vh]
          w-full
          flex-col
          overflow-hidden
          rounded-[14px]
          border
          border-[#e6e0d8]
          bg-white
          shadow-[0_25px_80px_rgba(0,0,0,0.18)]
          ${sizeClass[size]}
        `}
      >
        <header
          className="
            flex
            shrink-0
            items-start
            justify-between
            gap-4
            border-b
            border-[#ebe6df]
            px-5
            py-4
          "
        >
          <div>
            <h2
              className="
                text-[15px]
                font-semibold
                text-[#292929]
              "
            >
              {title}
            </h2>

            {description && (
              <div
                className="
                  mt-[3px]
                  text-[10px]
                  leading-[16px]
                  text-[#888]
                "
              >
                {description}
              </div>
            )}
          </div>

          <button
            type="button"
            disabled={loading}
            onClick={() =>
              onOpenChange(false)
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

        <footer
          className="
            flex
            shrink-0
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
              text-[11px]
            "
          >
            {cancelText}
          </Button>

          <Button
            type="button"
            loading={loading}
            disabled={disabled}
            onClick={
              onSubmit
            }
            className="
              h-[38px]
              min-w-[100px]
              text-[11px]
            "
          >
            {submitText}
          </Button>
        </footer>
      </section>
    </div>
  );
}
