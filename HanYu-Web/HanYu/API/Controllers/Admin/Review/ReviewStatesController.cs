using HanYu.API.Common.Extensions;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Review.Admin.States;
using HanYu.Application.Interfaces.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controllers.Admin.Review;

[ApiController]
[Route("api/v1/admin/review-states")]
[Authorize(Roles = "Admin,Teacher")]
public sealed class ReviewStatesController : ControllerBase
{
    private readonly IReviewAdminService _service;

    public ReviewStatesController(IReviewAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminVocabularyStateResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStates([FromQuery] AdminVocabularyStateQuery query, CancellationToken cancellationToken)
    {
        var result = await _service.GetStatesAsync(query, cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("{userId:guid}/{vocabularyId:long}")]
    [ProducesResponseType(typeof(AdminVocabularyStateDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetState(Guid userId, long vocabularyId, CancellationToken cancellationToken)
    {
        var result = await _service.GetStateAsync(userId, vocabularyId, cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("{userId:guid}/{vocabularyId:long}/reset")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetState(Guid userId, long vocabularyId, [FromBody] AdminResetVocabularyStateRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.ResetStateAsync(userId, vocabularyId, request, cancellationToken);
        return this.ToActionResult(result);
    }
}
