using HanYu.Application.Common.Models;
using HanYu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Application.Features.Identity.Admin.Roles.GetRoles;

public sealed class GetRolesQuery
{
    public string? Search { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SortBy { get; init; }
    public string? SortDirection { get; init; }
}

public sealed class GetRolesHandler
{
    private readonly RoleManager<Role> _roleManager;
    private readonly HanYu.Infrastructure.Persistence.HanYuDbContext _dbContext;

    public GetRolesHandler(
        RoleManager<Role> roleManager,
        HanYu.Infrastructure.Persistence.HanYuDbContext dbContext)
    {
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<AdminRoleListItemDto>>> ExecuteAsync(
        GetRolesQuery query,
        CancellationToken cancellationToken = default)
    {
        var rolesQuery = _roleManager.Roles.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var searchLower = query.Search.Trim().ToLower();
            rolesQuery = rolesQuery.Where(r =>
                (r.Name != null && r.Name.ToLower().Contains(searchLower)));
        }

        var total = await rolesQuery.LongCountAsync(cancellationToken);

        rolesQuery = query.SortBy?.ToLower() switch
        {
            "name" => query.SortDirection?.ToLower() == "desc"
                ? rolesQuery.OrderByDescending(r => r.Name)
                : rolesQuery.OrderBy(r => r.Name),
            _ => rolesQuery.OrderBy(r => r.Name),
        };

        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var roles = await rolesQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var roleIds = roles.Select(r => r.Id).ToList();

        var userCounts = await _dbContext.UserRoles
            .Where(ur => roleIds.Contains(ur.RoleId))
            .GroupBy(ur => ur.RoleId)
            .Select(g => new { RoleId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.RoleId, x => x.Count, cancellationToken);

        var permissionCounts = await _dbContext.RoleClaims
            .Where(rc => roleIds.Contains(rc.RoleId) && rc.ClaimType == "Permission")
            .GroupBy(rc => rc.RoleId)
            .Select(g => new { RoleId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.RoleId, x => x.Count, cancellationToken);

        var dtos = roles.Select(r => new AdminRoleListItemDto
        {
            Id = r.Id,
            Code = r.Name ?? string.Empty,
            Name = r.Description ?? r.Name ?? string.Empty,
            Description = r.Description,
            IsSystem = r.IsSystem,
            UserCount = userCounts.GetValueOrDefault(r.Id, 0),
            PermissionCount = permissionCounts.GetValueOrDefault(r.Id, 0),
            ConcurrencyToken = r.ConcurrencyStamp,
        }).ToList();

        return Result<PagedResult<AdminRoleListItemDto>>.Success(
            new PagedResult<AdminRoleListItemDto>(dtos, page, pageSize, total));
    }
}
