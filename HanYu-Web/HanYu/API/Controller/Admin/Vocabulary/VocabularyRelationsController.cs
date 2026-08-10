using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Vocabulary.Admin.Relations;
using HanYu.Application.Interfaces.Vocabulary;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Vocabulary;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route(
    "api/v1/admin/vocabularies/{vocabularyId:long}/relations")]
public sealed class VocabularyRelationsController
    : ControllerBase
{
    private readonly IVocabularyAdminService _service;

    public VocabularyRelationsController(
        IVocabularyAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        long vocabularyId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetRelationsAsync(
                vocabularyId,
                cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(
        long vocabularyId,
        CreateVocabularyRelationRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.CreateRelationAsync(
                vocabularyId,
                request,
                cancellationToken));

    [HttpPut("{relationId:long}")]
    public async Task<IActionResult> Update(
        long vocabularyId,
        long relationId,
        UpdateVocabularyRelationRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.UpdateRelationAsync(
                vocabularyId,
                relationId,
                request,
                cancellationToken));

    [HttpDelete("{relationId:long}")]
    public async Task<IActionResult> Delete(
        long vocabularyId,
        long relationId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.DeleteRelationAsync(
                vocabularyId,
                relationId,
                cancellationToken));
}
