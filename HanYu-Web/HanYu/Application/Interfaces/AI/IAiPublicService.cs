using HanYu.Application.Common.Models;
using HanYu.Application.Features.AI.Public.Conversations;
using HanYu.Application.Features.AI.Public.Feedback;

namespace HanYu.Application.Interfaces.AI;

public interface IAiPublicService
{
    Task<Result<IReadOnlyCollection<AiConversationResponse>>>
        GetMyConversationsAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

    Task<Result<AiConversationDetailResponse>>
        GetConversationAsync(
            Guid userId,
            Guid publicId,
            CancellationToken cancellationToken = default);

    Task<Result<AiConversationResponse>>
        CreateConversationAsync(
            Guid userId,
            CreateAiConversationRequest request,
            CancellationToken cancellationToken = default);

    Task<Result> UpdateConversationTitleAsync(
        Guid userId,
        Guid publicId,
        string? title,
        CancellationToken cancellationToken = default);

    Task<Result> ArchiveConversationAsync(
        Guid userId,
        Guid publicId,
        CancellationToken cancellationToken = default);

    Task<Result<AiConversationMessageResponse>>
        SendMessageAsync(
            Guid userId,
            Guid conversationPublicId,
            SendAiMessageRequest request,
            CancellationToken cancellationToken = default);

    Task<Result> SubmitFeedbackAsync(
        Guid userId,
        SubmitAiFeedbackRequest request,
        CancellationToken cancellationToken = default);
}
