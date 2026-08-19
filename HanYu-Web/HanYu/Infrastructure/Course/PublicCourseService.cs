using System.Text.Json;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Course.Public;
using HanYu.Application.Interfaces.Course;
using HanYu.Application.Interfaces.Persistence;
using HanYu.Domain.Entities.Course;
using HanYu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HanYu.Infrastructure.Course;

public sealed class PublicCourseService : IPublicCourseService
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(1);

    private readonly IHanYuDbContext _dbContext;
    private readonly IDistributedCache _cache;

    public PublicCourseService(
        IHanYuDbContext dbContext,
        IDistributedCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    private async Task<string> GetCacheVersionAsync(CancellationToken cancellationToken)
    {
        var version = await _cache.GetStringAsync(CourseCacheKeys.Version, cancellationToken);
        if (string.IsNullOrEmpty(version))
        {
            version = Guid.NewGuid().ToString("N");
            await _cache.SetStringAsync(CourseCacheKeys.Version, version, cancellationToken);
        }
        return version;
    }

    public async Task<Result<PagedResult<PublicCourseListItemDto>>> GetPublishedCoursesAsync(
        PublicCourseQuery request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var version = await GetCacheVersionAsync(cancellationToken);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? DefaultPageSize : Math.Min(request.PageSize, MaxPageSize);

        var queryKey = $"p{page}_s{pageSize}_{request.HskCode}_{request.Search?.ToLowerInvariant()}";
        var cacheKey = CourseCacheKeys.List(version, queryKey);

        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            var cachedResult = JsonSerializer.Deserialize<PagedResult<PublicCourseListItemDto>>(cachedData);
            if (cachedResult is null)
                return Result.Failure<PagedResult<PublicCourseListItemDto>>(
                    Error.Failure("Cache.Error", "Error deserializing cache"));

            return Result.Success(cachedResult);
        }

        var query = _dbContext.Courses
            .AsNoTracking()
            .Where(x => x.Status == ContentStatus.Published && x.IsActive);

        if (!string.IsNullOrWhiteSpace(request.HskCode))
        {
            query = query.Where(x => x.HskLevel != null && x.HskLevel.Code == request.HskCode);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var keyword = request.Search.Trim();
            query = query.Where(
                x => EF.Functions.ILike(x.Code, $"%{keyword}%") ||
                     EF.Functions.ILike(x.Slug, $"%{keyword}%") ||
                     EF.Functions.ILike(x.TitleVi, $"%{keyword}%"));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.IsFeatured)
            .ThenBy(x => x.SortOrder)
            .ThenByDescending(x => x.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new PublicCourseListItemDto(
                x.PublicId,
                x.Slug,
                x.TitleVi,
                x.ShortDescriptionVi,
                x.HskLevel != null ? x.HskLevel.Code : null,
                x.HskLevel != null ? x.HskLevel.NameVi : null,
                x.CoverImageUrl,
                x.EstimatedMinutes,
                x.IsFeatured,
                x.Chapters.Count(c => c.DeletedAt == null && c.IsActive)))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<PublicCourseListItemDto>(items, page, pageSize, total);

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(result),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _cacheDuration },
            cancellationToken);

        return Result.Success(result);
    }

    public async Task<Result<PublicCourseDetailDto>> GetCourseAsync(
        Guid publicId,
        CancellationToken cancellationToken = default)
    {
        if (publicId == Guid.Empty)
        {
            return Result.Failure<PublicCourseDetailDto>(
                Error.Validation("Course.InvalidId", "Public ID không hợp lệ."));
        }

        var version = await GetCacheVersionAsync(cancellationToken);
        var cacheKey = CourseCacheKeys.Detail(version, publicId);

        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            var cachedResult = JsonSerializer.Deserialize<PublicCourseDetailDto>(cachedData);
            if (cachedResult is null)
                return Result.Failure<PublicCourseDetailDto>(
                    Error.Failure("Cache.Error", "Error deserializing cache"));

            return Result.Success(cachedResult);
        }

        var course = await _dbContext.Courses
            .AsNoTracking()
            .Include(x => x.HskLevel)
            .Include(x => x.Prerequisites)
                .ThenInclude(p => p.RequiredCourse)
            .FirstOrDefaultAsync(
                x => x.PublicId == publicId && x.Status == ContentStatus.Published && x.IsActive,
                cancellationToken);

        if (course is null)
        {
            return Result.Failure<PublicCourseDetailDto>(
                Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));
        }

        var result = CoursePublicMapper.ToDetailDto(course);

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(result),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _cacheDuration },
            cancellationToken);

        return Result.Success(result);
    }

    public async Task<Result<PublicCourseDetailDto>> GetCourseBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return Result.Failure<PublicCourseDetailDto>(
                Error.Validation("Course.InvalidSlug", "Slug không hợp lệ."));
        }

        slug = slug.Trim().ToLowerInvariant();

        var version = await GetCacheVersionAsync(cancellationToken);
        var cacheKey = CourseCacheKeys.Slug(version, slug);

        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            var cachedResult = JsonSerializer.Deserialize<PublicCourseDetailDto>(cachedData);
            if (cachedResult is null)
                return Result.Failure<PublicCourseDetailDto>(
                    Error.Failure("Cache.Error", "Error deserializing cache"));

            return Result.Success(cachedResult);
        }

        var course = await _dbContext.Courses
            .AsNoTracking()
            .Include(x => x.HskLevel)
            .Include(x => x.Prerequisites)
                .ThenInclude(p => p.RequiredCourse)
            .FirstOrDefaultAsync(
                x => x.Slug == slug && x.Status == ContentStatus.Published && x.IsActive,
                cancellationToken);

        if (course is null)
        {
            return Result.Failure<PublicCourseDetailDto>(
                Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));
        }

        var result = CoursePublicMapper.ToDetailDto(course);

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(result),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _cacheDuration },
            cancellationToken);

        return Result.Success(result);
    }

    public async Task<Result<IReadOnlyCollection<PublicCourseLessonDto>>>
        GetLessonsByCourseSlugAsync(
            string slug,
            CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return Result.Failure<IReadOnlyCollection<PublicCourseLessonDto>>(
                Error.Validation("Course.InvalidSlug", "Slug không hợp lệ."));
        }

        slug = slug.Trim().ToLowerInvariant();

        var version = await GetCacheVersionAsync(cancellationToken);
        var cacheKey = CourseCacheKeys.Lessons(version, slug);

        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            var cachedItems = JsonSerializer.Deserialize<List<PublicCourseLessonDto>>(cachedData);
            if (cachedItems is null)
            {
                return Result.Failure<IReadOnlyCollection<PublicCourseLessonDto>>(
                    Error.Failure("Cache.Error", "Error deserializing cache"));
            }

            return Result.Success<IReadOnlyCollection<PublicCourseLessonDto>>(cachedItems);
        }

        var courseId = await _dbContext.Courses
            .AsNoTracking()
            .Where(x =>
                x.Slug == slug &&
                x.Status == ContentStatus.Published &&
                x.IsActive)
            .Select(x => (long?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!courseId.HasValue)
        {
            return Result.Failure<IReadOnlyCollection<PublicCourseLessonDto>>(
                Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));
        }

        var items = await (
            from lesson in _dbContext.Lessons.AsNoTracking()
            join chapter in _dbContext.Set<CourseChapter>().AsNoTracking()
                on lesson.CourseChapterId equals (long?)chapter.Id
            where lesson.Status == ContentStatus.Published &&
                  chapter.CourseId == courseId.Value &&
                  chapter.DeletedAt == null &&
                  chapter.IsActive
            orderby chapter.SortOrder, lesson.SortOrder, lesson.Id
            select new PublicCourseLessonDto(
                lesson.PublicId,
                lesson.Slug,
                lesson.TitleVi,
                lesson.SortOrder,
                lesson.EstimatedMinutes))
            .ToListAsync(cancellationToken);

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(items),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _cacheDuration },
            cancellationToken);

        return Result.Success<IReadOnlyCollection<PublicCourseLessonDto>>(items);
    }

    public async Task<Result<PublicCourseCurriculumDto>> GetCurriculumAsync(
        Guid publicId,
        CancellationToken cancellationToken = default)
    {
        if (publicId == Guid.Empty)
        {
            return Result.Failure<PublicCourseCurriculumDto>(
                Error.Validation("Course.InvalidId", "Public ID không hợp lệ."));
        }

        var version = await GetCacheVersionAsync(cancellationToken);
        var cacheKey = CourseCacheKeys.Curriculum(version, publicId);

        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            var cachedResult = JsonSerializer.Deserialize<PublicCourseCurriculumDto>(cachedData);
            if (cachedResult is null)
                return Result.Failure<PublicCourseCurriculumDto>(
                    Error.Failure("Cache.Error", "Error deserializing cache"));

            return Result.Success(cachedResult);
        }

        var course = await _dbContext.Courses
            .AsNoTracking()
            .Where(x => x.PublicId == publicId && x.Status == ContentStatus.Published && x.IsActive)
            .Select(x => new
            {
                x.PublicId,
                x.Slug,
                x.TitleVi,
                x.EstimatedMinutes,
                Chapters = x.Chapters
                    .Where(chapter => chapter.DeletedAt == null && chapter.IsActive)
                    .OrderBy(chapter => chapter.SortOrder)
                    .Select(chapter => new PublicCourseChapterDto(
                        chapter.PublicId,
                        chapter.TitleVi,
                        chapter.DescriptionVi,
                        chapter.SortOrder,
                        chapter.Lessons.Count(lesson => lesson.Status == ContentStatus.Published && lesson.DeletedAt == null),
                        chapter.Lessons
                            .Where(lesson => lesson.Status == ContentStatus.Published && lesson.DeletedAt == null)
                            .OrderBy(lesson => lesson.SortOrder)
                            .Select(lesson => new PublicCourseLessonDto(
                                lesson.PublicId,
                                lesson.Slug,
                                lesson.TitleVi,
                                lesson.SortOrder,
                                lesson.EstimatedMinutes))
                            .ToList()))
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (course is null)
        {
            return Result.Failure<PublicCourseCurriculumDto>(
                Error.NotFound("Course.NotFound", "Không tìm thấy khóa học."));
        }

        var lessonCount = course.Chapters.Sum(x => x.LessonCount);

        var result = new PublicCourseCurriculumDto(
            course.PublicId,
            course.Slug,
            course.TitleVi,
            course.Chapters.Count,
            lessonCount,
            course.EstimatedMinutes,
            course.Chapters);

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(result),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _cacheDuration },
            cancellationToken);

        return Result.Success(result);
    }
}
