using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Common;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Email;

namespace HanYu.Application.Features.Identity.Register;

public sealed class RegisterService
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;

    public RegisterService(
        IIdentityService identityService,
        IEmailService emailService)
    {
        _identityService = identityService;
        _emailService = emailService;
    }

    public async Task<Result<AuthResponse>> ExecuteAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var identityRequest =
            new RegisterIdentityRequest(
                request.UserName,
                request.Email,
                request.Password,
                request.DisplayName);

        var result =
            await _identityService.RegisterAsync(
                identityRequest,
                cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        var tokenResult =
            await _identityService
                .GenerateEmailVerificationTokenAsync(
                    result.Value.UserId.GetValueOrDefault(),
                    cancellationToken);

        if (tokenResult.IsSuccess)
        {
            try
            {
                await _emailService
                    .SendVerificationEmailAsync(
                        result.Value.Email!,
                        result.Value.DisplayName
                            ?? result.Value.UserName!,
                        result.Value.UserId.Value,
                        tokenResult.Value,
                        cancellationToken);
            }
            catch
            {
                // User vẫn được tạo.
                // Sau này ghi Outbox để retry email.
            }
        }

        return result;
    }
}