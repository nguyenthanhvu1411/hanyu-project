namespace HanYu.Application.Features.Quiz.Admin.QuestionBanks;

public sealed record AddQuestionToBankRequest(
    long QuestionId,
    int SortOrder);
