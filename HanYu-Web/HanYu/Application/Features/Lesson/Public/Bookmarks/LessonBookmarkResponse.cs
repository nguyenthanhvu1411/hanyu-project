namespace HanYu.Application.Features.Lesson.Public.Bookmarks;

public sealed record LessonBookmarkResponse(
    Guid LessonPublicId,
    string Slug,
    string TitleVi,
    string? CoverImageUrl,
    DateTimeOffset CreatedAt);
