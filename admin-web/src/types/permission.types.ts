export interface PermissionInfo {
  code: string;

  name: string;

  description?: string;

  group?: string;
}

export interface RoleInfo {
  id: string;

  name: string;

  permissions: string[];
}
