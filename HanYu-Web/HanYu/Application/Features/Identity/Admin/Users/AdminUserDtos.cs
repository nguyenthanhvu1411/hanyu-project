using System;
using System.Collections.Generic;

namespace HanYu.Application.Features.Identity.Admin.Users;

public class AdminUserRoleDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class AdminUserListItemDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Locale { get; set; }
    public DateTimeOffset? EmailVerifiedAt { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
    public int FailedLoginCount { get; set; }
    public DateTimeOffset? LockedUntil { get; set; }
    public List<AdminUserRoleDto> Roles { get; set; } = new();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string ConcurrencyToken { get; set; } = string.Empty;
}

public class AdminUserDetailDto : AdminUserListItemDto
{
    public List<string> Permissions { get; set; } = new();
    public int SessionCount { get; set; }
    public int ActiveSessionCount { get; set; }
}
