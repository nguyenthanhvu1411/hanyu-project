using HanYu.API.Common.Extensions;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Learning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.Learning;

[ApiController]
[Authorize]
[Route("api/v1/public/learning/dashboard")]
public sealed class LearningDashboardController : ControllerBase
{
    private readonly ILearningPublicService _learningService;
    private readonly ICurrentUserService _currentUser;

    public LearningDashboardController(
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
        var userId = _currentUser.UserId;

        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var result =
            await _learningService.GetDashboardAsync(
                userId.Value,
                cancellationToken);

        return this.ToActionResult(result);
    }
}
