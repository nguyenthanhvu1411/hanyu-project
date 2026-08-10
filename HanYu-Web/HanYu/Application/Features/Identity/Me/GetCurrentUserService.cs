using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Common;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Me;

public sealed class GetCurrentUserService
{
    private readonly IIdentityService _identityService;

    public GetCurrentUserService(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<Result<CurrentUserResponse>> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _identityService.GetCurrentUserAsync(
            userId,
            cancellationToken);
    }
}