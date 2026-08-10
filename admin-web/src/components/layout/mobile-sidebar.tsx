"use client";

import {
  X,
} from "lucide-react";

import {
  Logo,
} from "@/components/branding/logo";

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
  AdminSidebarContent,
} from "./admin-sidebar-content";

import {
  AdminSidebarFooter,
} from "./admin-sidebar-footer";

export function MobileSidebar() {
  const {
    mobileOpen,
    closeMobile,
  } =
    useSidebarStore();

  return (
    <>
      <div
        onClick={
          closeMobile
        }
        className={cn(
          "fixed inset-0",
          "z-50",
          "bg-black/35",
          "transition-opacity",
          "lg:hidden",

          mobileOpen
            ? "pointer-events-auto opacity-100"
            : "pointer-events-none opacity-0",
        )}
      />

      <aside
        className={cn(
          "fixed inset-y-0 left-0",
          "z-[60]",
          "flex w-[280px]",
          "flex-col",
          "border-r",
          "border-[#e9e4dc]",
          "bg-[#fffdf9]",
          "shadow-xl",
          "transition-transform",
          "duration-200",
          "lg:hidden",

          mobileOpen
            ? "translate-x-0"
            : "-translate-x-full",
        )}
      >
        <div
          className="
            flex h-[68px]
            items-center
            justify-between
            border-b
            border-[#ece7df]
            px-4
          "
        >
          <Logo />

          <Button
            type="button"
            size="icon"
            variant="ghost"
            onClick={
              closeMobile
            }
            className="h-9 w-9"
          >
            <X
              size={19}
            />
          </Button>
        </div>

        <AdminSidebarContent />

        <AdminSidebarFooter />
      </aside>
    </>
  );
}
