namespace HanYu.Application.Features.Course.Admin.Prerequisites;

public sealed class CreateCoursePrerequisiteRequest
{
    public long RequiredCourseId { get; init; }
    public bool IsRequired { get; init; } = true;
    public int SortOrder { get; init; }
}
