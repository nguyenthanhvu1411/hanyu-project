import {
  apiClient,
} from "@/lib/api/api-client";

import {
  API_ENDPOINTS,
} from "@/lib/api/api-endpoints";

import {
  createQueryString,
} from "@/utils/query.util";

import type {
  PagedResult,
} from "@/types/api.types";

import type {
  AdminPermissionDto,
  CreateAdminPermissionRequest,
  UpdateAdminPermissionRequest,
} from "@/dto/identity/admin-permission.dto";

import type {
  AdminRoleDto,
  CreateAdminRoleRequest,
  PatchAdminRoleRequest,
  UpdateAdminRoleRequest,
} from "@/dto/identity/admin-role.dto";

import type {
  AdminSessionDto,
  CreateAdminSessionRequest,
  UpdateAdminSessionRequest,
} from "@/dto/identity/admin-session.dto";

import type {
  AdminUserDetailDto,
  AdminUserListItemDto,
  CreateAdminUserRequest,
  LockUserRequest,
  PatchAdminUserRequest,
  ReplaceUserRolesRequest,
  ResetAdminUserPasswordRequest,
  UnlockUserRequest,
  UpdateAdminUserRequest,
  DeleteUserRequest,
} from "@/dto/identity/admin-user.dto";

import type {
  AdminPermissionListQuery,
  AdminRoleListQuery,
  AdminSessionListQuery,
  AdminUserListQuery,
} from "./identity.types";

export const identityApi = {
  users: {
    async list(
      query:
        AdminUserListQuery,
    ) {
      const qs =
        createQueryString(
          query,
        );

      return apiClient.get<
        PagedResult<AdminUserListItemDto>
      >(
        `${API_ENDPOINTS.ADMIN.USERS}?${qs}`,
      );
    },

    async get(
      id: string,
    ) {
      return apiClient.get<
        AdminUserDetailDto
      >(
        API_ENDPOINTS.ADMIN.USER(
          id,
        ),
      );
    },

    async create(
      request:
        CreateAdminUserRequest,
    ) {
      return apiClient.post<
        AdminUserDetailDto
      >(
        API_ENDPOINTS.ADMIN.USERS,
        request,
      );
    },

    async update(
      id: string,
      request:
        UpdateAdminUserRequest,
    ) {
      return apiClient.put<
        AdminUserDetailDto
      >(
        API_ENDPOINTS.ADMIN.USER(
          id,
        ),
        request,
      );
    },

    async patch(
      id: string,
      request:
        PatchAdminUserRequest,
    ) {
      return apiClient.patch<
        AdminUserDetailDto
      >(
        API_ENDPOINTS.ADMIN.USER(
          id,
        ),
        request,
      );
    },

    async remove(
      id: string,
      request: DeleteUserRequest,
    ) {
      await apiClient.delete(
        API_ENDPOINTS.ADMIN.USER(id),
        { body: request },
      );
    },

    async restore(
      id: string,
    ) {
      return apiClient.post<
        AdminUserDetailDto
      >(
        API_ENDPOINTS.ADMIN.USER_RESTORE(
          id,
        ),
      );
    },

    async lock(
      id: string,
      request:
        LockUserRequest,
    ) {
      return apiClient.post<
        AdminUserDetailDto
      >(
        API_ENDPOINTS.ADMIN.USER_LOCK(
          id,
        ),
        request,
      );
    },

    async unlock(
      id: string,
      request: UnlockUserRequest,
    ) {
      return apiClient.post<
        AdminUserDetailDto
      >(
        API_ENDPOINTS.ADMIN.USER_UNLOCK(
          id,
        ),
        request,
      );
    },

    async replaceRoles(
      id: string,
      request:
        ReplaceUserRolesRequest,
    ) {
      return apiClient.put<
        AdminUserDetailDto
      >(
        API_ENDPOINTS.ADMIN.USER_ROLES(
          id,
        ),
        request,
      );
    },

    async revokeSessions(
      id: string,
    ) {
      await apiClient.delete(
        API_ENDPOINTS.ADMIN.USER_SESSIONS(
          id,
        ),
      );
    },

    async resetPassword(
      id: string,
      request: ResetAdminUserPasswordRequest,
    ) {
      await apiClient.post(
        API_ENDPOINTS.ADMIN.USER_RESET_PASSWORD(id),
        request,
      );
    },
  },

  roles: {
    async list(
      query:
        AdminRoleListQuery,
    ) {
      const qs =
        createQueryString(
          query,
        );

      return apiClient.get<
        PagedResult<AdminRoleDto>
      >(
        `${API_ENDPOINTS.ADMIN.ROLES}?${qs}`,
      );
    },

    async get(
      id: string,
    ) {
      return apiClient.get<
        AdminRoleDto
      >(
        API_ENDPOINTS.ADMIN.ROLE(
          id,
        ),
      );
    },

    async create(
      request:
        CreateAdminRoleRequest,
    ) {
      return apiClient.post<
        AdminRoleDto
      >(
        API_ENDPOINTS.ADMIN.ROLES,
        request,
      );
    },

    async update(
      id: string,
      request:
        UpdateAdminRoleRequest,
    ) {
      return apiClient.put<
        AdminRoleDto
      >(
        API_ENDPOINTS.ADMIN.ROLE(
          id,
        ),
        request,
      );
    },

    async patch(
      id: string,
      request:
        PatchAdminRoleRequest,
    ) {
      return apiClient.patch<
        AdminRoleDto
      >(
        API_ENDPOINTS.ADMIN.ROLE(
          id,
        ),
        request,
      );
    },

    async remove(
      id: string,
    ) {
      await apiClient.delete(
        API_ENDPOINTS.ADMIN.ROLE(
          id,
        ),
      );
    },

    async restore(
      id: string,
    ) {
      return apiClient.post<
        AdminRoleDto
      >(
        API_ENDPOINTS.ADMIN.ROLE_RESTORE(
          id,
        ),
      );
    },
  },

  permissions: {
    async list(
      query:
        AdminPermissionListQuery,
    ) {
      const qs =
        createQueryString(
          query,
        );

      return apiClient.get<
        PagedResult<AdminPermissionDto>
      >(
        `${API_ENDPOINTS.ADMIN.PERMISSIONS}?${qs}`,
      );
    },

    async get(
      id: string,
    ) {
      return apiClient.get<
        AdminPermissionDto
      >(
        API_ENDPOINTS.ADMIN.PERMISSION(
          id,
        ),
      );
    },

    async create(
      request:
        CreateAdminPermissionRequest,
    ) {
      return apiClient.post<
        AdminPermissionDto
      >(
        API_ENDPOINTS.ADMIN.PERMISSIONS,
        request,
      );
    },

    async update(
      id: string,
      request:
        UpdateAdminPermissionRequest,
    ) {
      return apiClient.put<
        AdminPermissionDto
      >(
        API_ENDPOINTS.ADMIN.PERMISSION(
          id,
        ),
        request,
      );
    },

    async remove(
      id: string,
    ) {
      await apiClient.delete(
        API_ENDPOINTS.ADMIN.PERMISSION(
          id,
        ),
      );
    },

    async restore(
      id: string,
    ) {
      return apiClient.post<
        AdminPermissionDto
      >(
        API_ENDPOINTS.ADMIN.PERMISSION_RESTORE(
          id,
        ),
      );
    },
  },

  sessions: {
    async list(
      query:
        AdminSessionListQuery,
    ) {
      const qs =
        createQueryString(
          query,
        );

      return apiClient.get<
        PagedResult<AdminSessionDto>
      >(
        `${API_ENDPOINTS.ADMIN.SESSIONS}?${qs}`,
      );
    },

    async get(
      id: string,
    ) {
      return apiClient.get<
        AdminSessionDto
      >(
        API_ENDPOINTS.ADMIN.SESSION(
          id,
        ),
      );
    },

    async create(
      request:
        CreateAdminSessionRequest,
    ) {
      return apiClient.post<
        AdminSessionDto
      >(
        API_ENDPOINTS.ADMIN.SESSIONS,
        request,
      );
    },

    async update(
      id: string,
      request:
        UpdateAdminSessionRequest,
    ) {
      return apiClient.put<
        AdminSessionDto
      >(
        API_ENDPOINTS.ADMIN.SESSION(
          id,
        ),
        request,
      );
    },

    async remove(
      id: number | string,
    ) {
      await apiClient.delete(
        API_ENDPOINTS.ADMIN.SESSION(
          id.toString(),
        ),
      );
    },

    async restore(
      id: string,
    ) {
      return apiClient.post<
        AdminSessionDto
      >(
        API_ENDPOINTS.ADMIN.SESSION_RESTORE(
          id,
        ),
      );
    },
  },
};
