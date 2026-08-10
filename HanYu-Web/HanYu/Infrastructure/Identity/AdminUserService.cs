using HanYu.Application.Common.Exceptions;
using HanYu.Application.Interfaces.Identity;
using HanYu.Domain.Constants;
using HanYu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Identity;

public sealed class AdminUserService : IAdminUserService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public AdminUserService(
        UserManager<User> userManager,
        RoleManager<Role> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task EnsureCanModifyPrivilegedUserAsync(
        User targetUser,
        Guid currentUserId,
        bool removesSuperAdmin,
        CancellationToken cancellationToken = default)
    {
        var targetIsSuperAdmin = await _userManager.IsInRoleAsync(
            targetUser,
            RoleCodes.SuperAdmin);

        if (!targetIsSuperAdmin)
        {
            return;
        }

        // Không tự khóa/xóa/hạ quyền chính mình
        if (targetUser.Id == currentUserId)
        {
            throw new AppException("Không thể thực hiện thao tác đặc quyền này trên chính tài khoản đang đăng nhập.");
        }

        if (!removesSuperAdmin)
        {
            return;
        }

        var superAdminRole = await _roleManager.FindByNameAsync(RoleCodes.SuperAdmin);
        if (superAdminRole is null)
        {
            throw new InvalidOperationException("SUPER_ADMIN role không tồn tại.");
        }

        var superAdmins = await _userManager.GetUsersInRoleAsync(RoleCodes.SuperAdmin);

        var activeSuperAdminCount = superAdmins.Count(x =>
            !x.IsDeleted &&
            x.Id != targetUser.Id);

        if (activeSuperAdminCount == 0)
        {
            throw new AppException("Không thể khóa, xóa hoặc hạ quyền SuperAdmin cuối cùng của hệ thống.");
        }
    }

    public async Task<int> CountOtherActiveSuperAdminsAsync(
        Guid excludedUserId,
        CancellationToken cancellationToken = default)
    {
        var superAdmins = await _userManager.GetUsersInRoleAsync(RoleCodes.SuperAdmin);

        return superAdmins.Count(user =>
            user.Id != excludedUserId &&
            !user.IsDeleted &&
            !user.LockoutEnd.HasValue);
    }
}
