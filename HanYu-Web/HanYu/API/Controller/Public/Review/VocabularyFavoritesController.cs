using HanYu.API.Common.Extensions;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.Review;

[ApiController]
[Authorize]
[Route("api/v1/public/vocabulary-favorites")]
public sealed class VocabularyFavoritesController
    : ControllerBase
{
    private readonly IReviewService _service;
    private readonly ICurrentUserService _currentUser;

    public VocabularyFavoritesController(
        IReviewService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.GetFavoritesAsync(
                _currentUser.UserId.Value,
                cancellationToken));
    }

    [HttpPost("{vocabularyPublicId:guid}")]
    public async Task<IActionResult> Add(
        Guid vocabularyPublicId,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.FavoriteAsync(
                _currentUser.UserId.Value,
                vocabularyPublicId,
                cancellationToken));
    }

    [HttpDelete("{vocabularyPublicId:guid}")]
    public async Task<IActionResult> Delete(
        Guid vocabularyPublicId,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.UnfavoriteAsync(
                _currentUser.UserId.Value,
                vocabularyPublicId,
                cancellationToken));
    }
}
