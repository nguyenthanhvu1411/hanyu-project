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

      const response =
        await apiClient.get<
          PagedResult<AdminUserListItemDto>
        >(
          `${API_ENDPOINTS.ADMIN.USERS}?${qs}`,
        );

      return response;
    },

    async get(
      id: string,
    ) {
      const response =
        await apiClient.get<
          AdminUserDetailDto
        >(
          API_ENDPOINTS.ADMIN.USER(
            id,
          ),
        );

      return response;
    },

    async create(
      request:
        CreateAdminUserRequest,
    ) {
      const response =
        await apiClient.post<
          AdminUserDetailDto
        >(
          API_ENDPOINTS.ADMIN.USERS,
          request,
        );

      return response;
    },

    async update(
      id: string,
      request:
        UpdateAdminUserRequest,
    ) {
      const response =
        await apiClient.put<
          AdminUserDetailDto
        >(
          API_ENDPOINTS.ADMIN.USER(
            id,
          ),
          request,
        );

      return response;
    },

    async patch(
      id: string,
      request:
        PatchAdminUserRequest,
    ) {
      const response =
        await apiClient.patch<
          AdminUserDetailDto
        >(
          API_ENDPOINTS.ADMIN.USER(
            id,
          ),
          request,
        );

      return response;
    },

    async remove(
      id: string,
      request: DeleteUserRequest,
    ) {
      await apiClient.delete(
        API_ENDPOINTS.ADMIN.USER(id),
        { body: request }
      );
    },

    async restore(
      id: string,
    ) {
      const response =
        await apiClient.post<
          AdminUserDetailDto
        >(
          API_ENDPOINTS.ADMIN.USER_RESTORE(
            id,
          ),
        );

      return response;
    },

    async lock(
      id: string,
      request:
        LockUserRequest,
    ) {
      const response =
        await apiClient.post<
          AdminUserDetailDto
        >(
          API_ENDPOINTS.ADMIN.USER_LOCK(
            id,
          ),
          request,
        );

      return response;
    },

    async unlock(
      id: string,
      request: UnlockUserRequest,
    ) {
      const response =
        await apiClient.post<
          AdminUserDetailDto
        >(
          API_ENDPOINTS.ADMIN.USER_UNLOCK(
            id,
          ),
          request,
        );

      return response;
    },

    async replaceRoles(
      id: string,
      request:
        ReplaceUserRolesRequest,
    ) {
      const response =
        await apiClient.put<
          AdminUserDetailDto
        >(
          API_ENDPOINTS.ADMIN.USER_ROLES(
            id,
          ),
          request,
        );

      return response;
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

      const response =
        await apiClient.get<
          PagedResult<AdminRoleDto>
        >(
          `${API_ENDPOINTS.ADMIN.ROLES}?${qs}`,
        );

      return response;
    },

    async get(
      id: string,
    ) {
      const response =
        await apiClient.get<
          AdminRoleDto
        >(
          API_ENDPOINTS.ADMIN.ROLE(
            id,
          ),
        );

      return response;
    },

    async create(
      request:
        CreateAdminRoleRequest,
    ) {
      const response =
        await apiClient.post<
          AdminRoleDto
        >(
          API_ENDPOINTS.ADMIN.ROLES,
          request,
        );

      return response;
    },

    async update(
      id: string,
      request:
        UpdateAdminRoleRequest,
    ) {
      const response =
        await apiClient.put<
          AdminRoleDto
        >(
          API_ENDPOINTS.ADMIN.ROLE(
            id,
          ),
          request,
        );

      return response;
    },

    async patch(
      id: string,
      request:
        PatchAdminRoleRequest,
    ) {
      const response =
        await apiClient.patch<
          AdminRoleDto
        >(
          API_ENDPOINTS.ADMIN.ROLE(
            id,
          ),
          request,
        );

      return response;
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
      const response =
        await apiClient.post<
          AdminRoleDto
        >(
          API_ENDPOINTS.ADMIN.ROLE_RESTORE(
            id,
          ),
        );

      return response;
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

      const response =
        await apiClient.get<
          PagedResult<AdminPermissionDto>
        >(
          `${API_ENDPOINTS.ADMIN.PERMISSIONS}?${qs}`,
        );

      return response;
    },

    async get(
      id: string,
    ) {
      const response =
        await apiClient.get<
          AdminPermissionDto
        >(
          API_ENDPOINTS.ADMIN.PERMISSION(
            id,
          ),
        );

      return response;
    },

    async create(
      request:
        CreateAdminPermissionRequest,
    ) {
      const response =
        await apiClient.post<
          AdminPermissionDto
        >(
          API_ENDPOINTS.ADMIN.PERMISSIONS,
          request,
        );

      return response;
    },

    async update(
      id: string,
      request:
        UpdateAdminPermissionRequest,
    ) {
      const response =
        await apiClient.put<
          AdminPermissionDto
        >(
          API_ENDPOINTS.ADMIN.PERMISSION(
            id,
          ),
          request,
        );

      return response;
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
      const response =
        await apiClient.post<
          AdminPermissionDto
        >(
          API_ENDPOINTS.ADMIN.PERMISSION_RESTORE(
            id,
          ),
        );

      return response;
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

      const response =
        await apiClient.get<
          PagedResult<AdminSessionDto>
        >(
          `${API_ENDPOINTS.ADMIN.SESSIONS}?${qs}`,
        );

      return response;
    },

    async get(
      id: string,
    ) {
      const response =
        await apiClient.get<
          AdminSessionDto
        >(
          API_ENDPOINTS.ADMIN.SESSION(
            id,
          ),
        );

      return response;
    },

    async create(
      request:
        CreateAdminSessionRequest,
    ) {
      const response =
        await apiClient.post<
          AdminSessionDto
        >(
          API_ENDPOINTS.ADMIN.SESSIONS,
          request,
        );

      return response;
    },

    async update(
      id: string,
      request:
        UpdateAdminSessionRequest,
    ) {
      const response =
        await apiClient.put<
          AdminSessionDto
        >(
          API_ENDPOINTS.ADMIN.SESSION(
            id,
          ),
          request,
        );

      return response;
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
      const response =
        await apiClient.post<
          AdminSessionDto
        >(
          API_ENDPOINTS.ADMIN.SESSION_RESTORE(
            id,
          ),
        );

      return response;
    },
  },
};
