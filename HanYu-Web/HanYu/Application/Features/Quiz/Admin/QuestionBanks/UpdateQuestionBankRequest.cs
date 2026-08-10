namespace HanYu.Application.Features.Quiz.Admin.QuestionBanks;

public sealed record UpdateQuestionBankRequest(
    string Code,
    string NameVi,
    string? DescriptionVi,
    long? HskLevelId);
