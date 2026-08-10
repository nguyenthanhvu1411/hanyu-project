using HanYu.Domain.Entities.Identity;

namespace HanYu.Application.Interfaces.Identity;

public interface IAdminUserService
{
    Task EnsureCanModifyPrivilegedUserAsync(
        User targetUser,
        Guid currentUserId,
        bool removesSuperAdmin,
        CancellationToken cancellationToken = default);

    Task<int> CountOtherActiveSuperAdminsAsync(
        Guid excludedUserId,
        CancellationToken cancellationToken = default);
}
