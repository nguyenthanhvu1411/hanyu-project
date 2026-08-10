namespace HanYu.Application.Features.Course.Admin;

public sealed class CourseEntityWorkflowRequest
{
    public Guid ConcurrencyToken { get; init; }
}
