"use client";

import { useEffect, useRef } from "react";
import { authService } from "@/features/identity/auth/auth.service";
import { useAuthStore } from "@/stores/auth.store";

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const initialized = useAuthStore((state) => state.initialized);
  const setInitialized = useAuthStore((state) => state.setInitialized);
  const setStatus = useAuthStore((state) => state.setStatus);
  const clearAuth = useAuthStore((state) => state.clearAuth);
  const started = useRef(false);

  useEffect(() => {
    if (started.current) {
      return;
    }

    started.current = true;

    async function initialize() {
      setStatus("loading");

      try {
        await authService.restoreSession();
      } catch {
        clearAuth();
      } finally {
        setInitialized(true);
      }
    }

    void initialize();
  }, [clearAuth, setInitialized, setStatus]);

  if (!initialized) {
    return <AuthBootstrapLoading />;
  }

  return children;
}

function AuthBootstrapLoading() {
  return (
    <div className="flex min-h-screen items-center justify-center bg-[#fffdf8]">
      <div className="flex flex-col items-center gap-3">
        <div className="h-7 w-7 animate-spin rounded-full border-2 border-[#eadfda] border-t-[#ef241c]" />
        <span className="text-[11px] text-[#888]">Đang xác thực...</span>
      </div>
    </div>
  );
}
