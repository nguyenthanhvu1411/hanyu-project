using HanYu.API.Common;
using HanYu.API.Common.Extensions;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Vocabulary.Admin.Examples;
using HanYu.Application.Interfaces.Vocabulary;
using HanYu.Domain.Constants;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HanYu.API.Controller.Admin.Vocabulary;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route(
    "api/v1/admin/vocabularies/{vocabularyId:long}/examples")]
public sealed class VocabularyExamplesController
    : ControllerBase
{
    private readonly IVocabularyAdminService _service;
    private readonly HanYuDbContext _db;

    public VocabularyExamplesController(
        IVocabularyAdminService service,
        HanYuDbContext db)
    {
        _service = service;
        _db = db;
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
    {
        var audioValidation = await VocabularyAudioGuard.ValidateAsync(
            _db,
            request.AudioAssetId,
            AudioAssetKind.ExampleSentence,
            cancellationToken);

        if (audioValidation.IsFailure)
        {
            return this.ToActionResult(
                Result.Failure<AdminVocabularyExampleResponse>(
                    audioValidation.Error));
        }

        return this.ToActionResult(
            await _service.CreateExampleAsync(
                vocabularyId,
                request,
                cancellationToken));
    }

    [HttpPut("{exampleId:long}")]
    public async Task<IActionResult> Update(
        long vocabularyId,
        long exampleId,
        UpdateVocabularyExampleRequest request,
        CancellationToken cancellationToken)
    {
        var audioValidation = await VocabularyAudioGuard.ValidateAsync(
            _db,
            request.AudioAssetId,
            AudioAssetKind.ExampleSentence,
            cancellationToken);

        if (audioValidation.IsFailure)
        {
            return this.ToActionResult(
                Result.Failure<AdminVocabularyExampleResponse>(
                    audioValidation.Error));
        }

        return this.ToActionResult(
            await _service.UpdateExampleAsync(
                vocabularyId,
                exampleId,
                request,
                cancellationToken));
    }

    [HttpPut("{exampleId:long}/audio")]
    public async Task<IActionResult> ChangeAudio(
        long vocabularyId,
        long exampleId,
        ChangeVocabularyExampleAudioRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await _db.Set<VocabularyExample>()
            .FirstOrDefaultAsync(
                item =>
                    item.Id == exampleId &&
                    item.VocabularyId == vocabularyId,
                cancellationToken);

        if (entity is null)
        {
            return this.ToActionResult(
                Result.Failure<AdminVocabularyExampleResponse>(
                    Error.NotFound(
                        "VocabularyExample.NotFound",
                        "Không tìm thấy VocabularyExample.")));
        }

        if (entity.Status == ContentStatus.Archived)
        {
            return this.ToActionResult(
                Result.Failure<AdminVocabularyExampleResponse>(
                    Error.Conflict(
                        "VocabularyExample.Archived",
                        "Không thể đổi audio của example Archived.")));
        }

        var audioValidation = await VocabularyAudioGuard.ValidateAsync(
            _db,
            request.AudioAssetId,
            AudioAssetKind.ExampleSentence,
            cancellationToken);

        if (audioValidation.IsFailure)
        {
            return this.ToActionResult(
                Result.Failure<AdminVocabularyExampleResponse>(
                    audioValidation.Error));
        }

        entity.ChangeAudio(request.AudioAssetId);
        await _db.SaveChangesAsync(cancellationToken);

        var items = await _service.GetExamplesAsync(
            vocabularyId,
            cancellationToken);

        if (items.IsFailure)
        {
            return this.ToActionResult(
                Result.Failure<AdminVocabularyExampleResponse>(
                    items.Error));
        }

        var updated = items.Value.FirstOrDefault(item => item.Id == exampleId);
        return updated is null
            ? this.ToActionResult(
                Result.Failure<AdminVocabularyExampleResponse>(
                    Error.NotFound(
                        "VocabularyExample.NotFound",
                        "Không tìm thấy VocabularyExample.")))
            : Ok(updated);
    }

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
