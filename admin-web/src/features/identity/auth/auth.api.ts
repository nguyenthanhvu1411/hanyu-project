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
  DisableTwoFactorRequest,
  EnableTwoFactorRequest,
  PasswordConfirmationRequest,
  SecurityEvent,
  TwoFactorRecoveryCodesResponse,
  TwoFactorSetupResponse,
} from "./auth.types";

export const authApi = {
  async login(request: LoginRequest) {
    return apiClient.post<LoginResponseDto>(AUTH_ENDPOINTS.LOGIN, request, { skipAuthRefresh: true });
  },

  async currentUser() {
    return apiClient.get<CurrentUserDto>(AUTH_ENDPOINTS.CURRENT_USER);
  },

  async logout(refreshToken: string) {
    return apiClient.post<LogoutResponseDto>(AUTH_ENDPOINTS.LOGOUT, { refreshToken }, { skipAuthRefresh: true });
  },

  async logoutAll() {
    return apiClient.post<LogoutResponseDto>(AUTH_ENDPOINTS.LOGOUT_ALL, undefined, { skipAuthRefresh: true });
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

  async setupTwoFactor() {
    return apiClient.post<TwoFactorSetupResponse>("/auth/2fa/setup");
  },

  async enableTwoFactor(request: EnableTwoFactorRequest) {
    return apiClient.post<void>("/auth/2fa/enable", request);
  },

  async disableTwoFactor(request: DisableTwoFactorRequest) {
    return apiClient.post<void>("/auth/2fa/disable", request);
  },

  async generateRecoveryCodes(request: PasswordConfirmationRequest) {
    return apiClient.post<TwoFactorRecoveryCodesResponse>("/auth/2fa/recovery-codes", request);
  },

  async regenerateAuthenticatorKey(request: PasswordConfirmationRequest) {
    return apiClient.post<TwoFactorSetupResponse>("/auth/2fa/regenerate-key", request);
  },
};
