import {
  ADMIN_NAVIGATION,
} from "@/config/navigation.config";

import {
  AdminSidebarGroup,
} from "./admin-sidebar-group";

interface AdminSidebarContentProps {
  collapsed?: boolean;
}

export function AdminSidebarContent({
  collapsed = false,
}: AdminSidebarContentProps) {
  return (
    <div
      className="
        scrollbar-thin
        min-h-0
        flex-1
        overflow-y-auto
        px-[10px]
        py-4
      "
    >
      {ADMIN_NAVIGATION.map(
        (
          group,
          index,
        ) => (
          <AdminSidebarGroup
            key={
              group.title ??
              `group-${index}`
            }
            group={
              group
            }
            collapsed={
              collapsed
            }
          />
        ),
      )}
    </div>
  );
}
