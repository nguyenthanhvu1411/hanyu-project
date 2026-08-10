using HanYu.Application.Common.Models;

namespace HanYu.Application.Features.Learning.Admin.Summaries;

public sealed record AdminLearningSummaryQuery(
    Guid? UserId = null,
    short? CurrentHskLevel = null) : PaginationRequest;
