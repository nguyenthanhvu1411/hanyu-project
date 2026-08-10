using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Review.Admin.Dashboard;
using HanYu.Application.Interfaces.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controllers.Admin.Review;

[ApiController]
[Route("api/v1/admin/review-dashboard")]
[Authorize(Roles = "Admin,Teacher")]
public sealed class ReviewDashboardController : ControllerBase
{
    private readonly IReviewAdminService _service;

    public ReviewDashboardController(IReviewAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(AdminReviewDashboardResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var result = await _service.GetDashboardAsync(cancellationToken);
        return this.ToActionResult(result);
    }
}
