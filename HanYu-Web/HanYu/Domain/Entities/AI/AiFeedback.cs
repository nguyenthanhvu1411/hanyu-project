using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.AI;

public class AiFeedback : BaseEntity
{
    public Guid UserId { get; private set; }

    public long AiRequestId { get; private set; }

    public AiFeedbackRating Rating { get; private set; }

    public string? Comment { get; private set; }

    public string? IssueType { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    protected AiFeedback()
    {
    }

    public AiFeedback(
        Guid userId,
        long aiRequestId,
        AiFeedbackRating rating,
        string? comment = null,
        string? issueType = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId không hợp lệ.", nameof(userId));

        if (aiRequestId <= 0)
            throw new ArgumentOutOfRangeException(nameof(aiRequestId));

        UserId = userId;
        AiRequestId = aiRequestId;
        Rating = rating;
        Comment = NormalizeNullable(comment);
        IssueType = NormalizeNullable(issueType);
    }

    private static string? NormalizeNullable(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
