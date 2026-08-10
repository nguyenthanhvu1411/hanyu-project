using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Admin.Users;
using HanYu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Application.Features.Identity.Admin.Users.GetUserById;

public sealed class GetUserByIdHandler
{
    private readonly UserManager<User> _userManager;

    public GetUserByIdHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<AdminUserDetailDto>> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .Include(u => u.Profile)
            .Include(u => u.Sessions)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            return Result.Failure<AdminUserDetailDto>(
                Error.NotFound("User.NotFound", "Người dùng không tồn tại."));

        var roles = await _userManager.GetRolesAsync(user);

        var status = user.DeletedAt.HasValue
            ? "deleted"
            : (user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
                ? "locked"
                : "active";

        var dto = new AdminUserDetailDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            DisplayName = user.Profile?.DisplayName ?? user.UserName ?? string.Empty,
            AvatarUrl = user.Profile?.AvatarUrl,
            Status = status,
            Locale = user.Profile?.UiLanguage,
            EmailVerifiedAt = user.EmailConfirmed ? user.UpdatedAt : null,
            LastLoginAt = user.LastLoginAt,
            FailedLoginCount = user.AccessFailedCount,
            LockedUntil = user.LockoutEnd,
            Roles = roles.Select(r => new AdminUserRoleDto { Id = Guid.Empty, Code = r, Name = r }).ToList(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            DeletedAt = user.DeletedAt,
            ConcurrencyToken = user.ConcurrencyStamp ?? string.Empty,
            Permissions = [],
            SessionCount = user.Sessions.Count,
            ActiveSessionCount = user.Sessions.Count(s => s.IsActive),
        };

        return Result.Success(dto);
    }
}
