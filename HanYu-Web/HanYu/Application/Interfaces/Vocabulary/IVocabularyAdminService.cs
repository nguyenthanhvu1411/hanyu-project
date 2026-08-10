using HanYu.Application.Common.Models;
using HanYu.Application.Features.Vocabulary.Admin.AudioAssets;
using HanYu.Application.Features.Vocabulary.Admin.Examples;
using HanYu.Application.Features.Vocabulary.Admin.HskLevels;
using HanYu.Application.Features.Vocabulary.Admin.Meanings;
using HanYu.Application.Features.Vocabulary.Admin.PartsOfSpeech;
using HanYu.Application.Features.Vocabulary.Admin.Relations;
using HanYu.Application.Features.Vocabulary.Admin.Topics;
using HanYu.Application.Features.Vocabulary.Admin.Vocabulary;

namespace HanYu.Application.Interfaces.Vocabulary;

public interface IVocabularyAdminService
{
    Task<Result<PagedResult<AdminVocabularyResponse>>>
        GetVocabulariesAsync(
            AdminVocabularyQuery query,
            CancellationToken cancellationToken = default);

    Task<Result<AdminVocabularyResponse>>
        GetVocabularyAsync(
            long id,
            CancellationToken cancellationToken = default);

    Task<Result<AdminVocabularyResponse>>
        CreateVocabularyAsync(
            CreateVocabularyRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminVocabularyResponse>>
        UpdateVocabularyAsync(
            long id,
            UpdateVocabularyRequest request,
            CancellationToken cancellationToken = default);

    Task<Result> SubmitVocabularyForReviewAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result> ApproveVocabularyAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result> PublishVocabularyAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result> ArchiveVocabularyAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result> RestoreVocabularyAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteVocabularyAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<AdminVocabularyMeaningResponse>>>
        GetMeaningsAsync(
            long vocabularyId,
            CancellationToken cancellationToken = default);

    Task<Result<AdminVocabularyMeaningResponse>>
        CreateMeaningAsync(
            long vocabularyId,
            CreateVocabularyMeaningRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminVocabularyMeaningResponse>>
        UpdateMeaningAsync(
            long vocabularyId,
            long meaningId,
            UpdateVocabularyMeaningRequest request,
            CancellationToken cancellationToken = default);

    Task<Result> DeleteMeaningAsync(
        long vocabularyId,
        long meaningId,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<AdminVocabularyExampleResponse>>>
        GetExamplesAsync(
            long vocabularyId,
            CancellationToken cancellationToken = default);

    Task<Result<AdminVocabularyExampleResponse>>
        CreateExampleAsync(
            long vocabularyId,
            CreateVocabularyExampleRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminVocabularyExampleResponse>>
        UpdateExampleAsync(
            long vocabularyId,
            long exampleId,
            UpdateVocabularyExampleRequest request,
            CancellationToken cancellationToken = default);

    Task<Result> SubmitExampleForReviewAsync(
        long vocabularyId,
        long exampleId,
        CancellationToken cancellationToken = default);

    Task<Result> ApproveExampleAsync(
        long vocabularyId,
        long exampleId,
        CancellationToken cancellationToken = default);

    Task<Result> PublishExampleAsync(
        long vocabularyId,
        long exampleId,
        CancellationToken cancellationToken = default);

    Task<Result> ArchiveExampleAsync(
        long vocabularyId,
        long exampleId,
        CancellationToken cancellationToken = default);

    Task<Result> RestoreExampleAsync(
        long vocabularyId,
        long exampleId,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteExampleAsync(
        long vocabularyId,
        long exampleId,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<AdminVocabularyRelationResponse>>>
        GetRelationsAsync(
            long vocabularyId,
            CancellationToken cancellationToken = default);

    Task<Result<AdminVocabularyRelationResponse>>
        CreateRelationAsync(
            long vocabularyId,
            CreateVocabularyRelationRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminVocabularyRelationResponse>>
        UpdateRelationAsync(
            long vocabularyId,
            long relationId,
            UpdateVocabularyRelationRequest request,
            CancellationToken cancellationToken = default);

    Task<Result> DeleteRelationAsync(
        long vocabularyId,
        long relationId,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<AdminTopicResponse>>>
        GetTopicsAsync(
            CancellationToken cancellationToken = default);

    Task<Result<AdminTopicResponse>>
        CreateTopicAsync(
            CreateTopicRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminTopicResponse>>
        UpdateTopicAsync(
            long id,
            UpdateTopicRequest request,
            CancellationToken cancellationToken = default);

    Task<Result> PublishTopicAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result> ArchiveTopicAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result> RestoreTopicAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteTopicAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<AdminPartOfSpeechResponse>>>
        GetPartsOfSpeechAsync(
            CancellationToken cancellationToken = default);

    Task<Result<AdminPartOfSpeechResponse>>
        CreatePartOfSpeechAsync(
            CreatePartOfSpeechRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminPartOfSpeechResponse>>
        UpdatePartOfSpeechAsync(
            long id,
            UpdatePartOfSpeechRequest request,
            CancellationToken cancellationToken = default);

    Task<Result> DeletePartOfSpeechAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<AdminHskLevelResponse>>>
        GetHskLevelsAsync(
            CancellationToken cancellationToken = default);

    Task<Result<AdminHskLevelResponse>>
        CreateHskLevelAsync(
            CreateHskLevelRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminHskLevelResponse>>
        UpdateHskLevelAsync(
            long id,
            UpdateHskLevelRequest request,
            CancellationToken cancellationToken = default);

    Task<Result> ActivateHskLevelAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result> DeactivateHskLevelAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteHskLevelAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result<PagedResult<AdminAudioAssetResponse>>>
        GetAudioAssetsAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

    Task<Result<AdminAudioAssetResponse>>
        CreateAudioAssetAsync(
            CreateAudioAssetRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminAudioAssetResponse>>
        UpdateAudioAssetAsync(
            long id,
            UpdateAudioAssetRequest request,
            CancellationToken cancellationToken = default);

    Task<Result> PublishAudioAssetAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result> ArchiveAudioAssetAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAudioAssetAsync(
        long id,
        CancellationToken cancellationToken = default);
}
