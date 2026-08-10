namespace HanYu.Infrastructure.Options;

public sealed class VocabularyCacheOptions
{
    public const string SectionName =
        "VocabularyCache";

    public int DetailMinutes { get; init; } = 30;

    public int ListMinutes { get; init; } = 5;

    public int TaxonomyMinutes { get; init; } = 60;

    public int GenerationDays { get; init; } = 365;
}
