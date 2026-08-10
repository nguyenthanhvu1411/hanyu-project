using HanYu.API.Common.Extensions;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.DataPrivacy;
using HanYu.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Identity;

[ApiController]
[Authorize]
[Route("api/v1/privacy")]
public sealed class DataPrivacyController
    : ControllerBase
{
    [HttpGet("consents")]
    public async Task<IActionResult> GetConsents(
        GetConsentsService service,
        ICurrentUserService currentUser,
        CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue)
            return Unauthorized();

        var result =
            await service.ExecuteAsync(
                currentUser.UserId.Value,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("consents")]
    public async Task<IActionResult> UpdateConsent(
        UpdateConsentRequest request,
        UpdateConsentService service,
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

    [HttpPost("data-exports")]
    public async Task<IActionResult> RequestExport(
        RequestDataExportService service,
        ICurrentUserService currentUser,
        CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue)
            return Unauthorized();

        var result =
            await service.ExecuteAsync(
                currentUser.UserId.Value,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("data-exports")]
    public async Task<IActionResult> GetExports(
        GetDataExportsService service,
        ICurrentUserService currentUser,
        CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue)
            return Unauthorized();

        var result =
            await service.ExecuteAsync(
                currentUser.UserId.Value,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("data-exports/download")]
    public async Task<IActionResult> DownloadExport(
        [FromServices]
        GetDataExportDownloadService service,

        [FromServices]
        ICurrentUserService currentUser,

        CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue)
            return Unauthorized();

        var result =
            await service.ExecuteAsync(
                currentUser.UserId.Value,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("account-deletion")]
    public async Task<IActionResult> DeleteAccount(
        RequestAccountDeletionRequest request,
        RequestAccountDeletionService service,
        ICurrentUserService currentUser,
        CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue)
            return Unauthorized();

        var result =
            await service.ExecuteAsync(
                currentUser.UserId.Value,
                request,
                HttpContext.Connection
                    .RemoteIpAddress?
                    .ToString(),
                Request.Headers.UserAgent.ToString(),
                cancellationToken);

        return this.ToActionResult(result);
    }


}
