"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuthStore } from "@/stores/auth.store";
import { AUTH_ROUTES } from "@/features/identity/auth/auth.constants";

export function GuestGuard({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const initialized = useAuthStore((state) => state.initialized);
  const status = useAuthStore((state) => state.status);

  useEffect(() => {
    if (initialized && status === "authenticated") {
      router.replace(AUTH_ROUTES.HOME);
    }
  }, [initialized, router, status]);

  if (!initialized) {
    return null;
  }

  // Giữ nguyên giao diện (không return null) để tránh chớp trắng trang
  // Next.js router.replace sẽ chạy ngầm và tự động chuyển khi tải xong trang đích.
  if (status === "authenticated") {
    return children;
  }

  return children;
}
