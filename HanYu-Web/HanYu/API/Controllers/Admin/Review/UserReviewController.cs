using HanYu.API.Common.Extensions;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Review.Admin.Users;
using HanYu.Application.Interfaces.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controllers.Admin.Review;

[ApiController]
[Route("api/v1/admin/users/{userId:guid}/review-summary")]
[Authorize(Roles = "Admin,Teacher")]
public sealed class UserReviewController : ControllerBase
{
    private readonly IReviewAdminService _service;

    public UserReviewController(IReviewAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(AdminUserReviewSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSummary(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _service.GetUserSummaryAsync(userId, cancellationToken);
        return this.ToActionResult(result);
    }
}
