using HanYu.Application.Interfaces.DateTime;

namespace HanYu.Infrastructure.Services;

public sealed class DateTimeProvider
    : IDateTimeProvider
{
    public DateTime UtcNow =>
        DateTime.UtcNow;
}