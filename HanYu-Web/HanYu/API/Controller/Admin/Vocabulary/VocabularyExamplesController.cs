using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Vocabulary.Admin.Examples;
using HanYu.Application.Interfaces.Vocabulary;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Vocabulary;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route(
    "api/v1/admin/vocabularies/{vocabularyId:long}/examples")]
public sealed class VocabularyExamplesController
    : ControllerBase
{
    private readonly IVocabularyAdminService _service;

    public VocabularyExamplesController(
        IVocabularyAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        long vocabularyId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetExamplesAsync(
                vocabularyId,
                cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(
        long vocabularyId,
        CreateVocabularyExampleRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.CreateExampleAsync(
                vocabularyId,
                request,
                cancellationToken));

    [HttpPut("{exampleId:long}")]
    public async Task<IActionResult> Update(
        long vocabularyId,
        long exampleId,
        UpdateVocabularyExampleRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.UpdateExampleAsync(
                vocabularyId,
                exampleId,
                request,
                cancellationToken));

    [HttpPost("{exampleId:long}/submit-review")]
    public async Task<IActionResult> SubmitReview(
        long vocabularyId,
        long exampleId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service
                .SubmitExampleForReviewAsync(
                    vocabularyId,
                    exampleId,
                    cancellationToken));

    [HttpPost("{exampleId:long}/approve")]
    public async Task<IActionResult> Approve(
        long vocabularyId,
        long exampleId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.ApproveExampleAsync(
                vocabularyId,
                exampleId,
                cancellationToken));

    [HttpPost("{exampleId:long}/publish")]
    public async Task<IActionResult> Publish(
        long vocabularyId,
        long exampleId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.PublishExampleAsync(
                vocabularyId,
                exampleId,
                cancellationToken));

    [HttpPost("{exampleId:long}/archive")]
    public async Task<IActionResult> Archive(
        long vocabularyId,
        long exampleId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.ArchiveExampleAsync(
                vocabularyId,
                exampleId,
                cancellationToken));

    [HttpPost("{exampleId:long}/restore")]
    public async Task<IActionResult> Restore(
        long vocabularyId,
        long exampleId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.RestoreExampleAsync(
                vocabularyId,
                exampleId,
                cancellationToken));

    [HttpDelete("{exampleId:long}")]
    public async Task<IActionResult> Delete(
        long vocabularyId,
        long exampleId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.DeleteExampleAsync(
                vocabularyId,
                exampleId,
                cancellationToken));
}
