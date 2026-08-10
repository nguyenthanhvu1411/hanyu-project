using HanYu.API.Common.Extensions;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Account;
using HanYu.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Identity;

[ApiController]
[Authorize]
[Route("api/v1/account")]
public sealed class AccountController : ControllerBase
{
    [HttpPut("email")]
    public async Task<IActionResult> ChangeEmail(
        ChangeEmailRequest request,
        ChangeEmailService service,
        ICurrentUserService currentUser,
        CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue)
            return Unauthorized();

        var result =
            await service.ExecuteAsync(
                currentUser.UserId.Value,
                request,
                GetIpAddress(),
                Request.Headers.UserAgent.ToString(),
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("username")]
    public async Task<IActionResult> ChangeUsername(
        ChangeUsernameRequest request,
        ChangeUsernameService service,
        ICurrentUserService currentUser,
        CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue)
            return Unauthorized();

        var result =
            await service.ExecuteAsync(
                currentUser.UserId.Value,
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(
        DeleteAccountRequest request,
        DeleteAccountService service,
        ICurrentUserService currentUser,
        CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue)
            return Unauthorized();

        var result =
            await service.ExecuteAsync(
                currentUser.UserId.Value,
                request,
                GetIpAddress(),
                Request.Headers.UserAgent.ToString(),
                cancellationToken);

        return this.ToActionResult(result);
    }

    private string? GetIpAddress() =>
        HttpContext.Connection
            .RemoteIpAddress?
            .ToString();


}
