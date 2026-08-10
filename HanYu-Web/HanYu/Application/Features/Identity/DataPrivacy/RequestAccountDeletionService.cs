using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.DataPrivacy;

public sealed class RequestAccountDeletionService
{
    private readonly IAccountService _accountService;

    public RequestAccountDeletionService(
        IAccountService accountService)
    {
        _accountService = accountService;
    }

    public Task<Result> ExecuteAsync(
        Guid userId,
        RequestAccountDeletionRequest request,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        return _accountService.DeleteAsync(
            userId,
            request.Password,
            ipAddress,
            userAgent,
            cancellationToken);
    }
}
