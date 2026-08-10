using HanYu.API.Common.Extensions;
using HanYu.Application.Features.AI.Admin.Cache;
using HanYu.Application.Features.AI.Admin.Conversations;
using HanYu.Application.Features.AI.Admin.Feedback;
using HanYu.Application.Features.AI.Admin.Requests;
using HanYu.Application.Interfaces.AI;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.AI;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/ai")]
public sealed class AdminAiController : ControllerBase
{
    private readonly IAiAdminService _service;

    public AdminAiController(IAiAdminService service)
    {
        _service = service;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetDashboardAsync(cancellationToken));

    [HttpGet("requests")]
    public async Task<IActionResult> GetRequests(
        [FromQuery] AdminAiRequestQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetRequestsAsync(query, cancellationToken));

    [HttpGet("requests/{id:long}")]
    public async Task<IActionResult> GetRequest(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetRequestAsync(id, cancellationToken));

    [HttpPost("requests/{id:long}/cancel")]
    public async Task<IActionResult> CancelRequest(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.CancelRequestAsync(id, cancellationToken));

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations(
        [FromQuery] AdminAiConversationQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetConversationsAsync(query, cancellationToken));

    [HttpGet("conversations/{id:long}")]
    public async Task<IActionResult> GetConversation(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetConversationAsync(id, cancellationToken));

    [HttpGet("feedback")]
    public async Task<IActionResult> GetFeedback(
        [FromQuery] AdminAiFeedbackQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetFeedbacksAsync(query, cancellationToken));

    [HttpGet("cache")]
    public async Task<IActionResult> GetCache(
        [FromQuery] AdminAiCacheQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetCacheAsync(query, cancellationToken));

    [HttpDelete("cache/{id:long}")]
    public async Task<IActionResult> DeleteCacheEntry(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.DeleteExpiredCacheEntryAsync(id, cancellationToken));

    [HttpDelete("cache/expired")]
    public async Task<IActionResult> DeleteExpiredCache(
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.DeleteExpiredCacheAsync(cancellationToken));
}
