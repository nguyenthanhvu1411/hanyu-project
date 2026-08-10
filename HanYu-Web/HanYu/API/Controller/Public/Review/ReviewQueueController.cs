using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Review.Public.Queue;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.Review;

[ApiController]
[Authorize]
[Route("api/v1/public/review-queue")]
public sealed class ReviewQueueController
    : ControllerBase
{
    private readonly IReviewService _service;
    private readonly ICurrentUserService _currentUser;

    public ReviewQueueController(
        IReviewService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] ReviewQueueQuery query,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.GetQueueAsync(
                _currentUser.UserId.Value,
                query,
                cancellationToken));
    }

    [HttpGet("summary")]
    public async Task<IActionResult> Summary(
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.GetSummaryAsync(
                _currentUser.UserId.Value,
                cancellationToken));
    }
}
