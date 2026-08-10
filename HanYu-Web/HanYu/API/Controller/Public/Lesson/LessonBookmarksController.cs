using HanYu.API.Common.Extensions;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Lesson;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.Lesson;

[ApiController]
[Authorize]
[Route("api/v1/public/lesson-bookmarks")]
public sealed class LessonBookmarksController
    : ControllerBase
{
    private readonly ILessonPublicService _service;
    private readonly ICurrentUserService _currentUser;

    public LessonBookmarksController(
        ILessonPublicService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.GetBookmarksAsync(
                _currentUser.UserId.Value,
                cancellationToken));
    }

    [HttpPost("{lessonPublicId:guid}")]
    public async Task<IActionResult> Add(
        Guid lessonPublicId,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.BookmarkAsync(
                _currentUser.UserId.Value,
                lessonPublicId,
                cancellationToken));
    }

    [HttpDelete("{lessonPublicId:guid}")]
    public async Task<IActionResult> Delete(
        Guid lessonPublicId,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.RemoveBookmarkAsync(
                _currentUser.UserId.Value,
                lessonPublicId,
                cancellationToken));
    }
}
