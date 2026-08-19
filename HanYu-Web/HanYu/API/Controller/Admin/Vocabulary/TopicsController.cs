using HanYu.API.Common;
using HanYu.API.Common.Extensions;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Vocabulary.Admin.Topics;
using HanYu.Application.Interfaces.Persistence;
using HanYu.Application.Interfaces.Vocabulary;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Vocabulary;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/vocabulary-topics")]
public sealed class TopicsController
    : ControllerBase
{
    private readonly IVocabularyAdminService _service;
    private readonly IHanYuDbContext _dbContext;

    public TopicsController(
        IVocabularyAdminService service,
        IHanYuDbContext dbContext)
    {
        _service = service;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetTopicsAsync(
                cancellationToken));

    [HttpGet("slug-availability")]
    public async Task<IActionResult> GetSlugAvailability(
        [FromQuery] string slug,
        [FromQuery] long? excludeId,
        CancellationToken cancellationToken)
    {
        var normalized = SlugAvailabilityQueries.Normalize(slug);
        var available = await SlugAvailabilityQueries.IsTopicSlugAvailableAsync(
            _dbContext,
            normalized,
            excludeId,
            cancellationToken);

        return Ok(new
        {
            Slug = normalized,
            Available = available
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTopicRequest request,
        CancellationToken cancellationToken)
    {
        var slug = SlugAvailabilityQueries.Normalize(request.Slug);
        if (slug.Length > 0 &&
            !await SlugAvailabilityQueries.IsTopicSlugAvailableAsync(
                _dbContext,
                slug,
                cancellationToken: cancellationToken))
        {
            return this.ToActionResult(
                Result.Failure<AdminTopicResponse>(
                    Error.Conflict(
                        "Topic.SlugAlreadyExists",
                        "Slug chủ đề đã tồn tại.")));
        }

        return this.ToActionResult(
            await _service.CreateTopicAsync(
                request,
                cancellationToken));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        long id,
        UpdateTopicRequest request,
        CancellationToken cancellationToken)
    {
        var slug = SlugAvailabilityQueries.Normalize(request.Slug);
        if (slug.Length > 0 &&
            !await SlugAvailabilityQueries.IsTopicSlugAvailableAsync(
                _dbContext,
                slug,
                id,
                cancellationToken))
        {
            return this.ToActionResult(
                Result.Failure<AdminTopicResponse>(
                    Error.Conflict(
                        "Topic.SlugAlreadyExists",
                        "Slug chủ đề đã được chủ đề khác sử dụng.")));
        }

        return this.ToActionResult(
            await _service.UpdateTopicAsync(
                id,
                request,
                cancellationToken));
    }

    [HttpPost("{id:long}/publish")]
    public async Task<IActionResult> Publish(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.PublishTopicAsync(
                id,
                cancellationToken));

    [HttpPost("{id:long}/archive")]
    public async Task<IActionResult> Archive(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.ArchiveTopicAsync(
                id,
                cancellationToken));

    [HttpPost("{id:long}/restore")]
    public async Task<IActionResult> Restore(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.RestoreTopicAsync(
                id,
                cancellationToken));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.DeleteTopicAsync(
                id,
                cancellationToken));
}
