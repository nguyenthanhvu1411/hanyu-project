using HanYu.API.Common.Extensions;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Review.Admin.Events;
using HanYu.Application.Interfaces.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controllers.Admin.Review;

[ApiController]
[Route("api/v1/admin/review-events")]
[Authorize(Roles = "Admin,Teacher")]
public sealed class ReviewEventsController : ControllerBase
{
    private readonly IReviewAdminService _service;

    public ReviewEventsController(IReviewAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminReviewEventResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEvents([FromQuery] AdminReviewEventQuery query, CancellationToken cancellationToken)
    {
        var result = await _service.GetEventsAsync(query, cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(AdminReviewEventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEvent(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetEventAsync(id, cancellationToken);
        return this.ToActionResult(result);
    }
}
