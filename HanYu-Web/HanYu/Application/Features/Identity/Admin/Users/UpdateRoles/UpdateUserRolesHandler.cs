using HanYu.Application.Common.Exceptions;
using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Identity;
using HanYu.Application.Interfaces.Operations;
using HanYu.Domain.Constants;
using HanYu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace HanYu.Application.Features.Identity.Admin.Users.UpdateRoles;

public sealed class UpdateUserRolesCommand
{
    public Guid UserId { get; init; }
    public List<string> RoleCodes { get; init; } = new();
    public string Reason { get; init; } = string.Empty;
}

public sealed class UpdateUserRolesHandler
{
    private readonly UserManager<User> _userManager;
    private readonly IAdminUserService _adminUserService;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUserService;

    public UpdateUserRolesHandler(
        UserManager<User> userManager,
        IAdminUserService adminUserService,
        IAuditService auditService,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _adminUserService = adminUserService;
        _auditService = auditService;
        _currentUserService = currentUserService;
    }

    public async Task<Result> ExecuteAsync(UpdateUserRolesCommand command, CancellationToken cancellationToken = default)
    {
        var target = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (target is null)
        {
            throw new NotFoundException(BusinessErrors.UserNotFound);
        }

        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        var currentlySuperAdmin = await _userManager.IsInRoleAsync(target, RoleCodes.SuperAdmin);
        var willBeSuperAdmin = command.RoleCodes.Contains(RoleCodes.SuperAdmin, StringComparer.OrdinalIgnoreCase);

        var removesSuperAdmin = currentlySuperAdmin && !willBeSuperAdmin;

        if (removesSuperAdmin)
        {
            if (target.Id == currentUserId)
            {
                throw new AppException(BusinessErrors.CannotDemoteSelf);
            }

            // Đảm bảo không vi phạm luật quản lý đặc quyền
            await _adminUserService.EnsureCanModifyPrivilegedUserAsync(
                target, 
                currentUserId, 
                removesSuperAdmin: true,
                cancellationToken);
        }

        var currentRoles = await _userManager.GetRolesAsync(target);
        
        var rolesToRemove = currentRoles.Except(command.RoleCodes, StringComparer.OrdinalIgnoreCase).ToList();
        var rolesToAdd = command.RoleCodes.Except(currentRoles, StringComparer.OrdinalIgnoreCase).ToList();

        if (rolesToRemove.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(target, rolesToRemove);
            if (!removeResult.Succeeded) return Result.Failure(Error.Failure("IDENTITY.UPDATE_ROLES_FAILED", "Failed to remove roles."));
        }

        if (rolesToAdd.Any())
        {
            var addResult = await _userManager.AddToRolesAsync(target, rolesToAdd);
            if (!addResult.Succeeded) return Result.Failure(Error.Failure("IDENTITY.UPDATE_ROLES_FAILED", "Failed to add roles."));
        }

        // Audit
        await _auditService.WriteAsync(
            action: "users.roles.update",
            targetId: target.Id.ToString(),
            reason: command.Reason,
            cancellationToken);

        return Result.Success();
    }
}
