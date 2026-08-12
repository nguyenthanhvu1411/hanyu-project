using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Persistence;
using HanYu.Domain.Entities.Course;
using HanYu.Domain.Entities.Lesson;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Application.Features.Lesson.Admin.Lessons;

public enum LessonWorkflowValidationTarget
{
    General = 0,
    SubmitReview = 1,
    Publish = 2
}

/// <summary>
/// Central validation pipeline for Lesson workflow transitions.
/// The validator is intentionally read-only and may be called by API guards,
/// admin preview screens and integration tests without mutating the aggregate.
/// </summary>
public static class LessonWorkflowValidator
{
    public static async Task<Result<LessonValidationResultDto>> ValidateAsync(
        IHanYuDbContext dbContext,
        long lessonId,
        LessonWorkflowValidationTarget target = LessonWorkflowValidationTarget.General,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        if (lessonId <= 0)
        {
            return Result.Failure<LessonValidationResultDto>(
                Error.Validation(
                    "Lesson.InvalidId",
                    "Lesson ID không hợp lệ."));
        }

        var lesson = await dbContext.Lessons
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == lessonId,
                cancellationToken);

        if (lesson is null)
        {
            return Result.Failure<LessonValidationResultDto>(
                Error.NotFound(
                    "Lesson.NotFound",
                    "Không tìm thấy Lesson."));
        }

        var issues = new List<LessonValidationIssueDto>();

        void AddError(
            string code,
            string message,
            string? field = null)
            => issues.Add(
                new LessonValidationIssueDto(
                    code,
                    message,
                    field,
                    LessonValidationSeverity.Error));

        void AddWarning(
            string code,
            string message,
            string? field = null)
            => issues.Add(
                new LessonValidationIssueDto(
                    code,
                    message,
                    field,
                    LessonValidationSeverity.Warning));

        // ========================================================
        // 1. Aggregate / metadata
        // ========================================================

        if (lesson.IsDeleted)
        {
            AddError(
                "Lesson.Deleted",
                "Lesson đã bị xóa và không thể tiếp tục workflow.",
                "deletedAt");
        }

        if (string.IsNullOrWhiteSpace(lesson.Slug))
        {
            AddError(
                "Lesson.SlugRequired",
                "Lesson chưa có Slug.",
                "slug");
        }
        else
        {
            var duplicateSlug = await dbContext.Lessons
                .IgnoreQueryFilters()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id != lesson.Id && x.Slug == lesson.Slug,
                    cancellationToken);

            if (duplicateSlug)
            {
                AddError(
                    "Lesson.SlugDuplicate",
                    "Slug đang được một Lesson khác sử dụng.",
                    "slug");
            }
        }

        if (string.IsNullOrWhiteSpace(lesson.TitleVi))
        {
            AddError(
                "Lesson.TitleRequired",
                "Lesson chưa có tiêu đề.",
                "titleVi");
        }

        if (lesson.SortOrder < 0)
        {
            AddError(
                "Lesson.SortOrderInvalid",
                "Thứ tự Lesson không hợp lệ.",
                "sortOrder");
        }

        if (lesson.EstimatedMinutes is < 1 or > 300)
        {
            AddError(
                "Lesson.EstimatedMinutesInvalid",
                "Thời lượng Lesson phải từ 1 đến 300 phút.",
                "estimatedMinutes");
        }

        if (lesson.Difficulty is < 1 or > 5)
        {
            AddError(
                "Lesson.DifficultyInvalid",
                "Độ khó Lesson phải từ 1 đến 5.",
                "difficulty");
        }

        if (target == LessonWorkflowValidationTarget.SubmitReview &&
            lesson.Status != ContentStatus.Draft)
        {
            AddError(
                "Lesson.NotDraft",
                "Chỉ Lesson Draft mới có thể gửi duyệt.",
                "status");
        }

        if (target == LessonWorkflowValidationTarget.Publish &&
            lesson.Status != ContentStatus.Approved)
        {
            AddError(
                "Lesson.NotApproved",
                "Lesson phải ở trạng thái Approved trước khi Publish.",
                "status");
        }

        // ========================================================
        // 2. HSK / Topic / Course / Chapter
        // ========================================================

        var hskLevel = await dbContext.HskLevels
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == lesson.HskLevelId,
                cancellationToken);

        if (hskLevel is null)
        {
            AddError(
                "Lesson.HskNotFound",
                "HSK Level không tồn tại.",
                "hskLevelId");
        }
        else
        {
            if (hskLevel.IsDeleted)
            {
                AddError(
                    "Lesson.HskDeleted",
                    "HSK Level đã bị xóa.",
                    "hskLevelId");
            }

            if (!hskLevel.IsActive)
            {
                AddError(
                    "Lesson.HskInactive",
                    "HSK Level đang bị vô hiệu.",
                    "hskLevelId");
            }
        }

        if (lesson.TopicId.HasValue)
        {
            var topic = await dbContext.Set<Topic>()
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == lesson.TopicId.Value,
                    cancellationToken);

            if (topic is null)
            {
                AddError(
                    "Lesson.TopicNotFound",
                    "Topic không tồn tại.",
                    "topicId");
            }
            else if (topic.IsDeleted)
            {
                AddError(
                    "Lesson.TopicDeleted",
                    "Topic đã bị xóa.",
                    "topicId");
            }
            else if (topic.Status != ContentStatus.Published)
            {
                AddError(
                    "Lesson.TopicNotPublished",
                    "Topic phải được Published trước khi Lesson tiếp tục workflow.",
                    "topicId");
            }
        }

        if (lesson.CourseChapterId.HasValue)
        {
            var chapter = await dbContext.CourseChapters
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Include(x => x.Course)
                .FirstOrDefaultAsync(
                    x => x.Id == lesson.CourseChapterId.Value,
                    cancellationToken);

            if (chapter is null)
            {
                AddError(
                    "Lesson.ChapterNotFound",
                    "Chapter không tồn tại.",
                    "courseChapterId");
            }
            else
            {
                ValidateChapterAndCourse(
                    lesson,
                    chapter,
                    target,
                    AddError,
                    AddWarning);

                var duplicateOrder = await dbContext.Lessons
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.Id != lesson.Id &&
                            !x.IsDeleted &&
                            x.CourseChapterId == lesson.CourseChapterId &&
                            x.SortOrder == lesson.SortOrder,
                        cancellationToken);

                if (duplicateOrder)
                {
                    AddError(
                        "Lesson.ChapterSortOrderDuplicate",
                        "Trong cùng Chapter đã có Lesson sử dụng SortOrder này.",
                        "sortOrder");
                }
            }
        }

        // ========================================================
        // 3. Sections
        // ========================================================

        var sections = await dbContext.Set<LessonSection>()
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.LessonId == lesson.Id)
            .ToListAsync(cancellationToken);

        var activeSections = sections
            .Where(x => !x.IsDeleted)
            .ToArray();

        if (activeSections.Length == 0)
        {
            AddError(
                "Lesson.SectionRequired",
                "Lesson phải có ít nhất một Section.",
                "sections");
        }

        foreach (var duplicatedOrder in activeSections
                     .GroupBy(x => x.SortOrder)
                     .Where(group => group.Count() > 1)
                     .Select(group => group.Key))
        {
            AddError(
                "Lesson.SectionSortOrderDuplicate",
                $"Có nhiều Section cùng SortOrder {duplicatedOrder}.",
                "sections");
        }

        foreach (var section in activeSections)
        {
            if (section.SortOrder < 0)
            {
                AddError(
                    "Lesson.SectionSortOrderInvalid",
                    $"Section #{section.Id} có SortOrder không hợp lệ.",
                    "sections");
            }

            if (section.IsRequired &&
                string.IsNullOrWhiteSpace(section.ContentVi))
            {
                AddError(
                    "Lesson.SectionContentRequired",
                    $"Section bắt buộc '{section.TitleVi ?? section.SectionType.ToString()}' chưa có nội dung.",
                    "sections");
            }

            if (section.EstimatedSeconds.HasValue &&
                section.EstimatedSeconds.Value <= 0)
            {
                AddError(
                    "Lesson.SectionDurationInvalid",
                    $"Section #{section.Id} có thời lượng không hợp lệ.",
                    "sections");
            }
        }

        // ========================================================
        // 4. Vocabulary
        // ========================================================

        var lessonVocabulary = await dbContext.Set<LessonVocabulary>()
            .AsNoTracking()
            .Where(x => x.LessonId == lesson.Id)
            .ToListAsync(cancellationToken);

        foreach (var duplicatedOrder in lessonVocabulary
                     .GroupBy(x => x.SortOrder)
                     .Where(group => group.Count() > 1)
                     .Select(group => group.Key))
        {
            AddError(
                "Lesson.VocabularySortOrderDuplicate",
                $"Có nhiều từ vựng cùng SortOrder {duplicatedOrder}.",
                "vocabulary");
        }

        foreach (var duplicatedVocabulary in lessonVocabulary
                     .GroupBy(x => x.VocabularyId)
                     .Where(group => group.Count() > 1)
                     .Select(group => group.Key))
        {
            AddError(
                "Lesson.VocabularyDuplicate",
                $"Vocabulary #{duplicatedVocabulary} bị gắn trùng vào Lesson.",
                "vocabulary");
        }

        var vocabularyIds = lessonVocabulary
            .Select(x => x.VocabularyId)
            .Distinct()
            .ToArray();

        var vocabularies = vocabularyIds.Length == 0
            ? new Dictionary<long, Vocabulary>()
            : await dbContext.Set<Vocabulary>()
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(x => vocabularyIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

        foreach (var link in lessonVocabulary)
        {
            if (!vocabularies.TryGetValue(link.VocabularyId, out var vocabulary))
            {
                AddError(
                    "Lesson.VocabularyNotFound",
                    $"Vocabulary #{link.VocabularyId} không tồn tại.",
                    "vocabulary");
                continue;
            }

            if (vocabulary.IsDeleted)
            {
                AddError(
                    "Lesson.VocabularyDeleted",
                    $"Vocabulary '{vocabulary.Simplified}' đã bị xóa.",
                    "vocabulary");
                continue;
            }

            if (vocabulary.Status == ContentStatus.Archived)
            {
                AddError(
                    "Lesson.VocabularyArchived",
                    $"Vocabulary '{vocabulary.Simplified}' đã Archived.",
                    "vocabulary");
                continue;
            }

            if (vocabulary.Status != ContentStatus.Published)
            {
                if (target == LessonWorkflowValidationTarget.Publish)
                {
                    AddError(
                        "Lesson.VocabularyNotPublished",
                        $"Vocabulary '{vocabulary.Simplified}' phải Published trước khi Lesson được xuất bản.",
                        "vocabulary");
                }
                else
                {
                    AddWarning(
                        "Lesson.VocabularyNotPublished",
                        $"Vocabulary '{vocabulary.Simplified}' chưa Published.",
                        "vocabulary");
                }
            }
        }

        // ========================================================
        // 5. Assets
        // ========================================================

        var assets = await dbContext.Set<LessonAsset>()
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.LessonId == lesson.Id)
            .ToListAsync(cancellationToken);

        var activeAssets = assets
            .Where(x => !x.IsDeleted)
            .ToArray();

        foreach (var duplicatedOrder in activeAssets
                     .GroupBy(x => x.SortOrder)
                     .Where(group => group.Count() > 1)
                     .Select(group => group.Key))
        {
            AddError(
                "Lesson.AssetSortOrderDuplicate",
                $"Có nhiều Asset cùng SortOrder {duplicatedOrder}.",
                "assets");
        }

        foreach (var asset in activeAssets)
        {
            if (asset.SortOrder < 0)
            {
                AddError(
                    "Lesson.AssetSortOrderInvalid",
                    $"Asset #{asset.Id} có SortOrder không hợp lệ.",
                    "assets");
            }

            var hasUrl = !string.IsNullOrWhiteSpace(asset.Url);
            var hasAudioReference = asset.AudioAssetId.HasValue;

            if (asset.AssetType == LessonAssetType.Audio)
            {
                if (!hasUrl && !hasAudioReference)
                {
                    AddError(
                        "Lesson.AudioAssetSourceRequired",
                        $"Audio Asset #{asset.Id} phải có URL hoặc AudioAssetId.",
                        "assets");
                }
            }
            else if (!hasUrl)
            {
                AddError(
                    "Lesson.AssetUrlRequired",
                    $"{asset.AssetType} Asset #{asset.Id} phải có URL.",
                    "assets");
            }
        }

        var audioAssetIds = activeAssets
            .Where(x => x.AudioAssetId.HasValue)
            .Select(x => x.AudioAssetId!.Value)
            .Distinct()
            .ToArray();

        var audioAssets = audioAssetIds.Length == 0
            ? new Dictionary<long, AudioAsset>()
            : await dbContext.Set<AudioAsset>()
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(x => audioAssetIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

        foreach (var asset in activeAssets.Where(x => x.AudioAssetId.HasValue))
        {
            var audioId = asset.AudioAssetId!.Value;

            if (!audioAssets.TryGetValue(audioId, out var audio))
            {
                AddError(
                    "Lesson.AudioAssetNotFound",
                    $"AudioAsset #{audioId} không tồn tại.",
                    "assets");
                continue;
            }

            if (audio.IsDeleted)
            {
                AddError(
                    "Lesson.AudioAssetDeleted",
                    $"AudioAsset #{audioId} đã bị xóa.",
                    "assets");
                continue;
            }

            if (audio.Status != ContentStatus.Published)
            {
                if (target == LessonWorkflowValidationTarget.Publish)
                {
                    AddError(
                        "Lesson.AudioAssetNotPublished",
                        $"AudioAsset #{audioId} phải Published trước khi Lesson được xuất bản.",
                        "assets");
                }
                else
                {
                    AddWarning(
                        "Lesson.AudioAssetNotPublished",
                        $"AudioAsset #{audioId} chưa Published.",
                        "assets");
                }
            }
        }

        // ========================================================
        // 6. Prerequisites
        // ========================================================

        var prerequisites = await dbContext.Set<LessonPrerequisite>()
            .AsNoTracking()
            .Where(x => x.LessonId == lesson.Id)
            .ToListAsync(cancellationToken);

        foreach (var duplicatedPrerequisite in prerequisites
                     .GroupBy(x => x.RequiredLessonId)
                     .Where(group => group.Count() > 1)
                     .Select(group => group.Key))
        {
            AddError(
                "Lesson.PrerequisiteDuplicate",
                $"Lesson prerequisite #{duplicatedPrerequisite} bị gắn trùng.",
                "prerequisites");
        }

        if (prerequisites.Any(x => x.RequiredLessonId == lesson.Id))
        {
            AddError(
                "Lesson.PrerequisiteSelfReference",
                "Lesson không thể prerequisite chính nó.",
                "prerequisites");
        }

        var requiredLessonIds = prerequisites
            .Select(x => x.RequiredLessonId)
            .Distinct()
            .ToArray();

        var requiredLessons = requiredLessonIds.Length == 0
            ? new Dictionary<long, Domain.Entities.Lesson.Lesson>()
            : await dbContext.Lessons
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(x => requiredLessonIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

        foreach (var prerequisite in prerequisites)
        {
            if (!requiredLessons.TryGetValue(prerequisite.RequiredLessonId, out var requiredLesson))
            {
                AddError(
                    "Lesson.PrerequisiteNotFound",
                    $"Lesson prerequisite #{prerequisite.RequiredLessonId} không tồn tại.",
                    "prerequisites");
                continue;
            }

            if (requiredLesson.IsDeleted)
            {
                AddError(
                    "Lesson.PrerequisiteDeleted",
                    $"Lesson prerequisite '{requiredLesson.TitleVi}' đã bị xóa.",
                    "prerequisites");
                continue;
            }

            if (requiredLesson.Status != ContentStatus.Published)
            {
                AddError(
                    "Lesson.PrerequisiteNotPublished",
                    $"Lesson prerequisite '{requiredLesson.TitleVi}' phải ở trạng thái Published.",
                    "prerequisites");
            }
        }

        var prerequisiteEdges = await dbContext.Set<LessonPrerequisite>()
            .AsNoTracking()
            .Select(x => new PrerequisiteEdge(
                x.LessonId,
                x.RequiredLessonId))
            .ToListAsync(cancellationToken);

        if (HasPrerequisiteCycle(
                lesson.Id,
                prerequisiteEdges))
        {
            AddError(
                "Lesson.PrerequisiteCycle",
                "Chuỗi bài học tiên quyết đang tạo thành vòng lặp.",
                "prerequisites");
        }

        var isValid = issues.All(
            issue => issue.Severity != LessonValidationSeverity.Error);

        return Result.Success(
            new LessonValidationResultDto(
                isValid,
                issues));
    }

    private static void ValidateChapterAndCourse(
        Domain.Entities.Lesson.Lesson lesson,
        CourseChapter chapter,
        LessonWorkflowValidationTarget target,
        Action<string, string, string?> addError,
        Action<string, string, string?> addWarning)
    {
        if (chapter.IsDeleted)
        {
            addError(
                "Lesson.ChapterDeleted",
                "Chapter đã bị xóa.",
                "courseChapterId");
        }

        if (!chapter.IsActive)
        {
            addError(
                "Lesson.ChapterInactive",
                "Chapter đang bị vô hiệu.",
                "courseChapterId");
        }

        var course = chapter.Course;

        if (course is null)
        {
            addError(
                "Lesson.CourseNotFound",
                "Course của Chapter không tồn tại.",
                "courseChapterId");
            return;
        }

        if (course.IsDeleted)
        {
            addError(
                "Lesson.CourseDeleted",
                "Course đã bị xóa.",
                "courseChapterId");
        }

        if (!course.IsActive)
        {
            addError(
                "Lesson.CourseInactive",
                "Course đang bị vô hiệu.",
                "courseChapterId");
        }

        if (!course.HskLevelId.HasValue)
        {
            addWarning(
                "Lesson.CourseHskMissing",
                "Course chưa gắn HSK Level; Lesson không thể đối chiếu HSK với Course.",
                "hskLevelId");
        }
        else if (course.HskLevelId.Value != lesson.HskLevelId)
        {
            addError(
                "Lesson.CourseHskMismatch",
                "HSK của Lesson phải trùng với HSK của Course chứa Chapter.",
                "hskLevelId");
        }

        if (course.Status != ContentStatus.Published)
        {
            addWarning(
                "Lesson.CourseNotPublished",
                target == LessonWorkflowValidationTarget.Publish
                    ? "Course chứa Lesson chưa Published. Lesson vẫn có thể Publish độc lập nhưng sẽ chưa xuất hiện trong Course public."
                    : "Course chứa Lesson chưa Published.",
                "courseChapterId");
        }
    }

    private static bool HasPrerequisiteCycle(
        long rootLessonId,
        IReadOnlyList<PrerequisiteEdge> edges)
    {
        var graph = edges
            .GroupBy(x => x.LessonId)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(x => x.RequiredLessonId)
                    .Distinct()
                    .ToArray());

        if (!graph.TryGetValue(rootLessonId, out var directPrerequisites) ||
            directPrerequisites.Length == 0)
        {
            return false;
        }

        foreach (var prerequisiteId in directPrerequisites)
        {
            var stack = new Stack<long>();
            var visited = new HashSet<long>();
            stack.Push(prerequisiteId);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (current == rootLessonId)
                {
                    return true;
                }

                if (!visited.Add(current) ||
                    !graph.TryGetValue(current, out var next))
                {
                    continue;
                }

                foreach (var nextId in next)
                {
                    stack.Push(nextId);
                }
            }
        }

        return false;
    }

    private sealed record PrerequisiteEdge(
        long LessonId,
        long RequiredLessonId);
}
