function getApiBaseUrl() {
  const value = process.env.NEXT_PUBLIC_API_BASE_URL;

  if (!value) {
    throw new Error("NEXT_PUBLIC_API_BASE_URL chưa được cấu hình.");
  }

  return value.replace(/\/+$/, "");
}

export const API_CONFIG = {
  baseURL: getApiBaseUrl(),
  timeout: 30_000,
} as const;
