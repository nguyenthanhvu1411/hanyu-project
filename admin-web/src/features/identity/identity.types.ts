import type {
  UserStatus,
} from "./identity.constants";

export interface AdminListQuery {
  page?: number;
  pageSize?: number;

  search?: string;

  sortBy?: string;

  sortDirection?:
    | "asc"
    | "desc";
}

export interface AdminUserListQuery
  extends AdminListQuery {
  status?: UserStatus;

  roleId?: string;

  emailVerified?: boolean;

  includeDeleted?: boolean;
}

export interface AdminRoleListQuery
  extends AdminListQuery {
  isSystem?: boolean;

  includeDeleted?: boolean;
}

export interface AdminPermissionListQuery
  extends AdminListQuery {
  resource?: string;

  action?: string;

  includeDeleted?: boolean;
}

export interface AdminSessionListQuery
  extends AdminListQuery {
  userId?: string;

  active?: boolean;

  ipAddress?: string;

  includeDeleted?: boolean;
}
