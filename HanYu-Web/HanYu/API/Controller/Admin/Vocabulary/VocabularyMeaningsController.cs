using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Vocabulary.Admin.Meanings;
using HanYu.Application.Interfaces.Vocabulary;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Vocabulary;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route(
    "api/v1/admin/vocabularies/{vocabularyId:long}/meanings")]
public sealed class VocabularyMeaningsController
    : ControllerBase
{
    private readonly IVocabularyAdminService _service;

    public VocabularyMeaningsController(
        IVocabularyAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        long vocabularyId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetMeaningsAsync(
                vocabularyId,
                cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(
        long vocabularyId,
        CreateVocabularyMeaningRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.CreateMeaningAsync(
                vocabularyId,
                request,
                cancellationToken));

    [HttpPut("{meaningId:long}")]
    public async Task<IActionResult> Update(
        long vocabularyId,
        long meaningId,
        UpdateVocabularyMeaningRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.UpdateMeaningAsync(
                vocabularyId,
                meaningId,
                request,
                cancellationToken));

    [HttpDelete("{meaningId:long}")]
    public async Task<IActionResult> Delete(
        long vocabularyId,
        long meaningId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.DeleteMeaningAsync(
                vocabularyId,
                meaningId,
                cancellationToken));
}
