using HanYu.API.Common.Extensions;
using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Persistence;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HanYu.API.Controller.Admin.Identity.Permissions;

public sealed record AdminPermissionDto
{
    public string Id { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Resource { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public int RoleCount { get; init; }
}

public sealed class GetPermissionsQuery
{
    public string? Search { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/permissions")]
public sealed class AdminPermissionsController : ControllerBase
{
    private readonly IHanYuDbContext _dbContext;

    public AdminPermissionsController(IHanYuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetPermissionsQuery query, CancellationToken cancellationToken)
    {
        var queryable = _dbContext.Permissions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var searchLower = query.Search.Trim().ToLower();
            queryable = queryable.Where(p => 
                p.Code.ToLower().Contains(searchLower) ||
                (p.Description != null && p.Description.ToLower().Contains(searchLower)));
        }

        var total = await queryable.CountAsync(cancellationToken);
        
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 1000);

        var permissions = await queryable
            .OrderBy(p => p.Resource).ThenBy(p => p.Action)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var codes = permissions.Select(p => p.Code).ToList();

        var roleCounts = await _dbContext.RoleClaims
            .Where(rc => rc.ClaimType == "Permission" && codes.Contains(rc.ClaimValue))
            .GroupBy(rc => rc.ClaimValue)
            .Select(g => new { Code = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Code, x => x.Count, cancellationToken);

        var pagedPermissions = permissions
            .Select(p => new AdminPermissionDto
            {
                Id = p.Code,
                Code = p.Code,
                Description = p.Description,
                Resource = p.Resource,
                Action = p.Action,
                RoleCount = roleCounts.TryGetValue(p.Code, out var count) ? count : 0
            }).ToList();

        var result = new PagedResult<AdminPermissionDto>(pagedPermissions, page, pageSize, total);
        return Ok(result);
    }
}
