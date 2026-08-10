using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Gamification;

public class XpTransaction : BaseEntity
{
    public Guid UserId { get; private set; }

    public int Amount { get; private set; }

    public string Reason { get; private set; }
        = string.Empty;

    public string? SourceType { get; private set; }

    public string? SourceId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    protected XpTransaction()
    {
    }

    public XpTransaction(
        Guid userId,
        int amount,
        string reason,
        string? sourceType = null,
        string? sourceId = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        if (amount == 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "XP transaction không được bằng 0.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException(
                "Reason không được để trống.",
                nameof(reason));

        UserId = userId;
        Amount = amount;
        Reason = reason.Trim();

        SourceType = Normalize(sourceType);
        SourceId = Normalize(sourceId);
    }

    public bool IsCredit => Amount > 0;

    public bool IsDebit => Amount < 0;

    private static string? Normalize(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}