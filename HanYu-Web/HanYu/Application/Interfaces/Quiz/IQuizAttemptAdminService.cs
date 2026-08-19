using HanYu.Application.Common.Models;
using HanYu.Application.Features.Quiz.Admin.Attempts;

namespace HanYu.Application.Interfaces.Quiz;

public interface IQuizAttemptAdminService
{
    Task<Result<PagedResult<AdminQuizAttemptResponse>>> GetAttemptsAsync(
        AdminQuizAttemptQuery query,
        CancellationToken cancellationToken = default);

    Task<Result<AdminQuizAttemptDetailResponse>> GetAttemptAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result<AdminQuizAttemptStatisticsResponse>> GetStatisticsAsync(
        AdminQuizAttemptQuery query,
        CancellationToken cancellationToken = default);
}