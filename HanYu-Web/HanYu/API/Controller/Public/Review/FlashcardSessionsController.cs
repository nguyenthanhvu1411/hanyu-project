using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Review.Public.Flashcards;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.Review;

[ApiController]
[Authorize]
[Route("api/v1/public/flashcard-sessions")]
public sealed class FlashcardSessionsController
    : ControllerBase
{
    private readonly IFlashcardService _service;
    private readonly ICurrentUserService _currentUser;

    public FlashcardSessionsController(
        IFlashcardService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateFlashcardSessionRequest request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.CreateSessionAsync(
                _currentUser.UserId.Value,
                request,
                cancellationToken));
    }

    [HttpGet("{sessionPublicId:guid}")]
    public async Task<IActionResult> Get(
        Guid sessionPublicId,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.GetSessionAsync(
                _currentUser.UserId.Value,
                sessionPublicId,
                cancellationToken));
    }

    [HttpPost(
        "{sessionPublicId:guid}/items/{itemPublicId:guid}/answer")]
    public async Task<IActionResult> Answer(
        Guid sessionPublicId,
        Guid itemPublicId,
        AnswerFlashcardRequest request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.AnswerAsync(
                _currentUser.UserId.Value,
                sessionPublicId,
                itemPublicId,
                request,
                cancellationToken));
    }

    [HttpPost("{sessionPublicId:guid}/abandon")]
    public async Task<IActionResult> Abandon(
        Guid sessionPublicId,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.AbandonAsync(
                _currentUser.UserId.Value,
                sessionPublicId,
                cancellationToken));
    }
}
