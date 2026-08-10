using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Vocabulary.Admin.HskLevels;
using HanYu.Application.Interfaces.Vocabulary;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Vocabulary;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/hsk-levels")]
public sealed class HskLevelsController
    : ControllerBase
{
    private readonly IVocabularyAdminService _service;

    public HskLevelsController(
        IVocabularyAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetHskLevelsAsync(
                cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateHskLevelRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.CreateHskLevelAsync(
                request,
                cancellationToken));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        long id,
        UpdateHskLevelRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.UpdateHskLevelAsync(
                id,
                request,
                cancellationToken));

    [HttpPost("{id:long}/activate")]
    public async Task<IActionResult> Activate(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.ActivateHskLevelAsync(
                id,
                cancellationToken));

    [HttpPost("{id:long}/deactivate")]
    public async Task<IActionResult> Deactivate(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.DeactivateHskLevelAsync(
                id,
                cancellationToken));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.DeleteHskLevelAsync(
                id,
                cancellationToken));
}
