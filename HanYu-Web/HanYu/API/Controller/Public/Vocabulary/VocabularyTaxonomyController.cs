using HanYu.API.Common.Extensions;
using HanYu.Application.Interfaces.Vocabulary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.Vocabulary;

[ApiController]
[AllowAnonymous]
[Route("api/v1/public/vocabulary-taxonomy")]
public sealed class VocabularyTaxonomyController
    : ControllerBase
{
    private readonly IVocabularyPublicService _service;

    public VocabularyTaxonomyController(
        IVocabularyPublicService service)
    {
        _service = service;
    }

    [HttpGet("topics")]
    public async Task<IActionResult> Topics(
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetTopicsAsync(
                cancellationToken));

    [HttpGet("parts-of-speech")]
    public async Task<IActionResult> PartsOfSpeech(
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetPartsOfSpeechAsync(
                cancellationToken));

    [HttpGet("hsk-levels")]
    public async Task<IActionResult> HskLevels(
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetHskLevelsAsync(
                cancellationToken));
}
