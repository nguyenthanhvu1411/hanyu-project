"use client";

import {
  ChevronDown,
} from "lucide-react";

import Link from "next/link";
import {
  usePathname,
} from "next/navigation";

import {
  useState,
} from "react";

import {
  cn,
} from "@/lib/utils/cn";

import type {
  NavigationItem,
} from "@/types/navigation.types";

interface AdminSidebarItemProps {
  item: NavigationItem;
  collapsed?: boolean;
  level?: number;
}

export function AdminSidebarItem({
  item,
  collapsed = false,
  level = 0,
}: AdminSidebarItemProps) {
  const pathname =
    usePathname();

  const [expanded, setExpanded] =
    useState(true);

  const hasChildren =
    Boolean(
      item.children?.length,
    );

  const isActive =
    item.href
      ? pathname ===
          item.href ||
        pathname.startsWith(
          `${item.href}/`,
        )
      : false;

  const Icon =
    item.icon;

  if (hasChildren) {
    return (
      <div>
        <button
          type="button"
          onClick={() =>
            setExpanded(
              (value) =>
                !value,
            )
          }
          className={cn(
            "group flex w-full",
            "items-center",
            "rounded-[8px]",
            "text-[14px]",
            "transition-colors",

            collapsed
              ? "h-[44px] justify-center px-0"
              : "h-[42px] gap-3 px-3",

            "text-[#5d6168]",
            "hover:bg-[#fff1ef]",
            "hover:text-[#ef241c]",
          )}
        >
          {Icon && (
            <Icon
              size={18}
              strokeWidth={1.8}
              className="shrink-0"
            />
          )}

          {!collapsed && (
            <>
              <span className="min-w-0 flex-1 truncate text-left">
                {item.title}
              </span>

              <ChevronDown
                size={15}
                className={cn(
                  "transition-transform",

                  expanded &&
                    "rotate-180",
                )}
              />
            </>
          )}
        </button>

        {!collapsed &&
          expanded && (
            <div className="ml-[18px] mt-1 border-l border-[#eee8df] pl-3">
              {item.children?.map(
                (
                  child,
                ) => (
                  <AdminSidebarItem
                    key={
                      child.href ??
                      child.title
                    }
                    item={
                      child
                    }
                    level={
                      level +
                      1
                    }
                  />
                ),
              )}
            </div>
          )}
      </div>
    );
  }

  return (
    <Link
      href={
        item.href ?? "#"
      }
      title={
        collapsed
          ? item.title
          : undefined
      }
      className={cn(
        "group relative flex",
        "items-center",
        "rounded-[8px]",
        "text-[14px]",
        "transition-all",

        collapsed
          ? "h-[44px] justify-center px-0"
          : "h-[42px] gap-3 px-3",

        isActive
          ? "bg-[#fff0ee] font-medium text-[#ef241c]"
          : "text-[#5d6168] hover:bg-[#fff7f5] hover:text-[#ef241c]",
      )}
    >
      {isActive && (
        <span
          className={cn(
            "absolute",
            "left-0",
            "h-[24px]",
            "w-[3px]",
            "rounded-r-full",
            "bg-[#ef241c]",

            collapsed &&
              "left-[-10px]",
          )}
        />
      )}

      {Icon && (
        <Icon
          size={18}
          strokeWidth={
            isActive
              ? 2
              : 1.8
          }
          className="shrink-0"
        />
      )}

      {!collapsed && (
        <span className="min-w-0 flex-1 truncate">
          {item.title}
        </span>
      )}

      {!collapsed &&
        item.badge !==
          undefined && (
          <span
            className="
              rounded-full
              bg-[#ef241c]
              px-2
              py-[2px]
              text-[10px]
              font-semibold
              text-white
            "
          >
            {
              item.badge
            }
          </span>
        )}
    </Link>
  );
}
