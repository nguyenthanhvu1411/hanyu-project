using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Learning.Public.Goal;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Learning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.Learning;

[ApiController]
[Authorize]
[Route("api/v1/public/learning/goal")]
public sealed class LearningGoalController : ControllerBase
{
    private readonly ILearningPublicService _learningService;
    private readonly ICurrentUserService _currentUser;

    public LearningGoalController(
        ILearningPublicService learningService,
        ICurrentUserService currentUser)
    {
        _learningService = learningService;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var result =
            await _learningService.GetMyGoalAsync(
                userId.Value,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        [FromBody] UpdateLearningGoalRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var result =
            await _learningService.UpdateMyGoalAsync(
                userId.Value,
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("pause")]
    public async Task<IActionResult> Pause(
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var result =
            await _learningService.PauseMyGoalAsync(
                userId.Value,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("resume")]
    public async Task<IActionResult> Resume(
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var result =
            await _learningService.ResumeMyGoalAsync(
                userId.Value,
                cancellationToken);

        return this.ToActionResult(result);
    }

    private Guid? GetCurrentUserId()
    {
        return _currentUser.UserId;
    }
}
