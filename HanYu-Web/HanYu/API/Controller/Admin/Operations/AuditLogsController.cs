using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Operations.Admin.AuditLogs;
using HanYu.Application.Interfaces.Operations;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Operations;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/audit-logs")]
public sealed class AuditLogsController : ControllerBase
{
    private readonly IOperationsAdminService _service;

    public AuditLogsController(IOperationsAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] AdminAuditLogQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetAuditLogsAsync(query, cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetAuditLogAsync(id, cancellationToken));
}
