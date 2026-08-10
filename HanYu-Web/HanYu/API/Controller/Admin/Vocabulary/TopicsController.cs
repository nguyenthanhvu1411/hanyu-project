using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Vocabulary.Admin.Topics;
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

    public TopicsController(
        IVocabularyAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetTopicsAsync(
                cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTopicRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.CreateTopicAsync(
                request,
                cancellationToken));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        long id,
        UpdateTopicRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.UpdateTopicAsync(
                id,
                request,
                cancellationToken));

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
