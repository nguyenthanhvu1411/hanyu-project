using HanYu.Application.Common.Models;
using HanYu.Application.Features.Content.Admin.Imports;
using HanYu.Application.Features.Content.Admin.Reports;
using HanYu.Application.Interfaces.Content;
using HanYu.Domain.Entities.Content;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Content;

public sealed class ContentAdminService
    : IContentAdminService
{
    private readonly HanYuDbContext _db;

    public ContentAdminService(
        HanYuDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PagedResult<AdminContentImportJobResponse>>>
        GetImportJobsAsync(
            AdminContentImportJobQuery query,
            CancellationToken cancellationToken = default)
    {
        var q = _db.Set<ContentImportJob>().AsNoTracking();

        if (query.ImportType.HasValue)
            q = q.Where(x => x.ImportType == query.ImportType.Value);

        if (query.Status.HasValue)
            q = q.Where(x => x.Status == query.Status.Value);

        if (query.From.HasValue)
            q = q.Where(x => x.CreatedAt >= query.From.Value);

        if (query.To.HasValue)
            q = q.Where(x => x.CreatedAt <= query.To.Value);

        q = query.Sort switch
        {
            "createdAt" => q.OrderBy(x => x.CreatedAt),
            "-createdAt" => q.OrderByDescending(x => x.CreatedAt),
            _ => q.OrderByDescending(x => x.CreatedAt)
        };

        var total = await q.CountAsync(cancellationToken);

        var entities = await q
            .Skip((query.NormalizedPage - 1) * query.NormalizedPageSize)
            .Take(query.NormalizedPageSize)
            .ToArrayAsync(cancellationToken);

        return Result.Success(new PagedResult<AdminContentImportJobResponse>(
            entities.Select(MapImportJob).ToArray(),
            query.NormalizedPage,
            query.NormalizedPageSize,
            total));
    }

    public async Task<Result<AdminContentImportJobResponse>>
        GetImportJobAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ContentImportJob>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
        {
            return Result.Failure<AdminContentImportJobResponse>(
                Error.NotFound(
                    "ContentImport.NotFound",
                    "Không tìm thấy import job."));
        }

        return Result.Success(MapImportJob(entity));
    }

    public async Task<Result<AdminContentImportJobResponse>>
        CreateImportJobAsync(
            CreateContentImportJobRequest request,
            CancellationToken cancellationToken = default)
    {
        var entity = new ContentImportJob(
            request.ImportType,
            request.OriginalFileName,
            request.StoragePath);

        _db.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success(MapImportJob(entity));
    }

    public async Task<Result<AdminContentImportJobResponse>>
        UpdateImportSourceAsync(
            long id,
            UpdateContentImportSourceRequest request,
            CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ContentImportJob>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
        {
            return Result.Failure<AdminContentImportJobResponse>(
                Error.NotFound(
                    "ContentImport.NotFound",
                    "Không tìm thấy import job."));
        }

        if (entity.Status != ContentImportStatus.Pending)
        {
            return Result.Failure<AdminContentImportJobResponse>(
                Error.Conflict(
                    "ContentImport.UpdateInvalidStatus",
                    "Chỉ import job Pending mới được cập nhật source."));
        }

        entity.UpdateSource(request.OriginalFileName, request.StoragePath);

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success(MapImportJob(entity));
    }

    public async Task<Result<IReadOnlyCollection<AdminContentImportRowResponse>>>
        GetImportRowsAsync(
            long importJobId,
            CancellationToken cancellationToken = default)
    {
        var jobExists = await _db.Set<ContentImportJob>()
            .AnyAsync(x => x.Id == importJobId, cancellationToken);

        if (!jobExists)
        {
            return Result.Failure<IReadOnlyCollection<AdminContentImportRowResponse>>(
                Error.NotFound(
                    "ContentImport.NotFound",
                    "Không tìm thấy import job."));
        }

        var entities = await _db.Set<ContentImportRow>()
            .AsNoTracking()
            .Where(x => x.ImportJobId == importJobId)
            .OrderBy(x => x.RowNumber)
            .Select(x => new AdminContentImportRowResponse(
                x.Id,
                x.RowNumber,
                x.SourceJson,
                x.IsSuccessful,
                x.CreatedEntityId,
                x.ErrorCode,
                x.ErrorMessage,
                x.ProcessedAt))
            .ToArrayAsync(cancellationToken);

        return Result.Success<IReadOnlyCollection<AdminContentImportRowResponse>>(entities);
    }

    public async Task<Result> DeleteImportJobAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity =
            await _db.Set<ContentImportJob>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "ContentImport.NotFound",
                    "Không tìm thấy import job."));
        }

        if (entity.Status != ContentImportStatus.Pending)
        {
            return Result.Failure(
                Error.Conflict(
                    "ContentImport.DeleteInvalidStatus",
                    "Chỉ import job Pending mới được xóa."));
        }

        _db.Remove(entity);

        await _db.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result<PagedResult<AdminContentReportResponse>>>
        GetReportsAsync(
            AdminContentReportQuery query,
            CancellationToken cancellationToken = default)
    {
        var q = _db.Set<ContentReport>().AsNoTracking();

        if (query.UserId.HasValue)
            q = q.Where(x => x.UserId == query.UserId.Value);

        if (query.EntityType.HasValue)
            q = q.Where(x => x.EntityType == query.EntityType.Value);

        if (query.Reason.HasValue)
            q = q.Where(x => x.Reason == query.Reason.Value);

        if (query.Status.HasValue)
            q = q.Where(x => x.Status == query.Status.Value);

        if (query.From.HasValue)
            q = q.Where(x => x.CreatedAt >= query.From.Value);

        if (query.To.HasValue)
            q = q.Where(x => x.CreatedAt <= query.To.Value);

        q = query.Sort switch
        {
            "createdAt" => q.OrderBy(x => x.CreatedAt),
            "-createdAt" => q.OrderByDescending(x => x.CreatedAt),
            "status" => q.OrderBy(x => x.Status),
            "-status" => q.OrderByDescending(x => x.Status),
            _ => q.OrderByDescending(x => x.CreatedAt)
        };

        var total = await q.CountAsync(cancellationToken);

        var entities = await q
            .Skip((query.NormalizedPage - 1) * query.NormalizedPageSize)
            .Take(query.NormalizedPageSize)
            .ToArrayAsync(cancellationToken);

        return Result.Success(new PagedResult<AdminContentReportResponse>(
            entities.Select(MapReport).ToArray(),
            query.NormalizedPage,
            query.NormalizedPageSize,
            total));
    }

    public async Task<Result<AdminContentReportResponse>>
        GetReportAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ContentReport>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
        {
            return Result.Failure<AdminContentReportResponse>(
                Error.NotFound(
                    "ContentReport.NotFound",
                    "Không tìm thấy report."));
        }

        return Result.Success(MapReport(entity));
    }

    public async Task<Result> StartReportReviewAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity =
            await _db.Set<ContentReport>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
            return Result.Failure(
                Error.NotFound(
                    "ContentReport.NotFound",
                    "Không tìm thấy report."));

        entity.StartReview();

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ResolveReportAsync(
        long id,
        Guid adminUserId,
        ResolveContentReportRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity =
            await _db.Set<ContentReport>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
            return Result.Failure(
                Error.NotFound(
                    "ContentReport.NotFound",
                    "Không tìm thấy report."));

        entity.Resolve(
            adminUserId,
            request.ResolutionNote);

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RejectReportAsync(
        long id,
        Guid adminUserId,
        ResolveContentReportRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity =
            await _db.Set<ContentReport>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
            return Result.Failure(
                Error.NotFound(
                    "ContentReport.NotFound",
                    "Không tìm thấy report."));

        entity.Reject(
            adminUserId,
            request.ResolutionNote);

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ReopenReportAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity =
            await _db.Set<ContentReport>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
            return Result.Failure(
                Error.NotFound(
                    "ContentReport.NotFound",
                    "Không tìm thấy report."));

        entity.Reopen();

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static AdminContentImportJobResponse MapImportJob(ContentImportJob entity)
        => new(
            entity.Id,
            entity.PublicId,
            entity.ImportType,
            entity.OriginalFileName,
            entity.StoragePath,
            entity.Status,
            entity.TotalRows,
            entity.ProcessedRows,
            entity.SuccessRows,
            entity.FailedRows,
            entity.StartedAt,
            entity.CompletedAt,
            entity.ErrorMessage,
            entity.CreatedAt,
            entity.UpdatedAt);

    private static AdminContentReportResponse MapReport(ContentReport entity)
        => new(
            entity.Id,
            entity.PublicId,
            entity.UserId,
            entity.EntityType,
            entity.EntityId,
            entity.Reason,
            entity.Description,
            entity.Status,
            entity.ResolvedByUserId,
            entity.ResolvedAt,
            entity.ResolutionNote,
            entity.CreatedAt,
            entity.UpdatedAt);
}
