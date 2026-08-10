using HanYu.Application.Features.Operations.Public.Events;

namespace HanYu.Application.Interfaces.Operations;

public interface IProductEventTracker
{
    Task TrackAsync(
        Guid? userId,
        TrackProductEventRequest request,
        CancellationToken cancellationToken = default);
}

public interface IAuditWriter
{
    Task WriteAsync(
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
        CancellationToken cancellationToken = default);
}
