using HanYu.Application.Features.Quiz.Admin.QuestionBanks;
using HanYu.Application.Interfaces.Quiz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controllers.Admin.Quiz;

[ApiController]
[Route("api/admin/question-banks")]
[Authorize(Roles = "Admin")]
public class QuizQuestionBanksController : ControllerBase
{
    private readonly IQuizAdminService _service;

    public QuizQuestionBanksController(IQuizAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetQuestionBanks(CancellationToken ct)
    {
        var result = await _service.GetQuestionBanksAsync(ct);
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuestionBank(
        [FromBody] CreateQuestionBankRequest request,
        CancellationToken ct)
    {
        var result = await _service.CreateQuestionBankAsync(request, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuestionBank(
        long id,
        [FromBody] UpdateQuestionBankRequest request,
        CancellationToken ct)
    {
        var result = await _service.UpdateQuestionBankAsync(id, request, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{id}/questions")]
    public async Task<IActionResult> AddQuestionToBank(
        long id,
        [FromBody] AddQuestionToBankRequest request,
        CancellationToken ct)
    {
        var result = await _service.AddQuestionToBankAsync(id, request, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id}/questions/{questionId}")]
    public async Task<IActionResult> RemoveQuestionFromBank(
        long id,
        long questionId,
        CancellationToken ct)
    {
        var result = await _service.RemoveQuestionFromBankAsync(id, questionId, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id}/activate")]
    public async Task<IActionResult> ActivateQuestionBank(
        long id,
        CancellationToken ct)
    {
        var result = await _service.ActivateQuestionBankAsync(id, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivateQuestionBank(
        long id,
        CancellationToken ct)
    {
        var result = await _service.DeactivateQuestionBankAsync(id, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
