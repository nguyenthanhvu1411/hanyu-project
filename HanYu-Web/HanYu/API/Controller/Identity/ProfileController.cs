using HanYu.API.Common.Extensions;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Profile.GetProfile;
using HanYu.Application.Features.Identity.Profile.Onboarding;
using HanYu.Application.Features.Identity.Profile.UpdateProfile;
using HanYu.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Identity;

[ApiController]
[Authorize]
[Route("api/v1/profile")]
public sealed class ProfileController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProfile(
        [FromServices]
        GetUserProfileService service,

        [FromServices]
        ICurrentUserService currentUser,

        CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue)
        {
            return Unauthorized();
        }

        var result =
            await service.ExecuteAsync(
                currentUser.UserId.Value,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile(
        [FromBody]
        UpdateUserProfileRequest request,

        [FromServices]
        UpdateUserProfileService service,

        [FromServices]
        ICurrentUserService currentUser,

        CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue)
        {
            return Unauthorized();
        }

        var result =
            await service.ExecuteAsync(
                currentUser.UserId.Value,
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("onboarding/complete")]
    public async Task<IActionResult> CompleteOnboarding(
        [FromServices]
        CompleteOnboardingService service,

        [FromServices]
        ICurrentUserService currentUser,

        CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue)
        {
            return Unauthorized();
        }

        var result =
            await service.ExecuteAsync(
                currentUser.UserId.Value,
                cancellationToken);

        return this.ToActionResult(result);
    }


}
