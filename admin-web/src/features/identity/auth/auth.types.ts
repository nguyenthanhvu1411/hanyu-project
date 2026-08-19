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

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export interface AuthSession {
  sessionKey: string;
  deviceName: string | null;
  deviceType: string | null;
  browser: string | null;
  operatingSystem: string | null;
  ipAddress: string | null;
  lastActivityAt: string;
  revokedAt: string | null;
  status: string;
  isCurrent: boolean;
}

export interface SecurityEvent {
  eventType: string;
  ipAddress: string | null;
  userAgent: string | null;
  metadataJson: string | null;
  occurredAt: string;
}

export interface TwoFactorSetupResponse {
  sharedKey: string;
  authenticatorUri: string;
}

export interface TwoFactorRecoveryCodesResponse {
  recoveryCodes: string[];
}

export interface EnableTwoFactorRequest {
  code: string;
}

export interface DisableTwoFactorRequest {
  password: string;
  code: string;
}

export interface PasswordConfirmationRequest {
  password: string;
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
