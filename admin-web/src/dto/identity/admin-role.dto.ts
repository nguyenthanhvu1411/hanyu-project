export interface AdminRoleDto {
  id: string;

  code: string;

  name: string;

  description?: string | null;

  isSystem: boolean;

  permissions?: AdminRolePermissionDto[];

  userCount?: number;
  permissionCount?: number;

  createdAt?: string;

  updatedAt?: string;

  deletedAt?: string | null;

  concurrencyToken?: string;
}

export interface AdminRolePermissionDto {
  id: string;

  code: string;

  description?: string;

  resource?: string;

  action?: string;
}

export interface CreateAdminRoleRequest {
  code: string;

  name: string;

  description?: string;

  permissionIds?: string[];
}

export interface UpdateAdminRoleRequest {
  code: string;

  name: string;

  description?: string;

  permissionIds: string[];

  concurrencyToken?: string;
}

export interface PatchAdminRoleRequest {
  name?: string;

  description?: string;

  permissionIds?: string[];

  concurrencyToken?: string;
}
