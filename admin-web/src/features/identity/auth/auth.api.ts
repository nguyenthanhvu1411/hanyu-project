import { apiClient } from "@/lib/api/api-client";
import { AUTH_ENDPOINTS } from "./auth.constants";
import type {
  CurrentUserDto,
  LoginRequest,
  LoginResponseDto,
  LogoutResponseDto,
} from "@/dto/identity/auth.dto";

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
      }
    );

    return response;
  },
};
