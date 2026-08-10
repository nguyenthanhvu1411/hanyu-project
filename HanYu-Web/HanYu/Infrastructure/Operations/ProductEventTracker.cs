using HanYu.Application.Features.Operations.Public.Events;
using HanYu.Application.Interfaces.Operations;
using HanYu.Domain.Entities.Operations;
using HanYu.Infrastructure.Persistence;

namespace HanYu.Infrastructure.Operations;

public sealed class ProductEventTracker
    : IProductEventTracker
{
    private readonly HanYuDbContext _db;

    public ProductEventTracker(
        HanYuDbContext db)
    {
        _db = db;
    }

    public async Task TrackAsync(
        Guid? userId,
        TrackProductEventRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity =
            new ProductEvent(
                request.EventName,
                userId,
                request.SessionId,
                request.EntityType,
                request.EntityPublicId,
                request.PropertiesJson);

        entity.AttachPageContext(
            request.PagePath,
            request.Referrer,
            request.DeviceType);

        _db.Add(entity);

        await _db.SaveChangesAsync(
            cancellationToken);
    }
}
