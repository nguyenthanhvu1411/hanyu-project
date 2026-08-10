"use client";

import {
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";

import {
  identityKeys,
} from "../identity.keys";

import {
  identityService,
} from "../identity.service";

import type {
  AdminSessionListQuery,
} from "../identity.types";

export function useAdminSessions(
  query:
    AdminSessionListQuery,
) {
  return useQuery({
    queryKey:
      identityKeys.sessionList(
        query,
      ),

    queryFn: () =>
      identityService.sessions.list(
        query,
      ),
  });
}

export function useAdminSession(
  id?: string,
) {
  return useQuery({
    queryKey:
      identityKeys.session(
        id ?? "",
      ),

    queryFn: () =>
      identityService.sessions.get(
        id!,
      ),

    enabled:
      Boolean(id),
  });
}

export function useDeleteAdminSession() {
  const client =
    useQueryClient();

  return useMutation({
    mutationFn:
      identityService.sessions.remove,

    onSuccess: async () => {
      await client.invalidateQueries({
        queryKey:
          identityKeys.sessions(),
      });
    },
  });
}
