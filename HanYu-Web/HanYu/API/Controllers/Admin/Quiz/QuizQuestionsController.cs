using HanYu.Application.Features.Quiz.Admin.MatchingPairs;
using HanYu.Application.Features.Quiz.Admin.Options;
using HanYu.Application.Features.Quiz.Admin.Questions;
using HanYu.Application.Interfaces.Quiz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controllers.Admin.Quiz;

[ApiController]
[Route("api/admin/quizzes/{quizId}/questions")]
[Authorize(Roles = "Admin")]
public class QuizQuestionsController : ControllerBase
{
    private readonly IQuizAdminService _service;

    public QuizQuestionsController(IQuizAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetQuestions(
        long quizId,
        CancellationToken ct)
    {
        var result = await _service.GetQuestionsAsync(quizId, ct);
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuestion(
        long quizId,
        [FromBody] CreateQuizQuestionRequest request,
        CancellationToken ct)
    {
        var result = await _service.CreateQuestionAsync(quizId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{questionId}")]
    public async Task<IActionResult> UpdateQuestion(
        long quizId,
        long questionId,
        [FromBody] UpdateQuizQuestionRequest request,
        CancellationToken ct)
    {
        var result = await _service.UpdateQuestionAsync(quizId, questionId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{questionId}/submit-review")]
    public async Task<IActionResult> SubmitForReview(
        long quizId,
        long questionId,
        CancellationToken ct)
    {
        var result = await _service.SubmitQuestionForReviewAsync(quizId, questionId, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{questionId}/approve")]
    public async Task<IActionResult> Approve(
        long quizId,
        long questionId,
        CancellationToken ct)
    {
        var result = await _service.ApproveQuestionAsync(quizId, questionId, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{questionId}/publish")]
    public async Task<IActionResult> Publish(
        long quizId,
        long questionId,
        CancellationToken ct)
    {
        var result = await _service.PublishQuestionAsync(quizId, questionId, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{questionId}/archive")]
    public async Task<IActionResult> Archive(
        long quizId,
        long questionId,
        CancellationToken ct)
    {
        var result = await _service.ArchiveQuestionAsync(quizId, questionId, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{questionId}/restore")]
    public async Task<IActionResult> Restore(
        long quizId,
        long questionId,
        CancellationToken ct)
    {
        var result = await _service.RestoreQuestionAsync(quizId, questionId, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{questionId}")]
    public async Task<IActionResult> DeleteQuestion(
        long quizId,
        long questionId,
        CancellationToken ct)
    {
        var result = await _service.DeleteQuestionAsync(quizId, questionId, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    // ==========================================
    // Options
    // ==========================================

    [HttpGet("{questionId}/options")]
    public async Task<IActionResult> GetOptions(
        long questionId,
        CancellationToken ct)
    {
        var result = await _service.GetOptionsAsync(questionId, ct);
        return Ok(result.Value);
    }

    [HttpPost("{questionId}/options")]
    public async Task<IActionResult> CreateOption(
        long questionId,
        [FromBody] CreateQuizQuestionOptionRequest request,
        CancellationToken ct)
    {
        var result = await _service.CreateOptionAsync(questionId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{questionId}/options/{optionId}")]
    public async Task<IActionResult> UpdateOption(
        long questionId,
        long optionId,
        [FromBody] UpdateQuizQuestionOptionRequest request,
        CancellationToken ct)
    {
        var result = await _service.UpdateOptionAsync(questionId, optionId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{questionId}/options/{optionId}")]
    public async Task<IActionResult> DeleteOption(
        long questionId,
        long optionId,
        CancellationToken ct)
    {
        var result = await _service.DeleteOptionAsync(questionId, optionId, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    // ==========================================
    // Matching Pairs
    // ==========================================

    [HttpGet("{questionId}/matching-pairs")]
    public async Task<IActionResult> GetMatchingPairs(
        long questionId,
        CancellationToken ct)
    {
        var result = await _service.GetMatchingPairsAsync(questionId, ct);
        return Ok(result.Value);
    }

    [HttpPost("{questionId}/matching-pairs")]
    public async Task<IActionResult> CreateMatchingPair(
        long questionId,
        [FromBody] CreateQuizMatchingPairRequest request,
        CancellationToken ct)
    {
        var result = await _service.CreateMatchingPairAsync(questionId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{questionId}/matching-pairs/{pairId}")]
    public async Task<IActionResult> UpdateMatchingPair(
        long questionId,
        long pairId,
        [FromBody] UpdateQuizMatchingPairRequest request,
        CancellationToken ct)
    {
        var result = await _service.UpdateMatchingPairAsync(questionId, pairId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{questionId}/matching-pairs/{pairId}")]
    public async Task<IActionResult> DeleteMatchingPair(
        long questionId,
        long pairId,
        CancellationToken ct)
    {
        var result = await _service.DeleteMatchingPairAsync(questionId, pairId, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    // ==========================================
    // Tags (Question - Tag relationships)
    // ==========================================

    [HttpPost("{questionId}/tags/{tagId}")]
    public async Task<IActionResult> AttachTag(
        long questionId,
        long tagId,
        CancellationToken ct)
    {
        var result = await _service.AttachTagAsync(questionId, tagId, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{questionId}/tags/{tagId}")]
    public async Task<IActionResult> DetachTag(
        long questionId,
        long tagId,
        CancellationToken ct)
    {
        var result = await _service.DetachTagAsync(questionId, tagId, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
