using HanYu.API.Common.Extensions;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Quiz.Admin.Attempts;
using HanYu.Application.Interfaces.Quiz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Quiz;

[ApiController]
[Route("api/v1/admin/quiz-attempts")]
[Authorize(Roles = "Admin,Teacher")]
public sealed class QuizAttemptsController : ControllerBase
{
    private readonly IQuizAttemptAdminService _service;

    public QuizAttemptsController(IQuizAttemptAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminQuizAttemptResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAttempts(
        [FromQuery] AdminQuizAttemptQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.GetAttemptsAsync(query, cancellationToken));

    [HttpGet("statistics")]
    [ProducesResponseType(typeof(AdminQuizAttemptStatisticsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatistics(
        [FromQuery] AdminQuizAttemptQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.GetStatisticsAsync(query, cancellationToken));

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(AdminQuizAttemptDetailResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAttempt(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.GetAttemptAsync(id, cancellationToken));
}