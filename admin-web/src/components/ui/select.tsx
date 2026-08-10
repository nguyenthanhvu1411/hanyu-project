"use client";

import {
  Check,
  ChevronDown,
  X,
} from "lucide-react";

import {
  useEffect,
  useRef,
  useState,
} from "react";

import {
  cn,
} from "@/lib/utils/cn";

export interface SelectOption {
  label: string;

  value: string;

  description?: string;

  disabled?: boolean;
}

interface SelectProps {
  value?: string;

  onValueChange?: (
    value: string,
  ) => void;

  options: SelectOption[];

  placeholder?: string;

  disabled?: boolean;

  error?: boolean;

  clearable?: boolean;

  className?: string;
}

export function Select({
  value,
  onValueChange,
  options,
  placeholder = "Chọn dữ liệu",
  disabled = false,
  error = false,
  clearable = false,
  className,
}: SelectProps) {
  const [
    open,
    setOpen,
  ] = useState(false);

  const ref =
    useRef<HTMLDivElement>(
      null,
    );

  const selected =
    options.find(
      (item) =>
        item.value ===
        value,
    );

  useEffect(() => {
    function handleOutside(
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
      handleOutside,
    );

    return () =>
      document.removeEventListener(
        "mousedown",
        handleOutside,
      );
  }, []);

  return (
    <div
      ref={ref}
      className={cn(
        "relative w-full",
        open && "z-[90]",
        className,
      )}
    >
      <button
        type="button"
        disabled={disabled}
        onClick={() =>
          setOpen(
            (current) =>
              !current,
          )
        }
        className={cn(
          "flex",
          "h-[42px]",
          "w-full",
          "items-center",
          "justify-between",
          "gap-2",
          "rounded-[7px]",
          "border",
          "bg-white",
          "px-3",
          "text-left",
          "text-[12px]",
          "outline-none",
          "transition",

          error
            ? "border-[#ef453f]"
            : open
              ? "border-[#b9b3aa] ring-2 ring-black/[0.03]"
              : "border-[#dedbd6] hover:border-[#cbc6be]",

          disabled &&
            "cursor-not-allowed bg-[#f7f7f7] opacity-60",
        )}
      >
        <span
          className={cn(
            "min-w-0 flex-1 truncate",

            selected
              ? "text-[#3f3f3f]"
              : "text-[#9a9a9a]",
          )}
        >
          {selected
            ? selected.label
            : placeholder}
        </span>

        <div
          className="
            flex
            shrink-0
            items-center
            gap-1
          "
        >
          {clearable &&
            selected &&
            !disabled && (
              <span
                role="button"
                tabIndex={0}
                onClick={(
                  event,
                ) => {
                  event.stopPropagation();

                  onValueChange?.(
                    "",
                  );
                }}
                className="
                  flex h-6 w-6
                  items-center
                  justify-center
                  rounded
                  text-[#aaa]
                  hover:bg-[#f1f1f1]
                  hover:text-[#666]
                "
              >
                <X size={13} />
              </span>
            )}

          <ChevronDown
            size={15}
            className={cn(
              "text-[#888]",
              "transition-transform",

              open &&
                "rotate-180",
            )}
          />
        </div>
      </button>

      {open &&
        !disabled && (
          <div
            className="
              absolute
              left-0
              right-0
              top-[calc(100%+6px)]
              z-[90]
              max-h-[240px]
              overflow-y-auto
              rounded-[8px]
              border
              border-[#e1dcd5]
              bg-white
              p-1
              shadow-[0_12px_35px_rgba(0,0,0,0.12)]
            "
          >
            {options.length ===
            0 ? (
              <div
                className="
                  px-3 py-6
                  text-center
                  text-[11px]
                  text-[#999]
                "
              >
                Không có dữ liệu.
              </div>
            ) : (
              options.map(
                (
                  option,
                ) => {
                  const active =
                    value ===
                    option.value;

                  return (
                    <button
                      key={
                        option.value
                      }
                      type="button"
                      disabled={
                        option.disabled
                      }
                      onClick={() => {
                        onValueChange?.(
                          option.value,
                        );

                        setOpen(false);
                      }}
                      className={cn(
                        "flex",
                        "w-full",
                        "items-start",
                        "gap-2",
                        "rounded-[6px]",
                        "px-3",
                        "py-[8px]",
                        "text-left",
                        "transition",

                        active
                          ? "bg-[#fff0ee] text-[#ef241c]"
                          : "text-[#555] hover:bg-[#faf8f5]",

                        option.disabled &&
                          "cursor-not-allowed opacity-40",
                      )}
                    >
                      <div className="min-w-0 flex-1">
                        <div
                          className="
                            truncate
                            text-[12px]
                            font-medium
                          "
                        >
                          {
                            option.label
                          }
                        </div>

                        {option.description && (
                          <div
                            className="
                              mt-[2px]
                              text-[10px]
                              leading-[15px]
                              text-[#999]
                            "
                          >
                            {
                              option.description
                            }
                          </div>
                        )}
                      </div>

                      {active && (
                        <Check
                          size={14}
                          className="
                            mt-[1px]
                            shrink-0
                          "
                        />
                      )}
                    </button>
                  );
                },
              )
            )}
          </div>
        )}
    </div>
  );
}
