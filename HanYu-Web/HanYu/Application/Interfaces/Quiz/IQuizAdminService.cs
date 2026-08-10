using HanYu.Application.Common.Models;
using HanYu.Application.Features.Quiz.Admin.MatchingPairs;
using HanYu.Application.Features.Quiz.Admin.Options;
using HanYu.Application.Features.Quiz.Admin.QuestionBanks;
using HanYu.Application.Features.Quiz.Admin.Questions;
using HanYu.Application.Features.Quiz.Admin.Quizzes;
using HanYu.Application.Features.Quiz.Admin.Tags;

namespace HanYu.Application.Interfaces.Quiz;

public interface IQuizAdminService
{
    Task<Result<PagedResult<AdminQuizResponse>>>
        GetQuizzesAsync(
            AdminQuizQuery query,
            CancellationToken cancellationToken = default);

    Task<Result<AdminQuizResponse>>
        GetQuizAsync(
            long id,
            CancellationToken cancellationToken = default);

    Task<Result<AdminQuizResponse>>
        CreateQuizAsync(
            CreateQuizRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminQuizResponse>>
        UpdateQuizAsync(
            long id,
            UpdateQuizRequest request,
            CancellationToken cancellationToken = default);

    Task<Result> SubmitForReviewAsync(long id, CancellationToken ct = default);
    Task<Result> ApproveAsync(long id, CancellationToken ct = default);
    Task<Result> PublishAsync(long id, CancellationToken ct = default);
    Task<Result> ArchiveAsync(long id, CancellationToken ct = default);
    Task<Result> RestoreAsync(long id, CancellationToken ct = default);
    Task<Result> DeleteQuizAsync(long id, CancellationToken ct = default);

    Task<Result<IReadOnlyCollection<AdminQuizQuestionResponse>>>
        GetQuestionsAsync(long quizId, CancellationToken ct = default);

    Task<Result<AdminQuizQuestionResponse>>
        CreateQuestionAsync(
            long quizId,
            CreateQuizQuestionRequest request,
            CancellationToken ct = default);

    Task<Result<AdminQuizQuestionResponse>>
        UpdateQuestionAsync(
            long quizId,
            long questionId,
            UpdateQuizQuestionRequest request,
            CancellationToken ct = default);

    Task<Result> SubmitQuestionForReviewAsync(long quizId, long questionId, CancellationToken ct = default);
    Task<Result> ApproveQuestionAsync(long quizId, long questionId, CancellationToken ct = default);
    Task<Result> PublishQuestionAsync(long quizId, long questionId, CancellationToken ct = default);
    Task<Result> ArchiveQuestionAsync(long quizId, long questionId, CancellationToken ct = default);
    Task<Result> RestoreQuestionAsync(long quizId, long questionId, CancellationToken ct = default);
    Task<Result> DeleteQuestionAsync(long quizId, long questionId, CancellationToken ct = default);

    Task<Result<IReadOnlyCollection<AdminQuizQuestionOptionResponse>>>
        GetOptionsAsync(long questionId, CancellationToken ct = default);

    Task<Result<AdminQuizQuestionOptionResponse>>
        CreateOptionAsync(
            long questionId,
            CreateQuizQuestionOptionRequest request,
            CancellationToken ct = default);

    Task<Result<AdminQuizQuestionOptionResponse>>
        UpdateOptionAsync(
            long questionId,
            long optionId,
            UpdateQuizQuestionOptionRequest request,
            CancellationToken ct = default);

    Task<Result> DeleteOptionAsync(long questionId, long optionId, CancellationToken ct = default);

    Task<Result<IReadOnlyCollection<AdminQuizMatchingPairResponse>>>
        GetMatchingPairsAsync(long questionId, CancellationToken ct = default);

    Task<Result<AdminQuizMatchingPairResponse>>
        CreateMatchingPairAsync(
            long questionId,
            CreateQuizMatchingPairRequest request,
            CancellationToken ct = default);

    Task<Result<AdminQuizMatchingPairResponse>>
        UpdateMatchingPairAsync(
            long questionId,
            long pairId,
            UpdateQuizMatchingPairRequest request,
            CancellationToken ct = default);

    Task<Result> DeleteMatchingPairAsync(long questionId, long pairId, CancellationToken ct = default);

    Task<Result<IReadOnlyCollection<AdminQuizTagResponse>>>
        GetTagsAsync(CancellationToken ct = default);

    Task<Result<AdminQuizTagResponse>>
        CreateTagAsync(CreateQuizTagRequest request, CancellationToken ct = default);

    Task<Result<AdminQuizTagResponse>>
        UpdateTagAsync(long id, UpdateQuizTagRequest request, CancellationToken ct = default);

    Task<Result> ActivateTagAsync(long id, CancellationToken ct = default);
    Task<Result> DeactivateTagAsync(long id, CancellationToken ct = default);
    Task<Result> DeleteTagAsync(long id, CancellationToken ct = default);

    Task<Result> AttachTagAsync(long questionId, long tagId, CancellationToken ct = default);
    Task<Result> DetachTagAsync(long questionId, long tagId, CancellationToken ct = default);

    Task<Result<IReadOnlyCollection<AdminQuestionBankResponse>>>
        GetQuestionBanksAsync(CancellationToken ct = default);

    Task<Result<AdminQuestionBankResponse>>
        CreateQuestionBankAsync(CreateQuestionBankRequest request, CancellationToken ct = default);

    Task<Result<AdminQuestionBankResponse>>
        UpdateQuestionBankAsync(long id, UpdateQuestionBankRequest request, CancellationToken ct = default);

    Task<Result> AddQuestionToBankAsync(
        long bankId,
        AddQuestionToBankRequest request,
        CancellationToken ct = default);

    Task<Result> RemoveQuestionFromBankAsync(
        long bankId,
        long questionId,
        CancellationToken ct = default);

    Task<Result> ActivateQuestionBankAsync(long id, CancellationToken ct = default);
    Task<Result> DeactivateQuestionBankAsync(long id, CancellationToken ct = default);
}
