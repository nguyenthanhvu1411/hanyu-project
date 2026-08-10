using HanYu.API.Common.Extensions;
using HanYu.Application.Features.AI.Public.Conversations;
using HanYu.Application.Features.AI.Public.Feedback;
using HanYu.Application.Interfaces.AI;
using HanYu.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.AI;

[ApiController]
[Authorize]
[Route("api/v1/public/ai")]
public sealed class PublicAiController : ControllerBase
{
    private readonly IAiPublicService _service;
    private readonly ICurrentUserService _currentUser;

    public PublicAiController(
        IAiPublicService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetMyConversations(
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetMyConversationsAsync(
                _currentUser.UserId.Value,
                cancellationToken));

    [HttpGet("conversations/{publicId:guid}")]
    public async Task<IActionResult> GetConversation(
        Guid publicId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetConversationAsync(
                _currentUser.UserId.Value,
                publicId,
                cancellationToken));

    [HttpPost("conversations")]
    public async Task<IActionResult> CreateConversation(
        [FromBody] CreateAiConversationRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.CreateConversationAsync(
                _currentUser.UserId.Value,
                request,
                cancellationToken));

    [HttpPut("conversations/{publicId:guid}/title")]
    public async Task<IActionResult> UpdateTitle(
        Guid publicId,
        [FromBody] UpdateAiConversationTitleRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.UpdateConversationTitleAsync(
                _currentUser.UserId.Value,
                publicId,
                request.Title,
                cancellationToken));

    [HttpPost("conversations/{publicId:guid}/archive")]
    public async Task<IActionResult> ArchiveConversation(
        Guid publicId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.ArchiveConversationAsync(
                _currentUser.UserId.Value,
                publicId,
                cancellationToken));

    [HttpPost("conversations/{publicId:guid}/messages")]
    public async Task<IActionResult> SendMessage(
        Guid publicId,
        [FromBody] SendAiMessageRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.SendMessageAsync(
                _currentUser.UserId.Value,
                publicId,
                request,
                cancellationToken));

    [HttpPost("feedback")]
    public async Task<IActionResult> SubmitFeedback(
        [FromBody] SubmitAiFeedbackRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.SubmitFeedbackAsync(
                _currentUser.UserId.Value,
                request,
                cancellationToken));
}

public sealed record UpdateAiConversationTitleRequest(string? Title);
