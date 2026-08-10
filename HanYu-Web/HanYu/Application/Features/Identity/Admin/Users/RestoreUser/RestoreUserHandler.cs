using HanYu.Application.Common.Exceptions;
using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Operations;
using HanYu.Domain.Constants;
using HanYu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace HanYu.Application.Features.Identity.Admin.Users.RestoreUser;

public sealed class RestoreUserHandler
{
    private readonly UserManager<User> _userManager;
    private readonly IAuditService _auditService;

    public RestoreUserHandler(
        UserManager<User> userManager,
        IAuditService auditService)
    {
        _userManager = userManager;
        _auditService = auditService;
    }

    public async Task<Result> ExecuteAsync(Guid targetUserId, CancellationToken cancellationToken = default)
    {
        var target = await _userManager.FindByIdAsync(targetUserId.ToString());
        if (target is null)
        {
            throw new NotFoundException(BusinessErrors.UserNotFound);
        }

        // Restore
        target.Restore();
        var result = await _userManager.UpdateAsync(target);
        if (!result.Succeeded)
        {
            return Result.Failure(Error.Failure("IDENTITY.RESTORE_FAILED", "Failed to restore user."));
        }

        // Audit
        await _auditService.WriteAsync(
            action: "users.restore",
            targetId: target.Id.ToString(),
            reason: "Restore user",
            cancellationToken);

        return Result.Success();
    }
}
