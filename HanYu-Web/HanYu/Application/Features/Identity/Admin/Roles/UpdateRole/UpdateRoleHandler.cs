using HanYu.Application.Common.Models;
using HanYu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace HanYu.Application.Features.Identity.Admin.Roles.UpdateRole;

public sealed record UpdateRoleCommand
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public List<string>? PermissionIds { get; init; }
    public string? ConcurrencyToken { get; init; }
}

public sealed class UpdateRoleHandler
{
    private readonly RoleManager<Role> _roleManager;
    private readonly HanYu.Application.Interfaces.Persistence.IHanYuDbContext _dbContext;

    public UpdateRoleHandler(RoleManager<Role> roleManager, HanYu.Application.Interfaces.Persistence.IHanYuDbContext dbContext)
    {
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    public async Task<Result> ExecuteAsync(
        UpdateRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(command.Id.ToString());
        if (role is null)
            return Result.Failure(Error.NotFound("Role.NotFound", "Vai trò không tồn tại."));

        if (!string.Equals(role.Name, command.Name, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _roleManager.RoleExistsAsync(command.Name);
            if (exists)
                return Result.Failure(Error.Conflict("Role.NameConflict", "Tên vai trò đã tồn tại."));
            
            role.Rename(command.Name);
        }

        role.UpdateDescription(command.Description);

        // Optimistic Concurrency check
        if (!string.IsNullOrEmpty(command.ConcurrencyToken) && role.ConcurrencyStamp != command.ConcurrencyToken)
            return Result.Failure(Error.Conflict("Role.Concurrency", "Vai trò đã bị thay đổi bởi người khác. Vui lòng tải lại trang."));

        var result = await _roleManager.UpdateAsync(role);

        if (!result.Succeeded)
            return Result.Failure(Error.Validation("Role.UpdateFailed", "Không thể cập nhật vai trò."));

        if (command.PermissionIds != null)
        {
            var currentClaims = await _dbContext.RoleClaims
                .Where(c => c.RoleId == role.Id && c.ClaimType == "Permission")
                .ToListAsync(cancellationToken);
            
            // Remove all existing permission claims
            _dbContext.RoleClaims.RemoveRange(currentClaims);

            // Add new ones
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

        return Result.Success();
    }
}
