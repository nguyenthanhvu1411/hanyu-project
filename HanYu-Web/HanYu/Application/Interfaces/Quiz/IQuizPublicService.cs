using HanYu.Application.Common.Models;
using HanYu.Application.Features.Quiz.Public.Answers;
using HanYu.Application.Features.Quiz.Public.Attempts;
using HanYu.Application.Features.Quiz.Public.Quizzes;
using HanYu.Application.Features.Quiz.Public.Results;

namespace HanYu.Application.Interfaces.Quiz;

public interface IQuizPublicService
{
    Task<Result<PagedResult<QuizListItemResponse>>>
        GetQuizzesAsync(
            Guid? lessonPublicId,
            CancellationToken cancellationToken = default);

    Task<Result<QuizDetailResponse>>
        GetQuizAsync(
            Guid quizPublicId,
            CancellationToken cancellationToken = default);

    Task<Result<QuizAttemptResponse>>
        StartAttemptAsync(
            Guid userId,
            Guid quizPublicId,
            StartQuizAttemptRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<QuizAttemptResponse>>
        GetAttemptAsync(
            Guid userId,
            Guid attemptPublicId,
            CancellationToken cancellationToken = default);

    Task<Result<QuizAnswerResultResponse>>
        SubmitAnswerAsync(
            Guid userId,
            Guid attemptPublicId,
            Guid questionPublicId,
            SubmitQuizAnswerRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<QuizResultResponse>>
        SubmitAttemptAsync(
            Guid userId,
            Guid attemptPublicId,
            CancellationToken cancellationToken = default);

    Task<Result<QuizResultResponse>>
        GetResultAsync(
            Guid userId,
            Guid attemptPublicId,
            CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<QuizResultResponse>>>
        GetMyHistoryAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
}
