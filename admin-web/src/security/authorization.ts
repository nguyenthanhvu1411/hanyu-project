import type { AuthUser } from "@/features/identity/auth/auth.types";

export function hasPermission(user: AuthUser | null, permission: string) {
  if (!user) {
    return false;
  }

  // Admin luôn có tất cả các quyền
  if (user.roles?.includes("SUPER_ADMIN")) {
    return true;
  }

  return user.permissions?.includes(permission) ?? false;
}

export function hasAnyPermission(
  user: AuthUser | null,
  permissions: string[],
) {
  if (!user) {
    return false;
  }

  if (permissions.length === 0) {
    return true;
  }

  if (user.roles?.includes("SUPER_ADMIN")) {
    return true;
  }

  return permissions.some((permission) => user.permissions?.includes(permission));
}

export function hasAllPermissions(
  user: AuthUser | null,
  permissions: string[],
) {
  if (!user) {
    return false;
  }

  if (user.roles?.includes("SUPER_ADMIN")) {
    return true;
  }

  return permissions.every((permission) => user.permissions?.includes(permission));
}

export function hasRole(user: AuthUser | null, role: string) {
  if (!user) {
    return false;
  }

  return user.roles?.includes(role) ?? false;
}

export function hasAnyRole(user: AuthUser | null, roles: string[]) {
  if (!user) {
    return false;
  }

  return roles.some((role) => user.roles?.includes(role));
}
