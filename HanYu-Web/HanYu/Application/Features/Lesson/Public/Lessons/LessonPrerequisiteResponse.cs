namespace HanYu.Application.Features.Lesson.Public.Lessons;

public sealed record LessonPrerequisiteResponse(
    Guid PublicId,
    string Slug,
    string TitleVi,
    bool IsCompleted);
