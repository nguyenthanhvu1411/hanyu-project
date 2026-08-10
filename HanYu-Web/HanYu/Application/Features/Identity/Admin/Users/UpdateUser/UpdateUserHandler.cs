using HanYu.Application.Common.Exceptions;
using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Identity;
using HanYu.Domain.Constants;
using HanYu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HanYu.Application.Interfaces.Persistence;

namespace HanYu.Application.Features.Identity.Admin.Users.UpdateUser;

public sealed record UpdateUserCommand(
    Guid UserId,
    string Email,
    string DisplayName,
    string? Locale,
    string? Status,
    string? ConcurrencyToken);

public sealed class UpdateUserHandler
{
    private readonly UserManager<User> _userManager;
    private readonly IAdminUserService _adminUserService;
    private readonly IHanYuDbContext _context;

    public UpdateUserHandler(
        UserManager<User> userManager,
        IAdminUserService adminUserService,
        IHanYuDbContext context)
    {
        _userManager = userManager;
        _adminUserService = adminUserService;
        _context = context;
    }

    public async Task<Result> ExecuteAsync(UpdateUserCommand command, CancellationToken cancellationToken = default)
    {
        var target = await _userManager.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
            
        if (target is null)
        {
            throw new NotFoundException(BusinessErrors.UserNotFound);
        }

        // Handle concurrency token if provided (simplified for now as Identity handles its own concurrency Stamp)
        if (!string.IsNullOrWhiteSpace(command.ConcurrencyToken) && target.ConcurrencyStamp != command.ConcurrencyToken)
        {
            return Result.Failure(Error.Failure("IDENTITY.CONCURRENCY_CONFLICT", "Dữ liệu đã bị thay đổi bởi người khác."));
        }

        // Update Email if changed
        if (!target.Email!.Equals(command.Email, StringComparison.OrdinalIgnoreCase))
        {
            // check if new email exists
            var existingUser = await _userManager.FindByEmailAsync(command.Email);
            if (existingUser != null && existingUser.Id != target.Id)
            {
                return Result.Failure(Error.Failure("IDENTITY.EMAIL_IN_USE", "Email đã được sử dụng."));
            }
            
            target.UpdateEmail(command.Email);
            target.UpdateUserName(command.Email); // Assuming UserName == Email in this system
        }

        // Update Profile
        if (target.Profile != null)
        {
            target.Profile.UpdateDisplayName(command.DisplayName);
            if (!string.IsNullOrWhiteSpace(command.Locale))
            {
                target.Profile.UpdateLearningPreferences(
                    target.Profile.CurrentHskLevel,
                    target.Profile.DailyGoalMinutes,
                    target.Profile.Timezone,
                    command.Locale);
            }
        }
        
        var result = await _userManager.UpdateAsync(target);
        if (!result.Succeeded)
        {
            return Result.Failure(Error.Failure("IDENTITY.UPDATE_FAILED", "Cập nhật thất bại."));
        }

        return Result.Success();
    }
}
