namespace HanYu.Application.Features.Lesson.Admin.Prerequisites;

public sealed record AdminLessonPrerequisiteResponse(
    long RequiredLessonId,
    Guid RequiredLessonPublicId,
    string Slug,
    string TitleVi);
