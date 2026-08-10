using HanYu.Application.Common.Models;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Learning.Admin.Goals;

public sealed record AdminLearningGoalQuery(
    Guid? UserId = null,
    LearningGoalStatus? Status = null,
    short? TargetHskLevel = null) : PaginationRequest;
