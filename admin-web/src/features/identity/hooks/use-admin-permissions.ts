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
  AdminPermissionListQuery,
} from "../identity.types";

import type {
  CreateAdminPermissionRequest,
  UpdateAdminPermissionRequest,
} from "@/dto/identity/admin-permission.dto";

export function useAdminPermissions(
  query:
    AdminPermissionListQuery = {},
) {
  return useQuery({
    queryKey:
      identityKeys.permissionList(
        query,
      ),

    queryFn: () =>
      identityService.permissions.list(
        query,
      ),
  });
}

export function useAdminPermission(
  id?: string,
) {
  return useQuery({
    queryKey:
      identityKeys.permission(
        id ?? "",
      ),

    queryFn: () =>
      identityService.permissions.get(
        id!,
      ),

    enabled:
      Boolean(id),
  });
}

export function useCreateAdminPermission() {
  const client =
    useQueryClient();

  return useMutation({
    mutationFn: (
      request:
        CreateAdminPermissionRequest,
    ) =>
      identityService.permissions.create(
        request,
      ),

    onSuccess: async () => {
      await client.invalidateQueries({
        queryKey:
          identityKeys.permissions(),
      });
    },
  });
}

export function useUpdateAdminPermission(
  id: string,
) {
  const client =
    useQueryClient();

  return useMutation({
    mutationFn: (
      request:
        UpdateAdminPermissionRequest,
    ) =>
      identityService.permissions.update(
        id,
        request,
      ),

    onSuccess: async () => {
      await client.invalidateQueries({
        queryKey:
          identityKeys.permissions(),
      });
    },
  });
}

export function useDeleteAdminPermission() {
  const client =
    useQueryClient();

  return useMutation({
    mutationFn:
      identityService.permissions.remove,

    onSuccess: async () => {
      await client.invalidateQueries({
        queryKey:
          identityKeys.permissions(),
      });
    },
  });
}
