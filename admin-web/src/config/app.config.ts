export const appConfig = {
  name:
    process.env.NEXT_PUBLIC_APP_NAME ??
    "Học Tiếng Trung",

  shortName:
    process.env.NEXT_PUBLIC_APP_SHORT_NAME ??
    "HanYu Admin",

  apiBaseUrl:
    process.env.NEXT_PUBLIC_API_BASE_URL ??
    "http://localhost:5000",

  apiVersion:
    process.env.NEXT_PUBLIC_API_VERSION ??
    "/api",

  googleLoginEnabled:
    process.env.NEXT_PUBLIC_ENABLE_GOOGLE_LOGIN === "true",
} as const;
