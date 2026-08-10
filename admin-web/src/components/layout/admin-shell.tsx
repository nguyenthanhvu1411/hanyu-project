"use client";

import {
  cn,
} from "@/lib/utils/cn";

import {
  useSidebarStore,
} from "@/stores/sidebar.store";

import {
  AdminFooter,
} from "./admin-footer";

import {
  AdminSidebar,
} from "./admin-sidebar";

import {
  AdminTopbar,
} from "./admin-topbar";

import {
  MobileSidebar,
} from "./mobile-sidebar";

interface AdminShellProps {
  children: React.ReactNode;
}

export function AdminShell({
  children,
}: AdminShellProps) {
  const collapsed =
    useSidebarStore(
      (
        state,
      ) =>
        state.collapsed,
    );

  return (
    <div
      className="
        min-h-screen
        bg-[#f8f7f4]
      "
    >
      <AdminSidebar />

      <MobileSidebar />

      <AdminTopbar />

      <div
        className={cn(
          "flex",
          "min-h-screen",
          "flex-col",
          "pt-[64px]",
          "transition-[padding-left]",
          "duration-200",

          collapsed
            ? "lg:pl-[78px]"
            : "lg:pl-[264px]",
        )}
      >
        <main className="min-w-0 flex-1 overflow-visible">
          {children}
        </main>

        <AdminFooter />
      </div>
    </div>
  );
}
