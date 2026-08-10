using HanYu.Application.Features.Vocabulary.Admin.Vocabulary;
using HanYu.Application.Features.Vocabulary.Public.Notes;
using HanYu.Application.Features.Vocabulary.Public.Taxonomy;
using HanYu.Application.Features.Vocabulary.Public.Vocabulary;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Vocabulary.Mapping;

public static class VocabularyMapper
{
    public static AdminVocabularyResponse
        ToAdminResponse(
            Domain.Entities.Vocabulary.Vocabulary entity)
    {
        return new AdminVocabularyResponse(
            entity.Id,
            entity.HskLevelId,
            entity.HskLevel.Code,
            entity.HskLevel.NameVi,
            entity.PartOfSpeechId,
            entity.PartOfSpeech?.Code,
            entity.PartOfSpeech?.NameVi,
            entity.TopicId,
            entity.Topic?.Slug,
            entity.Topic?.NameVi,
            entity.AudioAssetId,
            entity.Simplified,
            entity.Traditional,
            entity.Pinyin,
            entity.PinyinNormalized,
            entity.PrimaryMeaningVi,
            entity.NotesVi,
            entity.Difficulty,
            entity.Status,
            entity.Version,
            entity.PublishedAt,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

    public static VocabularyListItemResponse
        ToPublicListItem(
            Domain.Entities.Vocabulary.Vocabulary entity)
    {
        return new VocabularyListItemResponse(
            entity.Simplified,
            entity.Traditional,
            entity.Pinyin,
            entity.PinyinNormalized,
            entity.PrimaryMeaningVi,
            entity.HskLevelId,
            entity.HskLevel.Code,
            entity.Difficulty,
            entity.PartOfSpeech?.Code,
            entity.Topic?.Slug,
            entity.AudioAsset?.Status ==
                ContentStatus.Published
                ? entity.AudioAsset.PublicUrl
                : null);
    }

    public static VocabularyDetailResponse
        ToPublicDetail(
            Domain.Entities.Vocabulary.Vocabulary entity,
            IReadOnlyCollection<
                VocabularyRelation> relations)
    {
        var meanings =
            entity.Meanings
                .OrderBy(x => x.SenseOrder)
                .Select(
                    x =>
                        new VocabularyMeaningResponse(
                            x.MeaningVi,
                            x.SenseOrder,
                            x.UsageNoteVi))
                .ToArray();

        var examples =
            entity.Examples
                .Where(
                    x =>
                        x.Status ==
                        ContentStatus.Published)
                .OrderBy(x => x.Id)
                .Select(
                    x =>
                        new VocabularyExampleResponse(
                            x.SentenceZh,
                            x.SentencePinyin,
                            x.SentenceVi,
                            x.Difficulty,
                            x.AudioAsset?.Status ==
                                ContentStatus.Published
                                ? x.AudioAsset.PublicUrl
                                : null))
                .ToArray();

        var relationResponses =
            relations
                .Select(
                    x =>
                        new VocabularyRelationResponse(
                            x.RelatedVocabulary.Simplified,
                            x.RelatedVocabulary.Traditional,
                            x.RelatedVocabulary.Pinyin,
                            x.RelatedVocabulary.PrimaryMeaningVi,
                            x.RelationType,
                            x.NoteVi))
                .ToArray();

        return new VocabularyDetailResponse(
            entity.Simplified,
            entity.Traditional,
            entity.Pinyin,
            entity.PinyinNormalized,
            entity.PrimaryMeaningVi,
            entity.NotesVi,
            entity.HskLevelId,
            entity.HskLevel.Code,
            entity.HskLevel.NameVi,
            entity.Difficulty,
            entity.PartOfSpeech?.Code,
            entity.PartOfSpeech?.NameVi,
            entity.Topic?.Slug,
            entity.Topic?.NameVi,
            entity.AudioAsset?.Status ==
                ContentStatus.Published
                ? entity.AudioAsset.PublicUrl
                : null,
            meanings,
            examples,
            relationResponses);
    }

    public static VocabularyNoteResponse
        ToNoteResponse(
            UserVocabularyNote entity)
    {
        return new VocabularyNoteResponse(
            entity.Content,
            entity.IsPinned,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

    public static PublicTopicResponse
        ToPublicTopic(
            Topic entity)
    {
        return new PublicTopicResponse(
            entity.Slug,
            entity.NameVi,
            entity.DescriptionVi);
    }

    public static PublicPartOfSpeechResponse
        ToPublicPartOfSpeech(
            PartOfSpeech entity)
    {
        return new PublicPartOfSpeechResponse(
            entity.Code,
            entity.NameVi,
            entity.NameEn);
    }

    public static PublicHskLevelResponse
        ToPublicHsk(
            HskLevel entity)
    {
        return new PublicHskLevelResponse(
            entity.Id,
            entity.Code,
            entity.NameVi);
    }
}
