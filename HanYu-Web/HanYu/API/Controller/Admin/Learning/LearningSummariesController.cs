using HanYu.API.Common.Extensions;
using HanYu.Domain.Constants;
using HanYu.Application.Features.Learning.Admin.Summaries;
using HanYu.Application.Interfaces.Learning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Learning;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/learning/summaries")]
public sealed class LearningSummariesController : ControllerBase
{
    private readonly ILearningAdminService _learningService;

    public LearningSummariesController(
        ILearningAdminService learningService)
    {
        _learningService = learningService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] AdminLearningSummaryQuery query,
        CancellationToken cancellationToken)
    {
        var result =
            await _learningService.GetSummariesAsync(
                query,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetByUserId(
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest(
                new
                {
                    Code = "Learning.InvalidUserId",
                    Message = "UserId không hợp lệ."
                });
        }

        var result =
            await _learningService.GetSummaryAsync(
                userId,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> Update(
        Guid userId,
        [FromBody] UpdateLearningSummaryRequest request,
        CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest(
                new
                {
                    Code = "Learning.InvalidUserId",
                    Message = "UserId không hợp lệ."
                });
        }

        var result =
            await _learningService.UpdateSummaryAsync(
                userId,
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("{userId:guid}/recompute")]
    public async Task<IActionResult> Recompute(
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest(
                new
                {
                    Code = "Learning.InvalidUserId",
                    Message = "UserId không hợp lệ."
                });
        }

        var result =
            await _learningService.RecomputeSummaryAsync(
                userId,
                cancellationToken);

        return this.ToActionResult(result);
    }
}
