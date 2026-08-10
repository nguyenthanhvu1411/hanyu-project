using HanYu.API.Common.Extensions;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Preferences.Common;
using HanYu.Application.Features.Identity.Preferences.GetPreferences;
using HanYu.Application.Features.Identity.Preferences.ResetPreferences;
using HanYu.Application.Features.Identity.Preferences.UpdateAudio;
using HanYu.Application.Features.Identity.Preferences.UpdateDisplay;
using HanYu.Application.Features.Identity.Preferences.UpdateFlashcardMode;
using HanYu.Application.Features.Identity.Preferences.UpdateTheme;
using HanYu.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Identity;

[ApiController]
[Authorize]
[Route("api/v1/preferences")]
public sealed class PreferencesController
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromServices]
        GetUserPreferencesService service,

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

    [HttpPut("display")]
    public async Task<IActionResult> UpdateDisplay(
        [FromBody]
        UpdateDisplayPreferencesRequest request,

        [FromServices]
        UpdateDisplayPreferencesService service,

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

    [HttpPut("audio")]
    public async Task<IActionResult> UpdateAudio(
        [FromBody]
        UpdateAudioPreferencesRequest request,

        [FromServices]
        UpdateAudioPreferencesService service,

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

    [HttpPut("theme")]
    public async Task<IActionResult> UpdateTheme(
        [FromBody]
        UpdateThemeRequest request,

        [FromServices]
        UpdateThemeService service,

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

    [HttpPut("flashcard-mode")]
    public async Task<IActionResult>
        UpdateFlashcardMode(
            [FromBody]
            UpdateFlashcardModeRequest request,

            [FromServices]
            UpdateFlashcardModeService service,

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

    [HttpPost("reset")]
    public async Task<IActionResult> Reset(
        [FromServices]
        ResetUserPreferencesService service,

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
