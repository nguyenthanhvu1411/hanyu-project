namespace HanYu.Application.Features.Quiz.Admin.QuestionBanks;

public sealed record AdminQuestionBankResponse(
    long Id,
    Guid PublicId,
    string Code,
    string NameVi,
    string? DescriptionVi,
    long? HskLevelId,
    bool IsActive,
    int QuestionCount);
