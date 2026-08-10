namespace HanYu.Application.Features.Quiz.Admin.QuestionBanks;

public sealed record CreateQuestionBankRequest(
    string Code,
    string NameVi,
    string? DescriptionVi,
    long? HskLevelId);
