using HanYu.Application.Common.Models;
using HanYu.Application.Features.AI.Admin.Cache;
using HanYu.Application.Features.AI.Admin.Conversations;
using HanYu.Application.Features.AI.Admin.Feedback;
using HanYu.Application.Features.AI.Admin.Requests;
using HanYu.Application.Interfaces.AI;
using HanYu.Domain.Entities.AI;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.AI;

public sealed class AiAdminService : IAiAdminService
{
    private readonly HanYuDbContext _db;

    public AiAdminService(HanYuDbContext db)
    {
        _db = db;
    }

    public async Task<Result<AdminAiDashboardResponse>>
        GetDashboardAsync(
            CancellationToken cancellationToken = default)
    {
        var today = DateTimeOffset.UtcNow.Date;
        var nextDay = today.AddDays(1);
        
        var q = _db.Set<AiRequest>()
            .AsNoTracking()
            .Where(x => x.RequestedAt >= today && x.RequestedAt < nextDay);

        var requests = await q.CountAsync(cancellationToken);
        var completed = await q.CountAsync(x => x.Status == AiRequestStatus.Completed, cancellationToken);
        var failed = await q.CountAsync(x => x.Status == AiRequestStatus.Failed, cancellationToken);
        var cancelled = await q.CountAsync(x => x.Status == AiRequestStatus.Cancelled, cancellationToken);
        
        var inputTokens = await q.SumAsync(x => (long)x.InputTokens, cancellationToken);
        var outputTokens = await q.SumAsync(x => (long)x.OutputTokens, cancellationToken);
        var totalTokens = await q.SumAsync(x => (long)x.TotalTokens, cancellationToken);
        var cost = await q.SumAsync(x => x.EstimatedCostUsd ?? 0m, cancellationToken);
        
        var latencyAvg = await q
            .Where(x => x.LatencyMs.HasValue)
            .AverageAsync(x => (double)x.LatencyMs!.Value, cancellationToken);

        return Result.Success(new AdminAiDashboardResponse(
            requests,
            completed,
            failed,
            cancelled,
            inputTokens,
            outputTokens,
            totalTokens,
            cost,
            (decimal)latencyAvg));
    }

    public async Task<Result<PagedResult<AdminAiRequestResponse>>>
        GetRequestsAsync(
            AdminAiRequestQuery query,
            CancellationToken cancellationToken = default)
    {
        var q = _db.Set<AiRequest>().AsNoTracking();

        if (query.UserId.HasValue)
            q = q.Where(x => x.UserId == query.UserId.Value);

        if (query.FeatureType.HasValue)
            q = q.Where(x => x.FeatureType == query.FeatureType.Value);

        if (query.Status.HasValue)
            q = q.Where(x => x.Status == query.Status.Value);

        if (!string.IsNullOrWhiteSpace(query.Provider))
            q = q.Where(x => x.Provider == query.Provider);

        if (!string.IsNullOrWhiteSpace(query.Model))
            q = q.Where(x => x.Model == query.Model);

        if (query.From.HasValue)
            q = q.Where(x => x.RequestedAt >= query.From.Value);

        if (query.To.HasValue)
            q = q.Where(x => x.RequestedAt <= query.To.Value);

        q = query.Sort switch
        {
            "requestedAt" => q.OrderBy(x => x.RequestedAt),
            "-requestedAt" => q.OrderByDescending(x => x.RequestedAt),
            "status" => q.OrderBy(x => x.Status),
            "-status" => q.OrderByDescending(x => x.Status),
            "cost" => q.OrderBy(x => x.EstimatedCostUsd),
            "-cost" => q.OrderByDescending(x => x.EstimatedCostUsd),
            _ => q.OrderByDescending(x => x.RequestedAt)
        };

        var total = await q.CountAsync(cancellationToken);

        var entities = await q
            .Skip((query.NormalizedPage - 1) * query.NormalizedPageSize)
            .Take(query.NormalizedPageSize)
            .ToArrayAsync(cancellationToken);

        var responses = entities.Select(x => new AdminAiRequestResponse(
            x.Id,
            x.PublicId,
            x.UserId,
            x.ConversationId,
            x.VocabularyId,
            x.LessonId,
            x.QuizAttemptAnswerId,
            x.FeatureType,
            x.Provider,
            x.Model,
            x.RequestHash,
            x.PromptVersion,
            x.InputTokens,
            x.OutputTokens,
            x.TotalTokens,
            x.EstimatedCostUsd,
            x.LatencyMs,
            x.Status,
            x.ErrorCode,
            x.ErrorMessage,
            x.RequestedAt,
            x.CompletedAt)).ToArray();

        return Result.Success(new PagedResult<AdminAiRequestResponse>(
            responses,
            query.NormalizedPage,
            query.NormalizedPageSize,
            total));
    }

    public async Task<Result<AdminAiRequestResponse>>
        GetRequestAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<AiRequest>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
        {
            return Result.Failure<AdminAiRequestResponse>(
                Error.NotFound("AI.RequestNotFound", "Không tìm thấy request AI."));
        }

        return Result.Success(new AdminAiRequestResponse(
            entity.Id,
            entity.PublicId,
            entity.UserId,
            entity.ConversationId,
            entity.VocabularyId,
            entity.LessonId,
            entity.QuizAttemptAnswerId,
            entity.FeatureType,
            entity.Provider,
            entity.Model,
            entity.RequestHash,
            entity.PromptVersion,
            entity.InputTokens,
            entity.OutputTokens,
            entity.TotalTokens,
            entity.EstimatedCostUsd,
            entity.LatencyMs,
            entity.Status,
            entity.ErrorCode,
            entity.ErrorMessage,
            entity.RequestedAt,
            entity.CompletedAt));
    }

    public async Task<Result> CancelRequestAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<AiRequest>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
        {
            return Result.Failure(Error.NotFound("AI.RequestNotFound", "Không tìm thấy request AI."));
        }

        if (entity.Status != AiRequestStatus.Pending)
        {
            return Result.Failure(Error.Conflict("AI.RequestNotPending", "Chỉ request Pending mới có thể cancel."));
        }

        entity.Cancel();
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<PagedResult<AdminAiConversationResponse>>>
        GetConversationsAsync(
            AdminAiConversationQuery query,
            CancellationToken cancellationToken = default)
    {
        var q = _db.Set<AiConversation>().AsNoTracking();

        if (query.UserId.HasValue)
            q = q.Where(x => x.UserId == query.UserId.Value);

        if (query.Status.HasValue)
            q = q.Where(x => x.Status == query.Status.Value);

        q = query.Sort switch
        {
            "updatedAt" => q.OrderBy(x => x.UpdatedAt),
            "-updatedAt" => q.OrderByDescending(x => x.UpdatedAt),
            "createdAt" => q.OrderBy(x => x.CreatedAt),
            "-createdAt" => q.OrderByDescending(x => x.CreatedAt),
            _ => q.OrderByDescending(x => x.UpdatedAt)
        };

        var total = await q.CountAsync(cancellationToken);

        var entities = await q
            .Skip((query.NormalizedPage - 1) * query.NormalizedPageSize)
            .Take(query.NormalizedPageSize)
            .ToArrayAsync(cancellationToken);

        var responses = entities.Select(x => new AdminAiConversationResponse(
            x.Id,
            x.PublicId,
            x.UserId,
            x.Title,
            x.Status,
            x.MessageCount,
            x.LastMessageAt,
            x.CreatedAt,
            x.UpdatedAt)).ToArray();

        return Result.Success(new PagedResult<AdminAiConversationResponse>(
            responses,
            query.NormalizedPage,
            query.NormalizedPageSize,
            total));
    }

    public async Task<Result<AdminAiConversationResponse>>
        GetConversationAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<AiConversation>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
        {
            return Result.Failure<AdminAiConversationResponse>(
                Error.NotFound("AI.ConversationNotFound", "Không tìm thấy conversation."));
        }

        return Result.Success(new AdminAiConversationResponse(
            entity.Id,
            entity.PublicId,
            entity.UserId,
            entity.Title,
            entity.Status,
            entity.MessageCount,
            entity.LastMessageAt,
            entity.CreatedAt,
            entity.UpdatedAt));
    }

    public async Task<Result<PagedResult<AdminAiFeedbackResponse>>>
        GetFeedbacksAsync(
            AdminAiFeedbackQuery query,
            CancellationToken cancellationToken = default)
    {
        var q = _db.Set<AiFeedback>().AsNoTracking();

        if (query.UserId.HasValue)
            q = q.Where(x => x.UserId == query.UserId.Value);

        if (query.Rating.HasValue)
            q = q.Where(x => x.Rating == query.Rating.Value);

        if (!string.IsNullOrWhiteSpace(query.IssueType))
            q = q.Where(x => x.IssueType == query.IssueType);

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

        var responses = entities.Select(x => new AdminAiFeedbackResponse(
            x.Id,
            x.PublicId,
            x.UserId,
            x.AiRequestId,
            x.Rating,
            x.Comment,
            x.IssueType,
            x.CreatedAt,
            x.CreatedAt)).ToArray();

        return Result.Success(new PagedResult<AdminAiFeedbackResponse>(
            responses,
            query.NormalizedPage,
            query.NormalizedPageSize,
            total));
    }

    public async Task<Result<PagedResult<AdminAiCacheResponse>>>
        GetCacheAsync(
            AdminAiCacheQuery query,
            CancellationToken cancellationToken = default)
    {
        var q = _db.Set<AiResponseCache>().AsNoTracking();

        if (query.FeatureType.HasValue)
            q = q.Where(x => x.FeatureType == query.FeatureType.Value);

        if (!string.IsNullOrWhiteSpace(query.Model))
            q = q.Where(x => x.Model == query.Model);

        var now = DateTimeOffset.UtcNow;
        if (query.Expired.HasValue)
        {
            if (query.Expired.Value)
            {
                q = q.Where(x => x.ExpiresAt.HasValue && x.ExpiresAt.Value < now);
            }
            else
            {
                q = q.Where(x => !x.ExpiresAt.HasValue || x.ExpiresAt.Value >= now);
            }
        }

        q = query.Sort switch
        {
            "updatedAt" => q.OrderBy(x => x.UpdatedAt),
            "-updatedAt" => q.OrderByDescending(x => x.UpdatedAt),
            "hitCount" => q.OrderBy(x => x.HitCount),
            "-hitCount" => q.OrderByDescending(x => x.HitCount),
            _ => q.OrderByDescending(x => x.UpdatedAt)
        };

        var total = await q.CountAsync(cancellationToken);

        var entities = await q
            .Skip((query.NormalizedPage - 1) * query.NormalizedPageSize)
            .Take(query.NormalizedPageSize)
            .ToArrayAsync(cancellationToken);

        var responses = entities.Select(x => new AdminAiCacheResponse(
            x.Id,
            x.PublicId,
            x.FeatureType,
            x.CacheKey,
            x.Model,
            x.PromptVersion,
            x.HitCount,
            x.LastAccessedAt,
            x.ExpiresAt,
            x.ExpiresAt.HasValue && x.ExpiresAt.Value < now,
            x.CreatedAt,
            x.UpdatedAt)).ToArray();

        return Result.Success(new PagedResult<AdminAiCacheResponse>(
            responses,
            query.NormalizedPage,
            query.NormalizedPageSize,
            total));
    }

    public async Task<Result> DeleteExpiredCacheEntryAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<AiResponseCache>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
        {
            return Result.Failure(Error.NotFound("AI.CacheNotFound", "Không tìm thấy cache AI."));
        }

        if (!entity.ExpiresAt.HasValue || entity.ExpiresAt.Value >= DateTimeOffset.UtcNow)
        {
            return Result.Failure(Error.Conflict("AI.CacheNotExpired", "Chỉ cache đã expired mới có thể bị xóa thủ công."));
        }

        _db.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<int>> DeleteExpiredCacheAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var expiredEntities = await _db.Set<AiResponseCache>()
            .Where(x => x.ExpiresAt.HasValue && x.ExpiresAt.Value < now)
            .ToArrayAsync(cancellationToken);

        var count = expiredEntities.Length;

        if (count > 0)
        {
            _db.RemoveRange(expiredEntities);
            await _db.SaveChangesAsync(cancellationToken);
        }

        return Result.Success(count);
    }
}
