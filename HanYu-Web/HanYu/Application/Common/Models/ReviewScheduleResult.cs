namespace HanYu.Application.Common.Models;

public sealed record ReviewScheduleResult(
    decimal MasteryBefore,
    decimal MasteryAfter,
    int? IntervalBeforeMinutes,
    int IntervalAfterMinutes,
    DateTimeOffset ReviewedAt,
    DateTimeOffset NextReviewAt);
