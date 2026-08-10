import type {
  AdminPermissionListQuery,
  AdminRoleListQuery,
  AdminSessionListQuery,
  AdminUserListQuery,
} from "./identity.types";

export const identityKeys = {
  all: [
    "identity",
  ] as const,

  users: () =>
    [
      ...identityKeys.all,
      "users",
    ] as const,

  userList: (
    query:
      AdminUserListQuery,
  ) =>
    [
      ...identityKeys.users(),
      "list",
      query,
    ] as const,

  user: (
    id: string,
  ) =>
    [
      ...identityKeys.users(),
      "detail",
      id,
    ] as const,

  roles: () =>
    [
      ...identityKeys.all,
      "roles",
    ] as const,

  roleList: (
    query:
      AdminRoleListQuery,
  ) =>
    [
      ...identityKeys.roles(),
      "list",
      query,
    ] as const,

  role: (
    id: string,
  ) =>
    [
      ...identityKeys.roles(),
      "detail",
      id,
    ] as const,

  permissions: () =>
    [
      ...identityKeys.all,
      "permissions",
    ] as const,

  permissionList: (
    query:
      AdminPermissionListQuery,
  ) =>
    [
      ...identityKeys.permissions(),
      "list",
      query,
    ] as const,

  permission: (
    id: string,
  ) =>
    [
      ...identityKeys.permissions(),
      "detail",
      id,
    ] as const,

  sessions: () =>
    [
      ...identityKeys.all,
      "sessions",
    ] as const,

  sessionList: (
    query:
      AdminSessionListQuery,
  ) =>
    [
      ...identityKeys.sessions(),
      "list",
      query,
    ] as const,

  session: (
    id: string,
  ) =>
    [
      ...identityKeys.sessions(),
      "detail",
      id,
    ] as const,
};
