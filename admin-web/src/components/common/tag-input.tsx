"use client";

import {
  X,
} from "lucide-react";

import {
  useState,
} from "react";

import {
  cn,
} from "@/lib/utils/cn";

interface TagInputProps {
  value: string[];

  onChange: (
    tags: string[],
  ) => void;

  placeholder?: string;

  maxTags?: number;

  disabled?: boolean;

  error?: boolean;
}

export function TagInput({
  value,
  onChange,
  placeholder =
    "Nhập và nhấn Enter...",
  maxTags = 20,
  disabled = false,
  error = false,
}: TagInputProps) {
  const [
    input,
    setInput,
  ] = useState("");

  function addTag() {
    const tag =
      input.trim();

    if (
      !tag ||
      value.includes(
        tag,
      ) ||
      value.length >=
        maxTags
    ) {
      setInput("");

      return;
    }

    onChange([
      ...value,
      tag,
    ]);

    setInput("");
  }

  function remove(
    tag: string,
  ) {
    onChange(
      value.filter(
        (item) =>
          item !== tag,
      ),
    );
  }

  return (
    <div
      className={cn(
        "flex",
        "min-h-[42px]",
        "w-full",
        "flex-wrap",
        "items-center",
        "gap-1",
        "rounded-[7px]",
        "border",
        "bg-white",
        "px-2",
        "py-[5px]",

        error
          ? "border-[#ef453f]"
          : "border-[#dedbd6] focus-within:border-[#bbb5ad]",

        disabled &&
          "bg-[#f7f7f7] opacity-60",
      )}
    >
      {value.map(
        (tag) => (
          <span
            key={tag}
            className="
              inline-flex
              max-w-[180px]
              items-center
              gap-1
              rounded-[5px]
              bg-[#edf8f2]
              px-2
              py-[4px]
              text-[10px]
              text-[#147f4f]
            "
          >
            <span className="truncate">
              {tag}
            </span>

            {!disabled && (
              <button
                type="button"
                onClick={() =>
                  remove(
                    tag,
                  )
                }
              >
                <X
                  size={11}
                />
              </button>
            )}
          </span>
        ),
      )}

      <input
        value={input}
        disabled={
          disabled
        }
        placeholder={
          value.length ===
          0
            ? placeholder
            : ""
        }
        onChange={(
          event,
        ) =>
          setInput(
            event.target
              .value,
          )
        }
        onKeyDown={(
          event,
        ) => {
          if (
            event.key ===
              "Enter" ||
            event.key ===
              ","
          ) {
            event.preventDefault();

            addTag();
          }

          if (
            event.key ===
              "Backspace" &&
            !input &&
            value.length >
              0
          ) {
            remove(
              value[
                value.length -
                  1
              ],
            );
          }
        }}
        onBlur={
          addTag
        }
        className="
          h-7
          min-w-[150px]
          flex-1
          bg-transparent
          px-1
          text-[11px]
          outline-none
          placeholder:text-[#999]
        "
      />
    </div>
  );
}
