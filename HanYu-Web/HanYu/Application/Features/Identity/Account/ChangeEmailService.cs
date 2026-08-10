using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Account;

public sealed class ChangeEmailService
{
    private readonly IAccountService _service;

    public ChangeEmailService(IAccountService service)
    {
        _service = service;
    }

    public Task<Result<AccountResponse>> ExecuteAsync(
        Guid userId,
        ChangeEmailRequest request,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        return _service.ChangeEmailAsync(
            userId,
            request,
            ipAddress,
            userAgent,
            cancellationToken);
    }
}
