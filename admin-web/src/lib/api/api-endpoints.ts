export const API_ENDPOINTS = {
  ADMIN: {
    USERS: "/admin/users",

    USER: (
      id: string,
    ) =>
      `/admin/users/${id}`,

    USER_RESTORE: (
      id: string,
    ) =>
      `/admin/users/${id}/restore`,

    USER_LOCK: (
      id: string,
    ) =>
      `/admin/users/${id}/lock`,

    USER_UNLOCK: (
      id: string,
    ) =>
      `/admin/users/${id}/unlock`,

    USER_ROLES: (
      id: string,
    ) =>
      `/admin/users/${id}/roles`,

    USER_SESSIONS: (
      id: string,
    ) =>
      `/admin/users/${id}/sessions`,

    ROLES:
      "/admin/roles",

    ROLE: (
      id: string,
    ) =>
      `/admin/roles/${id}`,

    ROLE_RESTORE: (
      id: string,
    ) =>
      `/admin/roles/${id}/restore`,

    PERMISSIONS:
      "/admin/permissions",

    PERMISSION: (
      id: string,
    ) =>
      `/admin/permissions/${id}`,

    PERMISSION_RESTORE: (
      id: string,
    ) =>
      `/admin/permissions/${id}/restore`,

    SESSIONS:
      "/admin/sessions",

    SESSION: (
      id: string,
    ) =>
      `/admin/sessions/${id}`,

    SESSION_RESTORE: (
      id: string,
    ) =>
      `/admin/sessions/${id}/restore`,
  },

  LEARNING: {
    HSK_LEVELS: "/admin/hsk-levels",

    HSK_LEVEL: (
      id: number,
    ) =>
      `/admin/hsk-levels/${id}`,

    HSK_LEVEL_ACTIVATE: (
      id: number,
    ) =>
      `/admin/hsk-levels/${id}/activate`,

    HSK_LEVEL_DEACTIVATE: (
      id: number,
    ) =>
      `/admin/hsk-levels/${id}/deactivate`,
  },
} as const;
