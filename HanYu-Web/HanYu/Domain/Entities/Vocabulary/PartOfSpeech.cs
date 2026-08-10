using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Vocabulary;

public class PartOfSpeech : AuditableEntity
{
    public string Code { get; private set; }
        = string.Empty;

    public string NameVi { get; private set; }
        = string.Empty;

    public string? NameEn { get; private set; }

    protected PartOfSpeech()
    {
    }

    public PartOfSpeech(
        string code,
        string nameVi,
        string? nameEn = null)
    {
        Update(
            code,
            nameVi,
            nameEn);
    }

    public void Update(
        string code,
        string nameVi,
        string? nameEn)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Code không được để trống.",
                nameof(code));
        }

        if (string.IsNullOrWhiteSpace(nameVi))
        {
            throw new ArgumentException(
                "NameVi không được để trống.",
                nameof(nameVi));
        }

        Code =
            code.Trim()
                .ToLowerInvariant();

        NameVi =
            nameVi.Trim();

        NameEn =
            string.IsNullOrWhiteSpace(nameEn)
                ? null
                : nameEn.Trim();

        MarkUpdated();
    }
}
