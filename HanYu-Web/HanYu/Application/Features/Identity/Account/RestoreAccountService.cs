using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Account;

public sealed class RestoreAccountService
{
    private readonly IAccountService _service;

    public RestoreAccountService(
        IAccountService service)
    {
        _service = service;
    }

    public Task<Result> ExecuteAsync(
        Guid publicId,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        return _service.RestoreAsync(
            publicId,
            ipAddress,
            userAgent,
            cancellationToken);
    }
}
