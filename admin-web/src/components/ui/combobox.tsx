"use client";

import {
  Check,
  ChevronDown,
  Search,
  X,
} from "lucide-react";

import {
  useEffect,
  useMemo,
  useRef,
  useState,
} from "react";

import {
  cn,
} from "@/lib/utils/cn";

export interface ComboboxOption {
  label: string;

  value: string;

  description?: string;

  disabled?: boolean;
}

interface ComboboxProps {
  value?: string;

  onValueChange?: (
    value: string,
  ) => void;

  options: ComboboxOption[];

  placeholder?: string;

  searchPlaceholder?: string;

  emptyText?: string;

  disabled?: boolean;

  error?: boolean;

  clearable?: boolean;

  className?: string;
}

export function Combobox({
  value,
  onValueChange,
  options,
  placeholder = "Chọn dữ liệu",
  searchPlaceholder = "Tìm kiếm...",
  emptyText = "Không tìm thấy dữ liệu.",
  disabled = false,
  error = false,
  clearable = true,
  className,
}: ComboboxProps) {
  const [
    open,
    setOpen,
  ] = useState(false);

  const [
    search,
    setSearch,
  ] = useState("");

  const ref =
    useRef<HTMLDivElement>(
      null,
    );

  const selected =
    options.find(
      (option) =>
        option.value ===
        value,
    );

  const filtered =
    useMemo(() => {
      const keyword =
        search
          .trim()
          .toLowerCase();

      if (!keyword) {
        return options;
      }

      return options.filter(
        (option) =>
          option.label
            .toLowerCase()
            .includes(
              keyword,
            ) ||
          option.description
            ?.toLowerCase()
            .includes(
              keyword,
            ),
      );
    }, [
      options,
      search,
    ]);

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

  useEffect(() => {
    if (!open) {
      setSearch("");
    }
  }, [open]);

  return (
    <div
      ref={ref}
      className={cn(
        "relative",
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
          "flex h-[42px]",
          "w-full",
          "items-center",
          "gap-2",
          "rounded-[7px]",
          "border",
          "bg-white",
          "px-3",
          "text-left",
          "text-[12px]",
          "transition",

          error
            ? "border-[#ef453f]"
            : open
              ? "border-[#bab4ab] ring-2 ring-black/[0.03]"
              : "border-[#dedbd6]",

          disabled &&
            "cursor-not-allowed bg-[#f7f7f7] opacity-60",
        )}
      >
        <span
          className={cn(
            "min-w-0 flex-1 truncate",

            selected
              ? "text-[#444]"
              : "text-[#999]",
          )}
        >
          {selected
            ? selected.label
            : placeholder}
        </span>

        {selected &&
          clearable &&
          !disabled && (
            <span
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
                hover:bg-[#f2f2f2]
              "
            >
              <X size={13} />
            </span>
          )}

        <ChevronDown
          size={15}
          className={cn(
            "shrink-0 text-[#888]",
            "transition-transform",

            open &&
              "rotate-180",
          )}
        />
      </button>

      {open &&
        !disabled && (
          <div
            className="
              absolute
              left-0 right-0
              top-[46px]
              z-50
              overflow-hidden
              rounded-[9px]
              border
              border-[#e4dfd7]
              bg-white
              shadow-[0_14px_40px_rgba(0,0,0,0.12)]
            "
          >
            <div
              className="
                border-b
                border-[#eee9e2]
                p-2
              "
            >
              <div className="relative">
                <Search
                  size={15}
                  className="
                    absolute
                    left-3 top-1/2
                    -translate-y-1/2
                    text-[#999]
                  "
                />

                <input
                  autoFocus
                  value={search}
                  onChange={(
                    event,
                  ) =>
                    setSearch(
                      event
                        .target
                        .value,
                    )
                  }
                  placeholder={
                    searchPlaceholder
                  }
                  className="
                    h-9 w-full
                    rounded-[6px]
                    border
                    border-[#dedbd6]
                    bg-[#faf9f7]
                    pl-9 pr-3
                    text-[11px]
                    outline-none
                    focus:bg-white
                  "
                />
              </div>
            </div>

            <div
              className="
                scrollbar-thin
                max-h-[240px]
                overflow-y-auto
                p-1
              "
            >
              {filtered.length ===
              0 ? (
                <div
                  className="
                    px-4 py-7
                    text-center
                    text-[11px]
                    text-[#999]
                  "
                >
                  {emptyText}
                </div>
              ) : (
                filtered.map(
                  (
                    option,
                  ) => {
                    const active =
                      option.value ===
                      value;

                    return (
                      <button
                        type="button"
                        key={
                          option.value
                        }
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
                          "flex w-full",
                          "items-start",
                          "gap-2",
                          "rounded-[6px]",
                          "px-3 py-2",
                          "text-left",
                          "transition",

                          active
                            ? "bg-[#fff0ee] text-[#ef241c]"
                            : "text-[#555] hover:bg-[#faf9f7]",

                          option.disabled &&
                            "opacity-40",
                        )}
                      >
                        <div className="min-w-0 flex-1">
                          <div
                            className="
                              truncate
                              text-[11px]
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
                                truncate
                                text-[10px]
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
                            className="shrink-0"
                          />
                        )}
                      </button>
                    );
                  },
                )
              )}
            </div>
          </div>
        )}
    </div>
  );
}
