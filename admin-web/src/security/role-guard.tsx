"use client";

import { AccessDenied } from "@/components/common/access-denied";
import { useAuthStore } from "@/stores/auth.store";
import { hasAnyRole, hasRole } from "./authorization";

interface RoleGuardProps {
  role?: string;
  roles?: string[];
  fallback?: React.ReactNode;
  children: React.ReactNode;
}

export function RoleGuard({
  role,
  roles = [],
  fallback,
  children,
}: RoleGuardProps) {
  const user = useAuthStore((state) => state.user);

  const allowed = role ? hasRole(user, role) : hasAnyRole(user, roles);

  if (!allowed) {
    return <>{fallback ?? <AccessDenied />}</>;
  }

  return children;
}
