"use client";

import { create } from "zustand";

import type {
  AuthStatus,
  AuthUser,
} from "@/features/identity/auth/auth.types";

import { persist, createJSONStorage } from "zustand/middleware";

interface AuthState {
  accessToken: string | null;
  refreshToken: string | null;
  user: AuthUser | null;
  status: AuthStatus;
  initialized: boolean;

  setAccessToken: (accessToken: string | null) => void;
  setRefreshToken: (refreshToken: string | null) => void;
  setUser: (user: AuthUser | null) => void;
  setAuthenticated: (accessToken: string, refreshToken: string, user?: AuthUser | null) => void;
  setStatus: (status: AuthStatus) => void;
  setInitialized: (initialized: boolean) => void;
  clearAuth: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      accessToken: null,
      refreshToken: null,
      user: null,
      status: "idle",
      initialized: false,

      setAccessToken: (accessToken) => set({ accessToken }),
      setRefreshToken: (refreshToken) => set({ refreshToken }),
      setUser: (user) => set({ user }),
      setAuthenticated: (accessToken, refreshToken, user = null) =>
        set({
          accessToken,
          refreshToken,
          user,
          status: "authenticated",
        }),
      setStatus: (status) => set({ status }),
      setInitialized: (initialized) => set({ initialized }),
      clearAuth: () =>
        set({
          accessToken: null,
          refreshToken: null,
          user: null,
          status: "unauthenticated",
        }),
    }),
    {
      name: "auth-storage",
      storage: createJSONStorage(() => localStorage),
      partialize: (state) => ({
        accessToken: state.accessToken,
        refreshToken: state.refreshToken,
        user: state.user,
        status: state.status === "loading" ? "idle" : state.status,
      }),
    }
  )
);

export function getAuthState() {
  return useAuthStore.getState();
}
