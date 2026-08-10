using System.Linq.Expressions;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Quiz.Admin.MatchingPairs;
using HanYu.Application.Features.Quiz.Admin.Options;
using HanYu.Application.Features.Quiz.Admin.QuestionBanks;
using HanYu.Application.Features.Quiz.Admin.Questions;
using HanYu.Application.Features.Quiz.Admin.Quizzes;
using HanYu.Application.Features.Quiz.Admin.Tags;
using HanYu.Application.Features.Quiz.Mapping;
using HanYu.Application.Interfaces.Quiz;
using HanYu.Domain.Entities.Lesson;
using HanYu.Domain.Entities.Quiz;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Quiz;

public sealed class QuizAdminService : IQuizAdminService
{
    private readonly HanYuDbContext _db;

    public QuizAdminService(HanYuDbContext db)
    {
        _db = db;
    }

    #region Quiz
    public async Task<Result<PagedResult<AdminQuizResponse>>> GetQuizzesAsync(
        AdminQuizQuery query,
        CancellationToken cancellationToken = default)
    {
        var q = _db.Set<Domain.Entities.Quiz.Quiz>()
            .AsNoTracking()
            .Include(x => x.Lesson)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Q))
        {
            var search = query.Q.Trim().ToLowerInvariant();
            q = q.Where(x => x.TitleVi.ToLower().Contains(search) || 
                             (x.DescriptionVi != null && x.DescriptionVi.ToLower().Contains(search)));
        }

        if (query.LessonId.HasValue)
        {
            q = q.Where(x => x.LessonId == query.LessonId.Value);
        }

        if (query.QuizType.HasValue)
        {
            q = q.Where(x => x.QuizType == query.QuizType.Value);
        }

        if (query.Status.HasValue)
        {
            q = q.Where(x => x.Status == query.Status.Value);
        }

        // Default sort handling
        q = ApplyQuizSort(q, query.Sort);

        var total = await q.CountAsync(cancellationToken);

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToArrayAsync(cancellationToken);

        var dtos = items.Select(x => x.ToAdminResponse()).ToArray();

        return Result.Success(new PagedResult<AdminQuizResponse>(dtos, total, query.Page, query.PageSize));
    }

    private static IQueryable<Domain.Entities.Quiz.Quiz> ApplyQuizSort(IQueryable<Domain.Entities.Quiz.Quiz> query, string? sort)
    {
        return sort switch
        {
            "updatedAt" => query.OrderBy(x => x.UpdatedAt),
            "-updatedAt" => query.OrderByDescending(x => x.UpdatedAt),
            "createdAt" => query.OrderBy(x => x.CreatedAt),
            "-createdAt" => query.OrderByDescending(x => x.CreatedAt),
            "title" => query.OrderBy(x => x.TitleVi),
            "-title" => query.OrderByDescending(x => x.TitleVi),
            _ => query.OrderByDescending(x => x.UpdatedAt)
        };
    }

    public async Task<Result<AdminQuizResponse>> GetQuizAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var quiz = await _db.Set<Domain.Entities.Quiz.Quiz>()
            .AsNoTracking()
            .Include(x => x.Lesson)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (quiz is null)
        {
            return Result.Failure<AdminQuizResponse>(
                Error.NotFound("Quiz.NotFound", "Không tìm thấy Quiz."));
        }

        return Result.Success(quiz.ToAdminResponse());
    }

    public async Task<Result<AdminQuizResponse>> CreateQuizAsync(
        CreateQuizRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.LessonId.HasValue)
        {
            var exists = await _db.Set<HanYu.Domain.Entities.Lesson.Lesson>()
                .AnyAsync(x => x.Id == request.LessonId.Value, cancellationToken);

            if (!exists)
            {
                return Result.Failure<AdminQuizResponse>(
                    Error.Validation("Quiz.InvalidLesson", "Lesson không tồn tại."));
            }
        }

        var entity = new Domain.Entities.Quiz.Quiz(
            request.TitleVi,
            request.QuizType,
            request.PassingScore);

        entity.UpdateCore(
            request.TitleVi,
            request.DescriptionVi,
            request.QuizType,
            request.PassingScore,
            request.TimeLimitSeconds,
            request.MaxAttempts);

        entity.AttachLesson(request.LessonId);

        entity.ConfigureBehavior(
            request.ShuffleMode,
            request.FeedbackMode,
            request.AllowRetry,
            request.ShowCorrectAnswer,
            request.ShowExplanation);

        _db.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return await GetQuizAsync(entity.Id, cancellationToken);
    }

    public async Task<Result<AdminQuizResponse>> UpdateQuizAsync(
        long id,
        UpdateQuizRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Domain.Entities.Quiz.Quiz>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
        {
            return Result.Failure<AdminQuizResponse>(
                Error.NotFound("Quiz.NotFound", "Không tìm thấy Quiz."));
        }

        if (entity.Version != request.Version)
        {
            return Result.Failure<AdminQuizResponse>(
                Error.Conflict("Quiz.VersionConflict", "Quiz đã được cập nhật bởi người khác."));
        }

        if (request.LessonId.HasValue)
        {
            var exists = await _db.Set<HanYu.Domain.Entities.Lesson.Lesson>()
                .AnyAsync(x => x.Id == request.LessonId.Value, cancellationToken);

            if (!exists)
            {
                return Result.Failure<AdminQuizResponse>(
                    Error.Validation("Quiz.InvalidLesson", "Lesson không tồn tại."));
            }
        }

        entity.UpdateCore(
            request.TitleVi,
            request.DescriptionVi,
            request.QuizType,
            request.PassingScore,
            request.TimeLimitSeconds,
            request.MaxAttempts);

        entity.AttachLesson(request.LessonId);

        entity.ConfigureBehavior(
            request.ShuffleMode,
            request.FeedbackMode,
            request.AllowRetry,
            request.ShowCorrectAnswer,
            request.ShowExplanation);

        await _db.SaveChangesAsync(cancellationToken);

        return await GetQuizAsync(entity.Id, cancellationToken);
    }

    public async Task<Result> SubmitForReviewAsync(long id, CancellationToken ct = default)
    {
        var entity = await _db.Set<Domain.Entities.Quiz.Quiz>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return Result.Failure(Error.NotFound("Quiz.NotFound", "Không tìm thấy Quiz."));

        entity.SubmitForReview();
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> ApproveAsync(long id, CancellationToken ct = default)
    {
        var entity = await _db.Set<Domain.Entities.Quiz.Quiz>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return Result.Failure(Error.NotFound("Quiz.NotFound", "Không tìm thấy Quiz."));

        entity.Approve();
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> PublishAsync(long id, CancellationToken ct = default)
    {
        var entity = await _db.Set<Domain.Entities.Quiz.Quiz>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return Result.Failure(Error.NotFound("Quiz.NotFound", "Không tìm thấy Quiz."));

        var validationResult = await ValidateQuizForPublishAsync(id, ct);
        if (!validationResult.IsSuccess) return validationResult;

        entity.Publish();
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    private async Task<Result> ValidateQuizForPublishAsync(long quizId, CancellationToken cancellationToken)
    {
        var questions = await _db.Set<QuizQuestion>()
            .AsNoTracking()
            .Include(x => x.Options)
            .Include(x => x.MatchingPairs)
            .Where(x => x.QuizId == quizId)
            .ToArrayAsync(cancellationToken);

        if (questions.Length == 0)
        {
            return Result.Failure(
                Error.Conflict("Quiz.NoQuestions", "Quiz phải có ít nhất một câu hỏi."));
        }

        if (questions.Any(x => x.Status != ContentStatus.Published))
        {
            return Result.Failure(
                Error.Conflict("Quiz.UnpublishedQuestions", "Tất cả câu hỏi phải Published trước khi publish Quiz."));
        }

        foreach (var question in questions)
        {
            switch (question.QuestionType)
            {
                case QuizQuestionType.MeaningChoice:
                case QuizQuestionType.PinyinChoice:
                case QuizQuestionType.HanziChoice:
                case QuizQuestionType.MultipleChoice:
                {
                    if (question.Options.Count < 2)
                    {
                        return Result.Failure(
                            Error.Conflict("QuizQuestion.NotEnoughOptions", $"Question {question.Id} phải có ít nhất 2 options."));
                    }

                    if (!question.Options.Any(x => x.IsCorrect))
                    {
                        return Result.Failure(
                            Error.Conflict("QuizQuestion.NoCorrectOption", $"Question {question.Id} chưa có đáp án đúng."));
                    }
                    break;
                }
                case QuizQuestionType.Matching:
                {
                    if (question.MatchingPairs.Count < 2)
                    {
                        return Result.Failure(
                            Error.Conflict("QuizQuestion.NotEnoughPairs", $"Question {question.Id} phải có ít nhất 2 matching pairs."));
                    }
                    break;
                }
                case QuizQuestionType.FillBlank:
                case QuizQuestionType.TrueFalse:
                case QuizQuestionType.SentenceOrder:
                {
                    if (string.IsNullOrWhiteSpace(question.CorrectAnswerText))
                    {
                        return Result.Failure(
                            Error.Conflict("QuizQuestion.NoCorrectAnswer", $"Question {question.Id} chưa có CorrectAnswerText."));
                    }
                    break;
                }
            }
        }

        return Result.Success();
    }

    public async Task<Result> ArchiveAsync(long id, CancellationToken ct = default)
    {
        var entity = await _db.Set<Domain.Entities.Quiz.Quiz>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return Result.Failure(Error.NotFound("Quiz.NotFound", "Không tìm thấy Quiz."));

        entity.Archive();
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> RestoreAsync(long id, CancellationToken ct = default)
    {
        var entity = await _db.Set<Domain.Entities.Quiz.Quiz>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return Result.Failure(Error.NotFound("Quiz.NotFound", "Không tìm thấy Quiz."));

        entity.RestoreToDraft();
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeleteQuizAsync(long id, CancellationToken ct = default)
    {
        var quiz = await _db.Set<Domain.Entities.Quiz.Quiz>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (quiz is null)
        {
            return Result.Failure(Error.NotFound("Quiz.NotFound", "Không tìm thấy Quiz."));
        }

        if (quiz.Status != ContentStatus.Draft && quiz.Status != ContentStatus.Archived)
        {
            return Result.Failure(
                Error.Conflict("Quiz.DeleteInvalidStatus", "Chỉ Quiz Draft hoặc Archived mới được xóa."));
        }

        var hasAttempts = await _db.Set<QuizAttempt>().AnyAsync(x => x.QuizId == id, ct);
        if (hasAttempts)
        {
            return Result.Failure(
                Error.Conflict("Quiz.HasAttempts", "Quiz đã có lịch sử làm bài. Hãy Archive."));
        }

        _db.Remove(quiz);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
    #endregion

    #region Questions
    public async Task<Result<IReadOnlyCollection<AdminQuizQuestionResponse>>> GetQuestionsAsync(long quizId, CancellationToken ct = default)
    {
        var items = await _db.Set<QuizQuestion>()
            .AsNoTracking()
            .Include(x => x.Vocabulary)
            .Where(x => x.QuizId == quizId)
            .OrderBy(x => x.SortOrder)
            .ToArrayAsync(ct);

        var dtos = items.Select(x => x.ToAdminResponse()).ToArray();
        return Result.Success((IReadOnlyCollection<AdminQuizQuestionResponse>)dtos);
    }

    public async Task<Result<AdminQuizQuestionResponse>> CreateQuestionAsync(long quizId, CreateQuizQuestionRequest request, CancellationToken ct = default)
    {
        var quiz = await _db.Set<Domain.Entities.Quiz.Quiz>().FirstOrDefaultAsync(x => x.Id == quizId, ct);
        if (quiz is null) return Result.Failure<AdminQuizQuestionResponse>(Error.NotFound("Quiz.NotFound", "Không tìm thấy Quiz."));

        var entity = new QuizQuestion(
            quizId,
            request.QuestionType,
            request.Prompt,
            request.Points,
            request.SortOrder);

        entity.ChangeQuestionType(request.QuestionType);
        entity.UpdateConfiguration(
            request.Prompt,
            request.PromptPinyin,
            request.CorrectAnswerText,
            request.ExplanationVi,
            request.HintVi,
            request.Points,
            request.SortOrder,
            request.TimeLimitSeconds,
            request.IsRequired);

        entity.AttachVocabulary(request.VocabularyId);

        _db.Add(entity);
        await _db.SaveChangesAsync(ct);

        return await GetQuestionResponseAsync(entity.Id, ct);
    }

    public async Task<Result<AdminQuizQuestionResponse>> UpdateQuestionAsync(long quizId, long questionId, UpdateQuizQuestionRequest request, CancellationToken ct = default)
    {
        var entity = await _db.Set<QuizQuestion>().FirstOrDefaultAsync(x => x.Id == questionId && x.QuizId == quizId, ct);
        if (entity is null) return Result.Failure<AdminQuizQuestionResponse>(Error.NotFound("Question.NotFound", "Không tìm thấy Question."));

        entity.ChangeQuestionType(request.QuestionType);
        entity.UpdateConfiguration(
            request.Prompt,
            request.PromptPinyin,
            request.CorrectAnswerText,
            request.ExplanationVi,
            request.HintVi,
            request.Points,
            request.SortOrder,
            request.TimeLimitSeconds,
            request.IsRequired);

        entity.AttachVocabulary(request.VocabularyId);

        await _db.SaveChangesAsync(ct);

        return await GetQuestionResponseAsync(entity.Id, ct);
    }

    private async Task<Result<AdminQuizQuestionResponse>> GetQuestionResponseAsync(long questionId, CancellationToken ct)
    {
        var entity = await _db.Set<QuizQuestion>()
            .AsNoTracking()
            .Include(x => x.Vocabulary)
            .FirstOrDefaultAsync(x => x.Id == questionId, ct);
            
        return Result.Success(entity!.ToAdminResponse());
    }

    public async Task<Result> SubmitQuestionForReviewAsync(long quizId, long questionId, CancellationToken ct = default)
    {
        var entity = await _db.Set<QuizQuestion>().FirstOrDefaultAsync(x => x.Id == questionId && x.QuizId == quizId, ct);
        if (entity is null) return Result.Failure(Error.NotFound("Question.NotFound", "Không tìm thấy Question."));

        entity.SubmitForReview();
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> ApproveQuestionAsync(long quizId, long questionId, CancellationToken ct = default)
    {
        var entity = await _db.Set<QuizQuestion>().FirstOrDefaultAsync(x => x.Id == questionId && x.QuizId == quizId, ct);
        if (entity is null) return Result.Failure(Error.NotFound("Question.NotFound", "Không tìm thấy Question."));

        entity.Approve();
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> PublishQuestionAsync(long quizId, long questionId, CancellationToken ct = default)
    {
        var entity = await _db.Set<QuizQuestion>().FirstOrDefaultAsync(x => x.Id == questionId && x.QuizId == quizId, ct);
        if (entity is null) return Result.Failure(Error.NotFound("Question.NotFound", "Không tìm thấy Question."));

        entity.Publish();
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> ArchiveQuestionAsync(long quizId, long questionId, CancellationToken ct = default)
    {
        var entity = await _db.Set<QuizQuestion>().FirstOrDefaultAsync(x => x.Id == questionId && x.QuizId == quizId, ct);
        if (entity is null) return Result.Failure(Error.NotFound("Question.NotFound", "Không tìm thấy Question."));

        entity.Archive();
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> RestoreQuestionAsync(long quizId, long questionId, CancellationToken ct = default)
    {
        var entity = await _db.Set<QuizQuestion>().FirstOrDefaultAsync(x => x.Id == questionId && x.QuizId == quizId, ct);
        if (entity is null) return Result.Failure(Error.NotFound("Question.NotFound", "Không tìm thấy Question."));

        entity.RestoreToDraft();
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeleteQuestionAsync(long quizId, long questionId, CancellationToken ct = default)
    {
        var entity = await _db.Set<QuizQuestion>().FirstOrDefaultAsync(x => x.Id == questionId && x.QuizId == quizId, ct);
        if (entity is null) return Result.Failure(Error.NotFound("Question.NotFound", "Không tìm thấy Question."));

        if (entity.Status != ContentStatus.Draft && entity.Status != ContentStatus.Archived)
        {
            return Result.Failure(Error.Conflict("Question.DeleteInvalidStatus", "Chỉ Question Draft hoặc Archived mới được xóa."));
        }

        _db.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
    #endregion

    #region Options
    public async Task<Result<IReadOnlyCollection<AdminQuizQuestionOptionResponse>>> GetOptionsAsync(long questionId, CancellationToken ct = default)
    {
        var items = await _db.Set<QuizQuestionOption>()
            .AsNoTracking()
            .Where(x => x.QuestionId == questionId)
            .OrderBy(x => x.SortOrder)
            .ToArrayAsync(ct);

        var dtos = items.Select(x => x.ToAdminResponse()).ToArray();
        return Result.Success((IReadOnlyCollection<AdminQuizQuestionOptionResponse>)dtos);
    }

    public async Task<Result<AdminQuizQuestionOptionResponse>> CreateOptionAsync(long questionId, CreateQuizQuestionOptionRequest request, CancellationToken ct = default)
    {
        var entity = new QuizQuestionOption(
            questionId,
            request.OptionText,
            request.IsCorrect,
            request.SortOrder);

        entity.Update(
            request.OptionText,
            request.OptionPinyin,
            request.IsCorrect,
            request.SortOrder,
            request.ExplanationVi);

        entity.ChangeOrder(request.SortOrder);
        
        if (request.IsCorrect)
        {
            entity.MarkCorrect();
        }
        else
        {
            entity.MarkIncorrect();
        }

        _db.Add(entity);
        await _db.SaveChangesAsync(ct);

        return Result.Success(entity.ToAdminResponse());
    }

    public async Task<Result<AdminQuizQuestionOptionResponse>> UpdateOptionAsync(long questionId, long optionId, UpdateQuizQuestionOptionRequest request, CancellationToken ct = default)
    {
        var entity = await _db.Set<QuizQuestionOption>().FirstOrDefaultAsync(x => x.Id == optionId && x.QuestionId == questionId, ct);
        if (entity is null) return Result.Failure<AdminQuizQuestionOptionResponse>(Error.NotFound("Option.NotFound", "Không tìm thấy Option."));

        entity.Update(
            request.OptionText,
            request.OptionPinyin,
            request.IsCorrect,
            request.SortOrder,
            request.ExplanationVi);

        entity.ChangeOrder(request.SortOrder);

        if (request.IsCorrect)
        {
            entity.MarkCorrect();
        }
        else
        {
            entity.MarkIncorrect();
        }

        await _db.SaveChangesAsync(ct);
        return Result.Success(entity.ToAdminResponse());
    }

    public async Task<Result> DeleteOptionAsync(long questionId, long optionId, CancellationToken ct = default)
    {
        var entity = await _db.Set<QuizQuestionOption>().FirstOrDefaultAsync(x => x.Id == optionId && x.QuestionId == questionId, ct);
        if (entity is null) return Result.Failure(Error.NotFound("Option.NotFound", "Không tìm thấy Option."));

        _db.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
    #endregion

    #region Matching Pairs
    public async Task<Result<IReadOnlyCollection<AdminQuizMatchingPairResponse>>> GetMatchingPairsAsync(long questionId, CancellationToken ct = default)
    {
        var items = await _db.Set<QuizMatchingPair>()
            .AsNoTracking()
            .Where(x => x.QuestionId == questionId)
            .OrderBy(x => x.SortOrder)
            .ToArrayAsync(ct);

        var dtos = items.Select(x => x.ToAdminResponse()).ToArray();
        return Result.Success((IReadOnlyCollection<AdminQuizMatchingPairResponse>)dtos);
    }

    public async Task<Result<AdminQuizMatchingPairResponse>> CreateMatchingPairAsync(long questionId, CreateQuizMatchingPairRequest request, CancellationToken ct = default)
    {
        var entity = new QuizMatchingPair(
            questionId,
            request.LeftText,
            request.RightText,
            request.SortOrder);

        entity.Update(request.LeftText, request.RightText, request.LeftPinyin, request.RightPinyin, request.SortOrder);

        entity.ChangeOrder(request.SortOrder);

        _db.Add(entity);
        await _db.SaveChangesAsync(ct);

        return Result.Success(entity.ToAdminResponse());
    }

    public async Task<Result<AdminQuizMatchingPairResponse>> UpdateMatchingPairAsync(long questionId, long pairId, UpdateQuizMatchingPairRequest request, CancellationToken ct = default)
    {
        var entity = await _db.Set<QuizMatchingPair>().FirstOrDefaultAsync(x => x.Id == pairId && x.QuestionId == questionId, ct);
        if (entity is null) return Result.Failure<AdminQuizMatchingPairResponse>(Error.NotFound("Pair.NotFound", "Không tìm thấy Matching Pair."));

        entity.Update(request.LeftText, request.RightText, request.LeftPinyin, request.RightPinyin, request.SortOrder);

        entity.ChangeOrder(request.SortOrder);

        await _db.SaveChangesAsync(ct);
        return Result.Success(entity.ToAdminResponse());
    }

    public async Task<Result> DeleteMatchingPairAsync(long questionId, long pairId, CancellationToken ct = default)
    {
        var entity = await _db.Set<QuizMatchingPair>().FirstOrDefaultAsync(x => x.Id == pairId && x.QuestionId == questionId, ct);
        if (entity is null) return Result.Failure(Error.NotFound("Pair.NotFound", "Không tìm thấy Matching Pair."));

        _db.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
    #endregion

    #region Tags
    public async Task<Result<IReadOnlyCollection<AdminQuizTagResponse>>> GetTagsAsync(CancellationToken ct = default)
    {
        var items = await _db.Set<QuizTag>()
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToArrayAsync(ct);

        var dtos = items.Select(x => x.ToAdminResponse()).ToArray();
        return Result.Success((IReadOnlyCollection<AdminQuizTagResponse>)dtos);
    }

    public async Task<Result<AdminQuizTagResponse>> CreateTagAsync(CreateQuizTagRequest request, CancellationToken ct = default)
    {
        var entity = new QuizTag(
            request.Slug,
            request.Name,
            request.NameVi);
        entity.Update(
            request.Slug,
            request.Name,
            request.NameVi,
            request.DescriptionVi);

        _db.Add(entity);
        await _db.SaveChangesAsync(ct);

        return Result.Success(entity.ToAdminResponse());
    }

    public async Task<Result<AdminQuizTagResponse>> UpdateTagAsync(long id, UpdateQuizTagRequest request, CancellationToken ct = default)
    {
        var entity = await _db.Set<QuizTag>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return Result.Failure<AdminQuizTagResponse>(Error.NotFound("Tag.NotFound", "Không tìm thấy Tag."));

        entity.Update(
            request.Slug,
            request.Name,
            request.NameVi,
            request.DescriptionVi);

        await _db.SaveChangesAsync(ct);
        return Result.Success(entity.ToAdminResponse());
    }

    public async Task<Result> ActivateTagAsync(long id, CancellationToken ct = default)
    {
        var entity = await _db.Set<QuizTag>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return Result.Failure(Error.NotFound("Tag.NotFound", "Không tìm thấy Tag."));

        entity.Activate();
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeactivateTagAsync(long id, CancellationToken ct = default)
    {
        var entity = await _db.Set<QuizTag>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return Result.Failure(Error.NotFound("Tag.NotFound", "Không tìm thấy Tag."));

        entity.Deactivate();
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeleteTagAsync(long id, CancellationToken ct = default)
    {
        var entity = await _db.Set<QuizTag>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return Result.Failure(Error.NotFound("Tag.NotFound", "Không tìm thấy Tag."));

        _db.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> AttachTagAsync(long questionId, long tagId, CancellationToken ct = default)
    {
        var question = await _db.Set<QuizQuestion>().FirstOrDefaultAsync(x => x.Id == questionId, ct);
        if (question is null) return Result.Failure(Error.NotFound("Question.NotFound", "Không tìm thấy Question."));

        var tag = await _db.Set<QuizTag>().FirstOrDefaultAsync(x => x.Id == tagId, ct);
        if (tag is null) return Result.Failure(Error.NotFound("Tag.NotFound", "Không tìm thấy Tag."));

        var exists = await _db.Set<QuizQuestionTag>().AnyAsync(x => x.QuestionId == questionId && x.TagId == tagId, ct);
        if (!exists)
        {
            _db.Set<QuizQuestionTag>().Add(new QuizQuestionTag(questionId, tagId));
            await _db.SaveChangesAsync(ct);
        }

        return Result.Success();
    }

    public async Task<Result> DetachTagAsync(long questionId, long tagId, CancellationToken ct = default)
    {
        var question = await _db.Set<QuizQuestion>().FirstOrDefaultAsync(x => x.Id == questionId, ct);
        if (question is null) return Result.Failure(Error.NotFound("Question.NotFound", "Không tìm thấy Question."));

        var qt = await _db.Set<QuizQuestionTag>().FirstOrDefaultAsync(x => x.QuestionId == questionId && x.TagId == tagId, ct);
        if (qt is not null)
        {
            _db.Set<QuizQuestionTag>().Remove(qt);
            await _db.SaveChangesAsync(ct);
        }

        return Result.Success();
    }
    #endregion

    #region Question Banks
    public async Task<Result<IReadOnlyCollection<AdminQuestionBankResponse>>> GetQuestionBanksAsync(CancellationToken ct = default)
    {
        var items = await _db.Set<QuizQuestionBank>()
            .AsNoTracking()
            .Include(x => x.Items)
            .OrderBy(x => x.Code)
            .ToArrayAsync(ct);

        var dtos = items.Select(x => x.ToAdminResponse()).ToArray();
        return Result.Success((IReadOnlyCollection<AdminQuestionBankResponse>)dtos);
    }

    public async Task<Result<AdminQuestionBankResponse>> CreateQuestionBankAsync(CreateQuestionBankRequest request, CancellationToken ct = default)
    {
        var entity = new QuizQuestionBank(
            request.Code,
            request.NameVi,
            request.HskLevelId,
            request.DescriptionVi);

        _db.Add(entity);
        await _db.SaveChangesAsync(ct);

        return await GetQuestionBankResponseAsync(entity.Id, ct);
    }

    public async Task<Result<AdminQuestionBankResponse>> UpdateQuestionBankAsync(long id, UpdateQuestionBankRequest request, CancellationToken ct = default)
    {
        var entity = await _db.Set<QuizQuestionBank>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return Result.Failure<AdminQuestionBankResponse>(Error.NotFound("Bank.NotFound", "Không tìm thấy Question Bank."));

        entity.Update(
            request.Code,
            request.NameVi,
            request.DescriptionVi,
            request.HskLevelId);

        await _db.SaveChangesAsync(ct);
        return await GetQuestionBankResponseAsync(entity.Id, ct);
    }

    private async Task<Result<AdminQuestionBankResponse>> GetQuestionBankResponseAsync(long id, CancellationToken ct)
    {
        var entity = await _db.Set<QuizQuestionBank>()
            .AsNoTracking()
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return Result.Success(entity!.ToAdminResponse());
    }

    public async Task<Result> AddQuestionToBankAsync(long bankId, AddQuestionToBankRequest request, CancellationToken ct = default)
    {
        var bank = await _db.Set<QuizQuestionBank>().Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == bankId, ct);
        if (bank is null) return Result.Failure(Error.NotFound("Bank.NotFound", "Không tìm thấy Question Bank."));

        if (bank.Items.Any(x => x.QuestionId == request.QuestionId))
        {
            return Result.Failure(Error.Conflict("Bank.DuplicateItem", "Question đã tồn tại trong Bank này."));
        }

        bank.Items.Add(new QuizQuestionBankItem(bankId, request.QuestionId, request.SortOrder));
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> RemoveQuestionFromBankAsync(long bankId, long questionId, CancellationToken ct = default)
    {
        var item = await _db.Set<QuizQuestionBankItem>().FirstOrDefaultAsync(x => x.QuestionBankId == bankId && x.QuestionId == questionId, ct);
        if (item is null) return Result.Failure(Error.NotFound("BankItem.NotFound", "Không tìm thấy Question trong Bank."));

        _db.Remove(item);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> ActivateQuestionBankAsync(long id, CancellationToken ct = default)
    {
        var entity = await _db.Set<QuizQuestionBank>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return Result.Failure(Error.NotFound("Bank.NotFound", "Không tìm thấy Question Bank."));

        entity.Activate();
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeactivateQuestionBankAsync(long id, CancellationToken ct = default)
    {
        var entity = await _db.Set<QuizQuestionBank>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return Result.Failure(Error.NotFound("Bank.NotFound", "Không tìm thấy Question Bank."));

        entity.Deactivate();
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
    #endregion
}


