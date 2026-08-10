using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.ResetPassword;

public sealed class ResetPasswordService
{
    private readonly IIdentityService _identityService;

    public ResetPasswordService(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<Result> ExecuteAsync(
        ResetPasswordRequest request,
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

        return _identityService.ResetPasswordAsync(
            request.Email,
            request.Token,
            request.NewPassword,
            cancellationToken);
    }
}
