"use client";

import { useCallback } from "react";
import { useRouter } from "next/navigation";
import { useQueryClient } from "@tanstack/react-query";
import { useAuthStore } from "@/stores/auth.store";
import { authService } from "../auth.service";
import { AUTH_ROUTES } from "../auth.constants";
import type { LoginCredentials } from "../auth.types";

export function useAuth() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const user = useAuthStore((state) => state.user);
  const status = useAuthStore((state) => state.status);
  const initialized = useAuthStore((state) => state.initialized);

  const login = useCallback(
    async (credentials: LoginCredentials, redirectTo: string = AUTH_ROUTES.HOME) => {
      const result = await authService.login(credentials);
      router.replace(redirectTo);
      return result;
    },
    [router],
  );

  const logout = useCallback(async () => {
    try {
      await authService.logout();
    } finally {
      queryClient.clear();
      router.replace(AUTH_ROUTES.LOGIN);
    }
  }, [queryClient, router]);

  const logoutAll = useCallback(async () => {
    try {
      await authService.logoutAll();
    } finally {
      queryClient.clear();
      router.replace(AUTH_ROUTES.LOGIN);
    }
  }, [queryClient, router]);

  return {
    user,
    status,
    initialized,
    authenticated: status === "authenticated",
    login,
    logout,
    logoutAll,
  };
}
