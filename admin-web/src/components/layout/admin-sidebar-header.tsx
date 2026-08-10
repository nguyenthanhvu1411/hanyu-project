"use client";

import {
  PanelLeftClose,
  PanelLeftOpen,
} from "lucide-react";

import { Logo } from "@/components/branding/logo";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils/cn";
import { useSidebarStore } from "@/stores/sidebar.store";

export function AdminSidebarHeader() {
  const {
    collapsed,
    toggleCollapsed,
  } = useSidebarStore();

  return (
    <div
      className={cn(
        "flex h-[72px] shrink-0",
        "items-center",
        "border-b border-[#ede9e2]",

        collapsed
          ? "justify-center px-0"
          : "justify-between px-4",
      )}
    >
      <Logo
        showName={!collapsed}
      />

      {!collapsed && (
        <Button
          type="button"
          size="icon"
          variant="ghost"
          onClick={
            toggleCollapsed
          }
          className="
            h-9 w-9
            text-[#777]
          "
          aria-label="Thu gọn thanh bên"
        >
          <PanelLeftClose
            size={18}
          />
        </Button>
      )}

      {collapsed && (
        <button
          type="button"
          onClick={
            toggleCollapsed
          }
          className="
            absolute
            -right-[14px]
            top-[22px]
            flex h-7 w-7
            items-center
            justify-center
            rounded-full
            border
            border-[#e5e0d8]
            bg-white
            text-[#777]
            shadow-sm
            transition
            hover:text-[#ef241c]
          "
          aria-label="Mở rộng thanh bên"
        >
          <PanelLeftOpen
            size={15}
          />
        </button>
      )}
    </div>
  );
}
