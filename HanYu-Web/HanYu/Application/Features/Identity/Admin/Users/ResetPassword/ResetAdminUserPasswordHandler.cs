using HanYu.Application.Common.Models;
using HanYu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace HanYu.Application.Features.Identity.Admin.Users.ResetPassword;

public sealed record ResetAdminUserPasswordCommand(
    Guid UserId,
    string NewPassword);

public sealed class ResetAdminUserPasswordHandler
{
    private readonly UserManager<User> _userManager;

    public ResetAdminUserPasswordHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> ExecuteAsync(
        ResetAdminUserPasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null || user.IsDeleted)
        {
            return Result.Failure(
                Error.NotFound("User.NotFound", "Người dùng không tồn tại."));
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, command.NewPassword);

        if (!result.Succeeded)
        {
            var message = string.Join(" ", result.Errors.Select(error => error.Description));
            return Result.Failure(
                Error.Validation(
                    "IDENTITY.RESET_PASSWORD_FAILED",
                    string.IsNullOrWhiteSpace(message)
                        ? "Không thể đặt lại mật khẩu."
                        : message));
        }

        // Đổi security stamp để vô hiệu hóa thông tin xác thực cũ khi cơ chế
        // kiểm tra security stamp được áp dụng và buộc phiên đăng nhập mới dùng mật khẩu mới.
        await _userManager.UpdateSecurityStampAsync(user);

        return Result.Success();
    }
}
