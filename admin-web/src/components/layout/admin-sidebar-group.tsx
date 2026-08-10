import type {
  NavigationGroup,
} from "@/types/navigation.types";

import {
  AdminSidebarItem,
} from "./admin-sidebar-item";

import { useAuthStore } from "@/stores/auth.store";
import { hasPermission } from "@/security/authorization";

interface AdminSidebarGroupProps {
  group: NavigationGroup;
  collapsed?: boolean;
}

export function AdminSidebarGroup({
  group,
  collapsed = false,
}: AdminSidebarGroupProps) {
  const user = useAuthStore((state) => state.user);

  const visibleItems = group.items.filter(
    (item) => !item.permission || hasPermission(user, item.permission)
  );

  if (visibleItems.length === 0) {
    return null;
  }

  return (
    <div className="mb-4">
      {group.title &&
        !collapsed && (
          <div
            className="
              mb-[7px]
              px-3
              text-[11px]
              font-semibold
              uppercase
              tracking-[0.08em]
              text-[#a09d98]
            "
          >
            {
              group.title
            }
          </div>
        )}

      <div className="space-y-[3px]">
        {visibleItems.map(
          (item) => (
            <AdminSidebarItem
              key={
                item.href ??
                item.title
              }
              item={
                item
              }
              collapsed={
                collapsed
              }
            />
          ),
        )}
      </div>
    </div>
  );
}
