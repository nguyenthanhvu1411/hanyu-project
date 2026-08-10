using HanYu.Application.Common.Models;
using HanYu.Application.Features.Vocabulary.Public.Notes;
using HanYu.Application.Features.Vocabulary.Public.Taxonomy;
using HanYu.Application.Features.Vocabulary.Public.Vocabulary;

namespace HanYu.Application.Interfaces.Vocabulary;

public interface IVocabularyPublicService
{
    Task<Result<
        PagedResult<VocabularyListItemResponse>>>
        GetVocabulariesAsync(
            VocabularyQuery query,
            CancellationToken cancellationToken = default);

    Task<Result<VocabularyDetailResponse>>
        GetVocabularyAsync(
            string simplified,
            string? pinyinNormalized,
            CancellationToken cancellationToken = default);

    Task<Result<
        IReadOnlyCollection<PublicTopicResponse>>>
        GetTopicsAsync(
            CancellationToken cancellationToken = default);

    Task<Result<
        IReadOnlyCollection<PublicPartOfSpeechResponse>>>
        GetPartsOfSpeechAsync(
            CancellationToken cancellationToken = default);

    Task<Result<
        IReadOnlyCollection<PublicHskLevelResponse>>>
        GetHskLevelsAsync(
            CancellationToken cancellationToken = default);

    Task<Result<VocabularyNoteResponse?>>
        GetMyNoteAsync(
            Guid userId,
            string simplified,
            string? pinyinNormalized,
            CancellationToken cancellationToken = default);

    Task<Result<VocabularyNoteResponse>>
        SaveMyNoteAsync(
            Guid userId,
            string simplified,
            string? pinyinNormalized,
            SaveVocabularyNoteRequest request,
            CancellationToken cancellationToken = default);

    Task<Result>
        DeleteMyNoteAsync(
            Guid userId,
            string simplified,
            string? pinyinNormalized,
            CancellationToken cancellationToken = default);
}
