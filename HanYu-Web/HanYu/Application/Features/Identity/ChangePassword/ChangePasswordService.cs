using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.ChangePassword;

public sealed class ChangePasswordService
{
    private readonly IIdentityService _identityService;

    public ChangePasswordService(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<Result> ExecuteAsync(
        Guid userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.NewPassword !=
            request.ConfirmPassword)
        {
            return Task.FromResult(
                Result.Failure(
                    Error.Validation(
                        "Identity.PasswordMismatch",
                        "Mật khẩu xác nhận không khớp.")));
        }

        if (request.CurrentPassword ==
            request.NewPassword)
        {
            return Task.FromResult(
                Result.Failure(
                    Error.Validation(
                        "Identity.SamePassword",
                        "Mật khẩu mới phải khác mật khẩu hiện tại.")));
        }

        return _identityService.ChangePasswordAsync(
            userId,
            request.CurrentPassword,
            request.NewPassword,
            cancellationToken);
    }
}
