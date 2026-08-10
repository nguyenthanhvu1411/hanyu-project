"use client";

import { useEffect } from "react";
import { usePathname, useRouter } from "next/navigation";
import { PageLoading } from "@/components/common/page-loading";
import { useAuthStore } from "@/stores/auth.store";
import { AUTH_ROUTES } from "@/features/identity/auth/auth.constants";

export function AuthGuard({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const pathname = usePathname();
  const initialized = useAuthStore((state) => state.initialized);
  const status = useAuthStore((state) => state.status);

  useEffect(() => {
    if (!initialized) {
      return;
    }

    if (status !== "authenticated") {
      router.replace(
        `${AUTH_ROUTES.LOGIN}?next=${encodeURIComponent(pathname)}`,
      );
    }
  }, [initialized, pathname, router, status]);

  if (!initialized || status === "loading") {
    return <PageLoading text="Đang kiểm tra phiên đăng nhập..." />;
  }

  if (status !== "authenticated") {
    return null;
  }

  return children;
}
