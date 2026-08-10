using System.Text.Json;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Quiz.Mapping;
using HanYu.Application.Features.Quiz.Public.Answers;
using HanYu.Application.Features.Quiz.Public.Attempts;
using HanYu.Application.Features.Quiz.Public.Quizzes;
using HanYu.Application.Features.Quiz.Public.Results;
using HanYu.Application.Interfaces.Gamification;
using HanYu.Application.Interfaces.Quiz;
using HanYu.Domain.Constants;
using HanYu.Domain.Entities.Quiz;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Quiz;

public sealed class QuizPublicService : IQuizPublicService
{
    private readonly HanYuDbContext _db;
    private readonly IGamificationService _gamification;
    private readonly IAchievementEvaluator _achievementEvaluator;

    public QuizPublicService(
        HanYuDbContext db,
        IGamificationService gamification,
        IAchievementEvaluator achievementEvaluator)
    {
        _db = db;
        _gamification = gamification;
        _achievementEvaluator = achievementEvaluator;
    }

    public async Task<Result<PagedResult<QuizListItemResponse>>> GetQuizzesAsync(
        Guid? lessonPublicId,
        CancellationToken cancellationToken = default)
    {
        var q = _db.Set<Domain.Entities.Quiz.Quiz>()
            .AsNoTracking()
            .Where(x => x.Status == ContentStatus.Published);

        if (lessonPublicId.HasValue)
        {
            q = q.Where(x => x.Lesson != null && x.Lesson.PublicId == lessonPublicId.Value);
        }

        var items = await q
            .OrderByDescending(x => x.PublishedAt)
            .Select(x => new { Quiz = x, QuestionCount = x.Questions.Count(q => q.Status == ContentStatus.Published) })
            .ToArrayAsync(cancellationToken);

        var dtos = items.Select(x => x.Quiz.ToListItemResponse(x.QuestionCount)).ToArray();

        return Result.Success(new PagedResult<QuizListItemResponse>(dtos, dtos.Length, 1, Math.Max(1, dtos.Length)));
    }

    public async Task<Result<QuizDetailResponse>> GetQuizAsync(
        Guid quizPublicId,
        CancellationToken cancellationToken = default)
    {
        var quiz = await _db.Set<Domain.Entities.Quiz.Quiz>()
            .AsNoTracking()
            .Where(x => x.PublicId == quizPublicId && x.Status == ContentStatus.Published)
            .Select(x => new { Quiz = x, QuestionCount = x.Questions.Count(q => q.Status == ContentStatus.Published) })
            .FirstOrDefaultAsync(cancellationToken);

        if (quiz is null)
        {
            return Result.Failure<QuizDetailResponse>(
                Error.NotFound("Quiz.NotFound", "Không tìm thấy bài Quiz."));
        }

        return Result.Success(quiz.Quiz.ToDetailResponse(quiz.QuestionCount));
    }

    public async Task<Result<QuizAttemptResponse>> StartAttemptAsync(
        Guid userId,
        Guid quizPublicId,
        StartQuizAttemptRequest request,
        CancellationToken cancellationToken = default)
    {
        var quiz = await _db.Set<Domain.Entities.Quiz.Quiz>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PublicId == quizPublicId && x.Status == ContentStatus.Published, cancellationToken);

        if (quiz is null)
        {
            return Result.Failure<QuizAttemptResponse>(Error.NotFound("Quiz.NotFound", "Không tìm thấy bài Quiz."));
        }

        var existing = await _db.Set<QuizAttempt>()
            .Include(x => x.Questions)
            .Include(x => x.Answers)
            .Include(x => x.Quiz)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.QuizId == quiz.Id && x.IdempotencyKey == request.IdempotencyKey, cancellationToken);

        if (existing is not null)
        {
            return Result.Success(existing.ToAttemptResponse());
        }

        var attemptCount = await _db.Set<QuizAttempt>()
            .CountAsync(x => x.UserId == userId && x.QuizId == quiz.Id, cancellationToken);

        if (quiz.MaxAttempts > 0 && attemptCount >= quiz.MaxAttempts)
        {
            return Result.Failure<QuizAttemptResponse>(
                Error.Conflict("Quiz.MaxAttemptsReached", "Bạn đã sử dụng hết số lần làm bài."));
        }

        if (!quiz.AllowRetry && attemptCount > 0)
        {
            return Result.Failure<QuizAttemptResponse>(
                Error.Conflict("Quiz.NoRetryAllowed", "Bài Quiz này không cho phép làm lại."));
        }

        var expiresAt = quiz.TimeLimitSeconds.HasValue
            ? (DateTimeOffset?)DateTimeOffset.UtcNow.AddSeconds(quiz.TimeLimitSeconds.Value)
            : null;

        var attempt = new QuizAttempt(
            userId,
            quiz.Id,
            attemptCount + 1,
            request.IdempotencyKey,
            expiresAt);

        _db.Add(attempt);

        // Snapshot questions
        var questions = await _db.Set<QuizQuestion>()
            .AsNoTracking()
            .Include(x => x.Options)
            .Include(x => x.MatchingPairs)
            .Where(x => x.QuizId == quiz.Id && x.Status == ContentStatus.Published)
            .ToArrayAsync(cancellationToken);

        // Shuffle logic (basic randomization if applicable)
        var rnd = new Random();
        if (quiz.ShuffleMode == QuizShuffleMode.QuestionsAndOptions || quiz.ShuffleMode == QuizShuffleMode.QuestionsOnly)
        {
            questions = questions.OrderBy(x => rnd.Next()).ToArray();
        }

        int order = 1;
        foreach (var q in questions)
        {
            var options = q.Options.AsEnumerable();
            if (quiz.ShuffleMode == QuizShuffleMode.QuestionsAndOptions || quiz.ShuffleMode == QuizShuffleMode.OptionsOnly)
            {
                options = options.OrderBy(x => rnd.Next());
            }

            var snapshotObj = new
            {
                q.PublicId,
                QuestionPublicId = q.PublicId,
                q.QuestionType,
                q.Prompt,
                q.PromptPinyin,
                q.HintVi,
                q.Points,
                q.TimeLimitSeconds,
                SortOrder = order,
                Options = options.Select((opt, idx) => new 
                {
                    opt.PublicId,
                    opt.OptionText,
                    opt.OptionPinyin,
                    SortOrder = idx + 1
                }),
                MatchingPairs = q.MatchingPairs.OrderBy(m => rnd.Next()).Select((m, idx) => new 
                {
                    m.PublicId,
                    m.LeftText,
                    m.LeftPinyin,
                    SortOrder = idx + 1
                })
            };

            var json = JsonSerializer.Serialize(snapshotObj);
            var aq = new QuizAttemptQuestion(0, q.Id, order, json);
            // Link manually since ID is 0
            attempt.Questions.Add(aq);

            order++;
        }

        await _db.SaveChangesAsync(cancellationToken);

        // Reload to get properly mapped quiz object
        var reloaded = await _db.Set<QuizAttempt>()
            .Include(x => x.Questions)
            .Include(x => x.Answers)
            .Include(x => x.Quiz)
            .FirstAsync(x => x.Id == attempt.Id, cancellationToken);

        return Result.Success(reloaded.ToAttemptResponse());
    }

    public async Task<Result<QuizAttemptResponse>> GetAttemptAsync(
        Guid userId,
        Guid attemptPublicId,
        CancellationToken cancellationToken = default)
    {
        var attempt = await _db.Set<QuizAttempt>()
            .Include(x => x.Questions)
            .Include(x => x.Answers)
            .Include(x => x.Quiz)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.PublicId == attemptPublicId, cancellationToken);

        if (attempt is null) return Result.Failure<QuizAttemptResponse>(Error.NotFound("Attempt.NotFound", "Không tìm thấy Attempt."));

        return Result.Success(attempt.ToAttemptResponse());
    }

    public async Task<Result<QuizAnswerResultResponse>> SubmitAnswerAsync(
        Guid userId,
        Guid attemptPublicId,
        Guid questionPublicId,
        SubmitQuizAnswerRequest request,
        CancellationToken cancellationToken = default)
    {
        var attempt = await _db.Set<QuizAttempt>()
            .Include(x => x.Quiz)
            .Include(x => x.Answers)
            .Include(x => x.Questions)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.PublicId == attemptPublicId, cancellationToken);

        if (attempt is null) return Result.Failure<QuizAnswerResultResponse>(Error.NotFound("Attempt.NotFound", "Không tìm thấy Attempt."));

        if (attempt.Status != QuizAttemptStatus.InProgress || attempt.IsExpired(DateTimeOffset.UtcNow))
        {
            if (attempt.Status == QuizAttemptStatus.InProgress && attempt.IsExpired(DateTimeOffset.UtcNow))
            {
                attempt.Expire();
                await _db.SaveChangesAsync(cancellationToken);
            }
            return Result.Failure<QuizAnswerResultResponse>(Error.Conflict("Attempt.Expired", "Bài Quiz đã kết thúc."));
        }

        var attemptQuestion = attempt.Questions.FirstOrDefault(x => QuizMapper.ParseSnapshot(x.QuestionSnapshotJson)?.QuestionPublicId == questionPublicId);
        if (attemptQuestion is null) return Result.Failure<QuizAnswerResultResponse>(Error.NotFound("Question.NotFound", "Câu hỏi không nằm trong bài này."));

        if (attempt.Answers.Any(x => x.QuestionId == attemptQuestion.QuestionId))
        {
            return Result.Failure<QuizAnswerResultResponse>(Error.Conflict("Answer.AlreadySubmitted", "Câu hỏi đã được trả lời."));
        }

        var realQuestion = await _db.Set<QuizQuestion>()
            .Include(x => x.Options)
            .FirstOrDefaultAsync(x => x.Id == attemptQuestion.QuestionId, cancellationToken);

        bool isCorrect = false;
        decimal earnedPoints = 0m;
        long? optionId = null;

        if (realQuestion!.QuestionType == QuizQuestionType.MultipleChoice || 
            realQuestion.QuestionType == QuizQuestionType.MeaningChoice ||
            realQuestion.QuestionType == QuizQuestionType.HanziChoice ||
            realQuestion.QuestionType == QuizQuestionType.PinyinChoice)
        {
            var opt = realQuestion.Options.FirstOrDefault(x => x.PublicId == request.SelectedOptionPublicId);
            if (opt != null)
            {
                isCorrect = opt.IsCorrect;
                earnedPoints = isCorrect ? realQuestion.Points : 0m;
                optionId = opt.Id;
            }
        }
        else
        {
            // Simple text match
            isCorrect = CompareText(request.AnswerText, realQuestion.CorrectAnswerText);
            earnedPoints = isCorrect ? realQuestion.Points : 0m;
        }

        var answer = new QuizAttemptAnswer(attempt.Id, realQuestion.Id);
        answer.Answer(optionId, request.AnswerText, request.AnswerJson, isCorrect, earnedPoints, request.ResponseTimeMs);

        _db.Add(answer);
        await _db.SaveChangesAsync(cancellationToken);

        if (attempt.Quiz.FeedbackMode == QuizFeedbackMode.AfterEachAnswer)
        {
            return Result.Success(new QuizAnswerResultResponse(
                true, 
                isCorrect, 
                earnedPoints, 
                attempt.Quiz.ShowCorrectAnswer ? realQuestion.CorrectAnswerText : null, 
                attempt.Quiz.ShowExplanation ? realQuestion.ExplanationVi : null));
        }
        else
        {
            return Result.Success(new QuizAnswerResultResponse(true, null, null, null, null));
        }
    }

    private static bool CompareText(string? answer, string? expected)
    {
        if (string.IsNullOrWhiteSpace(answer) || string.IsNullOrWhiteSpace(expected)) return false;

        static string Normalize(string value) => string.Join(' ', value.Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        return Normalize(answer) == Normalize(expected);
    }

    public async Task<Result<QuizResultResponse>> SubmitAttemptAsync(
        Guid userId,
        Guid attemptPublicId,
        CancellationToken cancellationToken = default)
    {
        var attempt = await _db.Set<QuizAttempt>()
            .Include(x => x.Quiz)
            .Include(x => x.Questions)
            .Include(x => x.Answers)
                .ThenInclude(a => a.Question)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.PublicId == attemptPublicId, cancellationToken);

        if (attempt is null) return Result.Failure<QuizResultResponse>(Error.NotFound("Attempt.NotFound", "Không tìm thấy Attempt."));

        if (attempt.Status != QuizAttemptStatus.InProgress && attempt.Status != QuizAttemptStatus.Expired)
        {
            return Result.Success(attempt.ToResultResponse());
        }

        var attemptQuestionsIds = attempt.Questions.Select(x => x.QuestionId).ToArray();
        var allQuestionsPoints = await _db.Set<QuizQuestion>()
            .Where(x => attemptQuestionsIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.Points, cancellationToken);

        var maxScore = allQuestionsPoints.Values.Sum();
        var score = attempt.Answers.Sum(x => x.EarnedPoints ?? 0m);
        var correct = attempt.Answers.Count(x => x.IsCorrect == true);
        var wrong = attempt.Answers.Count(x => x.IsCorrect == false);
        var unanswered = attempt.Questions.Count - attempt.Answers.Count;

        attempt.Submit(score, maxScore, attempt.Quiz.PassingScore, correct, wrong, unanswered);
        
        await _db.SaveChangesAsync(cancellationToken);

        if (attempt.IsPassed == true)
        {
            await _gamification.AwardXpAsync(
                userId,
                GamificationConstants.QuizPassedXp,
                "Vượt qua bài kiểm tra",
                XpSources.Quiz,
                attempt.PublicId.ToString(),
                cancellationToken);
        }
        
        if (attempt.Percentage == 100m)
        {
            await _gamification.AwardXpAsync(
                userId,
                GamificationConstants.PerfectQuizBonusXp,
                "Perfect Quiz",
                "quiz-perfect",
                attempt.PublicId.ToString(),
                cancellationToken);
        }

        await _achievementEvaluator.EvaluateAsync(
            userId,
            cancellationToken);

        return Result.Success(attempt.ToResultResponse());
    }

    public async Task<Result<QuizResultResponse>> GetResultAsync(
        Guid userId,
        Guid attemptPublicId,
        CancellationToken cancellationToken = default)
    {
        var attempt = await _db.Set<QuizAttempt>()
            .Include(x => x.Quiz)
            .Include(x => x.Answers)
                .ThenInclude(a => a.Question)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.PublicId == attemptPublicId, cancellationToken);

        if (attempt is null) return Result.Failure<QuizResultResponse>(Error.NotFound("Attempt.NotFound", "Không tìm thấy Attempt."));

        if (attempt.Status != QuizAttemptStatus.Submitted)
        {
            return Result.Failure<QuizResultResponse>(Error.Conflict("Attempt.NotSubmitted", "Bài thi chưa được nộp."));
        }

        return Result.Success(attempt.ToResultResponse());
    }

    public async Task<Result<IReadOnlyCollection<QuizResultResponse>>> GetMyHistoryAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var items = await _db.Set<QuizAttempt>()
            .AsNoTracking()
            .Include(x => x.Quiz)
            .Include(x => x.Answers)
                .ThenInclude(a => a.Question)
            .Where(x => x.UserId == userId && x.Status == QuizAttemptStatus.Submitted)
            .OrderByDescending(x => x.SubmittedAt)
            .ToArrayAsync(cancellationToken);

        var dtos = items.Select(x => x.ToResultResponse()).ToArray();
        return Result.Success((IReadOnlyCollection<QuizResultResponse>)dtos);
    }
}
