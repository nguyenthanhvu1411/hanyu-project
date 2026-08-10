using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Vocabulary.Admin.HskLevels;
using HanYu.Application.Features.Vocabulary.Mapping;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Caching;
using HanYu.Application.Interfaces.Vocabulary;
using HanYu.Domain.Constants;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Infrastructure.Options;
using HanYu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HanYu.API.Controller.Admin.Vocabulary;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/hsk-levels")]
public sealed class HskLevelsController
    : ControllerBase
{
    private const string GenerationCacheKey =
        "vocabulary:public:generation";

    private readonly IVocabularyAdminService _service;
    private readonly HanYuDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cache;
    private readonly VocabularyCacheOptions _cacheOptions;

    public HskLevelsController(
        IVocabularyAdminService service,
        HanYuDbContext db,
        ICurrentUserService currentUser,
        ICacheService cache,
        IOptions<VocabularyCacheOptions> cacheOptions)
    {
        _service = service;
        _db = db;
        _currentUser = currentUser;
        _cache = cache;
        _cacheOptions = cacheOptions.Value;
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
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var entity =
            await _db.Set<HskLevel>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
            return NotFound();

        var normalizedCode =
            request.Code.Trim().ToUpperInvariant();

        var duplicateCode =
            await _db.Set<HskLevel>()
                .AnyAsync(
                    x =>
                        x.Id != id &&
                        x.Code == normalizedCode,
                    cancellationToken);

        if (duplicateCode)
        {
            return Conflict(new
            {
                code = "HskLevel.Duplicate",
                message = "HSK level đã tồn tại."
            });
        }

        entity.Update(
            request.Code,
            request.NameVi,
            request.SortOrder,
            userId.Value);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Ok(
            VocabularyAdminMapper
                .ToHskResponse(entity));
    }

    [HttpPost("{id:long}/activate")]
    public async Task<IActionResult> Activate(
        long id,
        CancellationToken cancellationToken)
        => await ChangeStateAsync(
            id,
            activate: true,
            cancellationToken);

    [HttpPost("{id:long}/deactivate")]
    public async Task<IActionResult> Deactivate(
        long id,
        CancellationToken cancellationToken)
        => await ChangeStateAsync(
            id,
            activate: false,
            cancellationToken);

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.DeleteHskLevelAsync(
                id,
                cancellationToken));

    private async Task<IActionResult> ChangeStateAsync(
        long id,
        bool activate,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var entity =
            await _db.Set<HskLevel>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
            return NotFound();

        if (activate)
            entity.Activate(userId.Value);
        else
            entity.Deactivate(userId.Value);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Ok(
            VocabularyAdminMapper
                .ToHskResponse(entity));
    }

    private Guid? GetCurrentUserId()
    {
        var userId = _currentUser.UserId;

        return userId.HasValue &&
               userId.Value != Guid.Empty
            ? userId.Value
            : null;
    }

    private Task BumpCacheGenerationAsync(
        CancellationToken cancellationToken)
        => _cache.SetAsync(
            GenerationCacheKey,
            Guid.NewGuid().ToString("N"),
            TimeSpan.FromDays(
                _cacheOptions.GenerationDays),
            cancellationToken);
}
