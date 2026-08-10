export interface AuthUserRole {
  id?: number;
  code: string;
  name?: string;
}

export interface AuthUser {
  id?: number;
  publicId?: string;
  email: string;
  displayName: string;
  avatarUrl?: string | null;
  status?: string;
  locale?: string;
  emailVerified?: boolean;
  roles: string[];
  permissions: string[];
}

export type AuthStatus =
  | "idle"
  | "loading"
  | "authenticated"
  | "unauthenticated";

export interface LoginCredentials {
  email: string;
  password: string;
  rememberMe?: boolean;
}
