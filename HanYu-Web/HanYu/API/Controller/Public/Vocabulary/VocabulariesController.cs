using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Vocabulary.Public.Vocabulary;
using HanYu.Application.Interfaces.Vocabulary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.Vocabulary;

[ApiController]
[AllowAnonymous]
[Route("api/v1/public/vocabularies")]
public sealed class VocabulariesController
    : ControllerBase
{
    private readonly IVocabularyPublicService _service;

    public VocabulariesController(
        IVocabularyPublicService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] VocabularyQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetVocabulariesAsync(
                query,
                cancellationToken));

    [HttpGet("{simplified}")]
    public async Task<IActionResult> Get(
        string simplified,
        [FromQuery] string? pinyinNormalized,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetVocabularyAsync(
                simplified,
                pinyinNormalized,
                cancellationToken));
}
