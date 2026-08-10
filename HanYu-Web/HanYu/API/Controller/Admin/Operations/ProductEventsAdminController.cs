using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Operations.Admin.ProductEvents;
using HanYu.Application.Interfaces.Operations;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Operations;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/product-events")]
public sealed class ProductEventsAdminController : ControllerBase
{
    private readonly IOperationsAdminService _service;

    public ProductEventsAdminController(IOperationsAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] AdminProductEventQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetProductEventsAsync(query, cancellationToken));
}
