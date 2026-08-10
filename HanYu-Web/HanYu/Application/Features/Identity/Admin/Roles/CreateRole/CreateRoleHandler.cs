using HanYu.Application.Common.Models;
using HanYu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HanYu.Domain.Constants;

namespace HanYu.Application.Features.Identity.Admin.Roles.CreateRole;

public sealed record CreateRoleCommand
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public List<string>? PermissionIds { get; init; }
}

public sealed class CreateRoleHandler
{
    private readonly RoleManager<Role> _roleManager;
    private readonly HanYu.Application.Interfaces.Persistence.IHanYuDbContext _dbContext;

    public CreateRoleHandler(RoleManager<Role> roleManager, HanYu.Application.Interfaces.Persistence.IHanYuDbContext dbContext)
    {
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    public async Task<Result<Guid>> ExecuteAsync(
        CreateRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result.Failure<Guid>(
                Error.Validation("Role.NameRequired", "Tên vai trò không được để trống."));

        var exists = await _roleManager.RoleExistsAsync(command.Name);
        if (exists)
            return Result.Failure<Guid>(
                Error.Conflict("Role.NameConflict", "Tên vai trò đã tồn tại."));

        var role = new Role(command.Name, command.Description);
        var result = await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
            return Result.Failure<Guid>(
                Error.Validation("Role.CreateFailed", "Không thể tạo vai trò."));

        if (command.PermissionIds != null && command.PermissionIds.Any())
        {
            var newClaims = command.PermissionIds
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => new Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>
                {
                    RoleId = role.Id,
                    ClaimType = "Permission",
                    ClaimValue = c
                });

            await _dbContext.RoleClaims.AddRangeAsync(newClaims, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        
        return Result.Success(role.Id);
    }
}
