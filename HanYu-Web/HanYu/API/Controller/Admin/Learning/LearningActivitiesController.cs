using HanYu.API.Common.Extensions;
using HanYu.Domain.Constants;
using HanYu.Application.Features.Learning.Admin.Activities;
using HanYu.Application.Interfaces.Learning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Learning;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/learning/activities")]
public sealed class LearningActivitiesController : ControllerBase
{
    private readonly ILearningAdminService _learningService;

    public LearningActivitiesController(
        ILearningAdminService learningService)
    {
        _learningService = learningService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] AdminLearningActivityQuery query,
        CancellationToken cancellationToken)
    {
        var result =
            await _learningService.GetActivitiesAsync(
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
                    Code = "Learning.InvalidActivityId",
                    Message = "Learning activity ID không hợp lệ."
                });
        }

        var result =
            await _learningService.GetActivityAsync(
                id,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateLearningActivityRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _learningService.CreateActivityAsync(
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] UpdateLearningActivityRequest request,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest(
                new
                {
                    Code = "Learning.InvalidActivityId",
                    Message = "Learning activity ID không hợp lệ."
                });
        }

        var result =
            await _learningService.UpdateActivityAsync(
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
                    Code = "Learning.InvalidActivityId",
                    Message = "Learning activity ID không hợp lệ."
                });
        }

        var result =
            await _learningService.DeleteActivityAsync(
                id,
                cancellationToken);

        return this.ToActionResult(result);
    }
}
