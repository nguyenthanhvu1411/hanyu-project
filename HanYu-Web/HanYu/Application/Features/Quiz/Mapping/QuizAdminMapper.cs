using HanYu.Application.Features.Quiz.Admin.MatchingPairs;
using HanYu.Application.Features.Quiz.Admin.Options;
using HanYu.Application.Features.Quiz.Admin.QuestionBanks;
using HanYu.Application.Features.Quiz.Admin.Questions;
using HanYu.Application.Features.Quiz.Admin.Quizzes;
using HanYu.Application.Features.Quiz.Admin.Tags;
using HanYu.Domain.Entities.Quiz;

namespace HanYu.Application.Features.Quiz.Mapping;

public static class QuizAdminMapper
{
    public static AdminQuizResponse ToAdminResponse(this Domain.Entities.Quiz.Quiz entity)
    {
        return new AdminQuizResponse(
            entity.Id,
            entity.PublicId,
            entity.LessonId,
            entity.Lesson?.PublicId,
            entity.Lesson?.TitleVi,
            entity.TitleVi,
            entity.DescriptionVi,
            entity.QuizType,
            entity.PassingScore,
            entity.TimeLimitSeconds,
            entity.MaxAttempts,
            entity.ShuffleMode,
            entity.FeedbackMode,
            entity.AllowRetry,
            entity.ShowCorrectAnswer,
            entity.ShowExplanation,
            entity.Status,
            entity.Version,
            entity.PublishedAt,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

    public static AdminQuizQuestionResponse ToAdminResponse(this QuizQuestion entity)
    {
        return new AdminQuizQuestionResponse(
            entity.Id,
            entity.PublicId,
            entity.QuizId,
            entity.VocabularyId,
            entity.Vocabulary?.PublicId,
            entity.QuestionType,
            entity.Prompt,
            entity.PromptPinyin,
            entity.CorrectAnswerText,
            entity.ExplanationVi,
            entity.HintVi,
            entity.Points,
            entity.SortOrder,
            entity.TimeLimitSeconds,
            entity.IsRequired,
            entity.Status,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

    public static AdminQuizQuestionOptionResponse ToAdminResponse(this QuizQuestionOption entity)
    {
        return new AdminQuizQuestionOptionResponse(
            entity.Id,
            entity.PublicId,
            entity.QuestionId,
            entity.OptionText,
            entity.OptionPinyin,
            entity.IsCorrect,
            entity.SortOrder,
            entity.ExplanationVi);
    }

    public static AdminQuizMatchingPairResponse ToAdminResponse(this QuizMatchingPair entity)
    {
        return new AdminQuizMatchingPairResponse(
            entity.Id,
            entity.PublicId,
            entity.QuestionId,
            entity.LeftText,
            entity.RightText,
            entity.LeftPinyin,
            entity.RightPinyin,
            entity.SortOrder);
    }

    public static AdminQuizTagResponse ToAdminResponse(this QuizTag entity)
    {
        return new AdminQuizTagResponse(
            entity.Id,
            entity.PublicId,
            entity.Slug,
            entity.Name,
            entity.NameVi,
            entity.DescriptionVi,
            entity.IsActive);
    }

    public static AdminQuestionBankResponse ToAdminResponse(this QuizQuestionBank entity)
    {
        return new AdminQuestionBankResponse(
            entity.Id,
            entity.PublicId,
            entity.Code,
            entity.NameVi,
            entity.DescriptionVi,
            entity.HskLevelId,
            entity.IsActive,
            entity.Items?.Count ?? 0);
    }
}
