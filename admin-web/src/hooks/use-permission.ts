"use client";

import { useAuthStore } from "@/stores/auth.store";
import {
  hasAllPermissions,
  hasAnyPermission,
  hasPermission,
} from "@/security/authorization";

export function usePermission() {
  const user = useAuthStore((state) => state.user);

  return {
    can(permission: string) {
      return hasPermission(user, permission);
    },

    canAny(permissions: string[]) {
      return hasAnyPermission(user, permissions);
    },

    canAll(permissions: string[]) {
      return hasAllPermissions(user, permissions);
    },
  };
}
