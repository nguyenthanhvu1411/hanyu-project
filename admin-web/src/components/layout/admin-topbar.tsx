"use client";

import {
  Bell,
  Menu,
  Search,
} from "lucide-react";

import {
  useState,
} from "react";

import {
  Button,
} from "@/components/ui/button";

import {
  cn,
} from "@/lib/utils/cn";

import {
  useSidebarStore,
} from "@/stores/sidebar.store";

import {
  AdminUserMenu,
} from "./admin-user-menu";

import {
  NotificationMenu,
} from "./notification-menu";

export function AdminTopbar() {
  const collapsed =
    useSidebarStore(
      (
        state,
      ) =>
        state.collapsed,
    );

  const openMobile =
    useSidebarStore(
      (
        state,
      ) =>
        state.openMobile,
    );

  const [searchOpen, setSearchOpen] =
    useState(false);

  return (
    <header
      className={cn(
        "fixed",
        "right-0 top-0",
        "z-30",
        "flex h-[64px]",
        "items-center",
        "border-b",
        "border-[#e9e4dc]",
        "bg-white/95",
        "px-4",
        "backdrop-blur-md",
        "transition-[left]",
        "duration-200",
        "lg:px-6",

        collapsed
          ? "lg:left-[78px]"
          : "lg:left-[264px]",

        "left-0",
      )}
    >
      <div className="flex min-w-0 flex-1 items-center gap-3">
        <Button
          type="button"
          size="icon"
          variant="ghost"
          onClick={
            openMobile
          }
          className="h-9 w-9 lg:hidden"
        >
          <Menu
            size={20}
          />
        </Button>

        <div
          className={cn(
            "relative",
            "hidden",
            "w-full max-w-[430px]",
            "md:block",
          )}
        >
          <Search
            size={17}
            className="
              absolute
              left-3
              top-1/2
              -translate-y-1/2
              text-[#8c8c8c]
            "
          />

          <input
            type="text"
            placeholder="Tìm kiếm chức năng, dữ liệu..."
            className="
              h-[38px]
              w-full
              rounded-[8px]
              border
              border-[#e6e1da]
              bg-[#faf9f7]
              pl-10
              pr-4
              text-[13px]
              outline-none
              transition
              placeholder:text-[#9b9b9b]
              focus:border-[#d6d0c6]
              focus:bg-white
            "
          />
        </div>

        <Button
          type="button"
          size="icon"
          variant="ghost"
          onClick={() =>
            setSearchOpen(
              !searchOpen,
            )
          }
          className="h-9 w-9 md:hidden"
        >
          <Search
            size={19}
          />
        </Button>
      </div>

      <div className="flex items-center gap-1 sm:gap-2">
        <NotificationMenu />

        <AdminUserMenu />
      </div>

      {searchOpen && (
        <div
          className="
            absolute
            left-3
            right-3
            top-[69px]
            rounded-[10px]
            border
            border-[#e8e3dc]
            bg-white
            p-3
            shadow-lg
            md:hidden
          "
        >
          <div className="relative">
            <Search
              size={17}
              className="
                absolute
                left-3
                top-1/2
                -translate-y-1/2
                text-[#888]
              "
            />

            <input
              autoFocus
              placeholder="Tìm kiếm..."
              className="
                h-10
                w-full
                rounded-[7px]
                border
                border-[#dedbd6]
                pl-10
                pr-3
                text-[13px]
                outline-none
              "
            />
          </div>
        </div>
      )}
    </header>
  );
}
