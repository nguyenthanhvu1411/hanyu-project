using HanYu.API.Common.Extensions;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Review.Admin.Flashcards;
using HanYu.Application.Interfaces.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controllers.Admin.Review;

[ApiController]
[Route("api/v1/admin/flashcard-sessions")]
[Authorize(Roles = "Admin,Teacher")]
public sealed class ReviewFlashcardSessionsController : ControllerBase
{
    private readonly IReviewAdminService _service;

    public ReviewFlashcardSessionsController(IReviewAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminFlashcardSessionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSessions([FromQuery] AdminFlashcardSessionQuery query, CancellationToken cancellationToken)
    {
        var result = await _service.GetFlashcardSessionsAsync(query, cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(AdminFlashcardSessionDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSession(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetFlashcardSessionAsync(id, cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("{id:long}/abandon")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AbandonSession(long id, CancellationToken cancellationToken)
    {
        var result = await _service.AbandonFlashcardSessionAsync(id, cancellationToken);
        return this.ToActionResult(result);
    }
}
