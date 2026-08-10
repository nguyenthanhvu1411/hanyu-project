"use client";

import {
  LogOut,
  UserRound,
} from "lucide-react";

import Link from "next/link";

import {
  cn,
} from "@/lib/utils/cn";

import {
  ROUTES,
} from "@/constants/route.constants";

interface AdminSidebarFooterProps {
  collapsed?: boolean;
}

import { useAuthStore } from "@/stores/auth.store";
import { useAuth } from "@/features/identity/auth/hooks/use-auth";

export function AdminSidebarFooter({
  collapsed = false,
}: AdminSidebarFooterProps) {
  const user = useAuthStore((state) => state.user);
  const { logout } = useAuth();


  return (
    <div
      className="
        shrink-0
        border-t
        border-[#ece7df]
        p-[10px]
      "
    >
      <div
        className={cn(
          "rounded-[10px]",
          "bg-[#faf8f4]",

          collapsed
            ? "p-2"
            : "p-3",
        )}
      >
        <Link
          href={
            ROUTES.HO_SO
          }
          className={cn(
            "flex items-center",
            "rounded-[8px]",
            "transition",
            "hover:bg-white",

            collapsed
              ? "h-10 w-10 justify-center"
              : "gap-3 px-2 py-2",
          )}
        >
          <div
            className="
              flex h-9 w-9
              shrink-0
              items-center
              justify-center
              rounded-full
              bg-[#fff0ee]
              text-[#ef241c]
            "
          >
            <UserRound
              size={18}
            />
          </div>

          {!collapsed && (
            <div className="min-w-0 flex-1">
              <div
                className="
                  truncate
                  text-[13px]
                  font-semibold
                  text-[#282828]
                "
              >
                {user?.displayName || "Quản trị viên"}
              </div>

              <div
                className="
                  truncate
                  text-[11px]
                  text-[#8a8a8a]
                "
              >
                {user?.email || "admin@hanyu.vn"}
              </div>
            </div>
          )}
        </Link>

        {!collapsed && (
          <button
            type="button"
            onClick={() => { void logout(); }}
            className="
              mt-2
              flex h-9
              w-full
              items-center
              gap-2
              rounded-[7px]
              px-2
              text-[12px]
              text-[#666]
              transition
              hover:bg-[#fff0ee]
              hover:text-[#ef241c]
            "
          >
            <LogOut
              size={15}
            />

            Đăng xuất
          </button>
        )}
      </div>
    </div>
  );
}

