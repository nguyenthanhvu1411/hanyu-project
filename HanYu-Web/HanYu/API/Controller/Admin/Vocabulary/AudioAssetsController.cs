using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Vocabulary.Admin.AudioAssets;
using HanYu.Application.Interfaces.Vocabulary;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Vocabulary;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/audio-assets")]
public sealed class AudioAssetsController
    : ControllerBase
{
    private readonly IVocabularyAdminService _service;

    public AudioAssetsController(
        IVocabularyAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
        => this.ToActionResult(
            await _service.GetAudioAssetsAsync(
                page,
                pageSize,
                cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateAudioAssetRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.CreateAudioAssetAsync(
                request,
                cancellationToken));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        long id,
        UpdateAudioAssetRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.UpdateAudioAssetAsync(
                id,
                request,
                cancellationToken));

    [HttpPost("{id:long}/publish")]
    public async Task<IActionResult> Publish(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.PublishAudioAssetAsync(
                id,
                cancellationToken));

    [HttpPost("{id:long}/archive")]
    public async Task<IActionResult> Archive(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.ArchiveAudioAssetAsync(
                id,
                cancellationToken));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.DeleteAudioAssetAsync(
                id,
                cancellationToken));
}
