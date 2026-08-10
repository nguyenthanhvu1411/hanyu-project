namespace HanYu.Application.Features.Course.Admin;

public sealed class CourseWorkflowRequest
{
    public Guid ConcurrencyToken { get; init; }
}
