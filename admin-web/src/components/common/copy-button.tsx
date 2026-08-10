"use client";

import {
  Check,
  Copy,
} from "lucide-react";

import {
  useState,
} from "react";

import {
  Tooltip,
} from "@/components/ui/tooltip";

interface CopyButtonProps {
  value: string;

  label?: string;
}

export function CopyButton({
  value,
  label = "Sao chép",
}: CopyButtonProps) {
  const [
    copied,
    setCopied,
  ] = useState(false);

  async function handleCopy() {
    await navigator.clipboard.writeText(
      value,
    );

    setCopied(true);

    setTimeout(
      () =>
        setCopied(
          false,
        ),
      1500,
    );
  }

  return (
    <Tooltip
      content={
        copied
          ? "Đã sao chép"
          : label
      }
    >
      <button
        type="button"
        onClick={
          handleCopy
        }
        className="
          flex h-8 w-8
          items-center
          justify-center
          rounded-[6px]
          text-[#777]
          transition
          hover:bg-[#f4f3f1]
          hover:text-[#ef241c]
        "
      >
        {copied ? (
          <Check
            size={14}
            className="text-[#16975b]"
          />
        ) : (
          <Copy
            size={14}
          />
        )}
      </button>
    </Tooltip>
  );
}
