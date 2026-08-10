using HanYu.Application.Common.Exceptions;
using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Identity;
using HanYu.Application.Interfaces.Operations;
using HanYu.Domain.Constants;
using HanYu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace HanYu.Application.Features.Identity.Admin.Users.DeleteUser;

public sealed class DeleteUserHandler
{
    private readonly UserManager<User> _userManager;
    private readonly IAdminUserService _adminUserService;
    private readonly ISessionService _sessionService;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUserService;

    public DeleteUserHandler(
        UserManager<User> userManager,
        IAdminUserService adminUserService,
        ISessionService sessionService,
        IAuditService auditService,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _adminUserService = adminUserService;
        _sessionService = sessionService;
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

        var currentUserId = _currentUserService.UserId ?? Guid.Empty;

        // Đảm bảo không vi phạm luật quản lý đặc quyền
        await _adminUserService.EnsureCanModifyPrivilegedUserAsync(
            target, 
            currentUserId, 
            removesSuperAdmin: true, // Việc xoá xem như là loại bỏ SuperAdmin khỏi trạng thái hoạt động
            cancellationToken);

        // Soft Delete
        target.SoftDelete();
        var result = await _userManager.UpdateAsync(target);
        if (!result.Succeeded)
        {
            return Result.Failure(Error.Failure("IDENTITY.DELETE_FAILED", "Failed to delete user."));
        }

        // Revoke all sessions
        await _sessionService.RevokeAllAsync(target.Id, reason, cancellationToken);

        // Audit
        await _auditService.WriteAsync(
            action: "users.delete",
            targetId: target.Id.ToString(),
            reason: reason,
            cancellationToken);

        return Result.Success();
    }
}
