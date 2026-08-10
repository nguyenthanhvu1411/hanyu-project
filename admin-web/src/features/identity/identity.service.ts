import {
  identityApi,
} from "./identity.api";

import {
  mapPagedResult,
} from "./identity.mapper";

import type {
  CreateAdminPermissionRequest,
  UpdateAdminPermissionRequest,
} from "@/dto/identity/admin-permission.dto";

import type {
  CreateAdminRoleRequest,
  PatchAdminRoleRequest,
  UpdateAdminRoleRequest,
} from "@/dto/identity/admin-role.dto";

import type {
  CreateAdminUserRequest,
  LockUserRequest,
  PatchAdminUserRequest,
  ReplaceUserRolesRequest,
  UnlockUserRequest,
  UpdateAdminUserRequest,
} from "@/dto/identity/admin-user.dto";

import type {
  AdminPermissionListQuery,
  AdminRoleListQuery,
  AdminSessionListQuery,
  AdminUserListQuery,
} from "./identity.types";

export const identityService = {
  users: {
    async list(
      query:
        AdminUserListQuery,
    ) {
      return mapPagedResult(
        await identityApi.users.list(
          query,
        ),
      );
    },

    async get(
      id: string,
    ) {
      return (
        await identityApi.users.get(
          id,
        )
      );
    },

    async create(
      request:
        CreateAdminUserRequest,
    ) {
      return (
        await identityApi.users.create(
          request,
        )
      );
    },

    async update(
      id: string,
      request:
        UpdateAdminUserRequest,
    ) {
      return (
        await identityApi.users.update(
          id,
          request,
        )
      );
    },

    async patch(
      id: string,
      request:
        PatchAdminUserRequest,
    ) {
      return (
        await identityApi.users.patch(
          id,
          request,
        )
      );
    },

    remove:
      identityApi.users.remove,

    restore:
      identityApi.users.restore,

    lock(
      id: string,
      request:
        LockUserRequest,
    ) {
      return identityApi.users.lock(
        id,
        request,
      );
    },

    unlock(
      id: string,
      request:
        UnlockUserRequest,
    ) {
      return identityApi.users.unlock(
        id,
        request,
      );
    },

    replaceRoles(
      id: string,
      request:
        ReplaceUserRolesRequest,
    ) {
      return identityApi.users.replaceRoles(
        id,
        request,
      );
    },

    revokeSessions:
      identityApi.users
        .revokeSessions,
  },

  roles: {
    async list(
      query:
        AdminRoleListQuery,
    ) {
      return mapPagedResult(
        await identityApi.roles.list(
          query,
        ),
      );
    },

    async get(
      id: string,
    ) {
      return (
        await identityApi.roles.get(
          id,
        )
      );
    },

    async create(
      request:
        CreateAdminRoleRequest,
    ) {
      return (
        await identityApi.roles.create(
          request,
        )
      );
    },

    async update(
      id: string,
      request:
        UpdateAdminRoleRequest,
    ) {
      return (
        await identityApi.roles.update(
          id,
          request,
        )
      );
    },

    patch(
      id: string,
      request:
        PatchAdminRoleRequest,
    ) {
      return identityApi.roles.patch(
        id,
        request,
      );
    },

    remove:
      identityApi.roles.remove,

    restore:
      identityApi.roles.restore,
  },

  permissions: {
    async list(
      query:
        AdminPermissionListQuery,
    ) {
      return mapPagedResult(
        await identityApi.permissions.list(
          query,
        ),
      );
    },

    async get(
      id: string,
    ) {
      return (
        await identityApi.permissions.get(
          id,
        )
      );
    },

    create(
      request:
        CreateAdminPermissionRequest,
    ) {
      return identityApi.permissions.create(
        request,
      );
    },

    update(
      id: string,
      request:
        UpdateAdminPermissionRequest,
    ) {
      return identityApi.permissions.update(
        id,
        request,
      );
    },

    remove:
      identityApi.permissions
        .remove,

    restore:
      identityApi.permissions
        .restore,
  },

  sessions: {
    async list(
      query:
        AdminSessionListQuery,
    ) {
      return mapPagedResult(
        await identityApi.sessions.list(
          query,
        ),
      );
    },

    async get(
      id: string,
    ) {
      return (
        await identityApi.sessions.get(
          id,
        )
      );
    },

    remove:
      identityApi.sessions.remove,

    restore:
      identityApi.sessions.restore,
  },
};
