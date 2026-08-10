using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Identity.Admin.Users;
using HanYu.Application.Features.Identity.Admin.Users.GetUserById;
using HanYu.Application.Features.Identity.Admin.Users.GetUsers;
using HanYu.Application.Features.Identity.Admin.Users.LockUser;
using HanYu.Application.Features.Identity.Admin.Users.UnlockUser;
using HanYu.Application.Features.Identity.Admin.Users.DeleteUser;
using HanYu.Application.Features.Identity.Admin.Users.RestoreUser;
using HanYu.Application.Features.Identity.Admin.Users.UpdateUser;
using HanYu.Application.Features.Identity.Admin.Users.UpdateRoles;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Identity;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/users")]
public sealed class AdminUsersController : ControllerBase
{
    private readonly GetUsersHandler _getUsersHandler;
    private readonly GetUserByIdHandler _getUserByIdHandler;
    private readonly LockUserHandler _lockUserHandler;
    private readonly UnlockUserHandler _unlockUserHandler;
    private readonly UpdateUserHandler _updateUserHandler;
    private readonly DeleteUserHandler _deleteUserHandler;
    private readonly RestoreUserHandler _restoreUserHandler;
    private readonly UpdateUserRolesHandler _updateUserRolesHandler;

    public AdminUsersController(
        GetUsersHandler getUsersHandler,
        GetUserByIdHandler getUserByIdHandler,
        LockUserHandler lockUserHandler,
        UnlockUserHandler unlockUserHandler,
        UpdateUserHandler updateUserHandler,
        DeleteUserHandler deleteUserHandler,
        RestoreUserHandler restoreUserHandler,
        UpdateUserRolesHandler updateUserRolesHandler)
    {
        _getUsersHandler = getUsersHandler;
        _getUserByIdHandler = getUserByIdHandler;
        _lockUserHandler = lockUserHandler;
        _unlockUserHandler = unlockUserHandler;
        _updateUserHandler = updateUserHandler;
        _deleteUserHandler = deleteUserHandler;
        _restoreUserHandler = restoreUserHandler;
        _updateUserRolesHandler = updateUserRolesHandler;
    }

    /// <summary>
    /// Lấy danh sách người dùng (phân trang, lọc, tìm kiếm)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetUsersQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _getUsersHandler.ExecuteAsync(query, cancellationToken));

    /// <summary>
    /// Lấy chi tiết một người dùng theo ID (Guid)
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(
        Guid id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _getUserByIdHandler.ExecuteAsync(id, cancellationToken));

    /// <summary>
    /// Khóa người dùng
    /// </summary>
    [HttpPost("{id:guid}/lock")]
    public async Task<IActionResult> LockUser(
        Guid id,
        [FromBody] LockUserRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _lockUserHandler.ExecuteAsync(id, request.Reason, cancellationToken));

    /// <summary>
    /// Mở khóa người dùng
    /// </summary>
    [HttpPost("{id:guid}/unlock")]
    public async Task<IActionResult> UnlockUser(
        Guid id,
        [FromBody] UnlockUserRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _unlockUserHandler.ExecuteAsync(id, request.Reason, cancellationToken));

    /// <summary>
    /// Cập nhật thông tin người dùng
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _updateUserHandler.ExecuteAsync(
                new UpdateUserCommand(id, request.Email, request.DisplayName, request.Locale, request.Status, request.ConcurrencyToken), 
                cancellationToken));

    /// <summary>
    /// Xóa người dùng (Soft Delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(
        Guid id,
        [FromBody] DeleteUserRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _deleteUserHandler.ExecuteAsync(id, request.Reason, cancellationToken));

    /// <summary>
    /// Khôi phục người dùng (Restore Soft Delete)
    /// </summary>
    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> RestoreUser(
        Guid id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _restoreUserHandler.ExecuteAsync(id, cancellationToken));

    /// <summary>
    /// Cập nhật vai trò người dùng
    /// </summary>
    [HttpPut("{id:guid}/roles")]
    public async Task<IActionResult> UpdateUserRoles(
        Guid id,
        [FromBody] UpdateRolesRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _updateUserRolesHandler.ExecuteAsync(
                new UpdateUserRolesCommand
                {
                    UserId = id,
                    RoleCodes = request.RoleCodes,
                    Reason = request.Reason
                }, 
                cancellationToken));
}

public sealed record LockUserRequest(string Reason);
public sealed record UnlockUserRequest(string Reason);
public sealed record UpdateUserRequest(string Email, string DisplayName, string? Locale, string Status, string? ConcurrencyToken);
public sealed record DeleteUserRequest(string Reason);
public sealed record UpdateRolesRequest(List<string> RoleCodes, string Reason);
