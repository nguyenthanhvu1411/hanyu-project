using HanYu.Application.Common.Models;
using HanYu.Application.Features.Operations.Admin.AuditLogs;
using HanYu.Application.Features.Operations.Admin.ProductEvents;
using HanYu.Application.Interfaces.Operations;
using HanYu.Domain.Entities.Operations;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Operations;

public sealed class OperationsAdminService : IOperationsAdminService
{
    private readonly HanYuDbContext _db;

    public OperationsAdminService(HanYuDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PagedResult<AdminAuditLogResponse>>>
        GetAuditLogsAsync(
            AdminAuditLogQuery query,
            CancellationToken cancellationToken = default)
    {
        var q = _db.Set<AuditLog>().AsNoTracking();

        if (query.UserId.HasValue)
            q = q.Where(x => x.UserId == query.UserId.Value);

        if (!string.IsNullOrWhiteSpace(query.Action))
            q = q.Where(x => x.Action == query.Action);

        if (!string.IsNullOrWhiteSpace(query.EntityType))
            q = q.Where(x => x.EntityType == query.EntityType);

        if (!string.IsNullOrWhiteSpace(query.EntityId))
            q = q.Where(x => x.EntityId == query.EntityId);

        if (!string.IsNullOrWhiteSpace(query.CorrelationId))
            q = q.Where(x => x.CorrelationId == query.CorrelationId);

        if (query.From.HasValue)
            q = q.Where(x => x.OccurredAt >= query.From.Value);

        if (query.To.HasValue)
            q = q.Where(x => x.OccurredAt <= query.To.Value);

        q = query.Sort switch
        {
            "occurredAt" => q.OrderBy(x => x.OccurredAt),
            "-occurredAt" => q.OrderByDescending(x => x.OccurredAt),
            _ => q.OrderByDescending(x => x.OccurredAt)
        };

        var total = await q.CountAsync(cancellationToken);

        var entities = await q
            .Skip((query.NormalizedPage - 1) * query.NormalizedPageSize)
            .Take(query.NormalizedPageSize)
            .ToArrayAsync(cancellationToken);

        var responses = entities.Select(x => new AdminAuditLogResponse(
            x.Id,
            x.PublicId,
            x.UserId,
            x.Action,
            x.EntityType,
            x.EntityId,
            x.EntityPublicId,
            x.OldValuesJson,
            x.NewValuesJson,
            x.ChangedPropertiesJson,
            x.IpAddress,
            x.UserAgent,
            x.CorrelationId,
            x.OccurredAt)).ToArray();

        return Result.Success(new PagedResult<AdminAuditLogResponse>(
            responses,
            query.NormalizedPage,
            query.NormalizedPageSize,
            total));
    }

    public async Task<Result<AdminAuditLogResponse>>
        GetAuditLogAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<AuditLog>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
        {
            return Result.Failure<AdminAuditLogResponse>(
                Error.NotFound("Operations.AuditLogNotFound", "Không tìm thấy audit log."));
        }

        return Result.Success(new AdminAuditLogResponse(
            entity.Id,
            entity.PublicId,
            entity.UserId,
            entity.Action,
            entity.EntityType,
            entity.EntityId,
            entity.EntityPublicId,
            entity.OldValuesJson,
            entity.NewValuesJson,
            entity.ChangedPropertiesJson,
            entity.IpAddress,
            entity.UserAgent,
            entity.CorrelationId,
            entity.OccurredAt));
    }

    public async Task<Result<PagedResult<AdminProductEventResponse>>>
        GetProductEventsAsync(
            AdminProductEventQuery query,
            CancellationToken cancellationToken = default)
    {
        var q = _db.Set<ProductEvent>().AsNoTracking();

        if (query.UserId.HasValue)
            q = q.Where(x => x.UserId == query.UserId.Value);

        if (query.SessionId.HasValue)
            q = q.Where(x => x.SessionId == query.SessionId.Value);

        if (!string.IsNullOrWhiteSpace(query.EventName))
            q = q.Where(x => x.EventName == query.EventName);

        if (!string.IsNullOrWhiteSpace(query.EntityType))
            q = q.Where(x => x.EntityType == query.EntityType);

        if (!string.IsNullOrWhiteSpace(query.DeviceType))
            q = q.Where(x => x.DeviceType == query.DeviceType);

        if (query.From.HasValue)
            q = q.Where(x => x.OccurredAt >= query.From.Value);

        if (query.To.HasValue)
            q = q.Where(x => x.OccurredAt <= query.To.Value);

        q = query.Sort switch
        {
            "occurredAt" => q.OrderBy(x => x.OccurredAt),
            "-occurredAt" => q.OrderByDescending(x => x.OccurredAt),
            _ => q.OrderByDescending(x => x.OccurredAt)
        };

        var total = await q.CountAsync(cancellationToken);

        var entities = await q
            .Skip((query.NormalizedPage - 1) * query.NormalizedPageSize)
            .Take(query.NormalizedPageSize)
            .ToArrayAsync(cancellationToken);

        var responses = entities.Select(x => new AdminProductEventResponse(
            x.Id,
            x.PublicId,
            x.UserId,
            x.SessionId,
            x.EventName,
            x.EntityType,
            x.EntityPublicId,
            x.PropertiesJson,
            x.PagePath,
            x.Referrer,
            x.DeviceType,
            x.OccurredAt)).ToArray();

        return Result.Success(new PagedResult<AdminProductEventResponse>(
            responses,
            query.NormalizedPage,
            query.NormalizedPageSize,
            total));
    }
}
