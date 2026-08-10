using HanYu.Application.Features.Quiz.Admin.Quizzes;
using HanYu.Application.Interfaces.Quiz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controllers.Admin.Quiz;

[ApiController]
[Route("api/admin/quizzes")]
[Authorize(Roles = "Admin")]
public class QuizzesController : ControllerBase
{
    private readonly IQuizAdminService _service;

    public QuizzesController(IQuizAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetQuizzes(
        [FromQuery] AdminQuizQuery query,
        CancellationToken ct)
    {
        var result = await _service.GetQuizzesAsync(query, ct);
        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuiz(
        long id,
        CancellationToken ct)
    {
        var result = await _service.GetQuizAsync(id, ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuiz(
        [FromBody] CreateQuizRequest request,
        CancellationToken ct)
    {
        var result = await _service.CreateQuizAsync(request, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetQuiz), new { id = result.Value!.Id }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuiz(
        long id,
        [FromBody] UpdateQuizRequest request,
        CancellationToken ct)
    {
        var result = await _service.UpdateQuizAsync(id, request, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{id}/submit-review")]
    public async Task<IActionResult> SubmitForReview(
        long id,
        CancellationToken ct)
    {
        var result = await _service.SubmitForReviewAsync(id, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(
        long id,
        CancellationToken ct)
    {
        var result = await _service.ApproveAsync(id, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id}/publish")]
    public async Task<IActionResult> Publish(
        long id,
        CancellationToken ct)
    {
        var result = await _service.PublishAsync(id, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id}/archive")]
    public async Task<IActionResult> Archive(
        long id,
        CancellationToken ct)
    {
        var result = await _service.ArchiveAsync(id, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id}/restore")]
    public async Task<IActionResult> Restore(
        long id,
        CancellationToken ct)
    {
        var result = await _service.RestoreAsync(id, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        long id,
        CancellationToken ct)
    {
        var result = await _service.DeleteQuizAsync(id, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
