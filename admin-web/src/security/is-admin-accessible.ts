import type { AuthUser } from "@/features/identity/auth/auth.types";

export function isAdminAccessible(user: AuthUser | null) {
  if (!user) {
    return false;
  }

  if (user.status && user.status !== "active") {
    return false;
  }

  return user.permissions.length > 0 || user.roles.length > 0;
}
