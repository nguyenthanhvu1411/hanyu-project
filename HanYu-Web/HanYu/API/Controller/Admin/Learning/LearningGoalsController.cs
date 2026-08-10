using HanYu.API.Common.Extensions;
using HanYu.Domain.Constants;
using HanYu.Application.Features.Learning.Admin.Goals;
using HanYu.Application.Interfaces.Learning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Learning;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/learning/goals")]
public sealed class LearningGoalsController : ControllerBase
{
    private readonly ILearningAdminService _learningService;

    public LearningGoalsController(
        ILearningAdminService learningService)
    {
        _learningService = learningService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] AdminLearningGoalQuery query,
        CancellationToken cancellationToken)
    {
        var result =
            await _learningService.GetGoalsAsync(
                query,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest(
                new
                {
                    Code = "Learning.InvalidGoalId",
                    Message = "Learning goal ID không hợp lệ."
                });
        }

        var result =
            await _learningService.GetGoalAsync(
                id,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateLearningGoalRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _learningService.CreateGoalAsync(
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] UpdateLearningGoalRequest request,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest(
                new
                {
                    Code = "Learning.InvalidGoalId",
                    Message = "Learning goal ID không hợp lệ."
                });
        }

        var result =
            await _learningService.UpdateGoalAsync(
                id,
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(
        long id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest(
                new
                {
                    Code = "Learning.InvalidGoalId",
                    Message = "Learning goal ID không hợp lệ."
                });
        }

        var result =
            await _learningService.DeleteGoalAsync(
                id,
                cancellationToken);

        return this.ToActionResult(result);
    }
}
