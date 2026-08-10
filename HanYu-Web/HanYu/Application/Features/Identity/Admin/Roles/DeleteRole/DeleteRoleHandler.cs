using HanYu.Application.Common.Models;
using HanYu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Application.Features.Identity.Admin.Roles.DeleteRole;

public sealed class DeleteRoleHandler
{
    private readonly RoleManager<Role> _roleManager;

    public DeleteRoleHandler(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Result> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role is null)
            return Result.Failure(Error.NotFound("Role.NotFound", "Vai trò không tồn tại."));

        if (role.IsSystem)
            return Result.Failure(Error.Validation("Role.SystemRole", "Không thể xóa vai trò hệ thống."));

        var result = await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
            return Result.Failure(Error.Validation("Role.DeleteFailed", "Không thể xóa vai trò."));

        return Result.Success();
    }
}
