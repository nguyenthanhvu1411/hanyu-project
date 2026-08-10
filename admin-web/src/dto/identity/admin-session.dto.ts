export interface AdminSessionDto {
  id: number;

  userId: string;

  userEmail?: string;

  userDisplayName?: string;

  deviceInfo?: string | null;

  ipAddress?: string | null;

  userAgent?: string | null;

  createdAt?: string;

  lastUsedAt?: string | null;

  expiresAt?: string | null;

  revokedAt?: string | null;

  revokedReason?: string | null;

  isActive: boolean;

  deletedAt?: string | null;

  concurrencyToken?: string;
}

export interface CreateAdminSessionRequest {
  userId: string;

  deviceInfo?: string;

  ipAddress?: string;

  userAgent?: string;

  expiresAt?: string;
}

export interface UpdateAdminSessionRequest {
  deviceInfo?: string;

  expiresAt?: string;

  concurrencyToken?: string;
}
