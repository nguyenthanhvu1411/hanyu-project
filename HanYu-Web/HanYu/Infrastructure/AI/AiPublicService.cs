using HanYu.Application.Common.Models;
using HanYu.Application.Features.AI.Public.Conversations;
using HanYu.Application.Features.AI.Public.Feedback;
using HanYu.Application.Interfaces.AI;
using HanYu.Application.Interfaces.Analytics;
using HanYu.Domain.Entities.AI;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.AI;

public sealed class AiPublicService : IAiPublicService
{
    private readonly HanYuDbContext _db;
    private readonly IAiProvider _provider;
    private readonly ILearningAnalyticsCollector _analytics;

    public AiPublicService(
        HanYuDbContext db,
        IAiProvider provider,
        ILearningAnalyticsCollector analytics)
    {
        _db = db;
        _provider = provider;
        _analytics = analytics;
    }

    public async Task<Result<IReadOnlyCollection<AiConversationResponse>>>
        GetMyConversationsAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var entities = await _db.Set<AiConversation>()
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.Status != AiConversationStatus.Archived)
            .OrderByDescending(x => x.UpdatedAt)
            .ToArrayAsync(cancellationToken);

        return Result.Success<IReadOnlyCollection<AiConversationResponse>>(
            entities.Select(x => new AiConversationResponse(
                x.PublicId,
                x.Title,
                x.Status,
                x.MessageCount,
                x.LastMessageAt,
                x.CreatedAt,
                x.UpdatedAt)).ToArray());
    }

    public async Task<Result<AiConversationDetailResponse>>
        GetConversationAsync(
            Guid userId,
            Guid publicId,
            CancellationToken cancellationToken = default)
    {
        var conversation = await _db.Set<AiConversation>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.PublicId == publicId, cancellationToken);

        if (conversation is null)
        {
            return Result.Failure<AiConversationDetailResponse>(
                Error.NotFound(
                    "AI.ConversationNotFound",
                    "Không tìm thấy conversation."));
        }

        var messages = await _db.Set<AiConversationMessage>()
            .AsNoTracking()
            .Where(x => x.ConversationId == conversation.Id)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new AiConversationMessageResponse(
                x.PublicId,
                x.Role,
                x.Content,
                x.CreatedAt))
            .ToArrayAsync(cancellationToken);

        return Result.Success(new AiConversationDetailResponse(
            conversation.PublicId,
            conversation.Title,
            conversation.Status,
            conversation.MessageCount,
            conversation.LastMessageAt,
            messages));
    }

    public async Task<Result<AiConversationResponse>>
        CreateConversationAsync(
            Guid userId,
            CreateAiConversationRequest request,
            CancellationToken cancellationToken = default)
    {
        long? lessonId = null;
        long? vocabularyId = null;

        if (request.LessonPublicId.HasValue)
        {
            var lesson = await _db.Set<HanYu.Domain.Entities.Lesson.Lesson>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PublicId == request.LessonPublicId.Value, cancellationToken);
            lessonId = lesson?.Id;
        }

        if (request.VocabularyPublicId.HasValue)
        {
            var voc = await _db.Set<HanYu.Domain.Entities.Vocabulary.Vocabulary>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PublicId == request.VocabularyPublicId.Value, cancellationToken);
            vocabularyId = voc?.Id;
        }

        var entity = new AiConversation(userId, request.Title, lessonId, vocabularyId);

        _db.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success(new AiConversationResponse(
            entity.PublicId,
            entity.Title,
            entity.Status,
            entity.MessageCount,
            entity.LastMessageAt,
            entity.CreatedAt,
            entity.UpdatedAt));
    }

    public async Task<Result> UpdateConversationTitleAsync(
        Guid userId,
        Guid publicId,
        string? title,
        CancellationToken cancellationToken = default)
    {
        var conversation = await _db.Set<AiConversation>()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.PublicId == publicId, cancellationToken);

        if (conversation is null)
        {
            return Result.Failure(Error.NotFound("AI.ConversationNotFound", "Không tìm thấy conversation."));
        }

        if (conversation.Status == AiConversationStatus.Archived)
        {
            return Result.Failure(Error.Conflict("AI.ConversationArchived", "Conversation đã Archived."));
        }

        conversation.UpdateTitle(title);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ArchiveConversationAsync(
        Guid userId,
        Guid publicId,
        CancellationToken cancellationToken = default)
    {
        var conversation = await _db.Set<AiConversation>()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.PublicId == publicId, cancellationToken);

        if (conversation is null)
        {
            return Result.Failure(Error.NotFound("AI.ConversationNotFound", "Không tìm thấy conversation."));
        }

        conversation.Archive();
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<AiConversationMessageResponse>>
        SendMessageAsync(
            Guid userId,
            Guid conversationPublicId,
            SendAiMessageRequest request,
            CancellationToken cancellationToken = default)
    {
        var conversation =
            await _db.Set<AiConversation>()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.PublicId ==
                            conversationPublicId,
                    cancellationToken);

        if (conversation is null)
        {
            return Result.Failure<AiConversationMessageResponse>(
                Error.NotFound(
                    "AI.ConversationNotFound",
                    "Không tìm thấy conversation."));
        }

        if (conversation.Status == AiConversationStatus.Archived)
        {
            return Result.Failure<AiConversationMessageResponse>(
                Error.Conflict(
                    "AI.ConversationArchived",
                    "Conversation đã Archived."));
        }

        var userMessage =
            new AiConversationMessage(
                conversation.Id,
                AiMessageRole.User,
                request.Content);

        _db.Add(userMessage);
        conversation.RegisterMessage();

        await _db.SaveChangesAsync(cancellationToken);

        var history =
            await _db.Set<AiConversationMessage>()
                .AsNoTracking()
                .Where(x => x.ConversationId == conversation.Id)
                .OrderBy(x => x.CreatedAt)
                .Select(x => new AiProviderMessage(
                    x.Role.ToString(),
                    x.Content))
                .ToArrayAsync(cancellationToken);

        var aiRequest =
            new AiRequest(
                userId,
                AiFeatureType.AiTutor,
                "pending",
                "pending",
                conversationId: conversation.Id);

        _db.Add(aiRequest);
        await _db.SaveChangesAsync(cancellationToken);

        try
        {
            var result =
                await _provider.GenerateAsync(
                    new AiProviderRequest(
                        "You are HanYu Chinese learning assistant.",
                        history),
                    cancellationToken);

            aiRequest.Complete(
                result.InputTokens,
                result.OutputTokens,
                result.EstimatedCostUsd,
                result.LatencyMs);

            var assistant =
                new AiConversationMessage(
                    conversation.Id,
                    AiMessageRole.Assistant,
                    result.Content,
                    aiRequest.Id);

            _db.Add(assistant);
            conversation.RegisterMessage();

            await _db.SaveChangesAsync(cancellationToken);

            await _analytics.RegisterAiInteractionAsync(
                userId,
                cancellationToken);

            return Result.Success(
                new AiConversationMessageResponse(
                    assistant.PublicId,
                    assistant.Role,
                    assistant.Content,
                    assistant.CreatedAt));
        }
        catch (Exception ex)
        {
            aiRequest.Fail(
                "provider_error",
                ex.Message);

            await _db.SaveChangesAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Result> SubmitFeedbackAsync(
        Guid userId,
        SubmitAiFeedbackRequest request,
        CancellationToken cancellationToken = default)
    {
        var aiRequest = await _db.Set<AiRequest>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PublicId == request.AiRequestPublicId, cancellationToken);

        if (aiRequest is null)
        {
            return Result.Failure(Error.NotFound("AI.RequestNotFound", "Không tìm thấy request AI để feedback."));
        }

        var feedback = new AiFeedback(
            userId,
            aiRequest.Id,
            request.Rating,
            request.Comment,
            request.IssueType);

        _db.Add(feedback);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
