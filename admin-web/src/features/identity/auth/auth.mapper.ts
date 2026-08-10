import type { CurrentUserDto } from "@/dto/identity/auth.dto";
import type { AuthUser } from "./auth.types";

export function mapCurrentUser(dto: CurrentUserDto): AuthUser {
  return {
    id: dto.id,
    publicId: dto.publicId,
    email: dto.email,
    displayName: dto.displayName,
    avatarUrl: dto.avatarUrl,
    status: dto.status,
    locale: dto.locale,
    emailVerified: dto.emailVerified,
    roles: dto.roles ?? [],
    permissions: dto.permissions ?? [],
  };
}
