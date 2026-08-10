namespace HanYu.Application.Features.Course.Admin;

public sealed class ScheduleCoursePublishRequest
{
    public DateTimeOffset PublishAt { get; init; }

    public Guid ConcurrencyToken { get; init; }
}
