import axios from "axios";
import { API_CONFIG } from "./api-config";
import type { RefreshTokenResponseDto } from "@/dto/identity/auth.dto";
import { AUTH_ENDPOINTS } from "@/features/identity/auth/auth.constants";
import { getAuthState } from "@/stores/auth.store";

const refreshClient = axios.create({
  baseURL: API_CONFIG.baseURL,
  timeout: API_CONFIG.timeout,
  withCredentials: true,
  headers: {
    Accept: "application/json",
  },
});

let refreshPromise: Promise<string> | null = null;

export function refreshAccessToken(): Promise<string> {
  if (refreshPromise) {
    return refreshPromise;
  }

  refreshPromise = performRefresh().finally(() => {
    refreshPromise = null;
  });

  return refreshPromise;
}

async function performRefresh(): Promise<string> {
  const currentRefreshToken = getAuthState().refreshToken;

  if (!currentRefreshToken) {
    throw new Error("Không tìm thấy refresh token trong bộ nhớ.");
  }

  const response = await refreshClient.post<
    RefreshTokenResponseDto
  >(AUTH_ENDPOINTS.REFRESH, { refreshToken: currentRefreshToken });

  const accessToken = response.data.accessToken;
  const newRefreshToken = response.data.refreshToken;

  if (!accessToken) {
    throw new Error("Refresh API không trả access token.");
  }

  getAuthState().setAccessToken(accessToken);
  if (newRefreshToken) {
    getAuthState().setRefreshToken(newRefreshToken);
  }

  return accessToken;
}

export function clearRefreshLock() {
  refreshPromise = null;
}
