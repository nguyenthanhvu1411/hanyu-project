using HanYu.Application.Common.Exceptions;
using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Identity;
using HanYu.Application.Interfaces.Operations;
using HanYu.Domain.Constants;
using HanYu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace HanYu.Application.Features.Identity.Admin.Users.UnlockUser;

public sealed class UnlockUserHandler
{
    private readonly UserManager<User> _userManager;
    private readonly IAdminUserService _adminUserService;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUserService;

    public UnlockUserHandler(
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

    public async Task<Result> ExecuteAsync(Guid targetUserId, string reason, CancellationToken cancellationToken = default)
    {
        var target = await _userManager.FindByIdAsync(targetUserId.ToString());
        if (target is null)
        {
            throw new NotFoundException(BusinessErrors.UserNotFound);
        }

        // Unlock account
        var result = await _userManager.SetLockoutEndDateAsync(target, null);
        if (!result.Succeeded)
        {
            return Result.Failure(Error.Failure("IDENTITY.UNLOCK_FAILED", "Failed to unlock user."));
        }

        // Reset access failed count
        await _userManager.ResetAccessFailedCountAsync(target);

        // Audit
        await _auditService.WriteAsync(
            action: "users.unlock",
            targetId: target.Id.ToString(),
            reason: reason,
            cancellationToken);

        return Result.Success();
    }
}
