using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Content.Public.Reports;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Content;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.Content;

[ApiController]
[Authorize]
[Route("api/v1/public/content-reports")]
public sealed class PublicContentReportsController : ControllerBase
{
    private readonly IContentPublicService _service;
    private readonly ICurrentUserService _currentUser;

    public PublicContentReportsController(
        IContentPublicService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitReport(
        [FromBody] CreateContentReportRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.CreateReportAsync(
                _currentUser.UserId.Value,
                request,
                cancellationToken));

    [HttpGet]
    public async Task<IActionResult> GetMyReports(
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetMyReportsAsync(
                _currentUser.UserId.Value,
                cancellationToken));

    [HttpPut("{publicId:guid}")]
    public async Task<IActionResult> UpdateMyReport(
        Guid publicId,
        [FromBody] UpdateContentReportRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.UpdateMyReportAsync(
                _currentUser.UserId.Value,
                publicId,
                request.Description,
                cancellationToken));
}

public sealed record UpdateContentReportRequest(string? Description);
