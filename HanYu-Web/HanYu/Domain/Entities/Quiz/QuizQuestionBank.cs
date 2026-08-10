using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Quiz;

public class QuizQuestionBank : AuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string NameVi { get; private set; } = string.Empty;
    public string? DescriptionVi { get; private set; }
    public long? HskLevelId { get; private set; }
    public bool IsActive { get; private set; } = true;
    
    public ICollection<QuizQuestionBankItem> Items { get; private set; } = new List<QuizQuestionBankItem>();

    protected QuizQuestionBank() { }

    public QuizQuestionBank(
        string code,
        string nameVi,
        long? hskLevelId = null,
        string? descriptionVi = null)
    {
        Update(
            code,
            nameVi,
            descriptionVi,
            hskLevelId);
    }

    public void Update(
        string code,
        string nameVi,
        string? descriptionVi,
        long? hskLevelId)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(nameof(code));

        if (string.IsNullOrWhiteSpace(nameVi))
            throw new ArgumentException(nameof(nameVi));

        if (hskLevelId.HasValue &&
            hskLevelId.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(hskLevelId));
        }

        Code =
            code.Trim()
                .ToLowerInvariant();

        NameVi =
            nameVi.Trim();

        DescriptionVi =
            string.IsNullOrWhiteSpace(descriptionVi)
                ? null
                : descriptionVi.Trim();

        HskLevelId =
            hskLevelId;

        MarkUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }
}
