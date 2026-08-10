using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Email;

namespace HanYu.Application.Features.Identity.ResendVerificationEmail;

public sealed class ResendVerificationEmailService
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;

    public ResendVerificationEmailService(
        IIdentityService identityService,
        IEmailService emailService)
    {
        _identityService = identityService;
        _emailService = emailService;
    }

    public async Task<Result> ExecuteAsync(
        ResendVerificationEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult =
            await _identityService
                .FindUserForEmailAsync(
                    request.Email,
                    cancellationToken);

        // Không tiết lộ account existence.
        if (userResult.IsFailure ||
            userResult.Value is null)
        {
            return Result.Success();
        }

        var tokenResult =
            await _identityService
                .GenerateEmailVerificationTokenAsync(
                    userResult.Value.UserId,
                    cancellationToken);

        if (tokenResult.IsFailure)
        {
            return Result.Success();
        }

        await _emailService
            .SendVerificationEmailAsync(
                userResult.Value.Email,
                userResult.Value.DisplayName,
                userResult.Value.UserId,
                tokenResult.Value,
                cancellationToken);

        return Result.Success();
    }
}
