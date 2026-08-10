"use client";

import { AccessDenied } from "@/components/common/access-denied";
import { useAuthStore } from "@/stores/auth.store";
import { hasAllPermissions, hasAnyPermission } from "./authorization";

interface PermissionGuardProps {
  permission?: string;
  permissions?: string[];
  requireAll?: boolean;
  fallback?: React.ReactNode;
  children: React.ReactNode;
}

export function PermissionGuard({
  permission,
  permissions = [],
  requireAll = false,
  fallback,
  children,
}: PermissionGuardProps) {
  const user = useAuthStore((state) => state.user);

  const required = permission ? [permission, ...permissions] : permissions;

  if (required.length === 0) {
    return children;
  }

  const allowed = requireAll
    ? hasAllPermissions(user, required)
    : hasAnyPermission(user, required);

  if (!allowed) {
    return <>{fallback ?? <AccessDenied />}</>;
  }

  return children;
}
