export function hasPermission(
  userPermissions: string[],
  permission: string,
) {
  return userPermissions.includes(
    permission,
  );
}

export function hasAnyPermission(
  userPermissions: string[],
  requiredPermissions: string[],
) {
  if (
    requiredPermissions.length ===
    0
  ) {
    return true;
  }

  return requiredPermissions.some(
    (permission) =>
      userPermissions.includes(
        permission,
      ),
  );
}

export function hasAllPermissions(
  userPermissions: string[],
  requiredPermissions: string[],
) {
  return requiredPermissions.every(
    (permission) =>
      userPermissions.includes(
        permission,
      ),
  );
}

export function hasRole(
  roles: string[],
  role: string,
) {
  return roles.includes(
    role,
  );
}

export function hasAnyRole(
  roles: string[],
  requiredRoles: string[],
) {
  return requiredRoles.some(
    (role) =>
      roles.includes(
        role,
      ),
  );
}
