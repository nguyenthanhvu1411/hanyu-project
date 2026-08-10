namespace HanYu.Domain.Entities.Identity;

public class RolePermission
{
    public Guid RoleId { get; private set; }

    public long PermissionId { get; private set; }

    public Role Role { get; private set; }
        = null!;

    public Permission Permission { get; private set; }
        = null!;

    protected RolePermission()
    {
    }

    public RolePermission(
        Guid roleId,
        long permissionId)
    {
        if (roleId == Guid.Empty)
        {
            throw new ArgumentException(
                "RoleId không hợp lệ.",
                nameof(roleId));
        }

        if (permissionId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(permissionId));
        }

        RoleId =
            roleId;

        PermissionId =
            permissionId;
    }
}
