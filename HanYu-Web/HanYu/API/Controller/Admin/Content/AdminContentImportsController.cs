using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Content.Admin.Imports;
using HanYu.Application.Interfaces.Content;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Content;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/content-imports")]
public sealed class AdminContentImportsController : ControllerBase
{
    private readonly IContentAdminService _service;

    public AdminContentImportsController(IContentAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] AdminContentImportJobQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetImportJobsAsync(query, cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetImportJobAsync(id, cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateContentImportJobRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.CreateImportJobAsync(request, cancellationToken));

    [HttpPut("{id:long}/source")]
    public async Task<IActionResult> UpdateSource(
        long id,
        [FromBody] UpdateContentImportSourceRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.UpdateImportSourceAsync(id, request, cancellationToken));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.DeleteImportJobAsync(id, cancellationToken));

    [HttpGet("{id:long}/rows")]
    public async Task<IActionResult> GetRows(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetImportRowsAsync(id, cancellationToken));
}
