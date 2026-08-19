using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Persistence;
using HanYu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DomainRoles = HanYu.Domain.Constants.Roles;

namespace HanYu.Application.Features.Identity.Admin.Users.CreateUser;

public sealed record CreateAdminUserCommand(
    string Email,
    string Password,
    string DisplayName,
    string? Locale,
    string? Status,
    IReadOnlyCollection<Guid> RoleIds,
    bool EmailVerified);

public sealed class CreateAdminUserHandler
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IHanYuDbContext _context;

    public CreateAdminUserHandler(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IHanYuDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<Result<AdminUserDetailDto>> ExecuteAsync(
        CreateAdminUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var email = command.Email.Trim().ToLowerInvariant();
        var displayName = command.DisplayName.Trim();

        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure<AdminUserDetailDto>(
                Error.Validation("IDENTITY.EMAIL_REQUIRED", "Email không được để trống."));

        if (string.IsNullOrWhiteSpace(displayName))
            return Result.Failure<AdminUserDetailDto>(
                Error.Validation("IDENTITY.DISPLAY_NAME_REQUIRED", "Tên hiển thị không được để trống."));

        if (await _userManager.FindByEmailAsync(email) is not null)
            return Result.Failure<AdminUserDetailDto>(
                Error.Conflict("IDENTITY.EMAIL_IN_USE", "Email đã được sử dụng."));

        var user = new User(email, email)
        {
            EmailConfirmed = command.EmailVerified,
            LockoutEnabled = true
        };

        var createResult = await _userManager.CreateAsync(user, command.Password);
        if (!createResult.Succeeded)
        {
            var message = string.Join(" ", createResult.Errors.Select(error => error.Description));
            return Result.Failure<AdminUserDetailDto>(
                Error.Validation("IDENTITY.CREATE_FAILED", string.IsNullOrWhiteSpace(message)
                    ? "Không thể tạo người dùng."
                    : message));
        }

        try
        {
            var profile = new UserProfile(user.Id, displayName);
            if (!string.IsNullOrWhiteSpace(command.Locale))
            {
                profile.UpdateLearningPreferences(
                    profile.CurrentHskLevel,
                    profile.DailyGoalMinutes,
                    profile.Timezone,
                    command.Locale.Trim());
            }

            _context.Add(profile);
            await _context.SaveChangesAsync(cancellationToken);

            var selectedRoles = command.RoleIds.Count == 0
                ? await _roleManager.Roles
                    .Where(role => role.Code == DomainRoles.User)
                    .ToListAsync(cancellationToken)
                : await _roleManager.Roles
                    .Where(role => command.RoleIds.Contains(role.Id))
                    .ToListAsync(cancellationToken);

            if (command.RoleIds.Count > 0 && selectedRoles.Count != command.RoleIds.Distinct().Count())
            {
                await _userManager.DeleteAsync(user);
                return Result.Failure<AdminUserDetailDto>(
                    Error.Validation("IDENTITY.ROLE_NOT_FOUND", "Có vai trò được chọn không tồn tại."));
            }

            if (selectedRoles.Count > 0)
            {
                var roleResult = await _userManager.AddToRolesAsync(
                    user,
                    selectedRoles.Select(role => role.Name!).Where(name => !string.IsNullOrWhiteSpace(name)));

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    var message = string.Join(" ", roleResult.Errors.Select(error => error.Description));
                    return Result.Failure<AdminUserDetailDto>(
                        Error.Failure("IDENTITY.ASSIGN_ROLE_FAILED", message));
                }
            }

            if (string.Equals(command.Status, "locked", StringComparison.OrdinalIgnoreCase))
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
            }

            var dto = new AdminUserDetailDto
            {
                Id = user.Id,
                Email = user.Email ?? email,
                DisplayName = profile.DisplayName,
                Status = string.Equals(command.Status, "locked", StringComparison.OrdinalIgnoreCase)
                    ? "locked"
                    : "active",
                Locale = profile.UiLanguage,
                EmailVerifiedAt = user.EmailConfirmed ? user.UpdatedAt : null,
                LastLoginAt = user.LastLoginAt,
                FailedLoginCount = user.AccessFailedCount,
                LockedUntil = user.LockoutEnd,
                Roles = selectedRoles.Select(role => new AdminUserRoleDto
                {
                    Id = role.Id,
                    Code = role.Code,
                    Name = role.DisplayName
                }).ToList(),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                DeletedAt = user.DeletedAt,
                ConcurrencyToken = user.ConcurrencyStamp ?? string.Empty,
                Permissions = [],
                SessionCount = 0,
                ActiveSessionCount = 0
            };

            return Result.Success(dto);
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }
    }
}
