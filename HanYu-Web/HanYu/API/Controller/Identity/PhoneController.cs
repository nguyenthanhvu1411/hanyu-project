using HanYu.API.Common.Extensions;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Phone;
using HanYu.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Identity;

[ApiController]
[Authorize]
[Route("api/v1/account/phone")]
public sealed class PhoneController : ControllerBase
{
    [HttpPut]
    public async Task<IActionResult> Update(
        UpdatePhoneNumberRequest request,
        UpdatePhoneNumberService service,
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

    [HttpPost("send-verification")]
    public async Task<IActionResult> SendVerification(
        [FromServices]
        SendPhoneVerificationService service,

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

    [HttpPost("verify")]
    public async Task<IActionResult> Verify(
        VerifyPhoneNumberRequest request,
        VerifyPhoneNumberService service,
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


}
