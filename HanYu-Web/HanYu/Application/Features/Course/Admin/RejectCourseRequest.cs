namespace HanYu.Application.Features.Course.Admin;

public sealed class RejectCourseRequest
{
    public string Reason { get; init; }
        = string.Empty;

    public Guid ConcurrencyToken { get; init; }
}
