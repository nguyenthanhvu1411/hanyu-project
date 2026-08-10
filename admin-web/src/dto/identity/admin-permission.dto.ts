export interface AdminPermissionDto {
  id: string;

  code: string;

  description?: string | null;

  resource: string;

  action: string;

  roleCount?: number;

  createdAt?: string;

  updatedAt?: string;

  deletedAt?: string | null;

  concurrencyToken?: string;
}

export interface CreateAdminPermissionRequest {
  code: string;

  description?: string;

  resource: string;

  action: string;
}

export interface UpdateAdminPermissionRequest {
  code: string;

  description?: string;

  resource: string;

  action: string;

  concurrencyToken?: string;
}
