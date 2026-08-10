using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Vocabulary.Public.Notes;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Vocabulary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.Vocabulary;

[ApiController]
[Authorize]
[Route(
    "api/v1/public/users/me/vocabulary-notes")]
public sealed class VocabularyNotesController
    : ControllerBase
{
    private readonly IVocabularyPublicService _service;
    private readonly ICurrentUserService _currentUser;

    public VocabularyNotesController(
        IVocabularyPublicService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet("{simplified}")]
    public async Task<IActionResult> Get(
        string simplified,
        [FromQuery] string? pinyinNormalized,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.GetMyNoteAsync(
                _currentUser.UserId.Value,
                simplified,
                pinyinNormalized,
                cancellationToken));
    }

    [HttpPut("{simplified}")]
    public async Task<IActionResult> Save(
        string simplified,
        [FromQuery] string? pinyinNormalized,
        SaveVocabularyNoteRequest request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.SaveMyNoteAsync(
                _currentUser.UserId.Value,
                simplified,
                pinyinNormalized,
                request,
                cancellationToken));
    }

    [HttpDelete("{simplified}")]
    public async Task<IActionResult> Delete(
        string simplified,
        [FromQuery] string? pinyinNormalized,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        return this.ToActionResult(
            await _service.DeleteMyNoteAsync(
                _currentUser.UserId.Value,
                simplified,
                pinyinNormalized,
                cancellationToken));
    }
}
