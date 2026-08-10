using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Quiz.Public.Answers;
using HanYu.Application.Features.Quiz.Public.Attempts;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Quiz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controllers.Public.Quiz;

[ApiController]
[Route("api/v1/public/quizzes")]
public sealed class QuizzesController : ControllerBase
{
    private readonly IQuizPublicService _service;
    private readonly ICurrentUserService _currentUser;

    public QuizzesController(
        IQuizPublicService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    // Public endpoints — no auth required
    [HttpGet]
    public async Task<IActionResult> GetQuizzes(
        [FromQuery] Guid? lessonPublicId,
        CancellationToken ct)
    {
        var result = await _service.GetQuizzesAsync(lessonPublicId, ct);
        return this.ToActionResult(result);
    }

    [HttpGet("{publicId:guid}")]
    public async Task<IActionResult> GetQuiz(
        Guid publicId,
        CancellationToken ct)
    {
        var result = await _service.GetQuizAsync(publicId, ct);
        return this.ToActionResult(result);
    }

    // Auth-required endpoints — always pass userId explicitly
    [Authorize]
    [HttpPost("{publicId:guid}/attempts")]
    public async Task<IActionResult> StartAttempt(
        Guid publicId,
        [FromBody] StartQuizAttemptRequest request,
        CancellationToken ct)
    {
        if (!_currentUser.UserId.HasValue) return Unauthorized();
        var result = await _service.StartAttemptAsync(_currentUser.UserId.Value, publicId, request, ct);
        return this.ToActionResult(result);
    }

    [Authorize]
    [HttpGet("attempts/{attemptPublicId:guid}")]
    public async Task<IActionResult> GetAttempt(
        Guid attemptPublicId,
        CancellationToken ct)
    {
        if (!_currentUser.UserId.HasValue) return Unauthorized();
        var result = await _service.GetAttemptAsync(_currentUser.UserId.Value, attemptPublicId, ct);
        return this.ToActionResult(result);
    }

    [Authorize]
    [HttpPost("attempts/{attemptPublicId:guid}/questions/{questionPublicId:guid}/answer")]
    public async Task<IActionResult> SubmitAnswer(
        Guid attemptPublicId,
        Guid questionPublicId,
        [FromBody] SubmitQuizAnswerRequest request,
        CancellationToken ct)
    {
        if (!_currentUser.UserId.HasValue) return Unauthorized();
        var result = await _service.SubmitAnswerAsync(
            _currentUser.UserId.Value, attemptPublicId, questionPublicId, request, ct);
        return this.ToActionResult(result);
    }

    [Authorize]
    [HttpPost("attempts/{attemptPublicId:guid}/submit")]
    public async Task<IActionResult> SubmitAttempt(
        Guid attemptPublicId,
        CancellationToken ct)
    {
        if (!_currentUser.UserId.HasValue) return Unauthorized();
        var result = await _service.SubmitAttemptAsync(_currentUser.UserId.Value, attemptPublicId, ct);
        return this.ToActionResult(result);
    }

    [Authorize]
    [HttpGet("attempts/{attemptPublicId:guid}/result")]
    public async Task<IActionResult> GetResult(
        Guid attemptPublicId,
        CancellationToken ct)
    {
        if (!_currentUser.UserId.HasValue) return Unauthorized();
        var result = await _service.GetResultAsync(_currentUser.UserId.Value, attemptPublicId, ct);
        return this.ToActionResult(result);
    }

    [Authorize]
    [HttpGet("my-history")]
    public async Task<IActionResult> GetMyHistory(CancellationToken ct)
    {
        if (!_currentUser.UserId.HasValue) return Unauthorized();
        var result = await _service.GetMyHistoryAsync(_currentUser.UserId.Value, ct);
        return this.ToActionResult(result);
    }
}
