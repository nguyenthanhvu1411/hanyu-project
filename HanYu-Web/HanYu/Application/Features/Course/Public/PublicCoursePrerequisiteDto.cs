namespace HanYu.Application.Features.Course.Public;

public sealed record PublicCoursePrerequisiteDto(
    Guid PublicId,
    string Slug,
    string TitleVi,
    bool IsRequired);
