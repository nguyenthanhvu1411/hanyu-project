"use client";

import {
  AccessDenied,
} from "./access-denied";

interface PermissionGuardProps {
  permission?: string;

  permissions?: string[];

  userPermissions?: string[];

  requireAll?: boolean;

  fallback?: React.ReactNode;

  children: React.ReactNode;
}

export function PermissionGuard({
  permission,
  permissions = [],
  userPermissions = [],
  requireAll = false,
  fallback,
  children,
}: PermissionGuardProps) {
  const required =
    permission
      ? [
          permission,
          ...permissions,
        ]
      : permissions;

  if (
    required.length ===
    0
  ) {
    return children;
  }

  const allowed =
    requireAll
      ? required.every(
          (item) =>
            userPermissions.includes(
              item,
            ),
        )
      : required.some(
          (item) =>
            userPermissions.includes(
              item,
            ),
        );

  if (!allowed) {
    return (
      fallback ?? (
        <AccessDenied />
      )
    );
  }

  return children;
}
