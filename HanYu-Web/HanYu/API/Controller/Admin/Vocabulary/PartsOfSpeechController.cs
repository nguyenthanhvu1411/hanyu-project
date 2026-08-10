using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Vocabulary.Admin.PartsOfSpeech;
using HanYu.Application.Interfaces.Vocabulary;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Vocabulary;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/parts-of-speech")]
public sealed class PartsOfSpeechController
    : ControllerBase
{
    private readonly IVocabularyAdminService _service;

    public PartsOfSpeechController(
        IVocabularyAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetPartsOfSpeechAsync(
                cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(
        CreatePartOfSpeechRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.CreatePartOfSpeechAsync(
                request,
                cancellationToken));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        long id,
        UpdatePartOfSpeechRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.UpdatePartOfSpeechAsync(
                id,
                request,
                cancellationToken));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.DeletePartOfSpeechAsync(
                id,
                cancellationToken));
}
