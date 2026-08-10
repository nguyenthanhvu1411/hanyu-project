using HanYu.Application.Features.Quiz.Admin.Tags;
using HanYu.Application.Interfaces.Quiz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controllers.Admin.Quiz;

[ApiController]
[Route("api/admin/quiz-tags")]
[Authorize(Roles = "Admin")]
public class QuizTagsController : ControllerBase
{
    private readonly IQuizAdminService _service;

    public QuizTagsController(IQuizAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetTags(CancellationToken ct)
    {
        var result = await _service.GetTagsAsync(ct);
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTag(
        [FromBody] CreateQuizTagRequest request,
        CancellationToken ct)
    {
        var result = await _service.CreateTagAsync(request, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTag(
        long id,
        [FromBody] UpdateQuizTagRequest request,
        CancellationToken ct)
    {
        var result = await _service.UpdateTagAsync(id, request, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{id}/activate")]
    public async Task<IActionResult> ActivateTag(
        long id,
        CancellationToken ct)
    {
        var result = await _service.ActivateTagAsync(id, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivateTag(
        long id,
        CancellationToken ct)
    {
        var result = await _service.DeactivateTagAsync(id, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(
        long id,
        CancellationToken ct)
    {
        var result = await _service.DeleteTagAsync(id, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
