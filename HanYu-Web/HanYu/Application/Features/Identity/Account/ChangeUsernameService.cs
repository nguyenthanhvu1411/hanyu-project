using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Account;

public sealed class ChangeUsernameService
{
    private readonly IAccountService _service;

    public ChangeUsernameService(
        IAccountService service)
    {
        _service = service;
    }

    public Task<Result<AccountResponse>> ExecuteAsync(
        Guid userId,
        ChangeUsernameRequest request,
        CancellationToken cancellationToken = default)
    {
        return _service.ChangeUsernameAsync(
            userId,
            request,
            cancellationToken);
    }
}
