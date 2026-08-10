using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Learning.Public.Activities;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Learning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.Learning;

[ApiController]
[Authorize]
[Route("api/v1/public/learning/activities")]
public sealed class LearningActivitiesController : ControllerBase
{
    private readonly ILearningPublicService _learningService;
    private readonly ICurrentUserService _currentUser;

    public LearningActivitiesController(
        ILearningPublicService learningService,
        ICurrentUserService currentUser)
    {
        _learningService = learningService;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] LearningActivityQuery query,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var result =
            await _learningService.GetMyActivitiesAsync(
                userId.Value,
                query,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest(
                new
                {
                    Code = "Learning.InvalidActivityId",
                    Message = "Learning activity ID không hợp lệ."
                });
        }

        var userId = _currentUser.UserId;

        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var result =
            await _learningService.GetMyActivityAsync(
                userId.Value,
                id,
                cancellationToken);

        return this.ToActionResult(result);
    }
}
