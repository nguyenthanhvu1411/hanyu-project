using HanYu.Application.Common.Models;
using HanYu.Application.Features.Content.Admin.Imports;
using HanYu.Application.Features.Content.Admin.Reports;
using HanYu.Application.Interfaces.Content;
using HanYu.Domain.Entities.Content;
using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Content;

public sealed class ContentAdminService : IContentAdminService
{
    private readonly HanYuDbContext _db;

    public ContentAdminService(HanYuDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PagedResult<AdminContentImportJobResponse>>> GetImportJobsAsync(
        AdminContentImportJobQuery query,
        CancellationToken cancellationToken = default)
    {
        var q = _db.Set<ContentImportJob>().AsNoTracking();

        if (query.ImportType.HasValue) q = q.Where(x => x.ImportType == query.ImportType.Value);
        if (query.Status.HasValue) q = q.Where(x => x.Status == query.Status.Value);
        if (query.From.HasValue) q = q.Where(x => x.CreatedAt >= query.From.Value);
        if (query.To.HasValue) q = q.Where(x => x.CreatedAt <= query.To.Value);

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

    public async Task<Result<AdminContentImportJobResponse>> GetImportJobAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ContentImportJob>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null
            ? Result.Failure<AdminContentImportJobResponse>(Error.NotFound("ContentImport.NotFound", "Không tìm thấy import job."))
            : Result.Success(MapImportJob(entity));
    }

    public async Task<Result<AdminContentImportJobResponse>> CreateImportJobAsync(
        CreateContentImportJobRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = new ContentImportJob(request.ImportType, request.OriginalFileName, request.StoragePath);
        _db.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success(MapImportJob(entity));
    }

    public async Task<Result<AdminContentImportJobResponse>> UpdateImportSourceAsync(
        long id,
        UpdateContentImportSourceRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ContentImportJob>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return Result.Failure<AdminContentImportJobResponse>(Error.NotFound("ContentImport.NotFound", "Không tìm thấy import job."));
        if (entity.Status != ContentImportStatus.Pending)
            return Result.Failure<AdminContentImportJobResponse>(Error.Conflict("ContentImport.UpdateInvalidStatus", "Chỉ import job Pending mới được cập nhật source."));

        entity.UpdateSource(request.OriginalFileName, request.StoragePath);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success(MapImportJob(entity));
    }

    public async Task<Result<IReadOnlyCollection<AdminContentImportRowResponse>>> GetImportRowsAsync(
        long importJobId,
        CancellationToken cancellationToken = default)
    {
        var jobExists = await _db.Set<ContentImportJob>().AnyAsync(x => x.Id == importJobId, cancellationToken);
        if (!jobExists)
            return Result.Failure<IReadOnlyCollection<AdminContentImportRowResponse>>(Error.NotFound("ContentImport.NotFound", "Không tìm thấy import job."));

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

    public async Task<Result> DeleteImportJobAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ContentImportJob>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return Result.Failure(Error.NotFound("ContentImport.NotFound", "Không tìm thấy import job."));
        if (entity.Status != ContentImportStatus.Pending)
            return Result.Failure(Error.Conflict("ContentImport.DeleteInvalidStatus", "Chỉ import job Pending mới được xóa."));

        _db.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<PagedResult<AdminContentReportResponse>>> GetReportsAsync(
        AdminContentReportQuery query,
        CancellationToken cancellationToken = default)
    {
        var q = _db.Set<ContentReport>().AsNoTracking();

        if (query.UserId.HasValue) q = q.Where(x => x.UserId == query.UserId.Value);
        if (query.EntityType.HasValue) q = q.Where(x => x.EntityType == query.EntityType.Value);
        if (query.Reason.HasValue) q = q.Where(x => x.Reason == query.Reason.Value);
        if (query.Status.HasValue) q = q.Where(x => x.Status == query.Status.Value);
        if (query.From.HasValue) q = q.Where(x => x.CreatedAt >= query.From.Value);
        if (query.To.HasValue) q = q.Where(x => x.CreatedAt <= query.To.Value);

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

        var responses = await MapReportsAsync(entities, cancellationToken);
        return Result.Success(new PagedResult<AdminContentReportResponse>(
            responses,
            query.NormalizedPage,
            query.NormalizedPageSize,
            total));
    }

    public async Task<Result<AdminContentReportResponse>> GetReportAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ContentReport>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
            return Result.Failure<AdminContentReportResponse>(Error.NotFound("ContentReport.NotFound", "Không tìm thấy report."));

        var responses = await MapReportsAsync(new[] { entity }, cancellationToken);
        return Result.Success(responses[0]);
    }

    public async Task<Result> StartReportReviewAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ContentReport>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return Result.Failure(Error.NotFound("ContentReport.NotFound", "Không tìm thấy report."));
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
        var entity = await _db.Set<ContentReport>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return Result.Failure(Error.NotFound("ContentReport.NotFound", "Không tìm thấy report."));
        entity.Resolve(adminUserId, request.ResolutionNote);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> RejectReportAsync(
        long id,
        Guid adminUserId,
        ResolveContentReportRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ContentReport>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return Result.Failure(Error.NotFound("ContentReport.NotFound", "Không tìm thấy report."));
        entity.Reject(adminUserId, request.ResolutionNote);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ReopenReportAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ContentReport>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return Result.Failure(Error.NotFound("ContentReport.NotFound", "Không tìm thấy report."));
        entity.Reopen();
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<AdminContentReportResponse[]> MapReportsAsync(
        IReadOnlyCollection<ContentReport> reports,
        CancellationToken cancellationToken)
    {
        if (reports.Count == 0) return Array.Empty<AdminContentReportResponse>();

        var userIds = reports
            .Select(x => x.UserId)
            .Concat(reports.Where(x => x.ResolvedByUserId.HasValue).Select(x => x.ResolvedByUserId!.Value))
            .Distinct()
            .ToArray();

        var users = await _db.Set<User>()
            .AsNoTracking()
            .Where(x => userIds.Contains(x.Id))
            .Select(x => new UserLabel(
                x.Id,
                x.Profile != null ? x.Profile.DisplayName : x.UserName,
                x.Email))
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var labels = new Dictionary<(ContentEntityType Type, long Id), string>();
        await AddEntityLabelsAsync(reports, labels, cancellationToken);

        return reports.Select(entity =>
        {
            users.TryGetValue(entity.UserId, out var reporter);
            UserLabel? resolver = null;
            if (entity.ResolvedByUserId.HasValue)
                users.TryGetValue(entity.ResolvedByUserId.Value, out resolver);
            labels.TryGetValue((entity.EntityType, entity.EntityId), out var entityLabel);

            return new AdminContentReportResponse(
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
                entity.UpdatedAt,
                reporter?.DisplayName,
                reporter?.Email,
                entityLabel,
                resolver?.DisplayName);
        }).ToArray();
    }

    private async Task AddEntityLabelsAsync(
        IReadOnlyCollection<ContentReport> reports,
        Dictionary<(ContentEntityType Type, long Id), string> labels,
        CancellationToken cancellationToken)
    {
        long[] Ids(ContentEntityType type) => reports.Where(x => x.EntityType == type).Select(x => x.EntityId).Distinct().ToArray();

        var ids = Ids(ContentEntityType.Vocabulary);
        if (ids.Length > 0)
        {
            var rows = await _db.Set<HanYu.Domain.Entities.Vocabulary.Vocabulary>()
                .AsNoTracking().Where(x => ids.Contains(x.Id))
                .Select(x => new EntityLabel(x.Id, x.Simplified + " · " + x.Pinyin)).ToArrayAsync(cancellationToken);
            foreach (var row in rows) labels[(ContentEntityType.Vocabulary, row.Id)] = row.Label;
        }

        ids = Ids(ContentEntityType.VocabularyExample);
        if (ids.Length > 0)
        {
            var rows = await _db.Set<HanYu.Domain.Entities.Vocabulary.VocabularyExample>()
                .AsNoTracking().Where(x => ids.Contains(x.Id))
                .Select(x => new EntityLabel(x.Id, x.SentenceZh)).ToArrayAsync(cancellationToken);
            foreach (var row in rows) labels[(ContentEntityType.VocabularyExample, row.Id)] = row.Label;
        }

        ids = Ids(ContentEntityType.Lesson);
        if (ids.Length > 0)
        {
            var rows = await _db.Set<HanYu.Domain.Entities.Lesson.Lesson>()
                .AsNoTracking().Where(x => ids.Contains(x.Id))
                .Select(x => new EntityLabel(x.Id, x.TitleVi)).ToArrayAsync(cancellationToken);
            foreach (var row in rows) labels[(ContentEntityType.Lesson, row.Id)] = row.Label;
        }

        ids = Ids(ContentEntityType.LessonSection);
        if (ids.Length > 0)
        {
            var rows = await _db.Set<HanYu.Domain.Entities.Lesson.LessonSection>()
                .AsNoTracking().Where(x => ids.Contains(x.Id))
                .Select(x => new EntityLabel(x.Id, x.TitleVi ?? ("Phần " + x.SectionType))).ToArrayAsync(cancellationToken);
            foreach (var row in rows) labels[(ContentEntityType.LessonSection, row.Id)] = row.Label;
        }

        ids = Ids(ContentEntityType.QuizQuestion);
        if (ids.Length > 0)
        {
            var rows = await _db.Set<HanYu.Domain.Entities.Quiz.QuizQuestion>()
                .AsNoTracking().Where(x => ids.Contains(x.Id))
                .Select(x => new EntityLabel(x.Id, x.Prompt)).ToArrayAsync(cancellationToken);
            foreach (var row in rows) labels[(ContentEntityType.QuizQuestion, row.Id)] = row.Label;
        }

        ids = Ids(ContentEntityType.AudioAsset);
        if (ids.Length > 0)
        {
            var rows = await _db.Set<HanYu.Domain.Entities.Vocabulary.AudioAsset>()
                .AsNoTracking().Where(x => ids.Contains(x.Id))
                .Select(x => new EntityLabel(x.Id, x.Voice ?? x.StoragePath)).ToArrayAsync(cancellationToken);
            foreach (var row in rows) labels[(ContentEntityType.AudioAsset, row.Id)] = row.Label;
        }

        ids = Ids(ContentEntityType.Course);
        if (ids.Length > 0)
        {
            var rows = await _db.Set<HanYu.Domain.Entities.Course.Course>()
                .AsNoTracking().Where(x => ids.Contains(x.Id))
                .Select(x => new EntityLabel(x.Id, x.TitleVi)).ToArrayAsync(cancellationToken);
            foreach (var row in rows) labels[(ContentEntityType.Course, row.Id)] = row.Label;
        }

        ids = Ids(ContentEntityType.CourseChapter);
        if (ids.Length > 0)
        {
            var rows = await _db.Set<HanYu.Domain.Entities.Course.CourseChapter>()
                .AsNoTracking().Where(x => ids.Contains(x.Id))
                .Select(x => new EntityLabel(x.Id, x.TitleVi)).ToArrayAsync(cancellationToken);
            foreach (var row in rows) labels[(ContentEntityType.CourseChapter, row.Id)] = row.Label;
        }
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

    private sealed record UserLabel(Guid Id, string? DisplayName, string? Email);
    private sealed record EntityLabel(long Id, string Label);
}
