using System.Text.Json;
using HanYu.Application.Features.Quiz.Public.Attempts;
using HanYu.Application.Features.Quiz.Public.Questions;
using HanYu.Application.Features.Quiz.Public.Quizzes;
using HanYu.Application.Features.Quiz.Public.Results;
using HanYu.Domain.Entities.Quiz;

namespace HanYu.Application.Features.Quiz.Mapping;

public static class QuizMapper
{
    public static QuizListItemResponse ToListItemResponse(this Domain.Entities.Quiz.Quiz entity, int questionCount)
    {
        return new QuizListItemResponse(
            entity.PublicId,
            entity.TitleVi,
            entity.DescriptionVi,
            entity.QuizType,
            entity.PassingScore,
            entity.TimeLimitSeconds,
            entity.MaxAttempts,
            questionCount);
    }

    public static QuizDetailResponse ToDetailResponse(this Domain.Entities.Quiz.Quiz entity, int questionCount)
    {
        return new QuizDetailResponse(
            entity.PublicId,
            entity.TitleVi,
            entity.DescriptionVi,
            entity.QuizType,
            entity.PassingScore,
            entity.TimeLimitSeconds,
            entity.MaxAttempts,
            entity.AllowRetry,
            entity.FeedbackMode,
            questionCount);
    }

    public static QuizAttemptResponse ToAttemptResponse(this QuizAttempt entity)
    {
        var questions = entity.Questions
            .OrderBy(x => x.SortOrder)
            .Select(x => ParseSnapshot(x.QuestionSnapshotJson))
            .Where(x => x != null)
            .Select(x => x!)
            .ToList();

        var currentQuestionIndex = entity.Answers.Count; // Assuming simple linear flow

        return new QuizAttemptResponse(
            entity.PublicId,
            entity.Quiz.PublicId,
            entity.AttemptNumber,
            entity.Status,
            entity.StartedAt,
            entity.ExpiresAt,
            currentQuestionIndex,
            entity.Questions.Count,
            questions);
    }

    public static QuizAttemptQuestionResponse? ParseSnapshot(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<QuizAttemptQuestionResponse>(
                json, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }

    public static QuizResultResponse ToResultResponse(this QuizAttempt entity)
    {
        var questionResults = entity.Answers
            .Select(a => new QuizQuestionResultResponse(
                a.Question.PublicId,
                a.Question.Prompt,
                a.IsCorrect,
                a.EarnedPoints ?? 0m,
                a.Question.Points,
                a.Attempt.Quiz.ShowCorrectAnswer ? a.Question.CorrectAnswerText : null,
                a.Attempt.Quiz.ShowExplanation ? a.Question.ExplanationVi : null))
            .ToList();

        return new QuizResultResponse(
            entity.PublicId,
            entity.Quiz.PublicId,
            entity.Score ?? 0m,
            entity.MaxScore ?? 0m,
            entity.Percentage ?? 0m,
            entity.IsPassed ?? false,
            entity.CorrectAnswers,
            entity.WrongAnswers,
            entity.UnansweredQuestions,
            entity.DurationSeconds ?? 0,
            entity.SubmittedAt ?? DateTimeOffset.UtcNow,
            questionResults);
    }
}
