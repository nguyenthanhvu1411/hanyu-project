using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Admin.Users;
using HanYu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Application.Features.Identity.Admin.Users.GetUsers;

public sealed class GetUsersQuery
{
    public string? Search { get; init; }
    public string? Status { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SortBy { get; init; }
    public string? Direction { get; init; }
}

public sealed class GetUsersHandler
{
    private readonly UserManager<User> _userManager;

    public GetUsersHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<PagedResult<AdminUserListItemDto>>> ExecuteAsync(
        GetUsersQuery query,
        CancellationToken cancellationToken = default)
    {
        var usersQuery = _userManager.Users
            .Include(u => u.Profile)
            .AsNoTracking();

        // Filter by search (email or username)
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var searchLower = query.Search.Trim().ToLower();
            usersQuery = usersQuery.Where(u =>
                (u.Email != null && u.Email.ToLower().Contains(searchLower)) ||
                (u.UserName != null && u.UserName.ToLower().Contains(searchLower)));
        }

        // Filter by status (soft-deleted)
        if (query.Status == "deleted")
            usersQuery = usersQuery.Where(u => u.DeletedAt != null);
        else if (query.Status == "active")
            usersQuery = usersQuery.Where(u => u.DeletedAt == null && u.LockoutEnd == null);
        else if (query.Status == "locked")
            usersQuery = usersQuery.Where(u => u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow);

        var total = await usersQuery.LongCountAsync(cancellationToken);

        // Sorting
        usersQuery = query.SortBy?.ToLower() switch
        {
            "email" => query.Direction?.ToLower() == "desc"
                ? usersQuery.OrderByDescending(u => u.Email)
                : usersQuery.OrderBy(u => u.Email),
            "createdat" => query.Direction?.ToLower() == "desc"
                ? usersQuery.OrderByDescending(u => u.CreatedAt)
                : usersQuery.OrderBy(u => u.CreatedAt),
            _ => usersQuery.OrderByDescending(u => u.CreatedAt)
        };

        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var users = await usersQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = new List<AdminUserListItemDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            dtos.Add(MapToListItemDto(user, roles));
        }

        return Result<PagedResult<AdminUserListItemDto>>.Success(
            new PagedResult<AdminUserListItemDto>(dtos, page, pageSize, total));
    }

    private static AdminUserListItemDto MapToListItemDto(User user, IList<string> roles)
    {
        var status = user.DeletedAt.HasValue
            ? "deleted"
            : (user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
                ? "locked"
                : "active";

        return new AdminUserListItemDto
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
        };
    }
}
