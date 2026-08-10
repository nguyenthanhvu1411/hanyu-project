using HanYu.Application.Features.Review.Public.Favorites;
using HanYu.Application.Features.Review.Public.Queue;
using HanYu.Application.Features.Review.Public.Review;
using HanYu.Domain.Entities.Review;

namespace HanYu.Application.Features.Review.Mapping;

public static class ReviewMapper
{
    public static ReviewQueueItemResponse ToQueueItem(
        UserVocabularyState state)
    {
        return new ReviewQueueItemResponse(
            state.Vocabulary.PublicId,
            state.Vocabulary.Simplified,
            state.Vocabulary.Traditional,
            state.Vocabulary.Pinyin,
            state.Vocabulary.PrimaryMeaningVi,
            state.Vocabulary.HskLevelId,
            state.LearningState,
            state.MasteryScore,
            state.CorrectCount,
            state.WrongCount,
            state.ConsecutiveCorrect,
            state.LastReviewedAt,
            state.NextReviewAt,
            state.CurrentIntervalMinutes,
            state.IsFavorite);
    }

    public static VocabularyLearningStateResponse
        ToStateResponse(
            UserVocabularyState state)
    {
        return new VocabularyLearningStateResponse(
            state.Vocabulary.PublicId,
            state.LearningState,
            state.IsFavorite,
            state.MasteryScore,
            state.CorrectCount,
            state.WrongCount,
            state.ConsecutiveCorrect,
            state.DistinctCorrectDays,
            state.FirstLearnedAt,
            state.LastReviewedAt,
            state.NextReviewAt,
            state.CurrentIntervalMinutes,
            state.MasteredAt);
    }

    public static FavoriteVocabularyResponse
        ToFavoriteResponse(
            UserVocabularyState state)
    {
        return new FavoriteVocabularyResponse(
            state.Vocabulary.PublicId,
            state.Vocabulary.Simplified,
            state.Vocabulary.Traditional,
            state.Vocabulary.Pinyin,
            state.Vocabulary.PrimaryMeaningVi,
            state.MasteryScore,
            state.NextReviewAt);
    }
}
