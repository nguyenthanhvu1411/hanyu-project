using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Quiz;

public class QuizTag : AuditableEntity
{
    public string Slug { get; private set; }
        = string.Empty;

    public string Name { get; private set; }
        = string.Empty;

    public string? NameVi { get; private set; }

    public string? DescriptionVi { get; private set; }

    public bool IsActive { get; private set; }
        = true;

    public ICollection<QuizQuestionTag> QuestionTags { get; private set; }
        = new List<QuizQuestionTag>();

    protected QuizTag()
    {
    }

    public QuizTag(
        string slug,
        string name,
        string? nameVi = null)
    {
        Update(
            slug,
            name,
            nameVi,
            null);
    }

    public void Update(
        string slug,
        string name,
        string? nameVi,
        string? descriptionVi)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException(
                "Slug không được để trống.",
                nameof(slug));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Name không được để trống.",
                nameof(name));

        Slug = NormalizeSlug(slug);
        Name = name.Trim();
        NameVi = Normalize(nameVi);
        DescriptionVi = Normalize(descriptionVi);

        MarkUpdated();
    }

    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;

        MarkUpdated();
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;

        MarkUpdated();
    }

    private static string NormalizeSlug(
        string value)
        => string.Join(
            '-',
            value.Trim()
                .ToLowerInvariant()
                .Split(
                    [' ', '-'],
                    StringSplitOptions.RemoveEmptyEntries));

    private static string? Normalize(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
