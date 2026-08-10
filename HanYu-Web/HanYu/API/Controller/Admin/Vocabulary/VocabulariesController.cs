using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Vocabulary.Admin.Vocabulary;
using HanYu.Application.Interfaces.Vocabulary;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HanYu.API.Controller.Admin.Vocabulary;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/vocabularies")]
public sealed class VocabulariesController
    : ControllerBase
{
    private readonly IVocabularyAdminService _service;

    public VocabulariesController(
        IVocabularyAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] AdminVocabularyQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetVocabulariesAsync(
                query,
                cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetVocabularyAsync(
                id,
                cancellationToken));

    [HttpPost]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Create(
        CreateVocabularyRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.CreateVocabularyAsync(
                request,
                cancellationToken));

    [HttpPut("{id:long}")]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Update(
        long id,
        UpdateVocabularyRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.UpdateVocabularyAsync(
                id,
                request,
                cancellationToken));

    [HttpPost("{id:long}/submit-review")]
    public async Task<IActionResult> SubmitReview(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service
                .SubmitVocabularyForReviewAsync(
                    id,
                    cancellationToken));

    [HttpPost("{id:long}/approve")]
    public async Task<IActionResult> Approve(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service
                .ApproveVocabularyAsync(
                    id,
                    cancellationToken));

    [HttpPost("{id:long}/publish")]
    public async Task<IActionResult> Publish(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service
                .PublishVocabularyAsync(
                    id,
                    cancellationToken));

    [HttpPost("{id:long}/archive")]
    public async Task<IActionResult> Archive(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service
                .ArchiveVocabularyAsync(
                    id,
                    cancellationToken));

    [HttpPost("{id:long}/restore")]
    public async Task<IActionResult> Restore(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service
                .RestoreVocabularyAsync(
                    id,
                    cancellationToken));

    [HttpDelete("{id:long}")]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Delete(
        long id,
        CancellationToken cancellationToken)
    {
        return this.ToActionResult(
            await _service.DeleteVocabularyAsync(
                id,
                cancellationToken));
    }
}
