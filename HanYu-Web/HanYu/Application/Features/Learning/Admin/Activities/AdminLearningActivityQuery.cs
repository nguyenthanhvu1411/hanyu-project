using HanYu.Application.Common.Models;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Learning.Admin.Activities;

public sealed record AdminLearningActivityQuery(
    Guid? UserId = null,
    LearningActivityType? ActivityType = null,
    bool? IsCompleted = null,
    DateTimeOffset? From = null,
    DateTimeOffset? To = null) : PaginationRequest;
