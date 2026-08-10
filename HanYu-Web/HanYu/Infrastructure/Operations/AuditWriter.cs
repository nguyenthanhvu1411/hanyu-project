using HanYu.Application.Interfaces.Operations;
using HanYu.Domain.Entities.Operations;
using HanYu.Infrastructure.Persistence;

namespace HanYu.Infrastructure.Operations;

public sealed class AuditWriter
    : IAuditWriter
{
    private readonly HanYuDbContext _db;

    public AuditWriter(
        HanYuDbContext db)
    {
        _db = db;
    }

    public async Task WriteAsync(
        Guid? userId,
        string action,
        string entityType,
        string? entityId = null,
        string? entityPublicId = null,
        string? oldValuesJson = null,
        string? newValuesJson = null,
        string? changedPropertiesJson = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        _db.Add(
            new AuditLog(
                userId,
                action,
                entityType,
                entityId,
                entityPublicId,
                oldValuesJson,
                newValuesJson,
                changedPropertiesJson,
                ipAddress,
                userAgent,
                correlationId));

        await _db.SaveChangesAsync(
            cancellationToken);
    }
}
