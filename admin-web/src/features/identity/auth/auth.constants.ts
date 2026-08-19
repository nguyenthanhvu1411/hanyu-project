export const AUTH_ENDPOINTS = {
  LOGIN: "/auth/login",
  REFRESH: "/auth/refresh",
  LOGOUT: "/auth/logout",
  LOGOUT_ALL: "/auth/logout-all",
  CURRENT_USER: "/auth/me",
  CHANGE_PASSWORD: "/auth/change-password",
  SESSIONS: "/auth/sessions",
  REVOKE_SESSION: (sessionKey: string) => `/auth/sessions/${sessionKey}/revoke`,
  REVOKE_OTHER_SESSIONS: "/auth/sessions/revoke-others",
  SECURITY_EVENTS: (take = 50) => `/auth/security-events?take=${take}`,
  TWO_FACTOR_SETUP: "/auth/2fa/setup",
  TWO_FACTOR_ENABLE: "/auth/2fa/enable",
  TWO_FACTOR_DISABLE: "/auth/2fa/disable",
  TWO_FACTOR_RECOVERY_CODES: "/auth/2fa/recovery-codes",
  TWO_FACTOR_REGENERATE_KEY: "/auth/2fa/regenerate-key",
} as const;

export const AUTH_ROUTES = {
  LOGIN: "/dang-nhap",
  HOME: "/tong-quan",
  ACCESS_DENIED: "/khong-co-quyen",
} as const;
