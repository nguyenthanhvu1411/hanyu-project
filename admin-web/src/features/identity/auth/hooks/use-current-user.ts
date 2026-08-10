"use client";

import { useQuery } from "@tanstack/react-query";
import { authKeys } from "../auth.keys";
import { authService } from "../auth.service";
import { useAuthStore } from "@/stores/auth.store";

export function useCurrentUser() {
  const authenticated = useAuthStore(
    (state) => state.status === "authenticated",
  );

  return useQuery({
    queryKey: authKeys.currentUser,
    queryFn: authService.loadCurrentUser,
    enabled: authenticated,
    staleTime: 60_000,
    retry: false,
  });
}
