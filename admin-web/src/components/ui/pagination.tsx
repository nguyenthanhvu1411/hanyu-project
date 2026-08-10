"use client";

import {
  ChevronLeft,
  ChevronRight,
  ChevronsLeft,
  ChevronsRight,
} from "lucide-react";

import {
  cn,
} from "@/lib/utils/cn";

interface PaginationProps {
  page: number;

  totalPages: number;

  onPageChange: (
    page: number,
  ) => void;

  siblingCount?: number;
}

export function Pagination({
  page,
  totalPages,
  onPageChange,
  siblingCount = 1,
}: PaginationProps) {
  const pages =
    buildPages(
      page,
      totalPages,
      siblingCount,
    );

  function go(
    nextPage: number,
  ) {
    if (
      nextPage < 1 ||
      nextPage >
        totalPages ||
      nextPage ===
        page
    ) {
      return;
    }

    onPageChange(
      nextPage,
    );
  }

  return (
    <div
      className="
        flex
        items-center
        gap-1
      "
    >
      <PageButton
        disabled={
          page === 1
        }
        onClick={() =>
          go(1)
        }
      >
        <ChevronsLeft
          size={14}
        />
      </PageButton>

      <PageButton
        disabled={
          page === 1
        }
        onClick={() =>
          go(page - 1)
        }
      >
        <ChevronLeft
          size={14}
        />
      </PageButton>

      {pages.map(
        (
          item,
          index,
        ) => {
          if (
            item ===
            "ellipsis"
          ) {
            return (
              <span
                key={`ellipsis-${index}`}
                className="
                  flex h-8 w-8
                  items-center
                  justify-center
                  text-[11px]
                  text-[#999]
                "
              >
                ...
              </span>
            );
          }

          return (
            <PageButton
              key={item}
              active={
                item ===
                page
              }
              onClick={() =>
                go(item)
              }
            >
              {item}
            </PageButton>
          );
        },
      )}

      <PageButton
        disabled={
          page ===
          totalPages
        }
        onClick={() =>
          go(page + 1)
        }
      >
        <ChevronRight
          size={14}
        />
      </PageButton>

      <PageButton
        disabled={
          page ===
          totalPages
        }
        onClick={() =>
          go(
            totalPages,
          )
        }
      >
        <ChevronsRight
          size={14}
        />
      </PageButton>
    </div>
  );
}

function PageButton({
  children,
  onClick,
  active = false,
  disabled = false,
}: {
  children: React.ReactNode;
  onClick: () => void;
  active?: boolean;
  disabled?: boolean;
}) {
  return (
    <button
      type="button"
      disabled={
        disabled
      }
      onClick={
        onClick
      }
      className={cn(
        "flex",
        "h-8",
        "min-w-8",
        "items-center",
        "justify-center",
        "rounded-[6px]",
        "border",
        "px-2",
        "text-[10px]",
        "transition",

        active
          ? "border-[#ef241c] bg-[#ef241c] text-white"
          : "border-[#ddd8d1] bg-white text-[#666] hover:bg-[#fafafa]",

        disabled &&
          "cursor-not-allowed opacity-40",
      )}
    >
      {children}
    </button>
  );
}

function buildPages(
  current: number,
  total: number,
  sibling: number,
): Array<
  number | "ellipsis"
> {
  if (
    total <= 7
  ) {
    return Array.from(
      {
        length: total,
      },
      (
        _,
        index,
      ) => index + 1,
    );
  }

  const result: Array<
    number | "ellipsis"
  > = [];

  result.push(1);

  const start =
    Math.max(
      2,
      current -
        sibling,
    );

  const end =
    Math.min(
      total - 1,
      current +
        sibling,
    );

  if (
    start > 2
  ) {
    result.push(
      "ellipsis",
    );
  }

  for (
    let i = start;
    i <= end;
    i++
  ) {
    result.push(i);
  }

  if (
    end <
    total - 1
  ) {
    result.push(
      "ellipsis",
    );
  }

  result.push(
    total,
  );

  return result;
}
