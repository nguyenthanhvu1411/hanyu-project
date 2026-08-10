namespace HanYu.Application.Features.Course.Admin.Prerequisites;

public sealed class UpdateCoursePrerequisiteRequest
{
    public long RequiredCourseId { get; init; }
    public bool IsRequired { get; init; }
    public int SortOrder { get; init; }
    public Guid ConcurrencyToken { get; init; }
}
