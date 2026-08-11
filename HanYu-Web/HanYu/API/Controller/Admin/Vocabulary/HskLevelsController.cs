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
public sealed class HskLevelsController : ControllerBase
{
    private const string GenerationCacheKey = "vocabulary:public:generation";

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
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => this.ToActionResult(await _service.GetHskLevelsAsync(cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(long id, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return BadRequest(new { code = "HskLevel.InvalidId", message = "HSK level ID không hợp lệ." });

        // The normal detail endpoint intentionally hides soft-deleted rows.
        var entity = await _db.Set<HskLevel>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null
            ? NotFound(new { code = "HskLevel.NotFound", message = "Không tìm thấy cấp độ HSK." })
            : Ok(VocabularyAdminMapper.ToHskResponse(entity));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateHskLevelRequest request, CancellationToken cancellationToken)
        => this.ToActionResult(await _service.CreateHskLevelAsync(request, cancellationToken));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateHskLevelRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var entity = await _db.Set<HskLevel>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return NotFound(new { code = "HskLevel.NotFound", message = "Không tìm thấy cấp độ HSK." });

        var normalizedCode = request.Code.Trim().ToUpperInvariant();
        var duplicateCode = await _db.Set<HskLevel>()
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Id != id && x.Code == normalizedCode, cancellationToken);

        if (duplicateCode)
            return Conflict(new { code = "HskLevel.Duplicate", message = "HSK level đã tồn tại." });

        entity.Update(request.Code, request.NameVi, request.SortOrder, userId.Value);
        await _db.SaveChangesAsync(cancellationToken);
        await BumpCacheGenerationAsync(cancellationToken);

        return Ok(VocabularyAdminMapper.ToHskResponse(entity));
    }

    [HttpPost("{id:long}/activate")]
    public Task<IActionResult> Activate(long id, CancellationToken cancellationToken)
        => ChangeStateAsync(id, activate: true, cancellationToken);

    [HttpPost("{id:long}/deactivate")]
    public Task<IActionResult> Deactivate(long id, CancellationToken cancellationToken)
        => ChangeStateAsync(id, activate: false, cancellationToken);

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var entity = await _db.Set<HskLevel>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
            return NotFound(new { code = "HskLevel.NotFound", message = "Không tìm thấy cấp độ HSK." });

        // Keep HSK reference data consistent: do not hide a level that is still used by learning content.
        var inUseByVocabulary = await _db.Set<Domain.Entities.Vocabulary.Vocabulary>()
            .AnyAsync(x => x.HskLevelId == id, cancellationToken);
        var inUseByCourse = await _db.Set<Domain.Entities.Course.Course>()
            .AnyAsync(x => x.HskLevelId == id, cancellationToken);
        var inUseByLesson = await _db.Set<Domain.Entities.Lesson.Lesson>()
            .AnyAsync(x => x.HskLevelId == id, cancellationToken);

        if (inUseByVocabulary || inUseByCourse || inUseByLesson)
        {
            return Conflict(new
            {
                code = "HskLevel.InUse",
                message = "Cấp độ HSK đang được khóa học, bài giảng hoặc từ vựng sử dụng."
            });
        }

        entity.Delete(userId.Value);
        await _db.SaveChangesAsync(cancellationToken);
        await BumpCacheGenerationAsync(cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:long}/restore")]
    public async Task<IActionResult> Restore(long id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        // Restore must bypass the global soft-delete query filter.
        var entity = await _db.Set<HskLevel>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
            return NotFound(new { code = "HskLevel.NotFound", message = "Không tìm thấy cấp độ HSK." });

        entity.RestoreDeleted(userId.Value);
        await _db.SaveChangesAsync(cancellationToken);
        await BumpCacheGenerationAsync(cancellationToken);

        return Ok(VocabularyAdminMapper.ToHskResponse(entity));
    }

    private async Task<IActionResult> ChangeStateAsync(long id, bool activate, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var entity = await _db.Set<HskLevel>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return NotFound(new { code = "HskLevel.NotFound", message = "Không tìm thấy cấp độ HSK." });

        if (activate)
            entity.Activate(userId.Value);
        else
            entity.Deactivate(userId.Value);

        await _db.SaveChangesAsync(cancellationToken);
        await BumpCacheGenerationAsync(cancellationToken);

        return Ok(VocabularyAdminMapper.ToHskResponse(entity));
    }

    private Guid? GetCurrentUserId()
    {
        var userId = _currentUser.UserId;
        return userId.HasValue && userId.Value != Guid.Empty ? userId.Value : null;
    }

    private Task BumpCacheGenerationAsync(CancellationToken cancellationToken)
        => _cache.SetAsync(
            GenerationCacheKey,
            Guid.NewGuid().ToString("N"),
            TimeSpan.FromDays(_cacheOptions.GenerationDays),
            cancellationToken);
}
