namespace HanYu.Infrastructure.Options;

public sealed class LessonCacheOptions
{
    public const string SectionName = "LessonCache";

    public int ListMinutes { get; init; } = 10;

    public int DetailMinutes { get; init; } = 30;

    public int GenerationDays { get; init; } = 365;
}
