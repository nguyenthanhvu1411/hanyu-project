using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Content.Admin.Reports;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Content;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Content;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/content-reports")]
public sealed class AdminContentReportsController : ControllerBase
{
    private readonly IContentAdminService _service;
    private readonly ICurrentUserService _currentUser;

    public AdminContentReportsController(
        IContentAdminService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] AdminContentReportQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetReportsAsync(query, cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetReportAsync(id, cancellationToken));

    [HttpPost("{id:long}/start-review")]
    public async Task<IActionResult> StartReview(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.StartReportReviewAsync(id, cancellationToken));

    [HttpPost("{id:long}/resolve")]
    public async Task<IActionResult> ResolveReport(
        long id,
        [FromBody] ResolveContentReportRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.ResolveReportAsync(
                id,
                _currentUser.UserId.Value,
                request,
                cancellationToken));

    [HttpPost("{id:long}/reject")]
    public async Task<IActionResult> RejectReport(
        long id,
        [FromBody] ResolveContentReportRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.RejectReportAsync(
                id,
                _currentUser.UserId.Value,
                request,
                cancellationToken));

    [HttpPost("{id:long}/reopen")]
    public async Task<IActionResult> Reopen(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.ReopenReportAsync(id, cancellationToken));
}
