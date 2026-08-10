using HanYu.Application.Common.Models;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Learning.Public.Activities;

public sealed record LearningActivityQuery(
    LearningActivityType? ActivityType = null,
    bool? IsCompleted = null,
    DateTimeOffset? From = null,
    DateTimeOffset? To = null) : PaginationRequest;
