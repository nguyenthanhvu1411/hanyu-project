"use client";

import { cn } from "@/lib/utils/cn";

export interface RadioOption {
  value: string;

  label: string;

  description?: string;

  disabled?: boolean;
}

interface RadioGroupProps {
  value?: string;

  onValueChange: (
    value: string,
  ) => void;

  options: RadioOption[];

  orientation?:
    | "horizontal"
    | "vertical";

  disabled?: boolean;
}

export function RadioGroup({
  value,
  onValueChange,
  options,
  orientation = "vertical",
  disabled = false,
}: RadioGroupProps) {
  return (
    <div
      role="radiogroup"
      className={cn(
        "flex gap-3",

        orientation ===
        "horizontal"
          ? "flex-row flex-wrap"
          : "flex-col",
      )}
    >
      {options.map(
        (option) => {
          const active =
            option.value ===
            value;

          const itemDisabled =
            disabled ||
            option.disabled;

          return (
            <button
              type="button"
              role="radio"
              aria-checked={
                active
              }
              key={
                option.value
              }
              disabled={
                itemDisabled
              }
              onClick={() =>
                onValueChange(
                  option.value,
                )
              }
              className={cn(
                "flex",
                "items-start",
                "gap-3",
                "rounded-[8px]",
                "text-left",
                "transition",

                orientation ===
                  "horizontal" &&
                  "min-w-[170px]",

                itemDisabled
                  ? "cursor-not-allowed opacity-50"
                  : "cursor-pointer",
              )}
            >
              <span
                className={cn(
                  "mt-[1px]",
                  "flex",
                  "h-[18px]",
                  "w-[18px]",
                  "shrink-0",
                  "items-center",
                  "justify-center",
                  "rounded-full",
                  "border",
                  "transition",

                  active
                    ? "border-[#16975b]"
                    : "border-[#cfcfcf]",
                )}
              >
                {active && (
                  <span
                    className="
                      h-[9px]
                      w-[9px]
                      rounded-full
                      bg-[#16975b]
                    "
                  />
                )}
              </span>

              <span>
                <span
                  className="
                    block
                    text-[12px]
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
                      text-[10px]
                      leading-[15px]
                      text-[#929292]
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
      )}
    </div>
  );
}
