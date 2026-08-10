import { authApi } from "./auth.api";
import { mapCurrentUser } from "./auth.mapper";
import { refreshAccessToken } from "@/lib/api/refresh-token-manager";
import { getAuthState } from "@/stores/auth.store";
import type { LoginCredentials } from "./auth.types";

export const authService = {
  async login(credentials: LoginCredentials) {
    const response = await authApi.login(credentials);
    const accessToken = response.accessToken;
    const refreshToken = response.refreshToken;

    getAuthState().setAccessToken(accessToken);
    getAuthState().setRefreshToken(refreshToken);

    let user = response.user ? mapCurrentUser(response.user) : null;

    if (!user) {
      const current = await authApi.currentUser();
      user = mapCurrentUser(current);
    }

    getAuthState().setAuthenticated(accessToken, refreshToken, user);

    return user;
  },

  async loadCurrentUser() {
    const response = await authApi.currentUser();
    const user = mapCurrentUser(response);

    getAuthState().setUser(user);

    return user;
  },

  async restoreSession() {
    const token = await refreshAccessToken();
    const currentRefresh = getAuthState().refreshToken;
    const response = await authApi.currentUser();
    const user = mapCurrentUser(response);

    getAuthState().setAuthenticated(token, currentRefresh ?? "", user);

    return user;
  },

  async logout() {
    try {
      const refreshToken = getAuthState().refreshToken;
      if (refreshToken) {
        await authApi.logout(refreshToken);
      }
    } finally {
      getAuthState().clearAuth();
    }
  },

  async logoutAll() {
    try {
      await authApi.logoutAll();
    } finally {
      getAuthState().clearAuth();
    }
  },
};
