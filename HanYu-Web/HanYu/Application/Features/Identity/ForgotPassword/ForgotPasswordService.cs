using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Email;

namespace HanYu.Application.Features.Identity.ForgotPassword;

public sealed class ForgotPasswordService
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;

    public ForgotPasswordService(
        IIdentityService identityService,
        IEmailService emailService)
    {
        _identityService = identityService;
        _emailService = emailService;
    }

    public async Task<Result> ExecuteAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        // Luôn trả Success ngoài API để tránh
        // account enumeration.

        var userResult =
            await _identityService
                .FindUserForEmailAsync(
                    request.Email,
                    cancellationToken);

        if (userResult.IsFailure ||
            userResult.Value is null)
        {
            return Result.Success();
        }

        var tokenResult =
            await _identityService
                .GeneratePasswordResetTokenAsync(
                    request.Email,
                    cancellationToken);

        if (tokenResult.IsFailure ||
            string.IsNullOrWhiteSpace(
                tokenResult.Value))
        {
            return Result.Success();
        }

        await _emailService
            .SendResetPasswordEmailAsync(
                userResult.Value.Email,
                userResult.Value.DisplayName,
                tokenResult.Value,
                cancellationToken);

        return Result.Success();
    }
}
