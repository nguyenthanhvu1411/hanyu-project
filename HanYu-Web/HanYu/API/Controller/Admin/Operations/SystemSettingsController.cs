using HanYu.Application.Features.Operations.Admin.SystemSettings;
using HanYu.Domain.Entities.Operations;
using HanYu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HanYu.API.Controller.Admin.Operations;

[ApiController]
[Route("api/v1/admin/system-settings")]
[Authorize(Roles = "Admin")]
public sealed class SystemSettingsController : ControllerBase
{
    private readonly HanYuDbContext _db;

    public SystemSettingsController(HanYuDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<AdminSystemSettingResponse>>> GetSettings(
        [FromQuery] string? group,
        CancellationToken cancellationToken)
    {
        var query = _db.Set<SystemSetting>().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(group))
            query = query.Where(x => x.Group == group.Trim());

        var items = await query
            .OrderBy(x => x.Group)
            .ThenBy(x => x.DisplayName)
            .Select(x => new AdminSystemSettingResponse(
                x.Id,
                x.Key,
                x.DisplayName,
                x.Group,
                x.Value,
                x.ValueType,
                x.Description,
                x.CreatedAt,
                x.UpdatedAt))
            .ToArrayAsync(cancellationToken);

        return Ok(items);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<AdminSystemSettingResponse>> UpdateSetting(
        long id,
        [FromBody] UpsertSystemSettingRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await _db.Set<SystemSetting>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return NotFound(new { code = "SystemSetting.NotFound", message = "Không tìm thấy cấu hình hệ thống." });

        try
        {
            var duplicate = await _db.Set<SystemSetting>()
                .AnyAsync(x => x.Id != id && x.Key == request.Key.Trim().ToLower(), cancellationToken);
            if (duplicate)
                return Conflict(new { code = "SystemSetting.DuplicateKey", message = "Key cấu hình đã tồn tại." });

            entity.UpdateMetadata(request.Key, request.DisplayName, request.Group, request.ValueType, request.Description);
            entity.UpdateValue(request.Value);
            await _db.SaveChangesAsync(cancellationToken);
            return Ok(Map(entity));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { code = "SystemSetting.Validation", message = exception.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<AdminSystemSettingResponse>> CreateSetting(
        [FromBody] UpsertSystemSettingRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var normalizedKey = request.Key.Trim().ToLowerInvariant();
            if (await _db.Set<SystemSetting>().AnyAsync(x => x.Key == normalizedKey, cancellationToken))
                return Conflict(new { code = "SystemSetting.DuplicateKey", message = "Key cấu hình đã tồn tại." });

            var entity = new SystemSetting(request.Key, request.DisplayName, request.Group, request.Value, request.ValueType, request.Description);
            _db.Add(entity);
            await _db.SaveChangesAsync(cancellationToken);
            return CreatedAtAction(nameof(GetSettings), new { }, Map(entity));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { code = "SystemSetting.Validation", message = exception.Message });
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteSetting(long id, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<SystemSetting>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return NotFound(new { code = "SystemSetting.NotFound", message = "Không tìm thấy cấu hình hệ thống." });

        _db.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static AdminSystemSettingResponse Map(SystemSetting x)
        => new(x.Id, x.Key, x.DisplayName, x.Group, x.Value, x.ValueType, x.Description, x.CreatedAt, x.UpdatedAt);
}
