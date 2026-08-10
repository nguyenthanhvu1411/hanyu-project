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

export interface MultiSelectOption {
  value: string;

  label: string;

  description?: string;

  disabled?: boolean;
}

interface MultiSelectProps {
  value: string[];

  onValueChange: (
    value: string[],
  ) => void;

  options: MultiSelectOption[];

  placeholder?: string;

  searchPlaceholder?: string;

  emptyText?: string;

  disabled?: boolean;

  error?: boolean;

  maxVisibleTags?: number;
}

export function MultiSelect({
  value,
  onValueChange,
  options,
  placeholder = "Chọn dữ liệu",
  searchPlaceholder = "Tìm kiếm...",
  emptyText =
    "Không tìm thấy dữ liệu.",
  disabled = false,
  error = false,
  maxVisibleTags = 3,
}: MultiSelectProps) {
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

  const selectedOptions =
    options.filter(
      (option) =>
        value.includes(
          option.value,
        ),
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

  function toggle(
    optionValue: string,
  ) {
    if (
      value.includes(
        optionValue,
      )
    ) {
      onValueChange(
        value.filter(
          (item) =>
            item !==
            optionValue,
        ),
      );

      return;
    }

    onValueChange([
      ...value,
      optionValue,
    ]);
  }

  function remove(
    optionValue: string,
  ) {
    onValueChange(
      value.filter(
        (item) =>
          item !==
          optionValue,
      ),
    );
  }

  return (
    <div
      ref={ref}
      className={cn(
        "relative z-[1]",
        open && "z-[70]"
      )}
    >
      <button
        type="button"
        disabled={
          disabled
        }
        onClick={() =>
          setOpen(
            !open,
          )
        }
        className={cn(
          "flex",
          "min-h-[42px]",
          "w-full",
          "items-center",
          "gap-2",
          "rounded-[7px]",
          "border",
          "bg-white",
          "px-2",
          "py-[5px]",
          "text-left",
          "transition",

          error
            ? "border-[#ef453f]"
            : open
              ? "border-[#bbb5ac]"
              : "border-[#dedbd6]",

          disabled &&
            "cursor-not-allowed bg-[#f7f7f7] opacity-60",
        )}
      >
        <div
          className="
            flex
            min-w-0
            flex-1
            flex-wrap
            items-center
            gap-1
          "
        >
          {selectedOptions.length ===
          0 ? (
            <span
              className="
                px-1
                text-[12px]
                text-[#999]
              "
            >
              {placeholder}
            </span>
          ) : (
            <>
              {selectedOptions
                .slice(
                  0,
                  maxVisibleTags,
                )
                .map(
                  (
                    option,
                  ) => (
                    <span
                      key={
                        option.value
                      }
                      className="
                        inline-flex
                        max-w-[160px]
                        items-center
                        gap-1
                        rounded-[5px]
                        bg-[#fff0ee]
                        px-2
                        py-[4px]
                        text-[10px]
                        text-[#d9332c]
                      "
                    >
                      <span className="truncate">
                        {
                          option.label
                        }
                      </span>

                      <span
                        role="button"
                        onClick={(
                          event,
                        ) => {
                          event.stopPropagation();

                          remove(
                            option.value,
                          );
                        }}
                      >
                        <X
                          size={11}
                        />
                      </span>
                    </span>
                  ),
                )}

              {selectedOptions.length >
                maxVisibleTags && (
                <span
                  className="
                    rounded-[5px]
                    bg-[#f1f1f1]
                    px-2
                    py-[4px]
                    text-[10px]
                    text-[#666]
                  "
                >
                  +
                  {selectedOptions.length -
                    maxVisibleTags}
                </span>
              )}
            </>
          )}
        </div>

        <ChevronDown
          size={15}
          className={cn(
            "shrink-0",
            "text-[#888]",
            "transition",

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
              left-0
              right-0
              top-[calc(100%+6px)]
              z-[80]
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
                  size={14}
                  className="
                    absolute
                    left-3
                    top-1/2
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
                    h-9
                    w-full
                    rounded-[6px]
                    border
                    border-[#dedbd6]
                    bg-[#faf9f7]
                    pl-9
                    pr-3
                    text-[11px]
                    outline-none
                  "
                />
              </div>
            </div>

            <div
              className="
                scrollbar-thin
                max-h-[260px]
                overflow-y-auto
                p-1
              "
            >
              {filtered.length ===
              0 ? (
                <div
                  className="
                    py-7
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
                      value.includes(
                        option.value,
                      );

                    return (
                      <button
                        key={
                          option.value
                        }
                        type="button"
                        disabled={
                          option.disabled
                        }
                        onClick={() =>
                          toggle(
                            option.value,
                          )
                        }
                        className={cn(
                          "flex",
                          "w-full",
                          "items-start",
                          "gap-2",
                          "rounded-[6px]",
                          "px-3",
                          "py-2",
                          "text-left",
                          "transition",

                          active
                            ? "bg-[#fff0ee]"
                            : "hover:bg-[#faf9f7]",

                          option.disabled &&
                            "opacity-40",
                        )}
                      >
                        <span
                          className={cn(
                            "mt-[1px]",
                            "flex",
                            "h-4",
                            "w-4",
                            "shrink-0",
                            "items-center",
                            "justify-center",
                            "rounded-[4px]",
                            "border",

                            active
                              ? "border-[#16975b] bg-[#16975b] text-white"
                              : "border-[#ccc]",
                          )}
                        >
                          {active && (
                            <Check
                              size={11}
                            />
                          )}
                        </span>

                        <span className="min-w-0 flex-1">
                          <span
                            className="
                              block
                              truncate
                              text-[11px]
                              font-medium
                              text-[#444]
                            "
                          >
                            {
                              option.label
                            }
                          </span>

                          {option.description && (
                            <span
                              className="
                                mt-[2px]
                                block
                                truncate
                                text-[9px]
                                text-[#999]
                              "
                            >
                              {
                                option.description
                              }
                            </span>
                          )}
                        </span>
                      </button>
                    );
                  },
                )
              )}
            </div>

            {value.length >
              0 && (
              <div
                className="
                  flex
                  items-center
                  justify-between
                  border-t
                  border-[#eee9e2]
                  px-3
                  py-2
                "
              >
                <span
                  className="
                    text-[9px]
                    text-[#999]
                  "
                >
                  Đã chọn{" "}
                  {
                    value.length
                  }
                </span>

                <button
                  type="button"
                  onClick={() =>
                    onValueChange(
                      [],
                    )
                  }
                  className="
                    text-[10px]
                    text-[#ef241c]
                  "
                >
                  Bỏ chọn tất cả
                </button>
              </div>
            )}
          </div>
        )}
    </div>
  );
}
