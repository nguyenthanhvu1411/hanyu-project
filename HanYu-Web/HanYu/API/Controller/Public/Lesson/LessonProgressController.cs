using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Lesson.Public.Progress;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Lesson;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.Lesson;

[ApiController]
[Authorize]
[Route("api/v1/public/lessons")]
public sealed class LessonProgressController
    : ControllerBase
{
    private readonly ILessonPublicService _service;
    private readonly ICurrentUserService _currentUser;

    public LessonProgressController(
        ILessonPublicService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpPost("{publicId:guid}/start")]
    public async Task<IActionResult> Start(
        Guid publicId,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.StartLessonAsync(
                _currentUser.UserId.Value,
                publicId,
                cancellationToken));
    }

    [HttpPut("{publicId:guid}/progress")]
    public async Task<IActionResult> SaveProgress(
        Guid publicId,
        SaveLessonProgressRequest request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.SaveProgressAsync(
                _currentUser.UserId.Value,
                publicId,
                request,
                cancellationToken));
    }

    [HttpPut(
        "{publicId:guid}/sections/{sectionPublicId:guid}/progress")]
    public async Task<IActionResult> SaveSectionProgress(
        Guid publicId,
        Guid sectionPublicId,
        SaveSectionProgressRequest request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.SaveSectionProgressAsync(
                _currentUser.UserId.Value,
                publicId,
                sectionPublicId,
                request,
                cancellationToken));
    }

    [HttpPost("{publicId:guid}/complete")]
    public async Task<IActionResult> Complete(
        Guid publicId,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.CompleteLessonAsync(
                _currentUser.UserId.Value,
                publicId,
                cancellationToken));
    }
}
