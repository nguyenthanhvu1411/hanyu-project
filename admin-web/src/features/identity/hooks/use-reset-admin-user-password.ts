"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { identityService } from "../identity.service";
import { identityKeys } from "../identity.keys";
import type { ResetAdminUserPasswordRequest } from "@/dto/identity/admin-user.dto";

export function useResetAdminUserPassword(userId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: ResetAdminUserPasswordRequest) =>
      identityService.users.resetPassword(userId, request),
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: identityKeys.user(userId),
        }),
        queryClient.invalidateQueries({
          queryKey: identityKeys.sessions(),
        }),
      ]);
    },
  });
}
