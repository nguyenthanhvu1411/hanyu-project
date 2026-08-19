using System.Security.Cryptography;
using System.Text;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Lesson.Mapping;
using HanYu.Application.Features.Lesson.Public.Bookmarks;
using HanYu.Application.Features.Lesson.Public.Lessons;
using HanYu.Application.Features.Lesson.Public.Progress;
using HanYu.Application.Interfaces.Caching;
using HanYu.Application.Interfaces.Lesson;
using HanYu.Domain.Entities.Learning;
using HanYu.Domain.Entities.Lesson;
using HanYu.Domain.Enums;
using HanYu.Domain.Constants;
using HanYu.Application.Interfaces.Gamification;
using HanYu.Infrastructure.Options;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HanYu.Infrastructure.Lesson;

public sealed class LessonPublicService
    : ILessonPublicService
{
    private const string GenerationCacheKey =
        "lesson:public:generation";

    private readonly HanYuDbContext _db;
    private readonly ICacheService _cache;
    private readonly LessonCacheOptions _options;
    private readonly IGamificationService _gamification;
    private readonly IAchievementEvaluator _achievementEvaluator;

    public LessonPublicService(
        HanYuDbContext db,
        ICacheService cache,
        IOptions<LessonCacheOptions> options,
        IGamificationService gamification,
        IAchievementEvaluator achievementEvaluator)
    {
        _db = db;
        _cache = cache;
        _options = options.Value;
        _gamification = gamification;
        _achievementEvaluator = achievementEvaluator;
    }

    public async Task<Result<
        PagedResult<LessonListItemResponse>>>
        GetLessonsAsync(
            LessonQuery query,
            CancellationToken cancellationToken = default)
    {
        var generation =
            await GetGenerationAsync(
                cancellationToken);

        var rawKey =
            string.Join(
                "|",
                query.Q,
                query.HskLevel,
                query.Topic,
                query.Difficulty,
                query.Featured,
                query.Sort,
                query.NormalizedPage,
                query.NormalizedPageSize);

        var key =
            $"lesson:public:{generation}:list:" +
            Hash(rawKey);

        var cached =
            await _cache.GetAsync<
                PagedResult<LessonListItemResponse>>(
                key,
                cancellationToken);

        if (cached is not null)
            return Result.Success(cached);

        var source =
            _db.Set<Domain.Entities.Lesson.Lesson>()
                .AsNoTracking()
                .Include(x => x.HskLevel)
                .Include(x => x.Topic)
                .Where(
                    x =>
                        x.Status ==
                        ContentStatus.Published)
                .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Q))
        {
            var q =
                query.Q.Trim();

            source = source.Where(
                x =>
                    EF.Functions.ILike(
                        x.TitleVi,
                        $"%{q}%") ||
                    (
                        x.ShortDescriptionVi != null &&
                        EF.Functions.ILike(
                            x.ShortDescriptionVi,
                            $"%{q}%")
                    ));
        }

        if (query.HskLevel.HasValue)
            source = source.Where(
                x => x.HskLevelId ==
                     query.HskLevel.Value);

        if (!string.IsNullOrWhiteSpace(query.Topic))
        {
            var slug =
                query.Topic.Trim()
                    .ToLowerInvariant();

            source = source.Where(
                x =>
                    x.Topic != null &&
                    x.Topic.Slug == slug);
        }

        if (query.Difficulty.HasValue)
            source = source.Where(
                x => x.Difficulty ==
                     query.Difficulty.Value);

        if (query.Featured.HasValue)
            source = source.Where(
                x => x.IsFeatured ==
                     query.Featured.Value);

        source =
            query.Sort?.Trim()
                .ToLowerInvariant()
                switch
            {
                "-sortorder" =>
                    source.OrderByDescending(
                        x => x.SortOrder),

                "title" =>
                    source.OrderBy(
                        x => x.TitleVi),

                "-title" =>
                    source.OrderByDescending(
                        x => x.TitleVi),

                "hsk" =>
                    source
                        .OrderBy(x => x.HskLevelId)
                        .ThenBy(x => x.SortOrder),

                _ =>
                    source
                        .OrderBy(x => x.SortOrder)
                        .ThenBy(x => x.TitleVi)
            };

        var total =
            await source.LongCountAsync(
                cancellationToken);

        var entities =
            await source
                .Skip(
                    (query.NormalizedPage - 1) *
                    query.NormalizedPageSize)
                .Take(query.NormalizedPageSize)
                .ToArrayAsync(cancellationToken);

        var result =
            new PagedResult<LessonListItemResponse>(
                entities
                    .Select(LessonMapper.ToListItem)
                    .ToArray(),
                query.NormalizedPage,
                query.NormalizedPageSize,
                total);

        await _cache.SetAsync(
            key,
            result,
            TimeSpan.FromMinutes(
                _options.ListMinutes),
            cancellationToken);

        return Result.Success(result);
    }

    public async Task<Result<LessonDetailResponse>>
        GetLessonAsync(
            Guid? userId,
            Guid lessonPublicId,
            CancellationToken cancellationToken = default)
    {
        var entity =
            await LoadPublicLessonAsync(
                lessonPublicId,
                cancellationToken);

        if (entity is null)
        {
            return Result.Failure<LessonDetailResponse>(
                Error.NotFound(
                    "Lesson.NotFound",
                    "Không tìm thấy Lesson."));
        }

        var prerequisiteResult =
            await GetPrerequisiteStateAsync(
                userId,
                entity.Id,
                cancellationToken);

        var progress =
            userId.HasValue
                ? await _db.Set<UserLessonProgress>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.UserId == userId.Value &&
                            x.LessonId == entity.Id,
                        cancellationToken)
                : null;

        var bookmark =
            userId.HasValue &&
            await _db.Set<UserLessonBookmark>()
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.UserId == userId.Value &&
                        x.LessonId == entity.Id,
                    cancellationToken);

        Guid? lastSectionPublicId =
            null;

        if (progress?.LastSectionId is long lastSectionId)
        {
            lastSectionPublicId =
                await _db.Set<LessonSection>()
                    .AsNoTracking()
                    .Where(
                        x => x.Id == lastSectionId)
                    .Select(x => (Guid?)x.PublicId)
                    .FirstOrDefaultAsync(
                        cancellationToken);
        }

        var sectionIds = entity.Sections
            .Select(x => x.Id)
            .ToArray();

        var sectionMediaEntities = sectionIds.Length == 0
            ? Array.Empty<LessonSectionAsset>()
            : await _db.Set<LessonSectionAsset>()
                .AsNoTracking()
                .Include(x => x.LessonAsset)
                    .ThenInclude(x => x.AudioAsset)
                .Where(x => sectionIds.Contains(x.LessonSectionId))
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Id)
                .ToArrayAsync(cancellationToken);

        var sectionMedia = sectionMediaEntities
            .GroupBy(x => x.LessonSectionId)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyCollection<LessonSectionMediaResponse>)group
                    .Select(x => new LessonSectionMediaResponse(
                        x.PublicId,
                        x.LessonAsset.PublicId,
                        x.LessonAsset.AssetType,
                        x.LessonAsset.AudioAsset?.PublicUrl ?? x.LessonAsset.Url,
                        x.CaptionVi ?? x.LessonAsset.CaptionVi,
                        x.SortOrder,
                        x.IsRequired))
                    .ToArray());

        var response =
            new LessonDetailResponse(
                entity.PublicId,
                entity.Slug,
                entity.TitleVi,
                entity.ShortDescriptionVi,
                entity.DescriptionVi,
                entity.ObjectiveVi,
                entity.CoverImageUrl,
                entity.HskLevelId,
                entity.HskLevel.Code,
                entity.HskLevel.NameVi,
                entity.EstimatedMinutes,
                entity.Difficulty,
                entity.IsFeatured,
                entity.Topic?.Slug,
                entity.Topic?.NameVi,
                prerequisiteResult.IsLocked,
                bookmark,
                progress is null
                    ? null
                    : LessonMapper.ToProgress(
                        progress,
                        lastSectionPublicId),
                entity.Sections
                    .OrderBy(x => x.SortOrder)
                    .Select(x => LessonMapper.ToSection(
                        x,
                        sectionMedia.TryGetValue(x.Id, out var media)
                            ? media
                            : Array.Empty<LessonSectionMediaResponse>()))
                    .ToArray(),
                entity.LessonVocabularies
                    .Where(
                        x =>
                            x.Vocabulary.Status ==
                            ContentStatus.Published)
                    .OrderBy(x => x.SortOrder)
                    .Select(LessonMapper.ToVocabulary)
                    .ToArray(),
                entity.Assets
                    .OrderBy(x => x.SortOrder)
                    .Select(LessonMapper.ToAsset)
                    .ToArray(),
                prerequisiteResult.Items);

        return Result.Success(response);
    }

    public async Task<Result<LessonProgressResponse>>
        StartLessonAsync(
            Guid userId,
            Guid lessonPublicId,
            CancellationToken cancellationToken = default)
    {
        var lesson =
            await ResolvePublishedLessonAsync(
                lessonPublicId,
                cancellationToken);

        if (lesson.IsFailure)
            return Result.Failure<LessonProgressResponse>(
                lesson.Error);

        var prerequisite =
            await GetPrerequisiteStateAsync(
                userId,
                lesson.Value.Id,
                cancellationToken);

        if (prerequisite.IsLocked)
        {
            return Result.Failure<LessonProgressResponse>(
                Error.Forbidden(
                    "Lesson.Locked",
                    "Bạn chưa hoàn thành prerequisite của Lesson này."));
        }

        var progress =
            await _db.Set<UserLessonProgress>()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.LessonId == lesson.Value.Id,
                    cancellationToken);

        if (progress is null)
        {
            progress =
                new UserLessonProgress(
                    userId,
                    lesson.Value.Id);

            progress.Start();

            _db.Add(progress);
        }
        else
        {
            progress.RegisterAccess();
        }

        await _db.SaveChangesAsync(cancellationToken);

        var lastPublicId =
            await GetLastSectionPublicIdAsync(
                progress.LastSectionId,
                cancellationToken);

        return Result.Success(
            LessonMapper.ToProgress(
                progress,
                lastPublicId));
    }

    public async Task<Result<LessonProgressResponse>>
        SaveProgressAsync(
            Guid userId,
            Guid lessonPublicId,
            SaveLessonProgressRequest request,
            CancellationToken cancellationToken = default)
    {
        var lesson =
            await ResolvePublishedLessonAsync(
                lessonPublicId,
                cancellationToken);

        if (lesson.IsFailure)
            return Result.Failure<LessonProgressResponse>(
                lesson.Error);

        long? sectionId =
            null;

        if (request.LastSectionPublicId.HasValue)
        {
            sectionId =
                await _db.Set<LessonSection>()
                    .Where(
                        x =>
                            x.LessonId == lesson.Value.Id &&
                            x.PublicId ==
                            request.LastSectionPublicId.Value)
                    .Select(x => (long?)x.Id)
                    .FirstOrDefaultAsync(
                        cancellationToken);

            if (!sectionId.HasValue)
            {
                return Result.Failure<
                    LessonProgressResponse>(
                    Error.Validation(
                        "Lesson.InvalidSection",
                        "Section không thuộc Lesson."));
            }
        }

        var progress =
            await _db.Set<UserLessonProgress>()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.LessonId == lesson.Value.Id,
                    cancellationToken);

        if (progress is null)
        {
            progress =
                new UserLessonProgress(
                    userId,
                    lesson.Value.Id);

            _db.Add(progress);
        }

        progress.UpdateProgress(
            sectionId,
            request.LastPosition,
            request.CompletionPercent);

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success(
            LessonMapper.ToProgress(
                progress,
                request.LastSectionPublicId));
    }

    public async Task<Result<
        LessonSectionProgressResponse>>
        SaveSectionProgressAsync(
            Guid userId,
            Guid lessonPublicId,
            Guid sectionPublicId,
            SaveSectionProgressRequest request,
            CancellationToken cancellationToken = default)
    {
        var lesson =
            await ResolvePublishedLessonAsync(
                lessonPublicId,
                cancellationToken);

        if (lesson.IsFailure)
            return Result.Failure<
                LessonSectionProgressResponse>(
                lesson.Error);

        var section =
            await _db.Set<LessonSection>()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.LessonId == lesson.Value.Id &&
                        x.PublicId == sectionPublicId,
                    cancellationToken);

        if (section is null)
        {
            return Result.Failure<
                LessonSectionProgressResponse>(
                Error.NotFound(
                    "LessonSection.NotFound",
                    "Không tìm thấy Lesson Section."));
        }

        var progress =
            await _db.Set<UserLessonSectionProgress>()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.LessonSectionId ==
                            section.Id,
                    cancellationToken);

        if (progress is null)
        {
            progress =
                new UserLessonSectionProgress(
                    userId,
                    section.Id);

            _db.Add(progress);
        }

        if (!progress.IsCompleted)
        {
            progress.UpdateTimeSpent(
                request.TimeSpentSeconds);

            if (request.IsCompleted)
                progress.Complete();
        }

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success(
            LessonMapper.ToSectionProgress(
                progress,
                section.PublicId));
    }

    public async Task<Result<LessonProgressResponse>>
        CompleteLessonAsync(
            Guid userId,
            Guid lessonPublicId,
            CancellationToken cancellationToken = default)
    {
        var lesson =
            await ResolvePublishedLessonAsync(
                lessonPublicId,
                cancellationToken);

        if (lesson.IsFailure)
            return Result.Failure<LessonProgressResponse>(
                lesson.Error);

        var progress =
            await _db.Set<UserLessonProgress>()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.LessonId == lesson.Value.Id,
                    cancellationToken);

        if (progress is null)
        {
            return Result.Failure<LessonProgressResponse>(
                Error.Conflict(
                    "Lesson.NotStarted",
                    "Lesson chưa được bắt đầu."));
        }

        if (progress.Status ==
            LessonProgressStatus.Completed)
        {
            var currentLast =
                await GetLastSectionPublicIdAsync(
                    progress.LastSectionId,
                    cancellationToken);

            return Result.Success(
                LessonMapper.ToProgress(
                    progress,
                    currentLast));
        }

        var requiredSectionIds =
            await _db.Set<LessonSection>()
                .AsNoTracking()
                .Where(
                    x =>
                        x.LessonId == lesson.Value.Id &&
                        x.IsRequired)
                .Select(x => x.Id)
                .ToArrayAsync(cancellationToken);

        if (requiredSectionIds.Length > 0)
        {
            var completedRequired =
                await _db.Set<UserLessonSectionProgress>()
                    .AsNoTracking()
                    .CountAsync(
                        x =>
                            x.UserId == userId &&
                            requiredSectionIds.Contains(
                                x.LessonSectionId) &&
                            x.IsCompleted,
                        cancellationToken);

            if (completedRequired !=
                requiredSectionIds.Length)
            {
                return Result.Failure<
                    LessonProgressResponse>(
                    Error.Conflict(
                        "Lesson.RequiredSectionsIncomplete",
                        "Bạn chưa hoàn thành tất cả Section bắt buộc."));
            }
        }

        progress.Complete();

        await _db.SaveChangesAsync(cancellationToken);

        await _gamification.AwardXpAsync(
            userId,
            GamificationConstants.LessonCompletedXp,
            "Hoàn thành bài học",
            XpSources.Lesson,
            lessonPublicId.ToString(),
            cancellationToken);

        await _gamification.RegisterLearningActivityAsync(
            userId,
            DateTimeOffset.UtcNow,
            cancellationToken);
            
        await _achievementEvaluator.EvaluateAsync(
            userId,
            cancellationToken);

        var lastPublicId =
            await GetLastSectionPublicIdAsync(
                progress.LastSectionId,
                cancellationToken);

        return Result.Success(
            LessonMapper.ToProgress(
                progress,
                lastPublicId));
    }

    public async Task<Result> BookmarkAsync(
        Guid userId,
        Guid lessonPublicId,
        CancellationToken cancellationToken = default)
    {
        var lesson =
            await ResolvePublishedLessonAsync(
                lessonPublicId,
                cancellationToken);

        if (lesson.IsFailure)
            return Result.Failure(lesson.Error);

        var exists =
            await _db.Set<UserLessonBookmark>()
                .AnyAsync(
                    x =>
                        x.UserId == userId &&
                        x.LessonId == lesson.Value.Id,
                    cancellationToken);

        if (exists)
            return Result.Success();

        var bookmark =
            new UserLessonBookmark(
                userId,
                lesson.Value.Id);

        _db.Add(bookmark);

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RemoveBookmarkAsync(
        Guid userId,
        Guid lessonPublicId,
        CancellationToken cancellationToken = default)
    {
        var lesson =
            await ResolvePublishedLessonAsync(
                lessonPublicId,
                cancellationToken);

        if (lesson.IsFailure)
            return Result.Failure(lesson.Error);

        var bookmark =
            await _db.Set<UserLessonBookmark>()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.LessonId == lesson.Value.Id,
                    cancellationToken);

        if (bookmark is null)
            return Result.Success();

        _db.Remove(bookmark);

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<
        IReadOnlyCollection<LessonBookmarkResponse>>>
        GetBookmarksAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var values =
            await _db.Set<UserLessonBookmark>()
                .AsNoTracking()
                .Include(x => x.Lesson)
                .Where(
                    x =>
                        x.UserId == userId &&
                        x.Lesson.Status ==
                            ContentStatus.Published)
                .OrderByDescending(x => x.CreatedAt)
                .ToArrayAsync(cancellationToken);

        return Result.Success<
            IReadOnlyCollection<LessonBookmarkResponse>>(
            values
                .Select(LessonMapper.ToBookmark)
                .ToArray());
    }

    private async Task<
        (bool IsLocked,
         IReadOnlyCollection<
             LessonPrerequisiteResponse> Items)>
        GetPrerequisiteStateAsync(
            Guid? userId,
            long lessonId,
            CancellationToken cancellationToken)
    {
        var prerequisites =
            await _db.Set<LessonPrerequisite>()
                .AsNoTracking()
                .Include(x => x.RequiredLesson)
                .Where(x => x.LessonId == lessonId)
                .ToArrayAsync(cancellationToken);

        if (prerequisites.Length == 0)
        {
            return (
                false,
                Array.Empty<
                    LessonPrerequisiteResponse>());
        }

        HashSet<long> completedIds =
            [];

        if (userId.HasValue)
        {
            completedIds =
                (await _db.Set<UserLessonProgress>()
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.UserId == userId.Value &&
                            x.Status ==
                                LessonProgressStatus.Completed)
                    .Select(x => x.LessonId)
                    .ToArrayAsync(cancellationToken))
                .ToHashSet();
        }

        var items =
            prerequisites
                .Select(
                    x =>
                        new LessonPrerequisiteResponse(
                            x.RequiredLesson.PublicId,
                            x.RequiredLesson.Slug,
                            x.RequiredLesson.TitleVi,
                            completedIds.Contains(
                                x.RequiredLessonId)))
                .ToArray();

        var locked =
            items.Any(x => !x.IsCompleted);

        return (locked, items);
    }

    private async Task<
        Domain.Entities.Lesson.Lesson?>
        LoadPublicLessonAsync(
            Guid publicId,
            CancellationToken cancellationToken)
    {
        return await _db
            .Set<Domain.Entities.Lesson.Lesson>()
            .AsNoTracking()
            .Include(x => x.HskLevel)
            .Include(x => x.Topic)
            .Include(x => x.Sections)
            .Include(x => x.Assets)
                .ThenInclude(x => x.AudioAsset)
            .Include(x => x.LessonVocabularies)
                .ThenInclude(x => x.Vocabulary)
            .FirstOrDefaultAsync(
                x =>
                    x.PublicId == publicId &&
                    x.Status ==
                        ContentStatus.Published,
                cancellationToken);
    }

    private async Task<Result<
        Domain.Entities.Lesson.Lesson>>
        ResolvePublishedLessonAsync(
            Guid publicId,
            CancellationToken cancellationToken)
    {
        if (publicId == Guid.Empty)
        {
            return Result.Failure<
                Domain.Entities.Lesson.Lesson>(
                Error.Validation(
                    "Lesson.InvalidPublicId",
                    "Lesson PublicId không hợp lệ."));
        }

        var lesson =
            await _db.Set<
                    Domain.Entities.Lesson.Lesson>()
                .FirstOrDefaultAsync(
                    x =>
                        x.PublicId == publicId &&
                        x.Status ==
                            ContentStatus.Published,
                    cancellationToken);

        return lesson is null
            ? Result.Failure<
                Domain.Entities.Lesson.Lesson>(
                Error.NotFound(
                    "Lesson.NotFound",
                    "Không tìm thấy Lesson."))
            : Result.Success(lesson);
    }

    private async Task<Guid?>
        GetLastSectionPublicIdAsync(
            long? sectionId,
            CancellationToken cancellationToken)
    {
        if (!sectionId.HasValue)
            return null;

        return await _db.Set<LessonSection>()
            .AsNoTracking()
            .Where(x => x.Id == sectionId.Value)
            .Select(x => (Guid?)x.PublicId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<string> GetGenerationAsync(
        CancellationToken cancellationToken)
    {
        var value =
            await _cache.GetAsync<string>(
                GenerationCacheKey,
                cancellationToken);

        if (!string.IsNullOrWhiteSpace(value))
            return value;

        value =
            Guid.NewGuid().ToString("N");

        await _cache.SetAsync(
            GenerationCacheKey,
            value,
            TimeSpan.FromDays(
                _options.GenerationDays),
            cancellationToken);

        return value;
    }

    private static string Hash(
        string value)
    {
        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(value)));
    }

}
