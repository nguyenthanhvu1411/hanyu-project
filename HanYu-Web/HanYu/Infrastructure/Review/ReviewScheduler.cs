using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Review;
using HanYu.Domain.Constants;
using HanYu.Domain.Enums;

namespace HanYu.Infrastructure.Review;

public sealed class ReviewScheduler
    : IReviewScheduler
{
    public ReviewScheduleResult Calculate(
        decimal masteryBefore,
        int? currentIntervalMinutes,
        ReviewRating rating,
        bool wasCorrect,
        DateTimeOffset reviewedAt)
    {
        if (masteryBefore < 0 ||
            masteryBefore > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(masteryBefore));
        }

        if (currentIntervalMinutes.HasValue &&
            currentIntervalMinutes.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(currentIntervalMinutes));
        }

        decimal masteryAfter;
        int nextInterval;

        /*
         * WasCorrect có ưu tiên cao hơn Rating.
         *
         * Nếu answer sai thì luôn đưa về interval ngắn,
         * tránh trường hợp client gửi Rating=Easy nhưng
         * WasCorrect=false.
         */
        if (!wasCorrect)
        {
            masteryAfter =
                masteryBefore +
                ReviewConstants.AgainMasteryDelta;

            nextInterval =
                ReviewConstants.AgainIntervalMinutes;
        }
        else
        {
            switch (rating)
            {
                case ReviewRating.Again:
                    masteryAfter =
                        masteryBefore +
                        ReviewConstants.AgainMasteryDelta;

                    nextInterval =
                        ReviewConstants.AgainIntervalMinutes;

                    break;

                case ReviewRating.Hard:
                    masteryAfter =
                        masteryBefore +
                        ReviewConstants.HardMasteryDelta;

                    nextInterval =
                        currentIntervalMinutes.HasValue
                            ? MultiplyInterval(
                                currentIntervalMinutes.Value,
                                1.5m)
                            : ReviewConstants
                                .HardFirstIntervalMinutes;

                    break;

                case ReviewRating.Good:
                    masteryAfter =
                        masteryBefore +
                        ReviewConstants.GoodMasteryDelta;

                    nextInterval =
                        currentIntervalMinutes.HasValue
                            ? MultiplyInterval(
                                currentIntervalMinutes.Value,
                                2.5m)
                            : ReviewConstants
                                .GoodFirstIntervalMinutes;

                    break;

                case ReviewRating.Easy:
                    masteryAfter =
                        masteryBefore +
                        ReviewConstants.EasyMasteryDelta;

                    nextInterval =
                        currentIntervalMinutes.HasValue
                            ? MultiplyInterval(
                                currentIntervalMinutes.Value,
                                4m)
                            : ReviewConstants
                                .EasyFirstIntervalMinutes;

                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(rating));
            }
        }

        masteryAfter =
            Math.Clamp(
                masteryAfter,
                0m,
                100m);

        nextInterval =
            Math.Clamp(
                nextInterval,
                1,
                ReviewConstants.MaxIntervalMinutes);

        return new ReviewScheduleResult(
            masteryBefore,
            masteryAfter,
            currentIntervalMinutes,
            nextInterval,
            reviewedAt,
            reviewedAt.AddMinutes(
                nextInterval));
    }

    private static int MultiplyInterval(
        int currentInterval,
        decimal multiplier)
    {
        var result =
            decimal.Round(
                currentInterval * multiplier,
                0,
                MidpointRounding.AwayFromZero);

        if (result >
            ReviewConstants.MaxIntervalMinutes)
        {
            return ReviewConstants.MaxIntervalMinutes;
        }

        return Math.Max(
            1,
            (int)result);
    }
}
