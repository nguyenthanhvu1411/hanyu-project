using HanYu.Application.Common.Models;
using HanYu.Domain.Enums;

namespace HanYu.Application.Interfaces.Review;

public interface IReviewScheduler
{
    ReviewScheduleResult Calculate(
        decimal masteryBefore,
        int? currentIntervalMinutes,
        ReviewRating rating,
        bool wasCorrect,
        DateTimeOffset reviewedAt);
}
