using HanYu.Application.Common.Models;
using HanYu.Application.Features.Content.Public.Reports;
using HanYu.Application.Interfaces.Content;
using HanYu.Domain.Entities.Content;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Content;

public sealed class ContentPublicService
    : IContentPublicService
{
    private readonly HanYuDbContext _db;

    public ContentPublicService(
        HanYuDbContext db)
    {
        _db = db;
    }

    public async Task<Result<MyContentReportResponse>>
        CreateReportAsync(
            Guid userId,
            CreateContentReportRequest request,
            CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            return Result.Failure<MyContentReportResponse>(
                Error.Unauthorized(
                    "Identity.Unauthorized",
                    "Người dùng chưa đăng nhập."));
        }

        /*
         * Có thể bổ sung ValidateContentEntityExistsAsync()
         * theo EntityType nếu muốn validate sâu.
         */

        var entity =
            new ContentReport(
                userId,
                request.EntityType,
                request.EntityId,
                request.Reason,
                request.Description);

        _db.Add(entity);

        await _db.SaveChangesAsync(
            cancellationToken);

        return Result.Success(Map(entity));
    }

    public async Task<Result<
        IReadOnlyCollection<MyContentReportResponse>>>
        GetMyReportsAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var entities =
            await _db.Set<ContentReport>()
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToArrayAsync(cancellationToken);

        return Result.Success<
            IReadOnlyCollection<MyContentReportResponse>>(
            entities.Select(Map).ToArray());
    }

    public async Task<Result> UpdateMyReportAsync(
        Guid userId,
        Guid publicId,
        string? description,
        CancellationToken cancellationToken = default)
    {
        var entity =
            await _db.Set<ContentReport>()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.PublicId == publicId,
                    cancellationToken);

        if (entity is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "ContentReport.NotFound",
                    "Không tìm thấy report."));
        }

        entity.UpdateDescription(description);

        await _db.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    private static MyContentReportResponse Map(
        ContentReport entity)
        => new(
            entity.PublicId,
            entity.EntityType,
            entity.Reason,
            entity.Description,
            entity.Status,
            entity.ResolutionNote,
            entity.ResolvedAt,
            entity.CreatedAt,
            entity.UpdatedAt);
}
