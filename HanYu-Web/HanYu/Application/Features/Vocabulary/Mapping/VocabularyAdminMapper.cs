using HanYu.Application.Features.Vocabulary.Admin.AudioAssets;
using HanYu.Application.Features.Vocabulary.Admin.Examples;
using HanYu.Application.Features.Vocabulary.Admin.HskLevels;
using HanYu.Application.Features.Vocabulary.Admin.Meanings;
using HanYu.Application.Features.Vocabulary.Admin.PartsOfSpeech;
using HanYu.Application.Features.Vocabulary.Admin.Relations;
using HanYu.Application.Features.Vocabulary.Admin.Topics;
using HanYu.Domain.Entities.Vocabulary;

namespace HanYu.Application.Features.Vocabulary.Mapping;

public static class VocabularyAdminMapper
{
    public static AdminVocabularyMeaningResponse
        ToMeaningResponse(
            VocabularyMeaning entity)
    {
        return new AdminVocabularyMeaningResponse(
            entity.Id,
            entity.VocabularyId,
            entity.MeaningVi,
            entity.SenseOrder,
            entity.UsageNoteVi,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

    public static AdminVocabularyExampleResponse
        ToExampleResponse(
            VocabularyExample entity)
    {
        return new AdminVocabularyExampleResponse(
            entity.Id,
            entity.VocabularyId,
            entity.AudioAssetId,
            entity.SentenceZh,
            entity.SentencePinyin,
            entity.SentenceVi,
            entity.Difficulty,
            entity.Status,
            entity.SourceNote,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

    public static AdminVocabularyRelationResponse
        ToRelationResponse(
            VocabularyRelation entity)
    {
        return new AdminVocabularyRelationResponse(
            entity.Id,
            entity.VocabularyId,
            entity.RelatedVocabularyId,
            entity.RelatedVocabulary.Simplified,
            entity.RelatedVocabulary.Pinyin,
            entity.RelatedVocabulary.PrimaryMeaningVi,
            entity.RelationType,
            entity.NoteVi,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

    public static AdminTopicResponse
        ToTopicResponse(
            Topic entity)
    {
        return new AdminTopicResponse(
            entity.Id,
            entity.Slug,
            entity.NameVi,
            entity.DescriptionVi,
            entity.SortOrder,
            entity.Status,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

    public static AdminPartOfSpeechResponse
        ToPartOfSpeechResponse(
            PartOfSpeech entity)
    {
        return new AdminPartOfSpeechResponse(
            entity.Id,
            entity.Code,
            entity.NameVi,
            entity.NameEn,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

    public static AdminHskLevelResponse
        ToHskResponse(
            HskLevel entity)
    {
        return new AdminHskLevelResponse(
            entity.Id,
            entity.Code,
            entity.NameVi,
            entity.SortOrder,
            entity.IsActive);
    }

    public static AdminAudioAssetResponse
        ToAudioResponse(
            AudioAsset entity)
    {
        return new AdminAudioAssetResponse(
            entity.Id,
            entity.StoragePath,
            entity.PublicUrl,
            entity.Kind,
            entity.MimeType,
            entity.FileSizeBytes,
            entity.DurationMs,
            entity.Voice,
            entity.Provider,
            entity.LanguageCode,
            entity.Checksum,
            entity.Status,
            entity.CreatedAt,
            entity.UpdatedAt);
    }
}
