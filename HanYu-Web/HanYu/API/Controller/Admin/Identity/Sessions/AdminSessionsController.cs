using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Identity.Admin.Sessions;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Identity.Sessions;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/sessions")]
public sealed class AdminSessionsController : ControllerBase
{
    private readonly GetSessionsHandler _getSessionsHandler;
    private readonly GetSessionByIdHandler _getSessionByIdHandler;
    private readonly RevokeSessionHandler _revokeSessionHandler;

    public AdminSessionsController(
        GetSessionsHandler getSessionsHandler,
        GetSessionByIdHandler getSessionByIdHandler,
        RevokeSessionHandler revokeSessionHandler)
    {
        _getSessionsHandler = getSessionsHandler;
        _getSessionByIdHandler = getSessionByIdHandler;
        _revokeSessionHandler = revokeSessionHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetSessionsQuery query, CancellationToken cancellationToken)
        => this.ToActionResult(await _getSessionsHandler.ExecuteAsync(query, cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
        => this.ToActionResult(await _getSessionByIdHandler.ExecuteAsync(id, cancellationToken));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Revoke(long id, CancellationToken cancellationToken)
        => this.ToActionResult(await _revokeSessionHandler.ExecuteAsync(id, cancellationToken));
}
