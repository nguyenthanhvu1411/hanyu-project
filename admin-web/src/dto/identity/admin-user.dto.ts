import type {
  UserStatus,
} from "@/features/identity/identity.constants";

export interface AdminUserRoleDto {
  id: string;

  code: string;

  name: string;

  isSystem?: boolean;

  grantedAt?: string;

  expiresAt?: string | null;
}

export interface AdminUserListItemDto {
  id: string;

  email: string;

  displayName: string;

  avatarUrl?: string | null;

  status: UserStatus;

  locale?: string;

  emailVerifiedAt?: string | null;

  lastLoginAt?: string | null;

  failedLoginCount?: number;

  lockedUntil?: string | null;

  roles?: AdminUserRoleDto[];

  createdAt?: string;

  updatedAt?: string;

  deletedAt?: string | null;

  concurrencyToken?: string;
}

export interface AdminUserDetailDto
  extends AdminUserListItemDto {
  permissions?: string[];

  sessionCount?: number;

  activeSessionCount?: number;
}

export interface CreateAdminUserRequest {
  email: string;

  password: string;

  displayName: string;

  locale?: string;

  status?: UserStatus;

  roleIds?: string[];

  emailVerified?: boolean;
}

export interface UpdateAdminUserRequest {
  email: string;
  displayName: string;
  locale?: string;
  status: UserStatus;
  concurrencyToken?: string;
}

export interface PatchAdminUserRequest {
  displayName?: string;
  locale?: string;
  status?: UserStatus;
  concurrencyToken?: string;
}

export interface ReplaceUserRolesRequest {
  roleIds: string[];
  concurrencyToken?: string;
}

export interface LockUserRequest {
  reason: string;
  until?: string | null;
  concurrencyToken?: string;
}

export interface UnlockUserRequest {
  reason: string;
  concurrencyToken?: string;
}

export interface DeleteUserRequest {
  reason: string;
}

export interface ResetAdminUserPasswordRequest {
  newPassword: string;
}
