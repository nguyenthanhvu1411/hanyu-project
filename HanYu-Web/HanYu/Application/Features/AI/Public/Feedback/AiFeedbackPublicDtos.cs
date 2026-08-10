using HanYu.Domain.Enums;

namespace HanYu.Application.Features.AI.Public.Feedback;

public sealed record SubmitAiFeedbackRequest(
    Guid AiRequestPublicId,
    AiFeedbackRating Rating,
    string? Comment,
    string? IssueType);
