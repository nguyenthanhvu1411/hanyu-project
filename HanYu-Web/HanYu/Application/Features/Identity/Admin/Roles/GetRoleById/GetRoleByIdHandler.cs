using HanYu.Application.Common.Models;
using HanYu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using HanYu.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Application.Features.Identity.Admin.Roles.GetRoleById;

public sealed class GetRoleByIdHandler
{
    private readonly RoleManager<Role> _roleManager;
    private readonly HanYu.Application.Interfaces.Persistence.IHanYuDbContext _dbContext;

    public GetRoleByIdHandler(
        RoleManager<Role> roleManager,
        HanYu.Application.Interfaces.Persistence.IHanYuDbContext dbContext)
    {
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    public async Task<Result<AdminRoleDetailDto>> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());

        if (role is null)
            return Result.Failure<AdminRoleDetailDto>(
                Error.NotFound("Role.NotFound", "Vai trò không tồn tại."));

        var claims = await _roleManager.GetClaimsAsync(role);
        
        var claimValues = claims
            .Where(c => c.Type == "Permission")
            .Select(c => c.Value)
            .ToList();

        // Fetch real permissions from DB matching the claim values
        var dbPermissions = await _dbContext.Permissions
            .Where(p => claimValues.Contains(p.Code))
            .ToListAsync(cancellationToken);

        var permissions = dbPermissions
            .Select(p => new AdminRolePermissionDto
            {
                Id = p.Code,
                Code = p.Code,
                Description = p.Description,
                Resource = p.Resource,
                Action = p.Action
            }).ToList();

        var dto = new AdminRoleDetailDto
        {
            Id = role.Id,
            Code = role.Name ?? string.Empty,
            Name = role.Description ?? role.Name ?? string.Empty,
            Description = role.Description,
            IsSystem = false,
            UserCount = 0,
            Permissions = permissions,
            ConcurrencyToken = role.ConcurrencyStamp,
        };

        return Result.Success(dto);
    }
}
