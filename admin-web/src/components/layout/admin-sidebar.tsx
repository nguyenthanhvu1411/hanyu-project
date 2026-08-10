"use client";

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

import {
  AdminSidebarHeader,
} from "./admin-sidebar-header";

export function AdminSidebar() {
  const collapsed =
    useSidebarStore(
      (
        state,
      ) =>
        state.collapsed,
    );

  return (
    <aside
      className={cn(
        "fixed",
        "inset-y-0 left-0",
        "z-40",
        "hidden",
        "flex-col",
        "border-r",
        "border-[#e9e4dc]",
        "bg-[#fffdf9]",
        "transition-[width]",
        "duration-200",
        "lg:flex",

        collapsed
          ? "w-[78px]"
          : "w-[264px]",
      )}
    >
      <AdminSidebarHeader />

      <AdminSidebarContent
        collapsed={
          collapsed
        }
      />

      <AdminSidebarFooter
        collapsed={
          collapsed
        }
      />
    </aside>
  );
}
