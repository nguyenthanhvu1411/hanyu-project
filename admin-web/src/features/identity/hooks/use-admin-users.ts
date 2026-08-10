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
  AdminUserListQuery,
} from "../identity.types";

import type {
  CreateAdminUserRequest,
  LockUserRequest,
  ReplaceUserRolesRequest,
  UnlockUserRequest,
  UpdateAdminUserRequest,
  DeleteUserRequest,
} from "@/dto/identity/admin-user.dto";

export function useAdminUsers(
  query:
    AdminUserListQuery,
) {
  return useQuery({
    queryKey:
      identityKeys.userList(
        query,
      ),

    queryFn: () =>
      identityService.users.list(
        query,
      ),
  });
}

export function useAdminUser(
  id?: string,
) {
  return useQuery({
    queryKey:
      identityKeys.user(
        id ?? "",
      ),

    queryFn: () =>
      identityService.users.get(
        id!,
      ),

    enabled:
      Boolean(id),
  });
}

export function useCreateAdminUser() {
  const client =
    useQueryClient();

  return useMutation({
    mutationFn: (
      request:
        CreateAdminUserRequest,
    ) =>
      identityService.users.create(
        request,
      ),

    onSuccess: async () => {
      await client.invalidateQueries({
        queryKey:
          identityKeys.users(),
      });
    },
  });
}

export function useUpdateAdminUser(
  id: string,
) {
  const client =
    useQueryClient();

  return useMutation({
    mutationFn: (
      request:
        UpdateAdminUserRequest,
    ) =>
      identityService.users.update(
        id,
        request,
      ),

    onSuccess: async () => {
      await Promise.all([
        client.invalidateQueries({
          queryKey:
            identityKeys.users(),
        }),

        client.invalidateQueries({
          queryKey:
            identityKeys.user(
              id,
            ),
        }),
      ]);
    },
  });
}

export function useDeleteAdminUser() {
  const client =
    useQueryClient();

  return useMutation({
    mutationFn: ({ id, request }: { id: string; request: DeleteUserRequest }) =>
      identityService.users.remove(id, request),

    onSuccess: async () => {
      await client.invalidateQueries({
        queryKey:
          identityKeys.users(),
      });
    },
  });
}

export function useRestoreAdminUser() {
  const client =
    useQueryClient();

  return useMutation({
    mutationFn:
      identityService.users.restore,

    onSuccess:
      async (
        response,
        id,
      ) => {
        await Promise.all([
          client.invalidateQueries({
            queryKey:
              identityKeys.users(),
          }),

          client.invalidateQueries({
            queryKey:
              identityKeys.user(
                id,
              ),
          }),
        ]);
      },
  });
}

export function useLockAdminUser(
  id: string,
) {
  const client =
    useQueryClient();

  return useMutation({
    mutationFn: (
      request:
        LockUserRequest,
    ) =>
      identityService.users.lock(
        id,
        request,
      ),

    onSuccess:
      async () => {
        await Promise.all([
          client.invalidateQueries({
            queryKey:
              identityKeys.users(),
          }),

          client.invalidateQueries({
            queryKey:
              identityKeys.user(
                id,
              ),
          }),

          client.invalidateQueries({
            queryKey:
              identityKeys.sessions(),
          }),
        ]);
      },
  });
}

export function useUnlockAdminUser(
  id: string,
) {
  const client =
    useQueryClient();

  return useMutation({
    mutationFn: (
      request:
        UnlockUserRequest,
    ) =>
      identityService.users.unlock(
        id,
        request,
      ),

    onSuccess:
      async () => {
        await Promise.all([
          client.invalidateQueries({
            queryKey:
              identityKeys.users(),
          }),

          client.invalidateQueries({
            queryKey:
              identityKeys.user(
                id,
              ),
          }),
        ]);
      },
  });
}

export function useReplaceUserRoles(
  id: string,
) {
  const client =
    useQueryClient();

  return useMutation({
    mutationFn: (
      request:
        ReplaceUserRolesRequest,
    ) =>
      identityService.users.replaceRoles(
        id,
        request,
      ),

    onSuccess:
      async () => {
        await Promise.all([
          client.invalidateQueries({
            queryKey:
              identityKeys.user(
                id,
              ),
          }),

          client.invalidateQueries({
            queryKey:
              identityKeys.users(),
          }),
        ]);
      },
  });
}

export function useRevokeUserSessions(
  id: string,
) {
  const client =
    useQueryClient();

  return useMutation({
    mutationFn: () =>
      identityService.users.revokeSessions(
        id,
      ),

    onSuccess:
      async () => {
        await Promise.all([
          client.invalidateQueries({
            queryKey:
              identityKeys.sessions(),
          }),

          client.invalidateQueries({
            queryKey:
              identityKeys.user(
                id,
              ),
          }),
        ]);
      },
  });
}
