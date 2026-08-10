using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Account;

public sealed class DeleteAccountService
{
    private readonly IAccountService _service;

    public DeleteAccountService(
        IAccountService service)
    {
        _service = service;
    }

    public Task<Result> ExecuteAsync(
        Guid userId,
        DeleteAccountRequest request,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        return _service.DeleteAsync(
            userId,
            request.Password,
            ipAddress,
            userAgent,
            cancellationToken);
    }
}
