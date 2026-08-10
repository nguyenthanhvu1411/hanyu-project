using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.VerifyEmail;

public sealed class VerifyEmailService
{
    private readonly IIdentityService _identityService;

    public VerifyEmailService(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<Result> ExecuteAsync(
        VerifyEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        return _identityService.VerifyEmailAsync(
            request.UserId,
            request.Token,
            cancellationToken);
    }
}
