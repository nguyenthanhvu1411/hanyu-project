"use client";

import { useAuthStore } from "@/stores/auth.store";
import { hasAnyRole, hasRole } from "@/security/authorization";

export function useRole() {
  const user = useAuthStore((state) => state.user);

  return {
    hasRole(role: string) {
      return hasRole(user, role);
    },

    hasAnyRole(roles: string[]) {
      return hasAnyRole(user, roles);
    },
  };
}
