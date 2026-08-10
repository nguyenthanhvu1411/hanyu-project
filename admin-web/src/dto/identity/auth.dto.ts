export interface LoginRequest {
  email: string;
  password: string;
  rememberMe?: boolean;
}

export interface LoginResponseDto {
  accessToken: string;
  refreshToken: string;
  expiresIn?: number;
  tokenType?: string;
  user?: CurrentUserDto;
}

export interface RefreshTokenResponseDto {
  accessToken: string;
  refreshToken: string;
  expiresIn?: number;
  tokenType?: string;
}

export interface CurrentUserRoleDto {
  id?: number;
  code: string;
  name?: string;
}

export interface CurrentUserDto {
  id?: number;
  publicId?: string;
  email: string;
  displayName: string;
  avatarUrl?: string | null;
  status?: string;
  emailVerified?: boolean;
  locale?: string;
  roles: string[];
  permissions: string[];
}

export interface LogoutResponseDto {
  success?: boolean;
}
