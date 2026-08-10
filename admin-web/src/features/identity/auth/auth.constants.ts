export const AUTH_ENDPOINTS = {
  LOGIN: "/auth/login",
  REFRESH: "/auth/refresh",
  LOGOUT: "/auth/logout",
  LOGOUT_ALL: "/auth/logout-all",
  CURRENT_USER: "/auth/me",
} as const;

export const AUTH_ROUTES = {
  LOGIN: "/dang-nhap",
  HOME: "/tong-quan",
  ACCESS_DENIED: "/khong-co-quyen",
} as const;
