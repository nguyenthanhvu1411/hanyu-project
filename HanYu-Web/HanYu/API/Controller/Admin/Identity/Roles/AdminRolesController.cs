using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Identity.Admin.Roles.GetRoleById;
using HanYu.Application.Features.Identity.Admin.Roles.GetRoles;
using HanYu.Application.Features.Identity.Admin.Roles.CreateRole;
using HanYu.Application.Features.Identity.Admin.Roles.UpdateRole;
using HanYu.Application.Features.Identity.Admin.Roles.DeleteRole;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Identity.Roles;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/roles")]
public sealed class AdminRolesController : ControllerBase
{
    private readonly GetRolesHandler _getRolesHandler;
    private readonly GetRoleByIdHandler _getRoleByIdHandler;
    private readonly CreateRoleHandler _createRoleHandler;
    private readonly UpdateRoleHandler _updateRoleHandler;
    private readonly DeleteRoleHandler _deleteRoleHandler;

    public AdminRolesController(
        GetRolesHandler getRolesHandler,
        GetRoleByIdHandler getRoleByIdHandler,
        CreateRoleHandler createRoleHandler,
        UpdateRoleHandler updateRoleHandler,
        DeleteRoleHandler deleteRoleHandler)
    {
        _getRolesHandler = getRolesHandler;
        _getRoleByIdHandler = getRoleByIdHandler;
        _createRoleHandler = createRoleHandler;
        _updateRoleHandler = updateRoleHandler;
        _deleteRoleHandler = deleteRoleHandler;
    }

    /// <summary>
    /// Lấy danh sách vai trò (phân trang, tìm kiếm)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetRolesQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _getRolesHandler.ExecuteAsync(query, cancellationToken));

    /// <summary>
    /// Lấy chi tiết một vai trò theo ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(
        Guid id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _getRoleByIdHandler.ExecuteAsync(id, cancellationToken));

    /// <summary>
    /// Tạo vai trò mới
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateRoleCommand command,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _createRoleHandler.ExecuteAsync(command, cancellationToken));

    /// <summary>
    /// Cập nhật vai trò
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateRoleCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Id != Guid.Empty && id != command.Id)
            return BadRequest(new { Message = "ID trong URL và Body không khớp." });

        if (command.Id == Guid.Empty)
            command = command with { Id = id };

        return this.ToActionResult(
            await _updateRoleHandler.ExecuteAsync(command, cancellationToken));
    }

    /// <summary>
    /// Xóa vai trò
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _deleteRoleHandler.ExecuteAsync(id, cancellationToken));
}
