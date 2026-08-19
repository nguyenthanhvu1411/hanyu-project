import { apiClient } from "@/lib/api/api-client";
import { AUTH_ENDPOINTS } from "./auth.constants";
import type {
  CurrentUserDto,
  LoginRequest,
  LoginResponseDto,
  LogoutResponseDto,
} from "@/dto/identity/auth.dto";
import type {
  AuthSession,
  ChangePasswordRequest,
  SecurityEvent,
} from "./auth.types";

export const authApi = {
  async login(request: LoginRequest) {
    const response = await apiClient.post<LoginResponseDto>(
      AUTH_ENDPOINTS.LOGIN,
      request,
      {
        skipAuthRefresh: true,
      },
    );

    return response;
  },

  async currentUser() {
    const response = await apiClient.get<CurrentUserDto>(
      AUTH_ENDPOINTS.CURRENT_USER,
    );

    return response;
  },

  async logout(refreshToken: string) {
    const response = await apiClient.post<LogoutResponseDto>(
      AUTH_ENDPOINTS.LOGOUT,
      { refreshToken },
      {
        skipAuthRefresh: true,
      },
    );

    return response;
  },

  async logoutAll() {
    const response = await apiClient.post<LogoutResponseDto>(
      AUTH_ENDPOINTS.LOGOUT_ALL,
      undefined,
      {
        skipAuthRefresh: true,
      },
    );

    return response;
  },

  async changePassword(request: ChangePasswordRequest) {
    return apiClient.post<void>(AUTH_ENDPOINTS.CHANGE_PASSWORD, request);
  },

  async sessions() {
    return apiClient.get<AuthSession[]>(AUTH_ENDPOINTS.SESSIONS);
  },

  async revokeSession(sessionKey: string) {
    return apiClient.post<void>(AUTH_ENDPOINTS.REVOKE_SESSION(sessionKey));
  },

  async revokeOtherSessions() {
    return apiClient.post<void>(AUTH_ENDPOINTS.REVOKE_OTHER_SESSIONS);
  },

  async securityEvents(take = 50) {
    return apiClient.get<SecurityEvent[]>(AUTH_ENDPOINTS.SECURITY_EVENTS(take));
  },
};
