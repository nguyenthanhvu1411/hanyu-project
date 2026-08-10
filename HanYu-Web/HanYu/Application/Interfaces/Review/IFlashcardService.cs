using HanYu.Application.Common.Models;
using HanYu.Application.Features.Review.Public.Flashcards;

namespace HanYu.Application.Interfaces.Review;

public interface IFlashcardService
{
    Task<Result<FlashcardSessionResponse>>
        CreateSessionAsync(
            Guid userId,
            CreateFlashcardSessionRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<FlashcardSessionResponse>>
        GetSessionAsync(
            Guid userId,
            Guid sessionPublicId,
            CancellationToken cancellationToken = default);

    Task<Result<FlashcardAnswerResponse>>
        AnswerAsync(
            Guid userId,
            Guid sessionPublicId,
            Guid itemPublicId,
            AnswerFlashcardRequest request,
            CancellationToken cancellationToken = default);

    Task<Result> AbandonAsync(
        Guid userId,
        Guid sessionPublicId,
        CancellationToken cancellationToken = default);
}
