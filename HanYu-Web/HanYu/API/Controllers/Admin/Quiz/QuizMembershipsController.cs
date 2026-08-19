using HanYu.Application.Features.Quiz.Admin.Questions;
using HanYu.Application.Features.Quiz.Admin.Tags;
using HanYu.Domain.Entities.Quiz;
using HanYu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HanYu.API.Controllers.Admin.Quiz;

[ApiController]
[Authorize(Roles = "Admin")]
public sealed class QuizMembershipsController : ControllerBase
{
    private readonly HanYuDbContext _db;

    public QuizMembershipsController(HanYuDbContext db)
    {
        _db = db;
    }

    [HttpGet("api/admin/question-banks/{bankId:long}/questions")]
    public async Task<ActionResult<IReadOnlyCollection<AdminQuizQuestionResponse>>> GetQuestionBankQuestions(
        long bankId,
        CancellationToken cancellationToken)
    {
        var bankExists = await _db.Set<QuizQuestionBank>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == bankId, cancellationToken);

        if (!bankExists)
            return NotFound(new { code = "Quiz.QuestionBankNotFound", message = "Không tìm thấy question bank." });

        var items = await _db.Set<QuizQuestionBankItem>()
            .AsNoTracking()
            .Where(x => x.QuestionBankId == bankId)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.AddedAt)
            .Select(x => new AdminQuizQuestionResponse(
                x.Question.Id,
                x.Question.PublicId,
                x.Question.QuizId,
                x.Question.VocabularyId,
                x.Question.Vocabulary != null ? x.Question.Vocabulary.PublicId : null,
                x.Question.QuestionType,
                x.Question.Prompt,
                x.Question.PromptPinyin,
                x.Question.CorrectAnswerText,
                x.Question.ExplanationVi,
                x.Question.HintVi,
                x.Question.Points,
                x.Question.SortOrder,
                x.Question.TimeLimitSeconds,
                x.Question.IsRequired,
                x.Question.Status,
                x.Question.CreatedAt,
                x.Question.UpdatedAt))
            .ToArrayAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("api/admin/quizzes/{quizId:long}/questions/{questionId:long}/tags")]
    public async Task<ActionResult<IReadOnlyCollection<AdminQuizTagResponse>>> GetQuestionTags(
        long quizId,
        long questionId,
        CancellationToken cancellationToken)
    {
        var questionExists = await _db.Set<QuizQuestion>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == questionId && x.QuizId == quizId, cancellationToken);

        if (!questionExists)
            return NotFound(new { code = "Quiz.QuestionNotFound", message = "Không tìm thấy câu hỏi." });

        var tags = await _db.Set<QuizQuestionTag>()
            .AsNoTracking()
            .Where(x => x.QuestionId == questionId)
            .OrderBy(x => x.Tag.NameVi ?? x.Tag.Name)
            .Select(x => new AdminQuizTagResponse(
                x.Tag.Id,
                x.Tag.PublicId,
                x.Tag.Slug,
                x.Tag.Name,
                x.Tag.NameVi,
                x.Tag.DescriptionVi,
                x.Tag.IsActive))
            .ToArrayAsync(cancellationToken);

        return Ok(tags);
    }
}
