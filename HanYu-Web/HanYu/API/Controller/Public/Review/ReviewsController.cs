using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Review.Public.Review;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.Review;

[ApiController]
[Authorize]
[Route("api/v1/public/reviews")]
public sealed class ReviewsController
    : ControllerBase
{
    private readonly IReviewService _service;
    private readonly ICurrentUserService _currentUser;

    public ReviewsController(
        IReviewService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<IActionResult> Submit(
        SubmitReviewRequest request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.SubmitReviewAsync(
                _currentUser.UserId.Value,
                request,
                cancellationToken));
    }

    [HttpGet("vocabularies/{vocabularyPublicId:guid}/state")]
    public async Task<IActionResult> GetState(
        Guid vocabularyPublicId,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.GetStateAsync(
                _currentUser.UserId.Value,
                vocabularyPublicId,
                cancellationToken));
    }
}
