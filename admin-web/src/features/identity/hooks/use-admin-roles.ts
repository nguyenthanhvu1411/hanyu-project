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
  AdminRoleListQuery,
} from "../identity.types";

import type {
  CreateAdminRoleRequest,
  UpdateAdminRoleRequest,
} from "@/dto/identity/admin-role.dto";

export function useAdminRoles(
  query:
    AdminRoleListQuery = {},
) {
  return useQuery({
    queryKey:
      identityKeys.roleList(
        query,
      ),

    queryFn: () =>
      identityService.roles.list(
        query,
      ),
  });
}

export function useAdminRole(
  id?: string,
) {
  return useQuery({
    queryKey:
      identityKeys.role(
        id ?? "",
      ),

    queryFn: () =>
      identityService.roles.get(
        id!,
      ),

    enabled:
      Boolean(id),
  });
}

export function useCreateAdminRole() {
  const client =
    useQueryClient();

  return useMutation({
    mutationFn: (
      request:
        CreateAdminRoleRequest,
    ) =>
      identityService.roles.create(
        request,
      ),

    onSuccess: async () => {
      await client.invalidateQueries({
        queryKey:
          identityKeys.roles(),
      });
    },
  });
}

export function useUpdateAdminRole(
  id: string,
) {
  const client =
    useQueryClient();

  return useMutation({
    mutationFn: (
      request:
        UpdateAdminRoleRequest,
    ) =>
      identityService.roles.update(
        id,
        request,
      ),

    onSuccess: async () => {
      await client.invalidateQueries({
        queryKey:
          identityKeys.roles(),
      });
    },
  });
}

export function useDeleteAdminRole() {
  const client =
    useQueryClient();

  return useMutation({
    mutationFn:
      identityService.roles.remove,

    onSuccess: async () => {
      await client.invalidateQueries({
        queryKey:
          identityKeys.roles(),
      });
    },
  });
}

export function useRestoreAdminRole() {
  const client =
    useQueryClient();

  return useMutation({
    mutationFn:
      identityService.roles.restore,

    onSuccess: async () => {
      await client.invalidateQueries({
        queryKey:
          identityKeys.roles(),
      });
    },
  });
}
