using HanYu.Application.Features.Review.Admin.States;
using HanYu.Domain.Entities.Review;

namespace HanYu.Application.Features.Review.Mapping;

public static class ReviewAdminMapper
{
    public static AdminVocabularyStateResponse ToStateResponse(UserVocabularyState state)
    {
        return new AdminVocabularyStateResponse(
            state.UserId,
            state.VocabularyId,
            state.Vocabulary.PublicId,
            state.Vocabulary.Simplified,
            state.Vocabulary.Traditional,
            state.Vocabulary.Pinyin,
            state.Vocabulary.PrimaryMeaningVi,
            state.Vocabulary.HskLevelId,
            state.LearningState,
            state.IsFavorite,
            state.MasteryScore,
            state.CorrectCount,
            state.WrongCount,
            state.ConsecutiveCorrect,
            state.DistinctCorrectDays,
            state.LastCorrectAt,
            state.LastReviewedAt,
            state.NextReviewAt,
            state.CurrentIntervalMinutes,
            state.FirstLearnedAt,
            state.MasteredAt,
            state.UpdatedAt);
    }

    public static AdminVocabularyStateDetailResponse ToStateDetail(UserVocabularyState state)
    {
        var total = state.CorrectCount + state.WrongCount;
        var accuracy = total == 0 ? 0m : Math.Round(state.CorrectCount * 100m / total, 2);
        var now = DateTimeOffset.UtcNow;

        return new AdminVocabularyStateDetailResponse(
            state.UserId,
            state.VocabularyId,
            state.Vocabulary.PublicId,
            state.Vocabulary.Simplified,
            state.Vocabulary.Traditional,
            state.Vocabulary.Pinyin,
            state.Vocabulary.PinyinNormalized,
            state.Vocabulary.PrimaryMeaningVi,
            state.Vocabulary.HskLevelId,
            state.LearningState,
            state.IsFavorite,
            state.MasteryScore,
            state.CorrectCount,
            state.WrongCount,
            total,
            state.ConsecutiveCorrect,
            state.DistinctCorrectDays,
            accuracy,
            state.FirstLearnedAt,
            state.LastCorrectAt,
            state.LastReviewedAt,
            state.NextReviewAt,
            state.MasteredAt,
            state.CurrentIntervalMinutes,
            state.NextReviewAt.HasValue && state.NextReviewAt.Value <= now,
            state.NextReviewAt.HasValue && state.NextReviewAt.Value < now.AddHours(-24),
            state.UpdatedAt);
    }
}
