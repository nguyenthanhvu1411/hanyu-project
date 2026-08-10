using HanYu.Application.Common.Models;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Quiz.Admin.Quizzes;

public sealed record AdminQuizQuery : PaginationRequest
{
    public string? Q { get; init; }

    public long? LessonId { get; init; }

    public QuizType? QuizType { get; init; }

    public ContentStatus? Status { get; init; }

    public string? Sort { get; init; }
        = "-updatedAt";
}
